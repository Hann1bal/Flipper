namespace Flipper.Services;

public class TimerService : BackgroundService, IDisposable
{
    private readonly HttpNinjaService _httpNinjaService;
    private readonly CharacterListDownloaderService _characterList;
    private Timer? _timer = null;
    private Timer? _timer2 = null;
    
    public TimerService(HttpNinjaService httpNinjaService, CharacterListDownloaderService characterList)
    {
        _httpNinjaService = httpNinjaService;
        _characterList = characterList;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromHours(2));
        _timer2 = new Timer(DoWork2, null, TimeSpan.Zero,
            TimeSpan.FromHours(24));
        return Task.CompletedTask;
    }


    private async void DoWork(object? state)
    {
        Console.WriteLine($"Start sync cards in {DateTime.UtcNow.ToString()}");
        _httpNinjaService.StartSync();
    }
    private async void DoWork2(object? state)
    {
        Console.WriteLine($"Start sync Accounts in {DateTime.UtcNow.ToString()}");
        _characterList.GetAccountFromLadder();
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _timer2?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_timer != null) await _timer.DisposeAsync();
        if (_timer2 != null) await _timer2.DisposeAsync();
    }
}