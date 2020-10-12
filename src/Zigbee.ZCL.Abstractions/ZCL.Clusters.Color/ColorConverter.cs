using System;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Color
{
    public static class ColorConverter
    {
        public static CieColor Convert(Color color)
        {
            // Apply a gamma correction to the RGB values, which makes the color more vivid and 
            // more the like the color displayed on the screen of your device
            var red = (color.R > 0.04045) ? Math.Pow((color.R + 0.055) / (1.0 + 0.055), 2.4) : (color.R / 12.92);
            var green = (color.G > 0.04045) ? Math.Pow((color.G + 0.055) / (1.0 + 0.055), 2.4) : (color.G / 12.92);
            var blue = (color.B > 0.04045) ? Math.Pow((color.B + 0.055) / (1.0 + 0.055), 2.4) : (color.B / 12.92);

            //  RGB values to XYZ using the Wide RGB D65 conversion formula
            var X = red * 0.664511 + green * 0.154324 + blue * 0.162028;
            var Y = red * 0.283881 + green * 0.668433 + blue * 0.047685;
            var Z = red * 0.000088 + green * 0.072310 + blue * 0.986039;

            //Calculate the xy values from the XYZ values
            var x = (float)Math.Round((X / (X + Y + Z)), 5);
            var y = (float)Math.Round((Y / (X + Y + Z)), 4);

            if (double.IsNaN(x))
                x = 0;
            if (double.IsNaN(y))
                y = 0;

            return new CieColor(x, y);
        }
    }
}
