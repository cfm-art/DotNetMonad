using System;
using System.Threading.Tasks;

namespace CfmArt.Functional
{
    /// <summary></summary>
    public class MaybeTask
    {
        /// <summary></summary>
        public static MaybeTask<T> Return<T>(Task<Optional<T>> task)
            => new MaybeTask<T>(task);

        /// <summary></summary>
        public static MaybeTask<T> From<T>(Task<Optional<T>> task)
            => new MaybeTask<T>(task);

        /// <summary></summary>
        public static MaybeTask<T> Return<T>(Func<Task<Optional<T>>> task)
            => new MaybeTask<T>(task());

        /// <summary></summary>
        public static MaybeTask<T> From<T>(Func<Task<Optional<T>>> task)
            => new MaybeTask<T>(task());
    }

    /// <summary>
    /// Optionalを返すTask用。
    /// ツライTaskをなんとかするためのもの。
    /// </summary>
    public struct MaybeTask<T>
        : IMonad<T>
    {
        private Task<Optional<T>> awaitor_ { get; }
        /// <summary></summary>
        public Task<Optional<T>> Awaitor => awaitor_ ?? Task.FromResult(Optional<T>.Nothing);

        /// <summary></summary>
        public MaybeTask(Task<Optional<T>> task)
        {
            awaitor_ = task;
        }

        private async Task<Optional<(T, U)>> Next<U>(Task<Optional<U>> newState)
            => await (await Awaitor).IfPresent(
                    async current => (await newState).Map(next => (current, next)),
                    () => Task.FromResult(Optional.Nothing<(T, U)>()));

        /// <summary>
        /// Optionalを返すTaskをタプルに
        /// </summary>
        public MaybeTask<(T, U)> Map<U>(Task<Optional<U>> newState)
            => new MaybeTask<(T, U)>(Next(newState));

        private async Task<Optional<U>> Next<U>(Func<T, Task<Optional<U>>> newState)
            => await (await Awaitor).IfPresent(
                    current => newState(current),
                    () => Task.FromResult(Optional.Nothing<U>()));

        /// <summary>
        /// Optionalを返すTaskを独自の形式へ
        /// </summary>
        public MaybeTask<U> Map<U>(Func<T, Task<Optional<U>>> newState)
            => new MaybeTask<U>(Next(newState));

        private async Task<Optional<V>> Next<U, V>(
                Func<T, Task<Optional<U>>> runState,
                Func<T, U, Optional<V>> newState)
            => (await (await Awaitor).IfPresent(
                    async current => (await runState(current)).Map(next => (current, next)),
                    () => Task.FromResult(Optional.Nothing<(T, U)>())))
                .Bind(state => newState(state.Item1, state.Item2));

        /// <summary>
        /// ステートの更新と次のステートを分けたパターン
        /// </summary>
        public MaybeTask<V> Map<U, V>(
                Func<T, Task<Optional<U>>> runState,
                Func<T, U, Optional<V>> newState)
            => new MaybeTask<V>(Next(runState, newState));

        /// <summary></summary>
        public MaybeTask<U> Bind<U>(Func<T, MaybeTask<U>> func)
            => Map(t => func(t).Awaitor);

        /// <summary></summary>
        public IMonad<U> Bind<U>(Func<T, IMonad<U>> func)
            => Map(t => ((MaybeTask<U>) func(t)).Awaitor);

        /// <summary></summary>
        public IMonad<U> Fmap<U>(Func<T, U> func)
            => Map(t => Task.FromResult(Optional.Maybe(func(t))));

        /// <summary></summary>
        [Obsolete("Not Implemented.")]
        public MonadU Bind<U, MonadU>(Func<T, MonadU> func)
            where MonadU : struct, IMonad<U>
            => throw new NotImplementedException();
    }
}
