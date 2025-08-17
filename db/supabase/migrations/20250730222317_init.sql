create type "public"."agent_id" as enum ('trainer', 'nutritionist', 'orchestrator');

create type "public"."sender_enum" as enum ('user', 'assistant', 'system', 'tool');

create table "public"."chat_contexts" (
    "id" uuid not null default gen_random_uuid(),
    "user_id" uuid not null,
    "created_at" timestamp without time zone not null default now(),
    "system_prompt" text
);


create table "public"."chat_events" (
    "id" uuid not null default gen_random_uuid(),
    "chat_context_id" uuid not null,
    "agent" agent_id,
    "sender" sender_enum,
    "content" text,
    "created_at" timestamp without time zone not null default now()
);


CREATE UNIQUE INDEX chat_contexts_pkey ON public.chat_contexts USING btree (id);

CREATE UNIQUE INDEX chat_events_pkey ON public.chat_events USING btree (id);

alter table "public"."chat_contexts" add constraint "chat_contexts_pkey" PRIMARY KEY using index "chat_contexts_pkey";

alter table "public"."chat_events" add constraint "chat_events_pkey" PRIMARY KEY using index "chat_events_pkey";

alter table "public"."chat_contexts" add constraint "chat_contexts_user_id_fkey" FOREIGN KEY (user_id) REFERENCES auth.users(id) not valid;

alter table "public"."chat_contexts" validate constraint "chat_contexts_user_id_fkey";

alter table "public"."chat_events" add constraint "chat_events_chat_context_id_fkey" FOREIGN KEY (chat_context_id) REFERENCES chat_contexts(id) not valid;

alter table "public"."chat_events" validate constraint "chat_events_chat_context_id_fkey";

grant delete on table "public"."chat_contexts" to "anon";

grant insert on table "public"."chat_contexts" to "anon";

grant references on table "public"."chat_contexts" to "anon";

grant select on table "public"."chat_contexts" to "anon";

grant trigger on table "public"."chat_contexts" to "anon";

grant truncate on table "public"."chat_contexts" to "anon";

grant update on table "public"."chat_contexts" to "anon";

grant delete on table "public"."chat_contexts" to "authenticated";

grant insert on table "public"."chat_contexts" to "authenticated";

grant references on table "public"."chat_contexts" to "authenticated";

grant select on table "public"."chat_contexts" to "authenticated";

grant trigger on table "public"."chat_contexts" to "authenticated";

grant truncate on table "public"."chat_contexts" to "authenticated";

grant update on table "public"."chat_contexts" to "authenticated";

grant delete on table "public"."chat_contexts" to "service_role";

grant insert on table "public"."chat_contexts" to "service_role";

grant references on table "public"."chat_contexts" to "service_role";

grant select on table "public"."chat_contexts" to "service_role";

grant trigger on table "public"."chat_contexts" to "service_role";

grant truncate on table "public"."chat_contexts" to "service_role";

grant update on table "public"."chat_contexts" to "service_role";

grant delete on table "public"."chat_events" to "anon";

grant insert on table "public"."chat_events" to "anon";

grant references on table "public"."chat_events" to "anon";

grant select on table "public"."chat_events" to "anon";

grant trigger on table "public"."chat_events" to "anon";

grant truncate on table "public"."chat_events" to "anon";

grant update on table "public"."chat_events" to "anon";

grant delete on table "public"."chat_events" to "authenticated";

grant insert on table "public"."chat_events" to "authenticated";

grant references on table "public"."chat_events" to "authenticated";

grant select on table "public"."chat_events" to "authenticated";

grant trigger on table "public"."chat_events" to "authenticated";

grant truncate on table "public"."chat_events" to "authenticated";

grant update on table "public"."chat_events" to "authenticated";

grant delete on table "public"."chat_events" to "service_role";

grant insert on table "public"."chat_events" to "service_role";

grant references on table "public"."chat_events" to "service_role";

grant select on table "public"."chat_events" to "service_role";

grant trigger on table "public"."chat_events" to "service_role";

grant truncate on table "public"."chat_events" to "service_role";

grant update on table "public"."chat_events" to "service_role";


