namespace RoboLogic
{
    partial class FormGame
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlCanvas = new System.Windows.Forms.Panel();
            this.tlp = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tlpnlCode = new System.Windows.Forms.TableLayoutPanel();
            this.pbCompile = new System.Windows.Forms.ProgressBar();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tbcCode = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbCode1 = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbCode2 = new System.Windows.Forms.TextBox();
            this.gbErrorList = new System.Windows.Forms.GroupBox();
            this.dgvError = new System.Windows.Forms.DataGridView();
            this.lineNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsError = new System.Windows.Forms.BindingSource(this.components);
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.tsmiNewGame = new System.Windows.Forms.ToolStripMenuItem();
            this.bsRoboLogicGame = new System.Windows.Forms.BindingSource(this.components);
            this.tlp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tlpnlCode.SuspendLayout();
            this.tbcCode.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.gbErrorList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsError)).BeginInit();
            this.msMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsRoboLogicGame)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlCanvas
            // 
            this.pnlCanvas.DataBindings.Add(new System.Windows.Forms.Binding("TabIndex", this.bsRoboLogicGame, "CurrentRobotIndex", true));
            this.pnlCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCanvas.Location = new System.Drawing.Point(3, 3);
            this.pnlCanvas.Name = "pnlCanvas";
            this.pnlCanvas.Size = new System.Drawing.Size(508, 494);
            this.pnlCanvas.TabIndex = 0;
            // 
            // tlp
            // 
            this.tlp.ColumnCount = 2;
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tlp.Controls.Add(this.pnlCanvas, 0, 0);
            this.tlp.Controls.Add(this.splitContainer1, 1, 0);
            this.tlp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp.Location = new System.Drawing.Point(0, 24);
            this.tlp.Name = "tlp";
            this.tlp.RowCount = 1;
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp.Size = new System.Drawing.Size(857, 500);
            this.tlp.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(517, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tlpnlCode);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gbErrorList);
            this.splitContainer1.Panel2MinSize = 100;
            this.splitContainer1.Size = new System.Drawing.Size(337, 494);
            this.splitContainer1.SplitterDistance = 359;
            this.splitContainer1.TabIndex = 1;
            // 
            // tlpnlCode
            // 
            this.tlpnlCode.ColumnCount = 4;
            this.tlpnlCode.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tlpnlCode.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tlpnlCode.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tlpnlCode.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpnlCode.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpnlCode.Controls.Add(this.pbCompile, 3, 1);
            this.tlpnlCode.Controls.Add(this.btnRun, 0, 1);
            this.tlpnlCode.Controls.Add(this.btnClear, 2, 1);
            this.tlpnlCode.Controls.Add(this.btnStop, 1, 1);
            this.tlpnlCode.Controls.Add(this.tbcCode, 0, 0);
            this.tlpnlCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpnlCode.Location = new System.Drawing.Point(0, 0);
            this.tlpnlCode.Name = "tlpnlCode";
            this.tlpnlCode.RowCount = 2;
            this.tlpnlCode.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpnlCode.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpnlCode.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpnlCode.Size = new System.Drawing.Size(337, 359);
            this.tlpnlCode.TabIndex = 0;
            // 
            // pbCompile
            // 
            this.pbCompile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbCompile.Location = new System.Drawing.Point(246, 333);
            this.pbCompile.Name = "pbCompile";
            this.pbCompile.Size = new System.Drawing.Size(88, 23);
            this.pbCompile.TabIndex = 4;
            // 
            // btnRun
            // 
            this.btnRun.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.bsRoboLogicGame, "ScriptIsCompleted", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.btnRun.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRun.Location = new System.Drawing.Point(3, 333);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "&Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnClear
            // 
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClear.Location = new System.Drawing.Point(165, 333);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "&Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnStop
            // 
            this.btnStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStop.Location = new System.Drawing.Point(84, 333);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "&Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tbcCode
            // 
            this.tlpnlCode.SetColumnSpan(this.tbcCode, 4);
            this.tbcCode.Controls.Add(this.tabPage1);
            this.tbcCode.Controls.Add(this.tabPage2);
            this.tbcCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcCode.Location = new System.Drawing.Point(3, 3);
            this.tbcCode.Name = "tbcCode";
            this.tbcCode.SelectedIndex = 0;
            this.tbcCode.Size = new System.Drawing.Size(331, 324);
            this.tbcCode.TabIndex = 1;
            this.tbcCode.Tag = "";
            this.tbcCode.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tbcCode_Selecting);
            this.tbcCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TextBoxPreviewKeyDown);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.tbCode1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(323, 298);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Tag = "0";
            this.tabPage1.Text = "Player 1";
            // 
            // tbCode1
            // 
            this.tbCode1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCode1.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCode1.Location = new System.Drawing.Point(3, 3);
            this.tbCode1.Multiline = true;
            this.tbCode1.Name = "tbCode1";
            this.tbCode1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbCode1.Size = new System.Drawing.Size(317, 292);
            this.tbCode1.TabIndex = 0;
            this.tbCode1.WordWrap = false;
            this.tbCode1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TextBoxPreviewKeyDown);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.tbCode2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(323, 298);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Tag = "1";
            this.tabPage2.Text = "Player 2";
            // 
            // tbCode2
            // 
            this.tbCode2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCode2.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCode2.Location = new System.Drawing.Point(3, 3);
            this.tbCode2.Multiline = true;
            this.tbCode2.Name = "tbCode2";
            this.tbCode2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbCode2.Size = new System.Drawing.Size(317, 292);
            this.tbCode2.TabIndex = 1;
            this.tbCode2.WordWrap = false;
            this.tbCode2.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TextBoxPreviewKeyDown);
            // 
            // gbErrorList
            // 
            this.gbErrorList.Controls.Add(this.dgvError);
            this.gbErrorList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbErrorList.Location = new System.Drawing.Point(0, 0);
            this.gbErrorList.Name = "gbErrorList";
            this.gbErrorList.Size = new System.Drawing.Size(337, 131);
            this.gbErrorList.TabIndex = 1;
            this.gbErrorList.TabStop = false;
            this.gbErrorList.Text = "&Error List - 0";
            // 
            // dgvError
            // 
            this.dgvError.AllowUserToAddRows = false;
            this.dgvError.AllowUserToDeleteRows = false;
            this.dgvError.AllowUserToResizeRows = false;
            this.dgvError.AutoGenerateColumns = false;
            this.dgvError.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvError.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.lineNumberDataGridViewTextBoxColumn,
            this.textDataGridViewTextBoxColumn});
            this.dgvError.DataSource = this.bsError;
            this.dgvError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvError.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvError.Location = new System.Drawing.Point(3, 16);
            this.dgvError.MultiSelect = false;
            this.dgvError.Name = "dgvError";
            this.dgvError.ReadOnly = true;
            this.dgvError.RowHeadersVisible = false;
            this.dgvError.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvError.Size = new System.Drawing.Size(331, 112);
            this.dgvError.TabIndex = 0;
            // 
            // lineNumberDataGridViewTextBoxColumn
            // 
            this.lineNumberDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.lineNumberDataGridViewTextBoxColumn.DataPropertyName = "LineNumber";
            this.lineNumberDataGridViewTextBoxColumn.HeaderText = "Line";
            this.lineNumberDataGridViewTextBoxColumn.Name = "lineNumberDataGridViewTextBoxColumn";
            this.lineNumberDataGridViewTextBoxColumn.ReadOnly = true;
            this.lineNumberDataGridViewTextBoxColumn.Width = 52;
            // 
            // textDataGridViewTextBoxColumn
            // 
            this.textDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.textDataGridViewTextBoxColumn.DataPropertyName = "Text";
            this.textDataGridViewTextBoxColumn.HeaderText = "Description";
            this.textDataGridViewTextBoxColumn.Name = "textDataGridViewTextBoxColumn";
            this.textDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // bsError
            // 
            this.bsError.DataSource = typeof(Hig.ScriptEngine.Error);
            // 
            // msMain
            // 
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNewGame});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Size = new System.Drawing.Size(857, 24);
            this.msMain.TabIndex = 3;
            // 
            // tsmiNewGame
            // 
            this.tsmiNewGame.Name = "tsmiNewGame";
            this.tsmiNewGame.Size = new System.Drawing.Size(77, 20);
            this.tsmiNewGame.Text = "&New Game";
            this.tsmiNewGame.Click += new System.EventHandler(this.tsmiNewGame_Click);
            // 
            // bsRoboLogicGame
            // 
            this.bsRoboLogicGame.DataSource = typeof(RoboLogic.RoboLogicGame);
            this.bsRoboLogicGame.BindingComplete += new System.Windows.Forms.BindingCompleteEventHandler(this.bsRoboLogicGame_BindingComplete);
            // 
            // FormGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(857, 524);
            this.Controls.Add(this.tlp);
            this.Controls.Add(this.msMain);
            this.MainMenuStrip = this.msMain;
            this.MinimumSize = new System.Drawing.Size(612, 250);
            this.Name = "FormGame";
            this.Text = "RoboLogic";
            this.tlp.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tlpnlCode.ResumeLayout(false);
            this.tbcCode.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.gbErrorList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsError)).EndInit();
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsRoboLogicGame)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlCanvas;
        private System.Windows.Forms.TableLayoutPanel tlp;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tlpnlCode;
        private System.Windows.Forms.TextBox tbCode1;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.DataGridView dgvError;
        private System.Windows.Forms.BindingSource bsError;
        private System.Windows.Forms.DataGridViewTextBoxColumn lineNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn textDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource bsRoboLogicGame;
        private System.Windows.Forms.GroupBox gbErrorList;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ProgressBar pbCompile;
        private System.Windows.Forms.TabControl tbcCode;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox tbCode2;
        private System.Windows.Forms.MenuStrip msMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiNewGame;
    }
}

