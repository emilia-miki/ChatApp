create table AspNetUserRoles
(
    UserId nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS
        constraint FK_AspNetUserRoles_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    RoleId nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS
        constraint FK_AspNetUserRoles_AspNetRoles_RoleId
            references AspNetRoles
            on delete cascade,
    constraint PK_AspNetUserRoles
        primary key (UserId, RoleId)
)
go

create index IX_AspNetUserRoles_RoleId
    on AspNetUserRoles (RoleId)
go

