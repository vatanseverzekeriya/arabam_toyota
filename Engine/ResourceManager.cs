using Silk.NET.OpenGL;
using StardewValleyClone.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace StardewValleyClone.Engine;

public class ResourceManager : IDisposable
{
    private GL _gl;
    private Dictionary<string, Texture> _textures = new();
    private Dictionary<string, Shader> _shaders = new();

    public ResourceManager(GL gl)
    {
        _gl = gl;
        LoadDefaultShaders();
    }

    private void LoadDefaultShaders()
    {
        // Default sprite shader
        string vertexShaderSource = @"
#version 330 core

layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoord;

uniform mat4 uProjection;
uniform mat4 uModel;

void main()
{
    gl_Position = uProjection * uModel * vec4(aPosition, 0.0, 1.0);
    TexCoord = aTexCoord;
}
";

        string fragmentShaderSource = @"
#version 330 core

in vec2 TexCoord;
out vec4 FragColor;

uniform sampler2D uTexture;
uniform vec4 uColor;

void main()
{
    FragColor = texture(uTexture, TexCoord) * uColor;
}
";

        var shader = new Shader(_gl, vertexShaderSource, fragmentShaderSource);
        _shaders["default"] = shader;
    }

    public Texture LoadTexture(string name, string path)
    {
        if (_textures.ContainsKey(name))
            return _textures[name];

        if (!File.Exists(path))
        {
            Console.WriteLine($"Texture not found: {path}");
            return CreatePlaceholderTexture(name);
        }

        var texture = new Texture(_gl, path);
        _textures[name] = texture;
        return texture;
    }

    public Texture CreatePlaceholderTexture(string name, int width = 16, int height = 16)
    {
        if (_textures.ContainsKey(name))
            return _textures[name];

        // Create a pink placeholder texture
        var image = new Image<Rgba32>(width, height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                image[x, y] = new Rgba32(255, 0, 255, 255); // Magenta
            }
        }

        var texture = new Texture(_gl, image);
        _textures[name] = texture;
        return texture;
    }

    public Texture? GetTexture(string name)
    {
        return _textures.GetValueOrDefault(name);
    }

    public Shader? GetShader(string name)
    {
        return _shaders.GetValueOrDefault(name);
    }

    public void Dispose()
    {
        foreach (var texture in _textures.Values)
            texture.Dispose();

        foreach (var shader in _shaders.Values)
            shader.Dispose();

        _textures.Clear();
        _shaders.Clear();
    }
}
