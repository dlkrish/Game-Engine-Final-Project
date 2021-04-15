using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System;

namespace Lab09
{
    public class Lab9 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model cube;
        Model sphere;
        AStarSearch search;
        List<Vector3> path;

        Random random = new Random();
        int size = 20;

        Camera camera;

        public Lab9()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            search = new AStarSearch(size, size); // size of grid 

            foreach (AStarNode node in search.Nodes)
                if (random.NextDouble() < 0.2)
                    search.Nodes[random.Next(size), random.Next(size)].Passable = false;

            search.Start = search.Nodes[0, 0];
            search.Start.Passable = true;
            search.End = search.Nodes[size - 1, size - 1];
            search.End.Passable = true;

            search.Search(); // A search is made here.

            path = new List<Vector3>();

            //Extract list of path from End-node by using Parent
            AStarNode current = search.End;

            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.One * 7;
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);

            sphere = Content.Load<Model>("Sphere");
            cube = Content.Load<Model>("Cube");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Time.Update(gameTime);
            InputManager.Update();

            if (InputManager.IsKeyPressed(Keys.Space))
            {
                while (!(search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable);
                while(!(search.End = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable);

                search.Search();
                path.Clear();
                AStarNode current = search.End;

                while (current != null)
                {
                    path.Insert(0, current.Position);
                    current = current.Parent;
                }

                /*
                search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]; // assign a random start node (passable)
                search.End = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]; // assign a random end node (passable)
                search.Search();
                path.Clear();
                AStarNode current = search.End;

                while (current != null)
                {
                    path.Insert(0, current.Position);
                    current = current.Parent;
                }
                */
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (AStarNode node in search.Nodes)
                if (!node.Passable)
                    cube.Draw(Matrix.CreateScale(0.5f, 0.05f, 0.5f) *
                       Matrix.CreateTranslation(node.Position), camera.View, camera.Projection);

            foreach (Vector3 position in path)
                sphere.Draw(Matrix.CreateScale(0.1f, 0.1f, 0.1f) *
                     Matrix.CreateTranslation(position), camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}
