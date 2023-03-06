using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageFilters.Commands.Utils
{
    internal class MedianCutComparer : IComparer<Tuple<Color, List<int>>>
    {
        int _channelID = 0;

        public int ChannelID
        { 
            get { return _channelID; } 
            set { _channelID = value; }
        }

        public int Compare(Tuple<Color, List<int>> x, Tuple<Color, List<int>> y)
        {
            switch(ChannelID)
            {
                case 0:
                    return CompateNumbers(x.Item1.A, y.Item1.A);
                case 1:
                    return CompateNumbers(x.Item1.R, y.Item1.R);
                case 2:
                    return CompateNumbers(x.Item1.G, y.Item1.G);
                case 3:
                    return CompateNumbers(x.Item1.B, y.Item1.B);
                default:
                    throw new ArgumentException("Unknown channel");
            }
        }

        private int CompateNumbers(int x, int y)
        {
            if (x < y)
            {
                return -1;
            }
            else if (x > y)
            {
                return 1;
            }
            return 0;
        }
    }
}
