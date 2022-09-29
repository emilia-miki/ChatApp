using System.ComponentModel.DataAnnotations;

namespace ChatApp.DAL.Entities;

public class MessageDeletedForUser
{
    [Key]
    public int Id { get; set; }
    public int MessageId { get; set; }
    public string UserId { get; set; }
}
