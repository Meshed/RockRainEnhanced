using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RockRainEnhanced.GameScenes;

namespace RockRainEnhanced
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        private AudioLibrary _audio;

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
        private SpriteFont _smallFont, _largeFont, _scoreFont;

        private KeyboardState _oldKeyboardState;
        private GamePadState _oldGamePadState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
#if DEBUG
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.IsFullScreen = false;
#else
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
#endif
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), _spriteBatch);

            _audio = new AudioLibrary();
            _audio.LoadContent(this.Content);
            Services.AddService(typeof(AudioLibrary), _audio);

            helpBackgroundTexture = Content.Load<Texture2D>("helpbackground");
            helpForegroundTexture = Content.Load<Texture2D>("helpforeground");
            helpScene = new HelpScene(this, helpBackgroundTexture, helpForegroundTexture);
            Components.Add(helpScene);

            // Create the Start scene
            _smallFont = Content.Load<SpriteFont>("menuSmall");
            _largeFont = Content.Load<SpriteFont>("menuLarge");
            startBackgroundTexture = Content.Load<Texture2D>("startBackground");
            startElementsTexture = Content.Load<Texture2D>("startSceneElements");
            startScene = new StartScene(this, _smallFont, _largeFont, startBackgroundTexture, startElementsTexture);
            Components.Add(startScene);

            actionElementsTexture = Content.Load<Texture2D>("rockrainenhanced");
            actionBackgroundTexture = Content.Load<Texture2D>("spacebackground");
            _scoreFont = Content.Load<SpriteFont>("score");
            actionScene = new ActionScene(this, actionElementsTexture, actionBackgroundTexture, _scoreFont);
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
            _spriteBatch.Begin();
            base.Draw(gameTime);
            _spriteBatch.End();
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

            bool result = (_oldKeyboardState.IsKeyDown(Keys.Enter) && keyboardState.IsKeyUp(Keys.Enter));
            result |= (_oldGamePadState.Buttons.A == ButtonState.Pressed) &&
                      (gamepadState.Buttons.A == ButtonState.Released);

            _oldKeyboardState = keyboardState;
            _oldGamePadState = gamepadState;

            return result;
        }

        private void HandleStartSceneInput()
        {
            if (CheckEnterA())
            {
                _audio.MenuSelect.Play();

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

            bool backKey = (_oldKeyboardState.IsKeyDown(Keys.Escape) && (keyboardState.IsKeyUp(Keys.Escape)));
            backKey |= (_oldGamePadState.Buttons.Back == ButtonState.Pressed) &&
                       (gamepadState.Buttons.Back == ButtonState.Released);

            bool enterKey = (_oldKeyboardState.IsKeyDown(Keys.Enter) && (keyboardState.IsKeyUp(Keys.Enter)));
            enterKey |= (_oldGamePadState.Buttons.A == ButtonState.Pressed) &&
                        (gamepadState.Buttons.A == ButtonState.Released);

            _oldKeyboardState = keyboardState;
            _oldGamePadState = gamepadState;

            if (enterKey)
            {
                if (actionScene.GameOver)
                {
                    ShowScene(startScene);
                }
                else
                {
                    _audio.MenuBack.Play();
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
