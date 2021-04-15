using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;

namespace Assignment2
{
    public class Assn2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        bool fp;
        bool mouseOn;

        //Plane
        Model plane;
        Transform planeTransform;

        //Models
        Model sun;
        Transform sunTransform;

        Model mercury;
        Transform mercuryTransform;

        Model earth;
        Transform earthTransform;

        Model moon;
        Transform moonTransform;

        Model fpCam;
        Transform fpCamTransform;

        //Cameras
        Transform fpCameraTransform;
        Camera fpCamera;

        Transform tpCameraTransform;
        Camera tpCamera;

        //States
        KeyboardState curr;
        KeyboardState prev;

        float speed;

        public Assn2()
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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");

            speed = 1;
            fp = true;
            mouseOn = true;

            //Load cameras
            fpCameraTransform = new Transform();
            fpCameraTransform.LocalPosition = Vector3.Backward * 15;
            fpCamera = new Camera();
            fpCamera.Transform = fpCameraTransform;

            tpCameraTransform = new Transform();
            tpCameraTransform.LocalPosition = Vector3.Up * 15;
            tpCamera = new Camera();
            tpCamera.Transform = tpCameraTransform;
            tpCameraTransform.LocalPosition = new Vector3(-1, 15.85f, -4.225f);
            tpCameraTransform.Rotation = new Quaternion(-0.77f, 0.003f, -0.048f, 0.637f);


            //Load models
            sun = Content.Load<Model>("sun");
            sunTransform = new Transform();

            mercury = Content.Load<Model>("mercury");
            mercuryTransform = new Transform();
            mercuryTransform.LocalPosition = Vector3.Right * 10;

            earth = Content.Load<Model>("earth");
            earthTransform = new Transform();
            earthTransform.LocalPosition = Vector3.Right * 16;

            moon = Content.Load<Model>("moon");
            moonTransform = new Transform();
            moonTransform.LocalPosition = Vector3.Right * 4;

            //Load Plane
            plane = Content.Load<Model>("Plane");
            planeTransform = new Transform();
            planeTransform.LocalPosition = Vector3.Zero;
            planeTransform.LocalPosition -= new Vector3(0, 20, 0);

            fpCam = Content.Load<Model>("camera");
            fpCamTransform = new Transform();
            fpCamTransform.LocalPosition = fpCameraTransform.LocalPosition;
            fpCamTransform.LocalRotation = fpCameraTransform.LocalRotation;

            //Parenting
            mercuryTransform.Parent = sunTransform;
            earthTransform.Parent = sunTransform;
            moonTransform.Parent = earthTransform;

            prev = Keyboard.GetState();

