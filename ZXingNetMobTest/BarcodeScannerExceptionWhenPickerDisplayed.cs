﻿
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
  public class BarcodeScannerExceptionWhenPickerDisplayed : ContentPage
  {
    private readonly ZXingScannerView zxing;
    private readonly ZXingDefaultOverlay overlay;
    private readonly Picker picker;

    public BarcodeScannerExceptionWhenPickerDisplayed() : base()
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
        BottomText = "Scanning will happen automatically",
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

      var picker = new Picker()
      {
        Title = "Select a resolution",
        WidthRequest = 100,
        HeightRequest = 50,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.End
      };

      grid.Children.Add(zxing);
      grid.Children.Add(overlay);
      grid.Children.Add(picker);

      // The root page of your application
      Content = grid;
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
  }
}