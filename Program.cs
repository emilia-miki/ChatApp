using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChatApp.BLL;
using ChatApp.Controllers;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();
builder.Services.AddSignalR();

var baseUri = new Uri(builder.Configuration.GetValue<string>("BaseUri"));
builder.Services.AddScoped(_ => new HttpClient {BaseAddress = baseUri});

builder.Services.AddTransient<IAuthOptions, AuthOptions>();
builder.Services.AddDbContext<ChatsContext>(options => options
    .UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<MessageRepository>();
builder.Services.AddTransient<ChatRepository>();
builder.Services.AddTransient<GenericRepository<MessageDeletedForUser>>();
builder.Services.AddTransient<GenericRepository<MemberChat>>();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IViewService, ViewService>();
builder.Services.AddTransient<IMessageService, MessageService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
    }).AddEntityFrameworkStores<ChatsContext>()
    .AddDefaultTokenProviders();

var authOptions = new AuthOptions(builder.Configuration);
builder.Services.AddAuthentication(auth =>
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

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapHub<ChatHub>("/hub");
app.MapFallbackToPage("/_Host");

app.Run();