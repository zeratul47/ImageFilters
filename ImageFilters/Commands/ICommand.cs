using System.Drawing;
using System.Drawing.Imaging;

namespace ImageFilters.Commands
{
    /// <summary>
    /// The general Command interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Supported pixel formats for the command.
        /// </summary>
        public PixelFormat SupportPixelFormat
        {
            get;
        }

        /// <summary>
        /// The result image of the command.
        /// </summary>
        public Bitmap Result
        {
            get;
        }

        /// <summary>
        /// The main function for command execution.
        /// </summary>
        public void Execute();
    }
}
