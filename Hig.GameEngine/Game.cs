namespace Hig.GameEngine
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class Game : IDisposable
    {
        private PresentationParameters _presentParams;
        private readonly Stopwatch _stopwatch;
        protected GraphicsDevice _graphicsDevice;

        private bool _isContinue;
        private Thread _thread = null;
        private uint _msec;

        protected uint _interval = 0;
        public uint Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        protected abstract void LoadContent();
        protected abstract void UnloadContent();
        protected abstract void Update(uint milliseconds);
        protected abstract void Draw(uint milliseconds);

        private readonly IntPtr _canvasHandle;
        private int _backBufferWidth;
        private int _backBufferHeight;

        public Game(IntPtr canvasHandle, int backBufferWidth, int backBufferHeight, uint interval = 16)
        {
            _interval = interval;
            _stopwatch = new Stopwatch();

            _canvasHandle = canvasHandle;
            _backBufferWidth = backBufferWidth;
            _backBufferHeight = backBufferHeight;

            UpdateGraphicsDevice();
        }

        private void CreateGraphicsDevice()
        {
            // Try to create the graphics device during 10 seconds.
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    _graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, _presentParams);

                    return;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }

            StopMainLoop();
        }

        private void UpdateGraphicsDevice()
        {
            _msec = 0;
            _presentParams = new PresentationParameters();
            _presentParams.IsFullScreen = false;
            _presentParams.BackBufferWidth = _backBufferWidth;
            _presentParams.BackBufferHeight = _backBufferHeight;
            _presentParams.DeviceWindowHandle = _canvasHandle;

            if (_graphicsDevice != null)
            {
                UnloadContent();
                _graphicsDevice.Dispose();
            }

            CreateGraphicsDevice();
            LoadContent();
        }

        private void Loop()
        {
            while (_isContinue)
            {
                _stopwatch.Restart();

                while (_msec >= _interval)
                {
                    Update(_interval);
                    _msec -= _interval;
                }

                Draw(_msec);

                try
                {
                    _graphicsDevice.Present();
                }
                catch (DeviceLostException)
                {
                    UpdateGraphicsDevice();

                    continue;
                }

                _stopwatch.Stop();
                _msec += (uint)_stopwatch.ElapsedMilliseconds;
            }
        }

        public void StratMainLoop()
        {
            if (_thread != null)
                throw new InvalidOperationException("Game has started yet.");

            _msec = 0;
            _isContinue = true;

            _thread = new Thread(Loop);
            _thread.Start();
        }

        public void StopMainLoop()
        {
            if (_thread != null)
            {
                _isContinue = false;
                _thread.Abort();
                _thread = null;
            }
        }

        public virtual void Dispose()
        {
            StopMainLoop();
            UnloadContent();

            if (_graphicsDevice != null)
                _graphicsDevice.Dispose();
        }
    }
}
