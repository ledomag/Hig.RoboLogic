namespace RoboLogic
{
    using System;
    using System.Windows.Forms;
    using System.Reflection;

    public static class ControlExentsions
    {
        public static void InvokeIfRequired<T>(this T control, Action<T> action)
            where T : Control
        {
            if (control.InvokeRequired)
                control.Invoke(new Action(() => action(control)));
            else
                action(control);
        }

        public static void SetDoubleBuffered(this Control control, bool value)
        {
            control.GetType().InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.NonPublic, null, control, new[] { (object)value });
        }
    }
}
