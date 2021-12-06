namespace AzureServerlessDemo.Core;

public class LogModel
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Text { get; set; }
    public DateTime LoggedDate { get; set; }
    public string CalledFromMethod { get; set; }
}