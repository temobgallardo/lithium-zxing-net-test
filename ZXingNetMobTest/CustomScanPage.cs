using System;
using System.Collections.Generic;
using System.Linq;
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
        BottomText = "Scanning will happen automatically \n For ",
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
  }
}