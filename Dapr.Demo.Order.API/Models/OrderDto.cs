using System;

namespace Dapr.Demo.Order.API.Models;

public class OrderDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Count { get; set; }
}
