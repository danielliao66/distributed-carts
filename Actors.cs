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
        return Task.CompletedTask;
    }

    Task OnAddItem(IContext context, AddItemToCart msg)
    {
        if (_items.ContainsKey(msg.ItemId))
            _items[msg.ItemId] += msg.Quantity;
        else
            _items[msg.ItemId] = msg.Quantity;

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