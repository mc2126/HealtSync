namespace HealthSync.Models
{
    public class CartItem
    {
        public string Id { get; set; } = "healthsync-watch-001";
        public string Name { get; set; } = "HealthSync SmartBand Pro";
        public decimal Price { get; set; } = 89.99m;
        public string Image { get; set; } = "";
        public int Quantity { get; set; } = 1;

        public decimal Subtotal => Price * Quantity;
    }

    public static class Cart
    {
        private const string SessionKey = "HealthSyncCart";

        public static List<CartItem> GetItems(HttpContext context)
        {
            var json = context.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json))
                return new List<CartItem>();
            return System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        public static void SaveItems(HttpContext context, List<CartItem> items)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(items);
            context.Session.SetString(SessionKey, json);
        }

        public static void AddItem(HttpContext context, CartItem item)
        {
            var items = GetItems(context);
            var existing = items.FirstOrDefault(x => x.Id == item.Id);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                items.Add(item);
            }
            SaveItems(context, items);
        }

        public static void RemoveItem(HttpContext context, string itemId)
        {
            var items = GetItems(context);
            items.RemoveAll(x => x.Id == itemId);
            SaveItems(context, items);
        }

        public static void UpdateQuantity(HttpContext context, string itemId, int quantity)
        {
            var items = GetItems(context);
            var item = items.FirstOrDefault(x => x.Id == itemId);
            if (item != null)
            {
                item.Quantity = quantity > 0 ? quantity : 1;
                SaveItems(context, items);
            }
        }

        public static void Clear(HttpContext context)
        {
            context.Session.Remove(SessionKey);
        }

        public static int GetTotalItems(HttpContext context)
        {
            return GetItems(context).Sum(x => x.Quantity);
        }

        public static decimal GetSubtotal(HttpContext context)
        {
            return GetItems(context).Sum(x => x.Subtotal);
        }
    }
}