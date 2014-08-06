using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RoboLogic
{
    public partial class FormSelectRound : Form
    {
        private const string _path = "Rounds";
        private readonly static List<Round> _rounds = new List<Round>();

        private static int _roundIndex = 0;
        private static Color _player1Color;
        private static Color _player2Color;

        private bool _askAboutClose = true;
        private int _imgIndex = 0;

        public Round Round { get; protected set; }

        static FormSelectRound()
        {
            _player1Color = Color.FromArgb(255, Color.FromArgb(Guid.NewGuid().GetHashCode()));
            _player2Color = Color.FromArgb(255, Color.FromArgb(Guid.NewGuid().GetHashCode()));
            FindRounds();
        }

        public FormSelectRound(bool start)
        {
            InitializeComponent();

            btnContinue.Enabled = !start;
        }

        private static void FindRounds()
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);

                return;
            }

            foreach (var roundPath in Directory.GetFiles(_path, "*.rrl"))
                _rounds.Add(Round.Load(roundPath));
        }

        private void SelectImage(int index)
        {
            if (Round != null)
            {
                if (index < 0)
                    index = Round.ImagePaths.Count - 1;
                else if (index >= Round.ImagePaths.Count)
                    index = 0;

                _imgIndex = index;

                string imgPath = Round.ImagePaths[_imgIndex];

                if (!String.IsNullOrEmpty(imgPath))
                {
                    imgPath = _path + "\\" + imgPath;

                    if (File.Exists(imgPath))
                        picbAgenda.Image = Image.FromFile(imgPath);
                }
            }
        }

        private void SelectRound(int index)
        {
            if (index < 0)
                index = _rounds.Count - 1;
            else if (index >= _rounds.Count)
                index = 0;

            _roundIndex = index;

            if (_rounds.Count > 0)
            {
                _imgIndex = 0;
                Round = _rounds[_roundIndex];
                SelectImage(0);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            btnPlayer1Color.FillColor = _player1Color;
            btnPlayer2Color.FillColor = _player2Color;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (_rounds.Count == 0)
            {
                MessageBox.Show(this, String.Format("No rounds. {0} will closed.", Text), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Abort;
                _askAboutClose = false;
                Close();
            }

            SelectRound(_roundIndex);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                _player1Color = Round.Player1Color = btnPlayer1Color.FillColor;
                _player2Color = Round.Player2Color = btnPlayer2Color.FillColor;
            }
            else if (DialogResult == DialogResult.Abort && _askAboutClose)
            {
                if (MessageBox.Show(this, "Are you sure?", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    e.Cancel = true;
            }

            base.OnClosing(e);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            SelectRound(_roundIndex - 1);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SelectRound(_roundIndex + 1);
        }

        private void btnImgPrevious_Click(object sender, EventArgs e)
        {
            SelectImage(_imgIndex - 1);
        }

        private void btnImgNext_Click(object sender, EventArgs e)
        {
            SelectImage(_imgIndex + 1);
        }
    }
}
