namespace RockRainEnhanced.Core
{
    using System;

    public class DisplayAttribute : Attribute
    {

        public DisplayAttribute(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}
