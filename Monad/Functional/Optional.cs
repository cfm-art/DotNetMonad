using Functional.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Functional
{
    public static class Optional
    {
        /// <summary>
        /// left `action` right
        /// </summary>
        public static Optional<W> Apply<U, V, W>(Optional<U> left, Optional<V> right, Func<U, V, W> action)
        {
            if (!left.HasValue || !right.HasValue) { return Optional<W>.Nothing; }
            return Optional<W>.Just(action(left.Pollute(), right.Pollute()));
        }

        /// <summary>
        /// Maybeのリストから値を持つもののリストへ変換
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<U> MapPollute<U>(IEnumerable<Optional<U>> list)
        {
            return (from x in list where x.HasValue select x.Pollute());
        }

        public static Optional<T> Maybe<T>(T value) => Optional<T>.Maybe(value);
        public static Optional<T> Just<T>(T value) => Optional<T>.Just(value);
        public static Optional<T> Return<T>(T value) => Optional<T>.Just(value);
        public static Optional<T> Nothing<T>() => Optional<T>.Nothing;
    }

    /// <summary>
    /// Maybe Monad
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Optional<T>
        : IEquatable<Optional<T>>
        , IEquatable<T>
        , IMonad<T>
    {
        /// <summary>
        /// 中身を直接参照する
        /// </summary>
        private T Value { get; }

        /// <summary>
        /// 中身があるかどうか
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="hasValue"></param>
        private Optional(T instance, bool hasValue)
        {
            Value = instance;
            HasValue = hasValue;
        }

        /// <summary>
        /// 有効なインスタンスを指定して生成
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Optional<T> Just(T instance)
            => instance == null
                ? throw new ArgumentNullException()
                : new Optional<T>(instance, true);

        /// <summary>
        /// nullかもしれないインスタンスを指定して生成
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Optional<T> Maybe(T instance)
            => instance == null
                ? Nothing
                : new Optional<T>(instance, true);

        /// <summary>
        /// null
        /// </summary>
        /// <returns></returns>
        public static Optional<T> Nothing { get; } = new Optional<T>(default(T), false);

        /// <summary>
        /// 中身があれば処理を行う
        /// </summary>
        /// <param name="action"></param>
        public IfElseSentence.ElseSentence IfPresent(Action<T> action)
        {
            if (HasValue)
            {
                action(Value);
                return new IfElseSentence.DoNotDoElse();
            }
            return new IfElseSentence.DoElse();
        }

        /// <summary>
        /// 中身があれば処理を行う
        /// </summary>
        /// <param name="action"></param>
        public IfElseSentence.ElseSentence<U> IfPresent<U>(Func<T, U> action)
        {
            if (HasValue)
            {
                return new IfElseSentence.DoNotDoElse<U>(action(Value));
            }
            return new IfElseSentence.DoElse<U>();
        }

        /// <summary>
        /// 中身がある時と無い時で処理を行う
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public void IfPresent(Action<T> then, Action elseThen)
        {
            if (HasValue) { then(Value); }
            else { elseThen(); }
        }

        /// <summary>
        /// 中身がある時と無い時で処理を行う
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public U IfPresent<U>(Func<T, U> then, Func<U> elseThen)
        {
            return HasValue
                ? then(Value)
                : elseThen();
        }

        /// <summary>
        /// 中身を代替を指定して取得
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T OrElse<U>(U value)
            where U : T
            => HasValue ? Value : value;

        /// <summary>
        /// 中身を代替を指定して取得
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public Optional<U> OrElse<U>(Optional<U> value)
            where U: T
            => HasValue ? Optional<U>.Just((U) Value) : value;

        /// <summary>
        /// 中身を代替を指定して取得
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T OrElse<U>(Func<U> value)
            where U : T
            => HasValue ? Value : value();

        /// <summary>
        /// 中身を代替を指定して取得
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public Optional<U> OrElse<U>(Func<Optional<U>> value)
            where U : T
            => HasValue ? Optional<U>.Just((U)Value) : value();


        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="optional"></param>
        public static explicit operator T(Optional<T> optional) => optional.Value;

        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Optional<T>(T value) => Maybe(value);

        /// <summary>
        /// 比較
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(Optional<T> lhs, Optional<T> rhs)
        {
            // 両方null
            if (!lhs.HasValue == !rhs.HasValue) { return true; }

            // 参照が一緒
            if (ReferenceEquals(lhs.Value, rhs.Value)) { return true; }

            // どちらかがnull
            if (!lhs.HasValue || !rhs.HasValue) { return false; }

            return lhs.Value.Equals(rhs.Value);
        }

        /// <summary>
        /// 比較
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(Optional<T> lhs, Optional<T> rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// 中身のハッシュ値
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HasValue ? Value.GetHashCode() : 0;
        }

        /// <summary>
        /// 中身の文字列表現
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return HasValue ? Value.ToString() : "";
        }

        /// <summary>
        /// 比較
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Optional<T> optional)
            {
                return this == optional;
            }
            if (obj is T raw)
            {
                return HasValue ? Value.Equals(raw) : false;
            }
            return false;
        }

        /// <summary>
        /// 比較
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Optional<T> other)
        {
            return this == other;
        }

        /// <summary>
        /// 比較
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(T other)
        {
            return HasValue ? Value.Equals(other) : (other == null);
        }

        #region IMonad
        /// <summary>
        /// (>>=) :: m a -> (a -> m b) -> m b
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IMonad<U> Bind<U>(Func<T, IMonad<U>> func)
            => HasValue ? func(Value) : Optional<U>.Nothing;

        public Optional<U> Bind<U>(Func<T, Optional<U>> func)
            => HasValue ? func(Value) : Optional<U>.Nothing;

        /// <summary>
        /// fmap :: (a -> b) -> m a -> m b
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IMonad<U> Fmap<U>(Func<T, U> func)
            => HasValue ? Optional<U>.Just(func(Value)) : Optional<U>.Nothing;

        public Optional<U> Map<U>(Func<T, U> func)
            => HasValue ? Optional<U>.Just(func(Value)) : Optional<U>.Nothing;

        public T Pollute() => HasValue ? Value : throw new InvalidOperationException("Optional is nothing");
        #endregion
    }
}
