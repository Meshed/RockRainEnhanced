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
        private readonly SimpleRumblePad _rumblePad;
        private readonly ImageComponent _background;
        private readonly Score _score_player1;
        private readonly Score _score_player2;
        private Vector2 _pausePosition;
        private Vector2 _gameoverPosition;
        private Rectangle _pauseRect = new Rectangle(1, 120, 200, 44);
        private Rectangle _gameoverRect = new Rectangle(1, 170, 350, 48);
#if DEBUG
        TextComponent positionDebugText;
#endif
        private bool _paused;
        private bool _gameOver;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private bool _twoPlayers;
        protected Vector2 pausePosition;
        protected Vector2 gameoverPosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Rectangle gameoverRect = new Rectangle(1, 170, 350, 48);

        Game1 game1;
        Texture2D actionElementsTexture;
        Texture2D actionBackgroundTexture;
        SpriteFont scoreFont;
        IController[] controllers;
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
          
            
            _score_player1 = new Score(game, font, Color.Blue);
            _score_player1.Position = new Vector2(10, 10);
            Components.Add(_score_player1);
           

            _rumblePad = new SimpleRumblePad(game);
            Components.Add(_rumblePad);

            _powerSource = new PowerSource(game, ref _actionTexture);
            _powerSource.Initialize();
            Components.Add(_powerSource);
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

            _score_player2 = new Score(game, font, Color.Red);
            _score_player2.Position = new Vector2(Game.Window.ClientBounds.Width - 200, 10);
            Components.Add(_score_player2);
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

            _player1.Reset();
            if (_twoPlayers)
            {
                
                _player2.Reset();
                _player2.Visible = _twoPlayers;
                _player2.Enabled = _twoPlayers;
                _score_player2.Visible = _twoPlayers;
                _score_player2.Enabled = _twoPlayers;
                _score_player2.Visible = _twoPlayers;
                _score_player2.Enabled = _twoPlayers;
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

                // Update score
                _score_player1.Value = _player1.Score;
                _score_player1.Power = _player1.Power;
                if (_twoPlayers)
                {
                    _score_player2.Value = _player2.Score;
                    _score_player2.Power = _player2.Power;
                }

                // Check if player is dead
                _gameOver = ((_player1.Power <= 0) || (_player2 != null && _player2.Power <= 0));
                if (_gameOver)
                {
                    _player1.Visible = (_player1.Power > 0);
                    if (_twoPlayers)
                    {
                        _player2.Visible = (_player2.Power > 0);
                    }
                    MediaPlayer.Stop();
                }

                base.Update(gameTime);
            }

            if (_gameOver)
            {
                _meteors.Update(gameTime);
            }
            positionDebugText.Text =( _player2 ?? _player1).GetBounds().ToString();
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
            var q = from m1 in _meteors.AllMeteors
                    from m2 in _meteors.AllMeteors
                    where m1 != m2
                    where m1.CheckCollision(m2)
                    select new { m1, m2 };
            foreach (var m in q.ToArray())
            {
                _meteors.AllMeteors.Remove(m.m1);
                _meteors.AllMeteors.Remove(m.m2);
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
