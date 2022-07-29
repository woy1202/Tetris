using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tetris.Interfaces;
using Tetris.Utils;
using static Tetris.Utils.Delegates;

namespace Tetris
{
    public partial class FormMain : Form
    {
        ITransformer tetris;
        List<ITransformer> tetrisQueue;
        private System.Timers.Timer timer;
        Stage stage;

        public FormMain()
        {
            InitializeComponent();
            panelView.Width = 300;
            stage = TetrisFactory.GenerateStage(panelView, 300, 600, 30);
            tetrisQueue = new List<ITransformer>();

            //预览
            var tmpType = TetrisFactory.GenerateType();
            var tmp = TetrisFactory.Create(tmpType);

            var tmpType2 = TetrisFactory.GenerateType();
            var tmp2 = TetrisFactory.Create(tmpType2);

            tetrisQueue.Add(tmp);
            tetrisQueue.Add(tmp2);

            label1.Text = tetrisQueue[0].GetType().Name;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right) return false;
            return base.ProcessDialogKey(keyData);
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    tetris.MoveLeft();
                    break;
                case Keys.Right:
                    tetris.MoveRight();
                    break;
                case Keys.Down:
                    tetris.MoveDown();
                    break;
                case Keys.Up:
                    tetris.Transform();
                    break;
                default:
                    break;
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;

            if (stage.IsGameover)
            {
                stage.Reset();
                timer.Start();
            }
            else
            {
                timer = new System.Timers.Timer(1000);
                timer.Elapsed += new System.Timers.ElapsedEventHandler((source, eventArgs) =>
                {
                    stage.AddEventQueue(new QueueInfo { MethodType = 3 });
                    //tetris.MoveDown();
                    if (tetris.IsGameOver())
                    {
                        timer.Stop();
                        if (btnStart.InvokeRequired)
                        {
                            btnStart.Invoke((SetControlDisplay)delegate
                            {
                                btnStart.Enabled = true;
                            });
                        }
                        MessageBox.Show("Game over");
                    }
                    else if (tetris.IsOnBottom())
                    {
                        StartNew();
                    }
                });
                timer.AutoReset = true;
                timer.Enabled = true;
            }

            StartNew();
        }

        private void BtnLeft_Click(object sender, EventArgs e)
        {

            stage.AddEventQueue(new QueueInfo { MethodType = 1 });
        }

        private void BtnRight_Click(object sender, EventArgs e)
        {
            stage.AddEventQueue(new QueueInfo { MethodType = 2 });
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            stage.AddEventQueue(new QueueInfo { MethodType = 3 });
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void StartNew()
        {
            //var tetrisType = TetrisFactory.GenerateType();
            //tetris = TetrisFactory.Create(tetrisType);
            tetris = tetrisQueue[0];
            tetris.DisplayOn(stage);

            if (label1.InvokeRequired)
            {
                label1.Invoke((SetControlPos)delegate
                {
                    label1.Text = tetrisQueue[1].GetType().Name;
                });
            }

            tetrisQueue.RemoveAt(0);

            var tetrisType = TetrisFactory.GenerateType();
            tetrisQueue.Add(TetrisFactory.Create(tetrisType));
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (timer != null && timer.Enabled) timer.Stop();
            //if (thread != null && thread.IsAlive) thread.Abort();
            Rules.Displaying = false;
        }

        private void BtnTransform_Click(object sender, EventArgs e)
        {
            stage.AddEventQueue(new QueueInfo { MethodType = 4 });
        }
    }
}
