using Silk.NET.OpenGL;
using StardewValleyClone.Graphics;
using StardewValleyClone.GameObjects;

namespace StardewValleyClone.Engine;

public class SceneManager : IDisposable
{
    private GL _gl;
    private ResourceManager _resourceManager;
    private InputManager _inputManager;
    private TimeManager _timeManager;

    private Scene? _currentScene;
    private Dictionary<string, Func<Scene>> _sceneFactories = new();

    public Scene? CurrentScene => _currentScene;

    public SceneManager(GL gl, ResourceManager resourceManager, InputManager inputManager, TimeManager timeManager)
    {
        _gl = gl;
        _resourceManager = resourceManager;
        _inputManager = inputManager;
        _timeManager = timeManager;

        RegisterScenes();
    }

    private void RegisterScenes()
    {
        // Register all available scenes
        _sceneFactories["Farm"] = () => new FarmScene(_gl, _resourceManager, _inputManager, _timeManager);
        // Add more scenes as needed: Town, Mine, Beach, etc.
    }

    public void LoadScene(string sceneName)
    {
        if (!_sceneFactories.ContainsKey(sceneName))
        {
            Console.WriteLine($"Scene not found: {sceneName}");
            return;
        }

        _currentScene?.Dispose();
        _currentScene = _sceneFactories[sceneName]();
        _currentScene.Load();

        Console.WriteLine($"Scene loaded: {sceneName}");
    }

    public void Update(double deltaTime)
    {
        _currentScene?.Update(deltaTime);
    }

    public void Render(Renderer renderer)
    {
        _currentScene?.Render(renderer);
    }

    public void Dispose()
    {
        _currentScene?.Dispose();
    }
}

public abstract class Scene : IDisposable
{
    protected GL _gl;
    protected ResourceManager _resourceManager;
    protected InputManager _inputManager;
    protected TimeManager _timeManager;

    protected List<GameObject> _gameObjects = new();

    public Scene(GL gl, ResourceManager resourceManager, InputManager inputManager, TimeManager timeManager)
    {
        _gl = gl;
        _resourceManager = resourceManager;
        _inputManager = inputManager;
        _timeManager = timeManager;
    }

    public abstract void Load();
    public abstract void Update(double deltaTime);
    public abstract void Render(Renderer renderer);

    public virtual void Dispose()
    {
        foreach (var obj in _gameObjects)
            obj.Dispose();
        _gameObjects.Clear();
    }
}
