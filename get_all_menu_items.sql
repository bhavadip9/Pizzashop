CREATE OR REPLACE FUNCTION get_all_menu_items()
RETURNS SETOF menu_item AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM menu_item
    WHERE NOT is_deleted
      AND is_available;
END;
$$ LANGUAGE plpgsql;



select * from get_all_menu_items();