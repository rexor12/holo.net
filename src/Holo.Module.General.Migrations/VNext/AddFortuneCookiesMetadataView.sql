CREATE MATERIALIZED VIEW general.fortune_cookies_metadata AS
    SELECT COUNT(*) as total_row_count
    FROM general.fortune_cookies;

CREATE FUNCTION general.refresh_fortune_cookies_metadata()
    RETURNS TRIGGER LANGUAGE plpgsql
AS $$
BEGIN
    REFRESH MATERIALIZED VIEW general.fortune_cookies_metadata;
    RETURN NULL;
END $$;

CREATE FUNCTION general.get_fortune_cookie()
    RETURNS TABLE(id BIGINT, message VARCHAR(512))
AS $$
    SELECT id, message FROM general.fortune_cookies AS _data
    CROSS JOIN (
        SELECT (RANDOM() * (total_row_count - 1) + 1)::bigint as _random_id FROM general.fortune_cookies_metadata
    ) AS _random_table
    WHERE _data.id >= _random_table._random_id
    ORDER BY _data.id
    LIMIT 1;
$$ LANGUAGE SQL;

CREATE TRIGGER refresh_metadata
AFTER INSERT OR DELETE OR TRUNCATE
ON general.fortune_cookies
FOR EACH STATEMENT
EXECUTE PROCEDURE general.refresh_fortune_cookies_metadata();