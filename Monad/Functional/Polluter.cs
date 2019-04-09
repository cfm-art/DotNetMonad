using CfmArt.Functional.Internal;

namespace CfmArt.Functional
{
    public class EitherPolluter<L, R>
    {
        private Either<L, R> either_ { get; }
        internal EitherPolluter(Either<L, R> either) => either_ = either;

        public L PolluteLeft() => either_.LeftValue;
        public R PolluteRight() => either_.RightValue;
    }

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
        /// Either用
        /// </summary>
        public static EitherPolluter<L, R> Pollute<L, R>(Either<L, R> either)
            => new EitherPolluter<L, R>(either);
    }
}