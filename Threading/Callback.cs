using System;
using System.Threading;

/// <summary>
/// Callbacks help encapsule actions and their parameters.
/// </summary>
namespace NationalInstruments.Aecorn.Threading
{
    internal abstract class Callback
    {
        public abstract void Call();

        public static ActionCallback New(Action action)
        {
            return new ActionCallback(action);
        }

        public static ActionCallback<T> New<T>(Action<T> action, T param1)
        {
            return new ActionCallback<T>(action, param1);
        }

        public static ActionCallback<T1, T2> New<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
        {
            return new ActionCallback<T1, T2>(action, param1, param2);
        }

        public static ActionCallback<T1, T2, T3> New<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
        {
            return new ActionCallback<T1, T2, T3>(action, param1, param2, param3);
        }

        public static ActionCallback<T1, T2, T3, T4> New<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return new ActionCallback<T1, T2, T3, T4>(action, param1, param2, param3, param4);
        }

        public static ActionCallback<T1, T2, T3, T4, T5> New<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return new ActionCallback<T1, T2, T3, T4, T5>(action, param1, param2, param3, param4, param5);
        }

        public static FuncCallback<TResult> New<TResult>(Func<TResult> func)
        {
            return new FuncCallback<TResult>(func);
        }

        public static FuncCallback<T, TResult> New<T, TResult>(Func<T, TResult> func, T param1)
        {
            return new FuncCallback<T, TResult>(func, param1);
        }

        public static FuncCallback<T1, T2, TResult> New<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2)
        {
            return new FuncCallback<T1, T2, TResult>(func, param1, param2);
        }

        public static FuncCallback<T1, T2, T3, TResult> New<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3)
        {
            return new FuncCallback<T1, T2, T3, TResult>(func, param1, param2, param3);
        }

        public static FuncCallback<T1, T2, T3, T4, TResult> New<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return new FuncCallback<T1, T2, T3, T4, TResult>(func, param1, param2, param3, param4);
        }

        public static FuncCallback<T1, T2, T3, T4, T5, TResult> New<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return new FuncCallback<T1, T2, T3, T4, T5, TResult>(func, param1, param2, param3, param4, param5);
        }
    }
}
