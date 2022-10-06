create table AspNetRoles
(
    Id               nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS
        constraint PK_AspNetRoles
            primary key,
    Name             nvarchar(256) collate SQL_Latin1_General_CP1_CI_AS,
    NormalizedName   nvarchar(256) collate SQL_Latin1_General_CP1_CI_AS,
    ConcurrencyStamp nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS
)
go

create unique index RoleNameIndex
    on AspNetRoles (NormalizedName)
    where [NormalizedName] IS NOT NULL
go

