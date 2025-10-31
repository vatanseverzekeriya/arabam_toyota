using System.Numerics;
using StardewValleyClone.Engine;
using StardewValleyClone.Graphics;
using StardewValleyClone.Systems;

namespace StardewValleyClone.GameObjects;

public class Player : GameObject
{
    private InputManager _inputManager;
    private float _moveSpeed = 200f; // pixels per second

    // Animation states
    private Dictionary<string, AnimatedSprite> _animations = new();
    private string _currentAnimation = "idle_down";
    private Direction _facing = Direction.Down;

    // Inventory
    private Inventory _inventory;

    // Stamina/Energy (like Stardew Valley)
    private float _maxEnergy = 270f;
    private float _currentEnergy = 270f;

    // Money
    private int _gold = 500;

    public Inventory Inventory => _inventory;
    public float Energy => _currentEnergy;
    public float MaxEnergy => _maxEnergy;
    public int Gold => _gold;
    public Direction Facing => _facing;

    public Player(InputManager inputManager, Vector2 startPosition)
    {
        _inputManager = inputManager;
        Position = startPosition;
        Size = new Vector2(16, 32); // Stardew Valley character size
        _inventory = new Inventory(36); // 36 slots like Stardew Valley
    }

    public void LoadAnimations(ResourceManager resourceManager)
    {
        // In a real implementation, load actual sprite sheets here
        // For now, we'll use placeholder textures
        var placeholderTexture = resourceManager.CreatePlaceholderTexture("player_placeholder", 16, 32);

        _animations["idle_down"] = new AnimatedSprite(placeholderTexture, 16, 32, 1);
        _animations["idle_up"] = new AnimatedSprite(placeholderTexture, 16, 32, 1);
        _animations["idle_left"] = new AnimatedSprite(placeholderTexture, 16, 32, 1);
        _animations["idle_right"] = new AnimatedSprite(placeholderTexture, 16, 32, 1);

        _animations["walk_down"] = new AnimatedSprite(placeholderTexture, 16, 32, 4);
        _animations["walk_up"] = new AnimatedSprite(placeholderTexture, 16, 32, 4);
        _animations["walk_left"] = new AnimatedSprite(placeholderTexture, 16, 32, 4);
        _animations["walk_right"] = new AnimatedSprite(placeholderTexture, 16, 32, 4);
    }

    public override void Update(double deltaTime)
    {
        var movement = _inputManager.GetMovementInput();

        if (movement != Vector2.Zero)
        {
            // Move player
            Position += movement * _moveSpeed * (float)deltaTime;

            // Update facing direction and animation
            if (Math.Abs(movement.Y) > Math.Abs(movement.X))
            {
                if (movement.Y < 0)
                {
                    _facing = Direction.Up;
                    _currentAnimation = "walk_up";
                }
                else
                {
                    _facing = Direction.Down;
                    _currentAnimation = "walk_down";
                }
            }
            else
            {
                if (movement.X < 0)
                {
                    _facing = Direction.Left;
                    _currentAnimation = "walk_left";
                }
                else
                {
                    _facing = Direction.Right;
                    _currentAnimation = "walk_right";
                }
            }
        }
        else
        {
            // Idle animation
            _currentAnimation = _facing switch
            {
                Direction.Up => "idle_up",
                Direction.Down => "idle_down",
                Direction.Left => "idle_left",
                Direction.Right => "idle_right",
                _ => "idle_down"
            };
        }

        // Update current animation
        if (_animations.ContainsKey(_currentAnimation))
        {
            _animations[_currentAnimation].Update(deltaTime);
        }

        // Tool usage (left mouse button or C key)
        if (_inputManager.IsMouseButtonPressed(Silk.NET.Input.MouseButton.Left) ||
            _inputManager.IsKeyPressed(Silk.NET.Input.Key.C))
        {
            UseTool();
        }
    }

    private void UseTool()
    {
        // Get the current tool from inventory and use it
        var currentItem = _inventory.GetCurrentItem();
        if (currentItem != null && currentItem.Type == ItemType.Tool)
        {
            // Use tool based on facing direction
            // This would interact with the tile system
            Console.WriteLine($"Using tool: {currentItem.Name}");
        }
    }

    public override void Render(Renderer renderer, Shader shader)
    {
        if (_animations.ContainsKey(_currentAnimation))
        {
            var anim = _animations[_currentAnimation];
            var sprite = new Sprite(anim.SpriteSheet, anim.FrameWidth, anim.FrameHeight);

            renderer.DrawSprite(sprite, Position, shader, 0, new Vector2(2, 2)); // Scale up 2x
        }
    }

    public void AddGold(int amount)
    {
        _gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (_gold >= amount)
        {
            _gold -= amount;
            return true;
        }
        return false;
    }

    public void UseEnergy(float amount)
    {
        _currentEnergy = Math.Max(0, _currentEnergy - amount);
    }

    public void RestoreEnergy(float amount)
    {
        _currentEnergy = Math.Min(_maxEnergy, _currentEnergy + amount);
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
