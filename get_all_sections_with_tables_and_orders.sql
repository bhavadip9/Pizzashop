-- FUNCTION: public.get_all_sections_with_tables_and_orders()

-- DROP FUNCTION IF EXISTS public.get_all_sections_with_tables_and_orders();

CREATE OR REPLACE FUNCTION public.get_all_sections_with_tables_and_orders()
RETURNS jsonb
LANGUAGE plpgsql
COST 100
VOLATILE
PARALLEL UNSAFE
AS $BODY$
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
$BODY$;

select get_all_sections_with_tables_and_orders()
