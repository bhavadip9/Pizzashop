PGDMP      ;                }            New_pizzashop    16.3    16.3    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    49892    New_pizzashop    DATABASE     �   CREATE DATABASE "New_pizzashop" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_India.1252';
    DROP DATABASE "New_pizzashop";
                postgres    false            �           1247    50598    tax_item    TYPE     P   CREATE TYPE public.tax_item AS (
	taxid integer,
	taxamount double precision
);
    DROP TYPE public.tax_item;
       public          postgres    false                       1255    50533 ~   add_waiting_token_code(integer, character varying, character varying, character varying, integer, timestamp without time zone) 	   PROCEDURE     �  CREATE PROCEDURE public.add_waiting_token_code(IN p_total_person integer, IN p_user_name character varying, IN p_phone_no character varying, IN p_email character varying, IN p_section_id integer, IN p_created_at timestamp without time zone)
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO waiting_token_codes (
        total_person, user_name, "Phone_No", "Email", "SectionId", created_at
    ) VALUES (
        p_total_person, p_user_name, p_phone_no, p_email, p_section_id, p_created_at
    );
END;
$$;
 �   DROP PROCEDURE public.add_waiting_token_code(IN p_total_person integer, IN p_user_name character varying, IN p_phone_no character varying, IN p_email character varying, IN p_section_id integer, IN p_created_at timestamp without time zone);
       public          postgres    false            '           1255    50605 $   addorderitem(integer, numeric, json)    FUNCTION     �  CREATE FUNCTION public.addorderitem(p_order_id integer, p_total_amount numeric, p_model json) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_order_status TEXT;
    v_table RECORD;
    v_existing_order_details RECORD;
    v_existing_order_detail_ids INT[] := ARRAY[]::INT[];
    v_input_order_detail_ids INT[] := ARRAY[]::INT[];
    v_json_item JSON;
    v_json_modifier JSON;
    v_order_detail_id INT;
    v_item_id INT;
    v_quantity INT;
    v_modifier_id INT;
    v_found BOOLEAN;
BEGIN
    -- 1. Check order status
    SELECT "OrderStatus" INTO v_order_status FROM "order" WHERE order_id = p_order_id;
    IF v_order_status IS NULL THEN
        RAISE EXCEPTION 'Order % not found', p_order_id;
    END IF;
    IF v_order_status IN ('Complete', 'Cancelled') THEN
        RETURN -1;
    END IF;

    -- 2. Update order status and amount
    UPDATE "order"
    SET "OrderStatus" = 'Running',
        "Amount" = p_total_amount
    WHERE order_id = p_order_id;

    -- 3. Update related tables status + modified_at
    FOR v_table IN
        SELECT t.* FROM tables t
        JOIN order_table_mapping otm ON t.table_id = otm.table_id
        WHERE otm.order_id = p_order_id
    LOOP
        UPDATE tables SET status = 'Running', modified_at = NOW() WHERE table_id = v_table.table_id;
    END LOOP;

    -- 4. Get all existing (non-deleted) order_detail IDs for this order
    SELECT COALESCE(ARRAY_AGG("OrderDetailID"), ARRAY[]::INT[]) INTO v_existing_order_detail_ids
    FROM "OrderDetail"
    WHERE order_id = p_order_id AND ("isDeleted" IS NULL OR "isDeleted" = FALSE);

    -- 5. Process each input item:
    FOR v_json_item IN SELECT * FROM json_array_elements(p_model)
    LOOP
        -- Extract data
        v_order_detail_id := COALESCE((v_json_item->>'Id')::INT, 0);
        v_item_id := (v_json_item->>'ItemId')::INT;
        v_quantity := (v_json_item->>'Quantity')::INT;

        IF v_order_detail_id <> 0 THEN
            -- Existing order detail - update quantity and mark not deleted
            SELECT EXISTS (
                SELECT 1 FROM "OrderDetail" WHERE "OrderDetailID" = v_order_detail_id AND order_id = p_order_id
            ) INTO v_found;

            IF v_found THEN
                UPDATE "OrderDetail"
                SET "Quntity" = v_quantity,
                    "isDeleted" = FALSE
                WHERE "OrderDetailID" = v_order_detail_id AND order_id = p_order_id;

                -- Delete existing modifiers for this order_detail_id (to replace with new ones)
                DELETE FROM ordered_item_modifier WHERE "orderitemid" = v_order_detail_id;

                -- Insert new modifiers for this order_detail_id
                FOR v_json_modifier IN SELECT * FROM json_array_elements(v_json_item->'Modifiers')
                LOOP
                    v_modifier_id := (v_json_modifier->>'Id')::INT;
                    INSERT INTO ordered_item_modifier("orderitemid", modifier_id)
                    VALUES (v_order_detail_id, v_modifier_id);
                END LOOP;

                -- Keep track of processed order_detail_ids
                v_input_order_detail_ids := array_append(v_input_order_detail_ids, v_order_detail_id);

            ELSE
                -- order_detail_id not found, treat as new item
                v_order_detail_id := 0;
            END IF;
        END IF;

        IF v_order_detail_id = 0 THEN
            -- New order detail: insert order detail
            v_item_id := (v_json_item->>'Id')::INT;
            INSERT INTO "OrderDetail"(order_id, "ItemId", "Quntity", "Prepared", "isDeleted")
            VALUES (p_order_id, v_item_id, v_quantity, 0, FALSE)
            RETURNING "OrderDetailID" INTO v_order_detail_id;

            -- Insert modifiers
            FOR v_json_modifier IN SELECT * FROM json_array_elements(v_json_item->'Modifiers')
            LOOP
                v_modifier_id := (v_json_modifier->>'Id')::INT;
                INSERT INTO ordered_item_modifier("orderitemid", modifier_id)
                VALUES (v_order_detail_id, v_modifier_id);
            END LOOP;

            -- Track the new id
            v_input_order_detail_ids := array_append(v_input_order_detail_ids, v_order_detail_id);
        END IF;
    END LOOP;

    -- 6. Mark any old order_details not in input list as deleted
    IF array_length(v_existing_order_detail_ids, 1) IS NOT NULL THEN
        UPDATE "OrderDetail"
        SET "isDeleted" = TRUE
        WHERE order_id = p_order_id
          AND "OrderDetailID" = ANY(v_existing_order_detail_ids)
          AND "OrderDetailID" NOT IN (SELECT unnest(v_input_order_detail_ids));
    END IF;

    RETURN 0;

-- EXCEPTION WHEN OTHERS THEN
--     RAISE NOTICE 'Exception: %', SQLERRM;
--     RETURN 1;
END;
$$;
 ]   DROP FUNCTION public.addorderitem(p_order_id integer, p_total_amount numeric, p_model json);
       public          postgres    false            $           1255    50574 l   assign_table1(integer, integer, character varying, character varying, integer, character varying, integer[])    FUNCTION     �  CREATE FUNCTION public.assign_table1(p_waiting_user_id integer, p_section_id integer, p_user_name character varying, p_email character varying, p_total_person integer, p_phone character varying, p_selected_tablelist integer[]) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    waiting_user RECORD;
    table_capacity INT := 0;
    tbl_record RECORD;
    total_person INT;
    customer_record RECORD;
    new_customer_id INT;
    new_order_id INT;
    new_payment_id INT;
    tbl_id INT;
    v_context TEXT;
BEGIN
    -- 1. Fetch Waiting User Details.
    SELECT * INTO waiting_user 
    FROM waiting_token_codes 
    WHERE token_id = p_waiting_user_id;
    -- IF NOT FOUND THEN
    --     RAISE NOTICE 'Waiting user not found: %', p_waiting_user_id;
    --     RETURN 0;
    -- END IF;
    -- total_person := waiting_user.total_person;

    -- 2. Check Table Availability & Capacity
    IF p_selected_tablelist IS NOT NULL AND array_length(p_selected_tablelist, 1) > 0 THEN
        FOR tbl_record IN
            SELECT * FROM tables WHERE table_id = ANY(p_selected_tablelist)
        LOOP
            IF tbl_record.status <> 'Available' THEN
                RAISE NOTICE 'Table % is not available', tbl_record.table_id;
                RETURN 0;
            END IF;
            table_capacity := table_capacity + COALESCE(tbl_record.capacity, 0);
        END LOOP;
        IF table_capacity < total_person THEN
            RAISE NOTICE 'Selected tables (cap=%), but need %', table_capacity, total_person;
            RETURN 0;
        END IF;
    END IF;

    -- 3. Update Section if changed.
    IF waiting_user."SectionId" IS DISTINCT FROM p_section_id AND p_section_id IS NOT NULL THEN
        UPDATE "waiting_token_codes"
        SET "SectionId" = p_section_id
        WHERE token_id = p_waiting_user_id;
    END IF;

    -- 4. Find or Add/Update Customer.
    SELECT * INTO customer_record 
    FROM customers 
    WHERE lower(email) = lower(p_email)
    LIMIT 1;

    IF FOUND THEN
        -- update info (optional: can skip if unchanged)
        UPDATE customers
        SET name = p_user_name,
            phone = p_phone,
            email = p_email
        WHERE customer_id = customer_record.customer_id;
        new_customer_id := customer_record.customer_id;
    ELSE
        INSERT INTO customers (name, phone, email)
        VALUES (p_user_name, p_phone, p_email)
        RETURNING customer_id INTO new_customer_id;
    END IF;

    -- 5. Insert Order.
    INSERT INTO "order" (customer_id, "OrderStatus", "Total_Person", "Amount")
    VALUES (new_customer_id, 'Pending', p_total_person, 0)
    RETURNING order_id INTO new_order_id;

    -- 6. Insert Payment.
    INSERT INTO payment (order_id, payment_method)
    VALUES (new_order_id, '')
    RETURNING payment_id INTO new_payment_id;

    -- 7. Map Tables and Set Occupied.
    IF p_selected_tablelist IS NOT NULL AND array_length(p_selected_tablelist, 1) > 0 THEN
        FOREACH tbl_id IN ARRAY p_selected_tablelist LOOP
            INSERT INTO order_table_mapping (table_id, order_id)
            VALUES (tbl_id, new_order_id);

            UPDATE tables 
            SET status = 'Occupied', 
                modified_at = CURRENT_TIMESTAMP
            WHERE table_id = tbl_id;
        END LOOP;
    END IF;

    -- 8. Mark Waiting User as Deleted.

    IF p_waiting_user_id IS NOT NULL AND p_waiting_user_id <> 0 THEN
        UPDATE "waiting_token_codes" SET "isDelete" = true WHERE token_id = p_waiting_user_id;
    END IF;
 -- IF p_waiting_user_id IS NOT NULL AND p_waiting_user_id <> 0 THEN
 --         UPDATE "waiting_token_codes" SET "isDelete" = true WHERE token_id = p_waiting_user_id;
 --    END IF;
   -- UPDATE "waiting_token_codes" SET "isDelete" = true WHERE token_id = p_waiting_user_id;

    RETURN new_order_id;

EXCEPTION
    WHEN OTHERS THEN
        GET STACKED DIAGNOSTICS v_context = PG_EXCEPTION_CONTEXT;
        RAISE NOTICE 'Error: %, Context: %', SQLERRM, v_context;
