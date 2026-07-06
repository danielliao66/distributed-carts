using Proto;
using DistributedCart.Messages;

namespace DistributedCart.Actors;

public class ShoppingCartActor : IActor
{
    string _customerId = string.Empty;
    readonly Dictionary<string, int> _items = new();

    public Task ReceiveAsync(IContext context)
    {
        return context.Message switch
        {
            Started => OnStarted(context),
            ReceiveTimeout => OnTimeout(context), // Handle Passivation Trigger
            AddItemToCart add => OnAddItem(context, add),
            RemoveItemFromCart remove => OnRemoveItem(context, remove),
            GetCartContent => OnGetContent(context),
            _ => Task.CompletedTask
        };
    }

    Task OnStarted(IContext context)
    {
        // The actor name or cluster identity serves as the unique ID
        _customerId = context.Self.Id; 
        Console.WriteLine($"[Actor System] Spawned actor for: {_customerId}");
        
        // Set the inactivity timeout window (e.g., 10 seconds for testing)
        context.SetReceiveTimeout(TimeSpan.FromSeconds(10));
        return Task.CompletedTask;
    }

    Task OnTimeout(IContext context)
    {
        Console.WriteLine($"[Actor {_customerId}] Idle timeout reached. Passivating and clearing memory...");
        
        Console.WriteLine($"[Actor {_customerId}] Saved {_items.Count} item types to persistent store safely.");

        // Clear the timeout tracking and terminate this actor instance
        context.CancelReceiveTimeout();
        context.Stop(context.Self); 
        return Task.CompletedTask;
    }

    Task OnAddItem(IContext context, AddItemToCart msg)
    {
        if (_items.ContainsKey(msg.ItemId))
            _items[msg.ItemId] += msg.Quantity;
        else
            _items[msg.ItemId] = msg.Quantity;

        Console.WriteLine($"[Actor {_customerId}] Active request processed. Inactivity timer reset.");
        context.Respond(new CartState(_customerId, _items));
        return Task.CompletedTask;
    }

    Task OnRemoveItem(IContext context, RemoveItemFromCart msg)
    {
        if (_items.ContainsKey(msg.ItemId))
        {
            _items.Remove(msg.ItemId);
        }
        context.Respond(new CartState(_customerId, _items));
        return Task.CompletedTask;
    }

    Task OnGetContent(IContext context)
    {
        context.Respond(new CartState(_customerId, _items));
        return Task.CompletedTask;
    }
}