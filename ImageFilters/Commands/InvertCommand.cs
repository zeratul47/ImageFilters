using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageFilters.Commands
{
    /// <summary>
    /// The command inverts image colors.
    /// </summary>
    public class InvertCommand : ICommand
    {

        #region FIELDS

        /// <summary>
        /// Image to invert.
        /// </summary>
        Bitmap _inputImage;

        /// <summary>
        /// Result of image invertion.
        /// </summary>
        Bitmap _resultImage;

        /// <summary>
        /// Exception of command execution.
        /// </summary>
        Exception _error;

        #endregion


        #region PROPERTIES

        /// <summary>
        /// Supported PixelFormats by command.
        /// </summary>
        public PixelFormat SupportPixelFormat
        {
            get
            {
                return PixelFormat.Format16bppGrayScale
                    | PixelFormat.Format16bppArgb1555
                    | PixelFormat.Format16bppRgb555
                    | PixelFormat.Format16bppRgb565
                    | PixelFormat.Format24bppRgb
                    | PixelFormat.Format32bppArgb
                    | PixelFormat.Format32bppRgb
                    | PixelFormat.Format48bppRgb
                    | PixelFormat.Format64bppArgb
                    | PixelFormat.Format1bppIndexed
                    | PixelFormat.Format4bppIndexed
                    | PixelFormat.Format8bppIndexed;
            }
        }

        /// <summary>
        /// The result of command.
        /// </summary>
        public Bitmap Result
        {
            get
            {
                return _resultImage;
            }
        }

        /// <summary>
        /// Exception, during command execution.
        /// </summary>
        public Exception Error
        {
            get
            {
                return _error;
            }
        }

        #endregion



        #region Constructors

        /// <summary>
        /// Creates Invert command object.
        /// </summary>
        /// <param name="inputImage">The image to invert.</param>
        public InvertCommand(Bitmap inputImage)
        {
            _inputImage = inputImage;
        }

        #endregion



        #region METHODS

        /// <summary>
        /// Executes invert command.
        /// </summary>
        public void Execute()
        {
            // clone input image
            _resultImage = (Bitmap)_inputImage.Clone();

            // read bitmap to memory
            BitmapData srcData = _resultImage.LockBits(
                new Rectangle(0, 0, _resultImage.Width, _resultImage.Height),
                ImageLockMode.ReadWrite,
                _resultImage.PixelFormat);

            try
            {
                switch (_resultImage.PixelFormat)
                {
                    case PixelFormat.Format16bppGrayScale:
                        Execute16gray(srcData);
                        break;

                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                    case PixelFormat.Format32bppArgb:
                        //var watch = new System.Diagnostics.Stopwatch();

                        //watch.Start();
                        Execute24and32argb(srcData);
                        //watch.Stop();

                        //Debug.WriteLine($"Execution Time: {watch.ElapsedMilliseconds / 1000.0} s");
                        break;
                }

            }
            // if error happend
            catch (InvalidOperationException ex)
            {
                // delete result
                _resultImage.Dispose();
                _resultImage = null;

                // save error
                _error = ex;
            }
            finally
            {
                _resultImage.UnlockBits(srcData);
            }
        }


        #region PRIVATE

        /// <summary>
        /// Inverts 16Gray Bitmap.
        /// </summary>
        /// <param name="srcData">The bitmap to invert.</param>
        private void Execute16gray(BitmapData srcData)
        {
            unsafe
            {
                // get bytes per pixel
                int bytesPerPixel = Image.GetPixelFormatSize(_resultImage.PixelFormat) / 16;
                // get image height
                int heightInPixels = srcData.Height;
                // get image width in bytes
                int widthInBytes = srcData.Width * bytesPerPixel;
                // get pointer to the first byte
                Int16* ptrFirstPixel = (Int16*)srcData.Scan0;

                // memory for pixel values
                Int16 oldValue = 0;

                // for each row
                for (int y = 0; y < heightInPixels; y++)
                {
                    // get pointer to current row
                    Int16* currentLine = ptrFirstPixel + (y * srcData.Stride);
                    // for each pixel in row
                    for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                    {
                        // get pixel value
                        oldValue = currentLine[x];
                        // bitwise negation
                        currentLine[x] = (byte)~oldValue;
                    }
                }
            }
        }

        private void Execute16argb1555(BitmapData srcData)
        {
            
        }

        private void Execute16rgb555(BitmapData srcData)
        {

        }

        private void Execute16rgb565(BitmapData srcData)
        {

        }

        /// <summary>
        /// Inverts RGB24, RGB32, RGBA32 Bitmap.
        /// </summary>
        /// <param name="srcData">The bitmap to invert.</param>
        private void Execute24and32argb(BitmapData srcData)
        {
            unsafe
            {
                // get bytes per pixel
                int bytesPerPixel = Image.GetPixelFormatSize(_resultImage.PixelFormat) / 8;
                // get image height
                int heightInPixels = srcData.Height;
                // get image width in bytes
                int widthInBytes = srcData.Width * bytesPerPixel;
                // get pointer to the first byte
                byte* ptrFirstPixel = (byte*)srcData.Scan0;

                // memory for pixel values
                byte oldBlue = 0;
                byte oldGreen = 0;
                byte oldRed = 0;

                // for each row
                for (int y = 0; y < heightInPixels; y++)
                {
                    // get pointer to current row
                    byte* currentLine = ptrFirstPixel + (y * srcData.Stride);
                    // for each pixel in row
                    for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                    {
                        // get pixel value
                        oldBlue = currentLine[x];
                        oldGreen = currentLine[x + 1];
                        oldRed = currentLine[x + 2];

                        // calculate new pixel value
                        // casual variant (MaxValue - current)
                        //currentLine[x] = (byte)(255 - oldBlue);
                        //currentLine[x + 1] = (byte)(255 - oldGreen);
                        //currentLine[x + 2] = (byte)(255 - oldRed);

                        // bitwise negation
                        currentLine[x] = (byte)~oldBlue;
                        currentLine[x + 1] = (byte)~oldGreen;
                        currentLine[x + 2] = (byte)~oldRed;

                        // bitwise reading and writing from int32

                        // blue = ((*(int*)ptr)>>24)&0b_1111_1111
                        // green = ((*(int*)ptr)>>16)&0b_1111_1111
                        // red = ((*(int*)ptr)>>8)&0b_1111_1111
                        // Alpha = ((*(int*)ptr))&0b_1111_1111

                        // int32value =  blue<<24 | green << 16 | red <<8 |alpha
                    }
                }
            }
        }

        private void Execute48rgb(BitmapData srcData)
        {

        }

        private void Execute64rgb(BitmapData srcData)
        {

        }

        private void Execute1indexed(BitmapData srcData)
        {

        }

        private void Execute4indexed(BitmapData srcData)
        {

        }

        private void Execute8indexed(BitmapData srcData)
        {

        }

        #endregion

        #endregion

    }
}
