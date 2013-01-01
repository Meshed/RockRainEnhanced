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


namespace RockRainEnhanced.GameScenes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class CreditScene : GameScene
    {
        private readonly Texture2D _elements;
        private AudioLibrary _audio;
        private readonly SpriteBatch _spriteBatch;
        private readonly Rectangle _rockRect = new Rectangle(0, 0, 536, 131);
        private Vector2 _rockPosition;
        private Rectangle _rainRect = new Rectangle(120, 165, 517, 130);
        private Vector2 _rainPosition;
        private Rectangle _enhancedRect = new Rectangle(8, 304, 375, 144);
        private Vector2 _enhancedPosition;
        private bool _showEnchanced;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private readonly SpriteFont _font;

        public CreditScene(Game game, SpriteFont smallFont, SpriteFont largeFont, Texture2D background, Texture2D elements)
            : base(game)
        {
            _elements = elements;
            _spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            _audio = (AudioLibrary)Game.Services.GetService(typeof(AudioLibrary));

            Components.Add(new ImageComponent(game, background, ImageComponent.DrawMode.Center));
            _font = largeFont;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (MediaPlayer.State != MediaState.Playing) 
                MediaPlayer.Play(_audio.StartMusic);

#if XBOX360
            enhancedPosition = new Vector2((rainPosition.X + rainRect.Width - enhancedRect.Width/2), rainPosition.Y);
#else
            _enhancedPosition = new Vector2((_rainPosition.X + _rainRect.Width - _enhancedRect.Width / 2) - 80, _rainPosition.Y);
#endif

            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _showEnchanced = !_showEnchanced;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Draw(_elements, _rockPosition, _rockRect, Color.White);
            _spriteBatch.Draw(_elements, _rainPosition, _rainRect, Color.White);

            if (_showEnchanced)
            {
                _spriteBatch.Draw(_elements, _enhancedPosition, _enhancedRect, Color.White);
            }
            ShowDevelopers();
        }

        public override void Show()
        {
            SetGameTitleStartingPosition();
            _showEnchanced = false;

            base.Show();
        }

        private void SetGameTitleStartingPosition()
        {
            _rockPosition.X = (Game.Window.ClientBounds.Width - 700) / 2;
            _rockPosition.Y = 40;
            _rainPosition.X = (Game.Window.ClientBounds.Width - 400) / 2;
            _rainPosition.Y = 180;
        }
        private void ShowDevelopers()
        {
            const string measureString = "Temp";
            int fontHeight = (int)_font.MeasureString(measureString).Y + 10;
            int developerPositionX = (int)_rockPosition.X + 3;
            int developerPositionY = (int)_rainPosition.Y + _rainRect.Height + 20;
            _spriteBatch.DrawString(_font, "Developers:", new Vector2(developerPositionX + 2, developerPositionY + 20 + 2), Color.Black);
            _spriteBatch.DrawString(_font, "Developers:", new Vector2(developerPositionX, developerPositionY + 20), Color.Orange);
            _spriteBatch.DrawString(_font, "Mark Brown", new Vector2(developerPositionX + 75 + 2, developerPositionY + fontHeight + 2), Color.Black);
            _spriteBatch.DrawString(_font, "Mark Brown", new Vector2(developerPositionX + 75, developerPositionY + fontHeight), Color.Orange);
            _spriteBatch.DrawString(_font, "Brandon D'Imperio", new Vector2(developerPositionX + 75 + 2, developerPositionY + (fontHeight*2) + 2), Color.Black);
            _spriteBatch.DrawString(_font, "Brandon D'Imperio", new Vector2(developerPositionX + 75, developerPositionY + (fontHeight * 2)), Color.Orange);
        }
    }
}
