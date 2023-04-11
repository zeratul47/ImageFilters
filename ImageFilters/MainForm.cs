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

using ImageFilters.Commands;

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

            //openToolStripMenuItem_Click(this, new EventArgs());
            //changePixelFormatToolStripMenuItem_Click(this, new EventArgs());
            //invertToolStripMenuItem_Click(this, new EventArgs());
        }

        #endregion


        #region PRIVATE

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

                    filteredPictureBox.Image = null;
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
            // if there is already filtered image
            if (_filteredImage != null)
                // delete it
                _filteredImage.Dispose();

            // create command
            InvertCommand invertCommand = new InvertCommand(_originalImage);
            // execute it
            invertCommand.Execute();

            // if there is error
            if (invertCommand.Error != null)
            {
                MessageBox.Show(invertCommand.Error.Message);
                // return
                return;
            }

            // set filter image
            _filteredImage = invertCommand.Result;

            mainPictureBox.Image = _filteredImage;
            filteredPictureBox.Image = _filteredImage;
        }


        private void changePixelFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertPixelFormatCommand changeFormatCommad = new ConvertPixelFormatCommand(_originalImage);
            changeFormatCommad.TargetPixelFormat = PixelFormat.Format8bppIndexed;
            changeFormatCommad.Execute();
            
            // if there is error
            if (changeFormatCommad.Error != null)
            {
                MessageBox.Show(changeFormatCommad.Error.Message);
                // return
                return;
            }

            // set filter image
            _filteredImage = changeFormatCommad.Result;

            mainPictureBox.Image = _filteredImage;
            filteredPictureBox.Image = _filteredImage;
        }

        /// <summary>
        /// Changes image in main picture box by clicking on side once.
        /// </summary>
        /// <param name="sender">Caller object.</param>
        /// <param name="e">Event information.</param>
        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox sourceBox = sender as PictureBox;
            if (sourceBox != mainPictureBox && sourceBox != null)
                mainPictureBox.Image = sourceBox.Image;
        }

        /// <summary>
        /// Changes properties in property grid according to current image.
        /// </summary>
        /// <param name="sender">Caller object.</param>
        /// <param name="e">Event information.</param>
        private void mainPictureBox_Paint(object sender, PaintEventArgs e)
        {
            mainPicturePropertyGrid.SelectedObject = mainPictureBox.Image;
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #endregion

    }
}
