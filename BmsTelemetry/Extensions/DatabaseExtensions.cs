using Microsoft.EntityFrameworkCore;

public static class DatabaseExtensions
{
    public static void AddAppDatabase(this IServiceCollection services, string dbPath)
    {
        var directory = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });
    }

    public static void EnsureAppDatabaseCreated(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Creates the file + tables if missing
        db.Database.EnsureCreated();
    }
}
