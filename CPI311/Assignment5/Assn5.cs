using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;

namespace Assignment5
{
    public class Assn5 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TerrainRenderer terrain;
        Effect effect;
        Camera camera;
        Light light;

        SpriteFont font;
        int hits;

        Player player;
        Agent agent1;
        Agent agent2;
        Agent agent3;

        Treasure t1;
        Treasure t2;
        Treasure t3;
        Treasure t4;
        Treasure t5;

        Random random;

        public Assn5()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");

            terrain = new TerrainRenderer(Content.Load<Texture2D>("mazeH2"), Vector2.One * 100, Vector2.One * 200);
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale = new Vector3(1, 5, 1);
            terrain.NormalMap = Content.Load<Texture2D>("mazeN2");

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);

            camera = new Camera();
            camera.Transform = new Transform();     
            camera.Transform.LocalPosition = Vector3.Up * 60;
            camera.Transform.Rotation = new Quaternion(-0.707f, 0, 0, 0.707f);

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Backward * 5 + Vector3.Right * 5 + Vector3.Up * 5;

            random = new Random();

            hits = 0;

            player = new Player(terrain, Content, camera, GraphicsDevice, light, random);

            agent1 = new Agent(terrain, Content, camera, GraphicsDevice, light, random);
            agent2 = new Agent(terrain, Content, camera, GraphicsDevice, light, random);
            agent3 = new Agent(terrain, Content, camera, GraphicsDevice, light, random);

            /*
            t1 = new Treasure(terrain, Content, camera, GraphicsDevice, light, random);
            t2 = new Treasure(terrain, Content, camera, GraphicsDevice, light, random);
            t3 = new Treasure(terrain, Content, camera, GraphicsDevice, light, random);
            t4 = new Treasure(terrain, Content, camera, GraphicsDevice, light, random);
            t5 = new Treasure(terrain, Content, camera, GraphicsDevice, light, random);
            */
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            /*** Change Rotation ***
            if (InputManager.IsKeyDown(Keys.Up))
            {
                camera.Transform.Rotate(Vector3.Right, Time.ElapsedGameTime);
            }

            if (InputManager.IsKeyDown(Keys.Down))
            {
                camera.Transform.Rotate(Vector3.Right, -Time.ElapsedGameTime);
            }
            */

            if (agent1.CheckCollision(player) || agent2.CheckCollision(player) || agent3.CheckCollision(player))
            {
                hits++;
            }

            player.Update();
            agent1.Update();
            agent2.Update();
            agent3.Update();

            /*
            t1.Update();
            t2.Update();
            t3.Update();
            t4.Update();
            t5.Update();
            */

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["CameraPosition"].SetValue(camera.Position);
            effect.Parameters["LightPosition"].SetValue(light.Transform.Position);

            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrain.Draw();
            }

            player.Draw();

            agent1.Draw();
            agent2.Draw();
            agent3.Draw();

            /*
            t1.Draw();
            t2.Draw();
            t3.Draw();
            t4.Draw();
            t5.Draw();
            */

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Hits: " + (hits - 1), new Vector2(50, 50), Color.Red);
            spriteBatch.DrawString(font, "Time: " + Time.TotalGameTime, new Vector2(50, 100), Color.Red);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
