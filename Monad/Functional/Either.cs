using System;
using System.Collections.Generic;
using System.Text;
using CfmArt.Functional.Internal;

namespace CfmArt.Functional
{
    public static class Either
    {
        public static Either<L, R> Return<L, R>(L left)
            => Either<L, R>.Left(left);

        public static Either<L, R> Return<L, R>(L left, TypeMarker<R> _)
            => Either<L, R>.Left(left);

        public static Either<L, R> Return<L, R>(TypeMarker<L> _, R right)
            => Either<L, R>.Right(right);

        public static Either<L, R> Return<L, R>(R right)
            => Either<L, R>.Right(right);

        public static Either<L, R> CherryPick<L, R>(this Either<L, R> self, Action<R> func)
            => self.IfRight(
                right => {
                    func(right);
                    return self;
                },
                _ => self
            );
    }

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

        internal L LeftValue { get; }
        internal R RightValue { get; }
        private bool IsLeft { get; }
        private bool IsRight => !IsLeft;

        private Either(L left, R right, bool isLeft)
        {
            LeftValue = left;
            RightValue = right;
            IsLeft = isLeft;
        }

        #region if left
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

        /// <summary>
        /// 中身がある時と無い時で処理を行う
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public void IfLeft(Action<L> then, Action<R> elseThen)
        {
            if (IsLeft) { then(LeftValue); }
            else { elseThen(RightValue); }
        }

        /// <summary>
        /// 中身がある時と無い時で処理を行う
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public U IfLeft<U>(Func<L, U> then, Func<R, U> elseThen)
            => IsLeft
                ? then(LeftValue)
                : elseThen(RightValue);
        #endregion

        #region if right
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

        /// <summary>
        /// 中身がある時と無い時で処理を行う
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public void IfRight(Action<R> then, Action<L> elseThen)
        {
            if (IsRight) { then(RightValue); }
            else { elseThen(LeftValue); }
        }

        /// <summary>
        /// 中身がある時と無い時で処理を行う
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public U IfRight<U>(Func<R, U> then, Func<L, U> elseThen)
            => IsRight
                ? then(RightValue)
                : elseThen(LeftValue);
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
        public override int GetHashCode()
            => (IsLeft
                ? LeftValue?.GetHashCode()
                : RightValue?.GetHashCode()) ?? 0;

        public override bool Equals(object obj)
        {
            if (obj == null) { return false;  }
            return Equals((Either<L, R>) obj);
        }

        public bool Equals(Either<L, R> other)
        {
            if (IsLeft != other.IsLeft) { return false; }
            return IsLeft
                ? (LeftValue?.Equals(other.LeftValue) ?? false)
                : (RightValue?.Equals(other.RightValue) ?? false);
        }

        public bool Equals(L other)
        {
            if (IsRight) { return false; }
            return LeftValue?.Equals(other) ?? false;
        }

        public bool Equals(R other)
        {
            if (IsLeft) { return false; }
            return RightValue?.Equals(other) ?? false;
        }
        #endregion

        #region IMonad
        public IMonad<U> Bind<U>(Func<R, IMonad<U>> func)
            => IsRight ? func(RightValue) : Either<L, U>.Left(LeftValue);

        public Either<L, U> Bind<U>(Func<R, Either<L, U>> func)
            => IsRight ? func(RightValue) : Either<L, U>.Left(LeftValue);

        public IMonad<U> Fmap<U>(Func<R, U> func)
            => IsRight ? Either<L, U>.Right(func(RightValue)) : Either<L, U>.Left(LeftValue);

        public Either<L, U> Map<U>(Func<R, U> func)
            => IsRight ? Either<L, U>.Right(func(RightValue)) : Either<L, U>.Left(LeftValue);

        R IPollutable<R>.Pollute()
            => IsRight ? RightValue : throw new InvalidOperationException("Either is not right");
        #endregion
    }
}
