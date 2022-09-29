namespace ChatApp.ViewModels;

public class MessageView
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Text { get; set; }
    public DateTime DateTime { get; set; }
    public int ReplyTo { get; set; }
    public bool ReplyIsPersonal { get; set; }
}