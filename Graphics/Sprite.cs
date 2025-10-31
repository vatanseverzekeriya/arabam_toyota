using System.Numerics;

namespace StardewValleyClone.Graphics;

public class Sprite
{
    public Texture Texture { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Vector2 Origin { get; set; } = Vector2.Zero;

    public Sprite(Texture texture)
    {
        Texture = texture;
        Width = texture.Width;
        Height = texture.Height;
    }

    public Sprite(Texture texture, int width, int height)
    {
        Texture = texture;
        Width = width;
        Height = height;
    }
}

public class AnimatedSprite
{
    public Texture SpriteSheet { get; set; }
    public int FrameWidth { get; set; }
    public int FrameHeight { get; set; }
    public int FrameCount { get; set; }
    public float FrameDuration { get; set; } = 0.1f;

    private int _currentFrame = 0;
    private float _frameTimer = 0;

    public int CurrentFrame => _currentFrame;

    public AnimatedSprite(Texture spriteSheet, int frameWidth, int frameHeight, int frameCount)
    {
        SpriteSheet = spriteSheet;
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        FrameCount = frameCount;
    }

    public void Update(double deltaTime)
    {
        _frameTimer += (float)deltaTime;

        if (_frameTimer >= FrameDuration)
        {
            _frameTimer -= FrameDuration;
            _currentFrame = (_currentFrame + 1) % FrameCount;
        }
    }

    public void Reset()
    {
        _currentFrame = 0;
        _frameTimer = 0;
    }

    public Rectangle GetFrameRectangle()
    {
        int framesPerRow = SpriteSheet.Width / FrameWidth;
        int row = _currentFrame / framesPerRow;
        int col = _currentFrame % framesPerRow;

        return new Rectangle(
            col * FrameWidth,
            row * FrameHeight,
            FrameWidth,
            FrameHeight
        );
    }
}

public struct Rectangle
{
    public int X;
    public int Y;
    public int Width;
    public int Height;

    public Rectangle(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}
