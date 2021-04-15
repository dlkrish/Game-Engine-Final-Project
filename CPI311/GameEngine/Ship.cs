using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CPI311.GameEngine
{
    public class Ship : GameObject
    {
        public Ship(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, String filename) : base()
        {
            Transform = this.Transform;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>(filename),
                Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius / 15;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
        }

        public override void Update()
        {
            /*
            if (InputManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                this.Transform.LocalPosition += this.Transform.Forward * Time.ElapsedGameTime * GameConstants.PlayerSpeed;
                Console.WriteLine(this.Transform.LocalPosition);
            }

            if (InputManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
                this.Transform.LocalPosition += this.Transform.Backward * Time.ElapsedGameTime * GameConstants.PlayerSpeed;
                Console.WriteLine(this.Transform.LocalPosition);
            }
            */

            if (InputManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                this.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * GameConstants.PlayerRotateSpeed);
               // Console.WriteLine(this.Transform.LocalPosition);
            }

            if (InputManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            {
                this.Transform.Rotate(Vector3.Down, Time.ElapsedGameTime * GameConstants.PlayerRotateSpeed);
               // Console.WriteLine(this.Transform.LocalPosition);
            }

            //Screen wrapping
            /*
            if (this.Transform.Position.X > GameConstants.PlayfieldSizeX)
            {
                this.Transform.Position = new Vector3(0, this.Transform.LocalPosition.Y, this.Transform.LocalPosition.Z);
            }

            if (this.Transform.Position.X < 0)
            {
                this.Transform.Position = new Vector3(GameConstants.PlayfieldSizeX, this.Transform.LocalPosition.Y, this.Transform.LocalPosition.Z);
            }
            
            if (this.Transform.Position.Z > GameConstants.PlayfieldSizeY)
            {
                this.Transform.Position = new Vector3(this.Transform.LocalPosition.X, this.Transform.LocalPosition.Y, 0);
            }
            
            if (this.Transform.Position.Z < 0)
            {
                this.Transform.Position = new Vector3(this.Transform.LocalPosition.X, this.Transform.LocalPosition.Y, GameConstants.PlayfieldSizeY);
            
    */
            
            //Call parent's update
            base.Update();
        }
    }
}
