using CoreGraphics;
using UIKit;

namespace Polyariz.iOS.Shchurov.HorizontalWheelView
{
    /// <summary>
    /// Handles drawing of marks and cursor
    /// Port of Drawer.java
    /// </summary>
    internal class Drawer
    {
        private static readonly nfloat DP_CURSOR_CORNERS_RADIUS = 1;
        private static readonly nfloat DP_NORMAL_MARK_WIDTH = 1;
        private static readonly nfloat DP_ZERO_MARK_WIDTH = 2;
        private static readonly nfloat DP_CURSOR_WIDTH = 3;
        private static readonly nfloat NORMAL_MARK_RELATIVE_HEIGHT = 0.6f;
        private static readonly nfloat ZERO_MARK_RELATIVE_HEIGHT = 0.8f;
        private static readonly nfloat CURSOR_RELATIVE_HEIGHT = 1f;
        private static readonly nfloat SHADE_RANGE = 0.7f;
        private static readonly nfloat SCALE_RANGE = 0.1f;

        private readonly HorizontalWheelView view;
        private int marksCount;
        private UIColor normalColor = UIColor.White;
        private UIColor activeColor = UIColor.SystemBlue;
        private bool showActiveRange;
        private nfloat[] gaps = System.Array.Empty<nfloat>();
        private nfloat[] shades = System.Array.Empty<nfloat>();
        private nfloat[] scales = System.Array.Empty<nfloat>();
        private int[] colorSwitches = { -1, -1, -1 };
        private nfloat viewportHeight;
        private nfloat normalMarkWidth;
        private nfloat normalMarkHeight;
        private nfloat zeroMarkWidth;
        private nfloat zeroMarkHeight;
        private nfloat cursorCornersRadius;
        private CGRect cursorRect = CGRect.Empty;
        private int maxVisibleMarksCount;

        public Drawer(HorizontalWheelView view)
        {
            this.view = view;
            InitDpSizes();
        }

        private void InitDpSizes()
        {
            normalMarkWidth = Utils.ConvertToPx(DP_NORMAL_MARK_WIDTH);
            zeroMarkWidth = Utils.ConvertToPx(DP_ZERO_MARK_WIDTH);
            cursorCornersRadius = Utils.ConvertToPx(DP_CURSOR_CORNERS_RADIUS);
        }

        public void SetMarksCount(int marksCount)
        {
            this.marksCount = marksCount;
            maxVisibleMarksCount = (marksCount / 2) + 1;
            gaps = new nfloat[maxVisibleMarksCount];
            shades = new nfloat[maxVisibleMarksCount];
            scales = new nfloat[maxVisibleMarksCount];
        }

        public void SetNormalColor(UIColor color)
        {
            normalColor = color;
        }

        public void SetActiveColor(UIColor color)
        {
            activeColor = color;
        }

        public void SetShowActiveRange(bool show)
        {
            showActiveRange = show;
        }

        public void OnSizeChanged()
        {
            viewportHeight = view.Bounds.Height - view.ContentEdgeInsets.Top - view.ContentEdgeInsets.Bottom;
            normalMarkHeight = viewportHeight * NORMAL_MARK_RELATIVE_HEIGHT;
            zeroMarkHeight = viewportHeight * ZERO_MARK_RELATIVE_HEIGHT;
            SetupCursorRect();
        }

        private void SetupCursorRect()
        {
            nfloat cursorHeight = viewportHeight * CURSOR_RELATIVE_HEIGHT;
            nfloat cursorTop = view.ContentEdgeInsets.Top + (viewportHeight - cursorHeight) / 2;
            nfloat cursorBottom = cursorTop + cursorHeight;
            nfloat cursorWidth = Utils.ConvertToPx(DP_CURSOR_WIDTH);
            nfloat cursorLeft = (view.Bounds.Width - cursorWidth) / 2;
            nfloat cursorRight = cursorLeft + cursorWidth;

            cursorRect = new CGRect(cursorLeft, cursorTop, cursorRight - cursorLeft, cursorBottom - cursorTop);
        }

        public int GetMarksCount()
        {
            return marksCount;
        }

        public void OnDraw(CGContext context)
        {
            double step = 2 * System.Math.PI / marksCount;
            double offset = (System.Math.PI / 2 - view.GetRadiansAngle()) % step;
            if (offset < 0)
            {
                offset += step;
            }

            SetupGaps(step, offset);
            SetupShadesAndScales(step, offset);
            int zeroIndex = CalcZeroIndex(step);
            SetupColorSwitches(step, offset, zeroIndex);
            DrawMarks(context, zeroIndex);
            DrawCursor(context);
        }

