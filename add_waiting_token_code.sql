CREATE OR REPLACE FUNCTION add_waiting_token_code(
    p_total_person INTEGER,
    p_user_name VARCHAR,
    p_phone_no VARCHAR,
    p_email VARCHAR,
    p_section_id INTEGER,
    p_created_at TIMESTAMP
)
RETURNS TABLE (
    id INTEGER,
    total_person INTEGER,
    user_name VARCHAR,
    phone_no VARCHAR,
    email VARCHAR,
    section_id INTEGER,
    created_at TIMESTAMP
)
AS $$
BEGIN
    RETURN QUERY
    INSERT INTO waiting_token_codes (
        total_person, user_name, phone_no, email, section_id, created_at
    ) VALUES (
        p_total_person, p_user_name, p_phone_no, p_email, p_section_id, p_created_at
    )
    RETURNING id, total_person, user_name, phone_no, email, section_id, created_at;
END;
$$ LANGUAGE plpgsql;