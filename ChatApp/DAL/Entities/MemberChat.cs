using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChatApp.DAL.Entities;

public class MemberChat
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public int ChatId { get; set; }
    
    [JsonIgnore]
    public Chat Chat { get; set; }
}
