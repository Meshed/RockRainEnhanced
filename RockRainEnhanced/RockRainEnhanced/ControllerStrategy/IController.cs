// -----------------------------------------------------------------------
// <copyright file="IController.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RockRainEnhanced.ControllerStrategy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IController
    {
        void Update();
        /// <summary>
        /// -1 to 1
        /// </summary>
        float XSensitivity { get; set; }
         
        /// <summary>
        /// -1 to 1
        /// </summary>
        float YSensitivity { get; set; }
        bool IsEnter { get; }
        bool IsBack { get; }
        bool IsUp { get; }
        bool IsLeft { get; }
        bool IsRight { get; }
        bool IsDown { get; }
        float XThrottle { get; }
        float YThrottle { get; }
        bool UseContinuous { get; set; }
    }
}
