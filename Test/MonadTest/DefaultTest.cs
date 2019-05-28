using System;
using Xunit;
using CfmArt.Functional;

public class DefaultTest
{
    [Fact]
    public void Test_DefaultOptional()
    {
        var o = default(Optional<int>);
        Assert.False(o.HasValue);
    }

    [Fact]
    public void Test_DefaultEither()
    {
        var e = default(Either<int, int>);
        Assert.False(e.IfRight(_ => true, _ => false));
    }

    [Fact]
    public void Test_DefaultBool()
    {
        var b = default(Bool);
        Assert.False(b.When(() => true, () => false));
    }

    [Fact]
    public void Test_DefaultBoolLazy()
    {
        var b = default(BoolLazy);
        Assert.False(b.When(() => true, () => false));
    }

    [Fact]
    public void Test_DefaultStateTask()
    {
        var b = default(StateTask<int>);
        var o = b.Awaitor.Result;
        Assert.False(o.HasValue);
    }
}
