using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RockRainEnhanced.Core
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TextMenuComponent : DrawableGameComponent
    {
        readonly SpriteBatch _spriteBatch;
        readonly SpriteFont _regularFont, _selectedFont;
        readonly List<string> _menuItems;
        readonly AudioLibrary _audio;
        Color _regularColor = Color.White, _selectedColor = Color.Red;
        Vector2 _position;
        int _selectedIndex;
        int _width, _height;
        KeyboardState _oldKeyboardState;
        GamePadState _oldGamePadState;        

        public TextMenuComponent(Game game, SpriteFont normalFont, SpriteFont selectedFont)
            : base(game)
        {
            this._regularFont = normalFont;
            this._selectedFont = selectedFont;
            this._menuItems = new List<string>();
            this._spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            this._audio = (AudioLibrary)Game.Services.GetService(typeof(AudioLibrary));
            this._oldKeyboardState = Keyboard.GetState();
            this._oldGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public int Width
        {
            get { return this._width; }
        }

        public int Height
        {
            get { return this._height; }
        }

        public int SelectedIndex
        {
            get { return this._selectedIndex; }
            set { this._selectedIndex = value; }
        }

        public Color RegularColor
        {
            get { return this._regularColor; }
            set { this._regularColor = value; }
        }

        public Color SelectedColor
        {
            get { return this._selectedColor; }
            set { this._selectedColor = value; }
        }

        public Vector2 Position
        {
            get { return this._position; }
            set { this._position = value; }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            // Handle the keyboard
            bool down = this._oldKeyboardState.IsKeyDown(Keys.Down) &&
                         keyboardState.IsKeyUp(Keys.Down);
            bool up = this._oldKeyboardState.IsKeyDown(Keys.Up) &&
                      keyboardState.IsKeyUp(Keys.Up);

            // Handle the gamepad
            down |= (this._oldGamePadState.DPad.Down == ButtonState.Pressed) &&
                    (gamePadState.DPad.Down == ButtonState.Released);
            up |= (this._oldGamePadState.DPad.Up == ButtonState.Pressed) &&
                  (gamePadState.DPad.Up == ButtonState.Released);

            if (down || up)
            {
                this._audio.MenuScroll.Play();
            }

            if (down)
            {
                this._selectedIndex++;
                if (this._selectedIndex == this._menuItems.Count)
                {
                    this._selectedIndex = 0;
                }
            }

            if (up)
            {
                this._selectedIndex--;
                if (this._selectedIndex == -1)
                {
                    this._selectedIndex = this._menuItems.Count - 1;
                }
            }

            this._oldKeyboardState = keyboardState;
            this._oldGamePadState = gamePadState;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            float y = this._position.Y;

            for (int i = 0; i < this._menuItems.Count; i++)
            {
                SpriteFont font;
                Color theColor;

                if (i == this._selectedIndex)
                {
                    font = this._selectedFont;
                    theColor = this._selectedColor;
                }
                else
                {
                    font = this._regularFont;
                    theColor = this._regularColor;
                }

                // Draw the text shadow
                this._spriteBatch.DrawString(font, this._menuItems[i], new Vector2(this._position.X + 1, y + 1), Color.Black);

                // Draw the text item
                this._spriteBatch.DrawString(font, this._menuItems[i], new Vector2(this._position.X, y), theColor);
                y += font.LineSpacing;
            }

            base.Draw(gameTime);
        }

        public void SetMenuItems(params string[] items)
        {
            this._menuItems.Clear();
            this._menuItems.AddRange(items);
            CalculateBounds();
        }

        public void CalculateBounds()
        {
            this._width = 0;
            this._height = 0;

            foreach (string menuItem in this._menuItems)
            {
                Vector2 size = this._selectedFont.MeasureString(menuItem);

                if (size.X > this._width)
                {
                    this._width = (int)size.X;
                }

                this._height += this._selectedFont.LineSpacing;
            }
        }
    }
}
