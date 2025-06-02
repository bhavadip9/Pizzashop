CREATE OR REPLACE FUNCTION get_all_waiting_users()
RETURNS TABLE (
    "token_id" integer,
    "total_person" integer,
    "user_name" varchar,
    Phone_No varchar,
    Email varchar,
    SectionId integer,
    "created_at" TIMESTAMP,
	modified_at TIMESTAMP,
	created_by integer,
	modified_by integer,
	isDelete bool
) AS
$$
BEGIN
    RETURN QUERY
    SELECT
        w."token_id", w."total_person",w."user_name", "Phone_No", "Email", "SectionId", w."created_at",w.modified_at,w.created_by,w.modified_by,w."isDelete"
    FROM "waiting_token_codes" as w
    WHERE NOT "isDelete";
END;
$$
LANGUAGE plpgsql;



select * from get_all_waiting_users()