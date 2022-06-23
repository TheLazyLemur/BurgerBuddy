using BurgerBuddy.Data;
using Microsoft.AspNetCore.Components;

namespace BurgerBuddy.Pages;

public partial class Index
{
       [Inject] public IOrderService? OrderService { get; set; }

       private List<Order>? _ordersAccepted = new();
       private List<Order>? _ordersPreparing = new();
       private List<Order>? _ordersReady = new();
       private List<Order>? _ordersCompleted = new();
       private Timer? _timer;


       protected override async Task OnInitializedAsync()
       {
           _timer = new Timer(UpdateData, null, TimeSpan.Zero,
               TimeSpan.FromSeconds(1));
        
           await UpdateData();
       }

       private async void UpdateData(object? state)
       {
           await UpdateData();
       }

       private async Task UpdateData()
       {
           var ordersFromDb = await OrderService.Get((int)OrderStatus.Accepted);
           _ordersAccepted = ordersFromDb?.ToList();

           ordersFromDb = await OrderService!.Get((int)OrderStatus.Preparing);
           _ordersPreparing = ordersFromDb?.ToList();

           ordersFromDb = await OrderService!.Get((int)OrderStatus.ReadyForCollection);
           _ordersReady = ordersFromDb?.ToList();

           ordersFromDb = await OrderService!.Get((int)OrderStatus.Completed);
           _ordersCompleted = ordersFromDb?.ToList();

           await InvokeAsync(StateHasChanged);
       }
}