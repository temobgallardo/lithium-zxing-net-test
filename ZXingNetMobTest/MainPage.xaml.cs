using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;

namespace ZXingNetMobTest
{
  public partial class MainPage : ContentPage
  {
    private string _barcodeResult;

    public string BarcodeResult
    {
      get => _barcodeResult;
      set
      {
        if (_barcodeResult != value) _barcodeResult = value;
        OnPropertyChanged(nameof(BarcodeResult));
      }
    }

    private double pixels;

    public double Pixels
    {
      get { return pixels; }
      set
      {
        pixels = value;
        OnPropertyChanged(nameof(Pixels));
      }
    }

    public MainPage()
    {
      InitializeComponent();
      BindingContext = this;
    }

    private void OnScanResult(Result result)
    {
      Console.WriteLine($"Barcode captured: {result.Text}");
      this.BarcodeResult = result.Text;
      OnPropertyChanged(nameof(BarcodeResult));
      this.BarcodeScannerView.IsScanning = false;
    }

    private async void Button_ClickedAsync(object sender, EventArgs e)
    {
      this.BarcodeScannerView.Options.CameraResolutionSelector = this.SelectHighestResolutionMatchingDisplayAspectRatio;
      this.BarcodeScannerView.IsScanning = true;


      //var scanner = new ZXing.Mobile.MobileBarcodeScanner();
      //var result = await scanner.Scan();
      //if (result is null) return;

      //BarcodeResult = result.Text;
      //Console.WriteLine("Scanned Barcode: " + result.Text);
    }

    private CameraResolution SelectHighestResolutionMatchingDisplayAspectRatio(List<CameraResolution> availableResolutions)
    {
      if (availableResolutions == null || availableResolutions.Count < 1)
      {
        throw new ArgumentNullException(nameof(availableResolutions));
      }
     
      var pixels = availableResolutions.Select(r => r.Width * r.Height).ToArray();
      var seed = Pixels * 2000000;
      // The closest to 2megapixels
      var bestPerformanceIndex = FindIndexOfCloserToNumber((int)seed, pixels);

      return availableResolutions[bestPerformanceIndex];
    }

    /// <summary>
    /// Compute what is the index containing the closer element to a given value
    /// </summary>
    /// <param name="seed">Values should be as close to this one</param>
    /// <param name="pixels">A specific name but this is a list with values</param>
    /// <returns>Closest element index to the seed starting from the left</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    private int FindIndexOfCloserToNumber(int seed, int[] pixels)
    {
      if (pixels is null) throw new ArgumentNullException(nameof(pixels));
      if (pixels.Length == 0) throw new ArgumentException(nameof(pixels));

      var absolutes = new int[pixels.Length];
      for (int i = 0; i < pixels.Length; i++)
      {
        absolutes[i] = Math.Abs(pixels[i] - seed);
      }

      var min = absolutes.Min();
      for (int i = 0; i < absolutes.Length; i++)
      {
        if (absolutes[i] == absolutes.Min()) return i;
      }

      return -1;
    }
  }
}
