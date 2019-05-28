using System;

namespace CfmArt.Functional
{
    namespace Internal
    {
        public struct CaseOfEndPoint<Result>
        {
            public Result Value { get; }
            public CaseOfEndPoint(Result value) => Value = value;
        }

        public interface ICaseOf<T, Result>
        {
            ICaseOf<T, Result> Of<U>(Func<U, Result> func);
            CaseOfEndPoint<Result> Other(Func<Result> func);
        }

        public class CaseOf<T, Result>
            : ICaseOf<T, Result>
        {
            private IMonad<T> M { get; }
            public Result Value => default(Result);
            public ICaseOf<T, Result> Of<U>(Func<U, Result> func)
            {
                if (M is U v) { return new CaseOfComplete<T, Result>(func(v)); }
                return this;
            }
            public CaseOfEndPoint<Result> Other(Func<Result> func) => new CaseOfEndPoint<Result>(func());
            public CaseOf(IMonad<T> m) => M = m;
        }

        public class CaseOfComplete<T, Result>
            : ICaseOf<T, Result>
        {
            public CaseOfComplete(Result result) => Value = result;
            public Result Value { get; }
            public ICaseOf<T, Result> Of<U>(Func<U, Result> func) => this;
            public CaseOfEndPoint<Result> Other(Func<Result> func) => new CaseOfEndPoint<Result>(Value);
        }

        public class CaseOf<T>
        {
            private IMonad<T> M { get; }
            public CaseOf(IMonad<T> m) => M = m;

            public ICaseOf<T, Result> Of<U, Result>(Func<U, Result> func)
            {
                if (M is U v) { return new CaseOfComplete<T, Result>(func(v)); }
                return new CaseOf<T, Result>(M);
            }
        }
    }

    public static class CaseOf
    {
        public static Internal.CaseOf<T> Case<T>(this IMonad<T> m)
        {
            return new Internal.CaseOf<T>(m);
        }
    }
}
