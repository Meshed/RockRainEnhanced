using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RockRainEnhanced.ControllerStrategy;
using System.Collections.Generic;
using System.Linq;
using RockRainEnhanced.Extensions;

namespace RockRainEnhanced
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Player : DrawableGameComponent
    {
        readonly Texture2D _texture;
        Rectangle _spriteRectangle;
        Vector2 _position;
        TimeSpan _elapsedTime = TimeSpan.Zero;
        
        int _score;
        const int InitialPower = 100;
        const int Velocity = 5;
        IEnumerable<IController> _controllers;
        Vector2 _initialPosition;

        public Player(Game game, ref Texture2D theTexture, Vector2 initialPosition, Rectangle rectangle, params IController[] controllers)
            : base(game)
        {
            _texture = theTexture;
            _initialPosition = initialPosition;
            _position = initialPosition;
            _controllers = controllers;
            KeepInBound();
            _spriteRectangle = rectangle;
            var screenBounds = game.GetScreenBounds();
            if (screenBounds.Width == 0)
                throw new ArgumentOutOfRangeException("Screen Bounds");
            if (_spriteRectangle.Width < 1)
                throw new ArgumentOutOfRangeException("Player sprite");
            if (screenBounds.Width < _spriteRectangle.Width)
                throw new ArgumentOutOfRangeException("ScreenBounds or PlayerSprite");

        }

        public void Reset()
        {
           
            _position = _initialPosition;
            _position.Y = this.Game.GetScreenBounds().Height - _spriteRectangle.Height;
            _score = 0;
            Power = InitialPower;
            KeepInBound();
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
            
            _position.Y += (_controllers.MaxBy(c=>Math.Abs(c.YThrottle)).YThrottle *3)*-2;
            _position.X += (_controllers.MaxBy(c=>Math.Abs(c.XThrottle)).XThrottle *3)*2;

           

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
            var screenBounds=this.Game.GetScreenBounds();
            if (screenBounds.Width < _spriteRectangle.Width)
                throw new InvalidOperationException("Screen or player width");
            if (_position.X < screenBounds.Left)
            {
                _position.X = screenBounds.Left;
            }
            if (_position.X > screenBounds.Width - _spriteRectangle.Width)
            {
                _position.X = screenBounds.Width - _spriteRectangle.Width;
            }
            if (_position.Y < screenBounds.Top)
            {
                _position.Y = screenBounds.Top;
            }
            if (_position.Y > screenBounds.Height - _spriteRectangle.Height)
            {
                _position.Y = screenBounds.Height - _spriteRectangle.Height;
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
