create table AspNetUserTokens
(
    UserId        nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS
        constraint FK_AspNetUserTokens_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    LoginProvider nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS,
    Name          nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS,
    Value         nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
    constraint PK_AspNetUserTokens
        primary key (UserId, LoginProvider, Name)
)
go

