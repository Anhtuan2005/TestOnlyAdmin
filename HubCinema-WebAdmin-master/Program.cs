using HubCinemaAdmin.Middlewares;
using HubCinemaAdmin.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRoomService, RoomService>();
// builder.Services.AddScoped<ICinemaService, CinemaService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ShowtimeService>();
builder.Services.AddScoped<SeatLayoutService>();



builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseMiddleware<RequireLoginMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
