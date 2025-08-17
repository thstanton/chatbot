create extension if not exists "pgcrypto";

create type sender_enum as enum ('user', 'assistant', 'system', 'tool');

create type agent_id as enum ('trainer', 'nutritionist', 'orchestrator');

create table chat_events (
    "id" uuid primary key default gen_random_uuid(),
    "chat_context_id" uuid references chat_contexts(id) not null,
    "agent" agent_id,
    "sender" sender_enum,
    "content" text,
    "created_at" timestamp not null default now()
);