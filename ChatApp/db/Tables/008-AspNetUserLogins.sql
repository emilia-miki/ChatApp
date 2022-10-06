create table AspNetUserLogins
(
    LoginProvider       nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS,
    ProviderKey         nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS,
    ProviderDisplayName nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
    UserId              nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS
        constraint FK_AspNetUserLogins_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    constraint PK_AspNetUserLogins
        primary key (LoginProvider, ProviderKey)
)
go

create index IX_AspNetUserLogins_UserId
    on AspNetUserLogins (UserId)
go

