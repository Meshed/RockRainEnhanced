// -----------------------------------------------------------------------
// <copyright file="ControllerCommon.cs" company="">
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
    public class ControllerCommon<TState,TEnum>
    {
        TState previousState;
        TState currentState;
        private Func<TState> _getState;
        private Func<TState, TEnum, bool> _isUp;
        private Func<TState, TEnum, bool> _isDown;

        public TState State { get { return currentState; } }
        public ControllerCommon(Func<TState, TEnum, bool> isDown, Func<TState, TEnum, bool> isUp, Func<TState> getState)
        {
            _isDown = isDown;
            _isUp = isUp;
            _getState = getState;
        }

        public bool IsPressed(bool useContinuous, params TEnum[] keys)
        {

            return keys.Select(b => _isDown(currentState, b) && (useContinuous || _isUp(previousState, b)))
                .Aggregate((b1, b2) => b1 || b2);
        }

        public void Update()
        {
            previousState = currentState;
            currentState = _getState();
        }
    }
}
