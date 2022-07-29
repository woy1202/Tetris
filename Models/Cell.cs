using System.Drawing;

namespace Tetris.Models
{
    public class Cell
    {
        private int _row;
        private int _column;
        private Color _color;
        private string _imgPath;
        private int _width;
        private int _height;

        public Color BgColor
        {
            get => _color;
            private set => _color = value; 
        }

        public string BgImgPath
        {
            get => _imgPath; 
            private set => _imgPath = value; 
        }

        public int RowIndex
        {
            get => _row; 
            private set => _row = value; 
        }

        public int ColumnIndex
        {
            get => _column; 
            private set => _column = value; 
        }

        public int Width
        {
            get => _width;
            private set => _width = value;
        }

        public int Height
        {
            get => _height;
            private set => _height = value;
        }

        public bool IsEmpty
        {
            get => BgColor == Color.Empty && string.IsNullOrEmpty(BgImgPath); 
        }

        public Cell(int width, int height, int x, int y, Color color, string bgImgPath)
        {
            _width = width;
            _height = height;
            _row = x;
            _column = y;
            _color = color;
            _imgPath = bgImgPath;
        }
    }
}