            foreach (ModelMesh mesh in sun.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();     // use default lighthing parameters
                    effect.PreferPerPixelLighting = true; // ask for good quality rendering
                }
            }

            foreach (ModelMesh mesh in mercury.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();     // use default lighthing parameters
                    effect.PreferPerPixelLighting = true; // ask for good quality rendering
                }
            }

            foreach (ModelMesh mesh in earth.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();     // use default lighthing parameters
                    effect.PreferPerPixelLighting = true; // ask for good quality rendering
                }
            }

            foreach (ModelMesh mesh in moon.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();     // use default lighthing parameters
                    effect.PreferPerPixelLighting = true; // ask for good quality rendering
                }
            }
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            fpCamTransform.LocalPosition = fpCameraTransform.LocalPosition;
            fpCamTransform.LocalRotation = fpCameraTransform.LocalRotation;

            curr = Keyboard.GetState();

            //Mouse Stuff
            MouseState mouseState = Mouse.GetState();
            int x = mouseState.X;
            int y = mouseState.Y;

            InputManager.Update();
            Time.Update(gameTime);

            //Toggle cameras
            if (curr.IsKeyDown(Keys.Tab) && !prev.IsKeyDown(Keys.Tab))
                fp = !fp;

            if (curr.IsKeyDown(Keys.Space) && !prev.IsKeyDown(Keys.Space))
                mouseOn = !mouseOn;

            if (mouseOn == true)
            {
                this.IsMouseVisible = true;
            }

            else if (mouseOn == false)
            {
                this.IsMouseVisible = false;
            }

            if (fp == true)
            {
                //First Person Movement
                if (InputManager.IsKeyDown(Keys.W))
                    fpCameraTransform.LocalPosition += fpCameraTransform.Forward * Time.ElapsedGameTime * 5;
                if (InputManager.IsKeyDown(Keys.S))
                    fpCameraTransform.LocalPosition += fpCameraTransform.Backward * Time.ElapsedGameTime * 5;
                if (InputManager.IsKeyDown(Keys.A))
                    fpCameraTransform.LocalPosition += fpCameraTransform.Left * Time.ElapsedGameTime * 5;
                if (InputManager.IsKeyDown(Keys.D))
                    fpCameraTransform.LocalPosition += fpCameraTransform.Right * Time.ElapsedGameTime * 5;

                //First Person Rotation
                if (InputManager.IsKeyDown(Keys.Left))
                    fpCameraTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
                if (InputManager.IsKeyDown(Keys.Right))
                    fpCameraTransform.Rotate(Vector3.Up, -Time.ElapsedGameTime);
                if (InputManager.IsKeyDown(Keys.Up))
                    fpCameraTransform.Rotate(Vector3.Right, Time.ElapsedGameTime);
                if (InputManager.IsKeyDown(Keys.Down))
                    fpCameraTransform.Rotate(Vector3.Right, -Time.ElapsedGameTime);

                //First Person Toggle Zoom
                if (InputManager.IsKeyDown(Keys.PageUp) && fpCamera.FieldOfView > 0.05f)
                    fpCamera.FieldOfView -= 0.05f;
                if (InputManager.IsKeyDown(Keys.PageDown) && fpCamera.FieldOfView < 3.09f)
                    fpCamera.FieldOfView += 0.05f;

                if (mouseOn == true)
                {
                    //Mouse zoom
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        fpCameraTransform.LocalPosition += fpCameraTransform.Forward * Time.ElapsedGameTime * 10;
                    }

                    if (mouseState.RightButton == ButtonState.Pressed)
                    {
                        fpCameraTransform.LocalPosition -= fpCameraTransform.Forward * Time.ElapsedGameTime * 10;
                    }

                    //Mouse Position View
                    if (x < 200) //Left
                    {
                        fpCameraTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
                    }

                    else if (x > 600) //Right
                    {
                        fpCameraTransform.Rotate(Vector3.Up, -Time.ElapsedGameTime);
                    }

                    else if (y < 100) //Up
                    {
                        fpCameraTransform.Rotate(Vector3.Right, Time.ElapsedGameTime);
                    }

                    else if (y > 380) //Down
                    {
                        fpCameraTransform.Rotate(Vector3.Right, -Time.ElapsedGameTime);
                    }
                }
            }

            else if (fp == false)
            {
                //Third Person Movement
                if (InputManager.IsKeyDown(Keys.W))
                    tpCameraTransform.LocalPosition += new Vector3(0, 0, -1) * Time.ElapsedGameTime * 5;
                if (InputManager.IsKeyDown(Keys.S))
                    tpCameraTransform.LocalPosition += new Vector3(0, 0, 1) * Time.ElapsedGameTime * 5;
                if (InputManager.IsKeyDown(Keys.A))
                    tpCameraTransform.LocalPosition += new Vector3(-1, 0, 0) * Time.ElapsedGameTime * 5;
                if (InputManager.IsKeyDown(Keys.D))
                    tpCameraTransform.LocalPosition += new Vector3(1, 0, 0) * Time.ElapsedGameTime * 5;

                //Third Person Rotation
                if (InputManager.IsKeyDown(Keys.Left))
                    tpCameraTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
                if (InputManager.IsKeyDown(Keys.Right))
                    tpCameraTransform.Rotate(Vector3.Up, -Time.ElapsedGameTime);
                if (InputManager.IsKeyDown(Keys.Up))
                    tpCameraTransform.Rotate(Vector3.Right, Time.ElapsedGameTime);
                if (InputManager.IsKeyDown(Keys.Down))
                    tpCameraTransform.Rotate(Vector3.Right, -Time.ElapsedGameTime);

                //Third Person Toggle Zoom
                if (InputManager.IsKeyDown(Keys.PageUp) && tpCamera.FieldOfView > 0.05f)
                    tpCamera.FieldOfView -= 0.05f;
                if (InputManager.IsKeyDown(Keys.PageDown) && tpCamera.FieldOfView < 3.09f)
                    tpCamera.FieldOfView += 0.05f;

                if (mouseOn == true)
                {
                    //Mouse zoom
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        tpCameraTransform.LocalPosition += tpCameraTransform.Forward * Time.ElapsedGameTime * 10;
                    }

                    if (mouseState.RightButton == ButtonState.Pressed)
                    {
                        tpCameraTransform.LocalPosition -= tpCameraTransform.Forward * Time.ElapsedGameTime * 10;
                    }

                    //Mouse Position View
                    if (x < 200) //Left
                    {
                        tpCameraTransform.LocalPosition += tpCameraTransform.Left * Time.ElapsedGameTime * 5;
                    }

                    else if (x > 600) //Right
                    {
                        tpCameraTransform.LocalPosition += tpCameraTransform.Right * Time.ElapsedGameTime * 5;
                    }

                    else if (y < 100) //Up
                    {
                        tpCameraTransform.LocalPosition += new Vector3(0, 0, -1) * Time.ElapsedGameTime * 5;
                    }

                    else if (y > 380) //Down
                    {
                        tpCameraTransform.LocalPosition += new Vector3(0, 0, -1) * -Time.ElapsedGameTime * 5;
                    }
                }
            }

            //Toggle Speed
            if (InputManager.IsKeyDown(Keys.LeftShift))
                speed += 0.1f;
            if (InputManager.IsKeyDown(Keys.LeftControl) && speed > 0.1)
                speed -= 0.1f;

            //Rotate and Revolve
            sunTransform.Rotate(Vector3.Up, Time.ElapsedGameTime * speed);
            earthTransform.Rotate(Vector3.Up, Time.ElapsedGameTime * speed * 5);
            moonTransform.Rotate(Vector3.Up, Time.ElapsedGameTime * speed * 10);

            prev = curr;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Depth Buffer
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = dss;

            if (fp == true)
            {
                plane.Draw(planeTransform.World, fpCamera.View, fpCamera.Projection);
                sun.Draw(sunTransform.World, fpCamera.View, fpCamera.Projection);
                mercury.Draw(mercuryTransform.World, fpCamera.View, fpCamera.Projection);
                earth.Draw(earthTransform.World, fpCamera.View, fpCamera.Projection);
                moon.Draw(moonTransform.World, fpCamera.View, fpCamera.Projection);
            }
            
            if (fp == false)
            {
                plane.Draw(planeTransform.World, tpCamera.View, tpCamera.Projection);
                sun.Draw(sunTransform.World, tpCamera.View, tpCamera.Projection);
                mercury.Draw(mercuryTransform.World, tpCamera.View, tpCamera.Projection);
                earth.Draw(earthTransform.World, tpCamera.View, tpCamera.Projection);
                moon.Draw(moonTransform.World, tpCamera.View, tpCamera.Projection);
                fpCam.Draw(fpCamTransform.World, tpCamera.View, tpCamera.Projection);
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Camera: WASD (move), Arrow Keys (rotate), PgUp/PgDown (Change Field of View)", Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "Planets: Left Shift/Ctrl (Toggle Speed)", Vector2.UnitY * 20, Color.White);
            spriteBatch.DrawString(font, (fp ? "First Person" : "Third Person") +
                                            " (Tab to change)\n" +
                                            (mouseOn ? "Mouse Control ON" : "Mouse Control OFF") +
                                            " (Space to change)\nLeft/Right Click (Mouse Zoom In/Out)\n*First person camera represented by a green camera in third person mode", Vector2.UnitY * 40, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

