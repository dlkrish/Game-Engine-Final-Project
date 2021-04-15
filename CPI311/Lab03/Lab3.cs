﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Storage;

using CPI311.GameEngine;
#endregion


namespace Lab03
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab3 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Model torusModel;
        Vector3 torusPosition;
        Vector3 torusScale;
        Vector3 torusRotation;  // Rotation as yaw (not used), pitch, and roll

        // For the camera
        Vector3 cameraPosition;
        Vector2 cameraSize;     // How wide/tall I want the camera to see
        Vector2 cameraCenter;   // Where should the camera center be?

        // Matrix for 3D
        Matrix torusWorld;
        Matrix view;
        Matrix projection;
        // Toggling variables
        bool isPerspective;     // Perspective or Orthographic?
        bool isSRT;             // Scale*Rotate*Translate or vice versa?


        public Lab3()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferMultiSampling = true;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;
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
            Time.Initialize();
            InputManager.Initialize();
            isPerspective = true;
            isSRT = true;
            torusPosition = Vector3.Zero;
            torusScale = Vector3.One;
            torusRotation = Vector3.Zero;
            cameraPosition = Vector3.Backward * 10;
            cameraCenter = Vector2.Zero;
            cameraSize = Vector2.One;

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
            font = Content.Load<SpriteFont>("Font");
            torusModel = Content.Load<Model>("Torus");
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver2,
                GraphicsDevice.Viewport.AspectRatio,
                0.1f, 1000f);

            foreach (ModelMesh mesh in torusModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();     // use default lighthing parameters
                    effect.PreferPerPixelLighting = true; // ask for good quality rendering
                }
            }

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
            Time.Update(gameTime);
            InputManager.Update();
            if (InputManager.IsKeyDown(Keys.Escape))
                Exit();

            if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
            {
                // Scale the Torus
                if (InputManager.IsKeyDown(Keys.Up))
                    torusScale += Vector3.One * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.Down))
                    torusScale -= Vector3.One * Time.ElapsedGameTime;

                // Scale camera size
                // Size should be positive, but we don't enforce anything here
                if (InputManager.IsKeyDown(Keys.W))
                    cameraSize += Vector2.UnitY * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.S))
                    cameraSize -= Vector2.UnitY * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.D))
                    cameraSize += Vector2.UnitX * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.A))
                    cameraSize -= Vector2.UnitX * Time.ElapsedGameTime;
            }
            else if (InputManager.IsKeyDown(Keys.LeftControl) || InputManager.IsKeyDown(Keys.RightControl))
            {
                // Rotate Torus
                // Only rotating on the X (pitch) and Z (roll) axes.
                if (InputManager.IsKeyDown(Keys.Up))
                    torusRotation += Vector3.Forward * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.Down))
                    torusRotation += Vector3.Backward * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.Left))
                    torusRotation += Vector3.Left * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.Right))
                    torusRotation += Vector3.Right * Time.ElapsedGameTime;

                // Change camera center
                // (0,0) would be true center. Anything else skews the camera
                if (InputManager.IsKeyDown(Keys.W))
                    cameraCenter += Vector2.UnitY * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.S))
                    cameraCenter -= Vector2.UnitY * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.D))
                    cameraCenter += Vector2.UnitX * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.A))
                    cameraCenter -= Vector2.UnitX * Time.ElapsedGameTime;
            }
            else
            {
                // Move the Torus
                if (InputManager.IsKeyDown(Keys.Up))
                    torusPosition += Vector3.Up * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.Down))
                    torusPosition += Vector3.Down * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.Left))
                    torusPosition += Vector3.Left * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.Right))
                    torusPosition += Vector3.Right * Time.ElapsedGameTime;

                // Move the Camera
                if (InputManager.IsKeyDown(Keys.W))
                    cameraPosition += Vector3.Up * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.S))
                    cameraPosition += Vector3.Down * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.D))
                    cameraPosition += Vector3.Right * Time.ElapsedGameTime;
                if (InputManager.IsKeyDown(Keys.A))
                    cameraPosition += Vector3.Left * Time.ElapsedGameTime;
            }

            // Create a viewing matrix for a camera at camera position, and looking dead ahead
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + Vector3.Forward, Vector3.Up);

            // Compute the top, left, right, and bottom using the center and size values
            Vector2 topLeft = cameraCenter - cameraSize;
            Vector2 bottomRight = cameraCenter + cameraSize;
            // Toggle the boolean value, if necessary, and then use the appropriate method to create the projection matrix
            // ^ is the XOR operator
            if (isPerspective ^= InputManager.IsKeyPressed(Keys.Tab))
                projection = Matrix.CreatePerspectiveOffCenter(topLeft.X, bottomRight.X, topLeft.Y, bottomRight.Y, 1f, 100f);
            else
                projection = Matrix.CreateOrthographicOffCenter(topLeft.X * 10, bottomRight.X * 10, topLeft.Y * 10, bottomRight.Y * 10, 1f, 100f);

            // Toggle the boolean value, if necessary, and then create the world matrix
            if (isSRT ^= InputManager.IsKeyPressed(Keys.Space))
                torusWorld = Matrix.CreateScale(torusScale) * Matrix.CreateFromYawPitchRoll(torusRotation.Y, torusRotation.X, torusRotation.Z) * Matrix.CreateTranslation(torusPosition);
            else
                torusWorld = Matrix.CreateTranslation(torusPosition) * Matrix.CreateFromYawPitchRoll(torusRotation.Y, torusRotation.X, torusRotation.Z) * Matrix.CreateScale(torusScale);

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
            GraphicsDevice.Clear(Color.DarkGray);
            GraphicsDevice.BlendState = BlendState.Opaque; //*** IMPORTANT! Need for 3D view with 2D sprite **************

            // SpriteBatch use will cause the depth test to be disabled. So we need the next line!
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            torusModel.Draw(torusWorld, view, projection); // Draw the model using the world, view, and projection matrices

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Camera: WASD (move), Shift+WASD (size), Ctrl+WASD (center)", Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "Model: Arrows (translate), Shift+Up/Down (scale), Ctrl+Arrows (rotate)", Vector2.UnitY * 20, Color.White);
            spriteBatch.DrawString(font, (isPerspective ? "Perspective" : "Orthographic") +
                                            " (Tab to change)\n" +
                                            (isSRT ? "Scale * Rotate * Translate" : "Translate * Rotate * Scale") +
                                            " (Space to change)", Vector2.UnitY * 40, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
