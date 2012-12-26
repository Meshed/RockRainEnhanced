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


namespace RockRainEnhanced
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MeteorsManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // List of active meteors
        protected List<Meteor> meteors;
        // Constant for initial meteor count
        private const int STARTMETEORCOUNT = 10;
        // Time for new meteor
        private const int ADDMETEORTIME = 5000;

        protected Texture2D meteorTexture;
        protected TimeSpan elapsedTime = TimeSpan.Zero;
        protected AudioLibrary audio;

        public MeteorsManager(Game game, ref Texture2D theTexture)
            : base(game)
        {
            meteorTexture = theTexture;
            meteors = new List<Meteor>();
            audio = (AudioLibrary) Game.Services.GetService(typeof (AudioLibrary));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            meteors.Clear();

            Start();

            for (int i = 0; i < meteors.Count; i++)
            {
            }

            base.Initialize();
        }

        public void Start()
        {
            elapsedTime = TimeSpan.Zero;

            for (int i = 0; i < STARTMETEORCOUNT; i++)
            {
                AddNewMeteor();
            }
        }

        public List<Meteor> AllMeteors
        {
            get { return meteors; }
        }

        private void CheckForNewMeteor(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromMilliseconds(ADDMETEORTIME))
            {
                elapsedTime -= TimeSpan.FromMilliseconds(ADDMETEORTIME);
                AddNewMeteor();
                audio.NewMeteor.Play();
            }
        }

        private void AddNewMeteor()
        {
            Meteor newMeteor = new Meteor(Game, ref meteorTexture);
            meteors.Add(newMeteor);
            newMeteor.Index = meteors.Count - 1;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            CheckForNewMeteor(gameTime);

            for (int i = 0; i < meteors.Count; i++)
            {
                meteors[i].Update(gameTime);
            }

            base.Update(gameTime);
        }

        public bool CheckForCollisions(Rectangle rect)
        {
            for (int i = 0; i < meteors.Count; i++)
            {
                if (meteors[i].CheckCollision(rect))
                {
                    audio.Explosion.Play();
                    meteors[i].PutinStartPosition();

                    return true;
                }
            }

            return false;
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < meteors.Count; i++)
            {
                meteors[i].Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
