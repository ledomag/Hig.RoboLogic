namespace Hig.GameEngine.Net
{
    using System;

    public class NetworkEventArgs : EventArgs
    {
        public byte[] Message { get; private set; }

        public NetworkEventArgs(byte[] message)
        {
            Message = message;
        }
    }

    public delegate void NetworkEventHandler(object sender, NetworkEventArgs e);
}
