create table public.users (
    "id" uuid primary key default gen_random_uuid(),
    "auth_user_id" uuid not null references auth.users(id) on delete cascade,
    "display_name" text,
    "created_at" timestamp not null default now(),
    "updated_at" timestamp not null default now()
);

alter table public.users enable row level security;