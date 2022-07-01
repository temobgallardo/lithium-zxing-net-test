using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;

namespace ZXingNetMobTest
{
  public partial class MainPage : ContentPage
  {
    private double _pixels;
    private string _barcodeResult;
    private List<CameraResolution> _availableResolutions;
    private CameraResolution _selectedResolution;

    public string BarcodeResult
    {
      get => _barcodeResult;
      set
      {
        if (_barcodeResult != value) _barcodeResult = value;
        this.OnPropertyChanged(nameof(BarcodeResult));
      }
    }

    public double Pixels
    {
      get { return _pixels; }
      set
      {
        _pixels = value;
        this.OnPropertyChanged(nameof(Pixels));
      }
    }

    public List<CameraResolution> AvailableResolutions
    {
      get => this._availableResolutions;
      set
      {
        if (this._availableResolutions == value)
        {
          return;
        }

        this._availableResolutions = value;
        this.OnPropertyChanged(nameof(AvailableResolutions));
      }
    }

    public CameraResolution SelectedResolution
    {
      get => this._selectedResolution;
      set
      {
        if (this._selectedResolution == value)
        {
          return;
        }

        this._selectedResolution = value;
        this.OnPropertyChanged(nameof(SelectedResolution));
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
      //this.BarcodeScannerView.IsScanning = false;
    }

    private async void ButtonScan_ClickedAsync(object sender, EventArgs e)
    {
      await this.Navigation.PushAsync(new CustomScanPage());
    }

    private CameraResolution SelectHighestResolutionMatchingDisplayAspectRatio(List<CameraResolution> availableResolutions)
    {
      if (availableResolutions == null || availableResolutions.Count < 1)
      {
        throw new ArgumentNullException(nameof(availableResolutions));
      }

      this.AvailableResolutions = availableResolutions;
      this.OnPropertyChanged(nameof(AvailableResolutions));

      var pixels = availableResolutions.Select(r => r.Width * r.Height).ToArray();
      var seed = Pixels * 2000000;
      // The closest to 2megapixels
      var bestPerformanceIndex = FindIndexOfCloserToNumber((int)seed, pixels);

      if (this.SelectedResolution != null)
      {
        return this.SelectedResolution;
      }
      else
      {
        this.SelectedResolution = availableResolutions[bestPerformanceIndex];
      }

      return availableResolutions[bestPerformanceIndex];
    }


    private CameraResolution GetCameraResolutions(List<CameraResolution> availableResolutions)
    {
      if (availableResolutions == null || availableResolutions.Count < 1)
      {
        throw new ArgumentNullException(nameof(availableResolutions));
      }

      //this.AvailableResolutions = availableResolutions;
      
      return availableResolutions[0];
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

    private async void ScanBarcodeScannerExceptionPicker(object sender, EventArgs e)
    {
      await this.Navigation.PushAsync(new BarcodeScannerExceptionWhenPickerDisplayed());
    }

    private async void ScanCustomScannerWithXaml(object sender, EventArgs e)
    {
      await this.Navigation.PushAsync(new CustomScannerWithXaml());
    }

    private async void BarcodeScannerMobilePoC(object sender, EventArgs e)
    {
      await this.Navigation.PushAsync(new BarcodeScannerMobilePoC());
    }
  }
}
