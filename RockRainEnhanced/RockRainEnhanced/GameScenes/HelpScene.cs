namespace RockRainEnhanced.GameScenes
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using RockRainEnhanced.Core;

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class HelpScene : GameScene
    {
        public HelpScene(Game game, Texture2D textureBack, Texture2D textureFront)
            : base(game)
        {
            this.Components.Add(new ImageComponent(game, textureBack, ImageComponent.DrawMode.Stretch));
            this.Components.Add(new ImageComponent(game, textureFront, ImageComponent.DrawMode.Center));
        }
    }
}
