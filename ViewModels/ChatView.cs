namespace ChatApp.ViewModels;

public record ChatView
(
    string ChatName,
    string LastMessageSender,
    string LastMessageText,
    DateTime LastMessageTime,
    bool IsPersonal
);