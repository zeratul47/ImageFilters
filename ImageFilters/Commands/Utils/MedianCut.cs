using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageFilters.Commands.Utils
{
    internal class MedianCut
    {
        #region FIELDS

        int _quantizationNumber = 0;

        List<Tuple<Color, List<int>>> _colorDataResult;

        int[,] _colorDataSourceArr;
        int[] _colorDataSourceArrInds;

        MedianCutComparerArr _comparerArr;

        List<Tuple<int, int>> _indexes;

        #endregion


        #region PROPERTIES

        public int QuantizationNumber
        {
            get { return _quantizationNumber; }
            set 
            { 
                _quantizationNumber = value;
            }
        }


        public int[,] ColorDataSource
        {
            get { return _colorDataSourceArr; }
            set { _colorDataSourceArr = value; }
        }

        public List<Tuple<Color, List<int>>> ColorDataResult
        {
            get { return _colorDataResult; }
        }

        #endregion


        #region METHODS

        #region PUBLIC

        public MedianCut(int[,] data, int quantizationNumber)
        {
            ColorDataSource = data;
            QuantizationNumber = quantizationNumber;

            _comparerArr = new MedianCutComparerArr();

            int capacity = (int)Math.Pow(2, quantizationNumber);
            _indexes = new List<Tuple<int, int>>(capacity);
            _colorDataResult = new List<Tuple<Color, List<int>>>();
        }

        public void Execute()
        {
            if (QuantizationNumber < 1)
                return;

            int length = _colorDataSourceArr.GetLength(0);
            _colorDataSourceArrInds = new int[length];

            for (int i = 0; i < length; i++)
            {
                _colorDataSourceArrInds[i] = i;
            }
            _comparerArr.ColorData = _colorDataSourceArr;
            ExecuteCut(0, length, 1);

            _colorDataResult.Clear();
            
            int[] indexesToMap = new int[1];
            
            foreach (Tuple<int, int> indexPair in _indexes)
            {
                int avgA = 0;
                int avgR = 0;
                int avgG = 0;
                int avgB = 0;
                int size = indexPair.Item2 - indexPair.Item1;
                Array.Resize<int>(ref indexesToMap, size);
                int ind = 0;
                for (int i = indexPair.Item1; i < indexPair.Item2; i++)
                {
                    avgA += _colorDataSourceArr[_colorDataSourceArrInds[i], 0];
                    avgR += _colorDataSourceArr[_colorDataSourceArrInds[i], 1];
                    avgG += _colorDataSourceArr[_colorDataSourceArrInds[i], 2];
                    avgB += _colorDataSourceArr[_colorDataSourceArrInds[i], 3];
                    indexesToMap[ind] = _colorDataSourceArr[_colorDataSourceArrInds[i], 4];
                    ind++;
                }

                avgA /= size;
                avgR /= size;
                avgG /= size;
                avgB /= size;

                _colorDataResult.Add(
                    Tuple.Create(
                        Color.FromArgb(avgA, avgR, avgG, avgB),
                        new List<int>(indexesToMap)));
            }
        }

        public void ExecuteCut(int startIndex, int endIndex, int step)
        {
            if (step > QuantizationNumber + 1)
                return;

            if (step == QuantizationNumber + 1)
                _indexes.Add(Tuple.Create(startIndex, endIndex));

            Color ranges = GetRanges(_colorDataSourceArr, startIndex, endIndex);

            if (ranges.G >= ranges.B && ranges.G >= ranges.R)
            {
                _comparerArr.ChannelID = 2;
            }
            else if(ranges.B >= ranges.G && ranges.B >= ranges.R)
            {
                _comparerArr.ChannelID = 3;
            }
            else
            {
                _comparerArr.ChannelID = 1;
            }
            Array.Sort(_colorDataSourceArrInds, startIndex, endIndex - startIndex, _comparerArr);

            int midIndex = (endIndex + startIndex) / 2;
            ExecuteCut(startIndex, midIndex, step + 1);
            ExecuteCut(midIndex, endIndex, step + 1);
        }

        #endregion

        #region PRIVATE

        private Color GetRanges(
            int[,] colorData,
            int startIndex,
            int endIndex
            )
        {
            int minR = colorData[_colorDataSourceArrInds[startIndex], 1];
            int minG = colorData[_colorDataSourceArrInds[startIndex], 2];
            int minB = colorData[_colorDataSourceArrInds[startIndex], 3];

            int maxR = colorData[_colorDataSourceArrInds[startIndex], 1];
            int maxG = colorData[_colorDataSourceArrInds[startIndex], 2];
            int maxB = colorData[_colorDataSourceArrInds[startIndex], 3];

            for (int i = startIndex; i < endIndex; i++)
            {
                // get min values
                if (minR > colorData[_colorDataSourceArrInds[i], 1])
                {
                    minR = colorData[_colorDataSourceArrInds[i], 1];
                }
                if (minG > colorData[_colorDataSourceArrInds[i], 2])
                {
                    minG = colorData[_colorDataSourceArrInds[i], 2];
                }
                if (minB > colorData[_colorDataSourceArrInds[i], 3])
                {
                    minB = colorData[_colorDataSourceArrInds[i], 3];
                }

                // get max values
                if (maxR < colorData[_colorDataSourceArrInds[i], 1])
                {
                    maxR = colorData[_colorDataSourceArrInds[i], 1];
                }
                if (maxG < colorData[_colorDataSourceArrInds[i], 2])
                {
                    maxG = colorData[_colorDataSourceArrInds[i], 2];
                }
                if (maxB < colorData[_colorDataSourceArrInds[i], 3])
                {
                    maxB = colorData[_colorDataSourceArrInds[i], 3];
                }
            }

            // return ranges
            return Color.FromArgb(255, maxR - minR, maxG - minG, maxB - minB);
        }

        #endregion

        #endregion
    }
}
