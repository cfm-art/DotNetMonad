using CfmArt.Functional.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CfmArt.Functional
{
    /// <summary></summary>
    public static class Optional
    {
        /// <summary>
        /// left `action` right
        /// </summary>
        public static Optional<W> Apply<U, V, W>(Optional<U> left, Optional<V> right, Func<U, V, W> action)
        {
            if (!left.HasValue || !right.HasValue) { return Optional<W>.Nothing; }
            return Optional<W>.Just(action(Polluter.Pollute(left), Polluter.Pollute(right)));
        }

        /// <summary>
        ///
        /// </summary>
        public static Optional<T> Do<T>(this Optional<T> self, Action<T> action)
        {
            if (self.HasValue) { action(Polluter.Pollute(self)); }
            return self;
        }

        /// <summary>
        /// Maybeのリストから値を持つもののリストへ変換
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<U> MapPollute<U>(IEnumerable<Optional<U>> list)
            => (from x in list where x.HasValue select Polluter.Pollute(x));

        /// <summary>
        /// Maybe Task T -&gt; Task Maybe T
        /// 
        /// usecase: Optional&lt;T&gt; `bind` T -> Optioanl&lt;Task&lt;Optioanl&lt;T&gt;&gt;&gt;
        /// Taskの中にOptionalが包まれている場合にMapなどでTaskが一生つきまとうため、Taskだけを剥がすためのユーティリティ。
        /// var maybe = await Optional.Asynchronous(maybeTask); // :: Taskを剥がしOptional&lt;T&gt;へ
        /// </summary>
        public static async Task<Optional<T>> Asynchronous<T>(Optional<Task<T>> maybeTask)
            => await maybeTask.IfPresent(
                async task => Optional.Maybe(await task),
                () => Task.FromResult(Optional<T>.Nothing));

#if false   // TODO; .Net Standard 2.1以降
        public static async ValueTask<Optional<T>> Asynchronous<T>(Optional<ValueTask<T>> maybeTask)
            => await maybeTask.IfPresent(
                async task => Optional.Maybe(await task),
                () => ValueTask.FromResult(Optional<T>.Nothing));
#endif

        /// <summary>
        /// 長さ0もしくは1の配列へ
        /// </summary>
        public static IEnumerable<T> ToEnumerable<T>(Optional<T> maybeT)
            => maybeT.IfPresent(v => new T[] { v }, () => Enumerable.Empty<T>());

        /// <summary>
        /// リストの先頭をMaybeへ
        /// </summary>
        public static Optional<T> First<T>(IEnumerable<T> list)
            => Maybe(list.FirstOrDefault());

        /// <summary>
        /// nullかもしれない
        /// </summary>
        public static Optional<T> Maybe<T>(T? value) => Optional<T>.Maybe(value);

        /// <summary>
        /// nullではない
        /// </summary>
        public static Optional<T> Just<T>(T value) => Optional<T>.Just(value);

        /// <summary>
        /// Just
        /// </summary>
        public static Optional<T> Return<T>(T value) => Optional<T>.Just(value);

        /// <summary>
        /// null
        /// </summary>
        public static Optional<T> Nothing<T>() => Optional<T>.Nothing;

        /// <summary>NothingをEitherのLeftへ</summary>
        public static Either<L, R> NullToLeft<T, L, R>(
                this Optional<T> self,
                Func<T, R> functor,
                Func<L> nothing)
            => self.IfPresent(
                    o => Either<L, R>.Right(functor(o)),
                    () => Either<L, R>.Left(nothing()));

        /// <summary></summary>
        public static Either<L, T> NullToLeft<T, L>(
                this Optional<T> self,
                Func<L> nothing)
            => self.IfPresent(
                    o => Either<L, T>.Right(o),
                    () => Either<L, T>.Left(nothing()));

        /// <summary></summary>
        public static Optional<(T, T2)> Join<T, T2>(
            this Optional<T> o,
            Func<Optional<T2>> f2)
            => o.Bind(v1 => f2().Map(v2 => (v1, v2)));

        /// <summary></summary>
        public static Optional<(T, T2, T3)> Join<T, T2, T3>(
            this Optional<T> o,
            Func<Optional<T2>> f2,
            Func<Optional<T3>> f3)
            => o.Bind(v1 => f2().Bind(v2 => f3().Map(v3 => (v1, v2, v3))));

        /// <summary></summary>
        public static Optional<(T, T2, T3, T4)> Join<T, T2, T3, T4>(
            this Optional<T> o,
            Func<Optional<T2>> f2,
            Func<Optional<T3>> f3,
            Func<Optional<T4>> f4)
            => o.Bind(v1 => f2().Bind(v2 => f3().Bind(v3 => f4().Map(v4 => (v1, v2, v3, v4)))));

        /// <summary></summary>
        public static Optional<(T, T2, T3)> Join<T, T2, T3>(
            this Optional<(T, T2)> o,
            Func<Optional<T3>> f3)
            => o.Bind(v1 => f3().Map(v3 => (v1.Item1, v1.Item2, v3)));

        /// <summary></summary>
        public static Optional<(T, T2, T3, T4)> Join<T, T2, T3, T4>(
            this Optional<(T, T2, T3)> o,
            Func<Optional<T4>> f4)
            => o.Bind(v1 => f4().Map(v4 => (v1.Item1, v1.Item2, v1.Item3, v4)));
    }
}
