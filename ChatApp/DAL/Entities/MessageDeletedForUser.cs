using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChatApp.DAL.Entities;

public class MessageDeletedForUser
{
    [Key]
    public int Id { get; set; }
    public int MessageId { get; set; }
    public string UserId { get; set; }
    
    [JsonIgnore]
    public Message Message { get; set; }
}
