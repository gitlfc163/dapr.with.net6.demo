using Dapr.Demo.Product.API.Models;
using Dapr.Demo.Product.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dapr.Demo.Product.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    //因为Dapr默认的pubsub实现是基于Redis的，而在配置中为Redis设置的name就是 pubsub
    //C:/Users/Administrator/.dapr/components/pubsub.yaml或~/.dapr/components/pubsub.yaml
    private const string DaprPubSubName = "pubsub";

    private static readonly string[] FakeProducts = new[]
   {
        "SKU1", "SKU2", "SKU3", "SKU4", "SKU5", "SKU6", "SKU7", "SKU8", "SKU9", "SKU10"
    };

    private readonly ILogger<ProductController> _logger;
    private readonly IProductService _productService;

    public ProductController(ILogger<ProductController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }
    

    /// <summary>
    /// 对应ProductService的接口默认返回一些假数据
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IEnumerable<SKU> Get()
    {
        _logger.LogInformation("[Begin] Query product data.");

        var rng = new Random();
        var result = Enumerable.Range(1, 5).Select(index => new SKU
        {
            Date = DateTime.Now.AddDays(index),
            Index = rng.Next(1, 100),
            Summary = FakeProducts[rng.Next(FakeProducts.Length)]
        })
        .ToArray();

        _logger.LogInformation("[End] Query product data.");

        return result;
    }
    /// <summary>
    /// 订阅处理
    /// </summary>
    /// <param name="orderStockDto"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [HttpPost]
    [Topic(DaprPubSubName, "neworder")]
    public Models.Product SubProductStock(OrderStockDto orderStockDto)
    {
        _logger.LogInformation($"[Begin] Sub Product Stock, Stock Need : {orderStockDto.Count}.");

        var product = _productService.GetProductById(orderStockDto.ProductId);
        if (orderStockDto.Count < 0 || orderStockDto.Count > product.Stock)
        {
            throw new InvalidOperationException("Invalid Product Count!");
        }
        product.Stock = product.Stock - orderStockDto.Count;
        _productService.SaveProduct(product);

        _logger.LogInformation($"[End] Sub Product Stock Finished, Stock Now : {product.Stock}.");

        return product;
    }
}