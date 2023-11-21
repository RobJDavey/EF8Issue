using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;
using WebAPI;

await using var sql = new PostgreSqlBuilder()
    .Build();

await sql.StartAsync();

var builder = WebApplication.CreateBuilder(args);

var connectionString = new NpgsqlConnectionStringBuilder(sql.GetConnectionString())
    {
        Database = "blog",
    }
    .ConnectionString;

builder.Services.AddNpgsqlDataSource(connectionString);

builder.Services.AddDbContext<BlogContext>(options =>
{
    options.UseNpgsql();
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
    await context.Database.EnsureCreatedAsync();
}

app.MapGet("/{id:int}", (BlogContext context, int id) => BlogQueryHelper.CompiledStaticReadonly(context, id).ToList());

app.Run();
