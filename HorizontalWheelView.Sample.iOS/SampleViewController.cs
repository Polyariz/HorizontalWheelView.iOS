using UIKit;
using CoreGraphics;
using Foundation;
using Polyariz.iOS.Shchurov.HorizontalWheelView;

namespace HorizontalWheelView.Sample.iOS
{
    /// <summary>
    /// Sample view controller demonstrating HorizontalWheelView usage
    /// Port of SampleActivity.java from Android
    /// </summary>
    public class SampleViewController : UIViewController
    {
        private Polyariz.iOS.Shchurov.HorizontalWheelView.HorizontalWheelView? horizontalWheelView;
        private UILabel? angleLabel;
        private UIImageView? rocketImageView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Set background color
            View!.BackgroundColor = UIColor.FromRGB(0.2f, 0.2f, 0.25f); // Dark background similar to sample

            InitViews();
            SetupListeners();
            UpdateUI();
        }

        private void InitViews()
        {
            // Create rocket image view
            rocketImageView = new UIImageView
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // Try to load a system image as placeholder for rocket
            // In a real app, you would add your own rocket image
            var config = UIImageSymbolConfiguration.Create(UIImageSymbolScale.Large);
            var rocketImage = UIImage.GetSystemImage("airplane", config);
            if (rocketImage != null)
            {
                rocketImageView.Image = rocketImage;
                rocketImageView.TintColor = UIColor.White;
            }
            else
            {
                // Fallback: create a simple arrow shape
                rocketImageView.BackgroundColor = UIColor.White;
                rocketImageView.Layer.CornerRadius = 5;
            }

            // Create angle label
            angleLabel = new UILabel
            {
                Text = "0°",
                TextColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(14),
                TextAlignment = UITextAlignment.Center,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // Create horizontal wheel view
            horizontalWheelView = new Polyariz.iOS.Shchurov.HorizontalWheelView.HorizontalWheelView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // Set colors (similar to Android sample)
            horizontalWheelView.SetNormalColor(UIColor.White);
            horizontalWheelView.SetActiveColor(UIColor.FromRGB(1.0f, 0.84f, 0.0f)); // Yellow color
            horizontalWheelView.SetShowActiveRange(true);
            horizontalWheelView.SetMarksCount(40);

            // Add padding (similar to Android paddingBottom)
            horizontalWheelView.ContentEdgeInsets = new UIEdgeInsets(0, 0, 32, 0);

            // Add views to hierarchy
            View!.AddSubview(rocketImageView);
            View.AddSubview(angleLabel);
            View.AddSubview(horizontalWheelView);

            // Setup constraints for center layout
            SetupConstraints();
        }

        private void SetupConstraints()
        {
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                // Rocket image - centered horizontally, in upper portion of screen
                rocketImageView!.CenterXAnchor.ConstraintEqualTo(View!.CenterXAnchor),
                rocketImageView.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor, -80),
                rocketImageView.WidthAnchor.ConstraintEqualTo(100),
                rocketImageView.HeightAnchor.ConstraintEqualTo(100),

                // Angle label - below rocket
                angleLabel!.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
                angleLabel.TopAnchor.ConstraintEqualTo(rocketImageView.BottomAnchor, 16),

                // Horizontal wheel view - below angle label
                horizontalWheelView!.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
                horizontalWheelView.TopAnchor.ConstraintEqualTo(angleLabel.BottomAnchor, 16),
                horizontalWheelView.WidthAnchor.ConstraintEqualTo(200),
                horizontalWheelView.HeightAnchor.ConstraintEqualTo(64)
            });
        }

        private void SetupListeners()
        {
            var listener = new WheelViewListener(this);
            horizontalWheelView?.SetListener(listener);
        }

        private void UpdateUI()
        {
            UpdateText();
            UpdateImage();
        }

        private void UpdateText()
        {
            if (horizontalWheelView != null && angleLabel != null)
            {
                double degrees = horizontalWheelView.GetDegreesAngle();
                angleLabel.Text = $"{degrees:F0}°";
            }
        }

        private void UpdateImage()
        {
            if (horizontalWheelView != null && rocketImageView != null)
            {
                nfloat angle = (nfloat)horizontalWheelView.GetDegreesAngle();

                // Apply rotation transform
                rocketImageView.Transform = CGAffineTransform.MakeRotation((nfloat)(angle * System.Math.PI / 180.0));
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            UpdateUI();
        }

        // Private listener class to handle wheel view events
        private class WheelViewListener : HorizontalWheelViewListener
        {
            private readonly SampleViewController controller;

            public WheelViewListener(SampleViewController controller)
            {
                this.controller = controller;
            }

            public override void OnRotationChanged(double radians)
            {
                controller.UpdateUI();
            }

            public override void OnScrollStateChanged(int state)
            {
                // Optional: Handle scroll state changes
                // state can be SCROLL_STATE_IDLE, SCROLL_STATE_DRAGGING, or SCROLL_STATE_SETTLING
            }
        }
    }
}
