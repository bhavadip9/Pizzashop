CREATE OR REPLACE FUNCTION AddOrderItem(
    p_order_id INT,
    p_total_amount NUMERIC,
    p_model JSON
) RETURNS INT AS
$$
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
        v_order_detail_id := COALESCE((v_json_item->>'ItemId')::INT, 0);
        v_item_id := (v_json_item->>'Id')::INT;
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
                DELETE FROM ordered_item_modifier WHERE order_detail_id = v_order_detail_id;

                -- Insert new modifiers for this order_detail_id
                FOR v_json_modifier IN SELECT * FROM json_array_elements(v_json_item->'Modifiers')
                LOOP
                    v_modifier_id := (v_json_modifier->>'Id')::INT;
                    INSERT INTO ordered_item_modifier(order_detail_id, modifier_id)
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
$$ LANGUAGE plpgsql;




-- SELECT AddOrderItem(
--   75,
--   539.5,
--   '[{"Id":171,"ItemId":0,"Quantity":1,"Name":"Aloo Sandwich","Price":125,"Modifiers":[{"Id":87,"Name":"Mushroom","Price":25},{"Id":54,"Name":"Medium Size","Price":40}]},{"Id":129,"ItemId":0,"Quantity":1,"Name":"Margherita","Price":250,"Modifiers":[{"Id":109,"Name":"Tomato","Price":15}]}]'
-- );