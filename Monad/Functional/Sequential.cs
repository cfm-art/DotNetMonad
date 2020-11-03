using System;
using System.Threading.Tasks;
using CfmArt.Functional.Internal;

namespace CfmArt.Functional
{
    internal static class Sequential
    {
        public static Sequential<T, U> Continue<T, U>(T value, TypeMarker<U> _)
            => new Sequential<T, U>(value);

        public static Sequential<U, T> Break<U, T>(TypeMarker<U> _, T value)
            => new Sequential<U, T>(value, false);
    }

    internal static class Sequential<U>
    {
        public static Sequential<T, U> Continue<T>(T value)
            => new Sequential<T, U>(value);

        public static Sequential<U, T> Break<T>(T value)
            => new Sequential<U, T>(value, false);
    }

    internal struct Sequential<Suc, Err>
        : IMonad<Suc>
    {
        private bool IsContinue { get; }
        private Suc? Value { get; }
        private Err? Error { get; }

        internal Sequential(Suc value)
        {
            IsContinue = true;
            Value = value;
            Error = default(Err);
        }

        internal Sequential(Err value, bool _)
        {
            IsContinue = false;
            Value = default(Suc);
            Error = value;
        }

        public T IfSuccess<T>(Func<Suc, T> suc, Func<Err, T> err)
            => IsContinue ? NullCheck.DoAction(Value, suc) : NullCheck.DoAction(Error, err);

        public Sequential<Suc, Err> Bind(Func<Suc, Sequential<Suc, Err>> func)
            => IsContinue ? NullCheck.DoAction(Value, func) : this;

        public Sequential<Next, Err> Bind<Next>(Func<Suc, Sequential<Next, Err>> func)
            => IsContinue ? NullCheck.DoAction(Value, func) : NullCheck.DoAction(Error, e => Sequential<Next>.Break(e));

        public Sequential<Suc, Err> Bind(Func<Suc, Suc> func)
            => IsContinue ? Sequential<Err>.Continue(NullCheck.DoAction(Value, func)) : this;

        public IMonad<U> Bind<U>(Func<Suc, IMonad<U>> func)
            => IsContinue ? NullCheck.DoAction(Value, func) : NullCheck.DoAction(Error, e => Sequential<U>.Break(e));

        public MonadU Bind<U, MonadU>(Func<Suc, MonadU> func)
            where MonadU : struct, IMonad<U>
            => IsContinue ? NullCheck.DoAction(Value, func) : default(MonadU);

        public Task<Sequential<Suc, Err>> BindAsync(Func<Suc, Task<Sequential<Suc, Err>>> func)
            => IsContinue ? NullCheck.DoAction(Value, func) : Task.FromResult(this);

        public Task<Sequential<Next, Err>> BindAsync<Next>(Func<Suc, Task<Sequential<Next, Err>>> func)
            => IsContinue ? NullCheck.DoAction(Value, func) : Task.FromResult(NullCheck.DoAction(Error, e => Sequential<Next>.Break(e)));

        public async Task<Sequential<Suc, Err>> BindAsync(Func<Suc, Task<Suc>> func)
            => IsContinue ? Sequential<Err>.Continue(await NullCheck.DoAction(Value, func)) : this;

        public Task<IMonad<U>> BindAsync<U>(Func<Suc, Task<IMonad<U>>> func)
            => IsContinue ? NullCheck.DoAction(Value, func) : Task.FromResult((IMonad<U>) NullCheck.DoAction(Error, e => Sequential<U>.Break(e)));

        public Task<MonadU> BindAsync<U, MonadU>(Func<Suc, Task<MonadU>> func)
            where MonadU : struct, IMonad<U>
            => IsContinue ? NullCheck.DoAction(Value, func) : Task.FromResult(default(MonadU));

        public IMonad<U> Fmap<U>(Func<Suc, U> func)
            => IsContinue ? Sequential<Err>.Continue(NullCheck.DoAction(Value, func)) : NullCheck.DoAction(Error, e => Sequential<U>.Break(e));
    }
}