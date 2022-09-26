namespace ChatApp.ViewModels;

public record MessageView
(
    int Id,
    string Username,
    string Text,
    string Date,
    int ReplyTo,
    bool ReplyIsPersonal
);