using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFilters.Commands
{
    public interface ICommand
    {
        
        public PixelFormat SupportPixelFormat
        {
            get;
        }

        public Bitmap Result
        {
            get;
        }


        public void Execute();
    }
}
