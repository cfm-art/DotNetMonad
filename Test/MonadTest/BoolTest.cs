using System;
using Xunit;
using CfmArt.Functional;

namespace MonadTest
{
    public class BoolTest
    {
        private static void Abort()
        {
            throw new Exception("Abort");
        }

        [Fact]
        public void Test_TrueValue()
        {
            var trueValue = Bool.True();

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
            var falseValue = Bool.False();

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
            // Boolを返却する関数をしていする。
            // 関数はこのタイミングで即時に評価される。
            var trueValue = Bool.Return(() => true);

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
            var falseValue = Bool.Return(() => false);

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