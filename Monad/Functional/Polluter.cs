using Functional.Internal;

namespace Functional
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
    }
}