using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChatApp.DAL.Entities;

public class Message
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Text { get; set; }

    public int ChatId { get; set; }
    public DateTime DateTime { get; set; }
    public int ReplyTo { get; set; }
    
    [JsonIgnore]
    public ApplicationUser User { get; set; }
}
