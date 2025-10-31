using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace StardewValleyClone.Graphics;

public class Texture : IDisposable
{
    private GL _gl;
    private uint _handle;

    public uint Handle => _handle;
    public int Width { get; private set; }
    public int Height { get; private set; }

    public Texture(GL gl, string path)
    {
        _gl = gl;

        _handle = _gl.GenTexture();
        Bind();

        using (var img = Image.Load<Rgba32>(path))
        {
            Width = img.Width;
            Height = img.Height;

            // Flip the image vertically (OpenGL expects bottom-left origin)
            img.Mutate(x => x.Flip(FlipMode.Vertical));

            var pixels = new byte[4 * img.Width * img.Height];
            img.CopyPixelDataTo(pixels);

            unsafe
            {
                fixed (byte* ptr = pixels)
                {
                    _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                        (uint)img.Width, (uint)img.Height, 0,
                        PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                }
            }
        }

        SetParameters();
    }

    public Texture(GL gl, Image<Rgba32> image)
    {
        _gl = gl;
        Width = image.Width;
        Height = image.Height;

        _handle = _gl.GenTexture();
        Bind();

        image.Mutate(x => x.Flip(FlipMode.Vertical));

        var pixels = new byte[4 * image.Width * image.Height];
        image.CopyPixelDataTo(pixels);

        unsafe
        {
            fixed (byte* ptr = pixels)
            {
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                    (uint)image.Width, (uint)image.Height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
            }
        }

        SetParameters();
    }

    private void SetParameters()
    {
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
    }

    public void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        _gl.ActiveTexture(unit);
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }

    public void Dispose()
    {
        _gl.DeleteTexture(_handle);
    }
}
