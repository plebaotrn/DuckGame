
using CustomProgram;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CustomProgram
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D background;
        private Texture2D duckMenuTexture;
        private Texture2D instructionTexture;
        private Texture2D replayButtonTexture;
        private Texture2D startButtonTexture;
        private Rectangle startButtonRect;
        private Rectangle replayButtonRect;

        private Player player;
        private List<Collectible> collectibles;
        private List<Obstacle> obstacles;
        private int score;
        private int level;
        private bool gameOver;
        private bool win;

        private SpriteFont font;
        private Texture2D loveTexture;
        private Texture2D bombTexture;
        private Texture2D starTexture;
        private Texture2D explosionTexture;

        private bool showHitboxes = false;
        private KeyboardState previousKeyboardState;

        private bool isHoveringStart = false;

        private Vector2 explosionPosition;
        private bool showExplosion;
        private int explosionFrame;
        private double explosionFrameTimer;
        private const int explosionFrameCount = 6; //The explosion animation has 6 frames (images) to show.
        private const int explosionFrameWidth = 64;
        private const int explosionFrameHeight = 64;
        private const double explosionFrameDuration = 0.15;

        private float dayTime = 1; // 0 to 1 range (0 = night, 1 = day)
        private float daySpeed = 0.05f; // how fast day/night cycle happens
        private bool isFadingToNight = true;
        private Texture2D nightOverlay;
        private bool isPausedAtTransition = false;    // Pausing at day/night
        private float pauseTimer = 0f;                // Tracks pause time
        private float pauseDuration = 2f;             // Pause for 2 seconds


        private enum GameScreen { Title, Instructions, Game }//  State Pattern
        private GameScreen currentScreen = GameScreen.Title;// Current screen of the game

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 501;
        }

        protected override void Initialize()
        {
            player = new Player();
            collectibles = new List<Collectible>();
            obstacles = new List<Obstacle>();
            score = 0;
            level = 1;
            gameOver = false;
            win = false;
            showExplosion = false;
            explosionFrame = 0;

            int buttonSize = 120;
            startButtonRect = new Rectangle(320, 255, 360, 155);

            replayButtonRect = new Rectangle(
                (graphics.PreferredBackBufferWidth - buttonSize) / 2,
                (graphics.PreferredBackBufferHeight - buttonSize) / 2 + 15,
                buttonSize,
                buttonSize
            );

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Texture2D.FromFile(GraphicsDevice, "Content/Textures/AdobeStock_109776713.png");
            duckMenuTexture = Texture2D.FromFile(GraphicsDevice, "Content/Textures/Duck.png");
            instructionTexture = Texture2D.FromFile(GraphicsDevice, "Content/Textures/instruction.png");
            startButtonTexture = Texture2D.FromFile(GraphicsDevice, "Content/Textures/start_button.png");
            replayButtonTexture = Texture2D.FromFile(GraphicsDevice, "Content/Textures/replay_button.png");


            loveTexture = Texture2D.FromFile(GraphicsDevice, "Content/Textures/love.png");
            bombTexture = Texture2D.FromFile(GraphicsDevice, "Content/Textures/bomb.png");
            starTexture = Texture2D.FromFile(GraphicsDevice, "Content/Textures/star.png");
            explosionTexture = Texture2D.FromFile(GraphicsDevice, "Content/Textures/explosion.png");

            nightOverlay = Texture2D.FromFile(GraphicsDevice, "Content/Textures/night_overlay.png");


            font = Content.Load<SpriteFont>("Arial");

            player.LoadContent(Content);
            player.MoveDown(50);
            SpawnCollectibles();
            SpawnObstacles();
        }

        private void SpawnCollectibles()
        {
            collectibles.Clear();
            List<int> usedX = new List<int>();
            int spacing = 120;

            for (int i = 0; i < 4; i++)
            {
                int x;
                do { x = new System.Random().Next(0, GraphicsDevice.Viewport.Width - 60); }
                while (usedX.Exists(pos => System.Math.Abs(pos - x) < spacing));
                usedX.Add(x);
                collectibles.Add(new Collectible(loveTexture, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, CollectibleType.Normal, x));
            }

            int starX;
            do { starX = new System.Random().Next(0, GraphicsDevice.Viewport.Width - 60); }
            while (usedX.Exists(pos => System.Math.Abs(pos - starX) < spacing));
            collectibles.Add(new Collectible(starTexture, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, CollectibleType.Special, starX));
        }

        private void SpawnObstacles()
        {
            obstacles.Clear();
            List<int> usedX = new List<int>();
            int spacing = 120;

            for (int i = 0; i < 5; i++)
            {
                int x;
                do { x = new System.Random().Next(0, GraphicsDevice.Viewport.Width - 60); }
                while (usedX.Exists(pos => System.Math.Abs(pos - x) < spacing));
                usedX.Add(x);
                obstacles.Add(new Obstacle(bombTexture, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, x));
            } //similar to the Factory pattern
        }

        protected override void Update(GameTime gameTime) //Game loop
        {

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.F1) && previousKeyboardState.IsKeyUp(Keys.F1))
                showHitboxes = !showHitboxes;
            previousKeyboardState = keyboard;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            switch (currentScreen)
            {
                case GameScreen.Title:
                    if (mouse.LeftButton == ButtonState.Pressed && startButtonRect.Contains(mouse.Position))
                        currentScreen = GameScreen.Instructions;
                    isHoveringStart = startButtonRect.Contains(mouse.Position);

                    break;

                case GameScreen.Instructions:
                    if (keyboard.GetPressedKeys().Length > 0)
                        currentScreen = GameScreen.Game;
                    break;

                case GameScreen.Game:
                    if (gameOver || win)
                    {
                        if (mouse.LeftButton == ButtonState.Pressed && replayButtonRect.Contains(mouse.Position))
                            ResetGame();
                    }
                    else
                    {
                        player.Update(gameTime);
                        UpdateCollectibles();
                        UpdateObstacles(gameTime);
                        level = score / 50 + 1;
                        if (score >= 300) win = true; //Observer-like logic
                    }

                    if (showExplosion)
                    {
                        explosionFrameTimer += gameTime.ElapsedGameTime.TotalSeconds;
                        if (explosionFrameTimer >= explosionFrameDuration)
                        {
                            explosionFrame++;
                            explosionFrameTimer = 0;
                            if (explosionFrame >= explosionFrameCount)
                            {
                                showExplosion = false;
                                explosionFrame = 0;
                            }
                        }
                    }
                    break;

            }

            if (currentScreen == GameScreen.Game)
            {
                if (isPausedAtTransition)
                {
                    pauseTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (pauseTimer >= pauseDuration)
                    {
                        pauseTimer = 0f;
                        isPausedAtTransition = false;
                        isFadingToNight = !isFadingToNight; // Switch direction after pause
                    }
                }
                else
                {
                    if (isFadingToNight)
                    {
                        dayTime -= (float)gameTime.ElapsedGameTime.TotalSeconds * daySpeed;
                        if (dayTime <= 0f)
                        {
                            dayTime = 0f;   
                            isPausedAtTransition = true; // Pause before fading to day
                        }
                    }
                    else
                    {
                        dayTime += (float)gameTime.ElapsedGameTime.TotalSeconds * daySpeed;
                        if (dayTime >= 1f)
                        {
                            dayTime = 1f;
                            isPausedAtTransition = true; // Pause before fading to night
                        }
                    }
                }
            }



            base.Update(gameTime);
        }

        private void UpdateCollectibles()
        {
            Rectangle playerRect = new Rectangle(player.X + 30, player.Y + 50, player.Width - 70, player.Height - 70);
            for (int i = collectibles.Count - 1; i >= 0; i--)
            {
                collectibles[i].Update(level);
                Rectangle cRect = new Rectangle(collectibles[i].X + 10, collectibles[i].Y + 10, collectibles[i].Width - 20, collectibles[i].Height - 20);
                if (playerRect.Intersects(cRect))
                {
                    score += collectibles[i].ScoreValue;
                    collectibles.RemoveAt(i);
                    if (collectibles.Count == 0) SpawnCollectibles();
                }
            }
        }

        private void UpdateObstacles(GameTime gameTime)
        {
            Rectangle playerRect = new Rectangle(player.X + 30, player.Y + 50, player.Width - 70, player.Height - 70);
            foreach (var obstacle in obstacles)
            {
                obstacle.Update(level);
                Rectangle oRect = new Rectangle(obstacle.X + 10, obstacle.Y + 10, obstacle.Width - 20, obstacle.Height - 20);
                if (playerRect.Intersects(oRect) && !gameOver)
                {
                    explosionPosition = new Vector2(obstacle.X, obstacle.Y);
                    showExplosion = true;
                    explosionFrame = 0;
                    explosionFrameTimer = 0;
                    gameOver = true;
                }
            }
        }

        private void ResetGame()
        {
            score = 0;
            level = 1;
            gameOver = false;
            win = false;
            showExplosion = false;
            explosionFrame = 0;
            player = new Player();
            player.LoadContent(Content);
            player.MoveDown(40);
            SpawnCollectibles();
            SpawnObstacles();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            switch (currentScreen)
            {
                case GameScreen.Title:
                    spriteBatch.Draw(duckMenuTexture, GraphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.Draw(startButtonTexture, startButtonRect, isHoveringStart ? Color.Gray : Color.White);

                    break;

                case GameScreen.Instructions:
                    spriteBatch.Draw(instructionTexture, GraphicsDevice.Viewport.Bounds, Color.White);
                    break;

                case GameScreen.Game:
                    spriteBatch.Draw(background, GraphicsDevice.Viewport.Bounds, Color.White);

                    // Draw day/night overlay
                    float alpha = 1f - dayTime;
                    spriteBatch.Draw(nightOverlay, GraphicsDevice.Viewport.Bounds, Color.White * alpha);

                    foreach (var collectible in collectibles)
                        collectible.Draw(spriteBatch);
                    foreach (var obstacle in obstacles)
                        obstacle.Draw(spriteBatch);

                    player.Draw(spriteBatch);

                    spriteBatch.DrawString(font, $"Score: {score}", new Vector2(10, 10), Color.White);
                    spriteBatch.DrawString(font, $"Level: {level}", new Vector2(10, 40), Color.White);

                   

                    if (showExplosion)
                    {
                        Rectangle sourceRect = new Rectangle(explosionFrame * explosionFrameWidth, 0, explosionFrameWidth, explosionFrameHeight);
                        Rectangle destRect = new Rectangle((int)explosionPosition.X, (int)explosionPosition.Y, 64, 64);
                        spriteBatch.Draw(explosionTexture, destRect, sourceRect, Color.White);
                    }

                    if (gameOver)
                    {
                        Vector2 textSize = font.MeasureString("Game Over!");
                        Vector2 textPosition = new Vector2(
                            (graphics.PreferredBackBufferWidth - textSize.X) / 2,
                            (graphics.PreferredBackBufferHeight - textSize.Y) / 2 - 70
                        );
                        spriteBatch.DrawString(font, "Game Over!", textPosition, Color.Red);
                        spriteBatch.Draw(replayButtonTexture, replayButtonRect, Color.White);
                    }
                    else if (win)
                    {
                        string winText = "You Win!";
                        Vector2 textSize = font.MeasureString(winText);
                        Vector2 centerPosition = new Vector2(
                            (graphics.PreferredBackBufferWidth - textSize.X) / 2,
                            (graphics.PreferredBackBufferHeight - textSize.Y) / 2 - 70
                        );
                        spriteBatch.DrawString(font, "You Win!", centerPosition, Color.Green);
                        spriteBatch.Draw(replayButtonTexture, replayButtonRect, Color.White);

                    }

                    if (showHitboxes)
                    {
                        Texture2D debugPixel = new Texture2D(GraphicsDevice, 1, 1);
                        debugPixel.SetData(new[] { Color.Red });

                        Rectangle playerBox = new Rectangle(player.X + 30, player.Y + 50, player.Width - 70, player.Height - 70);
                        spriteBatch.Draw(debugPixel, playerBox, Color.Red);

                        foreach (var c in collectibles)
                        {
                            Rectangle cRect = new Rectangle(c.X + 10, c.Y + 10, c.Width - 20, c.Height - 20);
                            spriteBatch.Draw(debugPixel, cRect, Color.Yellow);
                        }

                        foreach (var o in obstacles)
                        {
                            Rectangle oRect = new Rectangle(o.X + 10, o.Y + 10, o.Width - 20, o.Height - 20);
                            spriteBatch.Draw(debugPixel, oRect, Color.Orange);
                        }
                    }

                    
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
