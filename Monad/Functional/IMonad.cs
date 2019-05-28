using System;

namespace CfmArt.Functional
{
    public static class Monad
    {
        public static T Id<T>(T t) => t;
    }

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
        /// (>>=) :: m a -> (a -> m b) -> m b
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="MonadU"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        MonadU Bind<U, MonadU>(Func<T, MonadU> func)
            where MonadU : IMonad<U>;

        /// <summary>
        /// fmap :: (a -> b) -> m a -> m b
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        IMonad<U> Fmap<U>(Func<T, U> func);

/*
        /// <summary>
        /// fmap :: (a -> b) -> m a -> m b
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="MonadU"></typeparam>
        /// <param name="func"></param>
        /// <param name="_"></param>
        /// <returns></returns>
        MonadU Fmap<U, MonadU>(Func<T, U> func, TypeMarker<MonadU> _ = null)
            where MonadU : IMonad<U>;
*/
    }
}
