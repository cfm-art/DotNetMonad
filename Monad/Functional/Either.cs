using System;
using System.Threading.Tasks;
using CfmArt.Functional.Internal;

namespace CfmArt.Functional
{
    /// <summary>
    /// either
    /// </summary>
    /// <typeparam name="L"></typeparam>
    /// <typeparam name="R"></typeparam>
    public struct Either<L, R>
        : IMonad<R>
        , IEquatable<Either<L, R>>
        , IEquatable<R>
        , IPollutable<R>
    {
#if false
        #region if-else
        public interface RightSentence
        {
            void Else(Action<R> onElse);
        }

        public interface RightSentence<U>
        {
            U Else(Func<R, U> onElse);
        }

        class DoRight
            : RightSentence
        {
            private R Value { get; }
            public DoRight(R value) => Value = value;
            public void Else(Action<R> onElse) => onElse(Value);
        }

        class DoRight<U>
            : RightSentence<U>
        {
            private R Value { get; }
            public DoRight(R value) => Value = value;
            public U Else(Func<R, U> onElse) => onElse(Value);
        }

        class DoNotDoRight
            : RightSentence
        {
            public void Else(Action<R> _) { }
        }

        class DoNotDoRight<U>
            : RightSentence<U>
        {
            private U Value { get; }
            public DoNotDoRight(U v) => Value = v;
            public U Else(Func<R, U> _) => Value;
        }

        public interface LeftSentence
        {
            void Else(Action<L> onElse);
        }

        public interface LeftSentence<U>
        {
            U Else(Func<L, U> onElse);
        }

        class DoLeft
            : LeftSentence
        {
            private L Value { get; }
            public DoLeft(L value) => Value = value;
            public void Else(Action<L> onElse) => onElse(Value);
        }

        class DoLeft<U>
            : LeftSentence<U>
        {
            private L Value { get; }
            public DoLeft(L value) => Value = value;
            public U Else(Func<L, U> onElse) => onElse(Value);
        }

        class DoNotDoLeft
            : LeftSentence
        {
            public void Else(Action<L> _) { }
        }

        class DoNotDoLeft<U>
            : LeftSentence<U>
        {
            private U Value { get; }
            public DoNotDoLeft(U v) => Value = v;
            public U Else(Func<L, U> _) => Value;
        }
        #endregion
#endif
        internal L? LeftValue { get; }
        internal R? RightValue { get; }
        private bool IsRight { get; }
        private bool IsLeft => !IsRight;

        private Either(L? left, R? right, bool isLeft)
        {
            LeftValue = left;
            RightValue = right;
            IsRight = !isLeft;
        }

        #region if left
#if false
        /// <summary>
        /// 中身があれば処理を行う
        /// </summary>
        /// <param name="action"></param>
        public RightSentence IfLeft(Action<L> action)
        {
            if (IsLeft)
            {
                action(LeftValue);
                return new DoNotDoRight();
            }
            return new DoRight(RightValue);
        }

        /// <summary>
        /// 中身があれば処理を行う
        /// </summary>
        /// <param name="action"></param>
        public RightSentence<U> IfLeft<U>(Func<L, U> action)
        {
            if (IsLeft)
            {
                return new DoNotDoRight<U>(action(LeftValue));
            }
            return new DoRight<U>(RightValue);
        }
#endif

        /// <summary>
        /// 中身がある時と無い時で処理を行う
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public void IfLeft(Action<L> then, Action<R> elseThen)
        {
            if (IsLeft) { NullCheck.DoAction(LeftValue, then); }
            else { NullCheck.DoAction(RightValue, elseThen); }
        }

        /// <summary>
        /// 中身がある時と無い時で処理を行う
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public U IfLeft<U>(Func<L, U> then, Func<R, U> elseThen)
            => IsLeft
                ? NullCheck.DoAction(LeftValue, then)
                : NullCheck.DoAction(RightValue, elseThen);
        #endregion

        #region if right
#if false
        /// <summary>
        /// 中身があれば処理を行う
        /// </summary>
        /// <param name="action"></param>
        public LeftSentence IfRight(Action<R> action)
        {
            if (IsRight)
            {
                action(RightValue);
                return new DoNotDoLeft();
            }
            return new DoLeft(LeftValue);
        }

        /// <summary>
        /// 中身があれば処理を行う
        /// </summary>
        /// <param name="action"></param>
        public LeftSentence<U> IfRight<U>(Func<R, U> action)
        {
            if (IsRight)
            {
                return new DoNotDoLeft<U>(action(RightValue));
            }
            return new DoLeft<U>(LeftValue);
        }
#endif

        /// <summary>
        /// 中身がある時と無い時で処理を行う
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public void IfRight(Action<R> then, Action<L> elseThen)
        {
            if (IsRight) { NullCheck.DoAction(RightValue, then); }
            else { NullCheck.DoAction(LeftValue, elseThen); }
        }

        /// <summary>
        /// 中身がある時と無い時で処理を行う
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public U IfRight<U>(Func<R, U> then, Func<L, U> elseThen)
            => IsRight
                ? NullCheck.DoAction(RightValue, then)
                : NullCheck.DoAction(LeftValue, elseThen);
        #endregion

        /// <summary>
        /// ←
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static Either<L, R> Left(L left)
            => new Either<L, R>(left, default(R), true);

        /// <summary>
        /// →
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Either<L, R> Right(R right)
            => new Either<L, R>(default(L), right, false);

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Either<L, R>(L value) => Left(value);

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Either<L, R>(R value) => Right(value);

        #region IEquatable
        /// <summary></summary>
        public override int GetHashCode()
            => (IsLeft
                ? LeftValue?.GetHashCode()
                : RightValue?.GetHashCode()) ?? 0;

        /// <summary></summary>
        public override bool Equals(object? obj)
        {
            if (obj == null) { return false;  }
            return Equals((Either<L, R>) obj);
        }

        /// <summary></summary>
        public bool Equals(Either<L, R> other)
        {
            if (IsLeft != other.IsLeft) { return false; }
            return IsLeft
                ? (LeftValue?.Equals(other.LeftValue) ?? false)
                : (RightValue?.Equals(other.RightValue) ?? false);
        }

        /// <summary></summary>
        public bool Equals(L other)
        {
            if (IsRight) { return false; }
            return LeftValue?.Equals(other) ?? false;
        }

        /// <summary></summary>
        public bool Equals(R? other)
        {
            if (IsLeft) { return false; }
            return RightValue?.Equals(other) ?? false;
        }
        #endregion

        #region IMonad
        /// <summary></summary>
        public IMonad<U> Bind<U>(Func<R, IMonad<U>> func)
            => IsRight ? NullCheck.DoAction(RightValue, func) : NullCheck.DoAction(LeftValue, l => Either<L, U>.Left(l));

        /// <summary></summary>
        public Either<L, U> Bind<U>(Func<R, Either<L, U>> func)
            => IsRight ? NullCheck.DoAction(RightValue, func) : NullCheck.DoAction(LeftValue, l => Either<L, U>.Left(l));

        /// <summary></summary>
        public Task<IMonad<U>> BindAsync<U>(Func<R, Task<IMonad<U>>> func)
            => IsRight ? NullCheck.DoAction(RightValue, func) : Task.FromResult(NullCheck.DoAction(LeftValue, l => (IMonad<U>) Either<L, U>.Left(l)));

        /// <summary></summary>
        public Task<Either<L, U>> BindAsync<U>(Func<R, Task<Either<L, U>>> func)
            => IsRight ? NullCheck.DoAction(RightValue, func) : Task.FromResult(NullCheck.DoAction(LeftValue, l => Either<L, U>.Left(l)));

        /// <summary></summary>
        public IMonad<U> Fmap<U>(Func<R, U> func)
            => IsRight ? Either<L, U>.Right(NullCheck.DoAction(RightValue, func)) : NullCheck.DoAction(LeftValue, l => Either<L, U>.Left(l));

        /// <summary></summary>
        public Either<L, U> Map<U>(Func<R, U> func)
            => IsRight ? Either<L, U>.Right(NullCheck.DoAction(RightValue, func)) : NullCheck.DoAction(LeftValue, l => Either<L, U>.Left(l));

        /// <summary></summary>
        public MonadU Bind<U, MonadU>(Func<R, MonadU> func)
            where MonadU : struct, IMonad<U>
            => IsRight ? NullCheck.DoAction(RightValue, func) : default(MonadU);

        /// <summary></summary>
        public Task<MonadU> BindAsync<U, MonadU>(Func<R, Task<MonadU>> func)
            where MonadU : struct, IMonad<U>
            => IsRight ? NullCheck.DoAction(RightValue, func) : Task.FromResult(default(MonadU));

        /// <summary></summary>
        R IPollutable<R>.Pollute()
            => IsRight ? NullCheck.DoAction(RightValue, Functional.Id) : throw new InvalidOperationException("Either is not right");
        #endregion
    }
}
