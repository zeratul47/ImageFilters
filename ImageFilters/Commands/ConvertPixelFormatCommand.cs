﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFilters.Commands
{
    public class ConvertPixelFormatCommand : ICommand
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

        PixelFormat _targetPixelFormat;
        /// <summary>
        /// Target PixelFormat for result image.
        /// </summary>
        public PixelFormat TargetPixelFormat
        {
            get
            {
                return _targetPixelFormat;
            }
            set 
            {
                _targetPixelFormat = value;
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


        #region METHODS

        /// <summary>
        /// Creates convert pixel format command object.
        /// </summary>
        /// <param name="inputImage">The image to convert.</param>
        public ConvertPixelFormatCommand(Bitmap inputImage)
        {
            _inputImage = inputImage;
        }

        /// <summary>
        /// Executes convert pixel format command.
        /// </summary>
        public void Execute()
        {
            // clone input image
            _resultImage = new Bitmap(_inputImage.Width, _inputImage.Height, TargetPixelFormat);

            // read bitmap to memory
            BitmapData resData = _resultImage.LockBits(
                new Rectangle(0, 0, _resultImage.Width, _resultImage.Height),
                ImageLockMode.ReadWrite,
                _resultImage.PixelFormat);

            BitmapData srcData = _inputImage.LockBits(
                new Rectangle(0, 0, _inputImage.Width, _inputImage.Height),
                ImageLockMode.ReadWrite,
                _inputImage.PixelFormat);

            try
            {
                switch (_resultImage.PixelFormat)
                {
                    case PixelFormat.Format16bppGrayScale:
                        //Execute16gray(srcData);
                        break;

                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                    case PixelFormat.Format32bppArgb:
                        Execute24rgb(srcData, resData);
                        break;
                }
            }
            // if error happend
            catch (InvalidOperationException ex)
            {
                // delete result
                _resultImage.UnlockBits(resData);
                _resultImage.Dispose();
                _resultImage = null;

                // save error
                _error = ex;
            }
            finally
            {
                _inputImage.UnlockBits(srcData);
                if (_resultImage != null)
                    _resultImage.UnlockBits(resData);
            }
        }


        #region PRIVATE

        private void Execute16gray(BitmapData srcData)
        {

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

        private void Execute24rgb(BitmapData srcData, BitmapData resData)
        {
            unsafe
            {
                // get bytes per pixel src
                int bytesPerPixelSrc = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
                // get image height
                int heightInPixels = srcData.Height;
                // get image width in bytes
                int widthInBytesSrc = srcData.Width * bytesPerPixelSrc;
                // get pointer to the first byte
                byte* ptrFirstPixelSrc = (byte*)srcData.Scan0;

                switch (resData.PixelFormat)
                {
                    case PixelFormat.Format16bppGrayScale:
                        //Execute16gray(srcData);
                        break;
                    case PixelFormat.Format16bppArgb1555:
                    case PixelFormat.Format16bppRgb555:
                        break;
                    case PixelFormat.Format16bppRgb565:
                        break;
                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                    case PixelFormat.Format32bppArgb:
                        // get bytes per pixel result
                        int bytesPerPixelRes = Image.GetPixelFormatSize(resData.PixelFormat) / 8;
                        // get pointer to the first byte result 
                        byte* ptrFirstPixelRes = (byte*)resData.Scan0;

                        // for each row
                        for (int y = 0; y < heightInPixels; y++)
                        {
                            // get pointer to current row
                            byte* currentLineSrc = ptrFirstPixelSrc + (y * srcData.Stride);
                            byte* currentLineRes = ptrFirstPixelRes + (y * resData.Stride);
                            // for each pixel in row
                            for (int xSrc = 0, xRes = 0;
                                xSrc < widthInBytesSrc;
                                xRes += bytesPerPixelRes, xSrc += bytesPerPixelSrc)
                            {
                                // get pixel value
                                currentLineSrc[xRes] = currentLineSrc[xSrc];
                                currentLineSrc[xRes + 1] = currentLineSrc[xSrc + 1];
                                currentLineSrc[xRes + 2] = currentLineSrc[xSrc + 2];
                            }
                        }
                        break;
                    case PixelFormat.Format48bppRgb:
                    case PixelFormat.Format64bppArgb:

                        break;
                    case PixelFormat.Format1bppIndexed:
                    case PixelFormat.Format4bppIndexed:
                    case PixelFormat.Format8bppIndexed:
                        break;
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