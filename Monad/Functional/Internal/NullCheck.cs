using System;

namespace CfmArt.Functional.Internal
{
    internal static class NullCheck
    {
            internal static void ThrowNull(string message = "") => throw new NullReferenceException(message);
            internal static T ThrowNull<T>(string message = "") => throw new NullReferenceException(message);
            internal static void DoAction<T>(T? v, Action<T> f)
            {
#if DEBUG
                if (v is T) { f(v); }
                else { ThrowNull(); }
#else
                f(v!);
#endif
            }

            internal static U DoAction<T, U>(T? v, Func<T, U> f)
                =>
#if DEBUG
                    v is T
                        ? f(v)
                        : ThrowNull<U>();
#else
                    f(v!);
#endif
    }
}
