-- FUNCTION: public.confirmation_order_status(integer, integer, double precision, double precision, double precision, jsonb)

-- DROP FUNCTION IF EXISTS public.confirmation_order_status(integer, integer, double precision, double precision, double precision, jsonb);

CREATE OR REPLACE FUNCTION public.confirmation_order_status(
	p_orderid integer,
	p_customerid integer,
	p_totalbill double precision,
	p_subtotal double precision,
	p_othertax double precision,
	p_selected_taxes jsonb)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
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
$BODY$;

ALTER FUNCTION public.confirmation_order_status(integer, integer, double precision, double precision, double precision, jsonb)
    OWNER TO postgres;
