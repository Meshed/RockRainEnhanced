using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RockRainEnhanced
{
    public class Score : DrawableGameComponent
    {
        protected SpriteBatch spriteBatch = null;
        protected Vector2 position = new Vector2();
        protected int value;
        protected int power;
        protected readonly SpriteFont font;
        protected readonly Color fontColor;

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public int Power
        {
            get { return power; }
            set { power = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Score(Game game, SpriteFont font, Color fontColor) : base(game)
        {
            this.font = font;
            this.fontColor = fontColor;
            spriteBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));
        }

        public override void Draw(GameTime gameTime)
        {
            string textToDraw = string.Format("Score: {0}", value);

            spriteBatch.DrawString(font, textToDraw, new Vector2(position.X + 1, position.Y + 1), Color.Black);
            spriteBatch.DrawString(font, textToDraw, new Vector2(position.X, position.Y), fontColor);

            float height = font.MeasureString(textToDraw).Y;
            textToDraw = string.Format("Power: {0}", power);
            spriteBatch.DrawString(font, textToDraw, new Vector2(position.X + 1, position.Y + 1 + height), Color.Black);
            spriteBatch.DrawString(font, textToDraw, new Vector2(position.X, position.Y + height), fontColor);

            base.Draw(gameTime);
        }
    }
}
