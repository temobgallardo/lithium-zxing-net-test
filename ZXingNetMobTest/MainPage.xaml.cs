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

    public MainPage()
    {
      InitializeComponent();
      BindingContext = this;
    }

    private async void OnScanResult(Result result)
    {
      Console.WriteLine($"Barcode captured: {result.Text}");
      this.BarcodeResult = result.Text;
      OnPropertyChanged(nameof(BarcodeResult));
      this.BarcodeScannerView.IsScanning = false;
    }

    private async void Button_ClickedAsync(object sender, EventArgs e)
    {
      var scanner = new ZXing.Mobile.MobileBarcodeScanner();

      this.BarcodeScannerView.Options.CameraResolutionSelector = this.SelectHighestResolutionMatchingDisplayAspectRatio;
      this.BarcodeScannerView.IsScanning = true;

      //var result = await scanner.Scan();

      //if (result is null) return;

      //BarcodeResult = result.Text;
      //Console.WriteLine("Scanned Barcode: " + result.Text);
    }

    private CameraResolution SelectHighestResolutionMatchingDisplayAspectRatio(List<CameraResolution> availableResolutions)
    {
      if (availableResolutions == null || availableResolutions.Count < 1)
        return new CameraResolution() { Width = 600, Height = 400 };

      var bestPerformance = availableResolutions.First(r => (r.Width >= 600 && r.Width < 650) && (r.Height >= 400 && r.Height < 550));

      var bestOption = availableResolutions.OrderByDescending(c => c.Width).ThenByDescending(c => c.Height).FirstOrDefault();
      // Get the highest resolution on the list, highest is the last one in the list

      var worstResolution = availableResolutions[availableResolutions.Count - 1];
      return availableResolutions[11];
    }
  }
}
