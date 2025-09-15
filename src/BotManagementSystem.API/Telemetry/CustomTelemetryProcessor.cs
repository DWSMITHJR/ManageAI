using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace BotManagementSystem.API.Telemetry
{
    public class CustomTelemetryProcessor : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public CustomTelemetryProcessor(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            // Example: Filter out requests to health check endpoints from telemetry
            if (item is RequestTelemetry request && 
                (request.Url.AbsolutePath.Contains("/health")))
            {
                return;
            }

            // Send everything else
            _next.Process(item);
        }
    }
}
