CREATE OR REPLACE FUNCTION confirmation_cancel(p_order_id INTEGER) RETURNS INTEGER AS $$
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
$$ LANGUAGE plpgsql;

select confirmation_cancel(64)

