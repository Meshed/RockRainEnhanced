namespace RockRainEnhanced.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GameScene : DrawableGameComponent
    {
        private readonly List<GameComponent> _components;

        public GameScene(Game game)
            : base(game)
        {
            this._components = new List<GameComponent>();
            this.Visible = false;
            this.Enabled = false;
        }

        public IList<GameComponent> Components
        {
            get { return this._components; }
        }

        /// <summary>
        /// Show the scene
        /// </summary>
        public virtual void Show()
        {
            this.Visible = true;
            this.Enabled = true;
        }

        /// <summary>
        /// Hide the scene
        /// </summary>
        public virtual void Hide()
        {
            this.Visible = false;
            this.Enabled = false;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Update the child GameComponents (if Enabled)
            foreach (GameComponent gc in this._components.Where(c => c.Enabled))
            {
                gc.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw the child GameComponents (if drawable)
            foreach (var dgc in this._components.OfType<DrawableGameComponent>().Where(c => c.Visible))
            {
                dgc.Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
