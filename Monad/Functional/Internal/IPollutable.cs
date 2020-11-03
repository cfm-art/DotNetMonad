namespace CfmArt.Functional.Internal
{
    /// <summary></summary>
    internal interface IPollutable<T>
        : IMonad<T>
    {
        /// <summary></summary>
        T Pollute();
    }
}