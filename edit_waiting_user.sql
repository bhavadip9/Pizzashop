CREATE OR REPLACE FUNCTION public.edit_waiting_user(
    p_id integer,
    p_no_of_person integer,
    p_user_name text,
    p_phone text,
    p_email text,
    p_section_id integer,
    p_modified_at timestamp with time zone
)
RETURNS void
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE "waiting_token_codes"
    SET
        total_person = p_no_of_person,
        user_name = p_user_name,
        "Phone_No" = p_phone,
        "Email" = p_email,
        "SectionId" = p_section_id,
        modified_at = p_modified_at
    WHERE token_id = p_id;
END;
$$;