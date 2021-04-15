using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameEngine;
using CPI311.GameEngine;

namespace Lab02
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        // *** Pre Lab2
        //KeyboardState preState;
        //Sprite sprite;

        // *** Lab2
        SpiralMover spiralMover;

        public Lab2()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;// show the cursole
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
            // *** Pre-lab2
            //preState = Keyboard.GetState();

            // *** Lab2
            Time.Initialize();
            InputManager.Initialize();
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

            // TODO: use this.Content to load your game content here
            // *** Pre-lab2
            //sprite = new Sprite(Content.Load<Texture2D>("Square"));
            // *** Lab2
            spiralMover = new SpiralMover(Content.Load<Texture2D>("Square"), Vector2.Zero);
            font = Content.Load<SpriteFont>("Font");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // *** Pre-Lab2
            /*
            KeyboardState currentState = Keyboard.GetState();
            if (currentState.IsKeyDown(Keys.Left) && preState.IsKeyUp(Keys.Left)) sprite.Position += Vector2.UnitX * -5;
            if (currentState.IsKeyDown(Keys.Right) && preState.IsKeyUp(Keys.Right)) sprite.Position += Vector2.UnitX * 5;
            if (currentState.IsKeyDown(Keys.Space)) sprite.Rotation += 0.05f;
            preState = currentState;*/
            // *** Lab2
            Time.Update(gameTime);
            InputManager.Update();
            spiralMover.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            // *** Pre Lab2
            //sprite.Draw(spriteBatch);
            //spriteBatch.DrawString(font, "Pos: " + sprite.Position, new Vector2(50,50), Color.White);
            // *** Lab2
            spiralMover.Draw(spriteBatch);
            spriteBatch.DrawString(font, "R (Up/Down):" + spiralMover.Radius + " A (Right/Left):" + spiralMover.Amplitude + " F (Shift + R/L):" + spiralMover.Frequency, Vector2.Zero, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
