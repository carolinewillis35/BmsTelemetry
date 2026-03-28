using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<TelemetryRecord> Telemetry { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TelemetryRecord>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Ip)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.DeviceKey)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.DataKey)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.DataValue)
                  .IsRequired()
                  .HasMaxLength(500);

            entity.Property(e => e.Timestamp)
                  .IsRequired();

            // Useful indexes for telemetry workloads
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => new { e.DeviceKey, e.Timestamp });
            entity.HasIndex(e => new { e.Ip, e.Timestamp });
            entity.HasIndex(e => new { e.DeviceKey, e.DataKey });
        });
    }
}
