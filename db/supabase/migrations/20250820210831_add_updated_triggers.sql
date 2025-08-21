set check_function_bodies = off;

CREATE OR REPLACE FUNCTION public.update_timestamp_function()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
begin
    new.updated_at = now();
    return new;
end;
$function$
;

CREATE TRIGGER update_timestamp BEFORE INSERT OR UPDATE ON public.user_profiles FOR EACH ROW EXECUTE FUNCTION update_timestamp_function();

CREATE TRIGGER update_timestamp BEFORE INSERT OR UPDATE ON public.users FOR EACH ROW EXECUTE FUNCTION update_timestamp_function();


