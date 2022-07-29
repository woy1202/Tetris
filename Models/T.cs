using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tetris.Interfaces;
using Tetris.Utils;
using static Tetris.Utils.Delegates;

namespace Tetris.Models
{
    public class T : ITransformer
    {
        private BaseBuilder builder;
        private readonly int CellWidth;
        private readonly int CellHeight;
        private readonly Color CellColor;
        private readonly string CellBgImgPath;
        //private Control parentControl;
        private Stage stage;
        private List<Control> cellList;
        private int _x;
        private int _y;

        public int GetWidth() => builder.Matrix.GetLength(1) * CellWidth;

        public int GetHeight() => builder.Matrix.GetLength(0) * CellHeight;

        public Color GetColor() => CellColor;

        public Cell[,] GetCells() => builder.Matrix;

        public List<Control> GetCellList() => cellList;

        public T()
        {
            CellWidth = 30;
            CellHeight = 30;
            CellColor = Color.Blue;
            CellBgImgPath = null;
            //parentControl = null;
            cellList = new List<Control>();
        }

        public void Init(TransformState state)
        {
            builder = new BaseBuilder(TetrisModelType.T, matrix =>
            {
                if (matrix == null || matrix.Length == 0) return;
                switch (state)
                {
                    case TransformState.Left:
                        LeftState(matrix);
                        break;
                    case TransformState.Top:
                        TopState(matrix);
                        break;
                    case TransformState.Right:
                        RightState(matrix);
                        break;
                    case TransformState.Bottom:
                        BottomState(matrix);
                        break;
                }
            });
        }

        private void LeftState(Cell[,] matrix)
        {
            var rowLength = matrix.GetLength(0);
            var columnLength = matrix.GetLength(1);
            var theX = 1; 
            var theY = 2;
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < columnLength; j++)
                {
                    if (j == 1)
                    {
                        matrix[i, j] = new Cell(CellWidth, CellHeight, i, j, CellColor, CellBgImgPath);
                    }
                }
            }
            matrix[theX, theY] = new Cell(CellWidth, CellHeight, theX, theY, CellColor, CellBgImgPath);
        }

        private void RightState(Cell[,] matrix)
        {
            var rowLength = matrix.GetLength(0);
            var columnLength = matrix.GetLength(1);
            var theX = 1;
            var theY = 0;
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < columnLength; j++)
                {
                    if (j == 1)
                    {
                        matrix[i, j] = new Cell(CellWidth, CellHeight, i, j, CellColor, CellBgImgPath);
                    }
                }
            }
            matrix[theX, theY] = new Cell(CellWidth, CellHeight, theX, theY, CellColor, CellBgImgPath);
        }

        private void TopState(Cell[,] matrix)
        {
            var rowLength = matrix.GetLength(0);
            var columnLength = matrix.GetLength(1);
            var theX = 2;
            var theY = 1;
            for (int i = 0; i < rowLength; i++)
            {
                if (i == 1)
                {
                    for (int j = 0; j < columnLength; j++)
                    {
                        matrix[i, j] = new Cell(CellWidth, CellHeight, i, j, CellColor, CellBgImgPath);
                    }
                }
            }
            matrix[theX, theY] = new Cell(CellWidth, CellHeight, theX, theY, CellColor, CellBgImgPath);
        }

        private void BottomState(Cell[,] matrix)
        {
            var rowLength = matrix.GetLength(0);
            var columnLength = matrix.GetLength(1);
            var theX = 0;
            var theY = 1;
            for (int i = 0; i < rowLength; i++)
            {
                if (i == 1)
                {
                    for (int j = 0; j < columnLength; j++)
                    {
                        matrix[i, j] = new Cell(CellWidth, CellHeight, i, j, CellColor, CellBgImgPath);
                    }
                }
            }
            matrix[theX, theY] = new Cell(CellWidth, CellHeight, theX, theY, CellColor, CellBgImgPath);
        }

        public Point GetLocation()
        {
            return new Point(_x, _y);
        }

        public void SetLocation(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public void DisplayOn(Stage stage)
        {
            if (stage.IsGameover) return;

            stage.OnBottom = false;
            this.stage = stage;
            this.stage.SetUp(this, builder.Matrix, CellColor, cellList);
        }

        public bool IsGameOver() => stage.IsGameover;

        public bool IsOnBottom() => stage.OnBottom;

        public void Transform()
        {
            stage.AddEventQueue(new QueueInfo { MethodType = 4 });
        }

        public void MoveDown()
        {
            stage.AddEventQueue(new QueueInfo { MethodType = 3 });
        }

        public void MoveLeft()
        {
            stage.AddEventQueue(new QueueInfo { MethodType = 1 });
        }

        public void MoveRight()
        {
            stage.AddEventQueue(new QueueInfo { MethodType = 2 });
        }
    }
}
