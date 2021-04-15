using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CPI311.GameEngine
{
    public class Agent : GameObject
    {
        public AStarSearch search;
        List<Vector3> path;

        public int timer = 5;
        public float catchDistance = 2;
        private float speed = 5f; //moving speed
        private int gridSize = 20; //grid size
        private TerrainRenderer Terrain;
        private Random random { get; set; }

        public Agent(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, Random r) : base()
        {
            Terrain = terrain;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("enemy"), Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            path = null;
            random = r;
            search = new AStarSearch(gridSize, gridSize);
            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * i + gridW / 2 - terrain.size.X / 2, 0, gridH * j + gridH / 2 - terrain.size.Y / 2);
                    if (Terrain.GetAltitude(pos) > 1.0)
                        search.Nodes[j, i].Passable = false;
                }
            }
        }

        public override void Update()
        {
            if (path != null && path.Count > 0)
            {
                Vector3 currentPosition = Transform.Position;
                Vector3 destinationPoint = GetGridPosition(path[0]);
                // Move to the destination along the path
                currentPosition.Y = 0;
                destinationPoint.Y = 0;
                Vector3 direction = Vector3.Distance(destinationPoint, currentPosition) == 0 ? Vector3.Zero : Vector3.Normalize(destinationPoint - currentPosition);

                this.Rigidbody.Velocity = new Vector3(direction.X, 0, direction.Z) * speed;
                if (Vector3.Distance(currentPosition, destinationPoint) < 1f) // if it reaches to a point, go to the next in path
                {
                    path.RemoveAt(0);
                    if (path.Count == 0) // if it reached to the goal
                    {
                        path = null;
                        return;
                    }
                }
            }

            else
            {
                // Search again to make a new path.
                RandomPathFinding();
                Transform.LocalPosition = GetGridPosition(path[0]);
            }

            this.Transform.LocalPosition = new Vector3(this.Transform.LocalPosition.X, Terrain.GetAltitude(this.Transform.LocalPosition), this.Transform.LocalPosition.Z) + Vector3.Up;
            Transform.Update();
            base.Update();
        }

        private Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.size.X / search.Cols;
            float gridH = Terrain.size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2, 0, gridH * gridPos.Z + gridH / 2 - Terrain.size.Y / 2);
        }

        private void RandomPathFinding()
        {
            while (!(search.Start = search.Nodes[random.Next(search.Rows), random.Next(search.Cols)]).Passable) ;
            search.End = search.Nodes[search.Rows / 2, search.Cols / 2];
            search.Search();
            path = new List<Vector3>();
            AStarNode current = search.End;
            var count = 0;

            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }
        }

        //Collision with player
        public virtual bool CheckCollision(Player player)
        {
            Vector3 normal;

            if (player.Get<Collider>().Collides(this.Get<Collider>(), out normal))
            {
                path = null;
                return true;
            }

            return false;
        }
    }
}




