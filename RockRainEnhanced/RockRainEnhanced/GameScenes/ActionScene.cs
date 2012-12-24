using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using RockRain;
using RockRainEnhanced.Core;
using RockRainEnhanced.ControllerStrategy;

namespace RockRainEnhanced.GameScenes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ActionScene : GameScene
    {
        private readonly Texture2D _actionTexture;
        private readonly AudioLibrary _audio;
        private readonly SpriteBatch _spriteBatch;
        private readonly Player _player1;
        private readonly Player _player2;
        private readonly MeteorsManager _meteors;
        private readonly PowerSource _powerSource;
        private readonly Wrench _wrench;
        private readonly SimpleRumblePad _rumblePad;
        private readonly ImageComponent _background;
        private readonly Score _scorePlayer1;
        private readonly Score _scorePlayer2;
        private Vector2 _pausePosition;
        private Vector2 _gameoverPosition;
        private Rectangle _pauseRect = new Rectangle(1, 120, 200, 44);
        private Rectangle _gameoverRect = new Rectangle(1, 170, 350, 48);
        private readonly Color _criticalFontColor = Color.Red;
        private readonly Color _player1FontColor = Color.Blue;
        private readonly Color _player2FontColor = Color.Green;
        private bool _paused;
        private bool _gameOver;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private TimeSpan _wrenchElapsedTime = TimeSpan.Zero;
        private bool _twoPlayers;
        Game1 game1;
        Texture2D actionElementsTexture;
        Texture2D actionBackgroundTexture;
        SpriteFont scoreFont;
        IController[] controllers;

#if DEBUG
        TextComponent positionDebugText;
#endif

        ActionScene(Game game, Texture2D theTexture, Texture2D backgroundTexture, SpriteFont font):base(game)
        {
            _audio = (AudioLibrary)Game.Services.GetService(typeof(AudioLibrary));
            _background = new ImageComponent(game, backgroundTexture, ImageComponent.DrawMode.Stretch);
            Components.Add(_background);

            _actionTexture = theTexture;

            _spriteBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));
            _meteors = new MeteorsManager(Game, ref _actionTexture);
            Components.Add(_meteors);


            // TODO: Complete member initialization
            this.game1 = game1;
            this.actionElementsTexture = actionElementsTexture;
            this.actionBackgroundTexture = actionBackgroundTexture;
            this.scoreFont = font;
            
            _scorePlayer1 = new Score(game, font, _player1FontColor);
            _scorePlayer1.Position = new Vector2(10, 10);
            Components.Add(_scorePlayer1);
           

            _rumblePad = new SimpleRumblePad(game);
            Components.Add(_rumblePad);

            _powerSource = new PowerSource(game, ref _actionTexture);
            _powerSource.Initialize();
            Components.Add(_powerSource);

            _wrench = new Wrench(game, game.Content.Load<Texture2D>("wrench"));
            _wrench.Initialize();
            Components.Add(_wrench);

#if DEBUG
            positionDebugText=new TextComponent(game,this.scoreFont,new Vector2(),Color.Red);
            Components.Add(positionDebugText);            
