using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using RockRainEnhanced.Core;
using RockRainEnhanced.Extensions;

namespace RockRainEnhanced.GameScenes
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class StartScene : GameScene
    {
        private readonly TextMenuComponent _menu;

        private readonly Texture2D _elements;

        readonly AudioLibrary _audio;

        readonly SpriteBatch _spriteBatch;
        Rectangle _rockRect = new Rectangle(0, 0, 536, 131);
        Vector2 _rockPosition;
        Rectangle _rainRect = new Rectangle(120, 165, 517, 130);
        Vector2 _rainPosition;
        Rectangle _enhancedRect = new Rectangle(8, 304, 375, 144);
        Vector2 _enhancedPosition;
        bool _showEnchanced;
        TimeSpan _elapsedTime = TimeSpan.Zero;

        public StartScene(Game game, SpriteFont smallFont, SpriteFont largeFont, Texture2D background, Texture2D elements)
            : base(game)
        {
            this._elements = elements;
            Components.Add(new ImageComponent(game, background, ImageComponent.DrawMode.Center));

            string[] items = Enum.GetValues(typeof(MenuItems)).Cast<MenuItems>().Select(s => s.GetDisplayOrName()).ToArray();
            this._menu = new TextMenuComponent(game, smallFont, largeFont);
            this._menu.SetMenuItems(items);
            Components.Add(this._menu);

            this._spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            this._audio = (AudioLibrary)Game.Services.GetService(typeof(AudioLibrary));
        }

        public enum MenuItems
        {
            [Display("One Player")]
            OnePlayer,
            [Display("Two Players")]
            TwoPlayers,
            Help,
            Scores,
            Quit
        }

        public int SelectedMenuIndex
        {
            get { return this._menu.SelectedIndex; }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        public override void Update(GameTime gameTime)
        {
            if (!this._menu.Visible)
            {
                if (this._rainPosition.X >= (Game.Window.ClientBounds.Width - 595) / 2)
                {
                    this._rainPosition.X -= 15;
                }

                if (this._rockPosition.X <= (Game.Window.ClientBounds.Width - 715) / 2)
                {
                    this._rockPosition.X += 15;
                }
                else
                {
                    this._menu.Visible = true;
                    this._menu.Enabled = true;

                    MediaPlayer.Play(this._audio.StartMusic);

#if XBOX360
                    enhancedPosition = new Vector2((rainPosition.X + rainRect.Width - enhancedRect.Width/2), rainPosition.Y);
#else
                    this._enhancedPosition = new Vector2((this._rainPosition.X + this._rainRect.Width - this._enhancedRect.Width / 2) - 80, this._rainPosition.Y);
#endif
                }
            }
            else
            {
                this._elapsedTime += gameTime.ElapsedGameTime;

                if (this._elapsedTime > TimeSpan.FromSeconds(1))
                {
                    this._elapsedTime -= TimeSpan.FromSeconds(1);
                    this._showEnchanced = !this._showEnchanced;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this._spriteBatch.Draw(this._elements, this._rockPosition, this._rockRect, Color.White);
            this._spriteBatch.Draw(this._elements, this._rainPosition, this._rainRect, Color.White);

            if (this._showEnchanced)
            {
                this._spriteBatch.Draw(this._elements, this._enhancedPosition, this._enhancedRect, Color.White);
            }
        }

        public override void Show()
        {
            this._audio.NewMeteor.Play();
            this._rockPosition.X = -1 * this._rockRect.Width;
            this._rockPosition.Y = 40;
            this._rainPosition.X = Game.Window.ClientBounds.Width;
            this._rainPosition.Y = 180;

            // Put the menu centered in screen
            this._menu.Position = new Vector2(x: (Game.Window.ClientBounds.Width - this._menu.Width) / 2, y: 330);

            this._menu.Visible = false;
            this._menu.Enabled = false;
            this._showEnchanced = false;

            base.Show();
        }

        public override void Hide()
        {
            MediaPlayer.Stop();
            base.Hide();
        }
    }
}
