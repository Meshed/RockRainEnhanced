using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RockRainEnhanced.GameScenes;
using RockRainEnhanced.ControllerStrategy;
using RockRainEnhanced.Extensions;

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
        HelpScene _helpScene;
        StartScene _startScene;
        ActionScene _actionScene;
        JoinScene _joinScene;
        GameScene _activeScene;

        // Textures
        protected Texture2D helpBackgroundTexture, helpForegroundTexture;
        protected Texture2D startBackgroundTexture, startElementsTexture;
        protected Texture2D actionElementsTexture, actionBackgroundTexture;

        // Fonts
        SpriteFont smallFont, largeFont, scoreFont;

        protected KeyboardState oldKeyboardState;
        //protected GamePadState oldGamePadState;

        HashSet<IController> menuControllers = new HashSet<IController>()
        {          
            new ControllerStrategy.WasdController(PlayerIndex.One),
            new ControllerStrategy.XboxController(PlayerIndex.One),
            new ControllerStrategy.ArrowController(PlayerIndex.One),
            new ControllerStrategy.XboxController(PlayerIndex.Two),
        };

        IDictionary<PlayerIndex, IController> _playerController = new Dictionary<PlayerIndex, IController>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _audio = new AudioLibrary();
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
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), _spriteBatch);

            _audio = new AudioLibrary();
            _audio.LoadContent(this.Content);
            Services.AddService(typeof(AudioLibrary), _audio);

            helpBackgroundTexture = Content.Load<Texture2D>("helpbackground");
            helpForegroundTexture = Content.Load<Texture2D>("helpforeground");
            _helpScene = new HelpScene(this, helpBackgroundTexture, helpForegroundTexture);
            Components.Add(_helpScene);

            // Create the Start scene
            smallFont = Content.Load<SpriteFont>("menuSmall");
            largeFont = Content.Load<SpriteFont>("menuLarge");
            startBackgroundTexture = Content.Load<Texture2D>("startBackground");
            startElementsTexture = Content.Load<Texture2D>("startSceneElements");
            _startScene = new StartScene(this, smallFont, largeFont, startBackgroundTexture, startElementsTexture);
            Components.Add(_startScene);

            actionElementsTexture = Content.Load<Texture2D>("rockrainenhanced");
            actionBackgroundTexture = Content.Load<Texture2D>("spacebackground");
            scoreFont = Content.Load<SpriteFont>("score");
            scoreFont = Content.Load<SpriteFont>("score");
            //_actionScene = new ActionScene(this, actionElementsTexture, actionBackgroundTexture, _scoreFont);
            //Components.Add(_actionScene);

            _joinScene = new JoinScene(this, largeFont, menuControllers);
            Components.Add(_joinScene);
            _startScene.Show();
            _activeScene = _startScene;
        }
        ActionScene SetUpActionScene2(IController controllerOne, IController controllerTwo)
        {
            Components.OfType<ActionScene>().ToList().ForEach(a => { Components.Remove(a); a.Dispose(); });
            _actionScene = new ActionScene(controllerOne, controllerTwo,this, actionElementsTexture,
                actionBackgroundTexture, this.GetScreenBounds(),scoreFont);
            Components.Add(_actionScene);
            return _actionScene;
        }

        ActionScene SetupActionScene1(params IController[] controllers)
        {

            Components.OfType<ActionScene>().ToList().ForEach(a =>
            {
                Components.Remove(a);
                a.Dispose();
            });
            _actionScene = new ActionScene(this, actionElementsTexture, actionBackgroundTexture, scoreFont, this.GetScreenBounds(), controllers);
            return _actionScene;
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
            menuControllers.ToList().Distinct().ToList().ForEach(c => c.Update());
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
            if (_activeScene == _startScene)
            {
                HandleStartSceneInput();
            }
            else if (_activeScene == _helpScene)
            {
                HandleHelpSceneInput();

            }
            else if (_activeScene == _actionScene)
            {
                HandleActionInput();
            }
            else if (_activeScene == _joinScene)
            {
                if (_joinScene.BothConfirmed)
                {
                    _actionScene = SetUpActionScene2(_joinScene.PlayerOne, _joinScene.PlayerTwo);
                    ShowScene(_actionScene);
                }
            }
        }

        private void HandleHelpSceneInput()
        {
            if (menuControllers.Any(m => m.IsBack || m.IsEnter))
                ShowScene(_startScene);
        }


        private void HandleStartSceneInput()
        {
            if (menuControllers.Any(m => m.IsEnter))
            {

                _audio.MenuSelect.Play();

                switch (_startScene.SelectedMenuIndex)
                {
                    case 0:
                        _actionScene = SetupActionScene1(menuControllers.ToArray());
                        this.Components.Add(_actionScene);
                        ShowScene(_actionScene); //skip join, use all controllers
                        break;
                    case 1:
                        ShowScene(_joinScene);
                        break;
                    case 2:
                        ShowScene(_helpScene);
                        break;
                    case 3:
                        Exit();
                        break;
                }
            }
        }

        private void HandleActionInput()
        {
            
            if ((!_actionScene.TwoPlayers && menuControllers.Any(m=>m.IsEnter )) || (_joinScene.PlayerOne != null && _joinScene.PlayerOne.IsEnter) || (_joinScene.PlayerTwo != null && _joinScene.PlayerTwo.IsEnter))
            {
                if (_actionScene.GameOver)
                {
                    ShowScene(_startScene);
                }
                else
                {
                    _audio.MenuBack.Play();
                    _actionScene.Paused = !_actionScene.Paused;
                }
            }

            if ((!_actionScene.TwoPlayers && menuControllers.Any(m=>m.IsBack)) || (_joinScene.PlayerOne!= null && _joinScene.PlayerOne.IsBack ) || (_joinScene.PlayerTwo != null && _joinScene.PlayerTwo.IsBack))
            {
                ShowScene(_startScene);
            }
        }

        protected void ShowScene(GameScene scene)
        {
            _activeScene.Hide();
            _activeScene = scene;
            scene.Show();
        }
    }
}
