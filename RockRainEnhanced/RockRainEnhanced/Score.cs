using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RockRainEnhanced.Extensions;

namespace RockRainEnhanced
{
    public class Score : DrawableGameComponent
    {
        readonly SpriteBatch _spriteBatch;
        Vector2 _position;
        int _value;
        int _power;
        readonly SpriteFont _font;

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public int Power
        {
            get { return _power; }
            set { _power = value; }
        }
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        
        public Color ValueFontColor { get; set; }
        public Color PowerFontColor { get; set; }

        public Score(Game game, SpriteFont font, Color fontColor) : base(game)
        {
            _font = font;
            ValueFontColor = fontColor;
            PowerFontColor = fontColor;

            _spriteBatch = Game.GetGameService<SpriteBatch>();
        }

        public override void Draw(GameTime gameTime)
        {
            string textToDraw = string.Format("Score: {0}", _value);

            _spriteBatch.DrawString(_font, textToDraw, new Vector2(_position.X + 1, _position.Y + 1), Color.Black);
            _spriteBatch.DrawString(_font, textToDraw, new Vector2(_position.X, _position.Y), ValueFontColor);

            float height = _font.MeasureString(textToDraw).Y;
            textToDraw = string.Format("Power: {0}", _power);
            _spriteBatch.DrawString(_font, textToDraw, new Vector2(_position.X + 1, _position.Y + 1 + height), Color.Black);
            _spriteBatch.DrawString(_font, textToDraw, new Vector2(_position.X, _position.Y + height), PowerFontColor);

            base.Draw(gameTime);
        }
    }
}
