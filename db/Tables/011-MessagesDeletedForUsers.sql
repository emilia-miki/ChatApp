create table MessagesDeletedForUsers
(
    message_id int           not null
        constraint MessagesDeletedForUsers_Messages_Id_fk
            references Messages
            on delete cascade,
    user_id    nvarchar(450) not null
        constraint MessagesDeletedForUsers_AspNetUsers_Id_fk
            references AspNetUsers,
    Id         int identity
        primary key
)
go

