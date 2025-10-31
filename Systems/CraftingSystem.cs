namespace StardewValleyClone.Systems;

public class CraftingSystem
{
    private Dictionary<string, CraftingRecipe> _recipes = new();

    public CraftingSystem()
    {
        LoadRecipes();
    }

    private void LoadRecipes()
    {
        // Basic tools
        AddRecipe(new CraftingRecipe
        {
            Id = "chest",
            Name = "Chest",
            Description = "A place to store items",
            RequiredItems = new()
            {
                { "wood", 50 }
            },
            Result = new Item("chest", "Chest", ItemType.Furniture, false),
            Category = CraftingCategory.Furniture
        });

        AddRecipe(new CraftingRecipe
        {
            Id = "scarecrow",
            Name = "Scarecrow",
            Description = "Prevents crows from eating crops",
            RequiredItems = new()
            {
                { "wood", 50 },
                { "coal", 1 },
                { "fiber", 20 }
            },
            Result = new Item("scarecrow", "Scarecrow", ItemType.Other, false),
            Category = CraftingCategory.Farming
        });

        AddRecipe(new CraftingRecipe
        {
            Id = "fence",
            Name = "Fence",
            Description = "Prevents animals from wandering",
            RequiredItems = new()
            {
                { "wood", 2 }
            },
            Result = new Item("fence", "Fence", ItemType.Other, true, 1),
            Category = CraftingCategory.Farming
        });

        AddRecipe(new CraftingRecipe
        {
            Id = "sprinkler",
            Name = "Sprinkler",
            Description = "Waters 4 adjacent tiles every morning",
            RequiredItems = new()
            {
                { "copper_bar", 1 },
                { "iron_bar", 1 }
            },
            Result = new Item("sprinkler", "Sprinkler", ItemType.Other, false),
            Category = CraftingCategory.Farming
        });

        // Cooking recipes
        AddRecipe(new CraftingRecipe
        {
            Id = "fried_egg",
            Name = "Fried Egg",
            Description = "Restores energy",
            RequiredItems = new()
            {
                { "egg", 1 }
            },
            Result = new Item("fried_egg", "Fried Egg", ItemType.Cooking, true, 1),
            Category = CraftingCategory.Cooking
        });

        AddRecipe(new CraftingRecipe
        {
            Id = "salad",
            Name = "Salad",
            Description = "A healthy salad",
            RequiredItems = new()
            {
                { "lettuce", 1 },
                { "tomato", 1 }
            },
            Result = new Item("salad", "Salad", ItemType.Cooking, true, 1),
            Category = CraftingCategory.Cooking
        });
    }

    private void AddRecipe(CraftingRecipe recipe)
    {
        _recipes[recipe.Id] = recipe;
    }

    public bool CanCraft(string recipeId, Inventory inventory)
    {
        if (!_recipes.ContainsKey(recipeId))
            return false;

        var recipe = _recipes[recipeId];

        foreach (var requirement in recipe.RequiredItems)
        {
            if (inventory.GetItemCount(requirement.Key) < requirement.Value)
                return false;
        }

        return true;
    }

    public bool Craft(string recipeId, Inventory inventory)
    {
        if (!CanCraft(recipeId, inventory))
            return false;

        var recipe = _recipes[recipeId];

        // Remove required items
        foreach (var requirement in recipe.RequiredItems)
        {
            int remaining = requirement.Value;
            for (int i = 0; i < inventory.Size && remaining > 0; i++)
            {
                var item = inventory.GetItem(i);
                if (item != null && item.Id == requirement.Key)
                {
                    int toRemove = Math.Min(remaining, item.Quantity);
                    inventory.RemoveItem(i, toRemove);
                    remaining -= toRemove;
                }
            }
        }

        // Add result
        inventory.AddItem(recipe.Result, recipe.Result.Quantity);

        Console.WriteLine($"Crafted: {recipe.Name}");
        return true;
    }

    public List<CraftingRecipe> GetRecipesByCategory(CraftingCategory category)
    {
        return _recipes.Values.Where(r => r.Category == category).ToList();
    }

    public List<CraftingRecipe> GetAllRecipes()
    {
        return _recipes.Values.ToList();
    }

    public CraftingRecipe? GetRecipe(string id)
    {
        return _recipes.GetValueOrDefault(id);
    }
}

public class CraftingRecipe
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public Dictionary<string, int> RequiredItems { get; set; } = new(); // Item ID -> Quantity
    public Item Result { get; set; } = new("", "", ItemType.Other);
    public CraftingCategory Category { get; set; }
    public bool IsUnlocked { get; set; } = true; // Some recipes need to be unlocked
}

public enum CraftingCategory
{
    Farming,
    Fishing,
    Mining,
    Combat,
    Cooking,
    Furniture,
    Equipment,
    Other
}
