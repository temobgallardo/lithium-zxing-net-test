using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;

namespace ZXingNetMobTest
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class CustomScannerWithXaml : ContentPage
  {
    private int selectedResolutionIndex;
    private int numberOfResolutions;
    private bool recommendedSelected = true;
    private Stopwatch timeTryingToScan = new Stopwatch();

    public CustomScannerWithXaml()
    {
      InitializeComponent();

      this.BarcodeScannerView.Options.CameraResolutionSelector += SelectResolutionMatchingDisplayAspectRatio;


      this.BindingContext = this;
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();

      this.BarcodeScannerView.IsScanning = true;

      this.timeTryingToScan.Start();
    }

    protected override void OnDisappearing()
    {
      this.BarcodeScannerView.IsScanning = false;

      this.timeTryingToScan.Stop();

      base.OnDisappearing();
    }

    private CameraResolution SelectResolutionMatchingDisplayAspectRatio(List<CameraResolution> availableResolutions)
    {
      // Setting a resolution not in the availables will cause an exception
      // Todo: [EHS] Find out where to handle this exception
      if (availableResolutions == null || availableResolutions.Count < 1)
      {
        throw new ArgumentNullException(nameof(availableResolutions));
      }

      //if (this.timeTryingToScan.ElapsedMilliseconds > 5000)
      //{
      //  //if (selectedResolutionIndex)
      //  return availableResolutions[selectedResolutionIndex--];
      //}

      if (!this.recommendedSelected)
      {
        return availableResolutions[this.selectedResolutionIndex];
      }

      // There is no reading issues on iOS, returning highest resolution
      if (DeviceInfo.Platform == DevicePlatform.iOS)
      {
        return this.SelectLowestResolutionMatchingDisplayAspectRatio(availableResolutions);
      }

      // Capturing input data in native resolution may create massive data which will slow the barcode reading. Android recommends around 2 Megapixels.
      // Selecting a resolution < 2 Mega Pixels for better performance. The smaller the fastest but accuracy get compromized.
      // TODO: [EHS] Very slow for Code38 and 128. At ~640*500 it is much faster but may affect some users.
      int seed = 1280 * 720;

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
      double aspectTolerance = 0.4;
      var displayOrientationHeight = DeviceDisplay.MainDisplayInfo.Width;
      var displayOrientationWidth = DeviceDisplay.MainDisplayInfo.Height;
      //calculatiing our targetRatio
      var targetRatio = displayOrientationWidth / displayOrientationHeight;
      var targetHeight = displayOrientationHeight;
      var minDiff = double.MaxValue;

      var reses = availableResolutions.Where(r => Math.Abs(((double)r.Width / r.Height) - targetRatio) < aspectTolerance).ToList();
      var aspects = availableResolutions.Select(r => (double)r.Width / r.Height).ToList();
      var tolerance = availableResolutions.Select(r => Math.Abs((double)r.Width / r.Height) - targetRatio).ToList();

      var reses1 = new List<CameraResolution>();
      foreach (var r in availableResolutions)
      {
        var ab = ((double)r.Width / r.Height);
        var c = ab - targetRatio;
        var d = Math.Abs(c);
        var e = d < aspectTolerance;

        if (e)
        {
          reses1.Add(r);
        }
      }

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

    private void BarcodeScannerView_OnScanResult(ZXing.Result result)
    {
      Device.BeginInvokeOnMainThread(async () =>
      {
        // Stop analysis until we navigate away so we don't keep reading barcodes
        this.BarcodeScannerView.IsAnalyzing = true;

        // Show an alert
        await DisplayAlert("Scanned Barcode", result.Text, "OK");

        // Navigate away
        //await Navigation.PopAsync();
      });
    }

    private void LowerResolution(object sender, EventArgs e)
    {
      this.selectedResolutionIndex--;

      if (this.selectedResolutionIndex < 0)
      {
        this.selectedResolutionIndex = this.numberOfResolutions - 1;
      }

      this.recommendedSelected = false;
    }

    private void IncreaseResolution(object sender, EventArgs e)
    {
      this.selectedResolutionIndex++;

      if (this.selectedResolutionIndex > this.numberOfResolutions - 1)
      {
        this.selectedResolutionIndex = 0;
      }

      this.recommendedSelected = false;
    }

    private void SelectRecommendedResolution(object sender, EventArgs e)
    {
      this.recommendedSelected = true;
    }
  }
}