using CoreGraphics;
using UIKit;
using Foundation;
using AudioToolbox;

namespace Polyariz.iOS.Shchurov.HorizontalWheelView
{
    /// <summary>
    /// A custom iOS view that displays a horizontal wheel selector
    /// Port of HorizontalWheelView.java from Android
    /// </summary>
    [Register("HorizontalWheelView")]
    public class HorizontalWheelView : UIView
    {
        private static readonly nfloat DP_DEFAULT_WIDTH = 200;
        private static readonly nfloat DP_DEFAULT_HEIGHT = 32;
        private const int DEFAULT_MARKS_COUNT = 40;
        private const int DEFAULT_NORMAL_COLOR = unchecked((int)0xffffffff); // White
        private const int DEFAULT_ACTIVE_COLOR = unchecked((int)0xff54acf0); // Blue
        private const bool DEFAULT_SHOW_ACTIVE_RANGE = true;
        private const bool DEFAULT_SNAP_TO_MARKS = false;
        private const bool DEFAULT_END_LOCK = false;
        private const bool DEFAULT_ONLY_POSITIVE_VALUES = false;

        public const int SCROLL_STATE_IDLE = 0;
        public const int SCROLL_STATE_DRAGGING = 1;
        public const int SCROLL_STATE_SETTLING = 2;

        private Drawer drawer = null!;
        private TouchHandler touchHandler = null!;
        private double angle;
        private bool onlyPositiveValues;
        private bool endLock;
        private HorizontalWheelViewListener? listener;

        // iOS specific - content insets (similar to Android padding)
        public UIEdgeInsets ContentEdgeInsets { get; set; } = UIEdgeInsets.Zero;

        // Haptic and sound feedback
        private bool hapticFeedbackEnabled = true;
        private bool soundFeedbackEnabled = true;
        private int lastMarkIndex = -1;

        #region Constructors

        public HorizontalWheelView()
        {
            Initialize();
        }

        public HorizontalWheelView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        public HorizontalWheelView(CGRect frame) : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            drawer = new Drawer(this);
            touchHandler = new TouchHandler(this);

            // Set default values
            drawer.SetMarksCount(DEFAULT_MARKS_COUNT);
            drawer.SetNormalColor(ColorFromInt(DEFAULT_NORMAL_COLOR));
            drawer.SetActiveColor(ColorFromInt(DEFAULT_ACTIVE_COLOR));
            drawer.SetShowActiveRange(DEFAULT_SHOW_ACTIVE_RANGE);
            touchHandler.SetSnapToMarks(DEFAULT_SNAP_TO_MARKS);
            endLock = DEFAULT_END_LOCK;
            onlyPositiveValues = DEFAULT_ONLY_POSITIVE_VALUES;

            // Enable user interaction
            UserInteractionEnabled = true;

            // Set background to clear
            BackgroundColor = UIColor.Clear;
        }

        #endregion

        #region Public API

        public void SetListener(HorizontalWheelViewListener? listener)
        {
            this.listener = listener;
            touchHandler.SetListener(listener);
        }

        public void SetRadiansAngle(double radians)
        {
            if (!CheckEndLock(radians))
            {
                angle = radians % (2 * System.Math.PI);
            }

            if (onlyPositiveValues && angle < 0)
            {
                angle += 2 * System.Math.PI;
            }

            // Check if we crossed a mark boundary for feedback
            CheckAndProvideFeedback();

            SetNeedsDisplay();

            if (listener != null)
            {
                listener.OnRotationChanged(angle);
            }
        }

        private bool CheckEndLock(double radians)
        {
            if (!endLock)
            {
                return false;
            }

            bool hit = false;
            if (radians >= 2 * System.Math.PI)
            {
                angle = System.Math.BitDecrement(2 * System.Math.PI); // Similar to Math.nextAfter in Java
                hit = true;
            }
            else if (onlyPositiveValues && radians < 0)
            {
                angle = 0;
                hit = true;
            }
            else if (radians <= -2 * System.Math.PI)
            {
                angle = System.Math.BitIncrement(-2 * System.Math.PI);
                hit = true;
            }

            if (hit)
            {
                touchHandler.CancelFling();
            }

            return hit;
        }

        public void SetDegreesAngle(double degrees)
        {
            double radians = degrees * System.Math.PI / 180;
            SetRadiansAngle(radians);
        }

        public void SetCompleteTurnFraction(double fraction)
        {
            double radians = fraction * 2 * System.Math.PI;
            SetRadiansAngle(radians);
        }

        public double GetRadiansAngle()
        {
            return angle;
        }

