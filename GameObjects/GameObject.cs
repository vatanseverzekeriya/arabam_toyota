using System.Numerics;
using StardewValleyClone.Graphics;

namespace StardewValleyClone.GameObjects;

public abstract class GameObject : IDisposable
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public float Rotation { get; set; } = 0;
    public Vector4 Color { get; set; } = Vector4.One;
    public bool IsActive { get; set; } = true;

    public Sprite? Sprite { get; set; }

    public virtual void Update(double deltaTime) { }
    public virtual void Render(Renderer renderer, Shader shader)
    {
        if (Sprite != null && IsActive)
        {
            renderer.DrawSprite(Sprite, Position, shader, Rotation, Size / new Vector2(Sprite.Width, Sprite.Height), Color);
        }
    }

    public virtual void Dispose() { }
}
