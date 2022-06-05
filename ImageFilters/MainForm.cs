using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageFilters
{
    public partial class MainForm : Form
    {
        #region FIELDS

        /// <summary>
        /// File dialog to open image.
        /// </summary>
        OpenFileDialog _openFileDialog;

        /// <summary>
        /// Original image.
        /// </summary>
        Bitmap _originalImage;

        /// <summary>
        /// Original image after filtering.
        /// </summary>
        Bitmap _filteredImage;

        #endregion


        #region METHODS

        #region PUBLIC

        /// <summary>
        /// Initialization of the viewer.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            _openFileDialog = new OpenFileDialog();
            _openFileDialog.Filter = 
                "bitmap files (*.bmp)|*.bmp|" +
                "jpeg files (*.jpg; *.jpeg)|*.jpg; *.jpeg|" +
                "png files (*.png)|*.png|" +
                "tiff files (*.tiff; *.tif)|*.tiff; *.tif|" +
                "All files (*.*)|*.*";
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Inverts image.
        /// </summary>
        /// <param name="inputImage">Image to invert.</param>
        /// <param name="outputImage">Output image.</param>
        private void InvertImage(Bitmap inputImage, out Bitmap outputImage)
        {
            // clone input image
            outputImage = (Bitmap)inputImage.Clone();

            try
            {
                // read bitmap to memory
                BitmapData srcData = outputImage.LockBits(
                    new Rectangle(0, 0, outputImage.Width, outputImage.Height),
                    ImageLockMode.ReadWrite,
                    outputImage.PixelFormat);

                unsafe
                {
                    // get bytes per pixel
                    int bytesPerPixel = Image.GetPixelFormatSize(outputImage.PixelFormat) / 8;
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

                outputImage.UnlockBits(srcData);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Opens an image when "OpenMenuItem" was clicked.
        /// </summary>
        /// <param name="sender">Caller object.</param>
        /// <param name="e">Event information.</param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // if user chose a file
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // if there is previous image
                    if (_originalImage != null)
                        // delete it
                        _originalImage.Dispose();

                    // if there is previous image
                    if (_filteredImage!= null)
                        // delete it
                        _filteredImage.Dispose();

                    // open new image
                    _originalImage = new Bitmap(_openFileDialog.FileName);
                    // set image to the viewer
                    mainPictureBox.Image = _originalImage;
                    originalPictureBox.Image = _originalImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Applyes Invert filter to the image.
        /// </summary>
        /// <param name="sender">Caller object.</param>
        /// <param name="e">Event information.</param>
        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvertImage(_originalImage, out _filteredImage);

            mainPictureBox.Image = _filteredImage;
            filteredPictureBox.Image = _filteredImage;
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox sourceBox = sender as PictureBox;
            if (sourceBox != mainPictureBox && sourceBox != null)
                mainPictureBox.Image = sourceBox.Image;
        }

        #endregion

        #endregion

    }
}
