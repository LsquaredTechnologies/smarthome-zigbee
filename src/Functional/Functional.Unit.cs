namespace Lsquared.Extensions
{
    /// <summary>
    /// Contains types and other stuff to use more functional-style programming in C#.
    /// </summary>
    public static class Functional
    {
        /// <summary>
        /// Represents the "no value" or "nothing" return type.
        /// </summary>
        /// <remarks>
        /// Use <c>return default;</c> in function which to not return nothing.
        /// </remarks>
        public readonly struct Unit { }
    }
}
