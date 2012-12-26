namespace RockRainEnhanced.Core
{
    using System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// This component helps shake your Joystick
    /// </summary>
    public class SimpleRumblePad : GameComponent
    {
        int _time;
        int _lastTickCount;

        public SimpleRumblePad(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (this._time > 0)
            {
                int elapsed = Environment.TickCount - this._lastTickCount;
                if (elapsed >= this._time)
                {
                    this._time = 0;
                    GamePad.SetVibration(PlayerIndex.One, 0, 0);
                }
            }

            base.Update(gameTime);
        }
        
        /// <summary>
        /// Set the vibration
        /// </summary>
        /// <param name="time">Vibration time</param>
        /// <param name="leftMotor">Left Motor Intensity</param>
        /// <param name="rightMotor">Right Motor Intensity</param>
        public void RumblePad(int time, float leftMotor, float rightMotor)
        {
            this._lastTickCount = Environment.TickCount;
            this._time = time;
            GamePad.SetVibration(PlayerIndex.One, leftMotor, rightMotor);
        }

        /// <summary>
        /// Turn off the rumble
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);

            base.Dispose(disposing);
        }
    }
}