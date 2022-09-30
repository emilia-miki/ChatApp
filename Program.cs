using ChatApp.Controllers;
using ChatApp;

var builder = WebApplication.CreateBuilder(args);
ServiceInjector.Configure(builder.Configuration, builder.Services);

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