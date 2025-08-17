create type sex_enum as enum ('male', 'female', 'other');

create table "user_profiles" (
    "id" uuid primary key default gen_random_uuid(),
    "user_id" uuid not null references public.users(id) on delete cascade,
    "height" int,
    "height_unit" text,
    "date_of_birth" date,
    "sex" sex_enum,
    "goal" text,
    "created_at" timestamp not null default now(),
    "updated_at" timestamp not null default now()
);

alter table user_profiles enable row level security;