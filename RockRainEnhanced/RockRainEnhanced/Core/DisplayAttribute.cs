// -----------------------------------------------------------------------
// <copyright file="DisplayAttribute.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RockRainEnhanced.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DisplayAttribute : Attribute
    {
        public string Text { get; private set; }

        public DisplayAttribute(string text)
        {
            Text = text;
        }
    }
}
