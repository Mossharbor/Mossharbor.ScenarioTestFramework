using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework
{
#if WIN32
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// ScreenCapture
    /// </summary>
    internal sealed class ScreenCapture
    {
        #region Members

        /// <summary>
        /// Empty color for blank screen shots
        /// </summary>
        private static readonly Color BlankColor = Color.FromArgb(0);

        /// <summary>
        /// Default file extension for screenshot image
        /// </summary>
        private const string ImageFileExtension = ".png";

        /// <summary>
        /// Copy source command for bit blit
        /// </summary>
        private const Int32 SourceCopy = 0x00CC0020;

        /// <summary>
        /// Font size of screen capture message
        /// </summary>
        private const int MessageFontSize = 20;

        /// <summary>
        /// Message written to screen capture image if it is blank
        /// </summary>
        private const string BlankScreenCaptureMessage = "Screen capture image is blank\nThis is likely caused by the screensaver running, or by the machine being locked during test";

        #endregion

        #region Enumerations

        /// <summary>
        /// Blank image could be black, white or empty
        /// </summary>
        private enum ImageBlank
        {
            NonBlank,
            Empty,
            Black,
            White
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Captures the Screen Shot in a file
        /// </summary>
        /// <returns>name of the file in which the screen shot was captured</returns>
        internal static string CaptureScreenshot()
        {
            return CaptureScreenshotWithFileName(Environment.CurrentDirectory, DateTime.Now.ToUniversalTime().ToFileTimeUtc().ToString());
        }

        /// <summary>
        /// Called when needed to create screenshot
        /// </summary>
        /// <param name="targetDir">dir to save the file</param>
        /// <param name="fileName">This is the file name that we would like to save.</param>
        /// <returns>name of the image file.</returns>
        internal static string CaptureScreenshotWithFileName(string targetDir, string fileName)
        {
            string fullPath = GetUniqueFileName(targetDir, fileName);

            if (!String.IsNullOrEmpty(fullPath))
            {
                Debug.Write(String.Format("Generating screenshot '{0}'", fullPath));
                Image image = ScreenCapture.GetDesktopScreenShot();

                // Check if resulting image is blank
                ImageBlank blank = CheckBlankImage(image);

                // If it is a blank image
                if (blank != ImageBlank.NonBlank)
                {
                    Debug.Write(BlankScreenCaptureMessage);

                    // If the image is all black, write with white letters
                    WriteToImage(ref image, BlankScreenCaptureMessage, blank == ImageBlank.Black ? Brushes.White : Brushes.Black);
                }

                image.Save(fullPath, ImageFormat.Png);
            }

            return fullPath;
        }

        /// <summary>
        /// Captures the Screen shot in a specified file
        /// </summary>
        /// <param name="filename">name of the file to save the screenshot</param>
        internal static void CaptureScreenShot(String filename)
        {
            Debug.Write(String.Format("Capturing screenshot '{0}'", filename));
            Image captureImage = ScreenCapture.GetDesktopScreenShot();
            captureImage.Save(filename);
        }

        /// <summary>
        /// This function gets the desktop screen shot.
        /// </summary>
        /// <returns>A bitmap with the desktop screen shot in it.</returns>
        private static Bitmap GetDesktopScreenShot()
        {
            return GetWindowScreenShot(GetDesktopWindow());
        }

        /// <summary>
        /// THis function gets the screen shot of a window.
        /// </summary>
        /// <param name="windowHandle">This is the handle (in int form) for the window to get the screen capture of.</param>
        /// <returns>Returns a bitmap with the captured window in it.</returns>
        private static Bitmap GetWindowScreenShot(int windowHandle)
        {
            return GetWindowScreenShot((IntPtr)windowHandle);
        }

        /// <summary>
        /// THis function gets the screen shot of a window..
        /// </summary>
        /// <param name="windowHandle">This is the handle (in IntPtr form) of the window to get the screen shot of.</param>
        /// <returns>Returns a bitmap of the window.</returns>
        private static Bitmap GetWindowScreenShot(IntPtr windowHandle)
        {
            Rectangle windowRect = Rectangle.Empty;

            if (windowHandle == IntPtr.Zero)
            {
                // Desktop image, so set rectangle appropriately.
                windowRect = Screen.PrimaryScreen.Bounds;
            }
            else
            {
                // Check that window handle is valid and get the window size info.
                Win32APIDataTypes.RECT rect = new Win32APIDataTypes.RECT();
                if (!GetWindowRect(windowHandle, ref rect))
                {
                    // Throw exception if window handle is invalid.
                    throw new ArgumentException("The window handle is not valid.");
                }

                // Set up the window rectangle.
                windowRect = Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }

            // Create a new bitmap the size of the primary display device
            Bitmap bmpScreenShot = new Bitmap(windowRect.Width, windowRect.Height);

            // Get a Graphics object from the bitmap so we can draw on it
            Graphics gfx = Graphics.FromImage(bmpScreenShot);

            // Get a win32 DC (Device Context) for destination blitting
            IntPtr dcDestination = gfx.GetHdc();

            // Now get a source DC from the window handle
            IntPtr dcSource = GetWindowDC(windowHandle);

            // Blit from the desktop's DC to our bitmap's DC
            BitBlt(dcDestination, 0, 0, windowRect.Width, windowRect.Height, dcSource, 0, 0, SourceCopy);

            // Release GDI and GDI+ objects
            gfx.ReleaseHdc(dcDestination);
            ReleaseDC(windowHandle, dcSource);

            return bmpScreenShot;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Return if image is a blank image
        /// </summary>
        /// <param name="image">Image to check</param>
        /// <returns>True if image is blank (all black or all white)</returns>
        private static ImageBlank CheckBlankImage(Image image)
        {
            // Just check a thumbnail of the image to save time
            Bitmap thumbnail = new Bitmap(image);

            // Check if every pixel is the same color
            Color baseColor = thumbnail.GetPixel(0, 0);

            // If first pixel is not black, white, transparent or blank, the image is likely okay
            if ((baseColor != BlankColor) && (baseColor != Color.Black) && (baseColor != Color.White))
            {
                return ImageBlank.NonBlank;
            }

            // Cycle through the image pixel by pixel looking for a different color
            for (int imageX = 9; imageX < thumbnail.Width; imageX += 10)
            {
                for (int imageY = 9; imageY < thumbnail.Height; imageY += 10)
                {
                    if (thumbnail.GetPixel(imageX, imageY) != baseColor)
                    {
                        return ImageBlank.NonBlank;
                    }
                }
            }

            // Image is all one color, return black, white or empty
            if (baseColor == BlankColor)
            {
                return ImageBlank.Empty;
            }
            else if (baseColor == Color.White)
            {
                return ImageBlank.White;
            }
            else
            {
                return ImageBlank.Black;
            }
        }

        /// <summary>
        /// Get a file name that doesn't already exist
        /// </summary>
        /// <param name="fileName">Unmodified file name without extension</param>
        /// <returns>Unique file name based on parameter</returns>
        private static string GetUniqueFileName(string targetPath, string fileName)
        {
            string fullFileName = null;

            if (!String.IsNullOrEmpty(targetPath) && !String.IsNullOrEmpty(fileName))
            {
                int count = 0;

                fullFileName = Path.Combine(targetPath, fileName + ImageFileExtension);

                // Get unique file name
                while (File.Exists(fullFileName))
                {
                    fullFileName = Path.Combine(targetPath, fileName + (count++) + ImageFileExtension);

                    // If we somehow cap out the number of integers (really?) just use a guid
                    if (count == int.MaxValue)
                    {
                        fullFileName = Path.Combine(targetPath, fileName + Guid.NewGuid().ToString() + ImageFileExtension);
                    }
                }
            }

            return fullFileName;
        }

        /// <summary>
        /// Write a message to the middle of the image
        /// </summary>
        /// <param name="image">Image to write to</param>
        /// <param name="message">Message to write</param>
        private static void WriteToImage(ref Image image, string message, Brush messageColor)
        {
            if (!String.IsNullOrEmpty(message))
            {
                // Find the center of the image
                Point location = new Point(image.Width / 2, image.Height / 2);

                Graphics graphics = Graphics.FromImage(image);
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Center the text
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;

                // Write the string to the image
                graphics.DrawString(message, new Font(FontFamily.GenericSansSerif, (float)MessageFontSize, FontStyle.Bold), messageColor, location, format);
                graphics.Dispose();
            }
        }

        #endregion

        #region Imported Objects

        private class Win32APIDataTypes
        {
            //In theory, C# will properly marshal rectangles.
            //Theory often differens from reality.
            [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
            public struct RECT
            {
                /// <summary>
                /// This indicates the position of the rectangle
                /// </summary>
                public int Left;
                /// <summary>
                /// This indicates the position of the rectangle
                /// </summary>
                public int Top;
                /// <summary>
                /// This indicates the position of the rectangle
                /// </summary>
                public int Right;
                /// <summary>
                /// This indicates the position of the rectangle
                /// </summary>
                public int Bottom;
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hwnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern UInt64 BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, Int32 dwRop);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref Win32APIDataTypes.RECT lpRect);

        #endregion
    }
#else

    internal sealed class ScreenCapture
    {
        /// <summary>
        /// Captures the Screen Shot in a file
        /// </summary>
        /// <returns>name of the file in which the screen shot was captured</returns>
        internal static string CaptureScreenshot()
        {
            return CaptureScreenshotWithFileName(Environment.CurrentDirectory, DateTime.Now.ToUniversalTime().ToFileTimeUtc().ToString());
        }

        /// <summary>
        /// Called when needed to create screenshot
        /// </summary>
        /// <param name="targetDir">dir to save the file</param>
        /// <param name="fileName">This is the file name that we would like to save.</param>
        /// <returns>name of the image file.</returns>
        internal static string CaptureScreenshotWithFileName(string targetDir, string fileName)
        {
            throw new NotImplementedException();
            return fileName;

        }

        /// <summary>
        /// Captures the Screen shot in a specified file
        /// </summary>
        /// <param name="filename">name of the file to save the screenshot</param>
        internal static void CaptureScreenShot(String filename)
        {
            throw new NotImplementedException();
        }
    }
#endif
}
