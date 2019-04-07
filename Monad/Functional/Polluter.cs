using CfmArt.Functional.Internal;

namespace CfmArt.Functional
{
    /// <summary>
    /// Monadに隠蔽されている値を取得する
    /// </summary>
    public static class Polluter
    {
        /// <summary>
        /// Monadに隠蔽されている値を取得する
        /// </summary>
        public static T Pollute<T>(IMonad<T> monad)
            => ((IPollutable<T>) monad).Pollute();

        /// <summary>
        /// Eitherの左を無理やり取得
        /// </summary>
        public static L PollluteLeft<L, R>(Either<L, R> either)
            => either.LeftValue;
    }
}