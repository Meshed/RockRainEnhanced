namespace RockRainEnhanced.GameScenes
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RockRainEnhanced.ControllerStrategy;
    using RockRainEnhanced.Core;

    /// <summary>
    /// Scene for players to join and/or select their controller
    /// </summary>
    public class JoinScene : GameScene
    {
        readonly IEnumerable<IController> _controllers;
        readonly TextComponent _playerOneStatus;
        readonly TextComponent _playerTwoStatus;
        IController _playerOne;
        IController _playerTwo;
        bool _oneConfirmed;
        bool _twoConfirmed;      

        public JoinScene(Game game, SpriteFont font, IEnumerable<IController> controllers)
            : base(game)
        {
            _controllers = controllers;
            _playerOneStatus = new TextComponent(game, font, new Vector2(x: Game.Window.ClientBounds.Width / 2, y: 330), Color.Blue)
                                   {
                                       Enabled = true,
                                       Visible = true,
                                       Text = "Press Start"
                                   };

            _playerTwoStatus = new Core.TextComponent(game, font, new Vector2(x: Game.Window.ClientBounds.Width / 2, y: 360), Color.Red)
                                   {
                                       Text = this._playerOneStatus.Text,
                                       Enabled = true,
                                       Visible = true
                                   };

            this.Components.Add(_playerOneStatus);
            this.Components.Add(_playerTwoStatus);
        }

        public IController PlayerOne { get { return _playerOne; } }

        public IController PlayerTwo { get { return _playerTwo; } }

        public bool BothConfirmed { get { return _oneConfirmed && _twoConfirmed; } }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateInput();
        }

        private void UpdateInput()
        {
            HandlePlayerInputs();
            if (_playerOne == null && _controllers.Any(c => c != _playerTwo && c.IsEnter))
            {
                _playerOne = _controllers.First(c => c.IsEnter);
                _playerOneStatus.Text = "Confirm " + _playerOne.GetType().Name;
            }
            else if (_playerTwo == null && _controllers.Any(c => c != _playerOne && c.IsEnter))
            {
                _playerTwo = _controllers.Where(c => c != _playerOne).First(c => c.IsEnter);
                _playerTwoStatus.Text = "Confirm " + _playerTwo.GetType().Name;
            }
        }

        private void HandlePlayerInputs()
        {
            if (_playerOne != null)
            {
                if (_playerOne.IsBack && !_oneConfirmed)
                {
                    _playerOne = null;
                    _playerOneStatus.Text = "Press Start";
                }
                else if (_playerOne.IsEnter)
                {
                    _oneConfirmed = true;
                    _playerOneStatus.Text = "Confirmed " + _playerOne.GetType().Name;
                }
            }

            if (_playerTwo != null)
            {
                if (_playerTwo.IsBack && !_twoConfirmed)
                {
                    _playerTwo = null;
                    _playerTwoStatus.Text = "Press Start";
                }
                else if (_playerTwo.IsEnter)
                {
                    _playerTwoStatus.Text = "Confirmed " + _playerTwo.GetType().Name;
                    _twoConfirmed = true;
                }
            }
        }
    }
}
