using Silk.NET.OpenGL;
using Silk.NET.Input;
using System.Numerics;
using StardewValleyClone.Engine;
using StardewValleyClone.Graphics;
using StardewValleyClone.Systems;

namespace StardewValleyClone.GameObjects;

public class FarmScene : Scene
{
    private Player _player;
    private FarmingSystem _farmingSystem;
    private Camera _camera;

    // Map
    private const int MAP_WIDTH = 50;
    private const int MAP_HEIGHT = 50;
    private const int TILE_SIZE = 64;

    private Texture? _grassTexture;
    private Texture? _dirtTexture;
    private Texture? _wateredDirtTexture;

    public FarmScene(GL gl, ResourceManager resourceManager, InputManager inputManager, TimeManager timeManager)
        : base(gl, resourceManager, inputManager, timeManager)
    {
        _camera = new Camera();
        _farmingSystem = new FarmingSystem(timeManager);
    }

    public override void Load()
    {
        // Load textures
        _grassTexture = _resourceManager.CreatePlaceholderTexture("grass", TILE_SIZE, TILE_SIZE);
        _dirtTexture = _resourceManager.CreatePlaceholderTexture("dirt", TILE_SIZE, TILE_SIZE);
        _wateredDirtTexture = _resourceManager.CreatePlaceholderTexture("watered_dirt", TILE_SIZE, TILE_SIZE);

        // Create player at center of farm
        _player = new Player(_inputManager, new Vector2(MAP_WIDTH * TILE_SIZE / 2, MAP_HEIGHT * TILE_SIZE / 2));
        _player.LoadAnimations(_resourceManager);

        // Start with basic tools
        _player.Inventory.AddItem(new Item("hoe", "Hoe", ItemType.Tool, false));
        _player.Inventory.AddItem(new Item("watering_can", "Watering Can", ItemType.Tool, false));
        _player.Inventory.AddItem(new Item("parsnip_seeds", "Parsnip Seeds", ItemType.Seed, true, 10));

        Console.WriteLine("Farm scene loaded! Controls:");
        Console.WriteLine("- WASD or Arrow Keys: Move");
        Console.WriteLine("- C or Left Click: Use tool");
        Console.WriteLine("- 1-9: Select inventory slot");
    }

    public override void Update(double deltaTime)
    {
        _player.Update(deltaTime);

        // Camera follows player
        _camera.Position = _player.Position - new Vector2(640, 360); // Center on player

        // Handle inventory selection (1-9 keys)
        for (int i = 0; i < 9; i++)
        {
            if (_inputManager.IsKeyPressed((Key)(Key.Number1 + i)))
            {
                _player.Inventory.SetSelectedSlot(i);
                Console.WriteLine($"Selected slot {i + 1}");
            }
        }

        // Handle tile interactions
        if (_inputManager.IsKeyPressed(Key.C) || _inputManager.IsMouseButtonPressed(MouseButton.Left))
        {
            HandleTileInteraction();
        }
    }

    private void HandleTileInteraction()
    {
        // Get tile in front of player based on facing direction
        Vector2 targetTile = GetTargetTile();
        var currentItem = _player.Inventory.GetCurrentItem();

        if (currentItem == null) return;

        switch (currentItem.Id)
        {
            case "hoe":
                _farmingSystem.TillSoil(targetTile);
                _player.UseEnergy(2);
                Console.WriteLine($"Tilled soil at {targetTile}");
                break;

            case "watering_can":
                if (_farmingSystem.WaterTile(targetTile))
                {
                    _player.UseEnergy(2);
                    Console.WriteLine($"Watered tile at {targetTile}");
                }
                break;

            case string id when id.EndsWith("_seeds"):
                if (_farmingSystem.PlantSeed(targetTile, CropType.Parsnip))
                {
                    _player.Inventory.RemoveItem(_player.Inventory.SelectedSlot, 1);
                    _player.UseEnergy(2);
                    Console.WriteLine($"Planted seed at {targetTile}");
                }
                break;
        }

        // Try to harvest crop
        var harvested = _farmingSystem.HarvestCrop(targetTile);
        if (harvested != null)
        {
            _player.Inventory.AddItem(harvested);
            Console.WriteLine($"Harvested {harvested.Name}!");
        }
    }

    private Vector2 GetTargetTile()
    {
        var offset = _player.Facing switch
        {
            Direction.Up => new Vector2(0, -TILE_SIZE),
            Direction.Down => new Vector2(0, TILE_SIZE),
            Direction.Left => new Vector2(-TILE_SIZE, 0),
            Direction.Right => new Vector2(TILE_SIZE, 0),
            _ => Vector2.Zero
        };

        var targetPos = _player.Position + offset;
        return new Vector2(
            (float)Math.Floor(targetPos.X / TILE_SIZE),
            (float)Math.Floor(targetPos.Y / TILE_SIZE)
        );
    }

    public override void Render(Renderer renderer)
    {
        var shader = _resourceManager.GetShader("default");
        if (shader == null) return;

        // Calculate visible tiles
        int startX = Math.Max(0, (int)(_camera.Position.X / TILE_SIZE) - 1);
        int startY = Math.Max(0, (int)(_camera.Position.Y / TILE_SIZE) - 1);
        int endX = Math.Min(MAP_WIDTH, startX + 25);
        int endY = Math.Min(MAP_HEIGHT, startY + 15);

        // Draw ground tiles
        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                var worldPos = new Vector2(x * TILE_SIZE, y * TILE_SIZE);
                var screenPos = worldPos - _camera.Position;

                var tile = _farmingSystem.GetTile(new Vector2(x, y));

                Texture? texture = _grassTexture;
                if (tile != null)
                {
                    texture = tile.State switch
                    {
                        TileState.Tilled => _dirtTexture,
                        TileState.Planted => tile.IsWatered ? _wateredDirtTexture : _dirtTexture,
                        _ => _grassTexture
                    };
                }

                if (texture != null)
                {
                    renderer.DrawTexture(texture, screenPos, new Vector2(TILE_SIZE, TILE_SIZE), shader);
                }

                // Draw crop
                if (tile?.Crop != null)
                {
                    var growthStage = Math.Min(3, (int)((float)tile.Crop.DaysGrown / tile.Crop.DaysToHarvest * 4));
                    var cropColor = tile.Crop.IsReadyToHarvest ? new Vector4(1, 1, 0, 1) : new Vector4(0, 1, 0, 1);

                    // Draw a simple representation of crop growth
                    var cropSize = 16 + growthStage * 8;
                    var cropOffset = (TILE_SIZE - cropSize) / 2;

                    if (_grassTexture != null)
                    {
                        renderer.DrawTexture(_grassTexture,
                            screenPos + new Vector2(cropOffset, cropOffset),
                            new Vector2(cropSize, cropSize),
                            shader,
                            0,
                            cropColor);
                    }
                }
            }
        }

        // Draw player
        _player.Position -= _camera.Position;
        _player.Render(renderer, shader);
        _player.Position += _camera.Position;

        // Draw UI (HUD)
        DrawHUD(renderer, shader);
    }

    private void DrawHUD(Renderer renderer, Shader shader)
    {
        // This would draw UI elements like:
        // - Time/Date
        // - Energy bar
        // - Money
        // - Inventory hotbar
        // For now, we'll just print to console occasionally
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}

public class Camera
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Zoom { get; set; } = 1.0f;
}
