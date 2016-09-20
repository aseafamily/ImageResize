using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ImageResizerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("ImageResizerConsole.exe [folder name] [flip] [side]");
                return;
            }

            string folderName = args[0];
            bool flip = false;
            int lowSide = 1600;

            if (args.Count() > 1)
            {
                flip = Boolean.Parse(args[1]);
            }

            if (args.Count() > 2)
            {
                lowSide = int.Parse(args[2]);
            }

            string[] filePaths = Directory.GetFiles(folderName, "*.jp*", SearchOption.AllDirectories);
            if (filePaths.Length == 0)
            {
                LogString("No image files in " + folderName);
            }
            else
            {
                DateTime startTime = DateTime.Now;
                DateTime endTime = DateTime.Now;

                for (int i = 0; i < filePaths.Length; i++)
                {
                    string fileName = filePaths[i];
                    LogString(string.Format("RESIZE {0}/{1}: {2}", i + 1, filePaths.Length, fileName));
                    ResizeImage(fileName, fileName, ImageFormat.Jpeg, lowSide, flip);
                    endTime = DateTime.Now;
                    LogString(string.Format("\tTime: {0}\n", endTime - startTime));
                }

                endTime = DateTime.Now;
                LogString(string.Format("\nDuration: {0} for {1} files", endTime - startTime, filePaths.Length));
            }
        }

        public static bool ResizeImage(string fileName, string imgFileName, ImageFormat format, int lowSide, bool flip)
        {
            try
            {
                Image thumbNail = null;

                using (Image img = Image.FromFile(fileName))
                {
                    int width = img.Width;
                    int height = img.Height;
                    bool portrait = height > width;
                    bool needResize = portrait ? height > lowSide : width > lowSide;
                    int newWidth, newHeight;

                    if (needResize)
                    {
                        if (portrait)
                        {
                            newHeight = lowSide;
                            newWidth = newHeight * width / height;
                        }
                        else
                        {
                            newWidth = lowSide;
                            newHeight = newWidth * height / width;
                        }
                    }
                    else
                    {
                        newWidth = width;
                        newHeight = height;
                    }

                    LogString(string.Format("\tSize - {0}x{1}", newWidth, newHeight), false);

                    thumbNail = new Bitmap(newWidth, newHeight, img.PixelFormat);
                    Graphics g = Graphics.FromImage(thumbNail);
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    Rectangle rect = new Rectangle(0, 0, newWidth, newHeight);
                    g.DrawImage(img, rect);
                    if (flip && (portrait & newHeight > 900))
                    {
                        thumbNail.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        LogString("\tFlipped");
                    }
                    else
                    {
                        LogString(string.Empty);
                    }
                    

                    foreach (var item in img.PropertyItems)
                    {
                        thumbNail.SetPropertyItem(item);
                    }
                }

                if (thumbNail != null)
                {
                    thumbNail.Save(imgFileName, format);
                }

                return true;
            }
            catch (Exception e)
            {
                LogException(e);
                return false;
            }
        }

        private static void LogException(Exception e)
        {
            LogString("ERROR: " + e.Source + " - " + e.Message + "\r\n" + e.StackTrace + "\r\n");
        }

        private static void LogString(string log)
        {
            Console.WriteLine(log, true);
        }

        private static void LogString(string log, bool newLine)
        {
            if (newLine)
            {
                Console.WriteLine(log);
            }
            else
            {
                Console.Write(log);
            }
        }
    }
}
