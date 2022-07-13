using System.Net.NetworkInformation;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheckApi;

public class PingHealthCheck : IHealthCheck
{
    private readonly string _host;
    private readonly int _timeout;

    public PingHealthCheck(string host, int timeout)
    {
        _host = host;
        _timeout = timeout;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(_host, _timeout);

            if (reply.Status != IPStatus.Success) return HealthCheckResult.Unhealthy();

            return reply.RoundtripTime > 100 ? HealthCheckResult.Degraded() : HealthCheckResult.Healthy();
        }
        catch
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}