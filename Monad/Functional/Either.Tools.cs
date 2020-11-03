using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CfmArt.Functional.Internal;

namespace CfmArt.Functional
{
    /// <summary></summary>
    public static class Either
    {
        /// <summary></summary>
        public static Either<L, R> Return<L, R>(L left)
            => Either<L, R>.Left(left);

        /// <summary></summary>
        public static Either<L, R> Return<L, R>(L left, TypeMarker<R> _)
            => Either<L, R>.Left(left);

        /// <summary></summary>
        public static Either<L, R> Return<L, R>(TypeMarker<L> _, R right)
            => Either<L, R>.Right(right);

        /// <summary></summary>
        public static Either<L, R> Return<L, R>(R right)
            => Either<L, R>.Right(right);

        /// <summary></summary>
        public static Either<L, R> Do<L, R>(this Either<L, R> self, Action<R> func)
            => self.IfRight(
                right => {
                    func(right);
                    return self;
                },
                _ => self);

        /// <summary>LとRを交換</summary>
        public static Either<R, L> Swap<R, L>(this Either<L, R> self)
            => self.IfRight(
                r => Either<R, L>.Left(r),
                l => Either<R, L>.Right(l));

        /// <summary></summary>
        public static R To<L, R>(this Either<L, R> self, Func<L, R> map)
            => self.IfLeft(map, Functional.Id);

        /// <summary></summary>
        public static L To<L, R>(this Either<L, R> self, Func<R, L> map)
            => self.IfRight(map, Functional.Id);
    }
}
