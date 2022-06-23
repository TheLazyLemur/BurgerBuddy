using System.Data;
using System.Data.SQLite;
using BurgerBuddy.Data;
using Dapper;

namespace BurgerBuddy.DataAccess;

public interface IOrderRepository
{
    Task<Order> Insert(Order order);
    Task<IEnumerable<Order>> Get(int orderStatus);
    Task Update(long id, int status);
    Task<IEnumerable<Order>> GetForDay(DateTime orderOrderTime);
    Task<IEnumerable<Order>> GetForDayAndMealType(DateTime orderOrderTime, int orderItem);
}

public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(IConfiguration config)
    {
        _connectionString = config.GetValue<string>("ConnectionString");
    }
    
    public async Task<Order> Insert(Order order)
    {
        using IDbConnection connection = new SQLiteConnection(_connectionString);

        var result = await connection.QuerySingleAsync(
            "INSERT INTO order_table(order_item, date_time, order_status, order_id, order_origin) values (@orderItem, @dt, @orderStatus, @orderId, @orderOrigin) RETURNING *;",
            new
            {
                orderItem = order.OrderItem,
                dt = order.OrderTime,
                orderStatus = order.OrderStatus,
                orderId = order.OrderId,
                orderOrigin = order.OrderOrigin
            }
        );
        
        return new Order
        {
            Id = result.id,
            OrderItem = result.order_item is OrderItem ? (OrderItem)result.order_item : OrderItem.Burger,
            OrderStatus = result.order_status is OrderStatus ? (OrderStatus)result.order_status : OrderStatus.Accepted,
            OrderTime = result.date_time,
            OrderId = result.order_id,
            OrderOrigin = result.OrderOrigin is OrderOrigin ? (OrderOrigin)result.order_origin : OrderOrigin.Pos
        };
    }

    public async Task<IEnumerable<Order>> GetForDay(DateTime dateTime)
    {
        using IDbConnection connection = new SQLiteConnection(_connectionString);
        
        var result = await connection.QueryAsync<dynamic>(
            "SELECT * FROM order_table WHERE DATE(date_time) = DATE(@dateTime,'-0 day');",
            new { dateTime }
        );
        
        return result.ToList().Select(it => new Order
        {
            Id = it.id,
            OrderItem = it.order_item is OrderItem ? (OrderItem)it.order_item : OrderItem.Burger,
            OrderStatus = it.order_status is OrderStatus ? (OrderStatus)it.order_status : OrderStatus.Accepted,
            OrderTime = it.date_time,
            OrderId = it.order_id,
            OrderOrigin = it.order_origin is OrderOrigin ? (OrderOrigin)it.order_origin : OrderOrigin.Pos
        });
    }
    
    public async Task<IEnumerable<Order>> GetForDayAndMealType(DateTime dateTime, int orderItem)
    {
        using IDbConnection connection = new SQLiteConnection(_connectionString);
        
        var result = await connection.QueryAsync<dynamic>(
            "SELECT * FROM order_table WHERE DATE(date_time) = DATE(@dateTime,'-0 day') AND order_item = @orderItem;",
            new { dateTime, orderItem }
        );
        
        return result.ToList().Select(it => new Order
        {
            Id = it.id,
            OrderItem = it.order_item is OrderItem ? (OrderItem)it.order_item : OrderItem.Burger,
            OrderStatus = it.order_status is OrderStatus ? (OrderStatus)it.order_status : OrderStatus.Accepted,
            OrderTime = it.date_time,
            OrderId = it.order_id,
            OrderOrigin = it.order_origin is OrderOrigin ? (OrderOrigin)it.order_origin : OrderOrigin.Pos
        });
    }
    
    public async Task<IEnumerable<Order>> Get(int orderStatus)
    {
        using IDbConnection connection = new SQLiteConnection(_connectionString);

        var result = await connection.QueryAsync<dynamic>(
            "SELECT * FROM order_table WHERE order_status = @orderStatus;",
            new { orderStatus }
        );
        
        return result.ToList().Select(it => new Order
        {
            Id = it.id,
            OrderItem = it.order_item is OrderItem ? (OrderItem)it.order_item : OrderItem.Burger,
            OrderStatus = it.order_status is OrderStatus ? (OrderStatus)it.order_status : OrderStatus.Accepted,
            OrderTime = it.date_time,
            OrderId = it.order_id,
            OrderOrigin = it.order_origin is OrderOrigin ? it.order_status : OrderOrigin.Pos
        });
    }

    public async Task Update(long id, int status)
    {
        using IDbConnection connection = new SQLiteConnection(_connectionString);
        await connection.ExecuteAsync("UPDATE order_table SET order_status = @status WHERE id = @id", new {id, status});
    }
}