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
        protected Texture2D texture;
        protected Random random;

        public PowerSource(Game game, ref Texture2D theTexture)
            : base(game, ref theTexture)
        {
            texture = theTexture;

            Frames = new List<Rectangle>();
            Rectangle frame = new Rectangle();
            frame.X = 291;
            frame.Y = 17;
            frame.Width = 14;
            frame.Height = 12;
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

            frameDelay = 200;
            random = new Random(GetHashCode());
            PutinStartPosition();
        }

        public void PutinStartPosition()
        {
            position.X = random.Next(Game.Window.ClientBounds.Width - currentFrame.Width);
            position.Y = -10;
            Enabled = false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Check if power source is still visible
            if (position.Y >= Game.Window.ClientBounds.Height)
            {
                position.Y = 0;
                Enabled = false;
            }

            position.Y += 1;

            base.Update(gameTime);
        }

        public bool CheckCollision(Rectangle rect)
        {
            Rectangle spriterect = new Rectangle((int) position.X, (int) position.Y, currentFrame.Width, currentFrame.Height);
            return spriterect.Intersects(rect);
        }
    }
}
