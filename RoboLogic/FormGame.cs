using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Hig.ScriptEngine;

namespace RoboLogic
{
    public partial class FormGame : Form
    {
        private Thread _compileThread;
        public RoboLogicGame _game;

        public FormGame()
        {
            InitializeComponent();
            tlpnlCode.SetDoubleBuffered(true);
        }

        private void GameSetMessage(string message)
        {
            _game.IsActive = false;

            this.InvokeIfRequired(control => 
                {
                    _game.IsActive = MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK;
                });
        }

        private void Run()
        {
            var errors = _game.SetCode((_game.CurrentRobotIndex == 0) ? tbCode1.Text : tbCode2.Text);
            this.InvokeIfRequired(control => bsError.DataSource = errors);
            gbErrorList.InvokeIfRequired(control => control.Text = "Error List - " + errors.Length);
            pbCompile.InvokeIfRequired(control => control.Style = ProgressBarStyle.Blocks);
        }

        private void ShowFormSelectRound(bool start)
        {
            Round round = null;
            DialogResult result;

            using (FormSelectRound frmSelect = new FormSelectRound(start))
            {
                result = frmSelect.ShowDialog(this);
                round = frmSelect.Round;
            }

            if (result == DialogResult.Abort)
            {
                Close();
            }
            else if (result == DialogResult.OK)
            {
                tabPage1.BackColor = round.Player1Color;
                tabPage2.BackColor = round.Player2Color;

                _game.NewGame(round);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _game = new RoboLogicGame(pnlCanvas.Handle, pnlCanvas.ClientSize.Width, pnlCanvas.ClientSize.Height);
            _game.SetMessage += GameSetMessage;
            _game.Win += GameWin;
            _game.StratMainLoop();
            bsRoboLogicGame.DataSource = _game;
        }

        private void GameWin()
        {
            GameSetMessage("You win.");
            this.InvokeIfRequired(frm => ShowFormSelectRound(true));
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            ShowFormSelectRound(true);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_compileThread != null)
                _compileThread.Abort();

            _game.StopMainLoop();
            _game.Dispose();

            base.OnClosed(e);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (_compileThread == null || !_compileThread.IsAlive)
            {
                pbCompile.Style = ProgressBarStyle.Marquee;
                _compileThread = new Thread(Run);
                _compileThread.Start();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _game.StopScript();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbCode1.Clear();
        }

        private void bsRoboLogicGame_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            btnRun.Invalidate();
        }

        private void tbcCode_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!_game.ScriptIsCompleted)
            {
                e.Cancel = true;
                GameSetMessage("You can not change tab until robot is working.");

                return;
            }

            _game.CurrentRobotIndex = e.TabPageIndex;
        }

        private void tsmiNewGame_Click(object sender, EventArgs e)
        {
            ShowFormSelectRound(false);
        }

        private void TextBoxPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                e.IsInputKey = true;
        }
    }
}
