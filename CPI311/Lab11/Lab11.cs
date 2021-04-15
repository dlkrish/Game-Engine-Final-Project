using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System;

namespace Lab11
{
    public class Lab11 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D texture;
        SpriteFont font;
        Color background = Color.Blue;

        Button exitButton;

        Dictionary<String, Scene> scenes;
        Scene currentScene;

        List<GUIElement> guiElements;

        public class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            { Update = update; Draw = draw; }
        }

        public Lab11()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            scenes = new Dictionary<string, Scene>();

            guiElements = new List<GUIElement>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>("Square");
            font = Content.Load<SpriteFont>("Font");

            GUIGroup group = new GUIGroup();

            exitButton = new Button();
            exitButton.Texture = texture;
            exitButton.Text = "Exit";
            exitButton.Bounds = new Rectangle(50, 50, 300, 20);
            exitButton.Action += ExitGame;

            group.Children.Add(exitButton);

            Checkbox optionBox = new Checkbox();
            optionBox.Texture = texture;
            optionBox.Box = texture;
            optionBox.Bounds = new Rectangle(50, 75, 300, 20);
            optionBox.Text = "Full Screen";
            optionBox.Action += MakeFullScreen;
            group.Children.Add(optionBox);

            guiElements.Add(group);

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            currentScene = scenes["Menu"];
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            /*
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            */

            Time.Update(gameTime);
            InputManager.Update();

            exitButton.Update();
            currentScene.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);

            spriteBatch.Begin();
            exitButton.Draw(spriteBatch, font);
            spriteBatch.End();

            currentScene.Draw();

            base.Draw(gameTime);
        }

        //Other methods

        void ExitGame(GUIElement element)
        {
            //Lab 11-2
            currentScene = scenes["Play"];
            // *************
            background = (background == Color.White ? Color.Blue : Color.White);
        }

        void MainMenuUpdate()
        {
            foreach (GUIElement element in guiElements)
                element.Update();
        }
        void MainMenuDraw()
        {
            spriteBatch.Begin();
            foreach (GUIElement element in guiElements)
                element.Draw(spriteBatch, font);
            spriteBatch.End();
        }
        void PlayUpdate()
        {
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
        }
        void PlayDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Play Mode! Press \"Esc\" to go back",
                Vector2.Zero, Color.Black);
            spriteBatch.End();
        }

        void MakeFullScreen(GUIElement element)
        {
            ScreenManager.Setup(!ScreenManager.isFullScreen, ScreenManager.Width + 1, ScreenManager.Height + 1);
        }
    }
}
