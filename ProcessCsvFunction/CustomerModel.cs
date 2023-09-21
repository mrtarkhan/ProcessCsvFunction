using Azure;
using Azure.Data.Tables;
using CsvHelper.Configuration.Attributes;

namespace ProcessCsvFunction;

public class CustomerModel : ITableEntity
{

    // csv props
    [Index(2)]
    public string Age { get; set; }
    [Index(3)]
    public string Gender { get; set; }
    [Index(5)]
    public string EducationLevel { get; set; }
    [Index(6)]
    public string MaritalStatus { get; set; }

    // Azure table specific props
    [Ignore]
    public string RowKey { get; set; }
    [Ignore]
    public string PartitionKey { get; set; } = "customer-partition-key";
    [Ignore]
    public ETag ETag { get; set; } = default!;
    [Ignore]
    public DateTimeOffset? Timestamp { get; set; } = DateTime.UtcNow!;

}