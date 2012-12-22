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
        protected Texture2D actionTexture;
        protected AudioLibrary audio;
        protected SpriteBatch spriteBatch = null;

        protected Player player1;
        protected Player player2;
        protected MeteorsManager meteors;
        protected PowerSource powerSource;
        protected SimpleRumblePad rumblePad;
        protected ImageComponent background;
        protected Score scorePlayer1;
        protected Score scorePlayer2;

        protected Vector2 pausePosition;
        protected Vector2 gameoverPosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Rectangle gameoverRect = new Rectangle(1, 170, 350, 48);

        protected bool paused;
        protected bool gameOver;
        protected TimeSpan elapsedTime = TimeSpan.Zero;
        protected bool twoPlayers;

        public ActionScene(Game game, Texture2D theTexture, Texture2D backgroundTexture, SpriteFont font)
            : base(game)
        {
            audio = (AudioLibrary) Game.Services.GetService(typeof (AudioLibrary));
            background = new ImageComponent(game, backgroundTexture, ImageComponent.DrawMode.Stretch);
            Components.Add(background);

            actionTexture = theTexture;

            spriteBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));
            meteors = new MeteorsManager(Game, ref actionTexture);
            Components.Add(meteors);

            player1 = new Player(Game, ref actionTexture, PlayerIndex.One, new Rectangle(323, 15, 30, 30));
            player1.Initialize();
            Components.Add(player1);

            player2 = new Player(Game, ref actionTexture, PlayerIndex.Two, new Rectangle(360, 17, 30, 30));
            player2.Initialize();
            Components.Add(player2);

            scorePlayer1 = new Score(game, font, Color.Blue);
            scorePlayer1.Position = new Vector2(10, 10);
            Components.Add(scorePlayer1);
            scorePlayer2 = new Score(game, font, Color.Red);
            scorePlayer2.Position = new Vector2(Game.Window.ClientBounds.Width - 200, 10);
            Components.Add(scorePlayer2);

            rumblePad = new SimpleRumblePad(game);
            Components.Add(rumblePad);

            powerSource = new PowerSource(game, ref actionTexture);
            powerSource.Initialize();
            Components.Add(powerSource);
        }

        public bool TwoPlayers
        {
            get { return twoPlayers; }
            set { twoPlayers = value; }
        }

        public bool GameOver
        {
            get { return gameOver; }
        }

        public bool Paused
        {
            get { return paused; }
            set
            {
                paused = value;
                if (paused)
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
            MediaPlayer.Play(audio.BackMusic);
            meteors.Initialize();
            powerSource.PutinStartPosition();

            player1.Reset();
            player2.Reset();

            paused = false;
            pausePosition.X = (Game.Window.ClientBounds.Width - pauseRect.Width/2);
            pausePosition.Y = (Game.Window.ClientBounds.Height - pauseRect.Height/2);

            player2.Visible = twoPlayers;
            player2.Enabled = twoPlayers;
            scorePlayer2.Visible = twoPlayers;
            scorePlayer2.Enabled = twoPlayers;

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
            if ((!paused) && (!gameOver))
            {
                HandleDamages();

                HandlePowerSourceSprite(gameTime);

                // Update score
                scorePlayer1.Value = player1.Score;
                scorePlayer1.Power = player1.Power;
                if (twoPlayers)
                {
                    scorePlayer2.Value = player2.Score;
                    scorePlayer2.Power = player2.Power;
                }

                // Check if player is dead
                gameOver = ((player1.Power <= 0) || (player2.Power <= 0));
                if (gameOver)
                {
                    player1.Visible = (player1.Power > 0);
                    player2.Visible = (player2.Power > 0) && twoPlayers;
                    MediaPlayer.Stop();
                }

                base.Update(gameTime);
            }

            if (gameOver)
            {
                meteors.Update(gameTime);
            }
        }

        private void HandleDamages()
        {
            // Check collision for Player 1
            if (meteors.CheckForCollisions(player1.GetBounds()))
            {
                player1.Power -= 10;
                player1.Score -= 1;
            }

            // Check collisions for Player 2
            if (twoPlayers)
            {
                if (meteors.CheckForCollisions(player2.GetBounds()))
                {
                    player2.Power -= 10;
                    player2.Score -= 10;
                }

                // Check for collision between players
                if (player1.GetBounds().Intersects(player2.GetBounds()))
                {
                    player1.Power -= 10;
                    player1.Score -= 10;
                    player2.Power -= 10;
                    player2.Score -= 10;
                }
            }
        }

        private void HandlePowerSourceSprite(GameTime gameTime)
        {
            if (powerSource.CheckCollision(player1.GetBounds()))
            {
                audio.PowerGet.Play();
                elapsedTime = TimeSpan.Zero;
                powerSource.PutinStartPosition();
                player1.Power += 50;
            }

            if (twoPlayers)
            {
                if (powerSource.CheckCollision(player2.GetBounds()))
                {
                    audio.PowerGet.Play();
                    powerSource.PutinStartPosition();
                    player2.Power += 50;
                }
            }

            // Check for sending a new power source
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(15))
            {
                elapsedTime -= TimeSpan.FromSeconds(15);
                powerSource.Enabled = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (paused)
            {
                spriteBatch.Draw(actionTexture, pausePosition, pauseRect, Color.White);
            }

            if (gameOver)
            {
                spriteBatch.Draw(actionTexture, gameoverPosition, gameoverRect, Color.White);
            }
        }
    }
}
