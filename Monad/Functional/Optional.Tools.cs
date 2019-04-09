using CfmArt.Functional.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CfmArt.Functional
{
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
        /// Maybeのリストから値を持つもののリストへ変換
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<U> MapPollute<U>(IEnumerable<Optional<U>> list)
            => (from x in list where x.HasValue select Polluter.Pollute(x));

        /// <summary>
        /// Maybe Task T -> Task Maybe T
        /// 
        /// usecase: Optional<T> `bind` T -> Optioanl<Task<Optioanl<T>>>
        /// Taskの中にOptionalが包まれている場合にMapなどでTaskが一生つきまとうため、Taskだけを剥がすためのユーティリティ。
        /// var maybe = await Optional.Asynchronous(maybeTask); // :: Taskを剥がしOptional<T>へ
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
        public static Optional<T> Maybe<T>(T value) => Optional<T>.Maybe(value);

        /// <summary>
        /// nillではない
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
    }
}
