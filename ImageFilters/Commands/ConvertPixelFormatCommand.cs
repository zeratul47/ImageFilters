using ImageFilters.Commands.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageFilters.Commands
{
    public class ConvertPixelFormatCommand : ICommand
    {
        #region FIELDS

        const byte BIT_PER_BYTE = 8;

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

                    case PixelFormat.Format1bppIndexed:
                        ExecuteIndexed(srcData, resData, 1);
                        break;
                    case PixelFormat.Format4bppIndexed:
                        ExecuteIndexed(srcData, resData, 4);
                        break;
                    case PixelFormat.Format8bppIndexed:
                        ExecuteIndexed(srcData, resData, 8);
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
                int bytesPerPixelSrc = Image.GetPixelFormatSize(srcData.PixelFormat) / BIT_PER_BYTE;
                // get image height
                int heightInPixels = srcData.Height;
                // get image width in bytes
                int widthInBytesSrc = srcData.Width * bytesPerPixelSrc;
                // get pointer to the first byte
                byte* ptrFirstPixelSrc = (byte*)srcData.Scan0;

                switch (srcData.PixelFormat)
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
                        int bytesPerPixelRes = Image.GetPixelFormatSize(resData.PixelFormat) / BIT_PER_BYTE;
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
                                currentLineRes[xRes] = currentLineSrc[xSrc];
                                currentLineRes[xRes + 1] = currentLineSrc[xSrc + 1];
                                currentLineRes[xRes + 2] = currentLineSrc[xSrc + 2];
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

        private void ExecuteIndexed(BitmapData srcData, BitmapData resData, int quantNumber)
        {
            unsafe
            {
                // get bytes per pixel src
                int bytesPerPixelSrc = Image.GetPixelFormatSize(srcData.PixelFormat) / BIT_PER_BYTE;
                // get image height
                int heightInPixels = srcData.Height;
                // get image width in bytes
                int widthInBytesSrc = srcData.Width * bytesPerPixelSrc;
                // get pointer to the first byte
                byte* ptrFirstPixelSrc = (byte*)srcData.Scan0;


                List<Tuple<Color, List<int>>> data = new List<Tuple<Color, List<int>>>(srcData.Height * srcData.Width);

                switch (srcData.PixelFormat)
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
                        // for each row
                        for (int y = 0; y < heightInPixels; y++)
                        {
                            // get pointer to current row
                            byte* currentLineSrc = ptrFirstPixelSrc + (y * srcData.Stride);
                            // for each pixel in row
                            for (int xSrc = 0;
                                xSrc < widthInBytesSrc; xSrc += bytesPerPixelSrc)
                            {
                                List<int> indexes = new List<int>();
                                indexes.Add(y * srcData.Width + xSrc / bytesPerPixelSrc);
                                data.Add(
                                    Tuple.Create(Color.FromArgb(255,
                                    currentLineSrc[xSrc + 2],
                                    currentLineSrc[xSrc + 1],
                                    currentLineSrc[xSrc]), indexes));
                            }
                        }

                        break;
                    case PixelFormat.Format32bppArgb:
                        // for each row
                        for (int y = 0; y < heightInPixels; y++)
                        {
                            // get pointer to current row
                            byte* currentLineSrc = ptrFirstPixelSrc + (y * srcData.Stride);
                            // for each pixel in row
                            for (int xSrc = 0;
                                xSrc < widthInBytesSrc; xSrc += bytesPerPixelSrc)
                            {
                                List<int> indexes = new List<int>();
                                indexes.Add(y * srcData.Width + xSrc / bytesPerPixelSrc);
                                data.Add(
                                    Tuple.Create(Color.FromArgb(currentLineSrc[xSrc + 3],
                                    currentLineSrc[xSrc + 2],
                                    currentLineSrc[xSrc + 1],
                                    currentLineSrc[xSrc]), indexes));
                            }
                        }

                        break;
                    case PixelFormat.Format48bppRgb:
                    case PixelFormat.Format64bppArgb:

                        break;
                    case PixelFormat.Format1bppIndexed:
                        // TODO посмотреть как 1bpp читается
                    case PixelFormat.Format4bppIndexed:
                    case PixelFormat.Format8bppIndexed:
                        break;
                }

                //var watch = new System.Diagnostics.Stopwatch();

                //watch.Start();
                MedianCut medianCut = new MedianCut(data, quantNumber);
                medianCut.Execute();
                //watch.Stop();

                //Debug.WriteLine($"Execution Time: {watch.ElapsedMilliseconds / 1000.0} s");

                switch (resData.PixelFormat)
                {
                    case PixelFormat.Format1bppIndexed:
                        WriteIndex1(resData, medianCut);
                        break;
                    case PixelFormat.Format4bppIndexed:
                        WriteIndex4(resData, medianCut);
                        break;
                    case PixelFormat.Format8bppIndexed:
                        WriteIndex8(resData, medianCut);
                        break;
                }
            }
        }

        /// <summary>
        /// Writes 1 bpp indexed image from the quantization result.
        /// </summary>
        /// <param name="resData">Bitmap data of result image.</param>
        /// <param name="medianCut">Quantization algorithm result.</param>
        private void WriteIndex1(
            BitmapData resData,
            MedianCut medianCut)
        {
            unsafe
            {
                // get pointer to the first byte result 
                byte* ptrFirstPixelRes = (byte*)resData.Scan0;
                // get bytes per pixel src
                int bitsPerPixel = Image.GetPixelFormatSize(resData.PixelFormat);
                // get pixels in one byte
                int pixelsInByte = BIT_PER_BYTE / bitsPerPixel;

                // y index of the image
                int yIndex;
                // x index of the image
                int xIndex;
                // bit number in byte
                int bitNumber;
                // current line of the image
                byte* currentLineRes;

                // get palette of the image
                ColorPalette palette = _resultImage.Palette;

                // for each color in the result
                for (int i = 0; i < medianCut.ColorDataResult.Count; i++)
                {
                    // get color and indecies
                    Tuple<Color, List<int>> colorCut = medianCut.ColorDataResult[i];
                    // write color into palette
                    palette.Entries[i] = colorCut.Item1;

                    // for each pixel of the current color
                    foreach (int linearIndex in colorCut.Item2)
                    {
                        // get indicies of the pixel
                        yIndex = linearIndex / resData.Width;
                        xIndex = linearIndex % resData.Width;
                        bitNumber = xIndex % pixelsInByte;
                        xIndex = xIndex / pixelsInByte;

                        // get pixel values
                        currentLineRes = ptrFirstPixelRes + ((yIndex * resData.Stride) + xIndex);

                        // write pixel data
                        currentLineRes[0] |= (byte)(i << (7 - bitNumber));
                    }
                }
                // write new palette to the result image
                _resultImage.Palette = palette;
            }
        }

        /// <summary>
        /// Writes 4 bpp indexed image from the quantization result.
        /// </summary>
        /// <param name="resData">Bitmap data of result image.</param>
        /// <param name="medianCut">Quantization algorithm result.</param>
        private void WriteIndex4(
            BitmapData resData,
            MedianCut medianCut)
        {
            unsafe
            {
                // get pointer to the first byte result 
                byte* ptrFirstPixelRes = (byte*)resData.Scan0;
                // get bytes per pixel src
                int bitsPerPixel = Image.GetPixelFormatSize(resData.PixelFormat);
                // get pixels in one byte
                int pixelsInByte = BIT_PER_BYTE / bitsPerPixel;

                // y index of the image
                int yIndex;
                // x index of the image
                int xIndex;
                // bit number in byte
                int bitNumber;
                // current line of the image
                byte* currentLineRes;

                // get palette of the image
                ColorPalette palette = _resultImage.Palette;

                // for each color in the result
                for (int i = 0; i < medianCut.ColorDataResult.Count; i++)
                {
                    // get color and indecies
                    Tuple<Color, List<int>> colorCut = medianCut.ColorDataResult[i];
                    // write color into palette
                    palette.Entries[i] = colorCut.Item1;

                    // for each pixel of the current color
                    foreach (int linearIndex in colorCut.Item2)
                    {
                        // get indicies of the pixel
                        yIndex = linearIndex / resData.Width;
                        xIndex = linearIndex % resData.Width;
                        bitNumber = xIndex % pixelsInByte == 0 ? 0 : 4;
                        xIndex = xIndex / pixelsInByte;

                        // get pixel values
                        currentLineRes = ptrFirstPixelRes + ((yIndex * resData.Stride) + xIndex);

                        // write pixel data
                        currentLineRes[0] |= (byte)(i << (4 - bitNumber));
                    }
                }
                // write new palette to the result image
                _resultImage.Palette = palette;
            }
        }

        /// <summary>
        /// Writes 8 bpp indexed image from the quantization result.
        /// </summary>
        /// <param name="resData">Bitmap data of result image.</param>
        /// <param name="medianCut">Quantization algorithm result.</param>
        private void WriteIndex8(
            BitmapData resData,
            MedianCut medianCut)
        {
            unsafe
            {
                // get pointer to the first byte result 
                byte* ptrFirstPixelRes = (byte*)resData.Scan0;
                // get bytes per pixel src
                int bytesPerPixelRes = Image.GetPixelFormatSize(resData.PixelFormat) / BIT_PER_BYTE;
                int widthInBytesRes = resData.Width * bytesPerPixelRes;

                // y index of the image
                int yIndex;
                // x index of the image
                int xIndex;
                // current line of the image
                byte* currentLineRes;

                // get palette of the image
                ColorPalette palette = _resultImage.Palette;

                // for each color in the result
                for (int i = 0; i < medianCut.ColorDataResult.Count; i++)
                {
                    // get color and indecies
                    Tuple<Color, List<int>> colorCut = medianCut.ColorDataResult[i];
                    // write color into palette
                    palette.Entries[i] = colorCut.Item1;

                    // for each pixel of the current color
                    foreach (int linearIndex in colorCut.Item2)
                    {
                        // get indicies of the pixel
                        yIndex = linearIndex / widthInBytesRes;
                        xIndex = linearIndex % widthInBytesRes;

                        // get pointer to current row
                        currentLineRes = ptrFirstPixelRes + (yIndex * resData.Stride);
                        // write pixel data
                        currentLineRes[xIndex] = (byte)i;
                    }
                }

                // write new palette to the result image
                _resultImage.Palette = palette;
            }
        }

        #endregion

        #endregion

    }
}
