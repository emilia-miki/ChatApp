using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ChatApp.DAL.Entities;

namespace ChatApp.DAL;

public class ChatsContext : IdentityDbContext<ApplicationUser>
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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Chat>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();
        builder.Entity<Message>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();
        builder.Entity<MemberChat>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();
        builder.Entity<MessageDeletedForUser>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Entity<Chat>()
            .HasMany(c => c.MembersChats)
            .WithOne(mc => mc.Chat)
            .HasForeignKey(mc => mc.ChatId);
        builder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.UserId);

        base.OnModelCreating(builder);
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