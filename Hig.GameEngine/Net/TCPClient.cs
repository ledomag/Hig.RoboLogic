namespace Hig.GameEngine.Net
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    public class TCPClient : IDisposable
    {
        private bool _disposed = false;
        protected Socket _socket;

        public bool Connected
        {
            get
            {
                if (_socket != null)
                    return _socket.Connected;

                return false;
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_socket != null)
                        _socket.Close();
                }

                _disposed = true;
            }
        }

        public void Connect(IPAddress ip, int port)
        {
            if (ip == null)
                throw new ArgumentNullException("ip");
            if (port < 0 || port > 65535)
                throw new ArgumentOutOfRangeException("port");

            if(_socket == null)
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (!Connected)
            {
                try
                {
                    IPEndPoint ipPoint = new IPEndPoint(ip, port);
                    _socket.Connect(ipPoint);
                }
                catch
                {
                    throw;
                }
            }
        }

        public void Disconnect()
        {
            if (Connected)
            {
                _socket.Disconnect(true);
                _socket = null;
            }
        }

        public virtual void Send(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            try
            {
                _socket.Send(BitConverter.GetBytes(buffer.Length));
                _socket.Send(buffer);
            }
            catch
            {
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
