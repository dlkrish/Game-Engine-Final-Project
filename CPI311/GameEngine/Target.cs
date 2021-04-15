using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CPI311.GameEngine
{
    public class Target : GameObject
    {
        public bool isActive { get; set; }

        public Target(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light)
            : base()
        {
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Target_V3"),
                Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius * 100;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            //*** Additional Property (for Asteroid, isActive = true)
            isActive = true;
        }

        public override void Update()
        {
            if (!isActive) return;

            /*
            if (Transform.Position.X > GameConstants.PlayfieldSizeX ||
               Transform.Position.X < -GameConstants.PlayfieldSizeX ||
               Transform.Position.Z > GameConstants.PlayfieldSizeY ||
               Transform.Position.Z < -GameConstants.PlayfieldSizeY)
            {
                isActive = false;
                Rigidbody.Velocity = Vector3.Zero; // stop moving
            }
            */

            //Screen wrapping
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
            }

            base.Update();
        }

        public override void Draw()
        {
            if (isActive) base.Draw();
        }
    }
}
