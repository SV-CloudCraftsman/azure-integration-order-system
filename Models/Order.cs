namespace OrderProcessor.Function.Models;

public class Order
{
    public string orderId { get; set; }
    public string customerName { get; set; }
    public decimal amount { get; set; }
}