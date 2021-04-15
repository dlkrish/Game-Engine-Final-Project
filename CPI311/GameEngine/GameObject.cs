using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace CPI311.GameEngine
{
    public class GameObject
    {
        public Transform Transform { get; protected set; }
        // Common components
        public Camera Camera { get { return Get<Camera>(); } }
        public Rigidbody Rigidbody { get { return Get<Rigidbody>(); } }
        //public Renderer Renderer { get { return Get<Renderer>(); } }
        public Collider Collider { get { return Get<Collider>(); } }

        // All Components
        private Dictionary<Type, Component> Components { get; set; }
        private List<IUpdateable> Updatables { get; set; }
        private List<IRenderable> Renderables { get; set; }
        private List<IDrawable> Drawables { get; set; }

        // *** After Lab6
        public Light Light { get { return Get<Light>(); } }

        public GameObject()
        {
            Transform = new Transform();
            Components = new Dictionary<Type, Component>();
            Updatables = new List<IUpdateable>();
            Renderables = new List<IRenderable>();
            Drawables = new List<IDrawable>();
        }
        // *** Updated in Assignment 3: Section2  ******************************
        public void Add<T>(T component) where T : Component
        {
            Remove<T>();
            component.GameObject = this;
            component.Transform = Transform;
            Components.Add(typeof(T), component);
            if (component is IUpdateable)
                Updatables.Add(component as IUpdateable);
            if (component is IRenderable)
                Renderables.Add(component as IRenderable);
            if (component is IDrawable)
                Drawables.Add(component as IDrawable);
        }
        //**********************************************************

        public T Add<T>() where T : Component, new()
        {
            Remove<T>();
            T component = new T();
            component.GameObject = this;
            component.Transform = Transform;
            Components.Add(typeof(T), component);
            if (component is IUpdateable)
                Updatables.Add(component as IUpdateable);
            if (component is IRenderable)
                Renderables.Add(component as IRenderable);
            if (component is IDrawable)
                Drawables.Add(component as IDrawable);
            return component;
        }

        public T Get<T>() where T : Component
        {
            if (Components.ContainsKey(typeof(T)))
                return Components[typeof(T)] as T;
            else
                return null;
        }

        public void Remove<T>() where T : Component
        {
            if (Components.ContainsKey(typeof(T)))
            {
                Component component = Components[typeof(T)];
                Components.Remove(typeof(T));
                if (component is IUpdateable)
                    Updatables.Remove(component as IUpdateable);
                if (component is IRenderable)
                    Renderables.Remove(component as IRenderable);
                if (component is IDrawable)
                    Drawables.Remove(component as IDrawable);
            }
        }

        public virtual void Update() //** Updated to virtual in Assignment 5 to override
        {
            foreach (IUpdatable component in Updatables)
                component.Update();
        }

        public virtual void Draw()  //** Updated to virtual in Assignment 5 to override
        {
            foreach (IRenderable component in Renderables)
                component.Draw();
        }

        public virtual void Draw(SpriteBatch spriteBatch) //** Updated to virtual in Assignment 5 to override
        {
            foreach (IDrawable component in Drawables)
                component.Draw(spriteBatch);
        }
    }
}


/*
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace CPI311.GameEngine
{
    public class GameObject : Component
    {
        public Transform Transform { get; protected set; }

        // Common components
        public Camera Camera { get { return Get<Camera>(); } }
        public Rigidbody Rigidbody { get; set; }
        public Renderer Renderer { get { return Get<Renderer>(); } }
        public Collider Collider { get; set; }

        // All Components
        private Dictionary<Type, Component> Components { get; set; }
        private List<IUpdatable> Updatables { get; set; }
        private List<IRenderable> Renderables { get; set; }
        private List<IDrawable> Drawables { get; set; }

        // *** After Lab6
        public Light Light { get { return Get<Light>(); } }

        public GameObject()
        {
            Transform = new Transform();

            Components = new Dictionary<Type, Component>();
            Updatables = new List<IUpdatable>();
            Renderables = new List<IRenderable>();
            Drawables = new List<IDrawable>();
        }
        // *** Updated in Assignment 3: Section2  ******************************
        public void Add<T>(T component) where T : Component
        {
            Remove<T>();
            component.GameObject = this;
            component.Transform = Transform;
            Components.Add(typeof(T), component);
            if (component is IUpdateable)
                Updatables.Add(component as IUpdatable);
            if (component is IRenderable)
                Renderables.Add(component as IRenderable);
            if (component is IDrawable)
                Drawables.Add(component as IDrawable);
        }
        //**********************************************************

        public T Add<T>() where T : Component, new()
        {
            Remove<T>();
            T component = new T();
            component.GameObject = this;
            component.Transform = Transform;
            Components.Add(typeof(T), component);
            if (component is IUpdateable)
                Updatables.Add(component as IUpdatable);
            if (component is IRenderable)
                Renderables.Add(component as IRenderable);
            if (component is IDrawable)
                Drawables.Add(component as IDrawable);
            return component;
        }

        public T Get<T>() where T : Component
        {
            if (Components.ContainsKey(typeof(T)))
                return Components[typeof(T)] as T;
            else
                return null;
        }

        public void Remove<T>() where T : Component
        {
            if (Components.ContainsKey(typeof(T)))
            {
                Component component = Components[typeof(T)];
                Components.Remove(typeof(T));
                if (component is IUpdateable)
                    Updatables.Remove(component as IUpdatable);
                if (component is IRenderable)
                    Renderables.Remove(component as IRenderable);
                if (component is IDrawable)
                    Drawables.Remove(component as IDrawable);
            }
        }

        public virtual void Update() //** Updated to virtual in Assignment 5 to override
        {
            foreach (IUpdatable component in Updatables)
                component.Update();
        }

        public virtual void Draw()  //** Updated to virtual in Assignment 5 to override
        {
            foreach (IRenderable component in Renderables)
                component.Draw();
        }

        public virtual void Draw(SpriteBatch spriteBatch) //** Updated to virtual in Assignment 5 to override
        {
            foreach (IDrawable component in Drawables)
                component.Draw(spriteBatch);
        }
    }
}
*/
