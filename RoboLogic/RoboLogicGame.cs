using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Hig.Compiler;
using Hig.GameEngine;
using Hig.GameEngine.GameObjects;
using Hig.GameEngine.Graphics;
using Hig.ScriptEngine;
using Hig.ScriptEngine.Operations;
using RoboLogic.GameObjects;

namespace RoboLogic
{
    public class RoboLogicGame : Game, INotifyPropertyChanged
    {
        private const float _scale = 1.0f;

        private readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private readonly ushort _mapWidth = (ushort)(Cell.Width * 2 * _scale);
        private readonly ushort _mapHeight = (ushort)(Cell.Height * _scale);

        private SpriteBatch _spriteBatch;
        private PrimitiveBatch _primitiveBatch;

        private int _scrX = 220;
        private int _scrY = 150;

        private Map _map;
        private Robot[] _robots = new Robot[2];
        private readonly Dictionary<IStatusObject, IDataValue> _winConditions = new Dictionary<IStatusObject, IDataValue>();
        private Round _round;
        public bool IsActive = true;

        public int CurrentRobotIndex { get; set; }

        private bool _scriptIsCompleted = true;
        public bool ScriptIsCompleted
        {
            get { return _scriptIsCompleted; }

            protected set
            {
                if (_scriptIsCompleted != value)
                {
                    _scriptIsCompleted = value;
                    OnPropertyChanged("ScriptIsCompleted");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<string> SetMessage;
        public event Action Win;

        public RoboLogicGame(IntPtr canvasHandle, int backBufferWidth, int backBufferHeight)
            : base(canvasHandle, backBufferWidth, backBufferHeight)
        {
            Initialize();
        }

        private Texture2D LoadTexture(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
                return Texture2D.FromStream(_graphicsDevice, fs);
        }

        private bool IsEmptyCell(Cell cell)
        {
            foreach (var item in cell.GameObjects)
                if (!(item is Button))
                    return false;

            return true;
        }

        #region " Methods for script engine "

        private void Move(Operation operation, IDataValue[] values)
        {
            Robot robot = _robots[CurrentRobotIndex];

            if (robot.MoveOperation == null)
            {
                Point? nextMapPos = _map.CalcMapPosition(robot.CalcNextWayPoint());

                if (nextMapPos != null && IsEmptyCell(_map[(Point)nextMapPos]))
                {
                    foreach (var obj in _map[(Point)_map.GetMapPosition(robot)].GameObjects)
                        if (obj is Button)
                            ((Button)obj).Release();

                    robot.MoveOperation = operation;
                    robot.StartMove();

                    return;
                }

                robot.Script.Stop();
                OnSetMessage("Robot can not move. Program was aborted.");
                operation.IsCompleted = true;
                operation.Value = new DataValue(false);
            }
        }

        private void TurnLeft(Operation operation, IDataValue[] values)
        {
            Robot robot = _robots[CurrentRobotIndex];
            short angle = (short)(robot.Angle - 90);

            if (angle == -90)
                angle = 270;

            robot.Angle = angle;

            operation.IsCompleted = true;
            operation.Value = new DataValue(true);
        }

        private void TurnRight(Operation operation, IDataValue[] values)
        {
            Robot robot = _robots[CurrentRobotIndex];
            short angle = (short)(robot.Angle + 90);

            if (angle == 360)
                angle = 0;

            robot.Angle = angle;

            operation.IsCompleted = true;
            operation.Value = new DataValue(true);
        }

        private bool UseTerminal(Action<IUsableObject> action, string error)
        {
            Robot robot = _robots[CurrentRobotIndex];

            bool result = false;
            Point? nextMapPos = _map.CalcMapPosition(robot.CalcNextWayPoint());

            if (nextMapPos != null)
            {
                foreach (var obj in _map[(Point)nextMapPos].GameObjects)
                {
                    IUsableObject tmpObj = obj as IUsableObject;

                    if (tmpObj != null && tmpObj.Status)
                    {
                        action(tmpObj);
                        result = true;
                        break;
                    }
                }
            }

            if (!result)
            {
                robot.Script.Stop();
                OnSetMessage(error);

                return false;
            }

            return true;
        }

        private void Read(Operation operation, IDataValue[] values)
        {
            IDataValue value = new DataValue(null);
            UseTerminal(o => 
            {
                value = o.Value;
                //OnSetMessage("Value read" + Environment.NewLine + value.ToString());
            }, "Robot can not read. Program was aborted.");

            operation.Value = value;
            operation.IsCompleted = true;
        }

        private void Write(Operation operation, IDataValue[] values)
        {
            bool result = UseTerminal(o => o.Value = values[0], "Robot can not write. Program was aborted.");

            operation.Value = new DataValue(result);
            operation.IsCompleted = true;
        }

        #endregion

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnSetMessage(string message)
        {
            IsActive = false;

            if (SetMessage != null)
                SetMessage(message);
        }

        protected virtual void OnWin()
        {
            if (Win != null)
                Win();
        }

        private void InitRound(Round round)
        {
            _round = round;

            _map = new Map((ushort)_round.MapSize.X, (ushort)_round.MapSize.Y, new Animation(_textures["Ground"], 64, 32, 0, Color.Green));
            _winConditions.Clear();

            int robotIndex = 0;

            string[] typeNames =
            {
                typeof(Robot).ToString(),
                typeof(Terminal).ToString(),
                typeof(Button).ToString()
            };

            for (int i = 0; i < _round.Data.Length; i++)
            {
                var data = _round.Data[i];
                IGameObject obj = null;
                Position pos = new Position(data.Position.X, data.Position.Y);

                if (data.TypeName == typeNames[0])
                {
                    if (robotIndex > 1)
                        throw new InvalidOperationException("Invalid round.");

                    Color color = (robotIndex == 0) ? round.Player1Color : round.Player2Color;

                    Robot robot = new Robot(new Animation(_textures["Robot"], 32, 32, 10f, new Point(16, -10), color, true), pos, 30f);
                    robot.FromBytes(data.Buffer);
                    _robots[robotIndex] = robot;
                    obj = robot;
                    
                    _map.RegistrObject(robot);
                    robotIndex++;
                }
                else if (data.TypeName == typeNames[1])
                {
                    Terminal terminal = new Terminal(new Animation(_textures["Terminal"], 32, 32, 10f, new Point(16, -10), true), pos);
                    terminal.FromBytes(data.Buffer);
                    obj = terminal;

                    _map.RegistrObject(terminal);
                }
                else if (data.TypeName == typeNames[2])
                {
                    Button button = new Button(new Animation(_textures["Button"], 64, 32, 10f, true), pos, _map);
                    button.FromBytes(data.Buffer);
                    obj = button;

                    _map.RegistrObject(button);
                }

                if (obj is IStatusObject && _round.WinConditions.ContainsKey(i))
                    _winConditions.Add((IStatusObject)obj, _round.WinConditions[i]);
            }
        }

        #region " Game methods "

        private void Initialize()
        {
            Script.RegisterOperation("move", Move);
            Script.RegisterOperation("turnleft", TurnLeft);
            Script.RegisterOperation("turnright", TurnRight);
            Script.RegisterOperation("read", Read);
            Script.RegisterOperation("write", Write, true);

            _map = new Map(8, 8, new Animation(_textures["Ground"], 64, 32, 0, Color.Green));
        }

        protected override void LoadContent()
        {
            _textures.Clear();
            _textures.Add("Robot", LoadTexture(@"Content\Robot.png"));
            _textures.Add("Ground", LoadTexture(@"Content\Ground.png"));
            _textures.Add("Terminal", LoadTexture(@"Content\Terminal.png"));
            _textures.Add("Button", LoadTexture(@"Content\Button.png"));

            for (int i = 0; i < _robots.Length; i++)
                if (_robots[i] != null)
                    _robots[i].Animation.Texture = _textures["Robot"];

            if (_map != null)
                _map.Animation.Texture = _textures["Ground"];

            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _primitiveBatch = new PrimitiveBatch(_graphicsDevice);
        }

        protected override void UnloadContent()
        {
            if (_primitiveBatch != null)
                _primitiveBatch.Dispose();

            if (_spriteBatch != null)
                _spriteBatch.Dispose();

            foreach (var item in _textures)
                item.Value.Dispose();
        }

        protected override void Update(uint milliseconds)
        {
            if (IsActive)
            {
                #region " Check win "

                bool win = _winConditions.Count > 0;

                foreach (var condition in _winConditions)
                {
                    if (!condition.Value.Equals(condition.Key.Value))
                    {
                        win = false;
                        break;
                    }
                }

                if (win)
                {
                    OnWin();
                    _winConditions.Clear();
                }

                #endregion

                Robot robot = _robots[CurrentRobotIndex];

                if (robot != null)
                {
                    robot.Script.Update();
                    robot.Move(milliseconds);

                    if (!robot.IsMoving && robot.MoveOperation != null)
                    {
                        robot.MoveOperation.IsCompleted = true;
                        robot.MoveOperation.Value = new DataValue(true);
                        robot.MoveOperation = null;

                        _map.RegistrObject(robot);

                        foreach (var obj in _map[(Point)_map.GetMapPosition(robot)].GameObjects)
                            if (obj is Button)
                                ((Button)obj).Press();
                    }

                    ScriptIsCompleted = robot.Script.IsCompleted;
                }
            }
        }

        private void DrawObjects(SpriteBatch spriteBatch, Cell cell)
        {
            for (int i = 0; i < cell.GameObjects.Count; i++)
            {
                IGameObject obj = cell.GameObjects[i];
                Position rPos = obj.Position.ToIsometric();

                spriteBatch.Draw(
                    obj.Animation,
                    new Rectangle(
                        _scrX + (int)((rPos.X + obj.Animation.DrawOffset.X) * _scale),
                        _scrY + (int)((rPos.Y + obj.Animation.DrawOffset.Y) * _scale),
                        (int)(obj.Animation.FrameWidth * _scale),
                        (int)(obj.Animation.FrameHeight * _scale)),
                    obj.Animation.Frame,
                    obj.Animation.Color);
            }
        }

        protected override void Draw(uint milliseconds)
        {
            _graphicsDevice.Clear(Color.SteelBlue);

            _spriteBatch.Begin();

            #region " Drawing map "

            for (int y = 0; y < _map.Height; y++)
            {
                for (int x = 0; x < _map.Width; x++)
                {
                    Cell cell = _map[x, y];
                    Position pos = Position.ConvertToIsometric(x * _mapWidth, y * _mapHeight);

                    _spriteBatch.Draw(
                        cell.Ground,
                        new Rectangle(_scrX + (int)pos.X, _scrY + (int)pos.Y, _mapWidth, _mapHeight),
                        cell.Ground.Frame,
                        cell.Ground.Color);
                }
            }

            #endregion

            #region " Drawing objects "

            for (int i = 0; i < _map.Height; i++)
                for (int j = 0; i + j < _map.Height; j++)
                    DrawObjects(_spriteBatch, _map[i + j, j]);

            for (int i = 1; i < _map.Width; i++)
                for (int j = 0; i + j < _map.Width; j++)
                    DrawObjects(_spriteBatch, _map[j, i + j]);

            #endregion

            _spriteBatch.End();
        }

        #endregion

        public void NewGame(Round round)
        {
            InitRound(round);
        }

        public Error[] SetCode(string text)
        {
            _robots[CurrentRobotIndex].Script.Compile(text);

            return _robots[CurrentRobotIndex].Script.Errors;
        }

        public void StopScript()
        {
            _robots[CurrentRobotIndex].Script.Stop();
        }
    }
}
