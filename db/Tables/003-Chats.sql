create table Chats
(
    Name       nvarchar(64) not null,
    Id         int identity
        primary key,
    IsPersonal bit          not null
)
go

