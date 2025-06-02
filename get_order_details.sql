-- FUNCTION: public.get_order_details(integer)

-- DROP FUNCTION IF EXISTS public.get_order_details(integer);

CREATE OR REPLACE FUNCTION public.get_order_details(
	in_order_id integer)
    RETURNS TABLE(order_id integer, order_status character varying, createdat timestamp without time zone, order_comment character varying, orderdetail_id integer, orderitem_isdelete boolean, item_comment character varying, prepared integer, quantity integer, item_id integer, item_name character varying, categoryid integer, category_name character varying, ordered_item_modifier_id integer, table_id integer, table_name character varying, table_modifiedat timestamp without time zone, section_id integer, section_name character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
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
$BODY$;

ALTER FUNCTION public.get_order_details(integer)
    OWNER TO postgres;