        public double GetDegreesAngle()
        {
            return GetRadiansAngle() * 180 / System.Math.PI;
        }

        public double GetCompleteTurnFraction()
        {
            return GetRadiansAngle() / (2 * System.Math.PI);
        }

        public void SetOnlyPositiveValues(bool onlyPositiveValues)
        {
            this.onlyPositiveValues = onlyPositiveValues;
        }

        public void SetEndLock(bool lockValue)
        {
            endLock = lockValue;
        }

        public void SetMarksCount(int marksCount)
        {
            drawer.SetMarksCount(marksCount);
            SetNeedsDisplay();
        }

        public void SetShowActiveRange(bool show)
        {
            drawer.SetShowActiveRange(show);
            SetNeedsDisplay();
        }

        public void SetNormalColor(UIColor color)
        {
            drawer.SetNormalColor(color);
            SetNeedsDisplay();
        }

        public void SetNormalColor(int color)
        {
            drawer.SetNormalColor(ColorFromInt(color));
            SetNeedsDisplay();
        }

        public void SetActiveColor(UIColor color)
        {
            drawer.SetActiveColor(color);
            SetNeedsDisplay();
        }

        public void SetActiveColor(int color)
        {
            drawer.SetActiveColor(ColorFromInt(color));
            SetNeedsDisplay();
        }

        public void SetSnapToMarks(bool snapToMarks)
        {
            touchHandler.SetSnapToMarks(snapToMarks);
        }

        /// <summary>
        /// Enable or disable haptic feedback when scrolling through marks
        /// Similar to UIPickerView behavior
        /// </summary>
        public void SetHapticFeedbackEnabled(bool enabled)
        {
            hapticFeedbackEnabled = enabled;
        }

        /// <summary>
        /// Enable or disable sound feedback when scrolling through marks
        /// Similar to UIPickerView behavior
        /// </summary>
        public void SetSoundFeedbackEnabled(bool enabled)
        {
            soundFeedbackEnabled = enabled;
        }

        #endregion

        #region Internal Methods

        internal int GetMarksCount()
        {
            return drawer.GetMarksCount();
        }

        #endregion

        #region UIView Overrides

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            drawer.OnSizeChanged();
        }

        public override CGSize IntrinsicContentSize
        {
            get
            {
                return new CGSize(
                    Utils.ConvertToPx(DP_DEFAULT_WIDTH),
                    Utils.ConvertToPx(DP_DEFAULT_HEIGHT)
                );
            }
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            var context = UIGraphics.GetCurrentContext();
            if (context != null)
            {
                drawer.OnDraw(context);
            }
        }

        #endregion

        #region Helper Methods

        private static UIColor ColorFromInt(int color)
        {
            nfloat a = ((color >> 24) & 0xff) / 255.0f;
            nfloat r = ((color >> 16) & 0xff) / 255.0f;
            nfloat g = ((color >> 8) & 0xff) / 255.0f;
            nfloat b = (color & 0xff) / 255.0f;

            return UIColor.FromRGBA(r, g, b, a);
        }

        private void CheckAndProvideFeedback()
        {
            if (!hapticFeedbackEnabled && !soundFeedbackEnabled)
                return;

            // Calculate current mark index
            int marksCount = drawer.GetMarksCount();
            if (marksCount == 0)
                return;

            double step = 2 * System.Math.PI / marksCount;
            int currentMarkIndex = (int)System.Math.Round(angle / step);

            // Normalize index to be within bounds
            currentMarkIndex = ((currentMarkIndex % marksCount) + marksCount) % marksCount;

            // Check if we crossed a mark boundary
            if (currentMarkIndex != lastMarkIndex)
            {
                lastMarkIndex = currentMarkIndex;

                // Provide haptic feedback
                if (hapticFeedbackEnabled)
                {
                    ProvideFeedback();
                }

                // Provide sound feedback
                if (soundFeedbackEnabled)
                {
                    PlaySelectionSound();
                }
            }
        }

        private void ProvideFeedback()
        {
            // Use UIImpactFeedbackGenerator for tactile feedback
            // This is the same feedback used by UIPickerView
            if (System.OperatingSystem.IsIOSVersionAtLeast(10))
            {
                var generator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light);
                generator.Prepare();
                generator.ImpactOccurred();
            }
        }

        private void PlaySelectionSound()
        {
            // Play the system sound that UIPickerView uses
            // System sound ID 1104 is the picker click sound
            var sound = new SystemSound(1104);
            sound.PlaySystemSound();
        }

        #endregion
    }
}
