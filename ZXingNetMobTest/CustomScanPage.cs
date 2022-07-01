using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace ZXingNetMobTest
{
  public class CustomScanPage : ContentPage
  {
    private readonly ZXingScannerView zxing;
    private readonly ZXingDefaultOverlay overlay;
    private List<CameraResolution> availableResolutions;
    private CameraResolution selectedResolution;
    private List<string> availableResolutionsString;
    private int selectedResolutionIndex;

    public CameraResolution SelectedResolution
    {
      get => this.selectedResolution;
      set
      {
        if (this.selectedResolution == value)
        {
          return;
        }

        this.selectedResolution = value;
        this.OnPropertyChanged(nameof(this.SelectedResolution));
      }
    }

    public List<CameraResolution> AvailableResolutions
    {
      get => this.availableResolutions;
      set
      {

        if (this.availableResolutions == value)
        {
          return;
        }

        this.availableResolutions = value;
        this.AvailableResolutionsString = value.Select(r => $"{r.Height}x{r.Width}").ToList();
        this.OnPropertyChanged(nameof(this.AvailableResolutions));
      }
    }

    public List<string> AvailableResolutionsString
    {
      get => this.availableResolutionsString;
      set
      {

        if (this.availableResolutionsString == value)
        {
          return;
        }

        this.availableResolutionsString = value;
        this.OnPropertyChanged(nameof(this.AvailableResolutionsString));
      }
    }

    public int SelectedResolutionIndex
    {
      get => this.selectedResolutionIndex;
      set
      {

        if (this.selectedResolutionIndex == value)
        {
          return;
        }

        this.selectedResolutionIndex = value;
        this.SelectedResolution = this.AvailableResolutions[value];
        this.OnPropertyChanged(nameof(this.SelectedResolutionIndex));
        this.RefreshBindings();
      }
    }

    public CustomScanPage() : base()
    {
      zxing = new ZXingScannerView
      {
        HorizontalOptions = LayoutOptions.FillAndExpand,
        VerticalOptions = LayoutOptions.FillAndExpand,
        AutomationId = "zxingScannerView",
      };
      zxing.OnScanResult += (result) =>
        Device.BeginInvokeOnMainThread(async () =>
        {
          // Stop analysis until we navigate away so we don't keep reading barcodes
          zxing.IsAnalyzing = false;

          // Show an alert
          await DisplayAlert("Scanned Barcode", result.Text, "OK");

          // Navigate away
          await Navigation.PopAsync();
        });

      overlay = new ZXingDefaultOverlay
      {
        TopText = "Hold your phone up to the barcode",
        BottomText = "Scanning will happen automatically \n For Barcode 128 ~640x400 res is advised",
        ShowFlashButton = zxing.HasTorch,
        AutomationId = "zxingDefaultOverlay",
      };
      overlay.FlashButtonClicked += (sender, e) =>
      {
        zxing.IsTorchOn = !zxing.IsTorchOn;
      };
      var grid = new Grid
      {
        VerticalOptions = LayoutOptions.FillAndExpand,
        HorizontalOptions = LayoutOptions.FillAndExpand,
      };

      zxing.Options.CameraResolutionSelector = this.SetAvailableResolutions;

      var picker = new Picker()
      {
        Title = "Select a resolution",
        ItemsSource = this.AvailableResolutionsString,
        WidthRequest = 100,
        HeightRequest = 50,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.End
      };

      picker.SetBinding(Picker.ItemsSourceProperty, nameof(this.AvailableResolutionsString));
      picker.SetBinding(Picker.SelectedIndexProperty, nameof(this.SelectedResolutionIndex));

      grid.Children.Add(zxing);
      grid.Children.Add(overlay);
      grid.Children.Add(picker);

      // The root page of your application
      Content = grid;
      this.BindingContext = this;
    }

    public CustomScanPage(List<CameraResolution> cameraResolutions) : base()
    {
      this.AvailableResolutions = cameraResolutions;

      zxing = new ZXingScannerView
      {
        HorizontalOptions = LayoutOptions.FillAndExpand,
        VerticalOptions = LayoutOptions.FillAndExpand,
        AutomationId = "zxingScannerView",
      };
      zxing.OnScanResult += (result) =>
        Device.BeginInvokeOnMainThread(async () =>
        {
          // Stop analysis until we navigate away so we don't keep reading barcodes
          zxing.IsAnalyzing = false;

          // Show an alert
          await DisplayAlert("Scanned Barcode", result.Text, "OK");

          // Navigate away
          await Navigation.PopAsync();
        });

      overlay = new ZXingDefaultOverlay
      {
        TopText = "Hold your phone up to the barcode",
        BottomText = "Scanning will happen automatically \n For Barcode 128 ~640x400 res is advised",
        ShowFlashButton = zxing.HasTorch,
        AutomationId = "zxingDefaultOverlay",
      };
      overlay.FlashButtonClicked += (sender, e) =>
      {
        zxing.IsTorchOn = !zxing.IsTorchOn;
      };
      var grid = new Grid
      {
        VerticalOptions = LayoutOptions.FillAndExpand,
        HorizontalOptions = LayoutOptions.FillAndExpand,
      };

      //zxing.Options.CameraResolutionSelector = this.SelectResolutionMatchingDisplayAspectRatio;

      var picker = new Picker()
      {
        Title = "Select a resolution",
        TitleColor = Color.White,
        TextColor = Color.White,
        ItemsSource = this.AvailableResolutionsString,
        WidthRequest = 100,
        HeightRequest = 50,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.End
      };

      picker.PropertyChanged += this.APropertyChanged;
      picker.SetBinding(Picker.ItemsSourceProperty, nameof(this.AvailableResolutionsString));
      picker.SetBinding(Picker.SelectedIndexProperty, nameof(this.SelectedResolutionIndex));

      grid.Children.Add(zxing);
      grid.Children.Add(overlay);
      grid.Children.Add(picker);

      // The root page of your application
      Content = grid;
      this.BindingContext = this;
    }

    private CameraResolution SetAvailableResolutions(List<CameraResolution> availableResolutions)
    {
      if (this.AvailableResolutions == null)
      {
        this.AvailableResolutions = availableResolutions;
        this.RefreshBindings();
      }

      if (this.SelectedResolution != null)
      {
        return this.SelectedResolution;
      }

      this.SelectedResolution = this.AvailableResolutions[this.SelectedResolutionIndex];
      this.RefreshBindings();

      return this.SelectedResolution;
    }

    private CameraResolution SelectResolutionMatchingDisplayAspectRatio(List<CameraResolution> availableResolutions)
    {
      // Setting a resolution not in the availables will cause an exception
      // Todo: [EHS] Find out where to handle this exception
      if (availableResolutions == null || availableResolutions.Count < 1)
      {
        throw new ArgumentNullException(nameof(availableResolutions));
      }

      // There is no reading issues on iOS, returning highest resolution
      if (DeviceInfo.Platform == DevicePlatform.iOS)
      {
        return this.SelectLowestResolutionMatchingDisplayAspectRatio(availableResolutions);
      }

      // Capturing input data in native resolution may create massive data which will slow the barcode reading. Android recommends around 2 Megapixels.
      // Selecting a resolution < 2 Mega Pixels for better performance. The smaller the fastest but accuracy get compromized.
      // TODO: [EHS] Very slow for Code38 and 128. At ~640*500 it is much faster but may affect some users.
      int seed = Convert.ToInt32(1280 * 720);

      var resolutionsLessThan2MegasPixels = availableResolutions.Where(r => r.Width * r.Height <= seed).ToList();

      var resWithAspectRatio = this.SelectLowestResolutionMatchingDisplayAspectRatio(resolutionsLessThan2MegasPixels);

      //var pixels = availableResolutions.Select(r => r.Width * r.Height).ToArray();
      //var performanceResIndex = this.FindIndexOfCloserToNumber(seed: seed, pixels);
      //return availableResolutions[performanceResIndex];
      return resWithAspectRatio;
    }

    public CameraResolution SelectLowestResolutionMatchingDisplayAspectRatio(List<CameraResolution> availableResolutions)
    {
      CameraResolution result = null;
      //a tolerance of 0.1 should not be visible to the user
      double aspectTolerance = 0.1;
      var displayOrientationHeight = DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait ? DeviceDisplay.MainDisplayInfo.Height : DeviceDisplay.MainDisplayInfo.Width;
      var displayOrientationWidth = DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait ? DeviceDisplay.MainDisplayInfo.Width : DeviceDisplay.MainDisplayInfo.Height;
      //calculatiing our targetRatio
      var targetRatio = displayOrientationHeight / displayOrientationWidth;
      var targetHeight = displayOrientationHeight;
      var minDiff = double.MaxValue;
      //camera API lists all available resolutions from highest to lowest, perfect for us
      //making use of this sorting, following code runs some comparisons to select the lowest resolution that matches the screen aspect ratio and lies within tolerance
      //selecting the lowest makes Qr detection actual faster most of the time
      foreach (var r in availableResolutions.Where(r => Math.Abs(((double)r.Width / r.Height) - targetRatio) < aspectTolerance))
      {
        //slowly going down the list to the lowest matching solution with the correct aspect ratio
        if (Math.Abs(r.Height - targetHeight) < minDiff)
        {
          minDiff = Math.Abs(r.Height - targetHeight);
          result = r;
        }
      }

      return result;
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();

      zxing.IsScanning = true;
    }

    protected override void OnDisappearing()
    {
      zxing.IsScanning = false;

      base.OnDisappearing();
    }

    private void RefreshBindings()
    {
      //this.OnPropertyChanged(nameof(this.AvailableResolutions));
      this.OnPropertyChanged(nameof(this.SelectedResolution));
      this.OnPropertyChanged(nameof(this.SelectedResolutionIndex));
    }



    private void APropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      var picker = (Picker)sender;

      var index = picker.SelectedIndex;
      var elements = picker.Items;
      var a = 0;
    }
  }
}