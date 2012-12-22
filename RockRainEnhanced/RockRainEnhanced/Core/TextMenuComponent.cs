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


namespace RockRainEnhanced.Core
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TextMenuComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch spriteBatch = null;
        protected readonly SpriteFont regularFont, selectedFont;
        protected Color regularColor = Color.White, selectedColor = Color.Red;
        protected Vector2 position = new Vector2();
        protected int selectedIndex = 0;
        private readonly List<String> menuItems;
        protected int width, height;
        protected KeyboardState oldKeyboardState;
        protected GamePadState oldGamePadState;
        protected AudioLibrary audio;

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; }
        }
        public Color RegularColor
        {
            get { return regularColor; }
            set { regularColor = value; }
        }
        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public TextMenuComponent(Game game, SpriteFont normalFont, SpriteFont selectedFont)
            : base(game)
        {
            regularFont = normalFont;
            this.selectedFont = selectedFont;
            menuItems = new List<string>();
            spriteBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));
            audio = (AudioLibrary) Game.Services.GetService(typeof (AudioLibrary));
            oldKeyboardState = Keyboard.GetState();
            oldGamePadState = GamePad.GetState(PlayerIndex.One);
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
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            bool down, up;

            // Handle the keyboard
            down = (oldKeyboardState.IsKeyDown(Keys.Down) &&
                    (keyboardState.IsKeyUp(Keys.Down)));
            up = (oldKeyboardState.IsKeyDown(Keys.Up) &&
                  (keyboardState.IsKeyUp(Keys.Up)));

            // Handle the gamepad
            down |= (oldGamePadState.DPad.Down == ButtonState.Pressed) &&
                    (gamePadState.DPad.Down == ButtonState.Released);
            up |= (oldGamePadState.DPad.Up == ButtonState.Pressed) &&
                  (gamePadState.DPad.Up == ButtonState.Released);

            if (down || up)
            {
                audio.MenuScroll.Play();
            }

            if (down)
            {
                selectedIndex++;
                if (selectedIndex == menuItems.Count)
                {
                    selectedIndex = 0;
                }
            }
            if (up)
            {
                selectedIndex--;
                if (selectedIndex == -1)
                {
                    selectedIndex = menuItems.Count - 1;
                }
            }

            oldKeyboardState = keyboardState;
            oldGamePadState = gamePadState;

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            float y = position.Y;

            for (int i = 0; i < menuItems.Count; i++)
            {
                SpriteFont font;
                Color theColor;

                if (i == selectedIndex)
                {
                    font = selectedFont;
                    theColor = selectedColor;
                }
                else
                {
                    font = regularFont;
                    theColor = regularColor;
                }

                // Draw the text shadow
                spriteBatch.DrawString(font, menuItems[i], new Vector2(position.X + 1, y + 1), Color.Black);
                // Draw the text item
                spriteBatch.DrawString(font, menuItems[i], new Vector2(position.X, y), theColor);
                y += font.LineSpacing;
            }

            base.Draw(gameTime);
        }

        public void SetMenuItems(string[] items)
        {
            menuItems.Clear();
            menuItems.AddRange(items);
            CalculateBounds();
        }
        public void CalculateBounds()
        {
            width = 0;
            height = 0;

            foreach (string menuItem in menuItems)
            {
                Vector2 size = selectedFont.MeasureString(menuItem);

                if (size.X > width)
                {
                    width = (int) size.X;
                }

                height += selectedFont.LineSpacing;
            }
        }
    }
}
