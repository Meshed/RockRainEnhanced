using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RockRainEnhanced
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Player : DrawableGameComponent
    {
        private readonly Texture2D _texture;
        private Rectangle _spriteRectangle;
        private Vector2 _position;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private readonly PlayerIndex _playerIndex;
        private Rectangle _screenBounds;
        private int _score;
        private const int InitialPower = 100;
        private const int Velocity = 5;

        public Player(Game game, ref Texture2D theTexture, PlayerIndex playerID, Rectangle rectangle)
            : base(game)
        {
            _texture = theTexture;
            _position = new Vector2();
            _playerIndex = playerID;

            _spriteRectangle = rectangle;
#if XBOX360
            screenBounds = new Rectangle((int) (Game.Window.ClientBounds.Width*
                                                0.03f), (int) (Game.Window.ClientBounds.Height*0.03f),
                                         Game.Window.ClientBounds.Width -
                                         (int) (Game.Window.ClientBounds.Width*0.03f),
                                         Game.Window.ClientBounds.Height -
                                         (int) (Game.Window.ClientBounds.Height*0.03f));
#else
            _screenBounds = new Rectangle(0, 0, Game.Window.ClientBounds.Width,
                                         Game.Window.ClientBounds.Height);
#endif
        }

        public void Reset()
        {
            if (_playerIndex == PlayerIndex.One)
            {
                _position.X = _screenBounds.Width/3;
            }
            else
            {
                _position.X = (int) (_screenBounds.Width/1.5);
            }

            _position.Y = _screenBounds.Height - _spriteRectangle.Height;
            _score = 0;
            Power = InitialPower;
        }

        public int Score
        {
            get { return _score; }
            set {
                _score = value < 0 ? 0 : value;
            }
        }

        public int Power { get; set; }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            GamePadState gamepadstatus = GamePad.GetState(_playerIndex);
            _position.Y += (int) ((gamepadstatus.ThumbSticks.Left.Y*3)*-2);
            _position.X += (int) ((gamepadstatus.ThumbSticks.Left.X*3)*2);

            if (_playerIndex == PlayerIndex.One)
            {
                HandlePlayer1KeyBoard();
            }
            else
            {
                HandlePlayer2KeyBoard();
            }

            KeepInBound();

            // Update score
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _score++;
                Power--;
            }

            base.Update(gameTime);
        }

        private void KeepInBound()
        {
            if (_position.X < _screenBounds.Left)
            {
                _position.X = _screenBounds.Left;
            }
            if (_position.X > _screenBounds.Width - _spriteRectangle.Width)
            {
                _position.X = _screenBounds.Width - _spriteRectangle.Width;
            }
            if (_position.Y < _screenBounds.Top)
            {
                _position.Y = _screenBounds.Top;
            }
            if (_position.Y > _screenBounds.Height - _spriteRectangle.Height)
            {
                _position.Y = _screenBounds.Height - _spriteRectangle.Height;
            }
        }

        private void HandlePlayer1KeyBoard()
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Up))
            {
                _position.Y -= Velocity;
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                _position.Y += Velocity;
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                _position.X -= Velocity;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                _position.X += Velocity;
            }
        }

        private void HandlePlayer2KeyBoard()
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.W))
            {
                _position.Y -= Velocity;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                _position.Y += Velocity;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                _position.X -= Velocity;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                _position.X += Velocity;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var sBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));

            sBatch.Draw(_texture, _position, _spriteRectangle, Color.White);

            base.Draw(gameTime);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int) _position.X, (int) _position.Y, _spriteRectangle.Width, _spriteRectangle.Height);
        }
    }
}
