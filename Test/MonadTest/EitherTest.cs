using System;
using Xunit;
using CfmArt.Functional;

namespace MonadTest
{
    public class EitherTest
    {
        private static void Abort()
        {
            throw new Exception("Abort");
        }

        [Fact]
        public void Test_Left()
        {
            var left = Either<bool, int>.Left(true);
            var isCalled = false;
            left.IfLeft(_ =>
            {
                Assert.True(_);
                isCalled = true;
            }).Else(_ => Abort());
            Assert.True(isCalled);

            isCalled = false;
            left.IfRight(_ => Abort())
                .Else(_ =>
                {
                    Assert.True(_);
                    isCalled = true;
                });
            Assert.True(isCalled);

            isCalled = false;
            left.IfLeft(_ =>
            {
                Assert.True(_);
                isCalled = true;
            }, _ => Abort());
            Assert.True(isCalled);

            isCalled = false;
            left.IfRight(_ => Abort(),
                _ =>
                {
                    Assert.True(_);
                    isCalled = true;
                });
            Assert.True(isCalled);

            var o = left.IfLeft(_ =>
            {
                Assert.True(_);
                return 1;
            }).Else(_ => 0);
            Assert.Equal(1, o);

            o = left.IfRight(_ => 0)
                .Else(_ =>
                {
                    Assert.True(_);
                    return 1;
                });
            Assert.Equal(1, o);

            o = left.IfLeft(_ =>
            {
                Assert.True(_);
                return 1;
            }, _ => 0);
            Assert.Equal(1, o);

            o = left.IfRight(_ => 0,
                _ =>
                {
                    Assert.True(_);
                    return 1;
                });
            Assert.Equal(1, o);

            left.Bind(_ => { Abort(); return Either<float, int>.Right(0); });
            left.Fmap(_ => { Abort(); return 1; });
            left.Map(_ => { Abort(); return 1; });
        }


        [Fact]
        public void Test_Right()
        {
            var right = Either<bool, int>.Right(1);
            var isCalled = false;
            right.IfRight(_ =>
            {
                Assert.Equal(1, _);
                isCalled = true;
            }).Else(_ => Abort());
            Assert.True(isCalled);

            isCalled = false;
            right.IfLeft(_ => Abort())
                .Else(_ =>
                {
                    Assert.Equal(1, _);
                    isCalled = true;
                });
            Assert.True(isCalled);

            isCalled = false;
            right.IfRight(_ =>
            {
                Assert.Equal(1, _);
                isCalled = true;
            }, _ => Abort());
            Assert.True(isCalled);

            isCalled = false;
            right.IfLeft(_ => Abort(),
                _ =>
                {
                    Assert.Equal(1, _);
                    isCalled = true;
                });
            Assert.True(isCalled);

            var o = right.IfRight(_ =>
            {
                Assert.Equal(1, _);
                return 1;
            }).Else(_ => 0);
            Assert.Equal(1, o);

            o = right.IfLeft(_ => 0)
                .Else(_ =>
                {
                    Assert.Equal(1, _);
                    return 1;
                });
            Assert.Equal(1, o);

            o = right.IfRight(_ =>
            {
                Assert.Equal(1, _);
                return 1;
            }, _ => 0);
            Assert.Equal(1, o);

            o = right.IfLeft(_ => 0,
                _ =>
                {
                    Assert.Equal(1, _);
                    return 1;
                });
            Assert.Equal(1, o);

            var z = right.Bind(_ => { return Either<float, int>.Right(0); });
            Assert.Equal(0, Polluter.Pollute(z));

            var o2 = right.Fmap(_ => { return 1; });
            Assert.Equal(1, Polluter.Pollute(o2));

            var o3 = right.Map(_ => { return 1; });
            Assert.Equal(1, Polluter.Pollute(o3).PolluteRight());
        }
    }
}