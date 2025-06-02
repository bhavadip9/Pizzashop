-- PROCEDURE: public.deletewaitingtoken(integer)

-- DROP PROCEDURE IF EXISTS public.deletewaitingtoken(integer);

CREATE OR REPLACE PROCEDURE public.deletewaitingtoken(
	IN token_id integer)
LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
    UPDATE waiting_token_codes
    SET "isDelete" = TRUE
    WHERE waiting_token_codes.token_id = DeleteWaitingToken.token_id;
END;
$BODY$;
ALTER PROCEDURE public.deletewaitingtoken(integer)
    OWNER TO postgres;
