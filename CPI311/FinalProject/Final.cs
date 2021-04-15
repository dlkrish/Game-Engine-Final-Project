using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using Microsoft.Xna.Framework.Audio;
using System;

namespace FinalProject
{
    public class Final : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Camera camera;
        Transform cameraTransform;
        Light light;

        //Audio components
        SoundEffect gunSound;
        SoundEffect hitSound;
        SoundEffect missSound;
        SoundEffect failSound;
        SoundEffect levelComplete;
        SoundEffect BackgroundMusic;
        SoundEffectInstance soundInstance;

        //Visual components
        Ship ship;
        OtherShip ship2;
        OtherShip ship3;
        OtherShip ship4;

        int numAsteroids = 2;

        Target[] targetList = new Target[2];
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
        bool firstShotFired = false;
        bool allGone = false;
        bool levelOver = false;
        bool contentLoaded = false;
        bool proceed = false;
        bool titleScreen = true;
        bool gameOver = false;
        bool gameBeaten = false;
        bool timeStop = false;
        bool showInstructions = false;
        bool restart = false;
        bool highScore = false;

        bool ship1shot = false;
        bool ship2shot = false;
        bool ship3shot = false;
        bool ship4shot = false;

        int currentLevel = 1;
        double timeTaken = 0;
        double bestTime = 99999;
        double previousTime = 99999;

       // Vector3[] asteroidMov = new Vector3[GameConstants.NumAsteroids];
        Vector3[] bulletMov = new Vector3[GameConstants.NumBullets];

        public Final()
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
            gunSound = Content.Load<SoundEffect>("Gun");
            hitSound = Content.Load<SoundEffect>("Hit");
            missSound = Content.Load<SoundEffect>("Explosion");
            failSound = Content.Load<SoundEffect>("Fail");
            BackgroundMusic = Content.Load<SoundEffect>("Music");
            levelComplete = Content.Load<SoundEffect>("LevelComplete");

            soundInstance = BackgroundMusic.CreateInstance();
            soundInstance.Volume = 0.1f;
            soundInstance.Play();

            targetList = new Target[numAsteroids];

            stars = Content.Load<Texture2D>("Background");
            random = new Random();

