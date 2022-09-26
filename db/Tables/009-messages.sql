create table messages
(
    id       int           not null
        constraint messages_pk
            primary key,
    user_id  nvarchar(450) not null
        constraint messages_users_null_fk
            references AspNetUsers
            on delete cascade,
    chat_id  int           not null
        constraint messages_chats_null_fk
            references chats
            on delete cascade,
    datetime datetime      not null,
    reply_to int           not null,
    reply_is_personal xml column_set for all_sparse_columns
)
go

