using System;
using System.Windows.Forms;
using Tetris.Interfaces;
using Tetris.Models;

namespace Tetris.Utils
{
    public static class TetrisFactory
    {
        public static Stage GenerateStage(Control control, int stageWidth, int stageHeight, int stepDistance)
        {
            return new Stage(control, stageWidth, stageHeight, stepDistance);
        }
        public static ITransformer Create(TetrisModelType modelType)
        {
            ITransformer obj;
            switch (modelType)
            {
                case TetrisModelType.I:
                    obj = new I();
                    break;
                case TetrisModelType.L:
                    obj = new L();
                    break;
                case TetrisModelType.L2:
                    obj = new L2();
                    break;
                case TetrisModelType.O:
                    obj = new O();
                    break;
                case TetrisModelType.T:
                    obj = new T();
                    break;
                case TetrisModelType.Z:
                    obj = new Z();
                    break;
                case TetrisModelType.Z2:
                    obj = new Z2();
                    break;
                default:
                    obj = null;
                    break;
            }

            if (obj != null)
            {
                var random = new Random().Next(1, 5); 
                obj.Init((TransformState)random);
            }

            return obj;
        }

        public static TetrisModelType GenerateType()
        {
            var ran = new Random((int)DateTime.Now.Ticks);
            var tetrisType = (TetrisModelType)ran.Next(1, 8);
            return tetrisType;
        }
    }

}
