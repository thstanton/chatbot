create function "update_timestamp_function"() returns trigger language plpgsql as $$
begin
    new.updated_at = now();
    return new;
end;
$$;