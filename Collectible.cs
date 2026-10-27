using Microsoft.Xna.Framework.Graphics;

public enum CollectibleType { Normal, Special }

public class Collectible
{
    public Texture2D Texture { get; private set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public CollectibleType Type { get; private set; }
    public int ScoreValue => Type == CollectibleType.Special ? 30 : 10;

    public Collectible(Texture2D texture, int screenWidth, int screenHeight, CollectibleType type = CollectibleType.Normal, int? fixedX = null)
    {
        Texture = texture;
        Width = 50;
        Height = 50;
        X = fixedX ?? new System.Random().Next(screenWidth - Width);
        Y = new System.Random().Next(-100, -20);
        Type = type;
    }

    public void Update(int level)
    {
        Y += level;
        if (Y > 600)
        {
            Y = new System.Random().Next(-100, -20);
            X = new System.Random().Next(0, 1000 - Width);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, new Microsoft.Xna.Framework.Rectangle(X, Y, Width, Height), Microsoft.Xna.Framework.Color.White);
    }
}
