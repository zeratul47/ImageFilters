using System;
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


        #region METHODS

        /// <summary>
        /// Create Invert command object.
        /// </summary>
        /// <param name="inputImage">The image to invert.</param>
        public InvertCommand(Bitmap inputImage)
        {
            _inputImage = inputImage;
        }

        /// <summary>
        /// Executes invert command.
        /// </summary>
        public void Execute()
        {
            // clone input image
            _resultImage = (Bitmap)_inputImage.Clone();

            try
            {
                // read bitmap to memory
                BitmapData srcData = _resultImage.LockBits(
                    new Rectangle(0, 0, _resultImage.Width, _resultImage.Height),
                    ImageLockMode.ReadWrite,
                    _resultImage.PixelFormat);

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
                            currentLine[x] = (byte)(255 - oldBlue);
                            currentLine[x + 1] = (byte)(255 - oldGreen);
                            currentLine[x + 2] = (byte)(255 - oldRed);
                        }
                    }
                }

                _resultImage.UnlockBits(srcData);
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
        }

        #endregion

    }
}
