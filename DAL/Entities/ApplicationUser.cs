using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.DAL.Entities;

public class ApplicationUser : IdentityUser
{
    [JsonIgnore]
    public ICollection<Message> Messages { get; set; }
}