#endif
        }
        public ActionScene(Game game, Texture2D theTexture, Texture2D backgroundTexture, SpriteFont font,Rectangle screenBounds, params IController[] controllers)
            :this(game,theTexture,backgroundTexture,font)
        {
            this.TwoPlayers = false;
            _player1 = new Player(Game, ref _actionTexture,  new Vector2(screenBounds.Width / 3, 0), new Rectangle(323, 15, 30, 30),controllers);
            _player1.Initialize();
            Components.Add(_player1);   
        }
        public ActionScene(IController playerOneController,IController playerTwoController,Game game, Texture2D theTexture, Texture2D backgroundTexture,Rectangle screenBounds, SpriteFont font)
            : this(game, theTexture, backgroundTexture, font)
        {
            
            this.TwoPlayers = true;
            _player1 = new Player(Game, ref _actionTexture,new Vector2(screenBounds.Width / 3, 0), new Rectangle(323, 15, 30, 30),playerOneController);
            _player1.Initialize();
            Components.Add(_player1);

            _scorePlayer2 = new Score(game, font, _player2FontColor);
            _scorePlayer2.Position = new Vector2(Game.Window.ClientBounds.Width - 200, 10);
            Components.Add(_scorePlayer2);
            _player2 = new Player(Game, ref _actionTexture, new Vector2((int)(screenBounds.Width / 1.5), 0), new Rectangle(360, 17, 30, 30), playerTwoController);
            _player2.Initialize();
            Components.Add(_player2);
        }

      

        public bool TwoPlayers
        {
            get { return _twoPlayers; }
            set { _twoPlayers = value; }
        }

        public bool GameOver
        {
            get { return _gameOver; }
        }

        public bool Paused
        {
            get { return _paused; }
            set
            {
                _paused = value;
                if (_paused)
                {
                    MediaPlayer.Pause();
                }
                else
                {
                    MediaPlayer.Resume();
                }
            }
        }

        public override void Show()
        {
            
            MediaPlayer.Play(_audio.BackMusic);
            _meteors.Initialize();
            _powerSource.PutinStartPosition();
            _wrench.PutinStartPosition();

            _player1.Reset();
            if (_twoPlayers)
            {
                
                _player2.Reset();
                _player2.Visible = _twoPlayers;
                _player2.Enabled = _twoPlayers;
                _scorePlayer2.Visible = _twoPlayers;
                _scorePlayer2.Enabled = _twoPlayers;
                _scorePlayer2.Visible = _twoPlayers;
                _scorePlayer2.Enabled = _twoPlayers;
            }
            
            _paused = false;
            _pausePosition.X = (Game.Window.ClientBounds.Width - _pauseRect.Width)/2;
            _pausePosition.Y = (Game.Window.ClientBounds.Height - _pauseRect.Height)/2;

            _gameOver = false;
            _gameoverPosition.X = (Game.Window.ClientBounds.Width - _gameoverRect.Width)/2;
            _gameoverPosition.Y = (Game.Window.ClientBounds.Height - _gameoverRect.Height)/2;

          
          

            base.Show();
        }

        public override void Hide()
        {
            MediaPlayer.Stop();

            base.Hide();
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
            
            if ((!_paused) && (!_gameOver))
            {
                HandleDamages();

                HandlePowerSourceSprite(gameTime);
                HandleWrenchSprite(gameTime);

                // Update score
                _scorePlayer1.Value = _player1.Score;
                _scorePlayer1.Power = _player1.Power;
                _scorePlayer1.PowerFontColor = _scorePlayer1.Power <= 30 ? _criticalFontColor : _player1FontColor;

                if (_twoPlayers)
                {
                    _scorePlayer2.Value = _player2.Score;
                    _scorePlayer2.Power = _player2.Power;
                    _scorePlayer2.PowerFontColor = _scorePlayer2.Power <= 30 ? _criticalFontColor : _player2FontColor;
                }

                // Check if player is dead
                _gameOver = ((_player1.Power <= 0) || (_player2 != null && _player2.Power <= 0));
                if (_gameOver)
                {
                    _player1.Visible = (_player1.Power > 0);
                    if (_twoPlayers)
                    {
                        if (_player2 != null) _player2.Visible = (_player2.Power > 0);
                    }
                    MediaPlayer.Stop();
                }

                base.Update(gameTime);
            }

            if (_gameOver)
            {
                _meteors.Update(gameTime);
            }
#if DEBUG
            positionDebugText.Text =( _player2 ?? _player1).GetBounds().ToString();
#endif
        }

        private void HandleDamages()
        {
            // Check collision for Player 1
            if (_meteors.CheckForCollisions(_player1.GetBounds()))
            {
                _player1.Power -= 10;
                _player1.PowerLossPerSecond++;
                _player1.Score -= 1;
            }

            // Check collisions for Player 2
            if (_twoPlayers)
            {
                if (_meteors.CheckForCollisions(_player2.GetBounds()))
                {
                    _player2.Power -= 10;
                    _player2.PowerLossPerSecond++;
                    _player2.Score -= 1;
                }

                // Check for collision between players
                if (_player1.GetBounds().Intersects(_player2.GetBounds()))
                {
                    if (_player1.Position.X < _player2.Position.X)
                    {
                        _player1.StopLeft(_player2.Position.X);
                        _player2.StopRight(_player1.Position.X);
                    }
                    else
                    {
                        _player1.StopRight(_player2.Position.X);
                        _player2.StopLeft(_player1.Position.X);
                    }

                    if (_player1.Position.Y < _player2.Position.Y)
                    {
                        _player1.StopDown(_player2.Position.Y);
                        _player2.StopUp(_player1.Position.Y);
                    }
                    else
                    {
                        _player1.StopUp(_player2.Position.Y);
                        _player2.StopDown(_player1.Position.Y);
                    }
                }
            }
        }

        private void HandlePowerSourceSprite(GameTime gameTime)
        {
            if (_powerSource.CheckCollision(_player1.GetBounds()))
            {
                _audio.PowerGet.Play();
                _elapsedTime = TimeSpan.Zero;
                _powerSource.PutinStartPosition();
                _player1.Power += _powerSource.EffectValue;

                if (_player1.Power > 100)
                {
                    _player1.Power = 100;
                }

                if (_player1.Power < 0)
                {
                    _player1.Power = 0;
                }
            }

            if (_twoPlayers)
            {
                if (_powerSource.CheckCollision(_player2.GetBounds()))
                {
                    _audio.PowerGet.Play();
                    _powerSource.PutinStartPosition();
                    _player2.Power += _powerSource.EffectValue;

                    if (_player2.Power > 100)
                    {
                        _player2.Power = 100;
                    }

                    if (_player2.Power < 0)
                    {
                        _player2.Power = 0;
                    }
                }
            }

            // Check for sending a new power source
            _elapsedTime += gameTime.ElapsedGameTime;
            if (_elapsedTime > TimeSpan.FromSeconds(10))
            {
                _elapsedTime -= TimeSpan.FromSeconds(10);
                _powerSource.Enabled = true;
            }
        }
        private void HandleWrenchSprite(GameTime gameTime)
        {
            if (_wrench.CheckCollision(_player1.GetBounds()))
            {
                _audio.PowerGet.Play();
                _wrenchElapsedTime = TimeSpan.Zero;
                _wrench.PutinStartPosition();
                _player1.PowerLossPerSecond -= _wrench.EffectValue;
                if (_player1.PowerLossPerSecond < 1)
                {
                    _player1.PowerLossPerSecond = 1;
                }

                if (_player1.Power > 100)
                {
                    _player1.Power = 100;
                }

                if (_player1.Power < 0)
                {
                    _player1.Power = 0;
                }
            }

            if (_twoPlayers)
            {
                if (_wrench.CheckCollision(_player2.GetBounds()))
                {
                    _audio.PowerGet.Play();
                    _wrench.PutinStartPosition();
                    _player2.PowerLossPerSecond -= _wrench.EffectValue;
                    if (_player2.PowerLossPerSecond < 1)
                    {
                        _player2.PowerLossPerSecond = 1;
                    }

                    if (_player2.Power > 100)
                    {
                        _player2.Power = 100;
                    }

                    if (_player2.Power < 0)
                    {
                        _player2.Power = 0;
                    }
                }
            }

            // Check for sending a new wrench
            _wrenchElapsedTime += gameTime.ElapsedGameTime;
            if (_wrenchElapsedTime > TimeSpan.FromSeconds(15))
            {
                _wrenchElapsedTime -= TimeSpan.FromSeconds(15);
                _wrench.Enabled = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (this.Components.Contains(this._player1) == false && this.Components.Contains(this._player2) == false)
                throw new InvalidOperationException("No players found");
            if (this.Components.OfType<Player>().Select(p => p.DrawOrder).Any(dOrder =>
                this.Components.OfType<DrawableGameComponent>()
                    .Except(this.Components.OfType<Player>())
                    .Select(c => c.DrawOrder)
                    .All(c => dOrder > c)))
            {
                throw new InvalidOperationException("Players are not top component");
            }

            if (_paused)
            {
                _spriteBatch.Draw(_actionTexture, _pausePosition, _pauseRect, Color.White);
            }

            if (_gameOver)
            {
                _spriteBatch.Draw(_actionTexture, _gameoverPosition, _gameoverRect, Color.White);
            }
        }
    }
}
