using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChatApp.DAL.Entities;

public class Chat
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsPersonal { get; set; }

    [JsonIgnore] 
    public ICollection<MemberChat> MembersChats { get; set; }
    [JsonIgnore]
    public ICollection<Message> Messages { get; set; }
}
