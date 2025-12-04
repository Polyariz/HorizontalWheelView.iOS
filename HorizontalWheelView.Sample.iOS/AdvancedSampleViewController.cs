using UIKit;
using CoreGraphics;
using Foundation;
using Polyariz.iOS.Shchurov.HorizontalWheelView;

namespace HorizontalWheelView.Sample.iOS
{
    /// <summary>
    /// Advanced sample demonstrating all HorizontalWheelView features
    /// </summary>
    public class AdvancedSampleViewController : UIViewController
    {
        private Polyariz.iOS.Shchurov.HorizontalWheelView.HorizontalWheelView? wheelView;
        private UILabel? angleLabel;
        private UILabel? stateLabel;
        private UISwitch? snapSwitch;
        private UISwitch? endLockSwitch;
        private UISwitch? onlyPositiveSwitch;
        private UISwitch? showActiveRangeSwitch;
        private UISwitch? hapticFeedbackSwitch;
        private UISwitch? soundFeedbackSwitch;
        private UISlider? marksCountSlider;
        private UILabel? marksCountLabel;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View!.BackgroundColor = UIColor.FromRGB(0.15f, 0.15f, 0.2f);

            CreateWheelView();
            CreateLabels();
            CreateControls();
            SetupConstraints();
        }

        private void CreateWheelView()
        {
            wheelView = new Polyariz.iOS.Shchurov.HorizontalWheelView.HorizontalWheelView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // Установка значений по умолчанию
            wheelView.SetMarksCount(40);
            wheelView.SetNormalColor(unchecked((int)0xffffffff));  // Белый через int
            wheelView.SetActiveColor(unchecked((int)0xff54acf0));  // Синий через int
            wheelView.SetShowActiveRange(true);
            wheelView.SetSnapToMarks(false);
            wheelView.SetEndLock(false);
            wheelView.SetOnlyPositiveValues(false);

            // Установка listener
            wheelView.SetListener(new WheelListener(this));

            View!.AddSubview(wheelView);
        }

