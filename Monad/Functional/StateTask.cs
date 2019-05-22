using System;
using System.Threading.Tasks;

namespace CfmArt.Functional
{
    public class StateTask
    {
        public static StateTask<T> Return<T>(Task<Optional<T>> task)
            => new StateTask<T>(task);

        public static StateTask<T> From<T>(Task<Optional<T>> task)
            => new StateTask<T>(task);
    }

    /// <summary>
    /// Optionalを返すTask用。
    /// ツライTaskをなんとかするためのもの。
    /// </summary>
    public class StateTask<T>
        : IMonad<T>
    {
        public Task<Optional<T>> Awaitor { get; }

        public StateTask(Task<Optional<T>> task)
        {
            Awaitor = task;
        }

        private async Task<Optional<(T, U)>> Next<U>(Task<Optional<U>> newState)
            => await (await Awaitor).IfPresent(
                    async current => (await newState).Map(next => (current, next)),
                    () => Task.FromResult(Optional.Nothing<(T, U)>()));

        /// <summary>
        /// Optionalを返すTaskをタプルに
        /// </summary>
        public StateTask<(T, U)> Map<U>(Task<Optional<U>> newState)
            => new StateTask<(T, U)>(Next(newState));

        private async Task<Optional<U>> Next<U>(Func<T, Task<Optional<U>>> newState)
            => await (await Awaitor).IfPresent(
                    current => newState(current),
                    () => Task.FromResult(Optional.Nothing<U>()));

        /// <summary>
        /// Optionalを返すTaskを独自の形式へ
        /// </summary>
        public StateTask<U> Map<U>(Func<T, Task<Optional<U>>> newState)
            => new StateTask<U>(Next(newState));

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
        public StateTask<V> Map<U, V>(
                Func<T, Task<Optional<U>>> runState,
                Func<T, U, Optional<V>> newState)
            => new StateTask<V>(Next(runState, newState));

        public IMonad<U> Bind<U>(Func<T, IMonad<U>> func)
            => Map(t => ((StateTask<U>) func(t)).Awaitor);

        public IMonad<U> Fmap<U>(Func<T, U> func)
            => Map(t => Task.FromResult(Optional.Maybe(func(t))));
    }
}