        private void SetupGaps(double step, double offset)
        {
            gaps[0] = (nfloat)System.Math.Sin(offset / 2);
            nfloat sum = gaps[0];
            double angle = offset;
            int n = 1;

            while (angle + step <= System.Math.PI)
            {
                gaps[n] = (nfloat)System.Math.Sin(angle + step / 2);
                sum += gaps[n];
                angle += step;
                n++;
            }

            nfloat lastGap = (nfloat)System.Math.Sin((System.Math.PI + angle) / 2);
            sum += lastGap;

            if (n != gaps.Length)
            {
                gaps[gaps.Length - 1] = -1;
            }

            nfloat k = view.Bounds.Width / sum;
            for (int i = 0; i < gaps.Length; i++)
            {
                if (gaps[i] != -1)
                {
                    gaps[i] *= k;
                }
            }
        }

        private void SetupShadesAndScales(double step, double offset)
        {
            double angle = offset;
            for (int i = 0; i < maxVisibleMarksCount; i++)
            {
                double sin = System.Math.Sin(angle);
                shades[i] = (nfloat)(1 - SHADE_RANGE * (1 - sin));
                scales[i] = (nfloat)(1 - SCALE_RANGE * (1 - sin));
                angle += step;
            }
        }

        private int CalcZeroIndex(double step)
        {
            double twoPi = 2 * System.Math.PI;
            double normalizedAngle = (view.GetRadiansAngle() + System.Math.PI / 2 + twoPi) % twoPi;
            if (normalizedAngle > System.Math.PI)
            {
                return -1;
            }
            return (int)((System.Math.PI - normalizedAngle) / step);
        }

        private void SetupColorSwitches(double step, double offset, int zeroIndex)
        {
            if (!showActiveRange)
            {
                System.Array.Fill(colorSwitches, -1);
                return;
            }

            double angle = view.GetRadiansAngle();
            int afterMiddleIndex = 0;

            if (offset < System.Math.PI / 2)
            {
                afterMiddleIndex = (int)((System.Math.PI / 2 - offset) / step) + 1;
            }

            if (angle > 3 * System.Math.PI / 2)
            {
                colorSwitches[0] = 0;
                colorSwitches[1] = afterMiddleIndex;
                colorSwitches[2] = zeroIndex;
            }
            else if (angle >= 0)
            {
                colorSwitches[0] = System.Math.Max(0, zeroIndex);
                colorSwitches[1] = afterMiddleIndex;
                colorSwitches[2] = -1;
            }
            else if (angle < -3 * System.Math.PI / 2)
            {
                colorSwitches[0] = 0;
                colorSwitches[1] = zeroIndex;
                colorSwitches[2] = afterMiddleIndex;
            }
            else if (angle < 0)
            {
                colorSwitches[0] = afterMiddleIndex;
                colorSwitches[1] = zeroIndex;
                colorSwitches[2] = -1;
            }
        }

        private void DrawMarks(CGContext context, int zeroIndex)
        {
            nfloat x = view.ContentEdgeInsets.Left;
            UIColor color = normalColor;
            int colorPointer = 0;

            for (int i = 0; i < gaps.Length; i++)
            {
                if (gaps[i] == -1)
                {
                    break;
                }

                x += gaps[i];

                while (colorPointer < 3 && i == colorSwitches[colorPointer])
                {
                    color = color == normalColor ? activeColor : normalColor;
                    colorPointer++;
                }

                if (i != zeroIndex)
                {
                    DrawNormalMark(context, x, scales[i], shades[i], color);
                }
                else
                {
                    DrawZeroMark(context, x, scales[i], shades[i]);
                }
            }
        }

        private void DrawNormalMark(CGContext context, nfloat x, nfloat scale, nfloat shade, UIColor color)
        {
            nfloat height = normalMarkHeight * scale;
            nfloat top = view.ContentEdgeInsets.Top + (viewportHeight - height) / 2;
            nfloat bottom = top + height;

            context.SetLineWidth(normalMarkWidth);
            context.SetStrokeColor(ApplyShade(color, shade).CGColor);
            context.MoveTo(x, top);
            context.AddLineToPoint(x, bottom);
            context.StrokePath();
        }

        private UIColor ApplyShade(UIColor color, nfloat shade)
        {
            nfloat r, g, b, a;
            color.GetRGBA(out r, out g, out b, out a);
            return UIColor.FromRGBA(r * shade, g * shade, b * shade, a);
        }

        private void DrawZeroMark(CGContext context, nfloat x, nfloat scale, nfloat shade)
        {
            nfloat height = zeroMarkHeight * scale;
            nfloat top = view.ContentEdgeInsets.Top + (viewportHeight - height) / 2;
            nfloat bottom = top + height;

            context.SetLineWidth(zeroMarkWidth);
            context.SetStrokeColor(ApplyShade(activeColor, shade).CGColor);
            context.MoveTo(x, top);
            context.AddLineToPoint(x, bottom);
            context.StrokePath();
        }

        private void DrawCursor(CGContext context)
        {
            var path = UIBezierPath.FromRoundedRect(cursorRect, cursorCornersRadius);
            context.SetFillColor(activeColor.CGColor);
            if (path.CGPath != null)
            {
                context.AddPath(path.CGPath);
            }
            context.FillPath();
        }
    }
}
