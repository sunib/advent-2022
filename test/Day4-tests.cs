namespace test;
using advent_of_code;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void EdgeOverlap()
    {
        var r1 = new Range("22", "100");
        var r2 = new Range("11", "22");
        Assert.True(r1.Overlap(r2));
        Assert.True(r2.Overlap(r1));

        var r3 = new Range("22", "100");
        var r4 = new Range("100", "120");
        Assert.True(r3.Overlap(r4));
        Assert.True(r4.Overlap(r3));
    }

    [Test]
    public void NoOverlap()
    {
        var r1 = new Range("22", "100");
        var r2 = new Range("11", "20");
        Assert.False(r1.Overlap(r2));
        Assert.False(r2.Overlap(r1));
    }

    [Test]
    public void InTheMiddleOverlap()
    {
        var r1 = new Range("1", "8");
        var r2 = new Range("5", "6");
        Assert.True(r1.Overlap(r2));
        Assert.True(r2.Overlap(r1)); //Not sure: is this an overlap? Or an underlap? :-)

    }

}