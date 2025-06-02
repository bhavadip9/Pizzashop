-- FUNCTION: public.assign_table(integer, integer, integer[])

-- DROP FUNCTION IF EXISTS public.assign_table(integer, integer, integer[]);

CREATE OR REPLACE FUNCTION public.assign_table(
	p_waiting_user_id integer,
	p_section_id integer,
	p_user_name varchar,
	p_email varchar,
	p_total_person integer,
	p_phone varchar,
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
 v_context text;
BEGIN
    -- 1. Fetch Waiting User Details.
    SELECT * INTO waiting_user FROM "waiting_token_codes" WHERE "token_id" = 65;
    IF NOT FOUND THEN
        RAISE NOTICE 'Waiting user not found: %', p_waiting_user_id;
        RETURN 0;
    END IF;
    total_person := waiting_user.total_person;

    -- 2. Check Table Availability & Capacity
    IF p_selected_tablelist IS NOT NULL THEN
        FOR tbl_record IN SELECT * FROM tables WHERE table_id = ANY(p_selected_tablelist)
        LOOP
            IF tbl_record.status <> 'Available' THEN
                RETURN 0;
            END IF;
            table_capacity := table_capacity + COALESCE(tbl_record.capacity, 0);
        END LOOP;
        IF table_capacity < total_person THEN
            RETURN 0;
        END IF;
    END IF;

    -- 3. Update Section if changed.
    IF waiting_user."SectionId" IS DISTINCT FROM p_section_id THEN
        UPDATE waiting_user SET section_id = p_section_id WHERE id = p_waiting_user_id;
    END IF;

    -- 4. Find or Add Customer.
    SELECT * INTO customer_record FROM customers WHERE lower(email) = lower(waiting_user."Email") LIMIT 1;
    IF FOUND THEN
        UPDATE customers
            SET name = waiting_user.user_name,
                phone = waiting_user."Phone_No",
                email = waiting_user."Email"
        WHERE customer_id = customer_record.customer_id;
        new_customer_id := customer_record.customer_id;
    ELSE
        INSERT INTO customers (name, phone, email)
            VALUES (waiting_user.user_name, waiting_user."Phone_No", waiting_user."Email")
            RETURNING customer_id INTO new_customer_id;
    END IF;

    -- 5. Insert Order.
    INSERT INTO "order"  (customer_id, "OrderStatus", "Total_Person", "Amount")
        VALUES (new_customer_id, 'Pending', total_person, 0)
        RETURNING order_id INTO new_order_id;

    -- 6. Insert Payment.
    INSERT INTO payment (order_id, payment_method)
        VALUES (new_order_id, '')
        RETURNING payment_id INTO new_payment_id;

    -- 7. Table Mappings & Set Tables to Occupied.
    IF p_selected_tablelist IS NOT NULL THEN
        FOREACH tbl_id IN ARRAY p_selected_tablelist LOOP
            INSERT INTO order_table_mapping (table_id, order_id)
                VALUES (tbl_id, new_order_id);

            UPDATE tables SET status = 'Occupied',modified_at=CURRENT_TIMESTAMP at time zone 'MVT' WHERE table_id = tbl_id;
        END LOOP;
    END IF;

    -- 8. Delete/Update Waiting User (mark as deleted).
    UPDATE "waiting_token_codes" SET "isDelete" = true WHERE token_id = p_waiting_user_id;
    RETURN new_order_id;

EXCEPTION
    WHEN OTHERS THEN
        GET STACKED DIAGNOSTICS v_context = PG_EXCEPTION_CONTEXT;
        RAISE NOTICE 'Error: %, Context: %', SQLERRM, v_context;
END;
	
   
$BODY$;

ALTER FUNCTION public.assign_table(integer, integer, integer[])
    OWNER TO postgres;



