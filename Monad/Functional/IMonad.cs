using System;

namespace Functional
{
    /// <summary>
    /// Monadを無理やり抽象化
    /// </summary>
    public interface IMonad<T>
    {
        /// <summary>
        /// (>>=) :: m a -> (a -> m b) -> m b
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        IMonad<U> Bind<U>(Func<T, IMonad<U>> func);

        /// <summary>
        /// fmap :: (a -> b) -> m a -> m b
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        IMonad<U> Fmap<U>(Func<T, U> func);
    }
}
