using System.Reflection;

namespace Dapr.Demo.Product.API.Services;

public interface IProductService
{
    Models.Product GetProductById(int productId);

    bool SaveProduct(Models.Product product);
}

