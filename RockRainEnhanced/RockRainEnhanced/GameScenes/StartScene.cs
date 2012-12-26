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
    public class StartScene : GameScene
    {
        TextMenuComponent menu;
        readonly Texture2D elements;
        AudioLibrary audio;
        SpriteBatch spriteBatch = null;
        Rectangle rockRect = new Rectangle(0, 0, 536, 131);
        Vector2 rockPosition;
        Rectangle rainRect = new Rectangle(120, 165, 517, 130);
        Vector2 rainPosition;
        Rectangle enhancedRect = new Rectangle(8, 304, 375, 144);
        Vector2 enhancedPosition;
        bool showEnchanced;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public StartScene(Game game, SpriteFont smallFont, SpriteFont largeFont, Texture2D background, Texture2D elements)
            : base(game)
        {
            this.elements = elements;
            Components.Add(new ImageComponent(game, background, ImageComponent.DrawMode.Center));

            string[] items = {"One Player", "Two Players", "Help", "Scores", "Quit"};
            menu = new TextMenuComponent(game, smallFont, largeFont);
            menu.SetMenuItems(items);
            Components.Add(menu);

            spriteBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));
            audio = (AudioLibrary) Game.Services.GetService(typeof (AudioLibrary));
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
            if (!menu.Visible)
            {
                if (rainPosition.X >= (Game.Window.ClientBounds.Width - 595) / 2)
                {
                    rainPosition.X -= 15;
                }
                if (rockPosition.X <= (Game.Window.ClientBounds.Width - 715) / 2)
                {
                    rockPosition.X += 15;
                }
                else
                {
                    menu.Visible = true;
                    menu.Enabled = true;

                    MediaPlayer.Play(audio.StartMusic);

#if XBOX360
                    enhancedPosition = new Vector2((rainPosition.X + rainRect.Width - enhancedRect.Width/2), rainPosition.Y);
#else
                    enhancedPosition = new Vector2((rainPosition.X + rainRect.Width - enhancedRect.Width / 2) - 80, rainPosition.Y);
#endif
                }
            }
            else
            {
                elapsedTime += gameTime.ElapsedGameTime;

                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    showEnchanced = !showEnchanced;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Draw(elements, rockPosition, rockRect, Color.White);
            spriteBatch.Draw(elements, rainPosition, rainRect, Color.White);

            if (showEnchanced)
            {
                spriteBatch.Draw(elements, enhancedPosition, enhancedRect, Color.White);
            }
        }

        public override void Show()
        {
            audio.NewMeteor.Play();
            rockPosition.X = -1*rockRect.Width;
            rockPosition.Y = 40;
            rainPosition.X = Game.Window.ClientBounds.Width;
            rainPosition.Y = 180;

            // Put the menu centered in screen
            menu.Position = new Vector2((Game.Window.ClientBounds.Width - menu.Width)/2, 330);

            menu.Visible = false;
            menu.Enabled = false;
            showEnchanced = false;

            base.Show();
        }

        public override void Hide()
        {
            MediaPlayer.Stop();
            base.Hide();
        }

        public int SelectedMenuIndex
        {
            get { return menu.SelectedIndex; }
        }
    }
}
