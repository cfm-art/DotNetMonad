using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CfmArt.Functional.Internal;

namespace CfmArt.Functional
{
    /// <summary>
    /// 分岐
    /// </summary>
    public struct Bool
        : IMonad<bool>
        , IPollutable<bool>
    {
        private bool Condition { get; }

        private Bool(bool condition) => Condition = condition;
        private Bool(Func<bool> condition) => Condition = condition();

        /// <summary>True</summary>
        public static Bool True() => new Bool(true);
        /// <summary>True</summary>
        public static Bool False() => new Bool(false);

        /// <summary>x -> m x</summary>
        public static Bool Return(bool condition) => new Bool(condition);
        /// <summary>x -> M x</summary>
        public static Bool Return(Func<bool> condition) => new Bool(condition);

        /// <summary>(>>=) :: m a -> (a -> m b) -> m b</summary>
        public IMonad<U> Bind<U>(Func<bool, IMonad<U>> func)
            => Condition ? func(true) : Optional<U>.Nothing;

        /// <summary>(>>=) :: m a -> (a -> m b) -> m b</summary>
        public Task<IMonad<U>> BindAsync<U>(Func<bool, Task<IMonad<U>>> func)
            => Condition ? func(true) : Task.FromResult((IMonad<U>) Optional<U>.Nothing);

        /// <summary>fmap :: (a -> b) -> m a -> m b</summary>
        public IMonad<U> Fmap<U>(Func<bool, U> func)
            => Condition ? func(true) : Optional<U>.Nothing;

        /// <summary>(>>=) :: m a -> (a -> m b) -> m b</summary>
        public MonadU Bind<U, MonadU>(Func<bool, MonadU> func)
            where MonadU : struct, IMonad<U>
            => Condition ? func(true) : default(MonadU);

        /// <summary>(>>=) :: m a -> (a -> m b) -> m b</summary>
        public Task<MonadU> BindAsync<U, MonadU>(Func<bool, Task<MonadU>> func)
            where MonadU : struct, IMonad<U>
            => Condition ? func(true) : Task.FromResult(default(MonadU));

        bool IPollutable<bool>.Pollute() => Condition;

#if false
        /// <summary>
        /// 条件が真なら処理を行う
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IfElseSentence.ElseSentence When(Action func)
        {
            if (Condition)
            {
                func();
                return new IfElseSentence.DoNotDoElse();
            }
            return new IfElseSentence.DoElse();
        }

        /// <summary>
        /// 条件が真なら処理を行う
        /// </summary>
        /// <param name="action"></param>
        public IfElseSentence.ElseSentence<U> When<U>(Func<U> action)
        {
            if (Condition)
            {
                return new IfElseSentence.DoNotDoElse<U>(action());
            }
            return new IfElseSentence.DoElse<U>();
        }
#endif
        /// <summary>
        /// 条件分岐
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public void When(Action then, Action elseThen)
        {
            if (Condition) { then(); }
            else { elseThen(); }
        }

        /// <summary>
        /// 条件分岐
        /// </summary>
        /// <param name="then"></param>
        /// <param name="elseThen"></param>
        public U When<U>(Func<U> then, Func<U> elseThen)
            => Condition
                ? then()
                : elseThen();
    }
}
