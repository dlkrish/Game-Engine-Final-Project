using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public interface IUpdatable
    {
        void Update();
    }

    public interface IRenderable
    {
        void Draw();
    }

    public interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }
}