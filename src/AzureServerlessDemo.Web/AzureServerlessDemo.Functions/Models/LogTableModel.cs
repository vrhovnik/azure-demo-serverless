using System;

namespace AzureServerlessDemo.Functions.Models;

public class LogTableModel
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime LoggedDate { get; set; } = DateTime.Now;
    public string CalledFromMethod { get; set; } =  string.Empty;
}