END;
	
   
$$;
 �   DROP FUNCTION public.assign_table1(p_waiting_user_id integer, p_section_id integer, p_user_name character varying, p_email character varying, p_total_person integer, p_phone character varying, p_selected_tablelist integer[]);
       public          postgres    false                       1255    50595    confirmation_cancel(integer)    FUNCTION     �  CREATE FUNCTION public.confirmation_cancel(p_order_id integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_prepared_cnt INTEGER;
    v_table_id INTEGER;
    v_order_detail_id INTEGER;
BEGIN
    -- 1. Check if any order detail is prepared > 0
    SELECT COUNT(1) INTO v_prepared_cnt 
    FROM "OrderDetail"
    WHERE order_id = p_order_id AND "Prepared" > 0 AND "isDeleted" = false;

    IF v_prepared_cnt > 0 THEN
        RETURN 1;
    END IF;

    -- 2. Mark order details as deleted
    UPDATE "OrderDetail"
    SET "isDeleted" = true
    WHERE order_id = p_order_id;

    -- 3. Get relevant tables and update their status to 'Available'
    FOR v_table_id IN 
        SELECT table_id FROM order_table_mapping WHERE order_id = p_order_id AND "IsDelete" = false
    LOOP
        UPDATE tables
        SET status = 'Available'
        WHERE table_id = v_table_id;

        -- 4. Mark table mapping as deleted
        UPDATE order_table_mapping
        SET "IsDelete" = true
        WHERE table_id = v_table_id AND order_id = p_order_id;
    END LOOP;
  
    -- 5. Update order status and reset amounts
    UPDATE "order"
    SET "OrderStatus" = 'Cancelled',
        "Amount" = 0,
        "SubTotal" = 0
    WHERE order_id = p_order_id;

    RETURN 0;
END;
$$;
 >   DROP FUNCTION public.confirmation_cancel(p_order_id integer);
       public          postgres    false            &           1255    50602 h   confirmation_order_status(integer, integer, double precision, double precision, double precision, jsonb)    FUNCTION     �  CREATE FUNCTION public.confirmation_order_status(p_orderid integer, p_customerid integer, p_totalbill double precision, p_subtotal double precision, p_othertax double precision, p_selected_taxes jsonb) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_tableid INT;
    v_exists BOOLEAN;
    v_tax JSONB;
    v_taxid INT;
    v_taxamount NUMERIC;
  
BEGIN
    -- 1. Update order status and totals
    UPDATE "order"
    SET "OrderStatus" = 'Complete',
        "Amount" = p_totalbill,
        "SubTotal" = p_subtotal,
        customer_id = p_customerid,      -- add if you store customer info
        "OtherTax" = p_othertax
    WHERE order_id = p_orderid;

    IF NOT FOUND THEN
        RETURN FALSE;
    END IF;

    -- 2. For each table mapping, update mapping and set table Available
    FOR v_tableid IN
        SELECT table_id FROM order_table_mapping WHERE order_id = p_orderid
    LOOP
        UPDATE order_table_mapping
           SET "IsDelete" = TRUE
         WHERE order_id = p_orderid AND table_id = v_tableid;

        UPDATE tables
           SET status = 'Available'
         WHERE table_id = v_tableid;
    END LOOP;

    -- 3. Update or insert tax mappings from p_selected_taxes
 IF p_selected_taxes IS NOT NULL THEN
        -- Loop through each element in the JSON array
        FOR v_tax IN SELECT * FROM jsonb_array_elements(p_selected_taxes)
        LOOP
            -- Extract tax_id and tax_amount from JSON
            v_taxid := (v_tax ->> 'taxid')::INT;
            v_taxamount := (v_tax ->> 'tax_amount')::NUMERIC;

            SELECT EXISTS(
                SELECT 1 FROM order_tax_mapping
                 WHERE order_id = p_orderid AND tax_id = v_taxid
            ) INTO v_exists;

            IF v_exists THEN
                UPDATE order_tax_mapping
                   SET tax_amount = v_taxamount
                 WHERE order_id = p_orderid AND tax_id = v_taxid;
            ELSE
                INSERT INTO order_tax_mapping(order_id, tax_id, tax_amount)
                VALUES (p_orderid, v_taxid, v_taxamount);
            END IF;
        END LOOP;
    END IF;
    -- IF p_selected_taxes IS NOT NULL THEN
    --     FOREACH v_tax IN ARRAY p_selected_taxes
    --     LOOP
    --         SELECT EXISTS(
    --             SELECT 1 FROM order_tax_mapping
    --             WHERE order_id = p_orderid AND tax_id = v_tax.taxid
    --         ) INTO v_exists;

    --         IF v_exists THEN
    --             UPDATE order_tax_mapping
    --                SET tax_amount = v_tax.taxamount
    --              WHERE order_id = p_orderid AND tax_id = v_tax.taxid;
    --         ELSE
    --             INSERT INTO order_tax_mapping(order_id, tax_id, tax_amount)
    --             VALUES (p_orderid, v_tax.taxid, v_tax.taxamount);
    --         END IF;
    --     END LOOP;
    -- END IF;

    RETURN TRUE;
-- EXCEPTION WHEN OTHERS THEN
--     -- On any error: return false
--     RETURN FALSE;
END;
$$;
 �   DROP FUNCTION public.confirmation_order_status(p_orderid integer, p_customerid integer, p_totalbill double precision, p_subtotal double precision, p_othertax double precision, p_selected_taxes jsonb);
       public          postgres    false                       1255    50534    deletewaitingtoken(integer) 	   PROCEDURE     �   CREATE PROCEDURE public.deletewaitingtoken(IN token_id integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE waiting_token_codes
    SET "isDelete" = TRUE
    WHERE waiting_token_codes.token_id = DeleteWaitingToken.token_id;
END;
$$;
 ?   DROP PROCEDURE public.deletewaitingtoken(IN token_id integer);
       public          postgres    false                       1255    50542 X   edit_waiting_user(integer, integer, text, text, text, integer, timestamp with time zone)    FUNCTION       CREATE FUNCTION public.edit_waiting_user(p_id integer, p_no_of_person integer, p_user_name text, p_phone text, p_email text, p_section_id integer, p_modified_at timestamp with time zone) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE "waiting_token_codes"
    SET
        total_person = p_no_of_person,
        user_name = p_user_name,
        "Phone_No" = p_phone,
        "Email" = p_email,
        "SectionId" = p_section_id,
        modified_at = p_modified_at
    WHERE token_id = p_id;
END;
$$;
 �   DROP FUNCTION public.edit_waiting_user(p_id integer, p_no_of_person integer, p_user_name text, p_phone text, p_email text, p_section_id integer, p_modified_at timestamp with time zone);
       public          postgres    false            �            1259    49959 	   menu_item    TABLE     �  CREATE TABLE public.menu_item (
    item_id integer NOT NULL,
    category_id integer NOT NULL,
    item_name character varying NOT NULL,
    description text,
    price real NOT NULL,
    is_deleted boolean DEFAULT true NOT NULL,
    is_available boolean DEFAULT true NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    "IsDefault" boolean DEFAULT false NOT NULL,
    "Tax" real NOT NULL,
    "Quantity" integer NOT NULL,
    shortcode character varying NOT NULL,
    "Image" character varying,
    unit_id integer NOT NULL,
    food_type character varying DEFAULT 'Vegetarian'::character varying NOT NULL,
    is_fav boolean DEFAULT false NOT NULL,
    CONSTRAINT check_food_type CHECK (((food_type)::text = ANY (ARRAY[('Vegetarian'::character varying)::text, ('Non-Vegetarian'::character varying)::text, ('Vegan'::character varying)::text])))
);
    DROP TABLE public.menu_item;
       public         heap    postgres    false            !           1255    50606    get_all_menu_items()    FUNCTION     �   CREATE FUNCTION public.get_all_menu_items() RETURNS SETOF public.menu_item
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM menu_item
    WHERE NOT is_deleted
      AND is_available;
END;
$$;
 +   DROP FUNCTION public.get_all_menu_items();
       public          postgres    false    232            "           1255    50526    get_all_orders()    FUNCTION       CREATE FUNCTION public.get_all_orders() RETURNS TABLE(order_id integer, order_status character varying, createdat timestamp without time zone, order_comment character varying, orderdetail_id integer, orderitem_isdelete boolean, item_comment character varying, prepared integer, quantity integer, item_id integer, item_name character varying, categoryid integer, category_name character varying, ordered_item_modifier_id integer, table_id integer, table_name character varying, table_modifiedat timestamp without time zone, section_id integer, section_name character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
RETURN QUERY
    SELECT 
         o.order_id AS "OrderId",
	o."OrderStatus" As OrderStatus,
	o."created_at" as "CreatedAt",
	o."OrderComment" as "OrderComment",
    od."OrderDetailID" AS "OrderDetailId",
	od."isDeleted" as "IsDeletedItem",
	od."Item_Comment" as "ItemComment",
	od."Prepared" as "Prepared",
	od."Quntity" as "Quntity",
    i.item_id AS "ItemId",
    i.item_name AS "ItemName",
    c.category_id AS "CategoryId",
    c.category_name AS "CategoryName",
    oim.ordered_item_modifier_id AS "OrderedItemModifierId",
    t.table_id AS "TableId",
    t.table_name AS "TableName",
	t.modified_at As "TableModifiedAt",
    s.section_id AS "SectionId",
    s.section_name AS "SectionName"
    FROM "order" o
    LEFT JOIN "OrderDetail" od ON od.order_id = o.order_id
    LEFT JOIN menu_item i ON od."ItemId" = i.item_id 
    LEFT JOIN category c ON i.category_id = c.category_id
    LEFT JOIN ordered_item_modifier oim ON oim.orderitemid = od."OrderDetailID"
    LEFT JOIN order_table_mapping otm ON otm.order_id = o.order_id
    LEFT JOIN tables t ON otm.table_id = t.table_id
    LEFT JOIN section s ON t.section_id = s.section_id
    WHERE o."isDelete" = false;
END;
$$;
 '   DROP FUNCTION public.get_all_orders();
       public          postgres    false            #           1255    50587 )   get_all_sections_with_tables_and_orders()    FUNCTION     �  CREATE FUNCTION public.get_all_sections_with_tables_and_orders() RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
DECLARE
    result jsonb;
BEGIN
    SELECT jsonb_agg(
        jsonb_build_object(
            'SectionId', s.section_id,
            'SectionName', s.section_name,
            'Tables', (
                SELECT jsonb_agg(
                    jsonb_build_object(
                        'TableId', t.table_id,
                        'TableName', t.table_name,
                        'Status', t.status,
                        'Capacity', t.capacity,
                        'Time', t.modified_at,
                        'ModifiedAt', t.modified_at,
                        'OrderId', otm.order_id,
                        'OrderTableMappings', 
                            COALESCE((
                                SELECT jsonb_agg(
                                    jsonb_build_object(
                                        'OrderDetailId', otm2."order_detail_id",
	                                    'TableId', otm2."table_id",
	                                    'OrderId', otm2.order_id,
                                        'Order', jsonb_build_object(
                                            'OrderId', o2.order_id,
                                            'OrderStatus', o2."OrderStatus",
                                            'Amount', o2."Amount",
                                            'CreatedAt', o2.created_at
                                        )
                                    )
                                )
                                FROM order_table_mapping otm2
                                JOIN "order" o2 ON otm2.order_id = o2.order_id AND otm2."IsDelete" = false 
                                WHERE otm2.table_id = t.table_id 
                            ), '[]'::jsonb)
                    )
                )
                FROM tables t
                LEFT JOIN order_table_mapping otm ON t.table_id = otm.table_id AND otm."IsDelete" = false
                LEFT JOIN "order" o ON otm.order_id = o.order_id
                WHERE t.section_id = s.section_id AND t."isDelete" = false
            )
        )
    ) INTO result
    FROM section s
    WHERE s."IsDeleted" = false;

    RETURN result;
END;
$$;
 @   DROP FUNCTION public.get_all_sections_with_tables_and_orders();
       public          postgres    false                        1255    50554    get_all_waiting_users()    FUNCTION     {  CREATE FUNCTION public.get_all_waiting_users() RETURNS TABLE(token_id integer, total_person integer, user_name character varying, phone_no character varying, email character varying, sectionid integer, created_at timestamp without time zone, modified_at timestamp without time zone, created_by integer, modified_by integer, isdelete boolean)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
        w."token_id", w."total_person",w."user_name", "Phone_No", "Email", "SectionId", w."created_at",w.modified_at,w.created_by,w.modified_by,w."isDelete"
    FROM "waiting_token_codes" as w
    WHERE NOT "isDelete";
END;
$$;
 .   DROP FUNCTION public.get_all_waiting_users();
       public          postgres    false                       1255    50594    get_order_detail_list(integer)    FUNCTION       CREATE FUNCTION public.get_order_detail_list(p_order_id integer) RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
DECLARE
    result jsonb;
BEGIN
    SELECT COALESCE(
            jsonb_agg(
                jsonb_build_object(
	                 'OrderDetailId', od."OrderDetailID", 
                    'Quntity', od."Quntity",
                    'Prepared', od."Prepared",
	                'Item_Comment', od."Item_Comment",
	                 'OrderId', p_order_id,
	                'ItemId', od."ItemId",
                     'isDeleted', od."isDeleted"
                )
            ),
            '[]'::jsonb  -- return empty array if no results
        )
    INTO result
    FROM "OrderDetail" od
    WHERE od."isDeleted" = false
      AND od."order_id" = p_order_id;

    RETURN result;
END;
$$;
 @   DROP FUNCTION public.get_order_detail_list(p_order_id integer);
       public          postgres    false            %           1255    50549    get_order_details(integer)    FUNCTION     �  CREATE FUNCTION public.get_order_details(in_order_id integer) RETURNS TABLE(order_id integer, order_status character varying, createdat timestamp without time zone, order_comment character varying, orderdetail_id integer, orderitem_isdelete boolean, item_comment character varying, prepared integer, quantity integer, item_id integer, item_name character varying, categoryid integer, category_name character varying, ordered_item_modifier_id integer, table_id integer, table_name character varying, table_modifiedat timestamp without time zone, section_id integer, section_name character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
RETURN QUERY
    SELECT 
        o.order_id AS order_id,
        o."OrderStatus" AS order_status,
        o."created_at" AS createdat,
        o."OrderComment" AS order_comment,
        od."OrderDetailID" AS orderdetail_id,
        od."isDeleted" AS orderitem_isdelete,
        od."Item_Comment" AS item_comment,
        od."Prepared" AS prepared,
        od."Quntity" AS quantity,
        i.item_id AS item_id,
        i.item_name AS item_name,
        c.category_id AS categoryid,
        c.category_name AS category_name,
        oim.ordered_item_modifier_id AS ordered_item_modifier_id,
        t.table_id AS table_id,
        t.table_name AS table_name,
        t.modified_at AS table_modifiedat,
        s.section_id AS section_id,
        s.section_name AS section_name
    FROM "order" o
    LEFT JOIN "OrderDetail" od ON od.order_id = o.order_id
    LEFT JOIN menu_item i ON od."ItemId" = i.item_id
    LEFT JOIN category c ON i.category_id = c.category_id
    LEFT JOIN ordered_item_modifier oim ON oim.orderitemid = od."OrderDetailID"
    LEFT JOIN order_table_mapping otm ON otm.order_id = o.order_id
    LEFT JOIN tables t ON otm.table_id = t.table_id
    LEFT JOIN section s ON t.section_id = s.section_id
    WHERE o."isDelete" = false AND o.order_id = in_order_id;
END;
$$;
 =   DROP FUNCTION public.get_order_details(in_order_id integer);
       public          postgres    false                       1255    50540 %   get_waiting_token_code_by_id(integer)    FUNCTION     �  CREATE FUNCTION public.get_waiting_token_code_by_id(p_token_id integer) RETURNS TABLE(token_id integer, total_person integer, user_name character varying, phone_no character varying, email character varying, sectionid integer, created_at timestamp without time zone, modified_at timestamp without time zone, created_by integer, modified_by integer, isdelete boolean)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
        W.token_id,
        w.total_person,
        w.user_name,
        w."Phone_No",
        w."Email",
        w."SectionId",
        w.created_at,
	    w.modified_at,
	    w.created_by,
	w.modified_by,
	w."isDelete"
    FROM waiting_token_codes W
    WHERE W.token_id = p_token_id
    LIMIT 1;
END;
$$;
 G   DROP FUNCTION public.get_waiting_token_code_by_id(p_token_id integer);
       public          postgres    false                       1255    50529 O   update_order_detail(integer, integer, integer, integer, integer, text, boolean) 	   PROCEDURE     4  CREATE PROCEDURE public.update_order_detail(IN p_order_detail_id integer, IN p_item_id integer, IN p_order_id integer, IN p_quntity integer, IN p_prepared integer, IN p_item_comment text, IN p_is_deleted boolean)
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE "OrderDetail"
    SET
        "ItemId"       = p_item_id,
        "order_id"     = p_order_id,
        "Quntity"      = p_quntity,
        "Prepared"     = p_prepared,
        "Item_Comment" = p_item_comment,
        "isDeleted"    = p_is_deleted
    WHERE "OrderDetailID" = p_order_detail_id;
END;
$$;
 �   DROP PROCEDURE public.update_order_detail(IN p_order_detail_id integer, IN p_item_id integer, IN p_order_id integer, IN p_quntity integer, IN p_prepared integer, IN p_item_comment text, IN p_is_deleted boolean);
       public          postgres    false            �            1259    49893    OrderDetail    TABLE     !  CREATE TABLE public."OrderDetail" (
    "OrderDetailID" integer NOT NULL,
    "ItemId" integer NOT NULL,
    order_id integer NOT NULL,
    "Quntity" integer NOT NULL,
    "Prepared" integer NOT NULL,
    "Item_Comment" character varying,
    "isDeleted" boolean DEFAULT false NOT NULL
);
 !   DROP TABLE public."OrderDetail";
       public         heap    postgres    false            �            1259    49899    OrderDetail_OrderDetailID_seq    SEQUENCE     �   CREATE SEQUENCE public."OrderDetail_OrderDetailID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 6   DROP SEQUENCE public."OrderDetail_OrderDetailID_seq";
       public          postgres    false    215            �           0    0    OrderDetail_OrderDetailID_seq    SEQUENCE OWNED BY     e   ALTER SEQUENCE public."OrderDetail_OrderDetailID_seq" OWNED BY public."OrderDetail"."OrderDetailID";
          public          postgres    false    216            �            1259    49900    category    TABLE     y  CREATE TABLE public.category (
    category_id integer NOT NULL,
    category_name character varying(255) NOT NULL,
    description text,
    is_deleted boolean DEFAULT false NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer
);
    DROP TABLE public.category;
       public         heap    postgres    false            �            1259    49908    category_category_id_seq    SEQUENCE     �   CREATE SEQUENCE public.category_category_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 /   DROP SEQUENCE public.category_category_id_seq;
       public          postgres    false    217            �           0    0    category_category_id_seq    SEQUENCE OWNED BY     U   ALTER SEQUENCE public.category_category_id_seq OWNED BY public.category.category_id;
          public          postgres    false    218            �            1259    49909    cities    TABLE       CREATE TABLE public.cities (
    city_id integer NOT NULL,
    state_id integer,
    city_name character varying(255) NOT NULL,
    create_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);
    DROP TABLE public.cities;
       public         heap    postgres    false            �            1259    49914    cities_city_id_seq    SEQUENCE     �   CREATE SEQUENCE public.cities_city_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 )   DROP SEQUENCE public.cities_city_id_seq;
       public          postgres    false    219            �           0    0    cities_city_id_seq    SEQUENCE OWNED BY     I   ALTER SEQUENCE public.cities_city_id_seq OWNED BY public.cities.city_id;
          public          postgres    false    220            �            1259    49915 	   countries    TABLE       CREATE TABLE public.countries (
    country_id integer NOT NULL,
    country_name character varying(255) NOT NULL,
    create_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);
    DROP TABLE public.countries;
       public         heap    postgres    false            �            1259    49920    countries_country_id_seq    SEQUENCE     �   CREATE SEQUENCE public.countries_country_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 /   DROP SEQUENCE public.countries_country_id_seq;
       public          postgres    false    221            �           0    0    countries_country_id_seq    SEQUENCE OWNED BY     U   ALTER SEQUENCE public.countries_country_id_seq OWNED BY public.countries.country_id;
          public          postgres    false    222            �            1259    49921 	   customers    TABLE     �  CREATE TABLE public.customers (
    customer_id integer NOT NULL,
    name character varying(255) NOT NULL,
    email character varying(255),
    phone character varying(20),
    visits integer,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    "isDelete" boolean DEFAULT false NOT NULL
);
    DROP TABLE public.customers;
       public         heap    postgres    false            �            1259    49929    customers_customer_id_seq    SEQUENCE     �   CREATE SEQUENCE public.customers_customer_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 0   DROP SEQUENCE public.customers_customer_id_seq;
       public          postgres    false    223            �           0    0    customers_customer_id_seq    SEQUENCE OWNED BY     W   ALTER SEQUENCE public.customers_customer_id_seq OWNED BY public.customers.customer_id;
          public          postgres    false    224            �            1259    49930    feedback    TABLE        CREATE TABLE public.feedback (
    feedback_id integer NOT NULL,
    rating integer,
    comments text,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    "Customer_id" integer,
    "Order_id" integer,
    "FoodRating" integer,
    "ServiceRating" integer,
    "AmbienceRating" integer,
    CONSTRAINT feedback_rating_check CHECK (((rating >= 1) AND (rating <= 5)))
);
    DROP TABLE public.feedback;
       public         heap    postgres    false            �            1259    49938    feedback_feedback_id_seq    SEQUENCE     �   CREATE SEQUENCE public.feedback_feedback_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 /   DROP SEQUENCE public.feedback_feedback_id_seq;
       public          postgres    false    225            �           0    0    feedback_feedback_id_seq    SEQUENCE OWNED BY     U   ALTER SEQUENCE public.feedback_feedback_id_seq OWNED BY public.feedback.feedback_id;
          public          postgres    false    226            �            1259    49939    invoice    TABLE     �  CREATE TABLE public.invoice (
    total_amount numeric,
    tax_amount numeric,
    final_amount numeric,
    issued_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    invoice_id integer NOT NULL,
    order_id integer
);
    DROP TABLE public.invoice;
       public         heap    postgres    false            �            1259    49947    invoice_invoice_id_seq    SEQUENCE     �   CREATE SEQUENCE public.invoice_invoice_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public.invoice_invoice_id_seq;
       public          postgres    false    227            �           0    0    invoice_invoice_id_seq    SEQUENCE OWNED BY     Q   ALTER SEQUENCE public.invoice_invoice_id_seq OWNED BY public.invoice.invoice_id;
          public          postgres    false    228            �            1259    49948    mapping_item_modifierGroup    TABLE     �   CREATE TABLE public."mapping_item_modifierGroup" (
    item_id integer,
    "Modifier_group_id" integer,
    mappingid integer NOT NULL,
    "minValue" integer,
    "maxValue" integer
);
 0   DROP TABLE public."mapping_item_modifierGroup";
       public         heap    postgres    false            �            1259    49951 (   mapping_item_modifierGroup_mappingid_seq    SEQUENCE     �   CREATE SEQUENCE public."mapping_item_modifierGroup_mappingid_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 A   DROP SEQUENCE public."mapping_item_modifierGroup_mappingid_seq";
       public          postgres    false    229            �           0    0 (   mapping_item_modifierGroup_mappingid_seq    SEQUENCE OWNED BY     y   ALTER SEQUENCE public."mapping_item_modifierGroup_mappingid_seq" OWNED BY public."mapping_item_modifierGroup".mappingid;
          public          postgres    false    230            �            1259    49952    mapping_modifier_modifiergroup    TABLE     �  CREATE TABLE public.mapping_modifier_modifiergroup (
    modifier_group_id integer NOT NULL,
    modifier_id integer NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    mapping_id integer DEFAULT nextval('public."mapping_item_modifierGroup_mappingid_seq"'::regclass) NOT NULL,
    "isDeleted" boolean DEFAULT false NOT NULL
);
 2   DROP TABLE public.mapping_modifier_modifiergroup;
       public         heap    postgres    false    230            �           0    0 $   TABLE mapping_modifier_modifiergroup    COMMENT     d   COMMENT ON TABLE public.mapping_modifier_modifiergroup IS 'Change name Modifier And ModifierGroup';
          public          postgres    false    231            �            1259    49972    menu_item_item_id_seq    SEQUENCE     �   CREATE SEQUENCE public.menu_item_item_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 ,   DROP SEQUENCE public.menu_item_item_id_seq;
       public          postgres    false    232            �           0    0    menu_item_item_id_seq    SEQUENCE OWNED BY     O   ALTER SEQUENCE public.menu_item_item_id_seq OWNED BY public.menu_item.item_id;
          public          postgres    false    233            �            1259    49973    modifier_group    TABLE     �  CREATE TABLE public.modifier_group (
    modifier_group_id integer NOT NULL,
    group_name character varying(255) NOT NULL,
    description text,
    is_deleted boolean DEFAULT true NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer
);
 "   DROP TABLE public.modifier_group;
       public         heap    postgres    false            �            1259    49981 $   modifier_group_modifier_group_id_seq    SEQUENCE     �   CREATE SEQUENCE public.modifier_group_modifier_group_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 ;   DROP SEQUENCE public.modifier_group_modifier_group_id_seq;
       public          postgres    false    234            �           0    0 $   modifier_group_modifier_group_id_seq    SEQUENCE OWNED BY     m   ALTER SEQUENCE public.modifier_group_modifier_group_id_seq OWNED BY public.modifier_group.modifier_group_id;
          public          postgres    false    235            �            1259    49982    modifiers_item    TABLE     �  CREATE TABLE public.modifiers_item (
    modifier_id integer NOT NULL,
    modifier_name character varying(255) NOT NULL,
    rate numeric(10,2) NOT NULL,
    description text,
    unit_id integer NOT NULL,
    is_deleted boolean DEFAULT false NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    "Quantity" integer NOT NULL
);
 "   DROP TABLE public.modifiers_item;
       public         heap    postgres    false            �            1259    49990    modifiers_item_modifier_id_seq    SEQUENCE     �   CREATE SEQUENCE public.modifiers_item_modifier_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 5   DROP SEQUENCE public.modifiers_item_modifier_id_seq;
       public          postgres    false    236            �           0    0    modifiers_item_modifier_id_seq    SEQUENCE OWNED BY     a   ALTER SEQUENCE public.modifiers_item_modifier_id_seq OWNED BY public.modifiers_item.modifier_id;
          public          postgres    false    237            �            1259    49991    order    TABLE       CREATE TABLE public."order" (
    order_id integer NOT NULL,
    customer_id integer NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    "Amount" real NOT NULL,
    "OrderStatus" character varying NOT NULL,
    order_type character varying(20) DEFAULT 'Dine-In'::character varying NOT NULL,
    "Total_Person" integer NOT NULL,
    "isDelete" boolean DEFAULT false NOT NULL,
    "OrderComment" character varying,
    "SubTotal" real,
    "OtherTax" real,
    CONSTRAINT check_order_type CHECK (((order_type)::text = ANY (ARRAY[('Dine-In'::character varying)::text, ('Take-Away'::character varying)::text])))
);
    DROP TABLE public."order";
       public         heap    postgres    false            �           0    0    TABLE "order"    COMMENT     8   COMMENT ON TABLE public."order" IS 'Change Name Order';
          public          postgres    false    238            �            1259    50001    order_table_mapping    TABLE     �   CREATE TABLE public.order_table_mapping (
    order_detail_id integer NOT NULL,
    table_id integer NOT NULL,
    order_id integer NOT NULL,
    "IsDelete" boolean DEFAULT false NOT NULL
);
 '   DROP TABLE public.order_table_mapping;
       public         heap    postgres    false            �            1259    50005 '   order_table_mapping_order_detail_id_seq    SEQUENCE     �   CREATE SEQUENCE public.order_table_mapping_order_detail_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 >   DROP SEQUENCE public.order_table_mapping_order_detail_id_seq;
       public          postgres    false    239            �           0    0 '   order_table_mapping_order_detail_id_seq    SEQUENCE OWNED BY     s   ALTER SEQUENCE public.order_table_mapping_order_detail_id_seq OWNED BY public.order_table_mapping.order_detail_id;
          public          postgres    false    240            �            1259    50006    order_tax_mapping    TABLE     �   CREATE TABLE public.order_tax_mapping (
    order_tax_id integer NOT NULL,
    order_id integer NOT NULL,
    tax_id integer NOT NULL,
    tax_amount real,
    "IsDeleted" boolean DEFAULT false NOT NULL
);
 %   DROP TABLE public.order_tax_mapping;
       public         heap    postgres    false            �            1259    50010 "   order_tax_mapping_order_tax_id_seq    SEQUENCE     �   CREATE SEQUENCE public.order_tax_mapping_order_tax_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 9   DROP SEQUENCE public.order_tax_mapping_order_tax_id_seq;
       public          postgres    false    241            �           0    0 "   order_tax_mapping_order_tax_id_seq    SEQUENCE OWNED BY     i   ALTER SEQUENCE public.order_tax_mapping_order_tax_id_seq OWNED BY public.order_tax_mapping.order_tax_id;
          public          postgres    false    242            �            1259    50011    ordered_item_modifier    TABLE     �   CREATE TABLE public.ordered_item_modifier (
    ordered_item_modifier_id integer NOT NULL,
    quantity integer,
    modifier_id integer,
    orderitemid integer
);
 )   DROP TABLE public.ordered_item_modifier;
       public         heap    postgres    false            �            1259    50014 2   ordered_item_modifier_ordered_item_modifier_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ordered_item_modifier_ordered_item_modifier_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 I   DROP SEQUENCE public.ordered_item_modifier_ordered_item_modifier_id_seq;
       public          postgres    false    243            �           0    0 2   ordered_item_modifier_ordered_item_modifier_id_seq    SEQUENCE OWNED BY     �   ALTER SEQUENCE public.ordered_item_modifier_ordered_item_modifier_id_seq OWNED BY public.ordered_item_modifier.ordered_item_modifier_id;
          public          postgres    false    244            �            1259    50015    ordered_order_id_seq    SEQUENCE     �   CREATE SEQUENCE public.ordered_order_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 +   DROP SEQUENCE public.ordered_order_id_seq;
       public          postgres    false    238            �           0    0    ordered_order_id_seq    SEQUENCE OWNED BY     M   ALTER SEQUENCE public.ordered_order_id_seq OWNED BY public."order".order_id;
          public          postgres    false    245            �            1259    50016    payment    TABLE       CREATE TABLE public.payment (
    payment_id integer NOT NULL,
    payment_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    order_id integer,
    payment_method character varying
);
    DROP TABLE public.payment;
       public         heap    postgres    false            �            1259    50023    payment_payment_id_seq    SEQUENCE     �   CREATE SEQUENCE public.payment_payment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public.payment_payment_id_seq;
       public          postgres    false    246            �           0    0    payment_payment_id_seq    SEQUENCE OWNED BY     Q   ALTER SEQUENCE public.payment_payment_id_seq OWNED BY public.payment.payment_id;
          public          postgres    false    247            �            1259    50024    permission_table    TABLE     @  CREATE TABLE public.permission_table (
    permission_id integer NOT NULL,
    permission_name character varying(255) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer
);
 $   DROP TABLE public.permission_table;
       public         heap    postgres    false            �            1259    50029 "   permission_table_permission_id_seq    SEQUENCE     �   CREATE SEQUENCE public.permission_table_permission_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 9   DROP SEQUENCE public.permission_table_permission_id_seq;
       public          postgres    false    248            �           0    0 "   permission_table_permission_id_seq    SEQUENCE OWNED BY     i   ALTER SEQUENCE public.permission_table_permission_id_seq OWNED BY public.permission_table.permission_id;
          public          postgres    false    249            �            1259    50030    role_permission_table    TABLE     �  CREATE TABLE public.role_permission_table (
    role_permission_id integer NOT NULL,
    roles integer,
    can_view boolean NOT NULL,
    can_add_edit boolean NOT NULL,
    can_delete boolean NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    "PermissionName" character varying
);
 )   DROP TABLE public.role_permission_table;
       public         heap    postgres    false            �            1259    50037 ,   role_permission_table_role_permission_id_seq    SEQUENCE     �   CREATE SEQUENCE public.role_permission_table_role_permission_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 C   DROP SEQUENCE public.role_permission_table_role_permission_id_seq;
       public          postgres    false    250            �           0    0 ,   role_permission_table_role_permission_id_seq    SEQUENCE OWNED BY     }   ALTER SEQUENCE public.role_permission_table_role_permission_id_seq OWNED BY public.role_permission_table.role_permission_id;
          public          postgres    false    251            �            1259    50038    roles    TABLE     j   CREATE TABLE public.roles (
    role_id integer NOT NULL,
    role_name character varying(50) NOT NULL
);
    DROP TABLE public.roles;
       public         heap    postgres    false            �            1259    50041    roles_role_id_seq    SEQUENCE     �   CREATE SEQUENCE public.roles_role_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 (   DROP SEQUENCE public.roles_role_id_seq;
       public          postgres    false    252            �           0    0    roles_role_id_seq    SEQUENCE OWNED BY     G   ALTER SEQUENCE public.roles_role_id_seq OWNED BY public.roles.role_id;
          public          postgres    false    253            �            1259    50042    section    TABLE     w  CREATE TABLE public.section (
    section_id integer NOT NULL,
    section_name character varying(255) NOT NULL,
    description text,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    "IsDeleted" boolean DEFAULT false NOT NULL
);
    DROP TABLE public.section;
       public         heap    postgres    false            �            1259    50050    section_section_id_seq    SEQUENCE     �   CREATE SEQUENCE public.section_section_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public.section_section_id_seq;
       public          postgres    false    254            �           0    0    section_section_id_seq    SEQUENCE OWNED BY     Q   ALTER SEQUENCE public.section_section_id_seq OWNED BY public.section.section_id;
          public          postgres    false    255                        1259    50051    states    TABLE       CREATE TABLE public.states (
    state_id integer NOT NULL,
    country_id integer,
    state_name character varying(255) NOT NULL,
    create_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);
    DROP TABLE public.states;
       public         heap    postgres    false                       1259    50056    states_state_id_seq    SEQUENCE     �   CREATE SEQUENCE public.states_state_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 *   DROP SEQUENCE public.states_state_id_seq;
       public          postgres    false    256            �           0    0    states_state_id_seq    SEQUENCE OWNED BY     K   ALTER SEQUENCE public.states_state_id_seq OWNED BY public.states.state_id;
          public          postgres    false    257                       1259    50057    tables    TABLE     �  CREATE TABLE public.tables (
    table_id integer NOT NULL,
    section_id integer NOT NULL,
    table_name character varying(255) NOT NULL,
    capacity integer NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    status character varying(10) DEFAULT 'Available'::character varying NOT NULL,
    "isDelete" boolean DEFAULT false NOT NULL,
    CONSTRAINT check_status CHECK (((status)::text = ANY (ARRAY[('Available'::character varying)::text, ('Occupied'::character varying)::text, ('Running'::character varying)::text])))
);
    DROP TABLE public.tables;
       public         heap    postgres    false                       1259    50065    tables_table_id_seq    SEQUENCE     �   CREATE SEQUENCE public.tables_table_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 *   DROP SEQUENCE public.tables_table_id_seq;
       public          postgres    false    258            �           0    0    tables_table_id_seq    SEQUENCE OWNED BY     K   ALTER SEQUENCE public.tables_table_id_seq OWNED BY public.tables.table_id;
          public          postgres    false    259                       1259    50066    taxes_and_fees    TABLE       CREATE TABLE public.taxes_and_fees (
    tax_id integer NOT NULL,
    tax_name character varying(255) NOT NULL,
    tax_value numeric(5,2) NOT NULL,
    is_enabled boolean DEFAULT false NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    "isDelete" boolean DEFAULT false NOT NULL,
    "IsDefault" boolean DEFAULT false NOT NULL,
    "TaxType" boolean DEFAULT false NOT NULL
);
 "   DROP TABLE public.taxes_and_fees;
       public         heap    postgres    false                       1259    50075    taxes_and_fees_tax_id_seq    SEQUENCE     �   CREATE SEQUENCE public.taxes_and_fees_tax_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 0   DROP SEQUENCE public.taxes_and_fees_tax_id_seq;
       public          postgres    false    260            �           0    0    taxes_and_fees_tax_id_seq    SEQUENCE OWNED BY     W   ALTER SEQUENCE public.taxes_and_fees_tax_id_seq OWNED BY public.taxes_and_fees.tax_id;
          public          postgres    false    261                       1259    50076    unit    TABLE     i   CREATE TABLE public.unit (
    unit_id integer NOT NULL,
    unit_name character varying(50) NOT NULL
);
    DROP TABLE public.unit;
       public         heap    postgres    false                       1259    50079    unit_unit_id_seq    SEQUENCE     �   CREATE SEQUENCE public.unit_unit_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE public.unit_unit_id_seq;
       public          postgres    false    262            �           0    0    unit_unit_id_seq    SEQUENCE OWNED BY     E   ALTER SEQUENCE public.unit_unit_id_seq OWNED BY public.unit.unit_id;
          public          postgres    false    263                       1259    50080    users    TABLE     }  CREATE TABLE public.users (
    user_id integer NOT NULL,
    email character varying(255) NOT NULL,
    password character varying(50) NOT NULL,
    first_name character varying(255) NOT NULL,
    last_name character varying(255),
    user_name character varying(255) NOT NULL,
    image character varying,
    roles integer NOT NULL,
    country_name integer NOT NULL,
    state_name integer NOT NULL,
    city_name integer NOT NULL,
    zip_code integer NOT NULL,
    create_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    "RememberMe" boolean,
    "ResetToken" character varying,
    "Address" character varying NOT NULL,
    "Status" boolean DEFAULT true NOT NULL,
    phone character varying NOT NULL,
    "isDelete" boolean DEFAULT false NOT NULL
);
    DROP TABLE public.users;
       public         heap    postgres    false            	           1259    50089    users_user_id_seq    SEQUENCE     �   CREATE SEQUENCE public.users_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 (   DROP SEQUENCE public.users_user_id_seq;
       public          postgres    false    264            �           0    0    users_user_id_seq    SEQUENCE OWNED BY     G   ALTER SEQUENCE public.users_user_id_seq OWNED BY public.users.user_id;
          public          postgres    false    265            
           1259    50090    waiting_token_codes    TABLE       CREATE TABLE public.waiting_token_codes (
    token_id integer NOT NULL,
    total_person integer NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_by integer,
    user_name character varying NOT NULL,
    "Email" character varying NOT NULL,
    "SectionId" integer NOT NULL,
    "Phone_No" character varying NOT NULL,
    "isDelete" boolean DEFAULT false NOT NULL
);
 '   DROP TABLE public.waiting_token_codes;
       public         heap    postgres    false                       1259    50098     waiting_token_codes_token_id_seq    SEQUENCE     �   CREATE SEQUENCE public.waiting_token_codes_token_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 7   DROP SEQUENCE public.waiting_token_codes_token_id_seq;
       public          postgres    false    266            �           0    0     waiting_token_codes_token_id_seq    SEQUENCE OWNED BY     e   ALTER SEQUENCE public.waiting_token_codes_token_id_seq OWNED BY public.waiting_token_codes.token_id;
          public          postgres    false    267                       1259    50563    waiting_user    TABLE     r  CREATE TABLE public.waiting_user (
    token_id integer,
    total_person integer,
    created_at timestamp without time zone,
    modified_at timestamp without time zone,
    created_by integer,
    modified_by integer,
    user_name character varying,
    "Email" character varying,
    "SectionId" integer,
    "Phone_No" character varying,
    "isDelete" boolean
);
     DROP TABLE public.waiting_user;
       public         heap    postgres    false            �           2604    50099    OrderDetail OrderDetailID    DEFAULT     �   ALTER TABLE ONLY public."OrderDetail" ALTER COLUMN "OrderDetailID" SET DEFAULT nextval('public."OrderDetail_OrderDetailID_seq"'::regclass);
 L   ALTER TABLE public."OrderDetail" ALTER COLUMN "OrderDetailID" DROP DEFAULT;
       public          postgres    false    216    215            �           2604    50100    category category_id    DEFAULT     |   ALTER TABLE ONLY public.category ALTER COLUMN category_id SET DEFAULT nextval('public.category_category_id_seq'::regclass);
 C   ALTER TABLE public.category ALTER COLUMN category_id DROP DEFAULT;
       public          postgres    false    218    217            �           2604    50101    cities city_id    DEFAULT     p   ALTER TABLE ONLY public.cities ALTER COLUMN city_id SET DEFAULT nextval('public.cities_city_id_seq'::regclass);
 =   ALTER TABLE public.cities ALTER COLUMN city_id DROP DEFAULT;
       public          postgres    false    220    219            �           2604    50102    countries country_id    DEFAULT     |   ALTER TABLE ONLY public.countries ALTER COLUMN country_id SET DEFAULT nextval('public.countries_country_id_seq'::regclass);
 C   ALTER TABLE public.countries ALTER COLUMN country_id DROP DEFAULT;
       public          postgres    false    222    221            �           2604    50103    customers customer_id    DEFAULT     ~   ALTER TABLE ONLY public.customers ALTER COLUMN customer_id SET DEFAULT nextval('public.customers_customer_id_seq'::regclass);
 D   ALTER TABLE public.customers ALTER COLUMN customer_id DROP DEFAULT;
       public          postgres    false    224    223            �           2604    50104    feedback feedback_id    DEFAULT     |   ALTER TABLE ONLY public.feedback ALTER COLUMN feedback_id SET DEFAULT nextval('public.feedback_feedback_id_seq'::regclass);
 C   ALTER TABLE public.feedback ALTER COLUMN feedback_id DROP DEFAULT;
       public          postgres    false    226    225            �           2604    50105    invoice invoice_id    DEFAULT     x   ALTER TABLE ONLY public.invoice ALTER COLUMN invoice_id SET DEFAULT nextval('public.invoice_invoice_id_seq'::regclass);
 A   ALTER TABLE public.invoice ALTER COLUMN invoice_id DROP DEFAULT;
       public          postgres    false    228    227            �           2604    50106 $   mapping_item_modifierGroup mappingid    DEFAULT     �   ALTER TABLE ONLY public."mapping_item_modifierGroup" ALTER COLUMN mappingid SET DEFAULT nextval('public."mapping_item_modifierGroup_mappingid_seq"'::regclass);
 U   ALTER TABLE public."mapping_item_modifierGroup" ALTER COLUMN mappingid DROP DEFAULT;
       public          postgres    false    230    229                       2604    50107    menu_item item_id    DEFAULT     v   ALTER TABLE ONLY public.menu_item ALTER COLUMN item_id SET DEFAULT nextval('public.menu_item_item_id_seq'::regclass);
 @   ALTER TABLE public.menu_item ALTER COLUMN item_id DROP DEFAULT;
       public          postgres    false    233    232                       2604    50108     modifier_group modifier_group_id    DEFAULT     �   ALTER TABLE ONLY public.modifier_group ALTER COLUMN modifier_group_id SET DEFAULT nextval('public.modifier_group_modifier_group_id_seq'::regclass);
 O   ALTER TABLE public.modifier_group ALTER COLUMN modifier_group_id DROP DEFAULT;
       public          postgres    false    235    234                       2604    50109    modifiers_item modifier_id    DEFAULT     �   ALTER TABLE ONLY public.modifiers_item ALTER COLUMN modifier_id SET DEFAULT nextval('public.modifiers_item_modifier_id_seq'::regclass);
 I   ALTER TABLE public.modifiers_item ALTER COLUMN modifier_id DROP DEFAULT;
       public          postgres    false    237    236                       2604    50110    order order_id    DEFAULT     t   ALTER TABLE ONLY public."order" ALTER COLUMN order_id SET DEFAULT nextval('public.ordered_order_id_seq'::regclass);
 ?   ALTER TABLE public."order" ALTER COLUMN order_id DROP DEFAULT;
       public          postgres    false    245    238                       2604    50111 #   order_table_mapping order_detail_id    DEFAULT     �   ALTER TABLE ONLY public.order_table_mapping ALTER COLUMN order_detail_id SET DEFAULT nextval('public.order_table_mapping_order_detail_id_seq'::regclass);
 R   ALTER TABLE public.order_table_mapping ALTER COLUMN order_detail_id DROP DEFAULT;
       public          postgres    false    240    239                       2604    50112    order_tax_mapping order_tax_id    DEFAULT     �   ALTER TABLE ONLY public.order_tax_mapping ALTER COLUMN order_tax_id SET DEFAULT nextval('public.order_tax_mapping_order_tax_id_seq'::regclass);
 M   ALTER TABLE public.order_tax_mapping ALTER COLUMN order_tax_id DROP DEFAULT;
       public          postgres    false    242    241                       2604    50113 .   ordered_item_modifier ordered_item_modifier_id    DEFAULT     �   ALTER TABLE ONLY public.ordered_item_modifier ALTER COLUMN ordered_item_modifier_id SET DEFAULT nextval('public.ordered_item_modifier_ordered_item_modifier_id_seq'::regclass);
 ]   ALTER TABLE public.ordered_item_modifier ALTER COLUMN ordered_item_modifier_id DROP DEFAULT;
       public          postgres    false    244    243                       2604    50114    payment payment_id    DEFAULT     x   ALTER TABLE ONLY public.payment ALTER COLUMN payment_id SET DEFAULT nextval('public.payment_payment_id_seq'::regclass);
 A   ALTER TABLE public.payment ALTER COLUMN payment_id DROP DEFAULT;
       public          postgres    false    247    246            !           2604    50115    permission_table permission_id    DEFAULT     �   ALTER TABLE ONLY public.permission_table ALTER COLUMN permission_id SET DEFAULT nextval('public.permission_table_permission_id_seq'::regclass);
 M   ALTER TABLE public.permission_table ALTER COLUMN permission_id DROP DEFAULT;
       public          postgres    false    249    248            $           2604    50116 (   role_permission_table role_permission_id    DEFAULT     �   ALTER TABLE ONLY public.role_permission_table ALTER COLUMN role_permission_id SET DEFAULT nextval('public.role_permission_table_role_permission_id_seq'::regclass);
 W   ALTER TABLE public.role_permission_table ALTER COLUMN role_permission_id DROP DEFAULT;
       public          postgres    false    251    250            '           2604    50117    roles role_id    DEFAULT     n   ALTER TABLE ONLY public.roles ALTER COLUMN role_id SET DEFAULT nextval('public.roles_role_id_seq'::regclass);
 <   ALTER TABLE public.roles ALTER COLUMN role_id DROP DEFAULT;
       public          postgres    false    253    252            (           2604    50118    section section_id    DEFAULT     x   ALTER TABLE ONLY public.section ALTER COLUMN section_id SET DEFAULT nextval('public.section_section_id_seq'::regclass);
 A   ALTER TABLE public.section ALTER COLUMN section_id DROP DEFAULT;
       public          postgres    false    255    254            ,           2604    50119    states state_id    DEFAULT     r   ALTER TABLE ONLY public.states ALTER COLUMN state_id SET DEFAULT nextval('public.states_state_id_seq'::regclass);
 >   ALTER TABLE public.states ALTER COLUMN state_id DROP DEFAULT;
       public          postgres    false    257    256            /           2604    50120    tables table_id    DEFAULT     r   ALTER TABLE ONLY public.tables ALTER COLUMN table_id SET DEFAULT nextval('public.tables_table_id_seq'::regclass);
 >   ALTER TABLE public.tables ALTER COLUMN table_id DROP DEFAULT;
       public          postgres    false    259    258            4           2604    50121    taxes_and_fees tax_id    DEFAULT     ~   ALTER TABLE ONLY public.taxes_and_fees ALTER COLUMN tax_id SET DEFAULT nextval('public.taxes_and_fees_tax_id_seq'::regclass);
 D   ALTER TABLE public.taxes_and_fees ALTER COLUMN tax_id DROP DEFAULT;
       public          postgres    false    261    260            ;           2604    50122    unit unit_id    DEFAULT     l   ALTER TABLE ONLY public.unit ALTER COLUMN unit_id SET DEFAULT nextval('public.unit_unit_id_seq'::regclass);
 ;   ALTER TABLE public.unit ALTER COLUMN unit_id DROP DEFAULT;
       public          postgres    false    263    262            <           2604    50123    users user_id    DEFAULT     n   ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.users_user_id_seq'::regclass);
 <   ALTER TABLE public.users ALTER COLUMN user_id DROP DEFAULT;
       public          postgres    false    265    264            A           2604    50124    waiting_token_codes token_id    DEFAULT     �   ALTER TABLE ONLY public.waiting_token_codes ALTER COLUMN token_id SET DEFAULT nextval('public.waiting_token_codes_token_id_seq'::regclass);
 K   ALTER TABLE public.waiting_token_codes ALTER COLUMN token_id DROP DEFAULT;
       public          postgres    false    267    266            L          0    49893    OrderDetail 
   TABLE DATA           �   COPY public."OrderDetail" ("OrderDetailID", "ItemId", order_id, "Quntity", "Prepared", "Item_Comment", "isDeleted") FROM stdin;
    public          postgres    false    215   ��      N          0    49900    category 
   TABLE DATA           �   COPY public.category (category_id, category_name, description, is_deleted, created_at, modified_at, created_by, modified_by) FROM stdin;
    public          postgres    false    217   ��      P          0    49909    cities 
   TABLE DATA           X   COPY public.cities (city_id, state_id, city_name, create_date, modified_at) FROM stdin;
    public          postgres    false    219   ~�      R          0    49915 	   countries 
   TABLE DATA           W   COPY public.countries (country_id, country_name, create_date, modified_at) FROM stdin;
    public          postgres    false    221   ��      T          0    49921 	   customers 
   TABLE DATA           �   COPY public.customers (customer_id, name, email, phone, visits, created_at, modified_at, created_by, modified_by, "isDelete") FROM stdin;
    public          postgres    false    223   *�      V          0    49930    feedback 
   TABLE DATA           �   COPY public.feedback (feedback_id, rating, comments, created_at, modified_at, created_by, modified_by, "Customer_id", "Order_id", "FoodRating", "ServiceRating", "AmbienceRating") FROM stdin;
    public          postgres    false    225   �      X          0    49939    invoice 
   TABLE DATA           �   COPY public.invoice (total_amount, tax_amount, final_amount, issued_at, created_at, modified_at, created_by, modified_by, invoice_id, order_id) FROM stdin;
    public          postgres    false    227   V�      Z          0    49948    mapping_item_modifierGroup 
   TABLE DATA           w   COPY public."mapping_item_modifierGroup" (item_id, "Modifier_group_id", mappingid, "minValue", "maxValue") FROM stdin;
    public          postgres    false    229   ��      \          0    49952    mapping_modifier_modifiergroup 
   TABLE DATA           �   COPY public.mapping_modifier_modifiergroup (modifier_group_id, modifier_id, created_at, modified_at, created_by, modified_by, mapping_id, "isDeleted") FROM stdin;
    public          postgres    false    231   �      ]          0    49959 	   menu_item 
   TABLE DATA           �   COPY public.menu_item (item_id, category_id, item_name, description, price, is_deleted, is_available, created_at, modified_at, created_by, modified_by, "IsDefault", "Tax", "Quantity", shortcode, "Image", unit_id, food_type, is_fav) FROM stdin;
    public          postgres    false    232   t�      _          0    49973    modifier_group 
   TABLE DATA           �   COPY public.modifier_group (modifier_group_id, group_name, description, is_deleted, created_at, modified_at, created_by, modified_by) FROM stdin;
    public          postgres    false    234   ��      a          0    49982    modifiers_item 
   TABLE DATA           �   COPY public.modifiers_item (modifier_id, modifier_name, rate, description, unit_id, is_deleted, created_at, modified_at, created_by, modified_by, "Quantity") FROM stdin;
    public          postgres    false    236   �      c          0    49991    order 
   TABLE DATA           �   COPY public."order" (order_id, customer_id, created_at, modified_at, created_by, modified_by, "Amount", "OrderStatus", order_type, "Total_Person", "isDelete", "OrderComment", "SubTotal", "OtherTax") FROM stdin;
    public          postgres    false    238   ��      d          0    50001    order_table_mapping 
   TABLE DATA           ^   COPY public.order_table_mapping (order_detail_id, table_id, order_id, "IsDelete") FROM stdin;
    public          postgres    false    239   �      f          0    50006    order_tax_mapping 
   TABLE DATA           d   COPY public.order_tax_mapping (order_tax_id, order_id, tax_id, tax_amount, "IsDeleted") FROM stdin;
    public          postgres    false    241   +      h          0    50011    ordered_item_modifier 
   TABLE DATA           m   COPY public.ordered_item_modifier (ordered_item_modifier_id, quantity, modifier_id, orderitemid) FROM stdin;
    public          postgres    false    243   M      k          0    50016    payment 
   TABLE DATA           a   COPY public.payment (payment_id, payment_date, created_at, order_id, payment_method) FROM stdin;
    public          postgres    false    246   �      m          0    50024    permission_table 
   TABLE DATA           |   COPY public.permission_table (permission_id, permission_name, created_at, modified_at, created_by, modified_by) FROM stdin;
    public          postgres    false    248   T      o          0    50030    role_permission_table 
   TABLE DATA           �   COPY public.role_permission_table (role_permission_id, roles, can_view, can_add_edit, can_delete, created_at, modified_at, created_by, "PermissionName") FROM stdin;
    public          postgres    false    250   q      q          0    50038    roles 
   TABLE DATA           3   COPY public.roles (role_id, role_name) FROM stdin;
    public          postgres    false    252   �      s          0    50042    section 
   TABLE DATA           �   COPY public.section (section_id, section_name, description, created_at, modified_at, created_by, modified_by, "IsDeleted") FROM stdin;
    public          postgres    false    254   �      u          0    50051    states 
   TABLE DATA           \   COPY public.states (state_id, country_id, state_name, create_date, modified_at) FROM stdin;
    public          postgres    false    256   6      w          0    50057    tables 
   TABLE DATA           �   COPY public.tables (table_id, section_id, table_name, capacity, created_at, modified_at, created_by, modified_by, status, "isDelete") FROM stdin;
    public          postgres    false    258   �      y          0    50066    taxes_and_fees 
   TABLE DATA           �   COPY public.taxes_and_fees (tax_id, tax_name, tax_value, is_enabled, created_at, modified_at, created_by, modified_by, "isDelete", "IsDefault", "TaxType") FROM stdin;
    public          postgres    false    260   �      {          0    50076    unit 
   TABLE DATA           2   COPY public.unit (unit_id, unit_name) FROM stdin;
    public          postgres    false    262         }          0    50080    users 
   TABLE DATA             COPY public.users (user_id, email, password, first_name, last_name, user_name, image, roles, country_name, state_name, city_name, zip_code, create_date, modified_at, created_by, modified_by, "RememberMe", "ResetToken", "Address", "Status", phone, "isDelete") FROM stdin;
    public          postgres    false    264   7                0    50090    waiting_token_codes 
   TABLE DATA           �   COPY public.waiting_token_codes (token_id, total_person, created_at, modified_at, created_by, modified_by, user_name, "Email", "SectionId", "Phone_No", "isDelete") FROM stdin;
    public          postgres    false    266   �      �          0    50563    waiting_user 
   TABLE DATA           �   COPY public.waiting_user (token_id, total_person, created_at, modified_at, created_by, modified_by, user_name, "Email", "SectionId", "Phone_No", "isDelete") FROM stdin;
    public          postgres    false    268   �&      �           0    0    OrderDetail_OrderDetailID_seq    SEQUENCE SET     N   SELECT pg_catalog.setval('public."OrderDetail_OrderDetailID_seq"', 97, true);
          public          postgres    false    216            �           0    0    category_category_id_seq    SEQUENCE SET     G   SELECT pg_catalog.setval('public.category_category_id_seq', 64, true);
          public          postgres    false    218            �           0    0    cities_city_id_seq    SEQUENCE SET     A   SELECT pg_catalog.setval('public.cities_city_id_seq', 1, false);
          public          postgres    false    220            �           0    0    countries_country_id_seq    SEQUENCE SET     G   SELECT pg_catalog.setval('public.countries_country_id_seq', 1, false);
          public          postgres    false    222            �           0    0    customers_customer_id_seq    SEQUENCE SET     I   SELECT pg_catalog.setval('public.customers_customer_id_seq', 116, true);
          public          postgres    false    224            �           0    0    feedback_feedback_id_seq    SEQUENCE SET     G   SELECT pg_catalog.setval('public.feedback_feedback_id_seq', 32, true);
          public          postgres    false    226            �           0    0    invoice_invoice_id_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.invoice_invoice_id_seq', 1, false);
          public          postgres    false    228            �           0    0 (   mapping_item_modifierGroup_mappingid_seq    SEQUENCE SET     Z   SELECT pg_catalog.setval('public."mapping_item_modifierGroup_mappingid_seq"', 284, true);
          public          postgres    false    230            �           0    0    menu_item_item_id_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.menu_item_item_id_seq', 247, true);
          public          postgres    false    233            �           0    0 $   modifier_group_modifier_group_id_seq    SEQUENCE SET     T   SELECT pg_catalog.setval('public.modifier_group_modifier_group_id_seq', 207, true);
          public          postgres    false    235            �           0    0    modifiers_item_modifier_id_seq    SEQUENCE SET     N   SELECT pg_catalog.setval('public.modifiers_item_modifier_id_seq', 122, true);
          public          postgres    false    237            �           0    0 '   order_table_mapping_order_detail_id_seq    SEQUENCE SET     V   SELECT pg_catalog.setval('public.order_table_mapping_order_detail_id_seq', 74, true);
          public          postgres    false    240            �           0    0 "   order_tax_mapping_order_tax_id_seq    SEQUENCE SET     Q   SELECT pg_catalog.setval('public.order_tax_mapping_order_tax_id_seq', 50, true);
          public          postgres    false    242            �           0    0 2   ordered_item_modifier_ordered_item_modifier_id_seq    SEQUENCE SET     b   SELECT pg_catalog.setval('public.ordered_item_modifier_ordered_item_modifier_id_seq', 181, true);
          public          postgres    false    244            �           0    0    ordered_order_id_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.ordered_order_id_seq', 83, true);
          public          postgres    false    245            �           0    0    payment_payment_id_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.payment_payment_id_seq', 46, true);
          public          postgres    false    247            �           0    0 "   permission_table_permission_id_seq    SEQUENCE SET     Q   SELECT pg_catalog.setval('public.permission_table_permission_id_seq', 1, false);
          public          postgres    false    249            �           0    0 ,   role_permission_table_role_permission_id_seq    SEQUENCE SET     Z   SELECT pg_catalog.setval('public.role_permission_table_role_permission_id_seq', 4, true);
          public          postgres    false    251            �           0    0    roles_role_id_seq    SEQUENCE SET     @   SELECT pg_catalog.setval('public.roles_role_id_seq', 1, false);
          public          postgres    false    253            �           0    0    section_section_id_seq    SEQUENCE SET     F   SELECT pg_catalog.setval('public.section_section_id_seq', 109, true);
          public          postgres    false    255            �           0    0    states_state_id_seq    SEQUENCE SET     B   SELECT pg_catalog.setval('public.states_state_id_seq', 1, false);
          public          postgres    false    257            �           0    0    tables_table_id_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.tables_table_id_seq', 107, true);
          public          postgres    false    259            �           0    0    taxes_and_fees_tax_id_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.taxes_and_fees_tax_id_seq', 16, true);
          public          postgres    false    261            �           0    0    unit_unit_id_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('public.unit_unit_id_seq', 1, false);
          public          postgres    false    263            �           0    0    users_user_id_seq    SEQUENCE SET     @   SELECT pg_catalog.setval('public.users_user_id_seq', 59, true);
          public          postgres    false    265            �           0    0     waiting_token_codes_token_id_seq    SEQUENCE SET     O   SELECT pg_catalog.setval('public.waiting_token_codes_token_id_seq', 75, true);
          public          postgres    false    267            J           2606    50126    OrderDetail OrderDetail_pkey 
   CONSTRAINT     k   ALTER TABLE ONLY public."OrderDetail"
    ADD CONSTRAINT "OrderDetail_pkey" PRIMARY KEY ("OrderDetailID");
 J   ALTER TABLE ONLY public."OrderDetail" DROP CONSTRAINT "OrderDetail_pkey";
       public            postgres    false    215            L           2606    50128    category category_pkey 
   CONSTRAINT     ]   ALTER TABLE ONLY public.category
    ADD CONSTRAINT category_pkey PRIMARY KEY (category_id);
 @   ALTER TABLE ONLY public.category DROP CONSTRAINT category_pkey;
       public            postgres    false    217            N           2606    50130    cities cities_pkey 
   CONSTRAINT     U   ALTER TABLE ONLY public.cities
    ADD CONSTRAINT cities_pkey PRIMARY KEY (city_id);
 <   ALTER TABLE ONLY public.cities DROP CONSTRAINT cities_pkey;
       public            postgres    false    219            P           2606    50132    countries countries_pkey 
   CONSTRAINT     ^   ALTER TABLE ONLY public.countries
    ADD CONSTRAINT countries_pkey PRIMARY KEY (country_id);
 B   ALTER TABLE ONLY public.countries DROP CONSTRAINT countries_pkey;
       public            postgres    false    221            R           2606    50134    customers customers_email_key 
   CONSTRAINT     Y   ALTER TABLE ONLY public.customers
    ADD CONSTRAINT customers_email_key UNIQUE (email);
 G   ALTER TABLE ONLY public.customers DROP CONSTRAINT customers_email_key;
       public            postgres    false    223            T           2606    50136    customers customers_pkey 
   CONSTRAINT     _   ALTER TABLE ONLY public.customers
    ADD CONSTRAINT customers_pkey PRIMARY KEY (customer_id);
 B   ALTER TABLE ONLY public.customers DROP CONSTRAINT customers_pkey;
       public            postgres    false    223            V           2606    50138    feedback feedback_pkey 
   CONSTRAINT     ]   ALTER TABLE ONLY public.feedback
    ADD CONSTRAINT feedback_pkey PRIMARY KEY (feedback_id);
 @   ALTER TABLE ONLY public.feedback DROP CONSTRAINT feedback_pkey;
       public            postgres    false    225            X           2606    50140    invoice invoice_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.invoice
    ADD CONSTRAINT invoice_pkey PRIMARY KEY (invoice_id);
 >   ALTER TABLE ONLY public.invoice DROP CONSTRAINT invoice_pkey;
       public            postgres    false    227            Z           2606    50142 :   mapping_item_modifierGroup mapping_item_modifierGroup_pkey 
   CONSTRAINT     �   ALTER TABLE ONLY public."mapping_item_modifierGroup"
    ADD CONSTRAINT "mapping_item_modifierGroup_pkey" PRIMARY KEY (mappingid);
 h   ALTER TABLE ONLY public."mapping_item_modifierGroup" DROP CONSTRAINT "mapping_item_modifierGroup_pkey";
       public            postgres    false    229            \           2606    50144 9   mapping_modifier_modifiergroup mapping_item_modifier_pkey 
   CONSTRAINT        ALTER TABLE ONLY public.mapping_modifier_modifiergroup
    ADD CONSTRAINT mapping_item_modifier_pkey PRIMARY KEY (mapping_id);
 c   ALTER TABLE ONLY public.mapping_modifier_modifiergroup DROP CONSTRAINT mapping_item_modifier_pkey;
       public            postgres    false    231            ^           2606    50146    menu_item menu_item_pkey 
   CONSTRAINT     [   ALTER TABLE ONLY public.menu_item
    ADD CONSTRAINT menu_item_pkey PRIMARY KEY (item_id);
 B   ALTER TABLE ONLY public.menu_item DROP CONSTRAINT menu_item_pkey;
       public            postgres    false    232            `           2606    50148 "   modifier_group modifier_group_pkey 
   CONSTRAINT     o   ALTER TABLE ONLY public.modifier_group
    ADD CONSTRAINT modifier_group_pkey PRIMARY KEY (modifier_group_id);
 L   ALTER TABLE ONLY public.modifier_group DROP CONSTRAINT modifier_group_pkey;
       public            postgres    false    234            b           2606    50150 "   modifiers_item modifiers_item_pkey 
   CONSTRAINT     i   ALTER TABLE ONLY public.modifiers_item
    ADD CONSTRAINT modifiers_item_pkey PRIMARY KEY (modifier_id);
 L   ALTER TABLE ONLY public.modifiers_item DROP CONSTRAINT modifiers_item_pkey;
       public            postgres    false    236            f           2606    50152 ,   order_table_mapping order_table_mapping_pkey 
   CONSTRAINT     w   ALTER TABLE ONLY public.order_table_mapping
    ADD CONSTRAINT order_table_mapping_pkey PRIMARY KEY (order_detail_id);
 V   ALTER TABLE ONLY public.order_table_mapping DROP CONSTRAINT order_table_mapping_pkey;
       public            postgres    false    239            h           2606    50154 (   order_tax_mapping order_tax_mapping_pkey 
   CONSTRAINT     p   ALTER TABLE ONLY public.order_tax_mapping
    ADD CONSTRAINT order_tax_mapping_pkey PRIMARY KEY (order_tax_id);
 R   ALTER TABLE ONLY public.order_tax_mapping DROP CONSTRAINT order_tax_mapping_pkey;
       public            postgres    false    241            j           2606    50156 0   ordered_item_modifier ordered_item_modifier_pkey 
   CONSTRAINT     �   ALTER TABLE ONLY public.ordered_item_modifier
    ADD CONSTRAINT ordered_item_modifier_pkey PRIMARY KEY (ordered_item_modifier_id);
 Z   ALTER TABLE ONLY public.ordered_item_modifier DROP CONSTRAINT ordered_item_modifier_pkey;
       public            postgres    false    243            d           2606    50158    order ordered_pkey 
   CONSTRAINT     X   ALTER TABLE ONLY public."order"
    ADD CONSTRAINT ordered_pkey PRIMARY KEY (order_id);
 >   ALTER TABLE ONLY public."order" DROP CONSTRAINT ordered_pkey;
       public            postgres    false    238            l           2606    50160    payment payment_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.payment
    ADD CONSTRAINT payment_pkey PRIMARY KEY (payment_id);
 >   ALTER TABLE ONLY public.payment DROP CONSTRAINT payment_pkey;
       public            postgres    false    246            n           2606    50162 &   permission_table permission_table_pkey 
   CONSTRAINT     o   ALTER TABLE ONLY public.permission_table
    ADD CONSTRAINT permission_table_pkey PRIMARY KEY (permission_id);
 P   ALTER TABLE ONLY public.permission_table DROP CONSTRAINT permission_table_pkey;
       public            postgres    false    248            p           2606    50164 0   role_permission_table role_permission_table_pkey 
   CONSTRAINT     ~   ALTER TABLE ONLY public.role_permission_table
    ADD CONSTRAINT role_permission_table_pkey PRIMARY KEY (role_permission_id);
 Z   ALTER TABLE ONLY public.role_permission_table DROP CONSTRAINT role_permission_table_pkey;
       public            postgres    false    250            r           2606    50166    roles roles_pkey 
   CONSTRAINT     S   ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (role_id);
 :   ALTER TABLE ONLY public.roles DROP CONSTRAINT roles_pkey;
       public            postgres    false    252            t           2606    50168    section section_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.section
    ADD CONSTRAINT section_pkey PRIMARY KEY (section_id);
 >   ALTER TABLE ONLY public.section DROP CONSTRAINT section_pkey;
       public            postgres    false    254            v           2606    50170    states states_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.states
    ADD CONSTRAINT states_pkey PRIMARY KEY (state_id);
 <   ALTER TABLE ONLY public.states DROP CONSTRAINT states_pkey;
       public            postgres    false    256            x           2606    50172    tables tables_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.tables
    ADD CONSTRAINT tables_pkey PRIMARY KEY (table_id);
 <   ALTER TABLE ONLY public.tables DROP CONSTRAINT tables_pkey;
       public            postgres    false    258            z           2606    50174 "   taxes_and_fees taxes_and_fees_pkey 
   CONSTRAINT     d   ALTER TABLE ONLY public.taxes_and_fees
    ADD CONSTRAINT taxes_and_fees_pkey PRIMARY KEY (tax_id);
 L   ALTER TABLE ONLY public.taxes_and_fees DROP CONSTRAINT taxes_and_fees_pkey;
       public            postgres    false    260            |           2606    50176    unit unit_pkey 
   CONSTRAINT     Q   ALTER TABLE ONLY public.unit
    ADD CONSTRAINT unit_pkey PRIMARY KEY (unit_id);
 8   ALTER TABLE ONLY public.unit DROP CONSTRAINT unit_pkey;
       public            postgres    false    262            ~           2606    50178    users users_pkey 
   CONSTRAINT     S   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);
 :   ALTER TABLE ONLY public.users DROP CONSTRAINT users_pkey;
       public            postgres    false    264            �           2606    50180 ,   waiting_token_codes waiting_token_codes_pkey 
   CONSTRAINT     p   ALTER TABLE ONLY public.waiting_token_codes
    ADD CONSTRAINT waiting_token_codes_pkey PRIMARY KEY (token_id);
 V   ALTER TABLE ONLY public.waiting_token_codes DROP CONSTRAINT waiting_token_codes_pkey;
       public            postgres    false    266            �           2606    50181 #   OrderDetail OrderDetail_ItemId_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public."OrderDetail"
    ADD CONSTRAINT "OrderDetail_ItemId_fkey" FOREIGN KEY ("ItemId") REFERENCES public.menu_item(item_id) NOT VALID;
 Q   ALTER TABLE ONLY public."OrderDetail" DROP CONSTRAINT "OrderDetail_ItemId_fkey";
       public          postgres    false    4958    232    215            �           2606    50186 %   OrderDetail OrderDetail_order_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public."OrderDetail"
    ADD CONSTRAINT "OrderDetail_order_id_fkey" FOREIGN KEY (order_id) REFERENCES public."order"(order_id) NOT VALID;
 S   ALTER TABLE ONLY public."OrderDetail" DROP CONSTRAINT "OrderDetail_order_id_fkey";
       public          postgres    false    215    238    4964            �           2606    50191 !   category category_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.category
    ADD CONSTRAINT category_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 K   ALTER TABLE ONLY public.category DROP CONSTRAINT category_created_by_fkey;
       public          postgres    false    264    4990    217            �           2606    50196 "   category category_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.category
    ADD CONSTRAINT category_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 L   ALTER TABLE ONLY public.category DROP CONSTRAINT category_modified_by_fkey;
       public          postgres    false    4990    217    264            �           2606    50201    cities cities_state_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.cities
    ADD CONSTRAINT cities_state_id_fkey FOREIGN KEY (state_id) REFERENCES public.states(state_id);
 E   ALTER TABLE ONLY public.cities DROP CONSTRAINT cities_state_id_fkey;
       public          postgres    false    4982    219    256            �           2606    50206 #   customers customers_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.customers
    ADD CONSTRAINT customers_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 M   ALTER TABLE ONLY public.customers DROP CONSTRAINT customers_created_by_fkey;
       public          postgres    false    223    264    4990            �           2606    50211 $   customers customers_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.customers
    ADD CONSTRAINT customers_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 N   ALTER TABLE ONLY public.customers DROP CONSTRAINT customers_modified_by_fkey;
       public          postgres    false    4990    223    264            �           2606    50216 "   feedback feedback_Customer_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.feedback
    ADD CONSTRAINT "feedback_Customer_id_fkey" FOREIGN KEY ("Customer_id") REFERENCES public.customers(customer_id) NOT VALID;
 N   ALTER TABLE ONLY public.feedback DROP CONSTRAINT "feedback_Customer_id_fkey";
       public          postgres    false    225    4948    223            �           2606    50221    feedback feedback_Order_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.feedback
    ADD CONSTRAINT "feedback_Order_id_fkey" FOREIGN KEY ("Order_id") REFERENCES public."order"(order_id) NOT VALID;
 K   ALTER TABLE ONLY public.feedback DROP CONSTRAINT "feedback_Order_id_fkey";
       public          postgres    false    4964    238    225            �           2606    50226 !   feedback feedback_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.feedback
    ADD CONSTRAINT feedback_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 K   ALTER TABLE ONLY public.feedback DROP CONSTRAINT feedback_created_by_fkey;
       public          postgres    false    264    4990    225            �           2606    50231 "   feedback feedback_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.feedback
    ADD CONSTRAINT feedback_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 L   ALTER TABLE ONLY public.feedback DROP CONSTRAINT feedback_modified_by_fkey;
       public          postgres    false    264    225    4990            �           2606    50236    invoice invoice_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.invoice
    ADD CONSTRAINT invoice_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 I   ALTER TABLE ONLY public.invoice DROP CONSTRAINT invoice_created_by_fkey;
       public          postgres    false    4990    264    227            �           2606    50241     invoice invoice_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.invoice
    ADD CONSTRAINT invoice_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 J   ALTER TABLE ONLY public.invoice DROP CONSTRAINT invoice_modified_by_fkey;
       public          postgres    false    4990    227    264            �           2606    50246    invoice invoice_order_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.invoice
    ADD CONSTRAINT invoice_order_id_fkey FOREIGN KEY (order_id) REFERENCES public."order"(order_id) NOT VALID;
 G   ALTER TABLE ONLY public.invoice DROP CONSTRAINT invoice_order_id_fkey;
       public          postgres    false    4964    227    238            �           2606    50251 L   mapping_item_modifierGroup mapping_item_modifierGroup_Modifier_group_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public."mapping_item_modifierGroup"
    ADD CONSTRAINT "mapping_item_modifierGroup_Modifier_group_id_fkey" FOREIGN KEY ("Modifier_group_id") REFERENCES public.modifier_group(modifier_group_id) NOT VALID;
 z   ALTER TABLE ONLY public."mapping_item_modifierGroup" DROP CONSTRAINT "mapping_item_modifierGroup_Modifier_group_id_fkey";
       public          postgres    false    4960    234    229            �           2606    50256 B   mapping_item_modifierGroup mapping_item_modifierGroup_item_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public."mapping_item_modifierGroup"
    ADD CONSTRAINT "mapping_item_modifierGroup_item_id_fkey" FOREIGN KEY (item_id) REFERENCES public.menu_item(item_id);
 p   ALTER TABLE ONLY public."mapping_item_modifierGroup" DROP CONSTRAINT "mapping_item_modifierGroup_item_id_fkey";
       public          postgres    false    4958    232    229            �           2606    50261 D   mapping_modifier_modifiergroup mapping_item_modifier_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.mapping_modifier_modifiergroup
    ADD CONSTRAINT mapping_item_modifier_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 n   ALTER TABLE ONLY public.mapping_modifier_modifiergroup DROP CONSTRAINT mapping_item_modifier_created_by_fkey;
       public          postgres    false    4990    264    231            �           2606    50266 E   mapping_modifier_modifiergroup mapping_item_modifier_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.mapping_modifier_modifiergroup
    ADD CONSTRAINT mapping_item_modifier_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 o   ALTER TABLE ONLY public.mapping_modifier_modifiergroup DROP CONSTRAINT mapping_item_modifier_modified_by_fkey;
       public          postgres    false    4990    264    231            �           2606    50271 K   mapping_modifier_modifiergroup mapping_item_modifier_modifier_group_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.mapping_modifier_modifiergroup
    ADD CONSTRAINT mapping_item_modifier_modifier_group_id_fkey FOREIGN KEY (modifier_group_id) REFERENCES public.modifier_group(modifier_group_id) NOT VALID;
 u   ALTER TABLE ONLY public.mapping_modifier_modifiergroup DROP CONSTRAINT mapping_item_modifier_modifier_group_id_fkey;
       public          postgres    false    4960    234    231            �           2606    50276 E   mapping_modifier_modifiergroup mapping_item_modifier_modifier_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.mapping_modifier_modifiergroup
    ADD CONSTRAINT mapping_item_modifier_modifier_id_fkey FOREIGN KEY (modifier_id) REFERENCES public.modifiers_item(modifier_id) NOT VALID;
 o   ALTER TABLE ONLY public.mapping_modifier_modifiergroup DROP CONSTRAINT mapping_item_modifier_modifier_id_fkey;
       public          postgres    false    4962    236    231            �           2606    50281 $   menu_item menu_item_category_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.menu_item
    ADD CONSTRAINT menu_item_category_id_fkey FOREIGN KEY (category_id) REFERENCES public.category(category_id);
 N   ALTER TABLE ONLY public.menu_item DROP CONSTRAINT menu_item_category_id_fkey;
       public          postgres    false    232    4940    217            �           2606    50286 #   menu_item menu_item_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.menu_item
    ADD CONSTRAINT menu_item_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 M   ALTER TABLE ONLY public.menu_item DROP CONSTRAINT menu_item_created_by_fkey;
       public          postgres    false    232    4990    264            �           2606    50291 $   menu_item menu_item_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.menu_item
    ADD CONSTRAINT menu_item_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 N   ALTER TABLE ONLY public.menu_item DROP CONSTRAINT menu_item_modified_by_fkey;
       public          postgres    false    232    4990    264            �           2606    50296     menu_item menu_item_unit_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.menu_item
    ADD CONSTRAINT menu_item_unit_id_fkey FOREIGN KEY (unit_id) REFERENCES public.unit(unit_id) NOT VALID;
 J   ALTER TABLE ONLY public.menu_item DROP CONSTRAINT menu_item_unit_id_fkey;
       public          postgres    false    4988    262    232            �           2606    50301 -   modifier_group modifier_group_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.modifier_group
    ADD CONSTRAINT modifier_group_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 W   ALTER TABLE ONLY public.modifier_group DROP CONSTRAINT modifier_group_created_by_fkey;
       public          postgres    false    4990    264    234            �           2606    50306 .   modifier_group modifier_group_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.modifier_group
    ADD CONSTRAINT modifier_group_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 X   ALTER TABLE ONLY public.modifier_group DROP CONSTRAINT modifier_group_modified_by_fkey;
       public          postgres    false    234    4990    264            �           2606    50311 -   modifiers_item modifiers_item_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.modifiers_item
    ADD CONSTRAINT modifiers_item_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 W   ALTER TABLE ONLY public.modifiers_item DROP CONSTRAINT modifiers_item_created_by_fkey;
       public          postgres    false    4990    236    264            �           2606    50316 .   modifiers_item modifiers_item_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.modifiers_item
    ADD CONSTRAINT modifiers_item_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 X   ALTER TABLE ONLY public.modifiers_item DROP CONSTRAINT modifiers_item_modified_by_fkey;
       public          postgres    false    264    4990    236            �           2606    50321 *   modifiers_item modifiers_item_unit_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.modifiers_item
    ADD CONSTRAINT modifiers_item_unit_id_fkey FOREIGN KEY (unit_id) REFERENCES public.unit(unit_id);
 T   ALTER TABLE ONLY public.modifiers_item DROP CONSTRAINT modifiers_item_unit_id_fkey;
       public          postgres    false    4988    236    262            �           2606    50326 5   order_table_mapping order_table_mapping_order_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.order_table_mapping
    ADD CONSTRAINT order_table_mapping_order_id_fkey FOREIGN KEY (order_id) REFERENCES public."order"(order_id);
 _   ALTER TABLE ONLY public.order_table_mapping DROP CONSTRAINT order_table_mapping_order_id_fkey;
       public          postgres    false    238    4964    239            �           2606    50331 5   order_table_mapping order_table_mapping_table_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.order_table_mapping
    ADD CONSTRAINT order_table_mapping_table_id_fkey FOREIGN KEY (table_id) REFERENCES public.tables(table_id);
 _   ALTER TABLE ONLY public.order_table_mapping DROP CONSTRAINT order_table_mapping_table_id_fkey;
       public          postgres    false    258    239    4984            �           2606    50336 1   order_tax_mapping order_tax_mapping_order_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.order_tax_mapping
    ADD CONSTRAINT order_tax_mapping_order_id_fkey FOREIGN KEY (order_id) REFERENCES public."order"(order_id);
 [   ALTER TABLE ONLY public.order_tax_mapping DROP CONSTRAINT order_tax_mapping_order_id_fkey;
       public          postgres    false    241    238    4964            �           2606    50341 /   order_tax_mapping order_tax_mapping_tax_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.order_tax_mapping
    ADD CONSTRAINT order_tax_mapping_tax_id_fkey FOREIGN KEY (tax_id) REFERENCES public.taxes_and_fees(tax_id);
 Y   ALTER TABLE ONLY public.order_tax_mapping DROP CONSTRAINT order_tax_mapping_tax_id_fkey;
       public          postgres    false    4986    241    260            �           2606    50346    order ordered_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public."order"
    ADD CONSTRAINT ordered_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 I   ALTER TABLE ONLY public."order" DROP CONSTRAINT ordered_created_by_fkey;
       public          postgres    false    238    4990    264            �           2606    50351    order ordered_customer_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public."order"
    ADD CONSTRAINT ordered_customer_id_fkey FOREIGN KEY (customer_id) REFERENCES public.customers(customer_id);
 J   ALTER TABLE ONLY public."order" DROP CONSTRAINT ordered_customer_id_fkey;
       public          postgres    false    238    4948    223            �           2606    50356 <   ordered_item_modifier ordered_item_modifier_modifier_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.ordered_item_modifier
    ADD CONSTRAINT ordered_item_modifier_modifier_id_fkey FOREIGN KEY (modifier_id) REFERENCES public.modifiers_item(modifier_id) NOT VALID;
 f   ALTER TABLE ONLY public.ordered_item_modifier DROP CONSTRAINT ordered_item_modifier_modifier_id_fkey;
       public          postgres    false    243    4962    236            �           2606    50361 <   ordered_item_modifier ordered_item_modifier_orderitemid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.ordered_item_modifier
    ADD CONSTRAINT ordered_item_modifier_orderitemid_fkey FOREIGN KEY (orderitemid) REFERENCES public."OrderDetail"("OrderDetailID") NOT VALID;
 f   ALTER TABLE ONLY public.ordered_item_modifier DROP CONSTRAINT ordered_item_modifier_orderitemid_fkey;
       public          postgres    false    4938    243    215            �           2606    50366    order ordered_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public."order"
    ADD CONSTRAINT ordered_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 J   ALTER TABLE ONLY public."order" DROP CONSTRAINT ordered_modified_by_fkey;
       public          postgres    false    238    4990    264            �           2606    50371    payment payment_order_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.payment
    ADD CONSTRAINT payment_order_id_fkey FOREIGN KEY (order_id) REFERENCES public."order"(order_id) NOT VALID;
 G   ALTER TABLE ONLY public.payment DROP CONSTRAINT payment_order_id_fkey;
       public          postgres    false    246    4964    238            �           2606    50376 1   permission_table permission_table_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.permission_table
    ADD CONSTRAINT permission_table_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 [   ALTER TABLE ONLY public.permission_table DROP CONSTRAINT permission_table_created_by_fkey;
       public          postgres    false    248    4990    264            �           2606    50381 2   permission_table permission_table_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.permission_table
    ADD CONSTRAINT permission_table_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 \   ALTER TABLE ONLY public.permission_table DROP CONSTRAINT permission_table_modified_by_fkey;
       public          postgres    false    248    4990    264            �           2606    50386 ;   role_permission_table role_permission_table_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.role_permission_table
    ADD CONSTRAINT role_permission_table_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 e   ALTER TABLE ONLY public.role_permission_table DROP CONSTRAINT role_permission_table_created_by_fkey;
       public          postgres    false    4990    250    264            �           2606    50391 6   role_permission_table role_permission_table_roles_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.role_permission_table
    ADD CONSTRAINT role_permission_table_roles_fkey FOREIGN KEY (roles) REFERENCES public.roles(role_id);
 `   ALTER TABLE ONLY public.role_permission_table DROP CONSTRAINT role_permission_table_roles_fkey;
       public          postgres    false    250    4978    252            �           2606    50396    section section_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.section
    ADD CONSTRAINT section_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 I   ALTER TABLE ONLY public.section DROP CONSTRAINT section_created_by_fkey;
       public          postgres    false    264    4990    254            �           2606    50401     section section_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.section
    ADD CONSTRAINT section_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 J   ALTER TABLE ONLY public.section DROP CONSTRAINT section_modified_by_fkey;
       public          postgres    false    264    4990    254            �           2606    50406    states states_country_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.states
    ADD CONSTRAINT states_country_id_fkey FOREIGN KEY (country_id) REFERENCES public.countries(country_id);
 G   ALTER TABLE ONLY public.states DROP CONSTRAINT states_country_id_fkey;
       public          postgres    false    256    4944    221            �           2606    50411    tables tables_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.tables
    ADD CONSTRAINT tables_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 G   ALTER TABLE ONLY public.tables DROP CONSTRAINT tables_created_by_fkey;
       public          postgres    false    264    4990    258            �           2606    50416    tables tables_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.tables
    ADD CONSTRAINT tables_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 H   ALTER TABLE ONLY public.tables DROP CONSTRAINT tables_modified_by_fkey;
       public          postgres    false    264    258    4990            �           2606    50421    tables tables_section_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.tables
    ADD CONSTRAINT tables_section_id_fkey FOREIGN KEY (section_id) REFERENCES public.section(section_id);
 G   ALTER TABLE ONLY public.tables DROP CONSTRAINT tables_section_id_fkey;
       public          postgres    false    4980    254    258            �           2606    50426 -   taxes_and_fees taxes_and_fees_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.taxes_and_fees
    ADD CONSTRAINT taxes_and_fees_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 W   ALTER TABLE ONLY public.taxes_and_fees DROP CONSTRAINT taxes_and_fees_created_by_fkey;
       public          postgres    false    260    4990    264            �           2606    50431 .   taxes_and_fees taxes_and_fees_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.taxes_and_fees
    ADD CONSTRAINT taxes_and_fees_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 X   ALTER TABLE ONLY public.taxes_and_fees DROP CONSTRAINT taxes_and_fees_modified_by_fkey;
       public          postgres    false    264    4990    260            �           2606    50436    users users_city_name_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_city_name_fkey FOREIGN KEY (city_name) REFERENCES public.cities(city_id);
 D   ALTER TABLE ONLY public.users DROP CONSTRAINT users_city_name_fkey;
       public          postgres    false    219    264    4942            �           2606    50441    users users_country_name_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_country_name_fkey FOREIGN KEY (country_name) REFERENCES public.countries(country_id);
 G   ALTER TABLE ONLY public.users DROP CONSTRAINT users_country_name_fkey;
       public          postgres    false    4944    264    221            �           2606    50446    users users_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 E   ALTER TABLE ONLY public.users DROP CONSTRAINT users_created_by_fkey;
       public          postgres    false    264    4990    264            �           2606    50451    users users_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 F   ALTER TABLE ONLY public.users DROP CONSTRAINT users_modified_by_fkey;
       public          postgres    false    4990    264    264            �           2606    50456    users users_roles_fkey    FK CONSTRAINT     x   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_roles_fkey FOREIGN KEY (roles) REFERENCES public.roles(role_id);
 @   ALTER TABLE ONLY public.users DROP CONSTRAINT users_roles_fkey;
       public          postgres    false    252    4978    264            �           2606    50461    users users_state_name_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_state_name_fkey FOREIGN KEY (state_name) REFERENCES public.states(state_id);
 E   ALTER TABLE ONLY public.users DROP CONSTRAINT users_state_name_fkey;
       public          postgres    false    264    256    4982            �           2606    50466 6   waiting_token_codes waiting_token_codes_SectionId_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.waiting_token_codes
    ADD CONSTRAINT "waiting_token_codes_SectionId_fkey" FOREIGN KEY ("SectionId") REFERENCES public.section(section_id) NOT VALID;
 b   ALTER TABLE ONLY public.waiting_token_codes DROP CONSTRAINT "waiting_token_codes_SectionId_fkey";
       public          postgres    false    266    4980    254            �           2606    50471 7   waiting_token_codes waiting_token_codes_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.waiting_token_codes
    ADD CONSTRAINT waiting_token_codes_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 a   ALTER TABLE ONLY public.waiting_token_codes DROP CONSTRAINT waiting_token_codes_created_by_fkey;
       public          postgres    false    266    4990    264            �           2606    50476 8   waiting_token_codes waiting_token_codes_modified_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.waiting_token_codes
    ADD CONSTRAINT waiting_token_codes_modified_by_fkey FOREIGN KEY (modified_by) REFERENCES public.users(user_id);
 b   ALTER TABLE ONLY public.waiting_token_codes DROP CONSTRAINT waiting_token_codes_modified_by_fkey;
       public          postgres    false    264    4990    266            L   �  x�m�;n1Dk�)r�@�W��R�	�����ѮFv��xK���X](�Is�y����G����0�ϑ��ѻ]:�RrA#]���P�+a%f=1�(	�Em�ʐ��T�*�`N)-N�~g�&%�!��4�ͧ{�U��JxR�u�(��B�%��3t�v�i��"*+;Ր���_�Jq!A�7�OT�b�n�U"����t*�F�e^��z�~K_R�~I�%FT]h�E1��P��@hm槼,T0�첢]���������~��!��YA�u,�G2�ή�1�c�=E���P~J,��!��_Q�V'L�ȿ�fC�F}e-�n%!*��V)��yG� �u���h&6�aݐ�e �u6��4�HoB�fCc���:N�._�1��ׂz����u��85��ޕyFR0��lJ����5��a�P�'**'*�O�zx�� _�WvC�-���c�e%�-d��z�`N      N   �  x�}��n�6���S���ԶYt�b��l��f9vRY��y�Jǲl�L�}:<����s����|f�_����uLp�W\��M��@g`R��9�~~��X�}}�/۷��~$��)y��d�9�넋L��Cj�TQF���u�)޷����a��_޿�fH����䙲���Ǥ�a� ��bB��%��2�R!��2&J�o����~m�tű�gH�	�D��9P�Eʦf����=˝JdZd�a���*&��B��%/�\e�Y�jc�71���>��.mWmp:"Q*ɕ�I�@~-p�Ώ��K9&HfڦF�0.&Ql�b��9$ ��=��𙔩Up-idJ����c�����?����#<�wB�:��4B�����z���5j:V�T�T	I�b�ϫM�n�S̟ӝ[�B�
klkH5P&&�8US{m**��DT<5N��1�r��2���9��#�1�X4�!ƶl�ꁧ2�3�S��.&Ѣ1�Cll�K�O���c �&���P�f��WǶ>T�d��S|V*�%�$�{G��P\����~�m��NO��"����n17���k��kꅣ���90^�$ʈ���t�*�zR�D07��k��-
"�<�!�kc�$"�)9��dʥ�:-"
���Q+
l~�T纙�jC��T)��5��f.���,Y_um��3C�,n���R8��JS�~X��ׇ9҇����
kg��D�AU��ݽϦT��
��75ޜ6���'��X�^���S蔸�Tz\�$�[W�����Z,��4i҈�D������ݶ_��>X�%b��_]mw'�=}�z����q�9f��)ħA3���nʷ�n�w�o˺��}���M0��.���IĶ���W�/E�t�y�;��Ub-�)��jX�4���f��������W��Y^�X�7G��6#�S�C�\��|jgz% 1���,�<��$�Bt���[OV6\��)!�"��h�(H5�ɲ8?����NBB�c�<��KP���l�ia^=f*u�Ώ�O$���S:ܽ��6����r�I3�.��[m�r�.�G��=Fv'L�M=����Y����¾兎(Ĳ��YYal�U��h<}:&�0ܥ.Z���/H�\�D$��ʏ;���	{	R�k3��'�k��*��~���<���I���N��P�as1���}�� c�+.�G�}O���%:>s��2R~�///�e�l�      P   S   x�3�4�t�H,�KLO,�4202�50�52V02�24�2��3�0042�#�e4�1#571%1)1�LC����'�ddR�=... �,P      R   9   x�3���K�L�4202�50�52V02�20�20�323�45�#�e��H��=... W�p      T   �  x����r�6���S��!�K�mO�$�N���(�mɟ�����r/�z�3�	~" ���i�Ի�lSoj����-�u�����p68o�hߟ(0����)�U�	H�r�#ֈ͹�v���4g�z6&�B"������z;��D?&0�u3�Ja�����|NJ�0�>��sw��ct&��r9i�����Fƕ}H�:�����[����G�|��8�g��	���`�،� �V�w���S�y9�bu��dЁ-��节TƱ7rR�踏]�Q�O�j��
{����Y����4���m�V�0x��U�椄�!�֛�۔Rl��B���ZJ�l�C_��A4�g���,8��Ĉ���|}YK!�1Pd=~�\K	b�b�s~��v�k�-6��q>�ѪB-T@���]
�hV��DfK�� I;�0'%��������9_�4��n�I	��Ј�:L�PID�p�OR���F���+AL&�����H	t}��}��KA#�Y�('%\�t�6�m��q}�.�:'%^����[M��dϲIi�(���k)��
�t����K�������{���R�.Q�"�b�Ĝ�Z<u������C|Fd�>'%𐱊s�j*.v���QF����H	��y��D�k	�@�ޒf���� $��i��y�5��6�my�rRJ1#����;q��tz���p��$�N��� V���(�>��
`b�j�U�$o�79)ÿ�>"b[���Q��HΫ���4`����O�[�.^>~L�J�S���"}�����Ĥ��:�<�O�f}��J�%��+%�ُY����N{��^��|,�.m�mm/���h��҆\_�b,ҏ�=����O΀�IiO!�vܴ�ɏ�"-G��N����ۭĮp���?,����YrR�^��5݆Kh39���ʸ����4O29)A֦������������aU�7���9���6�$k_���:�('���B��B;5Q�Gб���o2<�Y�I#G����ۮ��;������4��Ҁ�ͥ՛���/��*�|a�Vٜ4��kY-iz6y$�GM!'�8#��sY�u=�F��V��0H�{����H����}�;θ;�K��o�v����OH�'G��b�lN��ׇ�σ8��~o���X���,�;����W�㒯x���\:��ɐS����QF��U5���d+�5t�N�{�=e`|��mG�g      V   9  x���Mn�0���)|�
�)]��� ��tv����J��qm
I�}y|������ׯD@��F�5�= Ϥ�G0����c�D?'vȕȁ�1����xd%l79��I7��Ϣ��/��7�&��:T�I�/���?��DE8C�2�.4�$>xv0g�E@�ݪa�I�ք6pU���}��+`cm*وEq&]��v%\j�;��fn���*3���� $9t��@�C$�F�>��=J*�|���W�Bc��,�̤�H\��/��s�ihXK�e�Lz�����ȋ����KQ-e�k�Ń���A��P��"6��S�8˃C�sA�(|H$/3idHI�5���`���	�Ug�~�_��r1��>�/`���^�Q����I�A��'/xR�8@��4x1~���O-W���3i�b��Ƌ�~�+}e��2��D��J�������M�I��5�3�Ġ�l�7�(=	����������L�
�T�	䇽��I��.C�ՙ�!%;g(�Lޓ�H�2Za�t �=�}����[fc#�I�Զ�z�˲�2B�m      X   Q   x����� k{�, z"3��	I�����t@(B΂(�ѣ��Jz����X��43�L�_ k˸6�X!�UUo� &�      Z   G  x�M�ˍ�@D�8�U�s���X���,�����8���Czqq8=w�8r��w��^33��w����A5��\)E|�$	ua�ևD:��]�r��x<�+���5��0�
�|�;�
�&R�E)C�A�ʋUVW����+����E�ES�D��}�'7l�3"젒3��&:۪pl�V��:�A��9�yf�����_��v�F۾�Q:���݄����]�vm����`��r��(�Fm�嚃cm	�k�!9>{e(��	�d[e{�������Uƍr[c,^���s'�9u����aP�5�As�m�SLD�q����ߟ�� �T      \   V  x�}��m!E�g�HA�~���T�ߩ �+������ל��1fP���'�'�Ќ���P����k,6�7��1e�
S��5�%9�H١0���k���in���b���ey���"m��C!�Ջ�g�*��BdS�7m�Կ��g;�0��I�(뽩4 9)����"��TT��Dȥ�M:�)*�ԇ�vK#zܲ���P�x��׸yo���1��0I��ָ6�Tb/�>�0��d�:��D�TP����B�n;��<��(���G�.��8@�6�w(Leܜӵɍ��[P�<L�fc�$YL}��T�4�Ӿ2�}������e��.EZ;���������	�      ]   j  x��ZIo�F>W�
�2����i�&� =$3�6`�7I��rG���E�[�L{�֡����^���cH�%�f�!Z�Yv�P^��|�E֢�&��MDd�E��.B��g�Α1H�ߒ��[�(QY����~�[$�rw�cI��?�7�dy|x����$�c���|��W߿'�	e���Y��ɡJ�(�@5Gܢ����}I�u�~/�c��'&-�s�G�Q3��\�tѧe�<�]z����Ħ���DB���]�(�\�`����1���Xsn�AW�)�����}�C^&bb��w���Ev8�pxR�X����Xp��,�܄��3H�~�/\Ĺ�\(�����59�G���^ad����ߕ&�)8g!�z���h�LL)����c�X���ј*L�T���%�sV�=��TMxᧆ�LD�`�b��T�)�����O�}��C+TL	��X�B��e(<�p>Ι�WS�?g�ͮ�>mv7���O����׏_?��FG�Ŝ�LaJ��*]��}8���C�4����{���a�&RZ���Y��ltԖ#1���S}�йZo�8�%m1C2+�$�!��5N���Ȏ��j[m���l{���'%tT_V�z}^��g�q(�0��U�ER/ԥ�wW����؏�<��*�	��,�k������?=���D�b�J����YYG��g��tYD�vy�ZW�,���C�նJ�|��>�������Q2L5�B4�������2Y'�i���m�$V�bCP�{�e�#�֒?��t���~�ån%���,%�&�������5z��1�ʰ��#h����p��ԟ�H,HL%dh%�	AW���v�뜏E��&�N�%���ŊnX��evZ�ӧ�'Omv-�;Ehb��!h�Sj&���{9���B3o���A3���N{�e���.Qa	- 3XXjѹ�C=�g%�6x4P\z��:�F�:N��&F&�ǒ����0�`��6&�1��XOWO��BY�Z��l �C��1������i�5������BP���τ��I���P�j�?���r��ԕJm��!h9�6-y�$�6Z�O��Gk`��Zr�˦�������Y����t�Q��N��t(�M���O`H^R�4GY�H�͖�`Ȱrq��'0^���o�=�^=P=_�E-y=f�7���,/BP���^L���2_�O��[(Rʄ�Z+2=�zUnaB����u�V*�^��!��[[��s�N�sҔF*\f�D�U/�Q���^i�/�7��%[��ψ���j�Y��P_9�JI�BP�X`�����Q?r�L;��H�y���d� ��b4���d  �L������+S�q��_��U*�BP7���kI�J����C��J)�"��!�o%�$�k�Bsh�[i-U��k� �C+�F�1�R�����!U���Ƿ�y�F�XH��!�[�&�=MQyZU���+u�4���u�����j��!��ٜ�N{z+Fb)]C,aZ��q!x%�]�zm�#�t����z)��1/��oǋ����4{p֌�P�C���M۰�{�q��E,	����&/�#����/��v4n�C�n:@g��g0~,'B����~s�@5uů�C_�T]V���x��B-���Pu�gj�b&����Z8�[��S��g%�������v����s�Ț$���fBь����^��{�k���o4"LeYS�ǁ����O������^����.'/܊�9����c���~r9m��V�m�V��L�N���ए�Yx�$�)w�^i�A�Q�$����H��Te�z�Sf���׸��I���	�4��_Q�z�t�����lzS�E�"o:rN%�憘 �KF��|[�'W}�˫�k��p��a�������Ⅾ~@܃.��4���Y&�r�G� �L�>4�Z��鱉5�N<�\yM��|5��A��W�>&y�\�d�Xg[_������-4����uZ���"S�L��M��wۻQkvp?�/�� ]l�郹	�[-�
A�M�d*d���Կ܊U��x�>��'ٱv�)v��(�_�����(��h&h�	�v��f��n�m�mwyQg�iU"�TB��!��ޟx�7�<Y���&�^      _     x����r�8���S�,Fb��Ĝ�2}莾��Rq-��f�R��~�J
\uD�*��/~�\@�%��ajX?�e�&@���g`
a.r0V�����?q��4v�mÅ5K�B���֤�;�q�5ݙ�U��,��E&D�}!D�Ҧ$b	v����"nE���E.�!Sҝ&8{�۪-{�[Ε�L��C>�^Kp)���~�<?�����{L����h�HIw&�w�.K���*�eJ"��g,78��� G#q�R�L
��
�seA+���p�&X?v��Ԛ��2\E�U�gSy�fS;��iߕU�9��wF��7������p���G'n����-X���c��u��%����zy.3�Cq(��
͕{��)��0g���������H:%OD��r��Ou���{͵�)��*���X6D|.WB�h�w$"��t��v��`�q_r�Sc��a�i�B]�+��Ⱥ#Բ�;#N��,t��0�͔c�X.�4����J�
���p!�G����s�~�2e�$�
���Kw��xWׯ��
�wye`�,��<Zh=N[_!�F>%P�f<g��K��Tq�ީ��DP��4�DƘ�ri��B�xS��9�<�:$ߐ0��x�[����J_N�a5a�8L!&7�(+R��(1�F��j���2x������i#f�2��gK%��6%�E&\$��	������%�,�=>^�%~���E�s�KIw������s x���?��P�9g�Լ#ND�?^�xJ�]��D��08�ôk8���*���0���X��e���[�à$ֵ�B�����Զ��mN�adB!������S��i5H�a�EJ[�Sq���짡��R�5��V˔DX�����s����P� �MiaSAuT%��(�-����z���j��ƈk���f���ȳ�+~������������������2����^η��<��߲p���v*��T6�~��j�!�zk��)���"��
b=`U�DJ"Cr:I��gӅB!�m��MK�s}\���Ֆ���wGJ"",V緦�f��fѵl�4���"[��h���o|{��}o�!�8��im��t'`U�v}�'�Ͳ��h,���}_�	�����qrv���` ���j�@2������q�9)S���4�ˡ�%��bW	�pf�1�]�/ev�z�z r���r��=�.�a�ˆe�5	���#������]J"��hB��OXOy!݁>Z9h�*%�� �4Ӝ���s���]�s�H�MJ"��ph>��عF�S�>�I��Rn���b�p�CܭY�f�*�B��-s�vy�V�)X� ���7�\Ht���.Q��=�|҇��c���X�$bIv�bP�bd��-N�����ϧ���4k/S{a��46�e�s�3��e�t�+ϰ�ۗ/ϟ���O�_�P��� �9�R-���\���\G�.t�z��X&�$�9VW����vS�ɱZ�s�ߕ��q)����}��d��q�.$rC�~9g��x,=��>��8�\J��<���k�nq�-3 Lgؾ�V)�*%�c"onKP�wh�v�u��)�h�������{u����>w%�iv.__� @���M�_7b>6ܕ�$Y�p���7���76�Ҥ$�)ֵ��cݡ=�F�#`XSz3�H!�a�kqL�3�H"���Zr��"Y6��>]��	�2���R!�c��c�2T�I�ӝ�VX����Yw�ܡ��^��i�I���K��h#y��~�뀾�X� ��a�дJ�1C�$��0W�Ǧ:6ut�A��uh�0ܦ�{���֦/�u|Y�@�L��7ڭVy%Q�碒f�r���9א.�NF[��Hd\w�R��M��	���V�0ئ��D����ϡ��|?`ö��h��mG"TxӇ@�(����r֧$"IV��2�}K(X*��8�T�#H�~�=��䀴�ٔD$�CzŮ��G/K�;|������`������⺝�{/7)�0�M��g �k���7���ӧO��̕      a   �  x��X�r�8���P��a�xQ�ͦ�f�)��&NaY")�A�D)�|�|@ � ���9���88A���x޼\�a(F(JWYQD�;��a>G�`� K*��!���g�D���3A�w�<�NO�ǭ���[y0A��p�eX �CP˄��,�,��e	c	Ș"E��>�B]\����t�7:G4��$�� BP�7���>[�Ǟ�*?d��pg'��9@c$I�Z
 0c2�R���{��%ߜ����u�m����^�	��,�@�
A-2c�K2nzst@��	� �+�BP_FN�4_�l��b[�}:m��R�B���� �ϛ���cuk��n������AB����$�ԟɄ3��_�Q�I6�z �!�of�n\^��������'D�\2<͉�:�b��#��f�FaiB����,�a�L����' $�<��&!��xA���캮 ���_��uz��|Q��%�@��� ���Ӿ�)��qwٔ�G:�Sazq�AVi�Hi}BK���Xq"�
A}oj%h\v�rOX�OT�/FY���#U��x��v���$2�گo�1,0� ����Ô���R��$�b
\ⱞ KA��B��<��]�#A�b���.dI����<�>�>`�@0���Y�ɨ/�Ǎ�P3�\r"��Doe�Mk�񫏂���Rh�A�v��J���5�6׋��0��&d\�dy��h�]�����	��s�7di�F��c��)���|L�i 	��\��LdT\\�z���8�j��H�E#�ܹ�<���Z�~�Ga�U����8���X@X����l����e���"�G�'�����Q�+EB�ѡH�T�t�/W#ۈɜ4F_�VJG*��,�wy���٨"�7B�w�&�]�zC}�?�vs''�գw�Mw�͐y"M��Kۦ^�^�"�x9秲�w|��������C{;j-4BD�(��2���ŨBc�Vե��5���a�s+aQU��ɬ}m���8�.�����q;���Q]�}�ǭN���F�^W�����r ���]Ջ�GF��������"�p�O�M�~y$��aF��VU���m�v�LG)A�ݖ�2������id��O����j�H���f,����7�f�Oִ�G�a��za�@	��j.t�Z<6YZԞB�F�1V�t�r�S���M���@7�����l�q&d�En�䠉[�l �6ɥ�"�z�cҹE<��$}\�6�}{�ڷ���2����=G������Ӟ��'�T	v���]��p�/EUx&�4%���x�@6�� ��I�4��A"z6���ݎ�nV�&]����0h�n�ɠ5x����]�����ɸ�Э����9:{��vӛȲ"����Cl�[���,W�<����ʻW��1L3��II�q-�<Z��b$�& 17��l���	,������l�v�cE��	���<�U:(o�vҵ����z�O"ctP��tP��-���r�I���:?�/�Ihj�;ƃABP�腉v:�q:M��P�y�k��X�$�{���<:V��kF�?>}�z��) �ɝ�B�S��ȢԼ�uߚ.���)��Y����㑦ȭS!Z��m�Sv=UU6-�&F�`�����m����g��&�bz���=B���|�>���3Tϫ���'�!!�<�S����O�S<���`=��      c   �  x��X�n7<s�B? ��"�s ��r�I��(9���Oqv���R���~TW�q�)pb�O��]�A�@������H��b�ӗׯ痗�c����|��k�������'֐}2���w)�C�H�����H
�_�_�n8��meExH)�,��tȁI��N�$�2t1�#������^�?�k7{��Y�sLC*[V3�ASQ��M?^R�<��{�;��|���d�܃&/�5�֏^�PB<�=i��k�v���Pa�\{������F��Qk!O)k:�k(i�BF�֘��Mg/���B}�+���*�����)����ѩ�	��nZ{�œfޙF���x(u3V1�QT���[��.�b9��n�0:�����H��*�ҁ��݊b�����+th���US�[b�%q:�@J(�J5�;K�⬹]�dx�c��JS@FW�Pu��ƌ,�)�(e����*h6��y�G����IY,)��L�^'����%fMŤ]�T�q'ae�F�0���^$kLKyCK�Ao�u�oO��j�T�����٪�?6��I$�׆
��:i\�`(�LN�{Д5ڡ.���o�Cy�(9��B�f��{Y������Є�%�͔��WC��X�=�Y8�Pd��y`�I,�_7G��(L�S�V�_�>����r���ϯ�����	%�e����ʐ���9I�� +涼0�$��.kZe�"�܃�兆?�����Ye>�sZA�5��z��=��,�fsZG�Ƶ��T�<b�n���y�]4Pi{Јk�4e����x]�������9Pƿ��J4�$Ͳ�6$��I:���΀ڠ��F@/�c|� ���<��F(͡cK���V�]%�(��rPJ|m�p��l9��cg����<^;�b��ڃ��b��j=�,I(��� cƪ'Mki����AC���q�S�^K:��B�(��V��у�9��[k+����I}���-�����B� F�jc�k�Ds�0:1X !�uMy8��l�*=��[ԂϪa�
4�A�$���[h� �bC��ۂ���qsh:�j����pgh�v���!!�:�<�Q���D�Xm���t�R0�Co�0gw��v����T�d=NJW{L�)�f�ޅ&i���MM<~����<5m�Gr�T�v��W[����u�(u4�}��9t� �����OY�2^�E=�-�sʩ}�QǇ`}�s%M�Vi�����<����R˹���i�T��#���)��XnU �ЭP:�K�WDM� �&���2݁>r�r�N9���0u�z�!1�E���My �.(j�A�<ԝ�:~�P��=��؝3��,=HZ1�A�x�C$+3��i�9Wm�C�Q�m4��ȉVc��.Vͻ��e%|�2�k_Lz��ۅ��1�j�IҮ����*"�A���{d8�u��sX,�˒��H�s��0}��ݵ;�H�L(� Fwj��P$!v�AӢ�{���)m� $���t��Pc      d   F  x�-�I�X!BǼ���T�{��� �?��P(j��5��ͩ=���^Zx���^��
C���.�	�J0���a)iã��^�@��3�썘Y��xTW7�z) ��5��|�D��&�}i�r���^�&��h�zCUj#5ꄹɟ�0����{����u�R��_V6�^�$K�uS����n�,��=G0��8䙜��]kva8Q����E�1�h��^	���h�T�7�����h�\/b��2>��=�5�y>5v��F�/p�1˶��CAƥ�jyz�t�F/^��H1t����x+Jnݾ')�����������-Tq�      f     x�]��q1�L0*HI8��~v�ʾ���W���E�G~�R�m�Bց4�9y�R�K�2��nl��(]dNt�G���FH��H�ʺH�Ĭ�Ҧ,f�$(ʽN��֨D���L�QN;vu0�Y5tkjX�.�%$=��Y��|bd��L�щ,�=����������ry�����.Ϝ�t�!^O�{�Џ���J܉�ΝD澯�GVծA.��;Y��Fe>V��\�q.�_�Z��j�[�|ԕ�D�Ix���<�/½g�      h   �  x�M�ەe!D�%�^>@4����c
�Ś�]^����f�ӆ�6�m��oS4`�%�V������L��rw����ɖ<�W�d̑�V�_������.�Y�q޷��Y�Sf��/������+cT&�W^����r�y�eu�	��.k2��=��x�1����vƀ+�usE�[_]�}�\�����p��w-Q�����=���k�������].��GL��`c<x�=����`֫C쾾�7����N٬WqKc��WU�=�����_�a.0�3�;��+����^uq�:��G|��>����O�W�+����ߺ}�ۋ	f���������!��7��٦\��`�kK.�7���wd*����5�{o�,�6��!x�,�K����<��L��甂�1K:H
:1�`c���u>A{Bpz��5���*��!x�S���k!f9P� E9@�f9��Ѫo�E�;F������B��y����ELg^���B'���_��Y��Y�0~�h�*�f.N��E�[qh�҆�~-���l��������h�7ꢕ�v|�W?ũ�C�:���)���������w.ŷ��{𱥨*O���wѨl)&�.z�͘�C]�3OQ��(ҫ�'�8��j�����mM���O���GD�g�c�      k   Q  x�}V9��F��W���we�+K�B��uȐ-�PTgy좀�:�bdd��	��~��D��"ݣV�n߿�~D��l��&%]u�l>B���4��I�rf{����J��o�������Hg�-̲��m��]=����{�fbl���
���� �ں��A�n'�_�Cx�7-S�'a�"�O�q��m@�S�5_d`:B}Rv]AN��tF�� ʛy�
rF�-�����U�HV���&�H>�fy�&�yi�?�`Ŷ�3Fף�ޢt�śq��
r����E��s��r�>�U�wt����Ť7!��!�K���G�$FTj.��V�w$�Jey��3�n��n���lZ9q�s�筀�̛�ح X��*�C�_��#�� oIh�W�C�B����=�K����@A�%6���r*�u��e�6
��%�l��[�t䟺a����-�<e�`�KD��0X/�r뮓�i�NVH���0��f�E��{��G���|9�������,��z�����(�𖘇g?�!ei���_���3��ȏaް����x���.��/sI�L���$&ȯw������>���\؉c{�*C�oq8������3kp��X�Q`>B��vq�w�Lbޟ�2�e��N�@��R[�!y+`�A��S�=
�L_��̀\�'M>_��+(�F�wl�u�^B�(�3�jZ�Fd���5o6gc98|YA~�����`LA����n����A�E_A^m|�������Nf7yD@.s�ξ�p���3��f*=WP�v���*@�
U���20D�'��|��"��~���=d1�I�{���J��"      m      x������ � �      o     x��ӻj�0��z��@ĹI���@�6��-KۨHl���Gvq���� M�����ݰȬ��^���⼥\��/�buV4�`@
�� 8�\����o��ClN��=ԕ�yQ{�梄���Q2������=��<���=����\��L�m��.Ƕ�>]�\m�mW���
�eɚp� m�	�\46��;ƥ�b[��h|D�}��i���y���&+�87^�ib��s�D7��a��@^�}����J���i����(      q   &   x�3��M�KLO-�2�tL����2�t�HM����� ���      s   m  x�}XM��8=7��?�Q����R[��^��9�2`Yf𬱩x��2Ac�D���O�,��_���4�u������:m8�\=Q�tE�N�s�:*L��w���e0�:@��v=\2��#�;p�1�fj����_��C8Nt5~,g�vJ�9i���%(Q	h���=�y����N�4����ih��0Nm��jG�Bq�JНMP������}o`�͵�kqs��F��n2.�|�����	6�Џa�-~m�|���R���Z���:�#�qk]	Jdf����dg�ٝD_
��,A�5r�N�=�׵��[:�����n�E.����H���a��l�bЎ�O|"��%ʊ��;%�v����(�jyL���|nE�"�2&\	J�W�Ecd�ᗭ|b6f��;F�4L0U��[��}u�}���qB���Ԯ%R;�u��zc?�3�;��К;S����=�h�0�-#FR��S��^9I��&7�Er�-�F��;�\���� �G��|�vB��%f�?N�)�:m9ƣ.A)Ry��a���!��<�NJ�N��4I��[;M�c>�>�N\'q�*�JP��kP���X��"�j�E	J�r�[�~��4�Y3LLC�0N��(�s�j9䶘��(,�<O��hW�ˠ?�I�TN�Lkl	J���5ju�z��VN�vs�P焦���Dhq{�v�#4C�;�a!2�Ze�+A)a�s|�RY���K	?m�:G׽}D���d�#�2+$/A�Gk�юC?5p8���rh�&ۀ�oOs%,*�v+n	J�(����S^6��1K��H���&1�k��K�)7����j�t^�PbÞcQ�|�KѨ�ʖ�D�������|����آ���P"2P�ԫrk�P��%*M׌oo`���J�z����,v=}��{Y��Xӣ��"T���DB�Ú��J)b���������k#.��Y����<��[c�d�v\a�H��J\
B�bG��\��JKiW�C�KC�?�<.�Ny~�l���ΰ��Ll��[�˥ڿ�ӕJ�Z��t<B����Op!'�l��L9U�,,�D���B�q���<��M4
���JPbp�}����9��*7�#�#�9�e	J��χ6����-y��	��?��=�F�6�?�D���ųҊL��#�R��r�$*���I����ۨ����e	JT�v�J�6���hHõ) �'J� ���zTj���%(Űߴ���}xňy[K�V~�CifU���g�����AM7�ݩ50�q���Ǭ��0wT�VA6���*�ǫ+&�u����b��UpY�./������"�ƍ�V�ٟ���8��G���|�o_ߥ���������E����	]��e�;�l6�2
 �      u   D   x�3�4�t/�J,J,�4202�50�52V02�20�21�35276��#�e4���L��@͡�j����� ��%H      w   �  x�}Xˎ7<�|���>D=��H��G_{m809���)IOwS�O[��I�R۔��,!�w�q���1�!�����������������V���V��xҝ��nDg]A��?~����/ۗ[����uc�f�%�*��4!�A�>R�5h�Y�
�9n�@z�w�]^4�w]9�nJ=I�#����pȠDy�?>}��Ϸ��|x��2�$�Z��@a�[L�c�P+yW�<���eS�aݙ[Nu�\(�nF���5�lf�+ȑ��Vq�,91��I�eS=5�!��h&1� �A�X'�)�b��*+� w�,�g�bw��$�9�@��n箓s�,�R�]�!_�qHM$�w��R�j+�qZ�}<R�܉�4Z߫��)G�6��M8W���eI[�kƭ��>�bQW��x�򖼼YWp��J\A�ֳ�J2���BNV��<B�48uB��P��y�4Z]�5G��j��]9�|W�G�e)FX��~���Y�����8�NЈ$v�lr���~��
�w����'�脲�`Ȕ��y�|��<}���k���
	��k��v��q"���x�<aĄH!qT��D9�)�	iƈ���׮?A���^�(�ܐ��F0M�R\AJ�r5_i��
,��+h���/�9F�[8�уp��4a�[�W}*����2�KZA.�h�N�W��+�Xr]A�v��f����yy3i��s*��f�v5�'�sϠ'�!F�@���]����lfݰ�L��i��:�lF+�\	�wy�1>D&�PIVaܮ��9�4,��+'w���!5� �)�ӝ�w��T�
�<8�$���c���y�|վÜ�Gm߬�5����
���4v-����KL)�4#���E�ƛu�|@�8�#4#�NZ=),�f�����i����,w�@��G=���������3j�`<H[��U嚎��?�-*TQ�uFY����{����
���Q��		���]P>��Q�͊��r�8#MC���
p�].j�-�)<�����Z����X��'R��2�)g;��Ѱ֠���i��;>�La�T��|��eW$?a(��f'u�r�q%�m�.���>9��:�(�PU�S�y§N������&C�O�sw�o�ю���!��Y��AՏ	"��t��צ����҄_��E+�L���T�����E�P��)�$X��������X�~+�.z�'w[��e6;���S�3����.h��$����P�ܮ�fQ�1�4ov99�֌5�í� �q�=���}_�)�֍%�O���K��lޭ.!��1���ey�$5�Q`�c�Z��NU�\`02�8e�ih�H9�S�]�Z�d�d��F�J��щ����(��=R�йc�ֻ�v���s��T�M̍��L���݉Hk>T�`���D���6�*7i���rﲃ4X*|�Φ����s�zR}xEg�� S�P$~2*=�H�G�Z�S~���?f�O      y   u   x�}�;
�0D��S�Z��HۺH��)]��?���! f��<�๿A���e��V��4I5��x�:^W?s��1�B�,E-�g�nb�B?����'��6�����
���_}>0�      {   !   x�3�,H.�2�L�2����2��N����� Hq      }   �  x���is�8�?�_�~۝�nɞ�L	i�4���0���v��1�ί_�0GikL�W�z�[��DE��q�F�;S!]y� |T���y��ԩ�GW���<u�v}1M�`���z�t2��xP��$���.���?��20Ĭi�*d���9��I�k��S�� ��P% ��Br�~����fSa���j����025^�*V��i8��X�wb@d�����UHJ�-�f!��P"	Ɠ@�%&��ZI@3P��6YZ���բ^�d⦓��[��x�ݹ>CV4�� ﲗFU��-�$�$*�Q.���� �@-C��]d�l��e
�P���ӣ�6xC�b�K��dIy�(��{���yB��b�����_���-�<��S�^� �ui�N���.��(�m,F��$*���Li�J���Ib�fQ ��ݧ^��H�E����_!K��S�Ӳ^/��7������+�����UH̓�ő-2�vn�9��z��\�^�u"��!őAy\�0n�R_�n.H9���ӚvH�̺?���ꏭ ���Jb�8XZ�Il�hP�D��Pi�&%�4�4{�W��W�I�I� )܊;ڳ��&�~�#�)�-���)���g��p^,�0��h��0�o�]�/�p�W��K��"���y)G	�M�}��+T�>��:�f%�8Ȫ����u�#�	�h���G̞�x]� �� 1d2]��4���P���i��ЏWEH?U.r\��n��y�e��[�ً�>�{�ҚK�"C�aDHsK!=�
FllT��Nfq8J\�|�t?_��]�i�/A狶���޹�@daa#h�D6l��؍��ٷ���OZ6����#������U�ʢ6�n�tV.��åw_0�YYŊ����֍�n��D�$ڍ����痠��
U��	yܹl5��;��/�������Ѩ�o���#��_����l[��l�%��U�D����/k��@Oe��8,-��Mg�sy���G��l�O��U����s����Q�r�@#�����bgH�!�9H7F�"s�6������0.ўԩZv'gg���Q�6FAgz��ռ��� H����ԧˇ�]UrO�=�NVD�
Q�C&�[�}�~��Q����6Z�ν�<o�~�_��y|=j]�Tc&���>?���{x��[��ج7������]A�&��Ì�`��2Rb�kw����u��r[�O�_Ը7���n�l�7l:��+���R�Ç��HtO]yZ��_��;Y�i�y�{��@��f�_���ysG8���Do�~���!��|M����5�v���=�^�?>���-���wfM[�I�g�.ⳝ^�?����V�L�k�K�� ��+=�R����2�         �  x��X�r��=C_�D�VN�>�2}p8|��榵�C�-�3,�K6�|r{�Y���/�qa��Q�\lœ`�A���������<_�^���=o��������!��B�� l�w���	�� tD�gn�3:Vȷ��Lt�0��^~ݿ�ǿ&I/n�N�#�$Uh��{����Za!hn�A� w$����x9�s�q���H��s��0Md�i��C_����L~Ev����
d�����9_�F�/���^�7ڵ$�/X|���� �W�u>:���A�x�@=��\��5جr�;�6�o�*�sh��ۡ��7��C�4�^�KDj �X���7�yƐC:!�4'�?���/�ٮ��Hsč�=wl)�)�kb�.� �oo��Uo�2~�R�(,b��k1�O��H�R�1\-:m?�&�����paY ���ZĎ9�,��q���>�,�l���~����32+#}j5�4J�iY���qN�eT&]iShhAcOm�����JZWi����Gߵ\��1���6��$�BWꚨ�Z*�G���ʌ�`�	b��p�?�?�Ǐ��b�BP�F�i&@�+�	BY)���屦�+ס���+U�2@�\F����}�����=�kH�e�d�"����|�<�� �䬣u�b��q~��`r1@b9��r�����g0�q�SV��q~���H�N
�:�o��k�k�
�|�$-��#Dާ+�n�c{���,�����"b��WוLW(Qʕ�9<~��Z#P�Ή�SI3A��jN�}{�,H���a�!��Ѳ��P��yX&����\>?jq�A�s�z�@nɡ�=AAh5��\�1�&�L�9�JЦV]��h%Ƿ����J�Q&�rm1�{bI5AB�Tp���ukB�����Ʊu.�_��dMHژH�j�������y>���BƔ����L�\h!&m�7(�8� ����&ĩ̍9��������kOh�����o�Ȇ�k;?l��ߚ��Ӷ:}36\��,�Ne:���ۼv��:�'DR8��[w�������"Ha���Km��fJ�5h]I*�&$ \e���Ҽ���t�l�^�Uxx�@��������fq����9:'^�-�ub�9>��2�V$)�&Y�#%�i1gG�4ծ_^�V�v��H��J������"�nn4�4@I�^��j�|Z8rӺ��F�!B�u�}|AC�

