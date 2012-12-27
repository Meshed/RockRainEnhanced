namespace RockRainEnhanced.Core
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using RockRainEnhanced.Extensions;

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Sprite : DrawableGameComponent
    {
        readonly Texture2D _texture;

        long _frameDelay;

        int _activeFrame;
        
        List<Rectangle> _frames;

        Vector2 _position;

        TimeSpan _elapsedTime = TimeSpan.Zero;

        SpriteBatch _spriteBatch;

        public Sprite(Game game, ref Texture2D theTexture, Vector2 position = new Vector2(), long frameDelay = 0)
            : base(game)
        {
            this._texture = theTexture;
            this._activeFrame = 0;
            this.Position = position;
            this.FrameDelay = frameDelay;
            this._spriteBatch = Game.GetGameService<SpriteBatch>();
        }

        public List<Rectangle> Frames
        {
            get { return this._frames; }
            set { this._frames = value; }
        }

        protected Rectangle CurrentFrame { get; set; }

        protected Vector2 Position
        {
            get
            {
                return this._position;
            }
            set
            {
                this._position = value;
            }
        }

        protected long FrameDelay
        {
            get
            {
                return this._frameDelay;
            }
            set
            {
                this._frameDelay = value;
            }
        }

        protected Texture2D Texture
        {
            get
            {
                return this._texture;
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            this._elapsedTime += gameTime.ElapsedGameTime;

            // It's time for a next frame?
            if (this._elapsedTime > TimeSpan.FromMilliseconds(this.FrameDelay))
            {
                this._elapsedTime -= TimeSpan.FromMilliseconds(this.FrameDelay);
                this._activeFrame++;

                if (this._activeFrame == this._frames.Count)
                {
                    this._activeFrame = 0;
                }

                // Get the current frame
                this.CurrentFrame = this._frames[this._activeFrame];
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if(_spriteBatch != null)
            this._spriteBatch.Draw(this.Texture, this.Position, this.CurrentFrame, Color.White);

            base.Draw(gameTime);
        }
    }
}
