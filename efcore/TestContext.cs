using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace efcore
{
    public class TestContext : DbContext
    {
        public DbSet<RevenueScheduleLong> RevenueScheduleLong { get; set; }

        public DbSet<RevenueScheduleString> RevenueScheduleString { get; set; }

        public TestContext(DbContextOptions options)
            : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<RevenueScheduleLong>(entity =>
            {
                entity
                    .ToTable(nameof(RevenueScheduleLong));

                entity.HasKey(x => x.RevenueScheduleId);
            });

            builder.Entity<RevenueScheduleString>(entity =>
            {
                entity
                    .ToTable(nameof(RevenueScheduleString));

                entity.HasKey(x => x.RevenueScheduleId);
            });
        }

        public async Task BulkMerge<T>(IList<T> records, CancellationToken cancellationToken = default) where T : class
        {
            using (var transaction = await Database.BeginTransactionAsync())
            {
                try
                {
                    await this.BulkInsertOrUpdateAsync(records, new BulkConfig()
                    {
                        BatchSize = 1000,
                        UseTempDB = true
                    });

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            };
        }

        public static TestContext Create(string connectionString)
        {
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder<TestContext>()
                        .UseSqlServer(connectionString)
                        .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                        .EnableSensitiveDataLogging(true);

            return new TestContext(builder.Options);
        }
    }
}
