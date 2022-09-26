create table AspNetRoles
(
    Id               nvarchar(450) not null
        constraint PK_AspNetRoles
            primary key,
    Name             nvarchar(256),
    NormalizedName   nvarchar(256),
    ConcurrencyStamp nvarchar(max)
)
go

create unique index RoleNameIndex
    on AspNetRoles (NormalizedName)
    where [NormalizedName] IS NOT NULL
go

