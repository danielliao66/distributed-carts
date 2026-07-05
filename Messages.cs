namespace DistributedCart.Messages;

// Commands
public record AddItemToCart(string CustomerId, string ItemId, int Quantity);
public record RemoveItemFromCart(string CustomerId, string ItemId);
public record GetCartContent(string CustomerId);

// Events
public record ItemAdded(string ItemId, int Quantity);
public record ItemRemoved(string ItemId);

// Responses
public record CartState(string CustomerId, Dictionary<string, int> Items);