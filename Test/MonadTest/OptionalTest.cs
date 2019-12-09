using System;
using Xunit;
using CfmArt.Functional;

namespace MonadTest
{
    public class OptionalTest
    {
        private static void Abort()
        {
            throw new Exception("abort.");            
        }

        [Fact]
        public void Test_Nothhing()
        {
            var nothing = Optional.Nothing<int>();
            nothing.Bind(o => {
                Abort();
                return Optional.Nothing<int>();
            });
            nothing.Fmap(o => {
                Abort();
                return 1;
            });
            nothing.Map(o => {
                Abort();
                return 1;
            });
            nothing.IfPresent(
                _ => Abort(),
                () => {}
            );
            nothing.IfPresent(
                _ => Abort(),
                () => {});
            Assert.Equal(1, nothing.OrElse(1));
            Assert.Throws<InvalidOperationException>(
                () => Polluter.Pollute(nothing)
            );
        }

        [Fact]
        public void Test_Bind()
        {
            var maybe1 = Optional.Just(1);
            var maybe2 = maybe1.Bind(o => Optional.Just(o + 1));
            maybe2.IfPresent(t => Assert.Equal(2, t), () => Abort());

            var maybeString = maybe1.Bind(_ => Optional.Just("text"));
            maybeString.IfPresent(t => Assert.Equal("text", t), () => Abort());
        }

        [Fact]
        public void Test_Fmap()
        {
            var maybe1 = Optional.Just(1);
            var maybe2 = maybe1.Fmap(o => o + 1);
            Assert.Equal(2, Polluter.Pollute(maybe2));

            var m2o = (Optional<int>) maybe2;
            m2o.IfPresent(o => Assert.Equal(2, o), () => Abort());

            var maybeString = maybe1.Fmap(_ => "text");
            Assert.Equal("text", Polluter.Pollute(maybeString));
        }

        [Fact]
        public void Test_Map()
        {
            var maybe1 = Optional.Just(1);
            var maybe2 = maybe1.Map(o => o + 1);
            maybe2.IfPresent(t => Assert.Equal(2, t), () => Abort());

            var maybeString = maybe1.Map(_ => "text");
            maybeString.IfPresent(t => Assert.Equal("text", t), () => Abort());
        }

        [Fact]
        public void Test_If()
        {
            var maybe1 = Optional.Just(1);
            maybe1.IfPresent(
                one => Assert.Equal(1, one),
                () => Abort()
            );

            var two = maybe1.IfPresent(
                one => one + 1,
                () => 0
            );
            Assert.Equal(2, two);

            maybe1.IfPresent(
                one => Assert.Equal(1, one),
                () => Abort()
            );

            var three = maybe1.IfPresent(
                one => one + 2,
                () => 0
            );
            Assert.Equal(3, three);
        }

        [Fact]
        public void Test_MonadLaw()
        {
            // return x >>= f == f x
            // m >>= return == m
            // (m >>= f) >>= g == m >>= (\x -> f x >>= g)

            {   // return x >>= f == f x
                Func<int, Optional<int>> func = i => Optional.Just(i + 1);
                var result1 = func(10);
                var result2 = Optional.Return(10).Bind(func);
                Assert.Equal(result1, result2);
            }

            {   // m >>= return == m
                var result1 = Optional.Just(10);
                var result2 = result1.Bind(Optional.Return);
                Assert.Equal(result1, result2);
            }

            {   // (m >>= f) >>= g == m >>= (\x -> f x >>= g)
                Func<int, Optional<int>> f = i => Optional.Just(i + 1);
                Func<int, Optional<int>> g = i => Optional.Just(i + 10);
                var m = Optional.Just(10);

                var result1 = m.Bind(f).Bind(g);
                var result2 = m.Bind(i => f(i).Bind(g));
                Assert.Equal(result1, result2);
            }
        }
    }
}
