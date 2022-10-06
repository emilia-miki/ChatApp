create procedure GetUserViews 
    @userId nvarchar(450),
    @skip int,
    @size int,
    @sortBy varchar(32),
    @sortDesc bit
as 
select 
    ANU.UserName as Login, 
    ANU.Email as Email,
    ANU.PhoneNumber as Phone,
    max(M.DateTime) as LastMessageTime
from AspNetUsers ANU
left join MembersChats MC on ANU.Id = MC.UserId
left join Messages M on MC.ChatId = M.ChatId
where ANU.Id != @userId
group by ANU.UserName, ANU.Email, ANU.PhoneNumber
order by 
case when @sortDesc = 0 then
    case @sortBy
        when 'Login' then ANU.UserName
        when 'Email' then ANU.Email
        when 'Phone' then ANU.PhoneNumber
        when 'LastMessageTime' then max(M.DateTime)
    end 
end,
case when @sortDesc = 1 then
     case @sortBy
        when 'Login' then ANU.UserName
        when 'Email' then ANU.Email
        when 'Phone' then ANU.PhoneNumber
        when 'LastMessageTime' then max(M.DateTime)
     end
end desc
offset @skip rows
fetch next @size rows only
go

