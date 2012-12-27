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
            Position = new Vector2(Random.Next(Game.Window.ClientBounds.Width - CurrentFrame.Width), -Texture.Height);
            Enabled = false;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Check if power up is still visible
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
            Rectangle spriterect = new Rectangle((int)Position.X, (int)Position.Y, CurrentFrame.Width, CurrentFrame.Height);
            return spriterect.Intersects(rect);
        }
    }
}
