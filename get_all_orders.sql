-- FUNCTION: public.get_all_orders()

-- DROP FUNCTION IF EXISTS public.get_all_orders();

CREATE OR REPLACE FUNCTION public.get_all_orders(
	)
    RETURNS TABLE(order_id integer, order_status character varying, createdat timestamp without time zone, order_comment character varying, orderdetail_id integer, orderitem_isdelete boolean, item_comment character varying, prepared integer, quantity integer, item_id integer, item_name character varying, categoryid integer, category_name character varying, ordered_item_modifier_id integer, table_id integer, table_name character varying, table_modifiedat timestamp without time zone, section_id integer, section_name character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
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
$BODY$;

ALTER FUNCTION public.get_all_orders()
    OWNER TO postgres;
