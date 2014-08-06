namespace Hig.GameEngine.Net
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class TCPServer
    {
        private bool _started = false;
        private Thread _thread;

        public IPAddress IP { get; protected set; }
        public int Port { get; protected set; }

        public event NetworkEventHandler Received;

        #region " Basic methods "

        private void Listen(object ipe)
        {
            Socket socket = new Socket(((IPEndPoint)ipe).AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind((IPEndPoint)ipe);

            socket.Listen(10);
            Socket tmpSocket = socket.Accept();

            while (_started)
            {
                #region " Get length "

                byte[] buffer = new byte[8];

                try
                {
                    tmpSocket.Receive(buffer);
                }
                catch
                {
                    tmpSocket.Close();
                    socket.Close();

                    return;
                }

                #endregion

                int length = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[length];

                #region " Get data "

                int count = 0;
                int stratIndex = 0;
                byte[] tmpBuffer = new byte[256];

                do
                {
                    int index = 0;

                    count += tmpSocket.Receive(tmpBuffer);

                    for (int i = stratIndex; i < ((count < length) ? count : length); i++)
                        buffer[i] = tmpBuffer[index++];

                    //System.Threading.Thread.Sleep(50);

                } while ((stratIndex = count) < length);

                #endregion

                OnReceived(new NetworkEventArgs(buffer));
            }

            tmpSocket.Close();
        }

        protected void OnReceived(NetworkEventArgs e)
        {
            if (Received != null)
                Received(this, e);
        }

        public void Begin(IPAddress ip, int port)
        {
            if (ip == null)
                throw new ArgumentNullException("ip");
            if (port < 0 || port > 65535)
                throw new ArgumentOutOfRangeException("port");

            IP = ip;
            Port = port;

            _started = true;
            IPEndPoint ipe = new IPEndPoint(ip, port);

            _thread = new Thread(Listen);
            _thread.Start(ipe);
        }

        public void End()
        {
            if (_started)
            {
                _started = false;
                _thread.Abort();
            }
        }

        #endregion
    }
}
