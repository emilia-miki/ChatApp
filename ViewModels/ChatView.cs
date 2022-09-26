namespace ChatApp.ViewModels;

public record ChatView
(
    string ChatName,
    string LastMessageSender,
    string LastMessageText,
    bool IsPersonal
);