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
    public class Meteor : Sprite
    {
        protected int ySpeed;
        protected int xSpeed;
        protected Random random;

        private int index;

        public int YSpeed
        {
            get { return ySpeed; }
            set
            {
                ySpeed = value;
                FrameDelay = 200 - (ySpeed*5);
            }
        }

        public int XSpeed
        {
            get { return xSpeed; }
            set { xSpeed = value; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public Meteor(Game game, ref Texture2D theTexture)
            : base(game, ref theTexture)
        {
            Frames = new List<Rectangle>();
            Rectangle frame = new Rectangle();

            frame.X = 468;
            frame.Y = 0;
            frame.Width = 49;
            frame.Height = 44;
            Frames.Add(frame);

            frame.Y = 50;
            Frames.Add(frame);

            frame.Y = 98;
            frame.Height = 45;
            Frames.Add(frame);

            frame.Y = 146;
            frame.Height = 49;
            Frames.Add(frame);

            frame.Y = 200;
            frame.Height = 44;
            Frames.Add(frame);

            frame.Y = 250;
            Frames.Add(frame);

            frame.Y = 299;
            Frames.Add(frame);

            frame.Y = 350;
            frame.Height = 49;
            Frames.Add(frame);

            random = new Random(GetHashCode());
            PutinStartPosition();
        }

        public void PutinStartPosition()
        {
            Position = new Vector2(random.Next(Game.Window.ClientBounds.Width - CurrentFrame.Width), 0);
            YSpeed = 1 + random.Next(9);
            XSpeed = random.Next(3) - 1;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Check if the meteor is still visible
            if ((Position.Y >= Game.Window.ClientBounds.Height) ||
                (Position.X >= Game.Window.ClientBounds.Width) ||
                (Position.X <= 0))
            {
                PutinStartPosition();
            }

            // Move meteor
            Position = new Vector2(Position.X + xSpeed, Position.Y + ySpeed);
            

            base.Update(gameTime);
        }

        public bool CheckCollision(Rectangle rect)
        {
            var spriteRect = new Rectangle((int) Position.X, (int) Position.Y, CurrentFrame.Width, CurrentFrame.Height);
            return spriteRect.Intersects(rect);
        }
    }
}
