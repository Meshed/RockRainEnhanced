// -----------------------------------------------------------------------
// <copyright file="WasdController.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RockRainEnhanced.ControllerStrategy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WasdController : IController
    {
        readonly PlayerIndex _index;

        public WasdController(PlayerIndex index)
        {
            _index = index;
            _controllerCommon = new ControllerCommon<KeyboardState, Keys>((ks, k) => ks.IsKeyDown(k), (ks, k) => ks.IsKeyUp(k), () => Keyboard.GetState(_index));
            XSensitivity = -1;
            YSensitivity = 1;
            UseContinuous = true;
        }
        public float XSensitivity { get; set; }
        public float YSensitivity { get; set; }

        public void Update()
        {
            _controllerCommon.Update();
            
        }

     
        private ControllerCommon<KeyboardState, Keys> _controllerCommon;

        #region IController Members
        bool IsKeyDown(bool useContinuous,  params Keys[] keys)
        {
            return _controllerCommon.IsPressed(useContinuous, keys);
        }
        public bool IsEnter
        {
            get { return IsKeyDown(false, Keys.F, Keys.E); }
        }

        public bool IsBack
        {
            get { return IsKeyDown(false, Keys.Q); }
        }

        public bool IsUp
        {
            get { return IsKeyDown(UseContinuous,Keys.W); }
        }

        public bool IsLeft
        {
            get { return IsKeyDown(UseContinuous,Keys.A); }
        }

        public bool IsRight
        {
            get { return IsKeyDown(UseContinuous, Keys.D); }
        }

        public bool IsDown
        {
            get { return IsKeyDown(UseContinuous,Keys.S); }
        }

        #endregion

        #region IController Members

        float Throttle(bool left, bool right, float sensitivity)
        {
            if (left && right)
                return 0;
            if (!left && !right)
                return 0;
            if (left)
                return sensitivity;
            if (right)
                return -1 * sensitivity;
            return 0;
        }

        public float XThrottle
        {
            get
            {
                return Throttle(IsLeft, IsRight, XSensitivity);
            }
        }

        public float YThrottle
        {
            get { return Throttle(IsUp, IsDown, YSensitivity); }
        }

        #endregion

        #region IController Members


        public bool UseContinuous { get; set; }

        #endregion
    }
}
