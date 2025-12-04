namespace Polyariz.iOS.Shchurov.HorizontalWheelView
{
    /// <summary>
    /// Utility class for common operations
    /// Port of Utils.java
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Convert points to pixels
        /// iOS uses points as logical units, similar to Android's DP
        /// </summary>
        public static nfloat ConvertToPx(nfloat points)
        {
            // On iOS, UIScreen.MainScreen.Scale gives the scale factor
            // points * scale = pixels, but iOS APIs work in points directly
            // So we just return points as-is since iOS handles scaling automatically
            return points;
        }
    }
}
