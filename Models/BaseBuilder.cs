using System;
using Tetris.Utils;

namespace Tetris.Models
{
    public class BaseBuilder
    {
        //private Cell[,] _matrix;
        //private TetrisModelType _tetrisModelType;

        public BaseBuilder(TetrisModelType modelType, Action<Cell[,]> initCell)
        {
            ModelType = modelType;

            InitMetrix(initCell);
        }

        public TetrisModelType ModelType
        {
            get;
            private set;
        }

        public Cell[,] Matrix
        {
            get;
            private set;
        }

        private void InitMetrix(Action<Cell[,]> initCell)
        {
            switch (ModelType)
            {
                case TetrisModelType.I:
                    Matrix = new Cell[4, 4];
                    break;
                case TetrisModelType.L:
                    Matrix = new Cell[3, 3];
                    break;
                case TetrisModelType.O:
                    Matrix = new Cell[2, 2];
                    break;
                case TetrisModelType.T:
                    Matrix = new Cell[3, 3];
                    break;
                case TetrisModelType.Z:
                    Matrix = new Cell[3, 3];
                    break;
                default:
                    Matrix = new Cell[0, 0];
                    break;
            }

            initCell(Matrix);
        }
    }
}
