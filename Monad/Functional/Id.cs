namespace CfmArt.Functional
{
    /// <summary></summary>
    public static class Functional
    {
        /// <summary>a = a</summary>
        public static T Id<T>(T v) => v;
    }
}