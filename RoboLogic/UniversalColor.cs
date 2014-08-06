namespace RoboLogic
{
    using System;
    using System.Xml.Serialization;

    using Drawing = System.Drawing;
    //using WPF = System.Windows.Media;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Служит для интеграции между собой всех структур Color
    /// </summary>
    [Serializable]
    public struct UniversalColor
    {
        #region " Fields and props "

        /// <summary>
        /// Возвращает и устанавливает значение красного компонента
        /// </summary>
        [XmlAttribute]
        public byte R { get; set; }
        /// <summary>
        /// Возвращает и устанавливает значение зеленого компонента
        /// </summary>
        [XmlAttribute]
        public byte G { get; set; }
        /// <summary>
        /// Возвращает и устанавливает значение синего компонента
        /// </summary>
        [XmlAttribute]
        public byte B { get; set; }
        /// <summary>
        /// Возвращает и устанавливает значение альфа-компонента
        /// </summary>
        [XmlAttribute]
        public byte A { get; set; }

        /// <summary>
        /// Возвращает цвет
        /// </summary>
        public Drawing.Color DrawingColor
        {
            get { return Drawing.Color.FromArgb(A, R, G, B); }
        }

        /// <summary>
        /// Возвращает цвет
        /// </summary>
        public Xna.Color XnaColor
        {
            get { return new Xna.Color() { R = R, G = G, B = B, A = A }; }
        }

        ///// <summary>
        ///// Возвращает цвет
        ///// </summary>
        //public WPF.Color WPFColor
        //{
        //    get { return new WPF.Color() { R = R, G = G, B = B, A = A }; }
        //}

        #endregion

        #region " Ctors "

        /// <summary>
        /// Создает цвет
        /// </summary>
        /// <param name="r">Красный компонент</param>
        /// <param name="g">Зеленый компонент</param>
        /// <param name="b">Синий компонент</param>
        /// <param name="a">Альфа-компонент</param>
        public UniversalColor(byte r, byte g, byte b, byte a)
            : this()
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Создает цвет
        /// </summary>
        /// <param name="r">Красный компонент</param>
        /// <param name="g">Зеленый компонент</param>
        /// <param name="b">Синий компонент</param>
        public UniversalColor(byte r, byte g, byte b)
            : this(r, g, b, 255) { }

        /// <summary>
        /// Создает цвет на основе цвета из пространства имен: System.Drawing
        /// </summary>
        /// <param name="color">Цвет</param>
        public UniversalColor(Drawing.Color color)
            : this(color.R, color.G, color.B, color.A) { }

        /// <summary>
        /// Создает цвет на основе цвета из пространства имен: Microsoft.Xna.Framework.Graphics
        /// </summary>
        /// <param name="color">Цвет</param>
        public UniversalColor(Xna.Color color)
            : this(color.R, color.G, color.B, color.A) { }

        ///// <summary>
        ///// Создает цвет на основе цвета из пространства имен: System.Windows.Media
        ///// </summary>
        ///// <param name="color">Цвет</param>
        //public UniversalColor(WPF.Color color)
        //    : this(color.R, color.G, color.B, color.A) { }

        #endregion

        #region " Operators "

        /// <summary>
        /// Преобразует UniversalColor к System.Drawing.Color
        /// </summary>
        /// <param name="color">Цвет</param>
        /// <returns>Результат преобразования</returns>
        public static implicit operator Drawing.Color(UniversalColor color)
        {
            return color.DrawingColor;
        }

        /// <summary>
        /// Преобразует System.Drawing.Color к UniversalColor
        /// </summary>
        /// <param name="color">Цвет</param>
        /// <returns>Результат преобразования</returns>
        public static implicit operator UniversalColor(Drawing.Color color)
        {
            return new UniversalColor(color);
        }

        /// <summary>
        /// Преобразует UniversalColor к Microsoft.Xna.Framework.Graphics.Color
        /// </summary>
        /// <param name="color">Цвет</param>
        /// <returns>Результат преобразования</returns>
        public static implicit operator Xna.Color(UniversalColor color)
        {
            return color.XnaColor;
        }

        /// <summary>
        /// Преобразует Microsoft.Xna.Framework.Graphics.Color к UniversalColor
        /// </summary>
        /// <param name="color">Цвет</param>
        /// <returns>Результат преобразования</returns>
        public static implicit operator UniversalColor(Xna.Color color)
        {
            return new UniversalColor(color);
        }

        ///// <summary>
        ///// Преобразует UniversalColor к System.Windows.Media.Color
        ///// </summary>
        ///// <param name="color">Цвет</param>
        ///// <returns>Результат преобразования</returns>
        //public static implicit operator WPF.Color(UniversalColor color)
        //{
        //    return color.WPFColor;
        //}

        ///// <summary>
        ///// Преобразует System.Windows.Media.Color к UniversalColor
        ///// </summary>
        ///// <param name="color">Цвет</param>
        ///// <returns>Результат преобразования</returns>
        //public static implicit operator UniversalColor(WPF.Color color)
        //{
        //    return new UniversalColor(color);
        //}

        #endregion
    }
}
