using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RockRainEnhanced.Core
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ImageComponent : DrawableGameComponent
    {
        readonly Texture2D _texture;

        readonly SpriteBatch _spriteBatch;

        readonly Rectangle _imageRect;

        public ImageComponent(Game game, Texture2D texture, DrawMode drawMode)
            : base(game)
        {
            this._texture = texture;

            this._spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            switch (drawMode)
            {
                case DrawMode.Center:
                    this._imageRect = new Rectangle(
                        (Game.Window.ClientBounds.Width - texture.Width) / 2,
                        (Game.Window.ClientBounds.Height - texture.Height) / 2,
                        texture.Width,
                        texture.Height);
                    break;
                case DrawMode.Stretch:
                    this._imageRect = new Rectangle(0, 0, Game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
                    break;
            }
        }

        public enum DrawMode
        {
            Center = 1,
            Stretch
        }

        public override void Draw(GameTime gameTime)
        {
            this._spriteBatch.Draw(this._texture, this._imageRect, Color.White);
            base.Draw(gameTime);
        }
    }
}
