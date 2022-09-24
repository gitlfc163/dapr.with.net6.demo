using Dapr.Client;
using Dapr.Demo.Order.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Dapr.Demo.Order.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private const string DaprPubSubName = "pubsub";

    private readonly ILogger<OrderController> _logger;
    private readonly DaprClient _daprClient;

    public OrderController(ILogger<OrderController> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    [HttpPost]
    public async Task<Models.Order> Post(OrderDto orderDto)
    {
        _logger.LogInformation("[Begin] Create Order.");

        var order = new Models.Order()
        {
            // some mapping
            Id = orderDto.Id,
            ProductId = orderDto.ProductId,
            Count = orderDto.Count
        };
        // some other logic for order

        var orderStockDto = new OrderStockDto()
        {
            ProductId = orderDto.ProductId,
            Count = orderDto.Count
        };
        await _daprClient.PublishEventAsync(DaprPubSubName, "neworder", orderStockDto);

        _logger.LogInformation($"[End] Create Order Finished. Id : {orderStockDto.ProductId}, Count : {orderStockDto.Count}");

        return order;
    }
}

