// -----------------------------------------------------------------------
// <copyright file="GameExtensions.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RockRainEnhanced.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class GameExtensions
    {
        public static Rectangle GetScreenBounds(this Game game)
        {
#if XBOX360
            return new Rectangle((int) (game.Window.ClientBounds.Width*
                                                0.03f), (int) (game.Window.ClientBounds.Height*0.03f),
                                         game.Window.ClientBounds.Width -
                                         (int) (game.Window.ClientBounds.Width*0.03f),
                                         game.Window.ClientBounds.Height -
                                         (int) (game.Window.ClientBounds.Height*0.03f));
#else
            return new Rectangle(0, 0, game.Window.ClientBounds.Width,
                                         game.Window.ClientBounds.Height);
#endif
        }
    }
}
