using System;
using System.Threading.Tasks;

namespace CfmArt.Functional
{
    public class StateTask
    {
        public static StateTask<T> Return<T>(Task<T> task)
            => new StateTask<T>(task);

        public static StateTask<T> From<T>(Task<T> task)
            => new StateTask<T>(task);

        public static StateTask<T> Return<T>(Func<Task<T>> task)
            => new StateTask<T>(task());

        public static StateTask<T> From<T>(Func<Task<T>> task)
            => new StateTask<T>(task());
    }

    /// <summary>
    /// ツライTaskをなんとかするためのもの。
    /// </summary>
    public struct StateTask<T>
        : IMonad<T>
    {
        private Task<T> awaitor_ { get; }
        public Task<T> Awaitor => awaitor_ ?? Task.FromResult(default(T));

        public StateTask(Task<T> task)
        {
            awaitor_ = task;
        }

        private async Task<(T, U)> Next<U>(Task<U> newState)
            => (await Awaitor, await newState);

        /// <summary>
        /// 
        /// </summary>
        public StateTask<(T, U)> Map<U>(Task<U> newState)
            => new StateTask<(T, U)>(Next(newState));

        private async Task<U> Next<U>(Func<T, Task<U>> newState)
            => await newState(await Awaitor);

        /// <summary>
        /// Optionalを返すTaskを独自の形式へ
        /// </summary>
        public StateTask<U> Map<U>(Func<T, Task<U>> newState)
            => new StateTask<U>(Next(newState));

        private async Task<V> Next<U, V>(
                Func<T, Task<U>> runState,
                Func<T, U, V> newState)
        {
            var current = await Awaitor;
            return newState(current, await runState(current));
        }

        /// <summary>
        /// ステートの更新と次のステートを分けたパターン
        /// </summary>
        public StateTask<V> Map<U, V>(
                Func<T, Task<U>> runState,
                Func<T, U, V> newState)
            => new StateTask<V>(Next(runState, newState));

        public StateTask<U> Bind<U>(Func<T, StateTask<U>> func)
            => Map(t => func(t).Awaitor);

        public IMonad<U> Bind<U>(Func<T, IMonad<U>> func)
            => Map(t => ((StateTask<U>) func(t)).Awaitor);

        public StateTask<U> BindAsync<U>(Func<T, Task<U>> func)
            => Map(func);

        public Task<IMonad<U>> BindAsync<U>(Func<T, Task<IMonad<U>>> func)
            => Map(t => func(t)).Awaitor;

        public IMonad<U> Fmap<U>(Func<T, U> func)
            => Map(t => Task.FromResult(func(t)));

        public MonadU Bind<U, MonadU>(Func<T, MonadU> func)
            where MonadU : IMonad<U>
            => throw new NotImplementedException();

        public Task<MonadU> BindAsync<U, MonadU>(Func<T, Task<MonadU>> func)
            where MonadU : IMonad<U>
            => Map(func).Awaitor;
    }
}
