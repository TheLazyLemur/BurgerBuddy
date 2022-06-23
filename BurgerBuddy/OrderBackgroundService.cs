using BurgerBuddy.Data;

namespace BurgerBuddy;

public class OrderBackgroundService : IHostedService, IDisposable
{
    private int _executionCount;
    private readonly ILogger<OrderBackgroundService> _logger;
    private readonly IOrderService _orderService;
    private Timer? _timer;
    
    public OrderBackgroundService(ILogger<OrderBackgroundService> logger, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order service is running");

        _timer = new Timer(CookTheFood, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }
    
    private void CookTheFood(object? state)
    {
        Interlocked.Increment(ref _executionCount);
        
        StartPreparingAcceptedOrders();
        CheckForReadyOrders();
        MarkOrdersAsCollected();

        _logger.LogInformation(
            "Order Service is working");
    }

    private async void StartPreparingAcceptedOrders()
    {
        var acceptedOrders = await _orderService.Get((int)OrderStatus.Accepted);
        if(acceptedOrders is null)
            return;
        
        foreach (var order in acceptedOrders.ToList())
        {
            _orderService.UpdateOrderStatus(order.Id.Value, (int)OrderStatus.Preparing);
        }
    }

    private async void CheckForReadyOrders()
    {
        var ordersBeingPrepared = await _orderService.Get((int)OrderStatus.Preparing);
        if(ordersBeingPrepared is null)
            return;
        
        foreach (var order in ordersBeingPrepared.ToList())
        {
            if (DateTime.Now >= order.OrderTime.AddSeconds(10))
            {
                _orderService.UpdateOrderStatus(order.Id.Value, (int)OrderStatus.ReadyForCollection);
            }
        }
    }

    private async void MarkOrdersAsCollected()
    {
        var ordersReadyForCollection = await _orderService.Get((int)OrderStatus.ReadyForCollection);
        if(ordersReadyForCollection is null)
            return;
        
        foreach (var order in ordersReadyForCollection.ToList())
        {
            _orderService.UpdateOrderStatus(order.Id.Value, (int)OrderStatus.Completed);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}