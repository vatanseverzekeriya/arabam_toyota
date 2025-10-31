namespace StardewValleyClone.Systems;

public class Inventory
{
    private Item?[] _items;
    private int _selectedSlot = 0;

    public int Size => _items.Length;
    public int SelectedSlot => _selectedSlot;

    public Inventory(int size)
    {
        _items = new Item?[size];
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        // First, try to stack with existing items
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] != null && _items[i]!.Id == item.Id && _items[i]!.IsStackable)
            {
                _items[i]!.Quantity += quantity;
                return true;
            }
        }

        // If not stackable or no existing stack, find empty slot
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
            {
                _items[i] = new Item(item.Id, item.Name, item.Type, item.IsStackable, quantity);
                return true;
            }
        }

        return false; // Inventory full
    }

    public bool RemoveItem(int slot, int quantity = 1)
    {
        if (slot < 0 || slot >= _items.Length || _items[slot] == null)
            return false;

        _items[slot]!.Quantity -= quantity;

        if (_items[slot]!.Quantity <= 0)
            _items[slot] = null;

        return true;
    }

    public Item? GetItem(int slot)
    {
        if (slot < 0 || slot >= _items.Length)
            return null;

        return _items[slot];
    }

    public Item? GetCurrentItem()
    {
        return GetItem(_selectedSlot);
    }

    public void SetSelectedSlot(int slot)
    {
        if (slot >= 0 && slot < _items.Length)
            _selectedSlot = slot;
    }

    public bool HasItem(string itemId, int quantity = 1)
    {
        int totalQuantity = 0;
        foreach (var item in _items)
        {
            if (item != null && item.Id == itemId)
                totalQuantity += item.Quantity;
        }
        return totalQuantity >= quantity;
    }

    public int GetItemCount(string itemId)
    {
        int totalQuantity = 0;
        foreach (var item in _items)
        {
            if (item != null && item.Id == itemId)
                totalQuantity += item.Quantity;
        }
        return totalQuantity;
    }
}

public class Item
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ItemType Type { get; set; }
    public bool IsStackable { get; set; }
    public int Quantity { get; set; }
    public int SellPrice { get; set; }
    public int BuyPrice { get; set; }
    public string Description { get; set; } = "";

    public Item(string id, string name, ItemType type, bool isStackable = true, int quantity = 1)
    {
        Id = id;
        Name = name;
        Type = type;
        IsStackable = isStackable;
        Quantity = quantity;
    }
}

public enum ItemType
{
    Tool,
    Seed,
    Crop,
    Fish,
    Cooking,
    Material,
    Foraging,
    Mineral,
    Furniture,
    Other
}
