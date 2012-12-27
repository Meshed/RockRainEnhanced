namespace RockRainEnhanced
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RockRainEnhanced.ControllerStrategy;
    using RockRainEnhanced.Core;
    using RockRainEnhanced.Extensions;
    using RockRainEnhanced.GameScenes;
    
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;

        readonly Dictionary<int, string> _highScores = new Dictionary<int, string> { { 10, "Mas" }, { 11, "Bcn" } };

        readonly AudioLibrary _audio;

        readonly HashSet<IController> _menuControllers = new HashSet<IController>
                                                             {
                                                                 new WasdController(PlayerIndex.One),
                                                                 new XboxController(PlayerIndex.One),
                                                                 new ArrowController(PlayerIndex.One),
                                                                 new XboxController(PlayerIndex.Two),
                                                             };

        SpriteBatch _spriteBatch;

        // Game scenes
        HelpScene _helpScene;
        StartScene _startScene;
        ActionScene _actionScene;
        JoinScene _joinScene;
        GameScene _activeScene;
        ScoreScene _scoreScene;

        // Textures
        Texture2D _helpBackgroundTexture, _helpForegroundTexture;
        Texture2D _startBackgroundTexture, _startElementsTexture;
        Texture2D _actionElementsTexture, _actionBackgroundTexture;

        // Fonts
        SpriteFont _smallFont, _largeFont, _scoreFont;      

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _audio = new AudioLibrary();
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            // graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), _spriteBatch);

            _audio.LoadContent(this.Content);
            Services.AddService(typeof(AudioLibrary), _audio);

            this._helpBackgroundTexture = Content.Load<Texture2D>("helpbackground");
            this._helpForegroundTexture = Content.Load<Texture2D>("helpforeground");
            _helpScene = new HelpScene(this, this._helpBackgroundTexture, this._helpForegroundTexture);
            Components.Add(_helpScene);

            // Create the Start scene
            this._smallFont = Content.Load<SpriteFont>("menuSmall");
            this._largeFont = Content.Load<SpriteFont>("menuLarge");
            this._startBackgroundTexture = Content.Load<Texture2D>("startBackground");
            this._startElementsTexture = Content.Load<Texture2D>("startSceneElements");
            _startScene = new StartScene(this, this._smallFont, this._largeFont, this._startBackgroundTexture, this._startElementsTexture);
            Components.Add(_startScene);

            this._actionElementsTexture = Content.Load<Texture2D>("rockrainenhanced");
            this._actionBackgroundTexture = Content.Load<Texture2D>("spacebackground");
            this._scoreFont = Content.Load<SpriteFont>("score");

            _joinScene = new JoinScene(this, this._largeFont, this._menuControllers);
            Components.Add(_joinScene);
            _startScene.Show();
            _activeScene = _startScene;
            _scoreScene = new ScoreScene(this, this._largeFont, _highScores, _startBackgroundTexture);
            Components.Add(_scoreScene);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            this._menuControllers.ToList().Distinct().ToList().ForEach(c => c.Update());
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

        void HandleScenesInput()
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
            else if (_activeScene == _scoreScene)
            {
                HandleScoreSceneInput();
            }
        }

        void HandleScoreSceneInput()
        {
            if (this._menuControllers.Any(m => m.IsBack))
            {
                this.ShowScene(_startScene);
            }
        }

        ActionScene SetUpActionScene2(IController controllerOne, IController controllerTwo)
        {
            Components.OfType<ActionScene>().ToList().ForEach(
                a =>
                {
                    Components.Remove(a);
                    a.Dispose();
                });
            _actionScene = new ActionScene(
                controllerOne,
                controllerTwo,
                this,
                this._actionElementsTexture,
                this._actionBackgroundTexture,
                this.GetScreenBounds(),
                this._scoreFont,
                new Vector2(100, 100));
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
            _actionScene = new ActionScene(
                this,
                this._actionElementsTexture,
                this._actionBackgroundTexture,
                this._scoreFont,
                this.GetScreenBounds(),
                new Vector2(100, 100),
                controllers);
            return _actionScene;
        }

        void HandleHelpSceneInput()
        {
            if (this._menuControllers.Any(m => m.IsBack || m.IsEnter))
                ShowScene(_startScene);
        }

        void HandleStartSceneInput()
        {
            if (this._menuControllers.Any(m => m.IsEnter))
            {
                _audio.MenuSelect.Play();

                switch ((StartScene.MenuItems)_startScene.SelectedMenuIndex)
                {
                    case StartScene.MenuItems.OnePlayer:
                        _actionScene = SetupActionScene1(this._menuControllers.ToArray());
                        this.Components.Add(_actionScene);
                        ShowScene(_actionScene); // skip join, use all controllers
                        break;
                    case StartScene.MenuItems.TwoPlayers:
                        ShowScene(_joinScene);
                        break;
                    case StartScene.MenuItems.Help:
                        ShowScene(_helpScene);
                        break;
                    case StartScene.MenuItems.Scores: // scores
                        this.ShowScene(_scoreScene);
                        break;
                    case StartScene.MenuItems.Quit:
                        Exit();
                        break;
                }
            }
        }

        void HandleActionInput()
        {
            if ((!_actionScene.TwoPlayers && this._menuControllers.Any(m => m.IsEnter))
                || (_joinScene.PlayerOne != null && _joinScene.PlayerOne.IsEnter)
                || (_joinScene.PlayerTwo != null && _joinScene.PlayerTwo.IsEnter))
            {
                if (_actionScene.GameOver)
                {
                    ShowScene(_scoreScene);
                }
                else
                {
                    _audio.MenuBack.Play();
                    _actionScene.Paused = !_actionScene.Paused;
                }
            }

            if ((!_actionScene.TwoPlayers && this._menuControllers.Any(m => m.IsBack))
                || (_joinScene.PlayerOne != null && _joinScene.PlayerOne.IsBack)
                || (_joinScene.PlayerTwo != null && _joinScene.PlayerTwo.IsBack))
            {
                ShowScene(_startScene);
            }
        }

        void ShowScene(GameScene scene)
        {
            _activeScene.Hide();
            
            _activeScene = scene;
            scene.Show();
            Debug.WriteLine("Active scene is now " + scene.GetType().Name);
        }
    }
}
