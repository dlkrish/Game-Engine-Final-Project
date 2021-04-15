using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Assignment4
{
    public class Assn4 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
      
        Camera camera;
        Transform cameraTransform;
        Light light;

        //Audio components
        SoundEffect gunSound;
        SoundEffectInstance soundInstance;

        //Visual components
        Ship ship;
        Asteroid[] asteroidList = new Asteroid[GameConstants.NumAsteroids];
        Bullet[] bulletList = new Bullet[GameConstants.NumBullets];

        //Score & background
        int score;
        Texture2D stars;
        SpriteFont lucidaConsole;
        Vector2 scorePosition = new Vector2(100, 50);

        // Particles
        ParticleManager particleManager;
        Texture2D particleTex;
        Effect particleEffect;

        Random random;
        bool win = false;
        bool lose = false;
        Vector3[] asteroidMov = new Vector3[GameConstants.NumAsteroids];
        Vector3[] bulletMov = new Vector3[GameConstants.NumBullets];

        public Assn4()
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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");

            //Camera
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = new Vector3(1200, 1150, 1550);
            cameraTransform.Rotation = new Quaternion(-0.7f, 0, 0, 0.13f);
            camera = new Camera();
            camera.Transform = cameraTransform;
            cameraTransform.Rotate(Vector3.Right, Time.ElapsedGameTime * 180);
            camera.FarPlane = 10000;

            //Light
            light = new Light();

            //Sound effect
            gunSound = Content.Load<SoundEffect>("tx0_fire1");

            stars = Content.Load<Texture2D>("B1_stars");
            random = new Random();

            ship = new Ship(Content, camera, GraphicsDevice, light, "p1_wedge");
            ship.Transform.LocalPosition = new Vector3(1250, 0, 2200);
            ship.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);

            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light);
                bulletList[i].Transform.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                bulletMov[i] = Vector3.Zero;
            }

            ResetAsteroids(); // look at the below private method

            // *** Particle
            particleManager = new ParticleManager(GraphicsDevice, 100);
            particleEffect = Content.Load<Effect>("ParticleShader-complete");
            particleTex = Content.Load<Texture2D>("fire");

            IsMouseVisible = true;
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);
            ship.Update();

            for (int i = 0; i < GameConstants.NumBullets; i++)
                bulletList[i].Update();
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
                asteroidList[i].Update();

            if (InputManager.IsMousePressed(0) && lose == false)
            {
                for (int i = 0; i < GameConstants.NumBullets; i++)
                {
                    bulletMov[i] = ship.Transform.Forward;

                    if (!bulletList[i].isActive)
                    {
                        bulletList[i].Rigidbody.Velocity =
                   (ship.Transform.Forward) * GameConstants.BulletSpeedAdjustment;
                        bulletList[i].Transform.LocalPosition = ship.Transform.Position +
                                (200 * bulletList[i].Transform.Forward);
                        bulletList[i].isActive = true;

                        score -= GameConstants.ShotPenalty;
                        // sound
                        soundInstance = gunSound.CreateInstance();
                        soundInstance.Play();
                        break; //exit the loop     
                    }
                }
            }

            Vector3 normal;
            for (int i = 0; i < asteroidList.Length; i++)
                if (asteroidList[i].isActive)
                {
                    if (ship.Collider.Collides(asteroidList[i].Collider, out normal))
                    {
                        lose = true;
                    }
                    for (int j = 0; j < bulletList.Length; j++)
                        if (bulletList[j].isActive)
                            if (asteroidList[i].Collider.Collides(bulletList[j].Collider, out normal))
                            {
                                // Particles
                                Particle particle = particleManager.getNext();
                                particle.Position = asteroidList[i].Transform.Position;
                                particle.Velocity = new Vector3(
                                  random.Next(-5, 5), 2, random.Next(-50, 50));
                                particle.Acceleration = new Vector3(0, 3, 0);
                                particle.MaxAge = random.Next(1, 6);
                                particle.Init();
                                asteroidList[i].isActive = false;
                                bulletList[j].isActive = false;
                                score += GameConstants.KillBonus;
                                break; //no need to check other bullets
                            }
                }

            // particles update
            particleManager.Update();

            for (int i = 0; i < bulletList.Length; i++)
            {
                    bulletList[i].Transform.LocalPosition += bulletMov[i] * 100;
            }

            win = true;

            for (int i = 0; i < asteroidList.Length; i++)
            {
                if (asteroidList[i].isActive == true)
                {
                    win = false;
                }

                asteroidList[i].Transform.LocalPosition += (ship.Transform.LocalPosition - asteroidList[i].Transform.LocalPosition) * 0.001f;

               // asteroidList[i].Transform.LocalPosition += asteroidMov[i];
            }

            /*
            //Debug
            if (InputManager.IsKeyDown(Keys.P))
            {
                cameraTransform.Rotate(Vector3.Right, -Time.ElapsedGameTime);
            }

            if (InputManager.IsKeyDown(Keys.PageUp))
            {
                cameraTransform.Position += Vector3.Down * 50;
            }

            if (InputManager.IsKeyDown(Keys.PageDown))
            {
                cameraTransform.Position += Vector3.Up * 50;
            }

            if (InputManager.IsKeyDown(Keys.Up))
            {
                cameraTransform.Position += Vector3.Forward * 50;
            }

            if (InputManager.IsKeyDown(Keys.Down))
            {
                cameraTransform.Position += Vector3.Backward * 50;
            }

            if (InputManager.IsKeyDown(Keys.Right))
            {
                cameraTransform.Position += Vector3.Right * 50;
            }

            if (InputManager.IsKeyDown(Keys.Left))
            {
                cameraTransform.Position += Vector3.Left * 50;
            }
            */

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(stars, new Rectangle(0, 0, 800, 600), Color.White);

            if (win == true)
            {
                spriteBatch.DrawString(font, "You Win!\nFinal Score: " + score, new Vector2(400, 200), Color.LightGreen);
            }

            if (lose == true)
            {
                spriteBatch.DrawString(font, "You Lose!\nFinalScore: " + score, new Vector2(400, 200), Color.Red);
            }

            spriteBatch.End();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // ship, bullets, and asteroids
            if (lose == false)
            {
                ship.Draw();
                for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Draw();
                for (int i = 0; i < GameConstants.NumAsteroids; i++) asteroidList[i].Draw();

                //particle draw
                GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                particleEffect.CurrentTechnique = particleEffect.Techniques["particle"];
                particleEffect.CurrentTechnique.Passes[0].Apply();
                particleEffect.Parameters["ViewProj"].SetValue(camera.View * camera.Projection);
                particleEffect.Parameters["World"].SetValue(Matrix.Identity);
                particleEffect.Parameters["CamIRot"].SetValue(
                Matrix.Invert(Matrix.CreateFromQuaternion(camera.Transform.Rotation)));
                particleEffect.Parameters["Texture"].SetValue(particleTex);
                particleManager.Draw(GraphicsDevice);
            }

            if (lose == false && win == false)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Score: " + score + "\nShipPos: " + ship.Transform.LocalPosition, new Vector2(0, 10), Color.LightGreen);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void ResetAsteroids()
        {
            float xStart;
            float yStart;

            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (random.Next(2) == 0)
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                else
                    xStart = (float)GameConstants.PlayfieldSizeX;
                yStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeY;
                asteroidList[i] = new Asteroid(Content, camera, GraphicsDevice, light);

                //Random Location
                asteroidList[i].Transform.Position = new Vector3(random.Next(0, GameConstants.PlayfieldSizeX), 0f, random.Next(0, GameConstants.PlayfieldSizeY));

                Vector3 normal;
                
                while (asteroidList[i].Collider.Collides(ship.Collider, out normal))
                {
                    asteroidList[i].Transform.Position = new Vector3(random.Next(0, GameConstants.PlayfieldSizeX), 0f, random.Next(0, GameConstants.PlayfieldSizeY));
                }
                
                asteroidList[i].Transform.Scale = new Vector3(0.2f, 0.2f, 0.2f);

                //Random Direction
                //asteroidMov[i] = new Vector3(random.Next(-10, 10), 0, random.Next(-10, 10));

                double angle = random.NextDouble() * 2 * Math.PI;
                asteroidList[i].Rigidbody.Velocity = new Vector3(
                   -(float)Math.Sin(angle), 0, (float)Math.Cos(angle)) *
            (GameConstants.AsteroidMinSpeed + (float)random.NextDouble() *
            GameConstants.AsteroidMaxSpeed);
                asteroidList[i].isActive = true;
            }
        }
    }
}
