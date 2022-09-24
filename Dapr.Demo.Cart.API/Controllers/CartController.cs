using Dapr.Client;
using Dapr.Demo.Cart.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dapr.Demo.Cart.API.Controllers;

 [Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ILogger<CartController> _logger;
    private readonly DaprClient _daprClient;

    public CartController(ILogger<CartController> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }
    /// <summary>
    /// CartService通过DaprClient的InvokeMethod调用ProductService服务的Product进行通信
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IEnumerable<SKU>> Get()
    {
        _logger.LogInformation("[Begin] CartService调用ProductService--Query product data from Product Service");

        //注意ProductService的ProductController路由配置是api/[controller]
        var products = await _daprClient.InvokeMethodAsync<IEnumerable<SKU>>
            (HttpMethod.Get, "ProductService", "api/Product");

        _logger.LogInformation($"[End] CartService调用ProductService--Query product data from Product Service, data : {products.ToArray().ToString()}");

        return products;
    }
}