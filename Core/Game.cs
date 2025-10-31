using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using Silk.NET.Maths;
using StardewValleyClone.Engine;
using StardewValleyClone.Graphics;
using StardewValleyClone.Systems;

namespace StardewValleyClone.Core;

public class Game : IDisposable
{
    private IWindow _window;
    private GL _gl;
    private IInputContext _input;

    // Core Systems
    private TimeManager _timeManager;
    private InputManager _inputManager;
    private Renderer _renderer;
    private SceneManager _sceneManager;
    private ResourceManager _resourceManager;

    // Game Settings
    private const int WINDOW_WIDTH = 1280;
    private const int WINDOW_HEIGHT = 720;
    private const string WINDOW_TITLE = "Stardew Valley Clone";

    private double _deltaTime;
    private DateTime _lastFrameTime;

    public Game()
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(WINDOW_WIDTH, WINDOW_HEIGHT);
        options.Title = WINDOW_TITLE;
        options.VSync = true;

        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Closing += OnClose;
        _window.Resize += OnResize;

        _lastFrameTime = DateTime.Now;
    }

    public void Run()
    {
        _window.Run();
    }

    private void OnLoad()
    {
        _gl = GL.GetApi(_window);
        _input = _window.CreateInput();

        // Initialize core systems
        _inputManager = new InputManager(_input);
        _resourceManager = new ResourceManager(_gl);
        _renderer = new Renderer(_gl, WINDOW_WIDTH, WINDOW_HEIGHT);
        _timeManager = new TimeManager();
        _sceneManager = new SceneManager(_gl, _resourceManager, _inputManager, _timeManager);

        // Set OpenGL settings
        _gl.ClearColor(0.53f, 0.81f, 0.92f, 1.0f); // Sky blue
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // Load initial scene (Farm)
        _sceneManager.LoadScene("Farm");

        Console.WriteLine("Game initialized successfully!");
    }

    private void OnUpdate(double deltaTime)
    {
        _deltaTime = deltaTime;

        // Update input
        _inputManager.Update();

        // Update time system
        _timeManager.Update(deltaTime);

        // Update current scene
        _sceneManager.Update(deltaTime);

        // Calculate FPS
        var currentTime = DateTime.Now;
        var frameTime = (currentTime - _lastFrameTime).TotalMilliseconds;
        _lastFrameTime = currentTime;
    }

    private void OnRender(double deltaTime)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Render current scene
        _sceneManager.Render(_renderer);
    }

    private void OnResize(Vector2D<int> newSize)
    {
        _gl.Viewport(newSize);
        _renderer?.Resize(newSize.X, newSize.Y);
    }

    private void OnClose()
    {
        Dispose();
    }

    public void Dispose()
    {
        _sceneManager?.Dispose();
        _resourceManager?.Dispose();
        _renderer?.Dispose();
        _input?.Dispose();
        _gl?.Dispose();
    }
}
