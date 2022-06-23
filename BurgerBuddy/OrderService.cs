using BurgerBuddy.Data;
using BurgerBuddy.DataAccess;

namespace BurgerBuddy;

public interface IOrderService
{
    Task<Order> Insert(Order order);
    Task<IEnumerable<Order>?> Get(int orderStatus);
    void UpdateOrderStatus(long orderId, int preparing);
}

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private static readonly object LockObject = new();

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public Task<Order> Insert(Order order)
    {
        if (order.OrderTime.Hour >= 17)
            throw new Exception("Order time should be less than 5 pm");
        
        lock (LockObject)
        {
            if (order.OrderItem == OrderItem.SmileyMeal)
            {
                var smileyMeals = _orderRepository.GetForDayAndMealType(order.OrderTime, (int)OrderItem.SmileyMeal).Result.ToList();
                if(smileyMeals.Count >= 50)
                    throw new Exception("Only 50 smiley meals can be ordered for a day");
            }
            
            if (order.OrderOrigin == OrderOrigin.MrD)
            {
                var latest = _orderRepository.GetForDay(order.OrderTime).Result.ToList().Where(o => o.OrderId >= 10000).ToList();

                if (latest.Count <= 0)
                    order.OrderId = 10000;
                else
                    order.OrderId = latest.Max(o => o.OrderId) + 1;
                
                return Task.FromResult(_orderRepository.Insert(order).Result);
            }
            else
            {
                var latest = _orderRepository.GetForDay(order.OrderTime).Result.ToList().Where(o => o.OrderId < 10000).ToList();;

                if (latest.Count <= 0)
                    order.OrderId = 1;
                else
                    order.OrderId = latest.Max(o => o.OrderId) + 1;
                
                return Task.FromResult(_orderRepository.Insert(order).Result);
            }
        }
    }

    public async Task<IEnumerable<Order>?> Get(int orderStatus)
    {
        lock (LockObject)
        {
            return _orderRepository.Get(orderStatus).Result;
        }
    }

    public void UpdateOrderStatus(long orderId, int newStatus)
    {
        lock (LockObject)
        {
            _orderRepository.Update(orderId, newStatus);
        }
    }
}