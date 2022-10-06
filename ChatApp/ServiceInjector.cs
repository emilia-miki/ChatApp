using Blazorise;
using Blazorise.Bootstrap;
using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Services;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp;

public static class ServiceInjector
{
    public static void Configure(
        IConfiguration configuration,
        IServiceCollection services) 
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddControllers();
        services.AddSignalR();
        services
            .AddBlazorise(options =>
            {
                options.Immediate = true;
            })
            .AddBootstrapProviders();

        var baseUri = new Uri(configuration.GetValue<string>("BaseUri"));
        services.AddScoped(_ => new HttpClient {BaseAddress = baseUri});

        services.AddTransient<IAuthOptions, AuthOptions>();
        services.AddDbContext<ChatsContext>(options => options
            .UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddTransient<UserRepository>();
        services.AddTransient<MessageRepository>();
        services.AddTransient<ChatRepository>();
        services.AddTransient<GenericRepository<MessageDeletedForUser>>();
        services.AddTransient<MemberChatRepository>();

        services.AddTransient<IAuthorizationService, AuthorizationService>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IChatService, ChatService>();
        services.AddTransient<IAuthorizationService, AuthorizationService>();
        services.AddTransient<IViewService, ViewService>();
        services.AddTransient<BLL.Interfaces.IMessageService, MessageService>();

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<ChatsContext>()
            .AddDefaultTokenProviders();

        var authOptions = new AuthOptions(configuration);
        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = authOptions.Audience,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = authOptions.Key
            };
        });
    }
}