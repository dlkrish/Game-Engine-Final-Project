using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Lab04
{
    public class Lab4 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model model;
        Transform modelTransform;
        Transform cameraTransform;
        Camera camera;

        // **** Update
        Model model2;
        Transform model2Transform;

        public Lab4()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("Torus");
            modelTransform = new Transform();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 5;
            camera = new Camera();
            camera.Transform = cameraTransform;
            // *** Update ********************************
            model2 = Content.Load<Model>("Sphere");
            model2Transform = new Transform();
            model2Transform.LocalPosition = Vector3.Right * 4;
            //*** Parenting ************************************
            model2Transform.Parent = modelTransform;
            //**************************************************

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();     // use default lighthing parameters
                    effect.PreferPerPixelLighting = true; // ask for good quality rendering
                }
            }
            foreach (ModelMesh mesh in model2.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();     // use default lighthing parameters
                    effect.PreferPerPixelLighting = true; // ask for good quality rendering
                }
            }
        }

        protected override void UnloadContent() { }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyDown(Keys.W))
                cameraTransform.LocalPosition += cameraTransform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S))
                cameraTransform.LocalPosition += cameraTransform.Backward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.A))
                cameraTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D))
                cameraTransform.Rotate(Vector3.Up, -Time.ElapsedGameTime);

            // *** Update to rotate model *****
            if (InputManager.IsKeyDown(Keys.Right))
                modelTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Left))
                modelTransform.Rotate(Vector3.Up, -Time.ElapsedGameTime);

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            model.Draw(modelTransform.World, camera.View, camera.Projection);
            model2.Draw(model2Transform.World, camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}
