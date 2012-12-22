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
using RockRainEnhanced.GameScenes;

namespace RockRainEnhanced
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        protected AudioLibrary audio;

        // Game scenes
        protected HelpScene helpScene;
        protected StartScene startScene;
        protected ActionScene actionScene;
        protected GameScene activeScene;

        // Textures
        protected Texture2D helpBackgroundTexture, helpForegroundTexture;
        protected Texture2D startBackgroundTexture, startElementsTexture;
        protected Texture2D actionElementsTexture, actionBackgroundTexture;

        // Fonts
        private SpriteFont smallFont, largeFont, scoreFont;

        protected KeyboardState oldKeyboardState;
        protected GamePadState oldGamePadState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);

            audio = new AudioLibrary();
            audio.LoadContent(this.Content);
            Services.AddService(typeof(AudioLibrary), audio);

            helpBackgroundTexture = Content.Load<Texture2D>("helpbackground");
            helpForegroundTexture = Content.Load<Texture2D>("helpforeground");
            helpScene = new HelpScene(this, helpBackgroundTexture, helpForegroundTexture);
            Components.Add(helpScene);

            // Create the Start scene
            smallFont = Content.Load<SpriteFont>("menuSmall");
            largeFont = Content.Load<SpriteFont>("menuLarge");
            startBackgroundTexture = Content.Load<Texture2D>("startBackground");
            startElementsTexture = Content.Load<Texture2D>("startSceneElements");
            startScene = new StartScene(this, smallFont, largeFont, startBackgroundTexture, startElementsTexture);
            Components.Add(startScene);

            actionElementsTexture = Content.Load<Texture2D>("rockrainenhanced");
            actionBackgroundTexture = Content.Load<Texture2D>("spacebackground");
            scoreFont = Content.Load<SpriteFont>("score");
            actionScene = new ActionScene(this, actionElementsTexture, actionBackgroundTexture, scoreFont);
            Components.Add(actionScene);

            startScene.Show();
            activeScene = startScene;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleScenesInput();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            base.Draw(gameTime);
            spriteBatch.End();
        }

        private void HandleScenesInput()
        {
            if (activeScene == startScene)
            {
                HandleStartSceneInput();
            }
            else if (activeScene == helpScene)
            {
                if (CheckEnterA())
                {
                    ShowScene(startScene);
                }
            }
            else if (activeScene == actionScene)
            {
                HandleActionInput();
            }
        }

        private bool CheckEnterA()
        {
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            bool result = (oldKeyboardState.IsKeyDown(Keys.Enter) && keyboardState.IsKeyUp(Keys.Enter));
            result |= (oldGamePadState.Buttons.A == ButtonState.Pressed) &&
                      (gamepadState.Buttons.A == ButtonState.Released);

            oldKeyboardState = keyboardState;
            oldGamePadState = gamepadState;

            return result;
        }

        private void HandleStartSceneInput()
        {
            if (CheckEnterA())
            {
                audio.MenuSelect.Play();

                switch (startScene.SelectedMenuIndex)
                {
                    case 0:
                        actionScene.TwoPlayers = false;
                        ShowScene(actionScene);
                        break;
                    case 1:
                        actionScene.TwoPlayers = true;
                        ShowScene(actionScene);
                        break;
                    case 2:
                        ShowScene(helpScene);
                        break;
                    case 3:
                        Exit();
                        break;
                }
            }
        }

        private void HandleActionInput()
        {
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            bool backKey = (oldKeyboardState.IsKeyDown(Keys.Escape) && (keyboardState.IsKeyUp(Keys.Escape)));
            backKey |= (oldGamePadState.Buttons.Back == ButtonState.Pressed) &&
                       (gamepadState.Buttons.Back == ButtonState.Released);

            bool enterKey = (oldKeyboardState.IsKeyDown(Keys.Enter) && (keyboardState.IsKeyUp(Keys.Enter)));
            enterKey |= (oldGamePadState.Buttons.A == ButtonState.Pressed) &&
                        (gamepadState.Buttons.A == ButtonState.Released);

            oldKeyboardState = keyboardState;
            oldGamePadState = gamepadState;

            if (enterKey)
            {
                if (actionScene.GameOver)
                {
                    ShowScene(startScene);
                }
                else
                {
                    audio.MenuBack.Play();
                    actionScene.Paused = !actionScene.Paused;
                }
            }

            if (backKey)
            {
                ShowScene(startScene);
            }
        }

        protected void ShowScene(GameScene scene)
        {
            activeScene.Hide();
            activeScene = scene;
            scene.Show();
        }
    }
}
