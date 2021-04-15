using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using CPI311.GameEngine;

namespace Lab08
{
    public class Lab8 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Texture2D texture;

        SoundEffect gunSound;
        SoundEffectInstance soundInstance;

        Model model;
        Camera camera, topDownCamera;
        List<Transform> transforms;
        List<Collider> colliders;
        List<Camera> cameras;

        Effect effect;

        public Lab8()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Need for shader
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsMouseVisible = true;

            cameras = new List<Camera>();
            colliders = new List<Collider>();
            transforms = new List<Transform>();
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
            gunSound = Content.Load<SoundEffect>("Gun");
            model = Content.Load<Model>("Sphere");
            effect = Content.Load<Effect>("SimpleShading");
            texture = Content.Load<Texture2D>("Square");
            
            (model.Meshes[0].Effects[0] as BasicEffect).EnableDefaultLighting();

            Transform transform = new Transform();

            SphereCollider collider = new SphereCollider();
            collider.Radius = 1f;
            collider.Transform = transform;

            transforms.Add(transform);
            colliders.Add(collider);

            // *** Lab 8 Item ***********************
            ScreenManager.Setup(true, 1920, 1080);
            //***************************************

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5;
            camera.Position = new Vector2(0f, 0f);
            camera.Size = new Vector2(0.5f, 1f);
            camera.AspectRatio = camera.Viewport.AspectRatio;

            topDownCamera = new Camera();
            topDownCamera.Transform = new Transform();
            topDownCamera.Transform.LocalPosition = Vector3.Up * 10;
            topDownCamera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            topDownCamera.Position = new Vector2(0.5f, 0f);
            topDownCamera.Size = new Vector2(0.5f, 1f);
            topDownCamera.AspectRatio = topDownCamera.Viewport.AspectRatio;

            cameras.Add(topDownCamera);
            cameras.Add(camera);

        }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();
            if (InputManager.IsKeyDown(Keys.Escape))
                Exit();

            Ray ray = camera.ScreenPointToWorldRay(InputManager.GetMousePosition());

            foreach (Collider collider in colliders)
            {
                if (collider.Intersects(ray) != null)
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                    (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Blue.ToVector3();

                    //Play sound when clicked
                    soundInstance = gunSound.CreateInstance();

                    if (InputManager.IsMousePressed(0))
                    {
                        soundInstance.IsLooped = false;
                        soundInstance.Play();
                    }
                }

                else
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                    (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Red.ToVector3();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (Camera camera in cameras)
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState();
                GraphicsDevice.Viewport = camera.Viewport;
                Matrix view = camera.View;
                Matrix projection = camera.Projection;

                effect.CurrentTechnique = effect.Techniques[1];
                effect.Parameters["View"].SetValue(view);
                effect.Parameters["Projection"].SetValue(projection);
                effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 + Vector3.Right * 5);
                effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
                effect.Parameters["Shininess"].SetValue(15f);
                effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
                effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.5f));
                effect.Parameters["DiffuseTexture"].SetValue(texture);

                foreach (Transform transform in transforms)
                {
                    effect.Parameters["World"].SetValue(transform.World);

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        foreach (ModelMesh mesh in model.Meshes)
                        {
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                                GraphicsDevice.Indices = part.IndexBuffer;
                                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                            }
                        }
                    }
                }
            }

            base.Draw(gameTime);
        }
    }
}