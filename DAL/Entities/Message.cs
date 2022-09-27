namespace ChatApp.DAL.Entities;

public record Message
(
    int Id,
    string UserId,
    int ChatId,
    DateTime DateTime,
    int ReplyTo,
    string ReplyIsPersonal
);
