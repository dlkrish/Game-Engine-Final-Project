using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;

public class ProgressBar : Sprite
{
     public Color FillColor { get; set; }
     public float Value { get; set; }
     public float Speed { get; set; }

     public ProgressBar(Texture2D texture, float value, float speed) : base(texture)
     {
         FillColor = Color.White;
         Value = value;
         Speed = speed;
     }

    public override void Update()
    {
       
    }

    public override void Draw(SpriteBatch spriteBatch)
     {
         base.Draw(spriteBatch); // let the sprite do its work spriteBatch.Draw(Texture, Position, new Rectangle(?,?,?,?),    
         spriteBatch.Draw(Texture, Position, new Rectangle(10, 20, 0, 0), FillColor, Rotation, Origin, Scale, Effect, Layer);
     }
}