        private void CreateLabels()
        {
            // Заголовок
            var titleLabel = new UILabel
            {
                Text = "HorizontalWheelView Demo",
                Font = UIFont.BoldSystemFontOfSize(20),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            View!.AddSubview(titleLabel);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                titleLabel.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 20),
                titleLabel.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor)
            });

            // Метка угла
            angleLabel = new UILabel
            {
                Text = "Angle: 0°",
                Font = UIFont.SystemFontOfSize(16),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            View.AddSubview(angleLabel);

            // Метка состояния
            stateLabel = new UILabel
            {
                Text = "State: IDLE",
                Font = UIFont.SystemFontOfSize(14),
                TextColor = UIColor.LightGray,
                TextAlignment = UITextAlignment.Center,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            View.AddSubview(stateLabel);
        }

        private void CreateControls()
        {
            var scrollView = new UIScrollView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            View!.AddSubview(scrollView);

            var contentView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            scrollView.AddSubview(contentView);

            nfloat yOffset = 20;

            // Snap to marks
            yOffset = AddSwitchControl(contentView, "Snap to Marks", out snapSwitch, yOffset);
            snapSwitch!.On = false;
            snapSwitch.ValueChanged += (s, e) => wheelView?.SetSnapToMarks(snapSwitch.On);

            // End lock
            yOffset = AddSwitchControl(contentView, "End Lock", out endLockSwitch, yOffset);
            endLockSwitch!.On = false;
            endLockSwitch.ValueChanged += (s, e) => wheelView?.SetEndLock(endLockSwitch.On);

            // Only positive values
            yOffset = AddSwitchControl(contentView, "Only Positive Values", out onlyPositiveSwitch, yOffset);
            onlyPositiveSwitch!.On = false;
            onlyPositiveSwitch.ValueChanged += (s, e) =>
            {
                wheelView?.SetOnlyPositiveValues(onlyPositiveSwitch.On);
                UpdateAngleLabel();
            };

            // Show active range
            yOffset = AddSwitchControl(contentView, "Show Active Range", out showActiveRangeSwitch, yOffset);
            showActiveRangeSwitch!.On = true;
            showActiveRangeSwitch.ValueChanged += (s, e) => wheelView?.SetShowActiveRange(showActiveRangeSwitch.On);

            // Haptic feedback
            yOffset = AddSwitchControl(contentView, "Haptic Feedback", out hapticFeedbackSwitch, yOffset);
            hapticFeedbackSwitch!.On = true;
            hapticFeedbackSwitch.ValueChanged += (s, e) => wheelView?.SetHapticFeedbackEnabled(hapticFeedbackSwitch.On);

            // Sound feedback
            yOffset = AddSwitchControl(contentView, "Sound Feedback", out soundFeedbackSwitch, yOffset);
            soundFeedbackSwitch!.On = true;
            soundFeedbackSwitch.ValueChanged += (s, e) => wheelView?.SetSoundFeedbackEnabled(soundFeedbackSwitch.On);

            // Marks count slider
            var marksLabel = new UILabel
            {
                Text = "Marks Count",
                Font = UIFont.SystemFontOfSize(14),
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            contentView.AddSubview(marksLabel);

            marksCountLabel = new UILabel
            {
                Text = "40",
                Font = UIFont.BoldSystemFontOfSize(14),
                TextColor = UIColor.SystemBlue,
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            contentView.AddSubview(marksCountLabel);

            marksCountSlider = new UISlider
            {
                MinValue = 10,
                MaxValue = 100,
                Value = 40,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            marksCountSlider.ValueChanged += (s, e) =>
            {
                int value = (int)marksCountSlider.Value;
                marksCountLabel.Text = value.ToString();
                wheelView?.SetMarksCount(value);
            };
            contentView.AddSubview(marksCountSlider);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                marksLabel.TopAnchor.ConstraintEqualTo(contentView.TopAnchor, yOffset),
                marksLabel.LeadingAnchor.ConstraintEqualTo(contentView.LeadingAnchor, 20),

                marksCountLabel.CenterYAnchor.ConstraintEqualTo(marksLabel.CenterYAnchor),
                marksCountLabel.TrailingAnchor.ConstraintEqualTo(contentView.TrailingAnchor, -20),
                marksCountLabel.WidthAnchor.ConstraintEqualTo(50),

                marksCountSlider.TopAnchor.ConstraintEqualTo(marksLabel.BottomAnchor, 10),
                marksCountSlider.LeadingAnchor.ConstraintEqualTo(contentView.LeadingAnchor, 20),
                marksCountSlider.TrailingAnchor.ConstraintEqualTo(contentView.TrailingAnchor, -20)
            });

            yOffset += 70;

            // Set content size
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                scrollView.TopAnchor.ConstraintEqualTo(angleLabel!.BottomAnchor, 100),
                scrollView.LeadingAnchor.ConstraintEqualTo(View!.LeadingAnchor),
                scrollView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
                scrollView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor),

                contentView.TopAnchor.ConstraintEqualTo(scrollView.TopAnchor),
                contentView.LeadingAnchor.ConstraintEqualTo(scrollView.LeadingAnchor),
                contentView.TrailingAnchor.ConstraintEqualTo(scrollView.TrailingAnchor),
                contentView.BottomAnchor.ConstraintEqualTo(scrollView.BottomAnchor),
                contentView.WidthAnchor.ConstraintEqualTo(scrollView.WidthAnchor),
                contentView.HeightAnchor.ConstraintEqualTo(yOffset)
            });
        }

        private nfloat AddSwitchControl(UIView parent, string title, out UISwitch switchControl, nfloat yOffset)
        {
            var label = new UILabel
            {
                Text = title,
                Font = UIFont.SystemFontOfSize(14),
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            parent.AddSubview(label);

            switchControl = new UISwitch
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            parent.AddSubview(switchControl);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                label.TopAnchor.ConstraintEqualTo(parent.TopAnchor, yOffset),
                label.LeadingAnchor.ConstraintEqualTo(parent.LeadingAnchor, 20),

                switchControl.CenterYAnchor.ConstraintEqualTo(label.CenterYAnchor),
                switchControl.TrailingAnchor.ConstraintEqualTo(parent.TrailingAnchor, -20)
            });

            return yOffset + 50;
        }

        private void SetupConstraints()
        {
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                // Wheel view - fixed width centered
                wheelView!.LeadingAnchor.ConstraintEqualTo(View!.LeadingAnchor, 20),
                wheelView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -20),
                wheelView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 60),
                wheelView.HeightAnchor.ConstraintEqualTo(80),

                // Angle label - full width
                angleLabel!.TopAnchor.ConstraintEqualTo(wheelView.BottomAnchor, 20),
                angleLabel.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, 20),
                angleLabel.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -20),

                // State label - full width
                stateLabel!.TopAnchor.ConstraintEqualTo(angleLabel.BottomAnchor, 5),
                stateLabel.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, 20),
                stateLabel.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -20)
            });
        }

        private void UpdateAngleLabel()
        {
            if (wheelView != null && angleLabel != null)
            {
                double degrees = wheelView.GetDegreesAngle();
                double radians = wheelView.GetRadiansAngle();
                double fraction = wheelView.GetCompleteTurnFraction();

                angleLabel.Text = $"Angle: {degrees:F1}° ({radians:F3} rad, {fraction:F3} turn)";
            }
        }

        private void UpdateStateLabel(int state)
        {
            if (stateLabel != null)
            {
                string stateText = state switch
                {
                    Polyariz.iOS.Shchurov.HorizontalWheelView.HorizontalWheelView.SCROLL_STATE_IDLE => "IDLE",
                    Polyariz.iOS.Shchurov.HorizontalWheelView.HorizontalWheelView.SCROLL_STATE_DRAGGING => "DRAGGING",
                    Polyariz.iOS.Shchurov.HorizontalWheelView.HorizontalWheelView.SCROLL_STATE_SETTLING => "SETTLING",
                    _ => "UNKNOWN"
                };

                stateLabel.Text = $"State: {stateText}";
            }
        }

        private class WheelListener : HorizontalWheelViewListener
        {
            private readonly AdvancedSampleViewController controller;

            public WheelListener(AdvancedSampleViewController controller)
            {
                this.controller = controller;
            }

            public override void OnRotationChanged(double radians)
            {
                controller.UpdateAngleLabel();
            }

            public override void OnScrollStateChanged(int state)
            {
                controller.UpdateStateLabel(state);
            }
        }
    }
}
