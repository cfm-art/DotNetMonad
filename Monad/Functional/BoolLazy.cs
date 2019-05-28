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
    public struct BoolLazy
        : IMonad<bool>
        , IPollutable<bool>
    {
        private Func<bool> condition_ { get; set; }
        private bool cache_;

        private bool Condition
        {
            get {
                if (condition_ == null) { return cache_; }
                cache_ = condition_();
                condition_ = null;
                return cache_;
            }
        }

        private BoolLazy(bool condition)
        {
            cache_ = false;
            condition_ = () => condition;
        }

        private BoolLazy(Func<bool> condition)
        {
            cache_ = false;
            condition_ = condition;
        }

        public static BoolLazy True() => new BoolLazy(true);
        public static BoolLazy False() => new BoolLazy(false);

        public static BoolLazy Return(bool condition) => new BoolLazy(condition);
        public static BoolLazy Return(Func<bool> condition) => new BoolLazy(condition);

        public IMonad<U> Bind<U>(Func<bool, IMonad<U>> func)
            => Condition ? func(true) : Optional<U>.Nothing;

        public Task<IMonad<U>> Bind<U>(Func<bool, Task<IMonad<U>>> func)
            => Condition ? func(true) : Task.FromResult((IMonad<U>) Optional<U>.Nothing);

        public IMonad<U> Fmap<U>(Func<bool, U> func)
            => Condition ? func(true) : Optional<U>.Nothing;

        public MonadU Bind<U, MonadU>(Func<bool, MonadU> func)
            where MonadU : IMonad<U>
            => Condition ? func(true) : default(MonadU);

        public Task<MonadU> Bind<U, MonadU>(Func<bool, Task<MonadU>> func)
            where MonadU : IMonad<U>
            => Condition ? func(true) : Task.FromResult(default(MonadU));

        bool IPollutable<bool>.Pollute() => Condition;

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
