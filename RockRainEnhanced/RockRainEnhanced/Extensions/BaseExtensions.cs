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
    using System.Text;

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
    }
}
