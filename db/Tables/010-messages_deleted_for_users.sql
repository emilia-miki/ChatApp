create table messages_deleted_for_users
(
    id         int           not null
        constraint messages_deleted_for_users_pk
            primary key,
    message_id int           not null
        constraint messages_deleted_for_users_messages_null_fk
            references messages
            on delete cascade,
    user_id    nvarchar(450) not null
        constraint messages_deleted_for_users_AspNetUsers_null_fk
            references AspNetUsers
)
go

