using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using RockRainEnhanced.ControllerStrategy;
using RockRainEnhanced.Core;

namespace RockRainEnhanced.GameScenes
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ActionScene : GameScene
    {
        readonly Texture2D _actionTexture;

        readonly AudioLibrary _audio;

        readonly SpriteBatch _spriteBatch;

        readonly Player _player1;

        readonly Player _player2;

        readonly MeteorsManager _meteors;

        readonly PowerSource _powerSource;

        readonly SimpleRumblePad _rumblePad;

        readonly ImageComponent _background;

        readonly Score _scorePlayer1;
        readonly Score _scorePlayer2;
#if DEBUG
        readonly TextComponent _positionDebugText;
#endif
        readonly Rectangle _gameoverRect = new Rectangle(1, 170, 350, 48);
        readonly SpriteFont _scoreFont;
        readonly Vector2 _gameoverPosition;
        Rectangle _pauseRect = new Rectangle(1, 120, 200, 44);
        Vector2 _pausePosition;
        
        bool _paused;
        bool _gameOver;
        TimeSpan _elapsedTime = TimeSpan.Zero;
        bool _twoPlayers;
        

        public ActionScene(
            Game game,
            Texture2D theTexture,
            Texture2D backgroundTexture,
            SpriteFont font,
            Rectangle screenBounds,
            Vector2 gameoverPosition,
            params IController[] controllers)
            : this(game, theTexture, backgroundTexture, font, gameoverPosition)
        {
            this.TwoPlayers = false;
            this._player1 = new Player(
                Game,
                ref this._actionTexture,
                new Vector2(x: screenBounds.Width / 3, y: 0),
                new Rectangle(323, 15, 30, 30),
                controllers);
            this._player1.Initialize();
            Components.Add(this._player1);
        }

        public ActionScene(
            IController playerOneController,
            IController playerTwoController,
            Game game,
            Texture2D theTexture,
            Texture2D backgroundTexture,
            Rectangle screenBounds,
            SpriteFont font,
            Vector2 gameoverPosition)
            : this(game, theTexture, backgroundTexture, font, gameoverPosition)
        {
            this.TwoPlayers = true;
            this._player1 = new Player(Game, ref this._actionTexture, new Vector2(x: screenBounds.Width / 3, y: 0), new Rectangle(323, 15, 30, 30), playerOneController);
            this._player1.Initialize();
            Components.Add(this._player1);
            this._scorePlayer2 = new Score(game, font, Color.Red)
                               {
                                   Position =
                                       new Vector2(
                                       this.Game.Window.ClientBounds.Width - 200, 10)
                               };
            Components.Add(this._scorePlayer2);
            this._player2 = new Player(Game, ref this._actionTexture, new Vector2((int)(screenBounds.Width / 1.5), 0), new Rectangle(360, 17, 30, 30), playerTwoController);
            this._player2.Initialize();
            Components.Add(this._player2);
        }

        ActionScene(
            Game game, Texture2D theTexture, Texture2D backgroundTexture, SpriteFont font, Vector2 gameoverPosition)
            : base(game)
        {
            this._audio = (AudioLibrary)Game.Services.GetService(typeof(AudioLibrary));
            this._background = new ImageComponent(game, backgroundTexture, ImageComponent.DrawMode.Stretch);
            Components.Add(this._background);
            this._actionTexture = theTexture;
            this._spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            this._meteors = new MeteorsManager(Game, ref this._actionTexture);
            Components.Add(this._meteors);
            this._scoreFont = font;
            this._gameoverPosition = gameoverPosition;
            this._scorePlayer1 = new Score(game, font, Color.Blue) { Position = new Vector2(10, 10) };
            Components.Add(this._scorePlayer1);
           
            this._rumblePad = new SimpleRumblePad(game);
            Components.Add(this._rumblePad);
            this._powerSource = new PowerSource(game, ref this._actionTexture);
            Components.Add(this._powerSource);
#if DEBUG
            this._positionDebugText = new TextComponent(game, this._scoreFont, new Vector2(), Color.Red);
            Components.Add(this._positionDebugText);
#endif
        }
      
        public bool TwoPlayers
        {
            get { return this._twoPlayers; }
            set { this._twoPlayers = value; }
        }

        public bool GameOver
        {
            get { return this._gameOver; }
        }

        public bool Paused
        {
            get
            {
                return this._paused;
            }

            set
            {
                this._paused = value;
                if (this._paused)
                {
                    MediaPlayer.Pause();
                }
                else
                {
                    MediaPlayer.Resume();
                }
            }
        }

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        public override void Show()
        {
            MediaPlayer.Play(this._audio.BackMusic);
            this._meteors.Initialize();
            this._powerSource.PutinStartPosition();

            this._player1.Reset();
            if (this._twoPlayers)
            {
                this._player2.Reset();
                this._player2.Visible = this._twoPlayers;
                this._player2.Enabled = this._twoPlayers;
                this._scorePlayer2.Visible = this._twoPlayers;
                this._scorePlayer2.Enabled = this._twoPlayers;
            }

            this._paused = false;
            this._pausePosition.X = (Game.Window.ClientBounds.Width - this._pauseRect.Width) / 2;
            this._pausePosition.Y = (Game.Window.ClientBounds.Height - this._pauseRect.Height) / 2;

            base.Show();
        }

        public override void Hide()
        {
            MediaPlayer.Stop();

            base.Hide();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if ((!this._paused) && (!this._gameOver))
            {
                HandleDamages();

                HandlePowerSourceSprite(gameTime);

                // Update score
                this._scorePlayer1.Value = this._player1.Score;
                this._scorePlayer1.Power = this._player1.Power;
                if (this._twoPlayers)
                {
                    this._scorePlayer2.Value = this._player2.Score;
                    this._scorePlayer2.Power = this._player2.Power;
                }

                // Check if player is dead
                this._gameOver = (this._player1.Power <= 0) || (this._player2 != null && this._player2.Power <= 0);
                if (this._gameOver)
                {
                    this._player1.Visible = this._player1.Power > 0;
                    if (this._twoPlayers)
                    {
                        this._player2.Visible = this._player2.Power > 0;
                    }

                    MediaPlayer.Stop();
                }

                base.Update(gameTime);
            }

            if (this._gameOver)
            {
                this._meteors.Update(gameTime);
            }

            this._positionDebugText.Text = (this._player2 ?? this._player1).GetBounds().ToString();
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

            if (this._paused)
            {
                this._spriteBatch.Draw(this._actionTexture, this._pausePosition, this._pauseRect, Color.White);
            }

            if (this._gameOver)
            {
                this._spriteBatch.Draw(this._actionTexture, this._gameoverPosition, this._gameoverRect, Color.White);
            }
        }

        void HandleDamages()
        {
            // Check collision for Player 1
            if (this._meteors.CheckForCollisions(this._player1.GetBounds()))
            {
                this._player1.Power -= 10;
                this._player1.Score -= 1;
            }

            // Check collisions for Player 2
            if (this._twoPlayers)
            {
                if (this._meteors.CheckForCollisions(this._player2.GetBounds()))
                {
                    this._player2.Power -= 10;
                    this._player2.Score -= 10;
                }

                // Check for collision between players
                if (this._player1.GetBounds().Intersects(this._player2.GetBounds()))
                {
                    this._player1.Power -= 10;
                    this._player1.Score -= 10;
                    this._player2.Power -= 10;
                    this._player2.Score -= 10;
                }
            }
        }

        void HandlePowerSourceSprite(GameTime gameTime)
        {
            if (this._powerSource.CheckCollision(this._player1.GetBounds()))
            {
                this._audio.PowerGet.Play();
                this._elapsedTime = TimeSpan.Zero;
                this._powerSource.PutinStartPosition();
                this._player1.Power += 50;
            }

            if (this._twoPlayers)
            {
                if (this._powerSource.CheckCollision(this._player2.GetBounds()))
                {
                    this._audio.PowerGet.Play();
                    this._powerSource.PutinStartPosition();
                    this._player2.Power += 50;
                }
            }

            // Check for sending a new power source
            this._elapsedTime += gameTime.ElapsedGameTime;
            if (this._elapsedTime > TimeSpan.FromSeconds(15))
            {
                this._elapsedTime -= TimeSpan.FromSeconds(15);
                this._powerSource.Enabled = true;
            }
        }
    }
}
