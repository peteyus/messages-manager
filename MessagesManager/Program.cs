using Services.Data.Stores;
using System.IO.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IFileSystem, FileSystem>();
builder.Services.AddDbContext<SqliteContext>(); // TODO PRJ: How do we add / configure which context to use?

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Configure EF context, if not deployed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<SqliteContext>();
    context.Database.EnsureCreated(); // TODO PRJ: Fine for development, replace with long term storage.
}

    app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
