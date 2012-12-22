// -----------------------------------------------------------------------
// <copyright file="ArrowController.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RockRainEnhanced.ControllerStrategy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ArrowController:IController
    {
         readonly PlayerIndex _index;
         public float XSensitivity { get; set; }
         public float YSensitivity { get; set; }
         ControllerCommon<KeyboardState, Keys> _controllerCommon;
         public ArrowController(PlayerIndex index)
        {
            _index = index;
            XSensitivity = 1;
            YSensitivity = 1;
            _controllerCommon = new ControllerCommon<KeyboardState, Keys>((ks, k) => ks.IsKeyDown(k), (ks, k) => ks.IsKeyUp(k),  () => Keyboard.GetState(_index));
            UseContinuous = true;
        }

        public void Update()
        {
            _controllerCommon.Update();
        }

        
        #region IController Members
       
        public bool IsEnter
        {
            get { return _controllerCommon.IsPressed(false,Keys.Enter, Keys.Space); }
        }

        public bool IsBack
        {
            get { return _controllerCommon.IsPressed(false,Keys.Escape); }
        }

        public bool IsUp
        {
            get { return _controllerCommon.IsPressed(UseContinuous, Keys.Up); }
        }

        public bool IsLeft
        {
            get { return _controllerCommon.IsPressed(UseContinuous, Keys.Left); }
        }

        public bool IsRight
        {
            get { return _controllerCommon.IsPressed(UseContinuous, Keys.Right); }
        }

        public bool IsDown
        {
            get { return _controllerCommon.IsPressed(UseContinuous, Keys.Down); }
        }

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

        

        public bool UseContinuous { get; set; }

        #endregion
    }
}
