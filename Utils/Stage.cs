using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tetris.Interfaces;
using Tetris.Models;
using static Tetris.Utils.Delegates;

namespace Tetris.Utils
{
    public class Stage
    {
        private Control _control;
        private int _stageWidth;
        private int _stageHeight;
        private int _stepDistance;
        private Control[,] _stageSteps;
        private Queue<QueueInfo> transformQueue = new Queue<QueueInfo>();
        private ITransformer _tetris;
        private static object obj = new object();

        public Stage(Control control, int stageWidth, int stageHeight, int stepDistance)
        {
            _control = control;
            _stageWidth = stageWidth;
            _stageHeight = stageHeight;
            _stepDistance = stepDistance;
            _stageSteps = new Control[stageHeight / stepDistance, stageWidth / stepDistance];

            new System.Threading.Thread(StartQueue).Start();
        }

        private void StartQueue()
        {
            while (Rules.Displaying)
            {
                try
                {
                    if (transformQueue.Count > 0)
                    {
                        if (_tetris != null)
                        {
                            var queueInfo = transformQueue.Dequeue();
                            switch (queueInfo.MethodType)
                            {
                                case 1:
                                    lock (obj)
                                    {
                                        MoveItToLeft();
                                    }
                                    break;
                                case 2:
                                    lock (obj)
                                    {
                                        MoveItToRight();
                                    }
                                    break;
                                case 3:
                                    lock (obj)
                                    {
                                        MoveItDown();
                                    }
                                    break;
                                case 4:
                                    lock (obj)
                                    {
                                        TransformTetris();
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(30);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"err: {ex.Message}");
                }
            }
        }

        public void AddEventQueue(QueueInfo queueInfor)
        {
            transformQueue.Enqueue(queueInfor);
        }

        public Control Platform
        {
            get => _control;
        }

        public int StepDistance
        {
            get => _stepDistance;
        }

        public int StageWidth
        {
            get => _stageWidth;
        }

        public int StageHeight
        {
            get => _stageHeight;
        }

        public int HorizonCount
        {
            get => _stageWidth / _stepDistance;
        }

        public int VerticalCount
        {
            get => _stageHeight / _stepDistance;
        }

        public Control[,] StageSteps
        {
            get => _stageSteps;
        }

        public bool OnBottom { get; set; }

        public bool IsGameover { get; private set; }

        /// <summary>
        /// 能否移动
        /// </summary>
        /// <param name="movingTargets"></param>
        /// <param name="forward">-1向左，1向右，2向下</param>
        /// <returns></returns>
        public bool CanMove(List<Control> movingTargets, int forward)
        {
            int x, y;
            if (forward == -1)
            {
                for(var i = 0; i < movingTargets.Count; i++)
                {
                    x = movingTargets[i].Location.Y / StepDistance;
                    y = movingTargets[i].Location.X / StepDistance - 1;
                    if (y < 0 || _stageSteps[x, y] != null) return false;
                }
            }
            else if (forward == 1)
            {
                for (var i = 0; i < movingTargets.Count; i++)
                {
                    x = movingTargets[i].Location.Y / StepDistance;
                    y = movingTargets[i].Location.X / StepDistance + 1;
                    if (y >= _stageSteps.GetLength(1) || _stageSteps[x, y] != null) return false;
                }
            }
            else if (forward == 2)
            {
                for (var i = 0; i < movingTargets.Count; i++)
                {
                    x = movingTargets[i].Location.Y / StepDistance + 1;
                    y = movingTargets[i].Location.X / StepDistance;
                    if (x >= _stageSteps.GetLength(0) || _stageSteps[x, y] != null)
                    {
                        OnBottom = true;
                        return false;
                    }
                }
            }

            return true;
        }

        public void TransformTetris()
        {
            var transformTargets = _tetris.GetCellList();
            var p = _tetris.GetLocation();
            var sp = _tetris.GetCells().GetLength(0) - 1;
            int x, y;
            int newX, newY;

            bool CanTransform()
            {
                var pass = true;
                var rowCount = _stageSteps.GetLength(0);
                var colCount = _stageSteps.GetLength(1);
                int ii, jj;

                for (var i = 0; i < transformTargets.Count; i++)
                {
                    x = (transformTargets[i].Location.Y - p.Y) / StepDistance;
                    y = (transformTargets[i].Location.X - p.X) / StepDistance;
                    newX = y;
                    newY = sp - x;
                    ii = (newX * StepDistance + p.Y) / StepDistance;
                    jj = (newY * StepDistance + p.X) / StepDistance;
                    if (newX * StepDistance + p.Y >= StageHeight ||
                        newX * StepDistance + p.Y < 0 ||
                        newY * StepDistance + p.X >= StageWidth ||
                        ii < 0 ||
                        ii >= rowCount ||
                        jj < 0 ||
                        jj >= colCount ||
                        _stageSteps[ii, jj] != null)
                    {
                        pass = false;
                        break;
                    }
                }

                return pass;
            }

            if (!CanTransform()) return;

            for (var i = 0; i < transformTargets.Count; i++)
            {
                x = (transformTargets[i].Location.Y - p.Y) / StepDistance;
                y = (transformTargets[i].Location.X - p.X) / StepDistance;
                newX = y;
                newY = sp - x;
                if (transformTargets[i].InvokeRequired)
                {
                    transformTargets[i].Invoke(new SetControlPos(delegate ()
                    {
                        var l = transformTargets[i].Location;
                        l.X = newY * StepDistance + p.X;
                        l.Y = newX * StepDistance + p.Y;
                        transformTargets[i].Location = l;
                    }));
                }
                else
                {
                    var l = transformTargets[i].Location;
                    l.X = newY * StepDistance + p.X;
                    l.Y = newX * StepDistance + p.Y;
                    transformTargets[i].Location = l;
                }
            }
        }

        public void SetSolid(List<Control> controls)
        {
            int rowCount = _stageSteps.GetLength(0);
            int colCount = _stageSteps.GetLength(1);
            int x, y;
            foreach (var c in controls)
            {
                x = c.Location.Y / StepDistance;
                y = c.Location.X / StepDistance;
                if (x < rowCount && y < colCount) _stageSteps[x, y] = c;
            }
        }

        public void Check()
        {
            if (_tetris == null) return;
            int x, y;
            var controls = _tetris.GetCellList();
            foreach (var c in controls)
            {
                x = c.Location.Y / StepDistance;
                y = c.Location.X / StepDistance;
                if (_stageSteps[x, y] != null)
                {
                    IsGameover = true;
                    break;
                }
            }
        }

        public void MoveItToLeft()
        {
            var cellList = _tetris.GetCellList();
            if (!CanMove(cellList, -1)) return;

            Point l;
            for (var i = 0; i < cellList.Count; i++)
            {
                if (cellList[i].InvokeRequired)
                {
                    cellList[i].Invoke((SetControlPos)delegate
                    {
                        while (!cellList[i].IsHandleCreated)
                        {
                            if (cellList[i].Disposing || cellList[i].IsDisposed) return;
                        }
                        l = cellList[i].Location;
                        l.X -= 30;
                        cellList[i].Location = l;
                    });
                }
                else
                {
                    l = cellList[i].Location;
                    l.X -= 30;
                    cellList[i].Location = l;
                }
            }

            var p = _tetris.GetLocation();
            _tetris.SetLocation(p.X - StepDistance, p.Y);
        }

        public void MoveItToRight()
        {
            var cellList = _tetris.GetCellList();
            if (!CanMove(cellList, 1)) return;

            Point l;
            for (var i = 0; i < cellList.Count; i++)
            {
                if (cellList[i].InvokeRequired)
                {
                    cellList[i].Invoke((SetControlPos)delegate
                    {
                        while (!cellList[i].IsHandleCreated)
                        {
                            if (cellList[i].Disposing || cellList[i].IsDisposed) return;
                        }
                        l = cellList[i].Location;
                        l.X += 30;
                        cellList[i].Location = l;
                    });
                }
                else
                {
                    l = cellList[i].Location;
                    l.X += 30;
                    cellList[i].Location = l;
                }
            }

            var p = _tetris.GetLocation();
            _tetris.SetLocation(p.X + StepDistance, p.Y);
        }

        public void MoveItDown()
        {
            var cellList = _tetris.GetCellList();

            if (!CanMove(cellList, 2))
            {
                SetSolid(cellList);
                _tetris = null;
                CleanUp();
                return;
            }

            var p = _tetris.GetLocation();
            _tetris.SetLocation(p.X, p.Y + StepDistance);

            Point l;

            for (var i = 0; i < cellList.Count; i++)
            {
                if (cellList[i].InvokeRequired)
                {
                    cellList[i].Invoke((SetControlPos)delegate
                    {
                        while (!cellList[i].IsHandleCreated)
                        {
                            if (cellList[i].Disposing || cellList[i].IsDisposed) return;
                        }
                        l = cellList[i].Location;
                        l.Y += 30;
                        cellList[i].Location = l;
                    });
                }
                else
                {
                    l = cellList[i].Location;
                    l.Y += 30;
                    cellList[i].Location = l;
                }
            }
        }

        public void Reset()
        {
            _stageSteps = new Control[_stageHeight / _stepDistance, _stageWidth / _stepDistance];
            _control.Controls.Clear();
            IsGameover = false;
            OnBottom = false;
        }

        public void SetUp(ITransformer tetris, Cell[,] matrix, Color cellColor, List<Control> cellList)
        {
            transformQueue.Clear();

            _tetris = tetris;
            var rowCount = matrix.GetLength(0);
            var colCount = matrix.GetLength(1);

            _tetris.SetLocation((10 - colCount) / 2 * StepDistance, 0);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    if (matrix[i, j] == null || matrix[i, j].IsEmpty) continue;
                    var panel = new Panel
                    {
                        Width = matrix[i, j].Width - 2,
                        Height = matrix[i, j].Height - 2,
                        Location = new Point(((HorizonCount - colCount) / 2 + i) * matrix[i, j].Width,
                                             j * matrix[i, j].Height),
                        BackColor = cellColor
                    };
                    if (Platform.InvokeRequired)
                    {
                        Platform.Invoke((SetControlDisplay)delegate
                        {
                            Platform.Controls.Add(panel);
                            panel.BringToFront();
                        });
                    }
                    else
                    {
                        Platform.Controls.Add(panel);
                        panel.BringToFront();
                    }
                    cellList.Add(panel);
                }
            }

            Check();
        }

        public void CleanUp()
        {
            transformQueue.Clear();

            var rowCount = _stageSteps.GetLength(0);
            var colCount = _stageSteps.GetLength(1);
            int t;
            var cleanRows = new List<int>();

            //确认行
            for(var i = 0; i < rowCount; i++)
            {
                t = 0;
                for(var j = 0; j < colCount; j++)
                {
                    if (_stageSteps[i, j] == null) continue;
                    t++;
                }
                if(t==colCount)
                {
                    cleanRows.Add(i);
                }
            }

            if (cleanRows.Count == 0) return;

            //消除
            cleanRows.ForEach(rowIndex =>
            {
                for(var j = 0; j < colCount; j++)
                {
                    if (Platform.InvokeRequired)
                    {
                        Platform.Invoke((SetControlPos)delegate
                        {
                            Platform.Controls.Remove(_stageSteps[rowIndex, j]);
                            _stageSteps[rowIndex, j] = null;
                        });
                    }
                    else
                    {
                        Platform.Controls.Remove(StageSteps[rowIndex, j]);
                        _stageSteps[rowIndex, j] = null;
                    }
                }
            });

            Point l;
            cleanRows.ForEach(rowIndex =>
            {
                //向下行
                for (var i = 0; i < rowCount; i++)
                {
                    for (var j = 0; j < colCount; j++)
                    {
                        if (i < rowIndex && _stageSteps[i, j] != null)
                        {
                            if (_stageSteps[i, j].InvokeRequired)
                            {
                                _stageSteps[i, j].Invoke((SetControlPos)delegate
                                {
                                    l = _stageSteps[i, j].Location;
                                    l.X = _stageSteps[i, j].Location.X;
                                    l.Y = _stageSteps[i, j].Location.Y + StepDistance;
                                    _stageSteps[i, j].Location = l;
                                });
                            }
                            else
                            {
                                l = _stageSteps[i, j].Location;
                                l.X = _stageSteps[i, j].Location.X;
                                l.Y = _stageSteps[i, j].Location.Y + StepDistance;
                                _stageSteps[i, j].Location = l;
                            }
                        }
                    }
                }
                //向下位移
                for(var i = rowIndex; i > -1; i--)
                {
                    for(var j = 0; j < colCount; j++)
                    {
                        if (i == 0) _stageSteps[i, j] = null;
                        else _stageSteps[i, j] = _stageSteps[i - 1, j];
                    }
                }
            });
        }
    }
}
