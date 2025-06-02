-- FUNCTION: public.get_waiting_token_code_by_id(integer)

-- DROP FUNCTION IF EXISTS public.get_waiting_token_code_by_id(integer);

CREATE OR REPLACE FUNCTION public.get_waiting_token_code_by_id(
	p_token_id integer)
    RETURNS TABLE(token_id integer, total_person integer, user_name character varying, phone_no character varying, email character varying, sectionid integer, created_at timestamp without time zone, modified_at timestamp without time zone,created_by integer,modified_by integer,isDelete bool) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        W.token_id,
        w.total_person,
        w.user_name,
        w."Phone_No",
        w."Email",
        w."SectionId",
        w.created_at,
	    w.modified_at,
	    w.created_by,
	w.modified_by,
	w."isDelete"
    FROM waiting_token_codes W
    WHERE W.token_id = p_token_id
    LIMIT 1;
END;
$BODY$;

ALTER FUNCTION public.get_waiting_token_code_by_id(integer)
    OWNER TO postgres;


select * From get_waiting_token_code_by_id(66);
