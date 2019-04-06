using System;
using Xunit;
using Functional;

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
                _ => Abort()
            ).Else(() => {});
            Assert.Equal(1, nothing.OrElse(1));
            Assert.Throws<InvalidOperationException>(
                () => nothing.Pollute()
            );
        }

        [Fact]
        public void Test_Bind()
        {
            var maybe1 = Optional.Just(1);
            var maybe2 = maybe1.Bind(o => Optional.Just(o + 1));
            Assert.Equal(2, maybe2.Pollute());

            var maybeString = maybe1.Bind(_ => Optional.Just("text"));
            Assert.Equal("text", maybeString.Pollute());
        }

        [Fact]
        public void Test_Fmap()
        {
            var maybe1 = Optional.Just(1);
            var maybe2 = maybe1.Fmap(o => o + 1);
            Assert.Equal(2, maybe2.Pollute());

            var m2o = (Optional<int>) maybe2;
            m2o.IfPresent(o => Assert.Equal(2, o));

            var maybeString = maybe1.Fmap(_ => "text");
            Assert.Equal("text", maybeString.Pollute());
        }

        [Fact]
        public void Test_Map()
        {
            var maybe1 = Optional.Just(1);
            var maybe2 = maybe1.Map(o => o + 1);
            Assert.Equal(2, maybe2.Pollute());

            var maybeString = maybe1.Map(_ => "text");
            Assert.Equal("text", maybeString.Pollute());
        }

        [Fact]
        public void Test_If()
        {
            var maybe1 = Optional.Just(1);
            maybe1.IfPresent(
                one => Assert.Equal(1, one)
            ).Else(
                () => Abort()
            );

            var two = maybe1.IfPresent(
                one => one + 1
            ).Else(
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
    }
}
