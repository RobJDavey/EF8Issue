using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;
using WebAPI;
using Xunit.Abstractions;

namespace Tests;

public class BlogTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public Task TestStaticReadonly1() => RunAsync<BlogContext>(context =>
    {
        var query = BlogQueryHelper.CompiledStaticReadonly(context, 8)
            .ToList();

        Assert.Empty(query);
    });

    [Fact]
    public Task TestStaticReadonly2() => RunAsync<BlogContext>(context =>
    {
        var query = BlogQueryHelper.CompiledStaticReadonly(context, 8)
            .ToList();

        Assert.Empty(query);
    });

    [Fact]
    public Task TestReadonly1() => RunAsync<BlogContext>(context =>
    {
        var helper = new BlogQueryHelper();

        var query = helper.CompiledReadonly(context, 8)
            .ToList();

        Assert.Empty(query);
    });

    [Fact]
    public Task TestReadonly2() => RunAsync<BlogContext>(context =>
    {
        var helper = new BlogQueryHelper();

        var query = helper.CompiledReadonly(context, 8)
            .ToList();

        Assert.Empty(query);
    });

    private Task RunAsync<T>(Action<T> action) where T : notnull => RunAsync(serviceProvider =>
    {
        var t = serviceProvider.GetRequiredService<T>();
        action(t);
    });

    private async Task RunAsync(Action<IServiceProvider> action)
    {
        await using var testContainer = new PostgreSqlBuilder()
            .Build();

        await testContainer.StartAsync();

        var connectionString = new NpgsqlConnectionStringBuilder(testContainer.GetConnectionString())
            {
                Database = "blog",
            }
            .ConnectionString;

        testOutputHelper.WriteLine(connectionString);

        var services = new ServiceCollection();

        services.AddNpgsqlDataSource(connectionString);

        services.AddDbContext<BlogContext>(options =>
        {
            options.UseNpgsql();
            options.EnableServiceProviderCaching(false);
        });

        await using var serviceProvider = services.BuildServiceProvider();

        await using (var scope = serviceProvider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        await using (var scope = serviceProvider.CreateAsyncScope())
        {
            action(scope.ServiceProvider);
        }
    }
}
