using System.Data;
using System.Data.SQLite;
using System.Text.Json;
using BurgerBuddy;
using BurgerBuddy.Data;
using BurgerBuddy.DataAccess;
using Dapper;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 1000;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(60);
});

builder.Services.AddHostedService<OrderBackgroundService>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IOrderService, OrderService>();

var app = builder.Build();
SetupDatabase();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

async void SetupDatabase()
{
    var connectionString = app.Configuration.GetValue<string>("ConnectionString");
    using IDbConnection connection = new SQLiteConnection(connectionString);
    await connection.ExecuteAsync(
        "CREATE TABLE IF NOT EXISTS " +
        "order_table(" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT, order_item INT, date_time TIMESTAMP, order_status INT, order_id int" +
        ");"
    );
}