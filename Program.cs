using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChatApp.BLL;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

builder.Services.AddSingleton(new HttpClient
{
    BaseAddress = new Uri("https://localhost:7230")
});

builder.Services.AddTransient<IAuthOptions, AuthOptions>();
builder.Services.AddDbContext<ChatsContext>(options => options
    .UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<IGenericRepository<IdentityUser>, 
    GenericRepository<IdentityUser>>();
builder.Services.AddTransient<IGenericRepository<Message>, 
    GenericRepository<Message>>();
builder.Services.AddTransient<IGenericRepository<Chat>, 
    GenericRepository<Chat>>();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IAccountService, AccountService>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
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
app.MapFallbackToPage("/_Host");

app.Run();