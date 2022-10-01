create table AspNetRoleClaims
(
    Id         int identity
        constraint PK_AspNetRoleClaims
            primary key,
    RoleId     nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS
        constraint FK_AspNetRoleClaims_AspNetRoles_RoleId
            references AspNetRoles
            on delete cascade,
    ClaimType  nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
    ClaimValue nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS
)
go

create index IX_AspNetRoleClaims_RoleId
    on AspNetRoleClaims (RoleId)
go

