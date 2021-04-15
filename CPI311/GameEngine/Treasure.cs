using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CPI311.GameEngine
{
    public class Treasure : GameObject
    {
        public float catchDistance = 2;
        private int gridSize = 20; //grid size
        private TerrainRenderer Terrain;
        private Random random { get; set; }
        private Vector3 position;

        public Treasure(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, Random r)
        {
            Terrain = terrain;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("treasure"), Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            position = this.Transform.LocalPosition = new Vector3(this.Transform.LocalPosition.X, Terrain.GetAltitude(this.Transform.LocalPosition), this.Transform.LocalPosition.Z) + Vector3.Up;
        }

        public override void Update()
        {
            Transform.Update();
            base.Update();
        }
    }
}