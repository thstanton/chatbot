create extension if not exists "pgcrypto";

create table "chat_contexts" (
    "id" uuid primary key default gen_random_uuid(),
    "user_id" uuid references public.users(id) not null,
    "created_at" timestamp not null default now(),
    "system_prompt" text
);