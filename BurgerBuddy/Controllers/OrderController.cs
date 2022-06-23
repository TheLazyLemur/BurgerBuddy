using BurgerBuddy.Data;
using Microsoft.AspNetCore.Mvc;

namespace BurgerBuddy.Controllers;

public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpGet(Routes.Orders)]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.Get((int)OrderStatus.ReadyForCollection);
        return Ok(orders);
    }

    [HttpPost(Routes.Orders)]
    public async Task<IActionResult> NewOrder([FromBody] Order order)
    {
        try
        {
            var result = await _orderService.Insert(order);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}