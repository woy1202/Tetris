using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tetris.Interfaces;
using Tetris.Utils;
using static Tetris.Utils.Delegates;

namespace Tetris.Models
{
    public class I : ITransformer
    {
        private BaseBuilder builder;
        private readonly int CellWidth;
        private readonly int CellHeight;
        private readonly Color CellColor;
        private readonly string CellBgImgPath;
        //private Control parentControl;
        private List<Control> cellList;
        private Stage stage;
        private int _x;
        private int _y;
        public int GetWidth () => builder.Matrix.GetLength(1) * CellWidth;

        public int GetHeight() => builder.Matrix.GetLength(0) * CellHeight;

        public Color GetColor() => CellColor;

        public Cell[,] GetCells() => builder.Matrix;

        public List<Control> GetCellList() => cellList;

        public I()
        {
            CellWidth = 30;
            CellHeight = 30;
            CellColor = Color.DarkViolet;
            CellBgImgPath = null;
            //parentControl = null;
            cellList = new List<Control>();
        }

        public void Init(TransformState state)
        {
            builder = new BaseBuilder(TetrisModelType.I, matrix =>
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
            var rowCount = matrix.GetLength(0);
            var colCount = matrix.GetLength(1);
            for (int i = 0; i < rowCount; i++)
            {
                for(int j = 0; j < colCount; j++)
                {
                    if (j == 1)
                    {
                        matrix[i, j] = new Cell(CellWidth, CellHeight, i, j, CellColor, CellBgImgPath);
                    }
                }
            }
        }

        private void RightState(Cell[,] matrix)
        {
            var rowCount = matrix.GetLength(0);
            var colCount = matrix.GetLength(1);
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    if (j == 2)
                    {
                        matrix[i, j] = new Cell(CellWidth, CellHeight, i, j, CellColor, CellBgImgPath);
                    }
                }
            }
        }

        private void TopState(Cell[,] matrix)
        {
            var rowCount = matrix.GetLength(0);
            var colCount = matrix.GetLength(1);
            for (int i = 0; i < rowCount; i++)
            {
                if (i == 1)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        matrix[i, j] = new Cell(CellWidth, CellHeight, i, j, CellColor, CellBgImgPath);
                    }
                }                
            }
        }

        private void BottomState(Cell[,] matrix)
        {
            var rowCount = matrix.GetLength(0);
            var colCount = matrix.GetLength(1);
            for (int i = 0; i < rowCount; i++)
            {
                if (i == 2)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        matrix[i, j] = new Cell(CellWidth, CellHeight, i, j, CellColor, CellBgImgPath);
                    }
                }
            }
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

            //var rowCount = builder.Matrix.GetLength(0);
            //var colCount = builder.Matrix.GetLength(1);

            //_x = (10 - colCount) / 2 * stage.StepDistance;
            //_y = 0;

            //for (int i = 0; i < rowCount; i++)
            //{
            //    for (int j = 0; j < colCount; j++)
            //    {
            //        if (builder.Matrix[i, j] == null || builder.Matrix[i, j].IsEmpty) continue;
            //        var panel = new Panel
            //        {
            //            Width = builder.Matrix[i, j].Width - 2,
            //            Height = builder.Matrix[i, j].Height - 2,
            //            Location = new Point(((this.stage.HorizonCount - colCount) / 2 + i) * builder.Matrix[i, j].Width,
            //                                 j * builder.Matrix[i, j].Height),
            //            BackColor = CellColor
            //        };
            //        if (this.stage.Platform.InvokeRequired)
            //        {
            //            this.stage.Platform.Invoke((SetControlDisplay)delegate
            //           {
            //               this.stage.Platform.Controls.Add(panel);
            //               panel.BringToFront();
            //           });
            //        }
            //        else
            //        {
            //            this.stage.Platform.Controls.Add(panel);
            //            panel.BringToFront();
            //        }
            //        cellList.Add(panel);
            //    }
            //}

            //this.stage.Check(this);
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
