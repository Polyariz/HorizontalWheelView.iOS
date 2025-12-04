using UIKit;
using Foundation;
using CoreAnimation;

namespace Polyariz.iOS.Shchurov.HorizontalWheelView
{
    /// <summary>
    /// Handles touch events and animations
    /// Port of TouchHandler.java
    /// </summary>
    internal class TouchHandler : UIGestureRecognizerDelegate
    {
        private static readonly nfloat SCROLL_ANGLE_MULTIPLIER = 0.002f;
        private static readonly nfloat FLING_ANGLE_MULTIPLIER = 0.0002f;
        private const double SETTLING_DURATION_MULTIPLIER = 1000.0;
        private const double DECELERATION_RATE = 0.998; // Similar to DecelerateInterpolator(2.5f)

        private readonly HorizontalWheelView view;
        private HorizontalWheelViewListener? listener;
        private UIPanGestureRecognizer? panGestureRecognizer;
        private CADisplayLink? displayLink;
        private bool snapToMarks;
        private int scrollState = HorizontalWheelView.SCROLL_STATE_IDLE;

        // Animation state
        private double animationStartAngle;
        private double animationEndAngle;
        private double animationStartTime;
        private double animationDuration;
        private double lastTouchX;

        public TouchHandler(HorizontalWheelView view)
        {
            this.view = view;
            SetupGestureRecognizers();
        }

        private void SetupGestureRecognizers()
        {
            panGestureRecognizer = new UIPanGestureRecognizer(HandlePan);
            panGestureRecognizer.Delegate = this;
            view.AddGestureRecognizer(panGestureRecognizer);
        }

        public void SetListener(HorizontalWheelViewListener? listener)
        {
            this.listener = listener;
        }

        public void SetSnapToMarks(bool snapToMarks)
        {
            this.snapToMarks = snapToMarks;
        }

        private void HandlePan(UIPanGestureRecognizer recognizer)
        {
            var location = recognizer.LocationInView(view);
            var velocity = recognizer.VelocityInView(view);

            switch (recognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    CancelFling();
                    lastTouchX = (double)location.X;
                    UpdateScrollStateIfRequired(HorizontalWheelView.SCROLL_STATE_DRAGGING);
                    break;

                case UIGestureRecognizerState.Changed:
                    {
                        double distanceX = lastTouchX - (double)location.X;
                        lastTouchX = (double)location.X;
                        double newAngle = view.GetRadiansAngle() + distanceX * (double)SCROLL_ANGLE_MULTIPLIER;
                        view.SetRadiansAngle(newAngle);
                        UpdateScrollStateIfRequired(HorizontalWheelView.SCROLL_STATE_DRAGGING);
                    }
                    break;

                case UIGestureRecognizerState.Ended:
                    {
                        // Handle fling (matches Java onFling behavior)
                        double velocityX = velocity.X;
                        if (System.Math.Abs(velocityX) > 0) // Any velocity triggers fling
                        {
                            double endAngle = view.GetRadiansAngle() - velocityX * FLING_ANGLE_MULTIPLIER;
                            if (snapToMarks)
                            {
                                endAngle = FindNearestMarkAngle(endAngle);
                            }
                            PlaySettlingAnimation(endAngle);
                        }
                        else if (snapToMarks)
                        {
                            PlaySettlingAnimation(FindNearestMarkAngle(view.GetRadiansAngle()));
                        }
                        else
                        {
                            UpdateScrollStateIfRequired(HorizontalWheelView.SCROLL_STATE_IDLE);
                        }
                    }
                    break;

                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    if (snapToMarks)
                    {
                        PlaySettlingAnimation(FindNearestMarkAngle(view.GetRadiansAngle()));
                    }
                    else
                    {
                        UpdateScrollStateIfRequired(HorizontalWheelView.SCROLL_STATE_IDLE);
                    }
                    break;
            }
        }

        public void CancelFling()
        {
            if (scrollState == HorizontalWheelView.SCROLL_STATE_SETTLING)
            {
                if (displayLink != null)
                {
                    displayLink.Invalidate();
                    displayLink = null;
                }
            }
        }

        private void UpdateScrollStateIfRequired(int newState)
        {
            if (scrollState != newState)
            {
                scrollState = newState;
                if (listener != null)
                {
                    listener.OnScrollStateChanged(newState);
                }
            }
        }

        private double FindNearestMarkAngle(double angle)
        {
            double step = 2 * System.Math.PI / view.GetMarksCount();
            return System.Math.Round(angle / step) * step;
        }

        private void PlaySettlingAnimation(double endAngle)
        {
            UpdateScrollStateIfRequired(HorizontalWheelView.SCROLL_STATE_SETTLING);

            animationStartAngle = view.GetRadiansAngle();
            animationEndAngle = endAngle;
            animationStartTime = CACurrentMediaTime();
            animationDuration = System.Math.Abs(animationStartAngle - animationEndAngle) * SETTLING_DURATION_MULTIPLIER / 1000.0;

            // Stop any existing animation
            if (displayLink != null)
            {
                displayLink.Invalidate();
                displayLink = null;
            }

            // Use CADisplayLink for smooth animation (60 FPS)
            displayLink = CADisplayLink.Create(UpdateAnimation);
            displayLink.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Common);
        }

        private void UpdateAnimation()
        {
            double currentTime = CACurrentMediaTime();
            double elapsed = currentTime - animationStartTime;

            if (elapsed >= animationDuration)
            {
                view.SetRadiansAngle(animationEndAngle);

                // Stop the display link
                if (displayLink != null)
                {
                    displayLink.Invalidate();
                    displayLink = null;
                }

                UpdateScrollStateIfRequired(HorizontalWheelView.SCROLL_STATE_IDLE);
                return;
            }

            // Decelerate interpolation
            double t = elapsed / animationDuration;
            double interpolatedT = DecelerateInterpolation(t);

            double currentAngle = animationStartAngle + (animationEndAngle - animationStartAngle) * interpolatedT;
            view.SetRadiansAngle(currentAngle);
        }

        private double DecelerateInterpolation(double t)
        {
            // Simulate DecelerateInterpolator with factor 2.5
            // Formula: 1 - (1 - t)^(2 * factor)
            double factor = 2.5;
            return 1.0 - System.Math.Pow(1.0 - t, 2.0 * factor);
        }

        private double CACurrentMediaTime()
        {
            return CoreAnimation.CAAnimation.CurrentMediaTime();
        }
    }

    /// <summary>
    /// Listener interface for HorizontalWheelView events
    /// Port of HorizontalWheelView.Listener from Java
    /// </summary>
    public class HorizontalWheelViewListener
    {
        public virtual void OnRotationChanged(double radians)
        {
        }

        public virtual void OnScrollStateChanged(int state)
        {
        }
    }
}
