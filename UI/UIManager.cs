using System.Numerics;
using StardewValleyClone.Graphics;
using StardewValleyClone.Engine;
using StardewValleyClone.GameObjects;
using StardewValleyClone.Systems;

namespace StardewValleyClone.UI;

public class UIManager
{
    private TimeManager _timeManager;
    private Player _player;
    private Renderer _renderer;
    private ResourceManager _resourceManager;

    public UIManager(Renderer renderer, ResourceManager resourceManager, TimeManager timeManager, Player player)
    {
        _renderer = renderer;
        _resourceManager = resourceManager;
        _timeManager = timeManager;
        _player = player;
    }

    public void Render(Shader shader)
    {
        RenderTimeDisplay(shader);
        RenderEnergyBar(shader);
        RenderMoneyDisplay(shader);
        RenderHotbar(shader);
    }

    private void RenderTimeDisplay(Shader shader)
    {
        // Position: Top-right corner
        Vector2 position = new Vector2(_renderer.Width - 200, 20);

        // In a real implementation, render text here
        // For now, we'll just draw a colored box as placeholder
        var texture = _resourceManager.CreatePlaceholderTexture("time_bg", 180, 60);
        _renderer.DrawTexture(texture, position, new Vector2(180, 60), shader, 0, new Vector4(0.2f, 0.2f, 0.2f, 0.8f));

        // TODO: Render actual text showing time and date
        // Example: "10:30 AM"
        //          "Spring 15, Year 1"
    }

    private void RenderEnergyBar(Shader shader)
    {
        // Position: Bottom-right corner
        Vector2 position = new Vector2(_renderer.Width - 220, _renderer.Height - 60);

        // Background
        var bgTexture = _resourceManager.CreatePlaceholderTexture("energy_bg", 200, 30);
        _renderer.DrawTexture(bgTexture, position, new Vector2(200, 30), shader, 0, new Vector4(0.2f, 0.2f, 0.2f, 0.8f));

        // Energy bar (green)
        float energyPercent = _player.Energy / _player.MaxEnergy;
        var energyTexture = _resourceManager.CreatePlaceholderTexture("energy_fill", 1, 1);
        _renderer.DrawTexture(energyTexture, position + new Vector2(5, 5), new Vector2(190 * energyPercent, 20), shader, 0, new Vector4(0.2f, 1.0f, 0.2f, 1.0f));
    }

    private void RenderMoneyDisplay(Shader shader)
    {
        // Position: Top-right, below time
        Vector2 position = new Vector2(_renderer.Width - 200, 90);

        var texture = _resourceManager.CreatePlaceholderTexture("money_bg", 180, 40);
        _renderer.DrawTexture(texture, position, new Vector2(180, 40), shader, 0, new Vector4(0.8f, 0.6f, 0.2f, 0.9f));

        // TODO: Render actual text showing gold amount
        // Example: "500g"
    }

    private void RenderHotbar(Shader shader)
    {
        // Position: Bottom-center
        int slotCount = 12;
        int slotSize = 64;
        int spacing = 4;
        int totalWidth = slotCount * (slotSize + spacing);

        Vector2 startPosition = new Vector2((_renderer.Width - totalWidth) / 2, _renderer.Height - slotSize - 20);

        for (int i = 0; i < slotCount; i++)
        {
            Vector2 slotPosition = startPosition + new Vector2(i * (slotSize + spacing), 0);

            // Slot background
            var isSelected = i == _player.Inventory.SelectedSlot;
            var bgColor = isSelected ? new Vector4(1.0f, 1.0f, 0.5f, 0.9f) : new Vector4(0.2f, 0.2f, 0.2f, 0.8f);

            var bgTexture = _resourceManager.CreatePlaceholderTexture($"slot_bg_{i}", slotSize, slotSize);
            _renderer.DrawTexture(bgTexture, slotPosition, new Vector2(slotSize, slotSize), shader, 0, bgColor);

            // Item in slot
            var item = _player.Inventory.GetItem(i);
            if (item != null)
            {
                // Draw item icon (placeholder for now)
                var itemColor = item.Type switch
                {
                    ItemType.Tool => new Vector4(0.7f, 0.7f, 0.7f, 1.0f),
                    ItemType.Seed => new Vector4(0.6f, 0.4f, 0.2f, 1.0f),
                    ItemType.Crop => new Vector4(0.2f, 0.8f, 0.2f, 1.0f),
                    _ => new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
                };

                var itemTexture = _resourceManager.CreatePlaceholderTexture($"item_{item.Id}", 48, 48);
                _renderer.DrawTexture(itemTexture, slotPosition + new Vector2(8, 8), new Vector2(48, 48), shader, 0, itemColor);

                // TODO: Render quantity text if stackable
            }
        }
    }
}
