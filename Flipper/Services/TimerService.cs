namespace Flipper.Services;

public class TimerService : BackgroundService, IDisposable
{
    private readonly HttpNinjaService _httpNinjaService;
    private Timer? _timer = null;

    public TimerService(HttpNinjaService httpNinjaService)
    {
        _httpNinjaService = httpNinjaService;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromHours(2));
        return Task.CompletedTask;
    }


    private void DoWork(object? state)
    {
        _httpNinjaService.StartSync();
    }


    public void Dispose()
    {
        _timer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_timer != null) await _timer.DisposeAsync();
    }
}