            //Main Ship
            ship = new Ship(Content, camera, GraphicsDevice, light, "Spaceship_V3");
            ship.Transform.LocalPosition = new Vector3(1250, 0, 2200);
            ship.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);

            //Ship 2
            ship2 = new OtherShip(Content, camera, GraphicsDevice, light, "OtherSpaceship_V3");
            ship2.Transform.LocalPosition = new Vector3(2000, 0, 2000);
            ship2.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);

            //Ship 3
            ship3 = new OtherShip(Content, camera, GraphicsDevice, light, "OtherSpaceship_V3");
            ship3.Transform.LocalPosition = new Vector3(10, 0, 1500);
            ship3.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);

            //Ship 4
            ship4 = new OtherShip(Content, camera, GraphicsDevice, light, "OtherSpaceship_V3");
            ship4.Transform.LocalPosition = new Vector3(500, 0, 1500);
            ship4.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);

            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light);
                bulletList[i].Transform.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                bulletMov[i] = Vector3.Zero;
            }

            ResetAsteroids(); // look at the below private method

            targetList[0].Transform.LocalPosition = new Vector3(1250, 0, 500);
            targetList[1].Transform.LocalPosition = new Vector3(250, 0, 500);
            

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

            numAsteroids = currentLevel + 1;

            //Debug
            Console.WriteLine(currentLevel + "\n");

            if (proceed == true && currentLevel < 4 && gameOver == false)
            {
                if (currentLevel == 1)
                {
                    ship.Update();
                }

                if (currentLevel == 2)
                {
                    ship.Update();
                }

                if (currentLevel == 3)
                {
                    ship.Update();
                }

                for (int i = 0; i < GameConstants.NumBullets; i++)
                    bulletList[i].Update();
                for (int i = 0; i < numAsteroids; i++)
                    targetList[i].Update();

                //Level 1
                if (currentLevel == 1 && contentLoaded == false && restart == true)
                {
                    highScore = false;

                    ship.Transform.LocalPosition = new Vector3(1250, 0, 2200);
                    ship.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);

                    ship2.Transform.LocalPosition = new Vector3(2000, 0, 2000);
                    ship2.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);
                    ship2.Transform.Rotation = new Quaternion(0, 0, 0, 0.6614378f);

                    for (int i = 0; i < GameConstants.NumBullets; i++)
                    {
                        bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light);
                        bulletList[i].Transform.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                        bulletMov[i] = Vector3.Zero;
                    }

                    ResetAsteroids(); // look at the below private method

                    targetList[0].Transform.LocalPosition = new Vector3(1250, 0, 500);
                    targetList[1].Transform.LocalPosition = new Vector3(250, 0, 500);

                    contentLoaded = true;
                    restart = false;
                }

                //Level 2
                if (currentLevel == 2 && contentLoaded == false)
                {
                    ship.Transform.LocalPosition = new Vector3(2000, 0, 2200);
                    ship.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);

                    ship2.Transform.LocalPosition = new Vector3(1500, 0, 2000);
                    ship2.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);
                    ship2.Transform.Rotation = new Quaternion(0, 0, 0, 0.6614378f);

                    ship3.Transform.LocalPosition = new Vector3(10, 0, 1500);
                    ship3.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);
                    ship3.Transform.Rotation = new Quaternion(0, 0, 0, 0.6614378f);

                    for (int i = 0; i < GameConstants.NumBullets; i++)
                    {
                        bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light);
                        bulletList[i].Transform.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                        bulletMov[i] = Vector3.Zero;
                    }

                    ResetAsteroids(); // look at the below private method

                    targetList[0].Transform.LocalPosition = new Vector3(2000, 0, 500);
                    targetList[1].Transform.LocalPosition = new Vector3(1500, 0, 500);
                    targetList[2].Transform.LocalPosition = new Vector3(1000, 0, 500);

                    contentLoaded = true;
                }

                //Level 3
                if (currentLevel == 3 && contentLoaded == false)
                {
                    ship.Transform.LocalPosition = new Vector3(2000, 0, 2200);
                    ship.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);

                    ship2.Transform.LocalPosition = new Vector3(1500, 0, 2000);
                    ship2.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);
                    ship2.Transform.Rotation = new Quaternion(0, 0, 0, 0.6614378f);

                    ship3 = new OtherShip(Content, camera, GraphicsDevice, light, "OtherSpaceship_V3");
                    ship3.Transform.LocalPosition = new Vector3(10, 0, 1500);
                    ship3.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);

                    ship4 = new OtherShip(Content, camera, GraphicsDevice, light, "OtherSpaceship_V3");
                    ship4.Transform.LocalPosition = new Vector3(500, 0, 1500);
                    ship4.Transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);

                    for (int i = 0; i < GameConstants.NumBullets; i++)
                    {
                        bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light);
                        bulletList[i].Transform.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                        bulletMov[i] = Vector3.Zero;
                    }

                    ResetAsteroids(); // look at the below private method

                    targetList[0].Transform.LocalPosition = new Vector3(1250, 0, 500);
                    targetList[1].Transform.LocalPosition = new Vector3(2250, 0, 500);
                    targetList[2].Transform.LocalPosition = new Vector3(100, 0, 500);
                    targetList[3].Transform.LocalPosition = new Vector3(2000, 0, 500);

                    contentLoaded = true;
                }

                //Shoot
                if (InputManager.IsMousePressed(0) && lose == false && ship1shot == false)
                {
                    ship1shot = true;

                    //Bullet from main ship
                    for (int i = 0; i < GameConstants.NumBullets; i++)
                    {
                        bulletMov[i] = ship.Transform.Forward;
                        firstShotFired = true;

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

                //Ship 2 Shoot
                if (bulletList[0].isActive && bulletList[0].Transform.LocalPosition.Z <= ship2.Transform.LocalPosition.Z && lose == false && ship2shot == false)
                {
                    ship2shot = true;

                    for (int i = 0; i < GameConstants.NumBullets; i++)
                    {
                        bulletMov[i] = ship.Transform.Forward;

                        if (!bulletList[i].isActive)
                        {
                            bulletList[i].Rigidbody.Velocity = (ship.Transform.Forward) * GameConstants.BulletSpeedAdjustment;
                            bulletList[i].Transform.LocalPosition = ship2.Transform.Position + (200 * bulletList[i].Transform.Forward);
                            bulletList[i].isActive = true;

                            score -= GameConstants.ShotPenalty;

                            // sound
                            soundInstance = gunSound.CreateInstance();
                            soundInstance.Play();

                            break; //exit the loop     
                        }
                    }
                }

                //Ship 3 Shoot
                if (bulletList[0].isActive && bulletList[0].Transform.LocalPosition.Z <= ship3.Transform.LocalPosition.Z && lose == false && ship3shot == false && currentLevel > 1)
                {
                    ship3shot = true;

                    for (int i = 0; i < GameConstants.NumBullets; i++)
                    {
                        bulletMov[i] = ship.Transform.Forward;

                        if (!bulletList[i].isActive)
                        {
                            bulletList[i].Rigidbody.Velocity = (ship.Transform.Forward) * GameConstants.BulletSpeedAdjustment;
                            bulletList[i].Transform.LocalPosition = ship3.Transform.Position + (200 * bulletList[i].Transform.Forward);
                            bulletList[i].isActive = true;

                            score -= GameConstants.ShotPenalty;

                            // sound
                            soundInstance = gunSound.CreateInstance();
                            soundInstance.Play();

                            break; //exit the loop     
                        }
                    }
                }

                //Ship 4 Shoot
                if (bulletList[0].isActive && bulletList[0].Transform.LocalPosition.Z <= ship4.Transform.LocalPosition.Z && lose == false && ship4shot == false && currentLevel > 2)
                {
                    ship4shot = true;

                    for (int i = 0; i < GameConstants.NumBullets; i++)
                    {
                        bulletMov[i] = ship.Transform.Forward;

                        if (!bulletList[i].isActive)
                        {
                            bulletList[i].Rigidbody.Velocity = (ship.Transform.Forward) * GameConstants.BulletSpeedAdjustment;
                            bulletList[i].Transform.LocalPosition = ship4.Transform.Position + (200 * bulletList[i].Transform.Forward);
                            bulletList[i].isActive = true;

                            score -= GameConstants.ShotPenalty;

                            // sound
                            soundInstance = gunSound.CreateInstance();
                            soundInstance.Play();

                            break; //exit the loop     
                        }
                    }
                }

                //Toggle Instructions
                if (InputManager.IsKeyPressed(Keys.I))
                {
                    showInstructions = !showInstructions;
                }

                for (int i = 0; i < GameConstants.NumBullets; i++)
                {
                    bulletMov[i] = ship.Transform.Forward;
                }

                if (levelOver == true)
                {
                    levelOver = false;
                    proceed = false;
                }

                //Target Stuff
                Vector3 normal;
                for (int i = 0; i < targetList.Length; i++)
                    if (targetList[i].isActive)
                    {
                        if (ship.Collider.Collides(targetList[i].Collider, out normal))
                        {
                            lose = true;
                        }
                        for (int j = 0; j < bulletList.Length; j++)
                            if (bulletList[j].isActive)
                            {
                                if (targetList[i].Collider.Collides(bulletList[j].Collider, out normal))
                                {
                                    //Sound Effect
                                    soundInstance = hitSound.CreateInstance();
                                    soundInstance.Play();

                                    // Particles
                                    Particle particle = particleManager.getNext();
                                    particle.Position = targetList[i].Transform.Position;
                                    particle.Velocity = new Vector3(
                                      random.Next(-5, 5), 2, random.Next(-50, 50));
                                    particle.Acceleration = new Vector3(0, 3, 0);
                                    particle.MaxAge = random.Next(1, 6);
                                    particle.Init();
                                    targetList[i].isActive = false;
                                    bulletList[j].isActive = false;
                                    score += GameConstants.KillBonus;
                                    break; //no need to check other bullets
                                }
                            }
                    }

                // particles update
                particleManager.Update();


                for (int i = 0; i < bulletList.Length; i++)
                {
                    bulletList[i].Transform.LocalPosition += bulletMov[i] * 100;
                }

                win = true;

                for (int i = 0; i < targetList.Length; i++)
                {
                    if (targetList[i].isActive == true)
                    {
                        win = false;
                    }

                    //  targetList[i].Transform.LocalPosition += (ship.Transform.LocalPosition - targetList[i].Transform.LocalPosition) * 0.001f;
                }

                //Check for win
                win = true;
                for (int i = 0; i < targetList.Length; i++)
                {
                    if (targetList[i].isActive == true)
                    {
                        win = false;
                    }
                }

                //Win
                if (win == true && levelOver == false && currentLevel < 4)
                {
                    soundInstance = levelComplete.CreateInstance();
                    soundInstance.Play();
                    score += GameConstants.KillBonus;
                    win = false;
                    levelOver = true;
                    Console.WriteLine("REEEEEEEEEEEEEE");
                    currentLevel++;
                    numAsteroids++;
                    contentLoaded = false;
                    proceed = false;
                    
                    if (currentLevel == 2)
                    {
                        targetList = new Target[numAsteroids];

                        ResetAsteroids();

                        targetList[0].Transform.LocalPosition = new Vector3(250, 0, 500);
                        targetList[1].Transform.LocalPosition = new Vector3(1250, 0, 500);
                        targetList[2].Transform.LocalPosition = new Vector3(2000, 0, 500);
                    }

                    if (currentLevel == 3)
                    {
                        targetList = new Target[numAsteroids];

                        ResetAsteroids();

                        targetList[0].Transform.LocalPosition = new Vector3(250, 0, 500);
                        targetList[1].Transform.LocalPosition = new Vector3(1000, 0, 500);
                        targetList[2].Transform.LocalPosition = new Vector3(1250, 0, 500);
                        targetList[3].Transform.LocalPosition = new Vector3(2000, 0, 500);
                    }
                    //  firstShotFired = false;
                    //allGone = false;
                    // contentLoaded = false;
                }

                //Check if bullets aren't active
                allGone = true;
                for (int i = 0; i < bulletList.Length; i++)
                {
                    if (bulletList[i].isActive && firstShotFired == true)
                    {
                        allGone = false;
                    }
                }

                if (firstShotFired == true)
                {
                    //Console.WriteLine(bulletList[0].Transform.Position);

                    for (int i = 0; i < bulletList.Length && bulletList[i].isActive; i++)
                    {
                        if (bulletList[i].Transform.Position.X < -500 || bulletList[i].Transform.Position.X > 2800 || bulletList[i].Transform.Position.Z < 0 || bulletList[i].Transform.Position.Z > (GameConstants.PlayfieldSizeY + 200))
                        {
                            lose = true;
                          //  Console.WriteLine(i + ": " + bulletList[i].Transform.Position);
                        }
                    }
                }

                if (allGone == true && firstShotFired == true && win == false && levelOver == false)
                {
                    lose = true;
                }

                if (lose == true && levelOver == false)
                {
                    soundInstance = failSound.CreateInstance();
                    soundInstance.Play();
                    levelOver = true;
                }

                //Clear Bullets
                if (proceed == false)
                {
                    for (int i = 0; i < bulletList.Length; i++)
                    {
                        bulletList[i].isActive = false;
                    }
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
            }

            //Wait on click
            else
            {
                if (InputManager.IsKeyPressed(Keys.Enter))
                {
                    proceed = true;
                    win = false;
                    lose = false;
                    titleScreen = false;
                    levelOver = false;
                    firstShotFired = false;

                    ship1shot = false;
                    ship2shot = false;
                    ship3shot = false;
                    ship4shot = false;

                    if (InputManager.IsKeyDown(Keys.I))
                    {
                        showInstructions = !showInstructions;
                    }

                    // allGone = false;
                    // contentLoaded = false;

                    if (gameOver == true || gameBeaten == true)
                    {
                        numAsteroids = 2;
                        win = false;
                        lose = false;
                        currentLevel = 0;
                        gameOver = false;
                        gameBeaten = false;
                        timeTaken = 0;
                        titleScreen = true;
                        timeStop = false;
                        restart = true;
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(stars, new Rectangle(0, 0, 800, 600), Color.White);

            if (titleScreen == true && showInstructions == false)
            {
                spriteBatch.DrawString(font, "Target Blaster\nPress Enter to Play", new Vector2(375, 200), Color.LightGreen);

                if (InputManager.IsKeyDown(Keys.I))
                {
                    showInstructions = !showInstructions;
                }
            }

            if (proceed == false && titleScreen == false && currentLevel < 4 && lose == false && showInstructions == false)
            {
                spriteBatch.DrawString(font, "Level " + (currentLevel - 1) + " Complete!\nCurrent Score: " + score + "\nPress Enter to Continue", new Vector2(375, 200), Color.LightGreen);

                if (InputManager.IsKeyDown(Keys.I))
                {
                    showInstructions = !showInstructions;
                }    
            }

            if (lose == true && titleScreen == false && showInstructions == false)
            {
                spriteBatch.DrawString(font, "Too Bad!\nFinal Score: " + score + "\nPress Enter to Play Again!", new Vector2(375, 200), Color.Red);

                if (InputManager.IsKeyDown(Keys.I))
                {
                    showInstructions = !showInstructions;
                }

                gameOver = true;
            }

            if (currentLevel >= 4 && showInstructions == false)
            {
                if (timeStop == false)
                {
                    gameBeaten = true;

                    if (previousTime == 99999)
                    {
                        timeTaken = Time.TotalGameTime.TotalSeconds;
                        previousTime = 0;
                    }

                    else
                    {
                        timeTaken = Time.TotalGameTime.TotalSeconds - previousTime;
                    }

                    timeStop = true;
                    previousTime += timeTaken;
                }

                //Test if new high score
                if (timeTaken < bestTime)
                {
                    bestTime = timeTaken;
                    spriteBatch.DrawString(font, "New Best Time!!\n" + timeTaken + "seconds", new Vector2(300, 50), Color.LightGreen);
                    Console.WriteLine("BEST TIME!!!!!!!!!!!!!!!!!!!!!!!!!!1");
                    highScore = true;
                }

                if(highScore == true)
                {
                    spriteBatch.DrawString(font, "New Best Time!!\n" + timeTaken + "seconds", new Vector2(300, 50), Color.LightGreen);
                }

                spriteBatch.DrawString(font, "You Beat the Game!\nCongratulations!\nFinal Score: " + score + "\nTotal Time Taken: " + timeTaken + " seconds" + "\nPress Enter to Play Again!", new Vector2(300, 150), Color.Yellow);
                
                spriteBatch.DrawString(font, "Credits:\nCoding done by Danielle Krishna\n3D assets also made by Danielle Krishna\nSounds and Music provided by the internet\n\nThis game was brought to you by Yoshi Gang", new Vector2(300, 250), Color.Yellow);
            }

            //DEBUG
           // Console.WriteLine("Lose: " + lose + "\nWin: " + win + "\nOn Title Screen: " + titleScreen + "\nCurrent Level: " + currentLevel + "\nFirstShotFied: " + firstShotFired + "\nAll Gone" + allGone + "\nLevel Over" + levelOver + "\nContent Loaded" + contentLoaded + "\nProceed: " + proceed + "\n");

            if (lose == false && win == false && titleScreen == false && currentLevel < 4 && showInstructions == false && proceed == true)
            {
                    spriteBatch.DrawString(font, "Score: " + score, new Vector2(0, 10), Color.LightGreen);
                    spriteBatch.DrawString(font, "Level " + currentLevel, new Vector2(0, 30), Color.LightGreen);

                if (previousTime == 99999)
                {
                    spriteBatch.DrawString(font, "Time: " + Time.TotalGameTime, new Vector2(0, 50), Color.LightGreen);
                }

                else
                {
                    spriteBatch.DrawString(font, "Time: " + (Time.TotalGameTime.TotalSeconds - previousTime), new Vector2(0, 50), Color.LightGreen);
                }

                if (bestTime != 99999)
                {
                    spriteBatch.DrawString(font, "Best Time: " + bestTime + "seconds", new Vector2(0, 70), Color.LightGreen);
                }
            }

            if (showInstructions == false && gameBeaten == false)
            {
                spriteBatch.DrawString(font, "Press 'i' to show instructions", new Vector2(0, 90), Color.LightGreen);
            }

            if (showInstructions == true)
            {
                spriteBatch.DrawString(font, "Welcome to Target Blaster!\nThis is a strategy game that tests your timing and precision.\n\nHow to Play:\n1. Aim your main ship with the WASD keys and right click to shoot a bullet. You can \ncontinue to control the bullet with the WASD keys.\n\n2. Make sure to aim your bullet well, because once it passes another ship, another \nbullet from that ship will appear. Now both bullets are controlled with the WASD keys!\n\n3. The goal of the game is to hit all of the targets without the bullets flying offscreen. \nThis may be tricky, but that's the point of the game!\n\nHave Fun!\n\nPress 'B' to close the instructions menu", new Vector2(100, 100), Color.LightGreen);

                if (InputManager.IsKeyDown(Keys.B))
                {
                    showInstructions = !showInstructions;
                }
            }

            spriteBatch.End();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // ship, bullets, and asteroids
            if (lose == false && showInstructions == false)
            {
                if (currentLevel == 1 && proceed == true)
                {
                    ship.Draw();
                    ship2.Draw();
                }

                if (currentLevel == 2 && proceed == true)
                {
                    ship.Draw();
                    ship2.Draw();
                    ship3.Draw();
                }

                if (currentLevel == 3 && proceed == true)
                {
                    ship.Draw();
                    ship2.Draw();
                    ship3.Draw();
                    ship4.Draw();
                }

                if (proceed == true)
                {
                    for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Draw();
                    for (int i = 0; i < numAsteroids; i++) targetList[i].Draw();
                }

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

            base.Draw(gameTime);
        }

        private void ResetAsteroids()
        {
            float xStart;
            float yStart;

            for (int i = 0; i < numAsteroids; i++)
            {
                if (random.Next(2) == 0)
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                else
                    xStart = (float)GameConstants.PlayfieldSizeX;
                yStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeY;
                targetList[i] = new Target(Content, camera, GraphicsDevice, light);

                //Random Location
                targetList[i].Transform.Position = new Vector3(random.Next(0, GameConstants.PlayfieldSizeX), 0f, random.Next(0, GameConstants.PlayfieldSizeY));

                Vector3 normal;

                while (targetList[i].Collider.Collides(ship.Collider, out normal))
                {
                    targetList[i].Transform.Position = new Vector3(random.Next(0, GameConstants.PlayfieldSizeX), 0f, random.Next(0, GameConstants.PlayfieldSizeY));
                }

                targetList[i].Transform.Scale = new Vector3(0.2f, 0.2f, 0.2f);

                //Random Direction
                //asteroidMov[i] = new Vector3(random.Next(-10, 10), 0, random.Next(-10, 10));

                double angle = random.NextDouble() * 2 * Math.PI;
                targetList[i].Rigidbody.Velocity = new Vector3(
                   -(float)Math.Sin(angle), 0, (float)Math.Cos(angle)) *
            (GameConstants.AsteroidMinSpeed + (float)random.NextDouble() *
            GameConstants.AsteroidMaxSpeed);
                targetList[i].isActive = true;
            }
        }
    }
}