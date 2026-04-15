using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using OrderProcessor.Function.Models;

namespace OrderProcessor.Function;

public class ProcessOrder
{
    private readonly ILogger _logger;

    public ProcessOrder(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ProcessOrder>();
    }

    [Function("ProcessOrder")]
    public async Task Run(
        [ServiceBusTrigger("orders-queue", Connection = "ServiceBusConnection")] string message)
    {
        _logger.LogInformation($"Received message: {message}");

        var order = JsonSerializer.Deserialize<Order>(message);

        var connectionString = Environment.GetEnvironmentVariable("SqlConnection");

        using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand(
            "INSERT INTO Orders (OrderId, CustomerName, Amount) VALUES (@OrderId, @CustomerName, @Amount)",
            conn);

        cmd.Parameters.AddWithValue("@OrderId", order.orderId);
        cmd.Parameters.AddWithValue("@CustomerName", order.customerName);
        cmd.Parameters.AddWithValue("@Amount", order.amount);

        await cmd.ExecuteNonQueryAsync();

        _logger.LogInformation("Order inserted successfully.");
    }
}