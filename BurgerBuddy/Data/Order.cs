namespace BurgerBuddy.Data;

public class Order
{
    public long? Id { get; set; }
    public OrderItem OrderItem { get; set; }
    public DateTime OrderTime { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public OrderOrigin OrderOrigin { get; set; }
    public int OrderId { get; set; }
}