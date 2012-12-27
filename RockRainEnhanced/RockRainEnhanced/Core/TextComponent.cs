namespace RockRainEnhanced.Core
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class TextComponent : DrawableGameComponent
    {
        readonly SpriteFont _font;
        readonly Color _color;
        readonly SpriteBatch _spriteBatch;

        Vector2 _position;
        
        public TextComponent(Game game, SpriteFont font, Vector2 position, Color color)
            : base(game)
        {
            _color = color;
            _font = font;
            _position = position;
            _spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
        }

        public string Text { get; set; }

        public override void Draw(GameTime gameTime)
        {
            float y = _position.Y;

            // Draw the text shadow
            _spriteBatch.DrawString(_font, Text ?? string.Empty, new Vector2(_position.X + 1, y + 1), Color.Black);

            // Draw the text item
            _spriteBatch.DrawString(_font, Text ?? string.Empty, new Vector2(_position.X, y), _color);

            base.Draw(gameTime);
        }
    }
}
