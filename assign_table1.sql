-- FUNCTION: public.assign_table1(integer, integer, character varying, character varying, integer, character varying, integer[])

-- DROP FUNCTION IF EXISTS public.assign_table1(integer, integer, character varying, character varying, integer, character varying, integer[]);

CREATE OR REPLACE FUNCTION public.assign_table1(
	p_waiting_user_id integer,
	p_section_id integer,
	p_user_name character varying,
	p_email character varying,
	p_total_person integer,
	p_phone character varying,
	p_selected_tablelist integer[])
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
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
	
   
$BODY$;

ALTER FUNCTION public.assign_table1(integer, integer, character varying, character varying, integer, character varying, integer[])
    OWNER TO postgres;
