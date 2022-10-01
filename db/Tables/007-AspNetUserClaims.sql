create table AspNetUserClaims
(
    Id         int identity
        constraint PK_AspNetUserClaims
            primary key,
    UserId     nvarchar(450) not null collate SQL_Latin1_General_CP1_CI_AS
        constraint FK_AspNetUserClaims_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    ClaimType  nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
    ClaimValue nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS
)
go

create index IX_AspNetUserClaims_UserId
    on AspNetUserClaims (UserId)
go

