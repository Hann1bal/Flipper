namespace Flipper.Services;

public class TimerService : BackgroundService, IDisposable
{
    private readonly HttpNinjaService _httpNinjaService;
    private readonly CharacterListDownloaderService _characterList;
    private Timer? _timer = null;
    
    public TimerService(HttpNinjaService httpNinjaService, CharacterListDownloaderService characterList)
    {
        _httpNinjaService = httpNinjaService;
        _characterList = characterList;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromHours(2));
        return Task.CompletedTask;
    }


    private async void DoWork(object? state)
    {
        _httpNinjaService.StartSync();
        await _characterList.GetCsv();
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