using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tetris.Models;
using Tetris.Utils;

namespace Tetris.Interfaces
{
    public interface ITransformer
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="state"></param>
        void Init(TransformState state);

        /// <summary>
        /// 变形
        /// </summary>
        void Transform();

        void MoveDown();

        void MoveLeft();

        void MoveRight();

        Point GetLocation();

        void SetLocation(int x, int y);

        void DisplayOn(Stage stage);

        int GetWidth();

        int GetHeight();

        Color GetColor();

        Cell[,] GetCells();

        List<Control> GetCellList();

        bool IsGameOver();

        bool IsOnBottom();
    }
}
