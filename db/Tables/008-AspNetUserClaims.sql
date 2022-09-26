create table AspNetUserClaims
(
    Id         int identity
        constraint PK_AspNetUserClaims
            primary key,
    UserId     nvarchar(450) not null
        constraint FK_AspNetUserClaims_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    ClaimType  nvarchar(max),
    ClaimValue nvarchar(max)
)
go

create index IX_AspNetUserClaims_UserId
    on AspNetUserClaims (UserId)
go

