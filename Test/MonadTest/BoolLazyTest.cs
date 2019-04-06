using System;
using Xunit;
using Functional;

namespace MonadTest
{
    public class BoolLazyTest
    {
        private static void Abort()
        {
            throw new Exception("Abort");
        }

        [Fact]
        public void Test_TrueValue()
        {
            var trueValue = BoolLazy.True();

            var isCalled = false;
            trueValue.When(() => {isCalled = true;}).Else(() => Abort());
            Assert.True(isCalled);

            var one = trueValue.When(() => 1, () => 0);
            Assert.Equal(1, one);

            var t = trueValue.Bind(_ => {
                Assert.True(_);
                return Bool.False();
            });
            Assert.False(Polluter.Pollute(t));

            var t2 = trueValue.Fmap(_ => {
                Assert.True(_);
                return false;
            });
            Assert.False(Polluter.Pollute(t2));
        }

        [Fact]
        public void Test_FalseValue()
        {
            var falseValue = BoolLazy.False();

            var isCalled = false;
            falseValue.When(() => Abort()).Else(() => {isCalled = true;});
            Assert.True(isCalled);

            var one = falseValue.When(() => 0, () => 1);
            Assert.Equal(1, one);

            var t = falseValue.Bind(_ => {
                Abort();
                return Bool.False();
            });
            Assert.Throws<InvalidOperationException>(
                () => Polluter.Pollute(t)
            );

            var t2 = falseValue.Fmap(_ => {
                Abort();
                return false;
            });
            Assert.Throws<InvalidOperationException>(
                () => Polluter.Pollute(t2)
            );
        }


        [Fact]
        public void Test_TrueFunction()
        {
            var once = true;
            var trueValue = BoolLazy.Return(() => {
                if (!once) { Abort(); }
                once = false;
                return true;
            });

            var isCalled = false;
            trueValue.When(() => {isCalled = true;}).Else(() => Abort());
            Assert.True(isCalled);

            var one = trueValue.When(() => 1, () => 0);
            Assert.Equal(1, one);

            var t = trueValue.Bind(_ => {
                Assert.True(_);
                return Bool.False();
            });
            Assert.False(Polluter.Pollute(t));

            var t2 = trueValue.Fmap(_ => {
                Assert.True(_);
                return false;
            });
            Assert.False(Polluter.Pollute(t2));
        }

        [Fact]
        public void Test_FalseFunction()
        {
            var once = true;
            var falseValue = BoolLazy.Return(() => {
                if (!once) { Abort(); }
                once = false;
                return false;
            });

            var isCalled = false;
            falseValue.When(() => Abort()).Else(() => {isCalled = true;});
            Assert.True(isCalled);

            var one = falseValue.When(() => 0, () => 1);
            Assert.Equal(1, one);

            var t = falseValue.Bind(_ => {
                Abort();
                return Bool.False();
            });
            Assert.Throws<InvalidOperationException>(
                () => Polluter.Pollute(t)
            );

            var t2 = falseValue.Fmap(_ => {
                Abort();
                return false;
            });
            Assert.Throws<InvalidOperationException>(
                () => Polluter.Pollute(t2)
            );
        }
    }
}