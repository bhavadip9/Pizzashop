-- PROCEDURE: public.update_order_detail(integer, integer, integer, integer, integer, text, boolean)

-- DROP PROCEDURE IF EXISTS public.update_order_detail(integer, integer, integer, integer, integer, text, boolean);

CREATE OR REPLACE PROCEDURE public.update_order_detail(
	IN p_order_detail_id integer,
	IN p_item_id integer,
	IN p_order_id integer,
	IN p_quntity integer,
	IN p_prepared integer,
	IN p_item_comment text,
	IN p_is_deleted boolean)
LANGUAGE 'plpgsql'
AS $BODY$
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
$BODY$;
ALTER PROCEDURE public.update_order_detail(integer, integer, integer, integer, integer, text, boolean)
    OWNER TO postgres;
