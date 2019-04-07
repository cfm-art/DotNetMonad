namespace CfmArt.Functional.Internal
{
    internal interface IPollutable<T>
        : IMonad<T>
    {
        T Pollute();
    }
}