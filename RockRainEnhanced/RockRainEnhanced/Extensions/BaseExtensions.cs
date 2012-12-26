// -----------------------------------------------------------------------
// <copyright file="BaseExtensions.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RockRainEnhanced.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using RockRainEnhanced.Core;

    /// <summary>
    /// Extensions for T and object
    /// </summary>
    public static class BaseExtensions
    {
        public static void ThrowIfNull(this object o, string name)
        {
            if (o == null)
                throw new NullReferenceException(name);
        }

        /// <summary>
        /// http://stackoverflow.com/a/4368126/57883
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDisplayOrName(this Enum value)
        {
            var display=
                value.GetType()
                     .GetField(value.ToString())
                     .GetCustomAttributes(typeof(DisplayAttribute), false).OfType<DisplayAttribute>()
                     .SingleOrDefault();
            if (display == null) return value.ToString();

            return display.Text;
        }
    }
}
