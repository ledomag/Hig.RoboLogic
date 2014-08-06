namespace Hig.ScriptEngine.Operations
{
    using System.Collections.Generic;
    using Hig.Compiler;

    public class Function : Operation
    {
        protected bool _isInit = true;    // It shows update method is colling at the firts time;
        protected Dictionary<string, IDataValue> _variables = new Dictionary<string, IDataValue>();

        protected override void Action(Operation operation, IDataValue[] values)
        {
            IsCompleted = true;
        }

        public override Function GetParentFunction()
        {
            return this;
        }

        public virtual IDataValue GetVariable(string name)
        {
            return _variables[name];
        }

        public virtual void AddVariable(string name, IDataValue value)
        {
            if (_variables.ContainsKey(name))
                _variables.Remove(name);

            _variables.Add(name, value);
        }

        public virtual bool RemoveVariable(string name)
        {
            return _variables.Remove(name);
        }

        public override void Update()
        {
            // Update variables only at the first time;
            if (_isInit)
            {
                _variables.Clear();

                if (Parent != null)
                {
                    var parentVariables = ((Function)Parent.GetParentFunction())._variables;

                    if (parentVariables != null)
                        foreach (var item in parentVariables)
                            _variables.Add(item.Key, item.Value);
                }

                _isInit = false;
            }

            base.Update();
        }
    }
}
