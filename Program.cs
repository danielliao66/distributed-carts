using Microsoft.AspNetCore.Mvc;
using Proto;
using DistributedCart.Actors;
using DistributedCart.Messages;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Initialize Proto.Actor System
var system = new ActorSystem();
var context = system.Root;

// Define the blueprint for creating Cart Actors
var cartProps = Props.FromProducer(() => new ShoppingCartActor());

// A simple dictionary to map CustomerId -> Actor PID
var cartRegistry = new Dictionary<string, PID>();

app.MapPost("/api/cart/{customerId}/items", async (string customerId, [FromBody] AddItemInput input) =>
{
    if (!cartRegistry.TryGetValue(customerId, out var pid))
    {
        // Spawn a new independent actor for this specific customer session
        pid = context.SpawnNamed(cartProps, $"cart-{customerId}");
        cartRegistry[customerId] = pid;
    }

    // Asynchronously request the actor to mutate state
    var result = await context.RequestAsync<CartState>(pid, new AddItemToCart(customerId, input.ItemId, input.Quantity));
    return Results.Ok(result);
});

app.MapGet("/api/cart/{customerId}", async (string customerId) =>
{
    if (!cartRegistry.TryGetValue(customerId, out var pid))
    {
        return Results.NotFound($"No active session for customer {customerId}");
    }

    var result = await context.RequestAsync<CartState>(pid, new GetCartContent(customerId));
    return Results.Ok(result);
});

app.Run();

public record AddItemInput(string ItemId, int Quantity);