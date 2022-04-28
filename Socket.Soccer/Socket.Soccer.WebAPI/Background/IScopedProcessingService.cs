namespace Socket.Soccer.WebAPI.Background
{
    internal interface IScopedProcessingService
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}
