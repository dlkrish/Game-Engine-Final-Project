using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

public class AnimatedSprite : Sprite
{
    public int Frames { get; set; }
    public int Clips { get; set; }
    public float Speed { get; set; }
    public float Frame { get; set; }
    public float Clip { get; set; }

    public AnimatedSprite(Texture2D texture, int frames, int clips) : base(texture)
    {
        Frames = frames;
        Clips = clips;
        Speed = 3;
        Width = texture.Width / Frames;
        Height = texture.Height / Clips;
        Origin = new Vector2(Width / 2, Height / 2);
    }

    public override void Update()
    {
 
            Frame += (Speed * Time.ElapsedGameTime) % Frames;
        
        Source = new Rectangle((int)(Width * Frame), (int)(Height * Clip), Width, Height);
        Origin = new Vector2(5, 5);

        if (Frame >= 7)
        {
            Frame = 0;
        }
        
    }


}