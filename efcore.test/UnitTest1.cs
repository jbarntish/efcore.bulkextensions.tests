using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace efcore.test
{
    public class UnitTest1
    {
        private const string ConnectionString = "server=localhost;initial catalog=Test;user id=<THE USER>;password=<THE PASSWORD>";

        [Fact]
        public async Task RevenueScheduleLong_Fail()
        {
            List<RevenueScheduleLong> records = new List<RevenueScheduleLong>();

            var batch = Guid.NewGuid().ToString();

            for (int i = 0; i < 10; i++)
            {
                records.Add(new RevenueScheduleLong()
                {
                    BatchId = batch,
                    RevenueScheduleId = long.MaxValue - i
                });
            }

            using var db = TestContext.Create(ConnectionString);

            await db.BulkMerge(records);
        }

        [Fact]
        public async Task RevenueScheduleString_Success()
        {
            List<RevenueScheduleString> records = new List<RevenueScheduleString>();

            var batch = Guid.NewGuid().ToString();

            for (int i = 0; i < 10; i++)
            {
                records.Add(new RevenueScheduleString()
                {
                    BatchId = batch,
                    RevenueScheduleId = (long.MaxValue - i).ToString()
                });
            }

            using var db = TestContext.Create(ConnectionString);

            await db.BulkMerge(records);
        }
    }
}
