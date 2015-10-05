namespace ImageConverter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    /*internal static class Ext
    {
        public static void When<T>(this IEnumerable<T> list, bool ifTrue, Action thenExecute)
        {
            if (ifTrue)
            {
                thenExecute.Invoke();
            }
        }
    }*/

    /// <summary>
    /// entry point. Contains the Main Method to call the Image To Test class.
    /// It takes the user input and validate first before calling
    /// ImageToTest class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main method Entry point
        /// </summary>
        /// <param name="args">parameter to process the input image</param>
        private static void Main(string[] args)
        {
            // in(i) out(o) reverse(r)
            var param = new List<string> { "/i", "/o", "/r", "/h", "/w" };

            // i o r must be at odd position
            // -- /i is must            
            if (args.Count(c => c == "/i" || c == "/I") == 0)
            {
                Console.WriteLine(
                    "\t>>Please check the command format.\n\tCommand it not properly formatted.\n\t'/i file.jpg' is required.");
                ShowHelp();
                return;
            }

            if (param.Any(c =>
            {
                var index = Array.FindIndex(args, el => el.Equals(c, StringComparison.InvariantCultureIgnoreCase));
                return index > -1 && index % 2 == 1;
            }))
            {
                Console.WriteLine("\t>>Please check the command format.\n\tCommand it not properly formatted.");
                ShowHelp();
                return;
            }

            Func<string, string> myFunc = ope =>
            {
                var index = Array.FindIndex(args, el => el.Equals(ope, StringComparison.InvariantCultureIgnoreCase));
                if (index > -1 && index + 1 < args.Length)
                {
                    return args[index + 1];
                }

                return string.Empty;
            };
            Func<string, int> getPercentageValue = ope =>
            {
                var result = -1;
                var value = myFunc(ope);
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Last() == '%')
                    {
                        value = value.Substring(0, value.Length - 1).Trim();
                    }

                    int.TryParse(value, out result);
                    if (result > 100)
                    {
                        result = 100;
                    }
                }

                return result;
            };

            var filePassedIn = myFunc(param.ElementAt(0));
            var outputFile = myFunc(param.ElementAt(1));
            var isReverse = myFunc(param.ElementAt(2)).Equals("1");
            var height = getPercentageValue(param.ElementAt(3));
            var width = getPercentageValue(param.ElementAt(4));

            filePassedIn = GetTheFilePath(filePassedIn);
            outputFile = string.IsNullOrEmpty(outputFile)
                ? Path.GetFileNameWithoutExtension(filePassedIn) + ".txt"
                : outputFile;

            if (string.IsNullOrEmpty(filePassedIn))
            {
                return;
            }

            var obj = new ImageToText(filePassedIn);
            obj.Reverse = isReverse;
            Console.WriteLine("Converting from [{0}] to [{1}]...", Path.GetFileName(filePassedIn), outputFile);
            Thread.Sleep(500);
            if (obj.SaveTheLoad(outputFile, width, height))
            {
                Console.WriteLine("File Processed successfully.");
            }

            Thread.Sleep(200);
        }

        /// <summary>
        /// show the help for the utility
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine(" - File Conversion - ");
            Console.WriteLine(
                "A tool to create the text file from the image.jpg.\nDefault output file name will be same as input file name, and by default color will not be revers.");
            Console.WriteLine("ImageConverter /i Image.jpg [/o ImageFileAsText.txt] [/r 0 or 1] [/h 50%] [/w 100%]");
            Console.WriteLine("/i\t-\tFully specified input image file path.");
            Console.WriteLine("/o\t-\toutout text file path.");
            Console.WriteLine("/r\t-\t0(default) OR 1. 1 reverse the color.[Black-White]");
            Console.WriteLine("/h\t-\tSpecify the Height 0-100%");
            Console.WriteLine("/w\t-\tSpecify the Width 0-100%");
            Console.WriteLine("\neg.\n\t1.\tImageConverter /i Image.jpg /o ImageFileAsText.txt");
            Console.WriteLine("\t  \tit will generate the output ImageFileAsText.txt");
            Console.WriteLine("\t2.\tImageConverter /i Image.jpg");
            Console.WriteLine("\t  \tit will generate the output Image.txt");
            Console.WriteLine("\t3.\tImageConverter /i Image.jpg /r 1");
            Console.WriteLine("\t  \tit will generate the output Image.txt, color reverse.");
            Console.WriteLine("---------------------------------------------------------");
        }

        /// <summary>
        /// check if passed in filePath is not valid. 
        /// if so then ask the user to enter the file name again.
        /// </summary>
        /// <param name="filePath">inputImage full specified path</param>
        /// <returns>returns the correct file name </returns>
        private static string GetTheFilePath(string filePath)
        {
            while (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Please enter fully qualified Image file path.");
                filePath = Console.ReadLine();
                if (filePath.Equals("quit", StringComparison.InvariantCultureIgnoreCase) ||
                    filePath.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
            }

            while (!File.Exists(filePath))
            {
                Console.WriteLine("Can not locate the file.  Please enter the correct file location.");
                filePath = Console.ReadLine();
                if (filePath.Equals("quit", StringComparison.InvariantCultureIgnoreCase) ||
                    filePath.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
            }

            return filePath;
        }
    }
}