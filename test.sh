# Scenario A: Create a cart and add items
curl -X POST http://localhost:5000/api/cart/alice456/items \
-H "Content-Type: application/json" \
-d '{"itemId": "laptop-pro", "quantity": 2}'

# Scenario B: Update the cart asynchronously
curl -X POST http://localhost:5000/api/cart/alice456/items \
-H "Content-Type: application/json" \
-d '{"itemId": "mouse-wireless", "quantity": 1}'

# Scenario C: Fetch the state independently
curl http://localhost:5000/api/cart/alice456

# Scenario D: Test a completely different customer
curl -X POST http://localhost:5000/api/cart/bob789/items \
-H "Content-Type: application/json" \
-d '{"itemId": "keyboard-mechanical", "quantity": 1}'