using Azure;
using Azure.Data.Tables;

namespace AzureServerlessDemo.Core;

public class LogModel : ITableEntity
{
    public LogModel()
    {
        PartitionKey = "serverlesslogs";
        RowKey = Guid.NewGuid().ToString();
    }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime LoggedDate { get; set; } = DateTime.Now;
    public string CalledFromMethod { get; set; } =  string.Empty;
}