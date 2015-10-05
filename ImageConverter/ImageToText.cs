namespace ImageConverter
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Main class for Image processing.
    /// </summary>
    internal class ImageToText
    {
        /// <summary>
        /// We set the value of imagePath in constructor
        /// </summary>
        private readonly string imagePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageToText"/> class.
        /// </summary>
        /// <param name="imagePath">input image path - full specified.</param>        
        public ImageToText(string imagePath)
        {
            // var a = imagePath?.Length;
            this.imagePath = imagePath;
            this.Reverse = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether need to perform reverse operation or not
        /// </summary>        
        public bool Reverse { get; set; }        

        /// <summary>
        /// Main public method we will trigger from the Main Method
        /// after object creation
        /// </summary>
        /// <param name="saveAsFileName">file name to be saved as</param>
        /// <param name="widthPer">[OPTIONAL] width as percentage</param>
        /// <param name="heightPer">[OPTIONAL] height as percentage</param>
        /// <returns>it returns the boolean - either file generated successfully or not.</returns>
        internal bool SaveTheLoad(string saveAsFileName, double widthPer = 100, double heightPer = 100)
        {
            var result = false;

            try
            {
                var horizontalShift = widthPer == -1 || widthPer > 100 ? 1 : (int)(100 / widthPer);
                var verticalShift = heightPer == -1 || heightPer > 100 ? 1 : (int)(100 / heightPer);

                using (var writer = new StreamWriter(saveAsFileName))
                {
                    var image = new Bitmap(this.imagePath);
                    var colors = new List<Color>();
                    for (var y = 0; y < image.Height; y = y + verticalShift)
                    {
                        for (var x = 0; x < image.Width; x = x + horizontalShift)
                        {
                            for (var i = 0; i < verticalShift; i++)
                            {
                                for (var j = 0; j < horizontalShift; j++)
                                {
                                    if (x + j < image.Width && y + i < image.Height)
                                    {
                                        colors.Add(image.GetPixel(x + j, y + i));
                                    }
                                }
                            }

                            writer.Write(this.GetEqualAsciiChar(colors.ToArray()));
                            colors.Clear();
                        }

                        writer.Write(Environment.NewLine);
                    }
                }

                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Method to generate the ASCII char from the colors
        /// </summary>
        /// <param name="colors">Collection of the colors to get the "ASCII" value of </param>
        /// <returns>returns the "ASCII" value for all input color objects</returns>
        private string GetEqualAsciiChar(params Color[] colors)
        {
            // $@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\|()1{}[]?-_+~<>i!lI;:,"^`'. 
            var brightnessArray = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`'  ";
            brightnessArray = "@%#*+=-:. ";
            var avgBrg = colors.Average(c => this.GetBrightness(c));
            var index = (int)((avgBrg / 255.00) * brightnessArray.Length);
            var str = string.Format("{0}", brightnessArray[this.Reverse ? brightnessArray.Length - index - 1 : index]);
            return str;
        }

        /// <summary>
        /// Method to generate the Brightness for color object
        /// </summary>
        /// <param name="color">Color object to calculate the brightness of</param>
        /// <returns>returns the brightness of the color Range[0-255]</returns>
        private double GetBrightness(Color color)
        {
            return (0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B);
        }
    }
}