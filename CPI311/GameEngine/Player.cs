using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CPI311.GameEngine
{
    public class Player : GameObject
    {
        public TerrainRenderer Terrain { get; set; }

        public Player(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, Random random) : base()
        {

            Terrain = terrain;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);
        }

        public override void Update()
        {
            int playerSpeed = 10;

            Vector3 nextPosUp = this.Transform.LocalPosition + this.Transform.Forward * Time.ElapsedGameTime * playerSpeed;
            Vector3 nextPosDown = this.Transform.LocalPosition + this.Transform.Backward * Time.ElapsedGameTime * playerSpeed;
            Vector3 nextPosLeft = this.Transform.LocalPosition + this.Transform.Left * Time.ElapsedGameTime * playerSpeed;
            Vector3 nextPosRight = this.Transform.LocalPosition + this.Transform.Right * Time.ElapsedGameTime * playerSpeed;

            // Control the player
            if (InputManager.IsKeyDown(Keys.W)) // move forward
            {
                if (Terrain.GetAltitude(nextPosUp) <= 1 / playerSpeed)
                {
                    this.Transform.LocalPosition += this.Transform.Forward * Time.ElapsedGameTime * playerSpeed;
                }

                else
                {
                    this.Transform.LocalPosition = new Vector3(this.Transform.LocalPosition.X, 0.5f, this.Transform.LocalPosition.Z);
                }
            }

            if (InputManager.IsKeyDown(Keys.S)) // move backward
            {
                if (Terrain.GetAltitude(nextPosDown) <= 1 / playerSpeed)
                {
                    this.Transform.LocalPosition += this.Transform.Backward * Time.ElapsedGameTime * playerSpeed;
                }


                else
                {
                    this.Transform.LocalPosition = new Vector3(this.Transform.LocalPosition.X, 0.5f, this.Transform.LocalPosition.Z);
                }
            }

            if (InputManager.IsKeyDown(Keys.A)) // move left
            {
                if (Terrain.GetAltitude(nextPosLeft) <= 1 / playerSpeed)
                {
                    this.Transform.LocalPosition += this.Transform.Left * Time.ElapsedGameTime * playerSpeed;
                }
            }

            if (InputManager.IsKeyDown(Keys.D)) // move right
            {
                if (Terrain.GetAltitude(nextPosRight) <= 1 / playerSpeed)
                {
                    this.Transform.LocalPosition += this.Transform.Right * Time.ElapsedGameTime * playerSpeed;
                }
            }

            // change the Y position corresponding to the terrain (maze)
            this.Transform.LocalPosition = new Vector3(
                this.Transform.LocalPosition.X,
                Terrain.GetAltitude(this.Transform.LocalPosition),
                this.Transform.LocalPosition.Z) + Vector3.Up;

            //Debug
            Console.WriteLine(this.Transform.LocalPosition + "\n");

            base.Update();
        }
    }
}
