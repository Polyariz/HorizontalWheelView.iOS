using UIKit;
using Foundation;

namespace HorizontalWheelView.Sample.iOS
{
    /// <summary>
    /// Main tab bar controller with two example tabs
    /// </summary>
    public class MainTabBarController : UITabBarController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Configure tab bar appearance - dark background with light buttons and text
            TabBar.BarStyle = UIBarStyle.Black;
            TabBar.BackgroundColor = UIColor.Black;
            TabBar.TintColor = UIColor.White; // Color for selected items
            TabBar.UnselectedItemTintColor = UIColor.LightGray; // Color for unselected items

            // Create Simple Sample tab
            var simpleSample = new SampleViewController();
            simpleSample.Title = "Simple";
            simpleSample.TabBarItem = new UITabBarItem(
                title: "Simple",
                image: UIImage.GetSystemImage("airplane"),
                selectedImage: UIImage.GetSystemImage("airplane.circle.fill")
            );

            // Create Advanced Sample tab
            var advancedSample = new AdvancedSampleViewController();
            advancedSample.Title = "Advanced";
            advancedSample.TabBarItem = new UITabBarItem(
                title: "Advanced",
                image: UIImage.GetSystemImage("slider.horizontal.3"),
                selectedImage: UIImage.GetSystemImage("slider.horizontal.below.rectangle")
            );

            // Set view controllers
            ViewControllers = new UIViewController[]
            {
                simpleSample,
                advancedSample
            };

            // Start with Advanced tab (shows all features)
            SelectedIndex = 1;
        }
    }
}
