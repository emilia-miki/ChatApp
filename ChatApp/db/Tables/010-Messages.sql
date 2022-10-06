create table Messages
(
    UserId   nvarchar(450)  not null
        constraint messages_users_null_fk
            references AspNetUsers
            on delete cascade,
    ChatId   int            not null
        constraint Messages_Chats_null_fk
            references Chats
            on delete cascade,
    DateTime datetime,
    ReplyTo  int            not null,
    Id       int identity
        primary key,
    Text     nvarchar(4000) not null
)
go