�G?�����!2�GM~B^��"ck�>4x"�#�K+k�Hm��{�'[����)7ib����:��'O�u���\A3�n.%����&W�@'0�=7$�˧��y&�tx��aBV�� �/�M���q� �n��d/�q"��d��n!�����Pv�����GMD
�*�Z�qKhM���� ״3���~��B�]&�୔��0���s�%���7���F赿~VrO�����#� ��j�0�̹�4��O�Nh�4�%)�b��]�k#���Ss?|Ԝ�w<��Ŝ�phk�ؠ�m��ۦ���⼒J�[��&�[�"`�!�1�E]�Z!/�xyxx���7ZY�^������uu4s�qRa^��S+��<��� �2Y�Ga�?��|^��n-~XYJ�	��8�p0t?��s�4�e�X>&[�Y{H���K�L�y^Y�n��n�x?�f��^�TV�`��Kĳ�O��<��)���6�`0?��+���9�ԏ*����:������q�m䴖H�"�(���b�X�|<��O�\�́Kֶ��,���NY&*^c�h���H+J�
K�4AƓq���"j&XÑ�Zk�~#��^�u_��
�rmm�I/��u��Q0A��}L����"+�R�TB;����f>%;X�%����3W��}��k�ʗ��v5lV��2/�����y��=���N-�UZ�N�K���0xG�0�FH�}�����椯��m���+�-#B�)���V��?����vw��www�$��`      �   o   x�E�A
�0����@Cf�?�Y��'�&D-�����+�힁�$
��AԳd�� N���s�1eH�.B��3�z|��o����ro{g�ike�z4�H�]�d
fz�58�L     