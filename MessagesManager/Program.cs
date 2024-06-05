using Core.Interfaces;
using Core.Models.Application;
using Microsoft.AspNetCore.Http.Features;
using Services;
using Services.Data.Stores;
using Services.Interfaces;
using Services.Interfaces.Mappers;
using Services.Mappers;
using Services.Parsers;
using System.IO.Abstractions;

const long TenGigs = 10L * 1024 * 1024 * 1024;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = TenGigs;
});

// TODO PRJ: Is there a more standard way to do this?
// Constructor Parameters
builder.Services.AddSingleton<IFileSystem, FileSystem>();
builder.Services.AddTransient<IMessageParser, FacebookHtmlParser>();
builder.Services.AddTransient<IMessageParser, InstagramHtmlParser>();
builder.Services.AddTransient<IMessageParser, FacebookJsonParser>();
builder.Services.AddTransient<IMessageParser, InstagramJsonParser>();
builder.Services.AddTransient<IMessageParser, ZipFileParser>();
builder.Services.AddTransient<IUnzipService, UnzipService>();

// Mappers
builder.Services.AddTransient<IConversationMapper, ConversationMapper>();
builder.Services.AddTransient<IPersonMapper, PersonMapper>();

// DBContexts
builder.Services.AddDbContext<SqliteContext>(); // TODO PRJ: How do we add / configure which context to use?
builder.Services.AddTransient<IMessageContext, SqliteContext>();

// Service Implementations
builder.Services.AddTransient<IParserDetector, ParserDetector>();
builder.Services.AddTransient<IMessageRepository, EntityFrameworkMessageRepository>();
builder.Services.AddTransient<IMessageImporter, MessageImporter>();

// hosted services
builder.Services.AddHostedService<TempCleanupHostedService>();

// Add session management support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(120);
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = TenGigs;
});

var config = new ApplicationConfiguration();
builder.Configuration.Bind(config);

builder.Services.AddSingleton<ApplicationConfiguration>(config);

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
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
