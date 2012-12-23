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
using RockRainEnhanced.ControllerStrategy;


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
#if DEBUG
        TextComponent positionDebugText;
#endif
        protected Vector2 pausePosition;
        protected Vector2 gameoverPosition;
        protected Rectangle pauseRect = new Rectangle(1, 120, 200, 44);
        protected Rectangle gameoverRect = new Rectangle(1, 170, 350, 48);

        protected bool paused;
        protected bool gameOver;
        protected TimeSpan elapsedTime = TimeSpan.Zero;
        protected bool twoPlayers;
        Game1 game1;
        Texture2D actionElementsTexture;
        Texture2D actionBackgroundTexture;
        SpriteFont scoreFont;
        IController[] controllers;

        ActionScene(Game game, Texture2D theTexture, Texture2D backgroundTexture, SpriteFont font):base(game)
        {
            audio = (AudioLibrary)Game.Services.GetService(typeof(AudioLibrary));
            background = new ImageComponent(game, backgroundTexture, ImageComponent.DrawMode.Stretch);
            Components.Add(background);

            actionTexture = theTexture;

            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            meteors = new MeteorsManager(Game, ref actionTexture);
            Components.Add(meteors);


            // TODO: Complete member initialization
            this.game1 = game1;
            this.actionElementsTexture = actionElementsTexture;
            this.actionBackgroundTexture = actionBackgroundTexture;
            this.scoreFont = font;
          
            
            scorePlayer1 = new Score(game, font, Color.Blue);
            scorePlayer1.Position = new Vector2(10, 10);
            Components.Add(scorePlayer1);
           

            rumblePad = new SimpleRumblePad(game);
            Components.Add(rumblePad);

            powerSource = new PowerSource(game, ref actionTexture);
            powerSource.Initialize();
            Components.Add(powerSource);
#if DEBUG
            positionDebugText=new TextComponent(game,this.scoreFont,new Vector2(),Color.Red);
            Components.Add(positionDebugText);
            
#endif
        }
        public ActionScene(Game game, Texture2D theTexture, Texture2D backgroundTexture, SpriteFont font,Rectangle screenBounds, params IController[] controllers)
            :this(game,theTexture,backgroundTexture,font)
        {
            this.TwoPlayers = false;
            player1 = new Player(Game, ref actionTexture,  new Vector2(screenBounds.Width / 3, 0), new Rectangle(323, 15, 30, 30),controllers);
            player1.Initialize();
            Components.Add(player1);   
        }
        public ActionScene(IController playerOneController,IController playerTwoController,Game game, Texture2D theTexture, Texture2D backgroundTexture,Rectangle screenBounds, SpriteFont font)
            : this(game, theTexture, backgroundTexture, font)
        {
            
            this.TwoPlayers = true;
            player1 = new Player(Game, ref actionTexture,new Vector2(screenBounds.Width / 3, 0), new Rectangle(323, 15, 30, 30),playerOneController);
            player1.Initialize();
            Components.Add(player1);

            scorePlayer2 = new Score(game, font, Color.Red);
            scorePlayer2.Position = new Vector2(Game.Window.ClientBounds.Width - 200, 10);
            Components.Add(scorePlayer2);
            player2 = new Player(Game, ref actionTexture, new Vector2((int)(screenBounds.Width / 1.5), 0), new Rectangle(360, 17, 30, 30), playerTwoController);
            player2.Initialize();
            Components.Add(player2);
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
            if (twoPlayers)
            {
                
                player2.Reset();
                player2.Visible = twoPlayers;
                player2.Enabled = twoPlayers;
                scorePlayer2.Visible = twoPlayers;
                scorePlayer2.Enabled = twoPlayers;
            }
            
            paused = false;
            pausePosition.X = (Game.Window.ClientBounds.Width - pauseRect.Width/2);
            pausePosition.Y = (Game.Window.ClientBounds.Height - pauseRect.Height/2);

            

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
                gameOver = ((player1.Power <= 0) || (player2 != null && player2.Power <= 0));
                if (gameOver)
                {
                    player1.Visible = (player1.Power > 0);
                    if (twoPlayers)
                    {
                        player2.Visible = (player2.Power > 0);
                    }
                    MediaPlayer.Stop();
                }

                base.Update(gameTime);
            }

            if (gameOver)
            {
                meteors.Update(gameTime);
            }
            positionDebugText.Text =( player2 ?? player1).GetBounds().ToString();
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
            if (this.Components.Contains(this.player1) == false && this.Components.Contains(this.player2) == false)
                throw new InvalidOperationException("No players found");
            if (this.Components.OfType<Player>().Select(p => p.DrawOrder).Any(dOrder =>
                this.Components.OfType<DrawableGameComponent>()
                    .Except(this.Components.OfType<Player>())
                    .Select(c => c.DrawOrder)
                    .All(c => dOrder > c)))
            {
                throw new InvalidOperationException("Players are not top component");
            }

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
