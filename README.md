# efcore.bulkextensions.tests

Recreates [Github issue 610](https://github.com/borisdj/EFCore.BulkExtensions/issues/610): `BulkInsertOrUpdateAsync` fails when PK is `long`.

## How to setup

1. Run [schema.sql](.\efcore\SCHEMA.sql) to create test tables.
1. Update the connection string in [UnitTest1.cs](.\efcore.test\UnitTest1.cs).
1. Run all tests. Use debug to see SQL output to console.

## Notes

Example `string` PK SQL: 

```sql
MERGE [dbo].[RevenueScheduleString] WITH (HOLDLOCK) AS T 
USING (
        SELECT TOP 10 * 
        FROM [dbo].[#RevenueScheduleStringTemp4bed81fc] 
        ORDER BY [RevenueScheduleId]
) AS S 
    ON T.[RevenueScheduleId] = S.[RevenueScheduleId] 
WHEN NOT MATCHED BY TARGET THEN INSERT 
    -- RevenueScheduleId is in the merge statement when PK is string 
    ([RevenueScheduleId], [BatchId]) VALUES (S.[RevenueScheduleId], S.[BatchId]) 

WHEN MATCHED AND EXISTS (SELECT S.[RevenueScheduleId], S.[BatchId] EXCEPT SELECT T.[RevenueScheduleId], T.[BatchId]) THEN UPDATE SET T.[BatchId] = S.[BatchId];
```

Example `long` PK SQL: 

```sql
MERGE [dbo].[RevenueScheduleLong] WITH (HOLDLOCK) AS T 
USING (
        SELECT TOP 10 * 
        FROM [dbo].[#RevenueScheduleLongTempd17e5168] 
        ORDER BY [RevenueScheduleId]
) AS S 
    ON T.[RevenueScheduleId] = S.[RevenueScheduleId] 
        
-- RevenueScheduleId is missing
WHEN NOT MATCHED BY TARGET THEN INSERT ([BatchId]) VALUES (S.[BatchId]) 

WHEN MATCHED AND EXISTS (SELECT S.[BatchId] EXCEPT SELECT T.[BatchId]) THEN UPDATE SET T.[BatchId] = S.[BatchId];
```