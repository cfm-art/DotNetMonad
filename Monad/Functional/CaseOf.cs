using System;

namespace CfmArt.Functional
{
    namespace Internal
    {
        /// <summary></summary>
        public struct CaseOfEndPoint<Result>
        {
            /// <summary></summary>
            public Result Value { get; }
            /// <summary></summary>
            public CaseOfEndPoint(Result value) => Value = value;
        }

        /// <summary></summary>
        public interface ICaseOf<T, Result>
        {
            /// <summary></summary>
            ICaseOf<T, Result> Of<U>(Func<U, Result> func);
            /// <summary></summary>
            CaseOfEndPoint<Result> Other(Func<Result> func);
        }

        /// <summary></summary>
        public class CaseOf<T, Result>
            : ICaseOf<T, Result>
            where Result: struct
        {
            private IMonad<T> M { get; }
           /// <summary></summary>
            public Result Value => default(Result);
            /// <summary></summary>
            public ICaseOf<T, Result> Of<U>(Func<U, Result> func)
            {
                if (M is U v) { return new CaseOfComplete<T, Result>(func(v)); }
                return this;
            }
            /// <summary></summary>
            public CaseOfEndPoint<Result> Other(Func<Result> func) => new CaseOfEndPoint<Result>(func());
            /// <summary></summary>
            public CaseOf(IMonad<T> m) => M = m;
        }

        /// <summary></summary>
        public class CaseOfComplete<T, Result>
            : ICaseOf<T, Result>
        {
            /// <summary></summary>
            public CaseOfComplete(Result result) => Value = result;
            /// <summary></summary>
            public Result Value { get; }
            /// <summary></summary>
            public ICaseOf<T, Result> Of<U>(Func<U, Result> func) => this;
            /// <summary></summary>
            public CaseOfEndPoint<Result> Other(Func<Result> func) => new CaseOfEndPoint<Result>(Value);
        }

        /// <summary></summary>
        public class CaseOf<T>
        {
            private IMonad<T> M { get; }
            /// <summary></summary>
            public CaseOf(IMonad<T> m) => M = m;

            /// <summary></summary>
            public ICaseOf<T, Result> Of<U, Result>(Func<U, Result> func)
                where Result: struct
            {
                if (M is U v) { return new CaseOfComplete<T, Result>(func(v)); }
                return new CaseOf<T, Result>(M);
            }
        }
    }

    /// <summary></summary>
    public static class CaseOf
    {
        /// <summary></summary>
        public static Internal.CaseOf<T> Case<T>(this IMonad<T> m)
        {
            return new Internal.CaseOf<T>(m);
        }
    }
}
