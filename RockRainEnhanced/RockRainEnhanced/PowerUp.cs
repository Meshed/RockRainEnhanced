using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RockRainEnhanced.Core;

namespace RockRainEnhanced
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PowerUp : Sprite
    {
        protected Random Random;

        public int EffectValue { get; set; }

        public PowerUp(Game game, ref Texture2D texture)
            : base(game, ref texture)
        {
            // TODO: Construct any child components here
        }

        public void PutinStartPosition()
        {
            position.X = Random.Next(Game.Window.ClientBounds.Width - currentFrame.Width);
            position.Y = -texture.Height;
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
            // Check if power up is still visible
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
            Rectangle spriterect = new Rectangle((int)position.X, (int)position.Y, currentFrame.Width, currentFrame.Height);
            return spriterect.Intersects(rect);
        }
    }
}
