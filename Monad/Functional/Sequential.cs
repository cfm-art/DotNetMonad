using System;
using System.Threading.Tasks;

namespace CfmArt.Functional
{
    public static class Sequential<U>
    {
        public static Sequential<T, U> Continue<T>(T value)
            => new Sequential<T, U>(value);

        public static Sequential<U, T> Break<T>(T value)
            => new Sequential<U, T>(value, false);
    }

    public struct Sequential<Suc, Err>
        : IMonad<Suc>
    {
        private bool IsContinue { get; }
        private Suc Value { get; }
        private Err Error { get; }

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
            => IsContinue ? suc(Value) : err(Error);

        public Sequential<Suc, Err> Bind(Func<Suc, Sequential<Suc, Err>> func)
            => IsContinue ? func(Value) : this;

        public Sequential<Suc, Err> Bind(Func<Suc, Suc> func)
            => IsContinue ? Sequential<Err>.Continue(func(Value)) : this;

        public IMonad<U> Bind<U>(Func<Suc, IMonad<U>> func)
            => IsContinue ? func(Value) : Sequential<U>.Break(Error);

        public MonadU Bind<U, MonadU>(Func<Suc, MonadU> func)
            where MonadU : IMonad<U>
            => IsContinue ? func(Value) : default(MonadU);

        public Task<Sequential<Suc, Err>> BindAsync(Func<Suc, Task<Sequential<Suc, Err>>> func)
            => IsContinue ? func(Value) : Task.FromResult(this);

        public async Task<Sequential<Suc, Err>> BindAsync(Func<Suc, Task<Suc>> func)
            => IsContinue ? Sequential<Err>.Continue(await func(Value)) : this;

        public Task<IMonad<U>> BindAsync<U>(Func<Suc, Task<IMonad<U>>> func)
            => IsContinue ? func(Value) : Task.FromResult((IMonad<U>) Sequential<U>.Break(Error));

        public Task<MonadU> BindAsync<U, MonadU>(Func<Suc, Task<MonadU>> func)
            where MonadU : IMonad<U>
            => IsContinue ? func(Value) : Task.FromResult(default(MonadU));

        public IMonad<U> Fmap<U>(Func<Suc, U> func)
            => IsContinue ? Sequential<Err>.Continue(func(Value)) : Sequential<U>.Break(Error);
    }
}