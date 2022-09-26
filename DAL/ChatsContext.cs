using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ChatApp.DAL.Entities;

namespace ChatApp.DAL;

public class ChatsContext : IdentityDbContext
{
    public DbSet<Chat> Chats { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<MessageDeletedForUser> MessagesDeletedForUsers { get; set; } 
        = null!;
    public DbSet<MemberChat> MembersChats { get; set; } = null!;

    private readonly IConfiguration _config;

    public ChatsContext(IConfiguration config)
    {
        _config = config;
    }
    
    public ChatsContext(DbContextOptions<ChatsContext> options,
        IConfiguration config) : base(options)
    {
        _config = config;
    }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                _config.GetConnectionString("Default"));
        }
        
        optionsBuilder.UseSqlServer(
            _config.GetConnectionString("Default"));
        base.OnConfiguring(optionsBuilder);
    }
}