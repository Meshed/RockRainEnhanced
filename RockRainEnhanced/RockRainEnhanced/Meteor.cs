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
                frameDelay = 200 - (ySpeed*5);
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
            position.X = random.Next(Game.Window.ClientBounds.Width - currentFrame.Width);
            position.Y = 0;
            YSpeed = 1 + random.Next(9);
            XSpeed = random.Next(3) - 1;
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
            // Check if the meteor is still visible
            if ((position.Y >= Game.Window.ClientBounds.Height) ||
                (position.Y < 0) ||
                (position.X >= Game.Window.ClientBounds.Width) ||
                (position.X <= 0))
            {
                PutinStartPosition();
            }

            // Move meteor
            position.Y += ySpeed;
            position.X += xSpeed;

            base.Update(gameTime);
        }

        public bool CheckCollision(Meteor otherMeteor)
        {
            Rectangle spriteRect = new Rectangle((int)position.X, (int)position.Y, currentFrame.Width, currentFrame.Height);
            return otherMeteor.CheckCollision(spriteRect);
        }

        public bool CheckCollision(Rectangle rect)
        {
            Rectangle spriteRect = new Rectangle((int) position.X, (int) position.Y, currentFrame.Width, currentFrame.Height);
            return spriteRect.Intersects(rect);
        }

        public void Bounce(Meteor meteor)
        {
            XSpeed = XSpeed*-1;
            YSpeed = YSpeed*-1;

            if (position.X < meteor.position.X)
            {
                // Left
                position.X = meteor.position.X - currentFrame.Width;
            }
            else
            {
                // Right
                position.X = meteor.position.X + meteor.currentFrame.Width;
            }
        }
    }
}
