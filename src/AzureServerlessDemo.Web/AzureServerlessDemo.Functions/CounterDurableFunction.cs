using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace AzureServerlessDemo.Functions;
/// <summary>
/// demo from 
/// </summary>
/// <example>
///  curl -X POST -H "Content-Length: 0" "http://localhost:7071/runtime/webhooks/durabletask/entities/Counter/MyCounter?op=Reset"
//   curl -d "1" -X POST -H "Content-Type: application/json" http://localhost:7071/runtime/webhooks/durabletask/entities/Counter/MyCounter?op=Add
//   curl -d "2" -X POST -H "Content-Type: application/json" http://localhost:7071/runtime/webhooks/durabletask/entities/Counter/MyCounter?op=Add
//   curl http://localhost:7071/runtime/webhooks/durabletask/entities/Counter/MyCounter
/// </example>
public static class CounterDurableFunction
{
    public class Counter
    {
        [JsonProperty("value")]
        public int CurrentValue { get; set; }

        public void Add(int amount) => CurrentValue += amount;
        
        public void Reset() => CurrentValue = 0;
        
        public int Get() => CurrentValue;

        [FunctionName(nameof(Counter))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Counter>();
    }
}