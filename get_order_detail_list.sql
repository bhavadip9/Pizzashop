-- FUNCTION: public.get_all_sections_with_tables_and_orders()

-- DROP FUNCTION IF EXISTS public.get_all_sections_with_tables_and_orders();

-- CREATE OR REPLACE FUNCTION public.get_order_detail_list(order_id integer)
-- RETURNS jsonb
-- LANGUAGE plpgsql
-- COST 100
-- VOLATILE
-- PARALLEL UNSAFE
-- AS $BODY$
-- DECLARE
--     result jsonb;
-- BEGIN
-- 	select jsonb_agg(
-- 	jsonb_build_object(
-- 	"Quntity", od."Quntity",
-- 	"Prepared", od."Prepared"
-- 	)
	
-- 	)INTO result
-- 	FROM "Orderdetail" od
-- 	WHERE od.isDeleted =false AND od.order_id=order_id
   

--     RETURN result;
-- END;
-- $BODY$;

CREATE OR REPLACE FUNCTION public.get_order_detail_list(p_order_id integer)
RETURNS jsonb
LANGUAGE plpgsql
AS $BODY$
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
$BODY$;

 -- select get_order_detail_list(78);


