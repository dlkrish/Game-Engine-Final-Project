using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using CPI311.GameEngine;

namespace Assignment1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        AnimatedSprite player;
        Sprite bonus;
        ProgressBar timeBar;
        ProgressBar distanceBar;
        Random random = new Random();
        SpriteFont font;
        KeyboardState preState;

        float timeLeft = 90f;
        float distanceWalked = 0;
        Vector2 playerDestination = Vector2.Zero;
        Vector2 playerDirection = Vector2.UnitX;
        bool isFacing = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        AnimatedSprite anim;
        KeyboardState currentState;
        KeyboardState prevState;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            prevState = Keyboard.GetState();     
            Time.Initialize();
            InputManager.Initialize();
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            player = new AnimatedSprite(Content.Load<Texture2D>("explorer"), 8, 5); //8 frames, 5 clips
            player.Source = new Rectangle((int)(player.Width * player.Frame), (int)(player.Height * player.Clip), player.Width, player.Height);
            bonus = new Sprite(Content.Load<Texture2D>("Square"));
            timeBar = new ProgressBar(Content.Load<Texture2D>("Square"), 1, 2);
            distanceBar = new ProgressBar(Content.Load<Texture2D>("Square"), 1, 2);
            font = Content.Load<SpriteFont>("Font");
            timeBar.Position = new Vector2(50, 50);
            distanceBar.Position = new Vector2(50, 80);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (InputManager.IsKeyDown(Keys.Up))
            {
                player.Clip = 0;
                playerDirection = -Vector2.UnitY;
                player.Position += Vector2.UnitY * -5;
                player.Update();
            }

            else if (InputManager.IsKeyDown(Keys.Down))
            {
                player.Clip = 1;
                playerDirection = Vector2.UnitY;
                player.Position += Vector2.UnitY * 5;
                player.Update();
            }

            else if (InputManager.IsKeyDown(Keys.Left))
            {
                player.Clip = 2;
                playerDirection = Vector2.UnitX;
                player.Position += Vector2.UnitX * -5;
                player.Update();
            }

            else if (InputManager.IsKeyDown(Keys.Right))
            {
                player.Clip = 3;
                playerDirection = Vector2.UnitX;
                player.Position += Vector2.UnitX * 5;
                player.Update();
            }



            timeLeft = timeLeft - Time.ElapsedGameTime;
            timeBar.Value = timeLeft / 90f;
            timeBar.Update();
            distanceWalked++;
            distanceBar.Value = distanceWalked / 1000f;
            distanceBar.Update();

            Time.Update(gameTime);
            InputManager.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            player.Draw(spriteBatch);
            distanceBar.Draw(spriteBatch);
            timeBar.Draw(spriteBatch);
            //bonus.Draw(spriteBatch);
            //spriteBatch.DrawString(font, "TIME: " + timeBar.Value);
            //spriteBatch.DrawString(font, "DIS: " + distanceBar.Value);
            //spriteBatch.DrawString(font, "*", player.Position);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
