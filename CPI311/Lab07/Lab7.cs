using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Lab07
{
    public class Lab7 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        bool haveThreadRunning = true;
        int lastSecondCollision = 0;
        private int lastSecondCollisions;
        int numberCollisions = 0;

        //Stuff from lab 6
        SpriteFont font;
        Model model;
        Random random;
        Transform cameraTransform;
        Camera camera;

        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        List<Transform> transforms;
        List<Renderer> renderers;

        BoxCollider boxCollider;


        public Lab7()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        //Private Methods
        private void ThreadMethod(Object obj)
        {
            //To do
        }

        private void CollisionReset(Object obj)
        {
            while (haveThreadRunning)
            {
                lastSecondCollisions = numberCollisions;
                numberCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void AddSphere()
        {
            Transform transform = new Transform();
            transform.Position += Vector3.Zero; //move each sphere to avoid overlap. 
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = transform;
            rigidbody.Mass = 1;

            Vector3 direction = new Vector3(
              (float)random.NextDouble(), (float)random.NextDouble(),
              (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * transform.LocalScale.Y;

            sphereCollider.Transform = transform;

            transforms.Add(transform);
            colliders.Add(sphereCollider);
            rigidbodies.Add(rigidbody);
        }



        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();

            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            renderers = new List<Renderer>();
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            haveThreadRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

            for (int i = 0; i < 2; i++)
            {
                Transform transform = new Transform();
                transform.Position += Vector3.Right * 2 * i; //move each sphere to avoid overlap. 
                Rigidbody rigidbody = new Rigidbody();
                rigidbody.Transform = transform;
                rigidbody.Mass = 1;
                model = Content.Load<Model>("Sphere");

                Vector3 direction = new Vector3(
                  (float)random.NextDouble(), (float)random.NextDouble(),
                  (float)random.NextDouble());
                direction.Normalize();
                rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
                SphereCollider sphereCollider = new SphereCollider();
                sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
                sphereCollider.Transform = transform;

                transforms.Add(transform);
                colliders.Add(sphereCollider);
                rigidbodies.Add(rigidbody);
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();

            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            renderers = new List<Renderer>();

            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            model = Content.Load<Model>("Sphere");
                for (int i = 0; i < 5; i++) AddSphere();

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

            //From Lab 6
            foreach (Rigidbody rigidbody in rigidbodies)
                rigidbody.Update();

            Vector3 normal; // it is updated if a collision happens

            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    numberCollisions++;
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0)
                        rigidbodies[i].Impulse +=
                           Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                }

                for (int j = i + 1; j < transforms.Count; j++)
                {
                    if (colliders[i].Collides(colliders[j], out normal))
                        numberCollisions++;

                    Vector3 velocityNormal = Vector3.Dot(normal,
                        rigidbodies[i].Velocity - rigidbodies[j].Velocity) * -2
                           * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                    rigidbodies[i].Impulse += velocityNormal / 2;
                    rigidbodies[j].Impulse += -velocityNormal / 2;
                }
            }

           if (InputManager.IsKeyPressed(Keys.Space))
            {
                AddSphere();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            for (int i = 0; i < transforms.Count; i++)
            {
                Transform transform = transforms[i];
                float speed = rigidbodies[i].Velocity.Length();
                float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                           new Vector3(speedValue, speedValue, 1);
                model.Draw(transform.World, camera.View, camera.Projection);
            }

            for (int i = 0; i < renderers.Count; i++) renderers[i].Draw();


            base.Draw(gameTime);
        }
    }
}
