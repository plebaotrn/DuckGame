using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CustomProgram
{
    public class Player
    {
        private Texture2D[] walkingFrames;
        private Texture2D[] runningFrames;
        private int currentFrame;
        private int animationCounter;
        private bool running;
        private bool facingRight = true;

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; } = 100;
        public int Height { get; private set; } = 100;

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        private int AnimationSpeed => running ? 5 : 10;
        private Texture2D[] CurrentFrames => running ? runningFrames : walkingFrames;

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            var graphicsDevice = ((IGraphicsDeviceService)content.ServiceProvider
                .GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

            walkingFrames = new Texture2D[]
            {
                Texture2D.FromFile(graphicsDevice, "Content/Textures/walk1.png"),
                Texture2D.FromFile(graphicsDevice, "Content/Textures/walk2.png")
            };

            runningFrames = new Texture2D[]
            {
                Texture2D.FromFile(graphicsDevice, "Content/Textures/run1.png"),
                Texture2D.FromFile(graphicsDevice, "Content/Textures/run2.png")
            };

            X = 200;
            Y = 300;
        }

        public void Update(GameTime gameTime)
        {

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            running = currentKeyboardState.IsKeyDown(Keys.LeftShift) || currentKeyboardState.IsKeyDown(Keys.RightShift);

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                X -= running ? 10 : 5;
                facingRight = false;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                X += running ? 10 : 5;
                facingRight = true;
            }

            animationCounter++;
            if (animationCounter % AnimationSpeed == 0)
            {
                currentFrame = (currentFrame + 1) % CurrentFrames.Length;
            }

            // Clamp player position to stay within window boundaries (1000x501)
            X = (int)MathHelper.Clamp(X, 0, 1020 - Width);
            Y = (int)MathHelper.Clamp(Y, 0, 520 - Height);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D frame = CurrentFrames[currentFrame];
            SpriteEffects flip = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(frame, new Rectangle(X, Y, Width, Height), null, Color.White, 0f, Vector2.Zero, flip, 0f);
        }

        public bool CollidesWith(Collectible collectible)
        {
            Rectangle playerRect = new Rectangle(X, Y, Width, Height);
            Rectangle collectibleRect = new Rectangle(collectible.X, collectible.Y, collectible.Width, collectible.Height);
            return playerRect.Intersects(collectibleRect);
        }

        public bool CollidesWith(Obstacle obstacle)
        {
            Rectangle playerRect = new Rectangle(X, Y, Width, Height);
            Rectangle obstacleRect = new Rectangle(obstacle.X, obstacle.Y, obstacle.Width, obstacle.Height);
            return playerRect.Intersects(obstacleRect);
        }

        // Optional: allow external Y movement (like MoveDown from previous request)
        public void MoveDown(int amount)
        {
            Y += amount;
        }

        public void MoveUp(int amount)
        {
            Y -= amount;
        }
    }
}
