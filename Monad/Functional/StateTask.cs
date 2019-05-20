using System;
using System.Threading.Tasks;

namespace CfmArt.Functional
{
    public class StateTask
    {
        public static StateTask<T> From<T>(Task<Optional<T>> task)
            => new StateTask<T>(task);
    }

    /// <summary>
    /// Optionalを返すTask
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

        private async Task<Optional<U>> Next<U>(Func<T, Task<Optional<U>>> newState)
            => await (await Awaitor).IfPresent(
                    current => newState(current),
                    () => Task.FromResult(Optional.Nothing<U>()));

        public StateTask<(T, U)> Map<U>(Task<Optional<U>> newState)
            => new StateTask<(T, U)>(Next(newState));

        public StateTask<U> Map<U>(Func<T, Task<Optional<U>>> newState)
            => new StateTask<U>(Next(newState));

        public IMonad<U> Bind<U>(Func<T, IMonad<U>> func)
            => Map(t => Task.FromResult((Optional<U>)func(t)));

        public IMonad<U> Fmap<U>(Func<T, U> func)
            => Map(t => Task.FromResult(Optional.Maybe(func(t))));
    }
}
