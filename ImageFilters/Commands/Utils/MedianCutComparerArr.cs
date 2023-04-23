using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageFilters.Commands.Utils
{
    internal class MedianCutComparerArr : IComparer<int>
    {
        int _channelID = 0;
        int[,] _colorData;

        public int ChannelID
        { 
            get { return _channelID; } 
            set { _channelID = value; }
        }

        public int[,] ColorData
        {
            get { return _colorData; }
            set { _colorData = value; }
        }

        public int Compare(int x, int y)
        {
            if (_colorData[x, _channelID] < _colorData[y, _channelID])
            {
                return -1;
            }
            else if (_colorData[x, _channelID] > _colorData[y, _channelID])
            {
                return 1;
            }
            return 0;
        }
    }
}
