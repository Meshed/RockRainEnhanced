// -----------------------------------------------------------------------
// <copyright file="JoinScene.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RockRainEnhanced.GameScenes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using RockRainEnhanced.ControllerStrategy;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Scene for players to join and/or select their controller
    /// </summary>
    public class JoinScene : GameScene
    {
        IController _playerOne;
        IController _playerTwo;
        bool _oneConfirmed;
        bool _twoConfirmed;

        public IController PlayerOne { get { return _playerOne; } }
        public IController PlayerTwo { get { return _playerTwo; } }

        public bool BothConfirmed { get { return _oneConfirmed && _twoConfirmed; } }

        IEnumerable<IController> _controllers;
        readonly SpriteFont _font;
        private Core.TextComponent _playerOneStatus;
        private Core.TextComponent _playerTwoStatus;
        public JoinScene(Game game, SpriteFont font, IEnumerable<IController> controllers)
            : base(game)
        {
            _controllers = controllers;
            _font = font;
            _playerOneStatus = new Core.TextComponent(game, font, new Vector2((Game.Window.ClientBounds.Width) / 2, 330), Color.Blue);
            _playerOneStatus.Text = "Press Start";


            //_playerOneStatus.SetMenuItems("Press Start", "Confirm", "Confirmed");
            _playerTwoStatus = new Core.TextComponent(game, font, new Vector2((Game.Window.ClientBounds.Width) / 2, 360), Color.Red);
            //_playerTwoStatus.SetMenuItems("Press Start", "Confirm", "Confirmed");
            _playerTwoStatus.Text = _playerOneStatus.Text;
            _playerOneStatus.Enabled = true;
            _playerOneStatus.Visible = true;
            _playerTwoStatus.Enabled = true;
            _playerTwoStatus.Visible = true;
            this.Components.Add(_playerOneStatus);
            this.Components.Add(_playerTwoStatus);
        }
        public override void Update(GameTime gameTime)
        {
            
           
            base.Update(gameTime);
            UpdateInput();
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
           // DrawOne(gameTime);
        }

       
        private void UpdateInput()
        {
            HandlePlayerInputs();
            if (_playerOne == null && _controllers.Any(c =>c != _playerTwo && c.IsEnter))
            {
                _playerOne=_controllers.First(c => c.IsEnter);
                _playerOneStatus.Text = "Confirm " + _playerOne.GetType().Name;
            } else if (_playerTwo == null && _controllers.Any(c=>c != _playerOne && c.IsEnter))
            {
                _playerTwo=_controllers.Where(c=>c != _playerOne).First(c=> c.IsEnter);
                _playerTwoStatus.Text = "Confirm " +_playerTwo.GetType().Name;
            } 

        }

        private void HandlePlayerInputs()
        {
            if (_playerOne != null)
            {
                if( _playerOne.IsBack && !_oneConfirmed)
                {
                    _playerOne = null;
                    _playerOneStatus.Text = "Press Start";
                } else if (_playerOne.IsEnter) {
                    _oneConfirmed=true;
                    _playerOneStatus .Text= "Confirmed " +_playerOne.GetType().Name;
                }
            }
            if (_playerTwo != null )
            {
                if(_playerTwo.IsBack && !_twoConfirmed)
                {
                    _playerTwo = null;
                    _playerTwoStatus.Text = "Press Start";
                }
                else if (_playerTwo.IsEnter)
                {
                    _playerTwoStatus.Text = "Confirmed " + _playerTwo.GetType().Name;
                    _twoConfirmed=true;
                }
            }
        }
    }
}
