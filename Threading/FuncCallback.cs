using System;
using System.Threading;

namespace NationalInstruments.Aecorn.Threading
{
    internal class FuncCallback<TResult> : Callback
    {
        private readonly Func<TResult> func;
        public FuncResultToken<TResult> resultToken = new FuncResultToken<TResult>();

        public FuncCallback(Func<TResult> func)
        {
            this.func = func;
        }

        public override void Call()
        {
            resultToken.Result = func();
            resultToken.resetEvent.Set();
        }
    }

    internal class FuncCallback<T, TResult> : Callback
    {
        private readonly Func<T, TResult> func;
        private readonly T param1;
        public FuncResultToken<TResult> resultToken = new FuncResultToken<TResult>();

        public FuncCallback(Func<T, TResult> func, T param1)
        {
            this.param1 = param1;
            this.func = func;
        }

        public override void Call()
        {
            resultToken.Result = func(param1);
            resultToken.resetEvent.Set();
        }
    }

    internal class FuncCallback<T1, T2, TResult> : Callback
    {
        private readonly Func<T1, T2, TResult> func;
        private readonly T1 param1;
        private readonly T2 param2;
        public FuncResultToken<TResult> resultToken = new FuncResultToken<TResult>();

        public FuncCallback(Func<T1, T2, TResult> func, T1 param1, T2 param2)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.func = func;
        }

        public override void Call()
        {
            resultToken.Result = func(param1, param2);
            resultToken.resetEvent.Set();
        }
    }

    internal class FuncCallback<T1, T2, T3, TResult> : Callback
    {
        private readonly Func<T1, T2, T3, TResult> func;
        private readonly T1 param1;
        private readonly T2 param2;
        private readonly T3 param3;
        public FuncResultToken<TResult> resultToken = new FuncResultToken<TResult>();

        public FuncCallback(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.func = func;
        }

        public override void Call()
        {
            resultToken.Result = func(param1, param2, param3);
            resultToken.resetEvent.Set();
        }
    }

    internal class FuncCallback<T1, T2, T3, T4, TResult> : Callback
    {
        private readonly Func<T1, T2, T3, T4, TResult> func;
        private readonly T1 param1;
        private readonly T2 param2;
        private readonly T3 param3;
        private readonly T4 param4;
        public FuncResultToken<TResult> resultToken = new FuncResultToken<TResult>();

        public FuncCallback(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.func = func;
        }

        public override void Call()
        {
            resultToken.Result = func(param1, param2, param3, param4);
            resultToken.resetEvent.Set();
        }
    }

    internal class FuncCallback<T1, T2, T3, T4, T5, TResult> : Callback
    {
        private readonly Func<T1, T2, T3, T4, T5, TResult> func;
        private readonly T1 param1;
        private readonly T2 param2;
        private readonly T3 param3;
        private readonly T4 param4;
        private readonly T5 param5;
        public FuncResultToken<TResult> resultToken = new FuncResultToken<TResult>();

        public FuncCallback(Func<T1, T2, T3, T4, T5, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.param5 = param5;
            this.func = func;
        }

        public override void Call()
        {
            resultToken.Result = func(param1, param2, param3, param4, param5);
            resultToken.resetEvent.Set();
        }
    }
}
