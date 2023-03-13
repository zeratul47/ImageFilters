using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageFilters.Commands.Utils
{
    internal class MedianCut
    {
        #region FIELDS

        int _quantizationNumber = 0;

        List<Tuple<Color, List<int>>> _colorDataSource;

        List<Tuple<Color, List<int>>> _colorDataResult;

        MedianCutComparer _comparer;

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


        public List<Tuple<Color, List<int>>> ColorDataSource
        {
            get { return _colorDataSource; }
            set { _colorDataSource = value; }
        }

        public List<Tuple<Color, List<int>>> ColorDataResult
        {
            get { return _colorDataResult; }
        }

        #endregion


        #region METHODS

        #region PUBLIC

        public MedianCut(List<Tuple<Color, List<int>>> data, int quantizationNumber)
        {
            ColorDataSource = data;
            QuantizationNumber = quantizationNumber;
            _comparer = new MedianCutComparer();
            int capacity = (int)Math.Pow(2, quantizationNumber);
            _indexes = new List<Tuple<int, int>>(capacity);
            _colorDataResult = new List<Tuple<Color, List<int>>>();
        }

        public void Execute()
        {
            if (QuantizationNumber < 1)
                return;

            ExecuteCut(0, _colorDataSource.Count, 1);

            _colorDataResult.Clear();
            List<int> indexesToMap = new List<int>();
            foreach (Tuple<int, int> indexPair in _indexes)
            {
                int avgA = 0;
                int avgR = 0;
                int avgG = 0;
                int avgB = 0;
                indexesToMap.Clear();

                for (int i = indexPair.Item1; i < indexPair.Item2; i++)
                {
                    avgA += _colorDataSource[i].Item1.A;
                    avgR += _colorDataSource[i].Item1.R;
                    avgG += _colorDataSource[i].Item1.G;
                    avgB += _colorDataSource[i].Item1.B;
                    indexesToMap.AddRange(_colorDataSource[i].Item2);
                }

                avgA /= (indexPair.Item2 - indexPair.Item1);
                avgR /= (indexPair.Item2 - indexPair.Item1);
                avgG /= (indexPair.Item2 - indexPair.Item1);
                avgB /= (indexPair.Item2 - indexPair.Item1);

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

            Color ranges = GetRanges(_colorDataSource, startIndex, endIndex);

            if (ranges.R > ranges.B && ranges.R > ranges.G)
            {
                _comparer.ChannelID = 1;
                _colorDataSource.Sort(startIndex, endIndex - startIndex, _comparer);
            }
            else if (ranges.G > ranges.B && ranges.G > ranges.R)
            {
                _comparer.ChannelID = 2;
                _colorDataSource.Sort(startIndex, endIndex - startIndex, _comparer);
            }
            else
            {
                _comparer.ChannelID = 3;
                _colorDataSource.Sort(startIndex, endIndex - startIndex, _comparer);
            }

            int midIndex = (endIndex + startIndex) / 2;
            ExecuteCut(startIndex, midIndex, step + 1);
            ExecuteCut(midIndex, endIndex, step + 1);
        }

        #endregion

        #region PRIVATE

        private Color GetRanges(
            List<Tuple<Color, List<int>>> colorData,
            int startIndex,
            int endIndex)
        {
            int minA = colorData[startIndex].Item1.A;
            int minR = colorData[startIndex].Item1.R;
            int minG = colorData[startIndex].Item1.G;
            int minB = colorData[startIndex].Item1.B;

            int maxA = colorData[startIndex].Item1.A;
            int maxR = colorData[startIndex].Item1.R;
            int maxG = colorData[startIndex].Item1.G;
            int maxB = colorData[startIndex].Item1.B;

            for (int i = startIndex;  i < endIndex; i++)
            {
                // get min values
                if (minA > colorData[i].Item1.A)
                {
                    minA = colorData[i].Item1.A;
                }
                if (minR > colorData[i].Item1.R)
                {
                    minR = colorData[i].Item1.R;
                }
                if (minG > colorData[i].Item1.G)
                {
                    minG = colorData[i].Item1.G;
                }
                if (minB > colorData[i].Item1.B)
                {
                    minB = colorData[i].Item1.B;
                }

                // get max values
                if (maxA < colorData[i].Item1.A)
                {
                    maxA = colorData[i].Item1.A;
                }
                if (maxR < colorData[i].Item1.R)
                {
                    maxR = colorData[i].Item1.R;
                }
                if (maxG < colorData[i].Item1.G)
                {
                    maxG = colorData[i].Item1.G;
                }
                if (maxB < colorData[i].Item1.B)
                {
                    maxB = colorData[i].Item1.B;
                }
            }

            // return ranges
            return Color.FromArgb(maxA - minA, maxR - minR, maxG - minG, maxB - minB);
        }

        #endregion

        #endregion
    }
}
