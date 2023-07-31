using System.Collections.Generic;

namespace ImageFilters.Commands.Utils
{
    /// <summary>
    /// Comparator for <see cref="MedianCut"/> algorithm, to compare the color values only by given channel.
    /// </summary>
    internal class MedianCutComparerArr : IComparer<int>
    {

        #region PROPERTIES
        
        int _channelID = 0;
        /// <summary>
        /// The channel, which should be used for comparison.
        /// </summary>
        public int ChannelID
        { 
            get { return _channelID; } 
            set { _channelID = value; }
        }

        int[,] _colorData;
        /// <summary>
        /// The linearized color of the image.
        /// </summary>
        public int[,] ColorData
        {
            get { return _colorData; }
            set { _colorData = value; }
        }

        #endregion



        #region METHODS

        /// <summary>
        /// Compares two given values.
        /// </summary>
        /// <param name="x">Index in <see cref="ColorData"/>.</param>
        /// <param name="y">Index in <see cref="ColorData"/>.</param>
        /// <returns>
        /// If color of <paramref name="x"/> index is greater than color of <paramref name="y"/>, then 1.
        /// If color of <paramref name="x"/> index is less than color of <paramref name="y"/>, then -1.
        /// If colors are equal, then .
        /// </returns>
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

        #endregion
    }
}
