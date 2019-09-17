using System;

namespace NationalInstruments.Aecorn.Threading
{
    internal class ActionCallback : Callback
    {
        private readonly Action action;

        public ActionCallback(Action action)
        {
            this.action = action;
        }

        public override void Call()
        {
            action();
        }
    }

    internal class ActionCallback<T> : Callback
    {
        private readonly Action<T> action;
        private readonly T param1;

        public ActionCallback(Action<T> action, T param1)
        {
            this.param1 = param1;
            this.action = action;
        }

        public override void Call()
        {
            action(param1);
        }
    }

    internal class ActionCallback<T1, T2> : Callback
    {
        private readonly Action<T1, T2> action;
        private readonly T1 param1;
        private readonly T2 param2;

        public ActionCallback(Action<T1, T2> action, T1 param1, T2 param2)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.action = action;
        }

        public override void Call()
        {
            action(param1, param2);
        }
    }

    internal class ActionCallback<T1, T2, T3> : Callback
    {
        private readonly Action<T1, T2, T3> action;
        private readonly T1 param1;
        private readonly T2 param2;
        private readonly T3 param3;

        public ActionCallback(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.action = action;
        }

        public override void Call()
        {
            action(param1, param2, param3);
        }
    }

    internal class ActionCallback<T1, T2, T3, T4> : Callback
    {
        private readonly Action<T1, T2, T3, T4> action;
        private readonly T1 param1;
        private readonly T2 param2;
        private readonly T3 param3;
        private readonly T4 param4;

        public ActionCallback(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.action = action;
        }

        public override void Call()
        {
            action(param1, param2, param3, param4);
        }
    }

    internal class ActionCallback<T1, T2, T3, T4, T5> : Callback
    {
        private readonly Action<T1, T2, T3, T4, T5> action;
        private readonly T1 param1;
        private readonly T2 param2;
        private readonly T3 param3;
        private readonly T4 param4;
        private readonly T5 param5;

        public ActionCallback(Action<T1, T2, T3, T4, T5> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.param5 = param5;
            this.action = action;
        }

        public override void Call()
        {
            action(param1, param2, param3, param4, param5);
        }
    }
}
