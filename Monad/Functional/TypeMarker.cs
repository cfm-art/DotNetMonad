namespace CfmArt.Functional
{
    /// <summary></summary>
    public class TypeMarker<T>
    {
        /// <summary></summary>
        public static TypeMarker<T> Type => new();

        private TypeMarker() {}
    }
}
