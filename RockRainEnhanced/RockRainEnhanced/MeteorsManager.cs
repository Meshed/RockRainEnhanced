using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace RockRainEnhanced
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MeteorsManager : DrawableGameComponent
    {
        // List of active meteors
        protected List<Meteor> meteors;
        // Constant for initial meteor count
        private const int STARTMETEORCOUNT = 10;
        // Time for new meteor
        private const int STARTADDMETEORTIME = 1000;
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

            base.Initialize();
        }

        public List<Meteor> AllMeteors
        {
            get { return meteors; }
        }

        private void CheckForNewMeteor(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (meteors.Count < STARTMETEORCOUNT)
            {
                if (elapsedTime > TimeSpan.FromMilliseconds(STARTADDMETEORTIME))
                {
                    elapsedTime -= TimeSpan.FromMilliseconds(STARTADDMETEORTIME);
                    AddNewMeteor();
                    audio.NewMeteor.Play();
                }
            }
            else
            {
                if (elapsedTime > TimeSpan.FromMilliseconds(ADDMETEORTIME))
                {
                    elapsedTime -= TimeSpan.FromMilliseconds(ADDMETEORTIME);
                    AddNewMeteor();
                    audio.NewMeteor.Play();
                }
            }
        }

        private void AddNewMeteor()
        {
            var newMeteor = new Meteor(Game, ref meteorTexture);
            newMeteor.Initialize();
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

            foreach (Meteor t in meteors)
            {
                t.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public bool CheckForCollisions(Rectangle rect)
        {
            foreach (Meteor t in meteors)
            {
                if (t.CheckCollision(rect))
                {
                    audio.Explosion.Play();
                    t.PutinStartPosition();

                    return true;
                }
            }

            return false;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (Meteor t in meteors)
            {
                t.Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
