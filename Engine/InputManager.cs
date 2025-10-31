using Silk.NET.Input;
using System.Numerics;

namespace StardewValleyClone.Engine;

public class InputManager
{
    private IInputContext _input;
    private IKeyboard _keyboard;
    private IMouse _mouse;

    private HashSet<Key> _keysPressed = new();
    private HashSet<Key> _keysDown = new();
    private HashSet<Key> _keysReleased = new();

    private HashSet<MouseButton> _mouseButtonsPressed = new();
    private HashSet<MouseButton> _mouseButtonsDown = new();
    private HashSet<MouseButton> _mouseButtonsReleased = new();

    private Vector2 _mousePosition;
    private Vector2 _mouseDelta;

    public Vector2 MousePosition => _mousePosition;
    public Vector2 MouseDelta => _mouseDelta;

    public InputManager(IInputContext input)
    {
        _input = input;
        _keyboard = input.Keyboards[0];
        _mouse = input.Mice[0];

        _keyboard.KeyDown += OnKeyDown;
        _keyboard.KeyUp += OnKeyUp;
        _mouse.MouseDown += OnMouseDown;
        _mouse.MouseUp += OnMouseUp;
        _mouse.MouseMove += OnMouseMove;
    }

    public void Update()
    {
        _keysPressed.Clear();
        _keysReleased.Clear();
        _mouseButtonsPressed.Clear();
        _mouseButtonsReleased.Clear();
        _mouseDelta = Vector2.Zero;
    }

    private void OnKeyDown(IKeyboard keyboard, Key key, int scancode)
    {
        if (!_keysDown.Contains(key))
        {
            _keysPressed.Add(key);
            _keysDown.Add(key);
        }
    }

    private void OnKeyUp(IKeyboard keyboard, Key key, int scancode)
    {
        _keysReleased.Add(key);
        _keysDown.Remove(key);
    }

    private void OnMouseDown(IMouse mouse, MouseButton button)
    {
        if (!_mouseButtonsDown.Contains(button))
        {
            _mouseButtonsPressed.Add(button);
            _mouseButtonsDown.Add(button);
        }
    }

    private void OnMouseUp(IMouse mouse, MouseButton button)
    {
        _mouseButtonsReleased.Add(button);
        _mouseButtonsDown.Remove(button);
    }

    private void OnMouseMove(IMouse mouse, Vector2 position)
    {
        _mouseDelta = position - _mousePosition;
        _mousePosition = position;
    }

    // Keyboard methods
    public bool IsKeyPressed(Key key) => _keysPressed.Contains(key);
    public bool IsKeyDown(Key key) => _keysDown.Contains(key);
    public bool IsKeyReleased(Key key) => _keysReleased.Contains(key);

    // Mouse methods
    public bool IsMouseButtonPressed(MouseButton button) => _mouseButtonsPressed.Contains(button);
    public bool IsMouseButtonDown(MouseButton button) => _mouseButtonsDown.Contains(button);
    public bool IsMouseButtonReleased(MouseButton button) => _mouseButtonsReleased.Contains(button);

    // Movement helper (WASD or Arrow keys)
    public Vector2 GetMovementInput()
    {
        var movement = Vector2.Zero;

        if (IsKeyDown(Key.W) || IsKeyDown(Key.Up)) movement.Y -= 1;
        if (IsKeyDown(Key.S) || IsKeyDown(Key.Down)) movement.Y += 1;
        if (IsKeyDown(Key.A) || IsKeyDown(Key.Left)) movement.X -= 1;
        if (IsKeyDown(Key.D) || IsKeyDown(Key.Right)) movement.X += 1;

        if (movement != Vector2.Zero)
            movement = Vector2.Normalize(movement);

        return movement;
    }
}
