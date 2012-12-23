// -----------------------------------------------------------------------
// <copyright file="XboxController.cs" company="">
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
    public class XboxController:IController
    {
        readonly PlayerIndex _index;
        private ControllerCommon<GamePadState, Buttons> _controllerCommon;
        
        public float XSensitivity { get; set; }
        public float YSensitivity { get; set; }

        public XboxController(PlayerIndex index)
        {
            _index = index;
            XSensitivity = 1;
            YSensitivity = 1;
            _controllerCommon = new ControllerCommon<GamePadState, Buttons>((gs, b) => gs.IsButtonDown(b), (gs, b) => gs.IsButtonUp(b), () => GamePad.GetState(_index));
            UseContinuous = true;
        }

        bool IsButtonDown(bool useContinuous, params Buttons[] keys)
        {
            return _controllerCommon.IsPressed(useContinuous, keys);
        }

        #region IController Members

        public bool IsEnter
        {
            get { return IsButtonDown(false,Buttons.Start,Buttons.A); }
        }

        public bool IsBack
        {
            get { return IsButtonDown(false,Buttons.Back,Buttons.B); }
        }

        public bool IsUp
        {
            get { return IsButtonDown(UseContinuous, Buttons.LeftThumbstickUp, Buttons.DPadUp); }
        }

        public bool IsLeft
        {
            get { return IsButtonDown(UseContinuous, Buttons.LeftThumbstickLeft, Buttons.DPadLeft); }
        }

        public bool IsRight
        {
            get { return IsButtonDown(UseContinuous, Buttons.LeftThumbstickRight, Buttons.DPadRight); }
        }

        public bool IsDown
        {
            get { return IsButtonDown(UseContinuous, Buttons.LeftThumbstickDown, Buttons.DPadDown); }
        }

        float Throttle(bool left, bool right, float sensitivity,float value)
        {
            if (left && right)
                return 0;
            if (!left && !right)
                return 0;
            if (left)
                return value * sensitivity;
            if (right)
                return sensitivity * value;
            return 0;
        }

        public float XThrottle
        {
            get
            {
                return Throttle(IsLeft, IsRight, XSensitivity,_controllerCommon.State.ThumbSticks.Left.X);
            }
        }

        public float YThrottle
        {
            get { return Throttle(IsUp, IsDown, YSensitivity, _controllerCommon.State.ThumbSticks.Left.Y); }
        }

        public void Update()
        {
            _controllerCommon.Update();
            
        }

        public bool UseContinuous
        {
            get;
            set;
        }

        #endregion
    }
}
