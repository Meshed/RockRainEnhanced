using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RockRainEnhanced.Core;


namespace RockRainEnhanced
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PowerSource : Sprite
    {
        readonly Random random;

        public PowerSource(Game game, ref Texture2D theTexture)
            : base(game, ref theTexture)
        {
            Frames = new List<Rectangle>();
            var frame = new Rectangle { X = 291, Y = 17, Width = 14, Height = 12 };
            Frames.Add(frame);

            frame.Y = 30;
            Frames.Add(frame);

            frame.Y = 43;
            Frames.Add(frame);

            frame.Y = 57;
            Frames.Add(frame);

            frame.Y = 70;
            Frames.Add(frame);

            frame.Y = 82;
            Frames.Add(frame);

            FrameDelay = 200;
            random = new Random(GetHashCode());
            PutinStartPosition();
        }

        public void PutinStartPosition()
        {
            Position = new Vector2(random.Next(Game.Window.ClientBounds.Width - CurrentFrame.Width), -10);
            Enabled = false;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Check if power source is still visible
            if (Position.Y >= Game.Window.ClientBounds.Height)
            {
                Position = new Vector2(Position.X, 0);
                Enabled = false;
            }

            Position = new Vector2(Position.X, Position.Y + 1);

            base.Update(gameTime);
        }

        public bool CheckCollision(Rectangle rect)
        {
            var spriterect = new Rectangle((int) Position.X, (int) Position.Y, CurrentFrame.Width, CurrentFrame.Height);
            return spriterect.Intersects(rect);
        }
    }
}
