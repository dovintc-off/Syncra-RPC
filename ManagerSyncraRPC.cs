using System;
using System.Threading;
using System.Threading.Tasks;

namespace SyncraRPC;

public class ManagerSyncraRpc
{
    private Princess? princess = new Princess("1515830709384646776");
    private CancellationTokenSource? _cts;
    private Task? _workerTask;
    private bool _isActive;

    public bool IsActive => _isActive;

    public void Start()
    {
        if (_isActive) return;
        _isActive = true;
        _cts = new CancellationTokenSource();
        
        _workerTask = Task.Run(() => BackgroundWorker(_cts.Token));
    }

    public void Stop()
    {
        if (!_isActive) return;

        _isActive = false;
        _cts?.Cancel(); 

        try { _workerTask?.Wait(500); }
        catch (AggregateException) {}
        princess?.close();

        _cts?.Dispose();
        _cts = null;
        _workerTask = null;

        Console.WriteLine("[Поток 1] Остановка прошла успешно");
    }

    private void BackgroundWorker(CancellationToken token)
    {
        Console.WriteLine("[Поток 2] Фоновый поток работает.");

        try
        {
            princess?.connect();

            double start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;

            while (!token.IsCancellationRequested)
            {
                Console.WriteLine($"[Поток 2] Обновление статуса");

                princess?.update(
                    name: "Watch on Syncra RPC",
                    state: "Syncra RPC is best RPC hub",
                    start: start
                );

                Task.Delay(5000, token).Wait(token);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("[Поток 2] Выход осуществлен успешно");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Поток 2] Критическая ошибка в цикле: {ex.Message}");
            _isActive = false;
        }
        finally
        {
            Console.WriteLine("[Поток 2] Поток завершен");
        }
    }
}
