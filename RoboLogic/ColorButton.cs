namespace RoboLogic
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;


    //[ToolboxBitmap(typeof(ColorButton), "RoboLogic.ColorButton.bmp")]
    public class ColorButton : Button
    {
        private Rectangle _rectFill;
        private SolidBrush _fillColorBrush = new SolidBrush(Color.Red);
        private Pen _penFillBorder = new Pen(Color.Black);

        [Category("Appearance"),
        Browsable(true),
        DefaultValue(true),
        Description("Get or set value. Show the palitra by click.")]
        public bool ShowColorDialog { get; set; }

        private Color _fillColor = Color.Red;
        [Category("Appearance")]
        public Color FillColor
        {
            get { return _fillColor; }
            set
            {
                _fillColor = value;
                _fillColorBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        private Color _fillBorderColor = Color.Black;
        [Category("Appearance")]
        public Color FillBorderColor
        {
            get { return _fillBorderColor; }
            set
            {
                _fillBorderColor = value;
                _penFillBorder = new Pen(value);
                Invalidate();
            }
        }

        public ColorButton()
        {
            ShowColorDialog = true;
            RefreshRectangleFill();
        }

        protected virtual void RefreshRectangleFill()
        {
            _rectFill = new Rectangle(5, 5, Width - 11, Height - 11);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            pevent.Graphics.FillRectangle(_fillColorBrush, _rectFill);
            pevent.Graphics.DrawRectangle(_penFillBorder, _rectFill);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RefreshRectangleFill();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (ShowColorDialog)
            {
                using (ColorDialog cd = new ColorDialog())
                {
                    cd.SolidColorOnly = true;

                    if (cd.ShowDialog(this) == DialogResult.OK)
                        FillColor = cd.Color;
                }
            }
        }
    }
}
