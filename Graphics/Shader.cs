using Silk.NET.OpenGL;
using System.Numerics;

namespace StardewValleyClone.Graphics;

public class Shader : IDisposable
{
    private GL _gl;
    private uint _handle;
    private Dictionary<string, int> _uniformLocations = new();

    public Shader(GL gl, string vertexSource, string fragmentSource)
    {
        _gl = gl;

        uint vertexShader = CompileShader(ShaderType.VertexShader, vertexSource);
        uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource);

        _handle = _gl.CreateProgram();
        _gl.AttachShader(_handle, vertexShader);
        _gl.AttachShader(_handle, fragmentShader);
        _gl.LinkProgram(_handle);

        _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            string info = _gl.GetProgramInfoLog(_handle);
            throw new Exception($"Error linking shader: {info}");
        }

        _gl.DetachShader(_handle, vertexShader);
        _gl.DetachShader(_handle, fragmentShader);
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);
    }

    private uint CompileShader(ShaderType type, string source)
    {
        uint shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);

        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
        if (status == 0)
        {
            string info = _gl.GetShaderInfoLog(shader);
            throw new Exception($"Error compiling {type} shader: {info}");
        }

        return shader;
    }

    public void Use()
    {
        _gl.UseProgram(_handle);
    }

    public int GetUniformLocation(string name)
    {
        if (_uniformLocations.TryGetValue(name, out int location))
            return location;

        location = _gl.GetUniformLocation(_handle, name);
        _uniformLocations[name] = location;
        return location;
    }

    public void SetInt(string name, int value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform1(location, value);
    }

    public void SetFloat(string name, float value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform1(location, value);
    }

    public void SetVector2(string name, Vector2 value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform2(location, value.X, value.Y);
    }

    public void SetVector3(string name, Vector3 value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform3(location, value.X, value.Y, value.Z);
    }

    public void SetVector4(string name, Vector4 value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
    }

    public unsafe void SetMatrix4x4(string name, Matrix4x4 value)
    {
        int location = GetUniformLocation(name);
        _gl.UniformMatrix4(location, 1, false, (float*)&value);
    }

    public void Dispose()
    {
        _gl.DeleteProgram(_handle);
    }
}
