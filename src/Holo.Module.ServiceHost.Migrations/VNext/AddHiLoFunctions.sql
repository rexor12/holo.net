CREATE FUNCTION service_host.get_current_hilo_sequence_hi_value(sequence_id text)
    RETURNS bigint
    LANGUAGE plpgsql
AS $$
DECLARE
    current_hi_value bigint;
    row_array "service_host"."hilo_sequences"%ROWTYPE;
BEGIN
    -- force the table to be locked
    SELECT * FROM "service_host"."hilo_sequences" FOR UPDATE INTO row_array;

    IF NOT EXISTS(SELECT * FROM "service_host"."hilo_sequences" WHERE "id" = sequence_id)
    THEN
        INSERT INTO "service_host"."hilo_sequences"
        ("id", "current_hi")
        VALUES (sequence_id, 0)
        RETURNING "current_hi" INTO current_hi_value;
    ELSE
        UPDATE "service_host"."hilo_sequences"
        SET "current_hi" = "current_hi" + 1
        WHERE "id" = sequence_id
        RETURNING "current_hi" INTO current_hi_value;
    END IF;

    RETURN current_hi_value;
END $$;