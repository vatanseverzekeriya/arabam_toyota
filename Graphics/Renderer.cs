using Silk.NET.OpenGL;
using System.Numerics;

namespace StardewValleyClone.Graphics;

public class Renderer : IDisposable
{
    private GL _gl;
    private uint _vao;
    private uint _vbo;
    private uint _ebo;

    private Matrix4x4 _projection;
    private int _width;
    private int _height;

    public Matrix4x4 Projection => _projection;
    public int Width => _width;
    public int Height => _height;

    public Renderer(GL gl, int width, int height)
    {
        _gl = gl;
        _width = width;
        _height = height;

        InitializeQuad();
        UpdateProjection(width, height);
    }

    private void InitializeQuad()
    {
        // Vertex data for a quad (position + texcoord)
        float[] vertices = new float[]
        {
            // Position     // TexCoord
            0.0f, 1.0f,     0.0f, 1.0f,  // Top-left
            1.0f, 0.0f,     1.0f, 0.0f,  // Bottom-right
            0.0f, 0.0f,     0.0f, 0.0f,  // Bottom-left

            0.0f, 1.0f,     0.0f, 1.0f,  // Top-left
            1.0f, 1.0f,     1.0f, 1.0f,  // Top-right
            1.0f, 0.0f,     1.0f, 0.0f   // Bottom-right
        };

        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();

        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        unsafe
        {
            fixed (float* v = vertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)),
                    v, BufferUsageARB.StaticDraw);
            }
        }

        // Position attribute
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)0);

        // TexCoord attribute
        _gl.EnableVertexAttribArray(1);
        _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)(2 * sizeof(float)));

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _gl.BindVertexArray(0);
    }

    private void UpdateProjection(int width, int height)
    {
        _projection = Matrix4x4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);
    }

    public void Resize(int width, int height)
    {
        _width = width;
        _height = height;
        UpdateProjection(width, height);
    }

    public void DrawSprite(Sprite sprite, Vector2 position, Shader shader, float rotation = 0, Vector2? scale = null, Vector4? color = null)
    {
        shader.Use();

        // Set uniforms
        shader.SetMatrix4x4("uProjection", _projection);

        var scaleVec = scale ?? Vector2.One;
        var colorVec = color ?? Vector4.One;

        var model = Matrix4x4.Identity;
        model *= Matrix4x4.CreateTranslation(new Vector3(0, 0, 0));
        model *= Matrix4x4.CreateScale(new Vector3(sprite.Width * scaleVec.X, sprite.Height * scaleVec.Y, 1));
        model *= Matrix4x4.CreateRotationZ(rotation);
        model *= Matrix4x4.CreateTranslation(new Vector3(position.X, position.Y, 0));

        shader.SetMatrix4x4("uModel", model);
        shader.SetVector4("uColor", colorVec);
        shader.SetInt("uTexture", 0);

        sprite.Texture.Bind(TextureUnit.Texture0);

        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
        _gl.BindVertexArray(0);
    }

    public void DrawTexture(Texture texture, Vector2 position, Vector2 size, Shader shader, float rotation = 0, Vector4? color = null)
    {
        shader.Use();

        shader.SetMatrix4x4("uProjection", _projection);

        var colorVec = color ?? Vector4.One;

        var model = Matrix4x4.Identity;
        model *= Matrix4x4.CreateTranslation(new Vector3(0, 0, 0));
        model *= Matrix4x4.CreateScale(new Vector3(size.X, size.Y, 1));
        model *= Matrix4x4.CreateRotationZ(rotation);
        model *= Matrix4x4.CreateTranslation(new Vector3(position.X, position.Y, 0));

        shader.SetMatrix4x4("uModel", model);
        shader.SetVector4("uColor", colorVec);
        shader.SetInt("uTexture", 0);

        texture.Bind(TextureUnit.Texture0);

        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
        _gl.BindVertexArray(0);
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
    }
}
