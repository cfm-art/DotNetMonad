using System;
using Xunit;
using CfmArt.Functional;
using System.Threading.Tasks;

namespace MonadTest
{
    public class StateTaskTest
    {
        private static void Abort()
        {
            throw new Exception("abort.");            
        }

        // テスト用のJustを返却するTask
        public async Task<Optional<int>> DummyTask(int arg)
        {
            await Task.Delay(10);
            return Optional.Just(arg);
        }

        // テスト様のNothingを返すTask
        public async Task<Optional<int>> NothingTask()
        {
            await Task.Delay(10);
            return Optional.Nothing<int>();
        }

        public async Task<Optional<int>> CallbackTask(Func<int> arg)
        {
            await Task.Delay(10);
            return Optional.Just(arg());
        }

        [Fact]
        public void Test_Awaitor1()
        {
            // 単一Task
            var state = StateTask.From(DummyTask(10));
            var result = state.Awaitor.Result;
            Assert.True(result.HasValue);
            Assert.Equal(10, Polluter.Pollute(result));
        }

        [Fact]
        public void Test_Awaitor2()
        {
            // 2つのTaskを直列化
            var state = StateTask
                            .From(DummyTask(10))
                            .Map(DummyTask(15));
            var result = state.Awaitor.Result;
            Assert.True(result.HasValue);
            Assert.Equal(10, Polluter.Pollute(result).Item1);
            Assert.Equal(15, Polluter.Pollute(result).Item2);
        }

        [Fact]
        public void Test_Awaitor3()
        {
            // 3つのTaskを直列化。
            // 結果はTuple<Tuple<1, 2>, 3>
            var state = StateTask
                            .From(DummyTask(10))
                            .Map(DummyTask(15))
                            .Map(DummyTask(20));
            var result = state.Awaitor.Result;
            Assert.True(result.HasValue);
            Assert.Equal(10, Polluter.Pollute(result).Item1.Item1);
            Assert.Equal(15, Polluter.Pollute(result).Item1.Item2);
            Assert.Equal(20, Polluter.Pollute(result).Item2);
        }

        [Fact]
        public void Test_Awaitor3_Flat()
        {
            // 3つのTaskを直列化
            // 結果は自身で定義する。
            var state = StateTask
                            .From(DummyTask(10))
                            .Map(async prev => (await DummyTask(15)).Map(next => (prev, next)))
                            .Map(async prev => (await DummyTask(20)).Map(next => (prev.Item1, prev.Item2, next)));
            var result = state.Awaitor.Result;
            Assert.True(result.HasValue);
            Assert.Equal(10, Polluter.Pollute(result).Item1);
            Assert.Equal(15, Polluter.Pollute(result).Item2);
            Assert.Equal(20, Polluter.Pollute(result).Item3);
        }

        [Fact]
        public void Test_Awaitor3_Flat2()
        {
            // 3つのTaskを直列化
            // 結果は自身で定義する。
            // Maybe a -> (a -> Task<Maybe (a b)>) -> Maybe (a b)は実装がわかりにくいので自動化
            // Maybe a -> (a -> Task<(a Maybe b)>) -> (a Maybe b)とならないように。
            var state = StateTask
                            .From(DummyTask(10))
                            .Map(prev => DummyTask(15), (prev, next) => Optional.Just((prev, next)))
                            .Map(prev => DummyTask(20), (prev, next) => Optional.Just((prev.Item1, prev.Item2, next)));
            var result = state.Awaitor.Result;
            Assert.True(result.HasValue);
            Assert.Equal(10, Polluter.Pollute(result).Item1);
            Assert.Equal(15, Polluter.Pollute(result).Item2);
            Assert.Equal(20, Polluter.Pollute(result).Item3);
        }

        [Fact]
        public void Test_Nothing()
        {
            // Nothingを返すTaskはNothingになる
            var state = StateTask
                            .From(NothingTask());
            var result = state.Awaitor.Result;
            Assert.False(result.HasValue);
        }

        [Fact]
        public void Test_Nothing2()
        {
            // NothingをかえすTaskがある場合はNothingになる。
            var state = StateTask
                            .From(DummyTask(10))
                            .Map(NothingTask());
            var result = state.Awaitor.Result;
            Assert.False(result.HasValue);
        }


        [Fact]
        public void Test_Nothing3()
        {
            bool called1 = false;
            bool called2 = false;
            // NothingをかえすTaskがある場合はNothingになる。
            // ※Nothingの前のTaskを返却する関数は実行される。
            // ※Nothingの後のTaskを返却する関数は実行されない。
            var state = StateTask
                            .From(DummyTask(10))
                            .Map(prev => CallbackTask(() => {called1 = true; return 1;}))
                            .Map(NothingTask())
                            .Map(prev => CallbackTask(() => {called2 = true; return 1;}));
            var result = state.Awaitor.Result;
            Assert.False(result.HasValue);
            Assert.True(called1);
            Assert.False(called2);
        }

        [Fact]
        public void Test_MonadLaw()
        {
            // return x >>= f == f x
            // m >>= return == m
            // (m >>= f) >>= g == m >>= (\x -> f x >>= g)

            {   // return x >>= f == f x
                Func<int, StateTask<int>> func = i => StateTask.Return(DummyTask(i));
                var result1 = func(10);
                var result2 = StateTask.Return(DummyTask(10)).Bind(func);
                Assert.Equal(result1.Awaitor.Result, result2.Awaitor.Result);
            }

            {   // m >>= return == m
                var result1 = StateTask.Return(DummyTask(10));
                var result2 = result1.Bind(i => StateTask.Return(DummyTask(i)));
                Assert.Equal(result1.Awaitor.Result, result2.Awaitor.Result);
            }

            {   // (m >>= f) >>= g == m >>= (\x -> f x >>= g)
                Func<int, StateTask<int>> f = i => StateTask.Return(DummyTask(i + 1));
                Func<int, StateTask<int>> g = i => StateTask.Return(DummyTask(i + 10));
                var m = StateTask.Return(DummyTask(10));

                var result1 = m.Bind(f).Bind(g);
                var result2 = m.Bind(i => f(i).Bind(g));
                Assert.Equal(result1.Awaitor.Result, result2.Awaitor.Result);
            }
        }        
    }
}
