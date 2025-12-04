# HorizontalWheelView Sample App

This is a demonstration application showing how to use the HorizontalWheelView library in an iOS app.

## What's Included

### AdvancedSampleViewController (Default)

The main demo screen showing ALL features of HorizontalWheelView:

- **Live wheel view** at the top showing 3D perspective marks
- **Real-time angle display** in degrees, radians, and turn fraction
- **Scroll state indicator** showing IDLE/DRAGGING/SETTLING
- **Interactive controls**:
  - ✅ Snap to Marks - Enable snap-to-mark behavior
  - ✅ End Lock - Lock rotation at boundaries
  - ✅ Only Positive Values - Restrict to 0-360° range
  - ✅ Show Active Range - Highlight active marks
  - ✅ Haptic Feedback - UIPickerView-style haptic feedback
  - ✅ Sound Feedback - Picker click sound on mark crossing
  - 🎚️ Marks Count Slider - Adjust number of marks (10-100)

### SampleViewController (Alternative)

A simpler example demonstrating basic usage:

- Rotating rocket/airplane image
- Angle display
- Basic wheel configuration

To use this instead, change `SceneDelegate.cs`:
```csharp
var vc = new SampleViewController();
```

## Building and Running

### Prerequisites

- macOS with Xcode 15+
- .NET 10.0-ios SDK
- iOS Simulator or physical device (iOS 15.0+)

### Build Steps

```bash
# From the Sample project directory
dotnet build

# Or from the root directory
cd "HorizontalWheelView.Sample.iOS"
dotnet build
```

### Running on Simulator

```bash
# Build and run
dotnet build -t:Run

# Or open in Xcode
open HorizontalWheelView.Sample.iOS.csproj
```

## Testing Features

### Haptic Feedback
**Note**: Haptic feedback only works on physical devices, not in the simulator.

1. Build and deploy to a physical iPhone (iOS 10+)
2. Enable "Haptic Feedback" switch
3. Rotate the wheel slowly
4. Feel a light tap when crossing each mark

### Sound Feedback

1. Ensure device volume is up and not in silent mode
2. Enable "Sound Feedback" switch
3. Rotate the wheel
4. Hear the UIPickerView click sound at each mark

### Snap to Marks

1. Enable "Snap to Marks" switch
2. Drag the wheel and release
3. Watch it snap to the nearest mark automatically

### End Lock & Only Positive Values

1. Enable both "End Lock" and "Only Positive Values"
2. Try rotating past 0° or 360°
3. Rotation stops at boundaries (no wrap-around)

### Active Range Coloring

1. Ensure "Show Active Range" is enabled
2. Rotate the wheel left and right
3. Observe marks changing color based on rotation angle
4. Marks within the rotation range are highlighted

## Code Examples

### Accessing the Wheel View

```csharp
using Polyariz.iOS.Shchurov.HorizontalWheelView;

// Create wheel
var wheelView = new HorizontalWheelView();

// Configure
wheelView.SetMarksCount(40);
wheelView.SetNormalColor(UIColor.White);
wheelView.SetActiveColor(UIColor.SystemBlue);
```

### Listening to Events

```csharp
public class MyListener : HorizontalWheelViewListener
{
    public override void OnRotationChanged(double radians)
    {
        double degrees = radians * 180 / Math.PI;
        Console.WriteLine($"Angle: {degrees}°");
    }

    public override void OnScrollStateChanged(int state)
    {
        Console.WriteLine($"State: {state}");
    }
}

wheelView.SetListener(new MyListener());
```

### Setting Angles Programmatically

```csharp
// Set to 90 degrees
wheelView.SetDegreesAngle(90);

// Set to π/2 radians
wheelView.SetRadiansAngle(Math.PI / 2);

// Set to quarter turn
wheelView.SetCompleteTurnFraction(0.25);
```

## Project Structure

```
HorizontalWheelView.Sample.iOS/
├── Main.cs                              # App entry point
├── AppDelegate.cs                       # Application delegate
├── SceneDelegate.cs                     # Scene setup
├── AdvancedSampleViewController.cs      # Full feature demo
├── SampleViewController.cs              # Simple rocket demo
├── Info.plist                           # App configuration
├── Entitlements.plist                   # App entitlements
├── LaunchScreen.storyboard              # Launch screen
└── Assets.xcassets/                     # App icons and images
```

## Library Reference

This sample app references the HorizontalWheelView library via ProjectReference:

```xml
<ItemGroup>
  <ProjectReference Include="..\HorizontalWheelView.Library.iOS\HorizontalWheelView.Library.iOS.csproj" />
</ItemGroup>
```

The library namespace is:
```csharp
using Polyariz.iOS.Shchurov.HorizontalWheelView;
```

## Customization Ideas

Try modifying the sample to experiment with:

1. **Different color schemes**
   ```csharp
   wheelView.SetNormalColor(0xFF00FF00);    // Green
   wheelView.SetActiveColor(0xFFFF0000);     // Red
   ```

2. **Different mark counts**
   ```csharp
   wheelView.SetMarksCount(20);  // Fewer marks
   wheelView.SetMarksCount(100); // More marks
   ```

3. **Content insets for custom layouts**
   ```csharp
   wheelView.ContentEdgeInsets = new UIEdgeInsets(10, 20, 10, 20);
   ```

4. **Custom animations based on rotation**
   ```csharp
   public override void OnRotationChanged(double radians)
   {
       // Animate views, change colors, play sounds, etc.
       myView.Alpha = Math.Abs(Math.Sin(radians));
   }
   ```

## Troubleshooting

### App doesn't launch
- Ensure you have iOS 15.0+ deployment target
- Check that .NET 10.0-ios SDK is installed
- Verify Xcode command line tools are configured

### Wheel doesn't appear
- Check console for constraint errors
- Verify view hierarchy in Xcode View Debugger
- Ensure background colors aren't conflicting

### No haptic feedback
- Use a physical device (not simulator)
- Device must support haptic feedback (iPhone 7+)
- Check that "Haptic Feedback" switch is enabled

### No sound
- Check device volume
- Disable silent mode
- Verify "Sound Feedback" switch is enabled

## Learn More

- Full API documentation: `../HorizontalWheelView.Library.iOS/README.md`
- Original Android library: https://github.com/shchurov/HorizontalWheelView

---

Happy coding! 🎉
