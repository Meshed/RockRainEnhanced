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


namespace RockRainEnhanced.Core
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Sprite : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private int activeFrame;
        protected readonly Texture2D texture;
        private List<Rectangle> frames;

        protected Vector2 position;
        protected TimeSpan elapsedTime = TimeSpan.Zero;
        protected Rectangle currentFrame;
        protected long frameDelay;
        protected SpriteBatch sbBatch;

        public Sprite(Game game, ref Texture2D theTexture)
            : base(game)
        {
            texture = theTexture;
            activeFrame = 0;
        }

        public List<Rectangle> Frames
        {
            get { return frames; }
            set { frames = value; }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            sbBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            // It's time for a next frame?
            if (elapsedTime > TimeSpan.FromMilliseconds(frameDelay))
            {
                elapsedTime -= TimeSpan.FromMilliseconds(frameDelay);
                activeFrame++;

                if (activeFrame == frames.Count)
                {
                    activeFrame = 0;
                }

                // Get the current frame
                currentFrame = frames[activeFrame];
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            sbBatch.Draw(texture, position, currentFrame, Color.White);

            base.Draw(gameTime);
        }
    }
}
