using System;

/// <summary>
/// Callbacks help encapsule actions and their parameters.
/// </summary>

namespace NationalInstruments.Aecorn.Threading
{
    internal class Callback : ICallable
    {
        private readonly Action action;

        public Callback(Action action)
        {
            this.action = action;
        }

        public void Call()
        {
            action();
        }

        public static Callback New(Action action)
        {
            return new Callback(action);
        }
        
        public static Callback<T> New<T>(Action<T> action, T param1)
        {
            return new Callback<T>(action, param1);
        }

        public static Callback<T1, T2> New<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
        {
            return new Callback<T1, T2>(action, param1, param2);
        }

        public static Callback<T1, T2, T3> New<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
        {
            return new Callback<T1, T2, T3>(action, param1, param2, param3);
        }

        public static Callback<T1, T2, T3, T4> New<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return new Callback<T1, T2, T3, T4>(action, param1, param2, param3, param4);
        }

        public static Callback<T1, T2, T3, T4, T5> New<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return new Callback<T1, T2, T3, T4, T5>(action, param1, param2, param3, param4, param5);
        }
    }

    internal class Callback<T> : ICallable
    {
        private readonly Action<T> action;
        private readonly T param1;

        public Callback(Action<T> action, T param1)
        {
            this.param1 = param1;
            this.action = action;
        }

        public void Call()
        {
            action(param1);
        }
    }

    internal class Callback<T1, T2> : ICallable
    {
        private readonly Action<T1, T2> action;
        private readonly T1 param1;
        private readonly T2 param2;

        public Callback(Action<T1, T2> action, T1 param1, T2 param2)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.action = action;
        }

        public void Call()
        {
            action(param1, param2);
        }
    }

    internal class Callback<T1, T2, T3> : ICallable
    {
        private readonly Action<T1, T2, T3> action;
        private readonly T1 param1;
        private readonly T2 param2;
        private readonly T3 param3;

        public Callback(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.action = action;
        }

        public void Call()
        {
            action(param1, param2, param3);
        }
    }

    internal class Callback<T1, T2, T3, T4> : ICallable
    {
        private readonly Action<T1, T2, T3, T4> action;
        private readonly T1 param1;
        private readonly T2 param2;
        private readonly T3 param3;
        private readonly T4 param4;

        public Callback(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.action = action;
        }

        public void Call()
        {
            action(param1, param2, param3, param4);
        }
    }

    internal class Callback<T1, T2, T3, T4, T5> : ICallable
    {
        private readonly Action<T1, T2, T3, T4, T5> action;
        private readonly T1 param1;
        private readonly T2 param2;
        private readonly T3 param3;
        private readonly T4 param4;
        private readonly T5 param5;

        public Callback(Action<T1, T2, T3, T4, T5> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.param5 = param5;
            this.action = action;
        }

        public void Call()
        {
            action(param1, param2, param3, param4, param5);
        }
    }
}
