using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Assignment3
{
    public class Assn3 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Model model;
        Random random;

        bool fast;
        float speed;
        Vector3 defaultColor;

        //Multithreading
        bool haveThreadRunning = true;
        int lastSecondCollision = 0;
        private int lastSecondCollisions;

        bool showDiagnostics;
        bool speedColors;
        bool showTextures;
        bool multithreading;
        int numberCollisions;

        float mass;

        float fps;
        float currFps;
        int max = 100;
        Queue<float> q;


        Texture2D texture;
        Effect effect;

        Transform cameraTransform;
        Camera camera;

        Light light;

        BoxCollider boxCollider;

        List<GameObject> gameObjects;
        List<Renderer> renderers;

        public Assn3()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Need for shader
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            fast = false;
            speed = 1f;

            showDiagnostics = true;
            speedColors = false;
            showTextures = false;
            multithreading = true;

            random = new Random();

            gameObjects = new List<GameObject>();
            renderers = new List<Renderer>();

            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            //Multithreading
            haveThreadRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

            //FPS
            q = new Queue<float>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");

            effect = Content.Load<Effect>("SimpleShading");
            texture = Content.Load<Texture2D>("Square");

            for (int i = 0; i < 5; i++)
            {
                GameObject gObj = new GameObject();

                gObj.Transform.Position += Vector3.Right * 2 * i; //move each sphere to avoid overlap. 
                Rigidbody rigidbody = new Rigidbody();
                rigidbody.Transform = gObj.Transform;
                mass = (float)random.NextDouble() + 1f;
                rigidbody.Mass = mass;

                Vector3 direction = new Vector3
                (
                  (float)random.NextDouble(), (float)random.NextDouble(),
                  (float)random.NextDouble()
                );

                direction.Normalize();
                rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
                SphereCollider sphereCollider = new SphereCollider();
                sphereCollider.Radius = 1.0f * gObj.Transform.LocalScale.Y;
                sphereCollider.Transform = gObj.Transform;

               // gObj.Rigidbody = rigidbody;
               // gObj.Collider = sphereCollider;

                gameObjects.Add(gObj);
            }

            model = Content.Load<Model>("Sphere");
            font = Content.Load<SpriteFont>("font");
            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();

            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            renderers = new List<Renderer>();

            defaultColor = (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor;
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

            //FPS
            q.Clear();

            currFps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            q.Enqueue(currFps);

            if (q.Count > max)
            {
                q.Dequeue();
                fps = q.Average(i => i);
            }

            else
            {
                fps = currFps;
            }

            //Speed
            if (fast == true)
            {
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    Vector3 newVel = new Vector3((gameObjects[i].Rigidbody.Velocity.X * speed), (gameObjects[i].Rigidbody.Velocity.Y * speed), (gameObjects[i].Rigidbody.Velocity.Z * speed));

                    int limit = 50;

                    if (newVel.X >= limit)
                    {
                        newVel.X = limit;
                    }

                    if (newVel.X <= (0 - limit))
                    {
                        newVel.X = (0 - limit);
                    }

                    if (newVel.Y >= limit)
                    {
                        newVel.Y = limit;
                    }

                    if (newVel.Y <= (0 - limit))
                    {
                        newVel.Y = (0 - limit);
                    }

                    if (newVel.Z >= limit)
                    {
                        newVel.Z = limit;
                    }

                    if (newVel.Z <= (0 - limit))
                    {
                        newVel.Z = (0 - limit);
                    }

                    gameObjects[i].Rigidbody.Velocity = newVel;
                }
            }
            
            foreach (GameObject gameobject in gameObjects) gameobject.Rigidbody.Update();

            Vector3 normal; // it is updated if a collision happens

            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (boxCollider.Collides(gameObjects[i].Collider, out normal))
                {
                    numberCollisions++;

                    if (Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) < 0)
                        gameObjects[i].Rigidbody.Impulse +=
                            Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) * -2 * normal;
                }

                for (int j = i + 1; j < gameObjects.Count; j++)
                {
                    if (gameObjects[i].Collider.Collides(gameObjects[j].Collider, out normal))
                        numberCollisions++;

                    Vector3 velocityNormal = Vector3.Dot(normal,
                        gameObjects[i].Rigidbody.Velocity - gameObjects[j].Rigidbody.Velocity) * -2
                            * normal * gameObjects[i].Rigidbody.Mass * gameObjects[j].Rigidbody.Mass;
                    gameObjects[i].Rigidbody.Impulse += velocityNormal / 2;
                    gameObjects[j].Rigidbody.Impulse += -velocityNormal / 2;
                }
                
            }

            //Add more spheres
            if (InputManager.IsKeyPressed(Keys.A))
                {
                    GameObject gObj = new GameObject();

                    gObj.Transform.Position += Vector3.Right * 2;
                    Rigidbody rigidbody = new Rigidbody();
                    rigidbody.Transform = gObj.Transform;
                    mass = (float)random.NextDouble() + 1f;
                    rigidbody.Mass = mass;

                    Vector3 direction = new Vector3
                    (
                      (float)random.NextDouble(), (float)random.NextDouble(),
                      (float)random.NextDouble()
                    );

                    direction.Normalize();
                    rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
                    SphereCollider sphereCollider = new SphereCollider();
                    sphereCollider.Radius = 1.0f * gObj.Transform.LocalScale.Y;
                    sphereCollider.Transform = gObj.Transform;

                  //  gObj.Rigidbody = rigidbody;
                    //gObj.Collider = sphereCollider;

                    gameObjects.Add(gObj);
                }

                //Add sphere with random velocity at random position
                if (InputManager.IsKeyPressed(Keys.Up))
                {
                    addRandom();
                }

                //Remove Sphere
                if (InputManager.IsKeyPressed(Keys.Down))
                {
                    removeRandom();
                }

                //Increase Speed
                if (InputManager.IsKeyDown(Keys.Right))
                {
                    if (speed < 1.5f)
                    {
                        speed += 0.001f;
                    }

                    Console.WriteLine(speed + "\n");
                    fast = true;
                }

                //Decrease Speed
                if (InputManager.IsKeyDown(Keys.Left))
                {
                   for (int i = 0; i < gameObjects.Count; i++)
                    {
                        Vector3 newVel = new Vector3((gameObjects[i].Rigidbody.Velocity.X * 0.95f), (gameObjects[i].Rigidbody.Velocity.Y * 0.95f), (gameObjects[i].Rigidbody.Velocity.Z * 0.95f));

                        gameObjects[i].Rigidbody.Velocity = newVel;
                    }

                    speed = 1;
                    fast = false;
                }

                //Show/Hide Diagnostics
                if (InputManager.IsKeyPressed(Keys.LeftShift))
                {
                    showDiagnostics = !showDiagnostics;
                }

                //Show/Hide Speed Colors
                if (InputManager.IsKeyPressed(Keys.Space))
                {
                    speedColors = !speedColors;
                }

                //Show/Hide Textures
                if (InputManager.IsKeyPressed(Keys.LeftAlt))
                {
                    showTextures = !showTextures;
                }

                if (showTextures == true)
                {
                Console.WriteLine("BRUH");
                    foreach (GameObject gameobject in gameObjects)
                    {
                        effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                        (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Blue.ToVector3();
                    }
                }

                //Toggle Multithreading
                if (InputManager.IsKeyPressed(Keys.M))
                {
                    multithreading = !multithreading;

                    if (multithreading == false)
                    {
                        haveThreadRunning = false;
                    }

                    else
                    {
                        //Multithreading
                        haveThreadRunning = true;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));
                    }  
                }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Show colors according to speed
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (speedColors == true)
                {
                    Transform transform = gameObjects[i].Transform;
                    float speed = gameObjects[i].Rigidbody.Velocity.Length();
                    float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                    (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                               new Vector3(speedValue, speedValue, 1);
                    model.Draw(transform.World, camera.View, camera.Projection);
                }

                else
                {
                   (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = defaultColor;
                    model.Draw(gameObjects[i].Transform.World, camera.View, camera.Projection);
                }
            
            }

            //Renderers
            for (int i = 0; i < renderers.Count; i++) renderers[i].Draw();

            spriteBatch.Begin();

            //Show Diagnostics
            if (showDiagnostics == true)
            {
                spriteBatch.DrawString(font, "Number of Balls: " + gameObjects.Count + "\nNumber of Collisions: " + numberCollisions + "\nAverage Frame Rate over Last 10 Seconds: " + fps, Vector2.Zero, Color.White);
            }

            spriteBatch.DrawString(font, "Add Sphere at Setup Location: 'A' Key\nAdd / Remove Spheres at Random Position with Random Velocity: Up / Down Arrows\nChange Speed of Animation: Left / Right Arrows\nRemove Diagnostics Above: Shift\nShow Colors Corresponding to Speed: Spacebar\nShow Textures: Alt\nToggle Multithreading: 'M' Key (" + (multithreading ? "Multithreading ON" : "Multithreading OFF") + ")", new Vector2(0, 350), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Methods
        public void addRandom()
        {
            GameObject gObj = new GameObject();

            //Random Position
            float x = ((float)random.NextDouble() * 20) - 10;
            float y = ((float)random.NextDouble() * 20) - 10;
            float z = ((float)random.NextDouble() * 20) - 10;

            gObj.Transform.Position = new Vector3(x, y, z);

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = gObj.Transform;
            mass = (float)random.NextDouble() + 1f;
            rigidbody.Mass = mass;

            Vector3 direction = new Vector3
            (
              (float)random.NextDouble(), (float)random.NextDouble(),
              (float)random.NextDouble()
            );

            direction.Normalize();

            Vector3 v = direction * ((float)random.NextDouble() * 20);

            rigidbody.Velocity = v; //Random Velocity

            Console.WriteLine(v.X + "\n" + v.Y + "\n" + v.Z + "\n");

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * gObj.Transform.LocalScale.Y;
            sphereCollider.Transform = gObj.Transform;

            //gObj.Rigidbody = rigidbody;
            //gObj.Collider = sphereCollider;

            gameObjects.Add(gObj);
        }

        public void removeRandom()
        {
            if (gameObjects.Count > 0)
            {
                gameObjects.Remove(gameObjects[gameObjects.Count - 1]);
            }
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

        public void addGameObject()
        {
            GameObject gObj = new GameObject();

            Model model = Content.Load<Model>("Sphere");
            Rigidbody rigidbody = new Rigidbody();
            Collider collider = new SphereCollider();

            //Random Position
            float x = ((float)random.NextDouble() * 20) - 10;
            float y = ((float)random.NextDouble() * 20) - 10;
            float z = ((float)random.NextDouble() * 20) - 10;

            gObj.Transform.Position = new Vector3(x, y, z);

            rigidbody.Transform = gObj.Transform;
            mass = (float)random.NextDouble() + 1f;
            rigidbody.Mass = mass;

            Vector3 direction = new Vector3
            (
              (float)random.NextDouble(), (float)random.NextDouble(),
              (float)random.NextDouble()
            );

            direction.Normalize();

            Vector3 v = direction * ((float)random.NextDouble() * 20);

            rigidbody.Velocity = v; //Random Velocity

            Console.WriteLine(v.X + "\n" + v.Y + "\n" + v.Z + "\n");

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * gObj.Transform.LocalScale.Y;
            sphereCollider.Transform = gObj.Transform;

            Renderer renderer = new Renderer(model, gObj.Transform, camera, Content, GraphicsDevice, light, 1, "SimpleShading.fx", 0.20f, texture);

            gObj.Add<Rigidbody>(rigidbody);
            gObj.Add<Collider>(sphereCollider);
            gObj.Add<Renderer>(renderer);

            gameObjects.Add(gObj);
        }
    }
}
