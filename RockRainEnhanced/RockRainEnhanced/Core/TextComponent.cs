// -----------------------------------------------------------------------
// <copyright file="TextComponent.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RockRainEnhanced.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TextComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected readonly SpriteFont _font;
        public string Text { get; set; }
        Vector2 _position;
        private SpriteBatch _spriteBatch;
        Color _color;
        public TextComponent(Game game, SpriteFont font, Vector2 position, Color color)
            : base(game)
        {
            _color = color;
            _font = font;
            _position = position;
            _spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
        }

        public override void Draw(GameTime gameTime)
        {
            float y = _position.Y;

            Color theColor;

            // Draw the text shadow
            _spriteBatch.DrawString(_font, Text ?? string.Empty, new Vector2(_position.X + 1, y + 1), Color.Black);
            // Draw the text item
            _spriteBatch.DrawString(_font, Text ?? string.Empty, new Vector2(_position.X, y), _color);



            base.Draw(gameTime);
        }
    }
}
