create table member_chat
(
    id      int           not null
        constraint member_chat_pk
            primary key,
    user_id nvarchar(450) not null,
    chat_id int
)
go

