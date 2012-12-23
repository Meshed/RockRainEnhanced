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
using RockRain;
using RockRainEnhanced.Core;


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
        private readonly SimpleRumblePad _rumblePad;
        private readonly ImageComponent _background;
        private readonly Score _scorePlayer1;
        private readonly Score _scorePlayer2;

        private Vector2 _pausePosition;
        private Vector2 gameoverPosition;
        private Rectangle _pauseRect = new Rectangle(1, 120, 200, 44);
        private Rectangle gameoverRect = new Rectangle(1, 170, 350, 48);

        private bool _paused;
        private bool _gameOver;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private bool _twoPlayers;

        public ActionScene(Game game, Texture2D theTexture, Texture2D backgroundTexture, SpriteFont font)
            : base(game)
        {
            _audio = (AudioLibrary) Game.Services.GetService(typeof (AudioLibrary));
            _background = new ImageComponent(game, backgroundTexture, ImageComponent.DrawMode.Stretch);
            Components.Add(_background);

            _actionTexture = theTexture;

            _spriteBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));
            _meteors = new MeteorsManager(Game, ref _actionTexture);
            Components.Add(_meteors);

            _player1 = new Player(Game, ref _actionTexture, PlayerIndex.One, new Rectangle(323, 15, 30, 30));
            _player1.Initialize();
            Components.Add(_player1);

            _player2 = new Player(Game, ref _actionTexture, PlayerIndex.Two, new Rectangle(360, 17, 30, 30));
            _player2.Initialize();
            Components.Add(_player2);

            _scorePlayer1 = new Score(game, font, Color.Blue);
            _scorePlayer1.Position = new Vector2(10, 10);
            Components.Add(_scorePlayer1);
            _scorePlayer2 = new Score(game, font, Color.Red);
            _scorePlayer2.Position = new Vector2(Game.Window.ClientBounds.Width - 200, 10);
            Components.Add(_scorePlayer2);

            _rumblePad = new SimpleRumblePad(game);
            Components.Add(_rumblePad);

            _powerSource = new PowerSource(game, ref _actionTexture);
            _powerSource.Initialize();
            Components.Add(_powerSource);
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

            _player1.Reset();
            _player2.Reset();

            _paused = false;
            _pausePosition.X = (Game.Window.ClientBounds.Width - _pauseRect.Width)/2;
            _pausePosition.Y = (Game.Window.ClientBounds.Height - _pauseRect.Height)/2;

            _player2.Visible = _twoPlayers;
            _player2.Enabled = _twoPlayers;
            _scorePlayer2.Visible = _twoPlayers;
            _scorePlayer2.Enabled = _twoPlayers;

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

                // Update score
                _scorePlayer1.Value = _player1.Score;
                _scorePlayer1.Power = _player1.Power;
                if (_twoPlayers)
                {
                    _scorePlayer2.Value = _player2.Score;
                    _scorePlayer2.Power = _player2.Power;
                }

                // Check if player is dead
                _gameOver = ((_player1.Power <= 0) || (_player2.Power <= 0));
                if (_gameOver)
                {
                    _player1.Visible = (_player1.Power > 0);
                    _player2.Visible = (_player2.Power > 0) && _twoPlayers;
                    MediaPlayer.Stop();
                }

                base.Update(gameTime);
            }

            if (_gameOver)
            {
                _meteors.Update(gameTime);
            }
        }

        private void HandleDamages()
        {
            // Check collision for Player 1
            if (_meteors.CheckForCollisions(_player1.GetBounds()))
            {
                _player1.Power -= 10;
                _player1.Score -= 1;
            }

            // Check collisions for Player 2
            if (_twoPlayers)
            {
                if (_meteors.CheckForCollisions(_player2.GetBounds()))
                {
                    _player2.Power -= 10;
                    _player2.Score -= 10;
                }

                // Check for collision between players
                if (_player1.GetBounds().Intersects(_player2.GetBounds()))
                {
                    _player1.Power -= 10;
                    _player1.Score -= 10;
                    _player2.Power -= 10;
                    _player2.Score -= 10;
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
                _player1.Power += _powerSource.PowerValue;

                if (_player1.Power > 100)
                {
                    _player1.Power = 100;
                }
            }

            if (_twoPlayers)
            {
                if (_powerSource.CheckCollision(_player2.GetBounds()))
                {
                    _audio.PowerGet.Play();
                    _powerSource.PutinStartPosition();
                    _player2.Power += _powerSource.PowerValue;

                    if (_player2.Power > 100)
                    {
                        _player2.Power = 100;
                    }
                }
            }

            // Check for sending a new power source
            _elapsedTime += gameTime.ElapsedGameTime;
            if (_elapsedTime > TimeSpan.FromSeconds(15))
            {
                _elapsedTime -= TimeSpan.FromSeconds(15);
                _powerSource.Enabled = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_paused)
            {
                _spriteBatch.Draw(_actionTexture, _pausePosition, _pauseRect, Color.White);
            }

            if (_gameOver)
            {
                _spriteBatch.Draw(_actionTexture, gameoverPosition, gameoverRect, Color.White);
            }
        }
    }
}
