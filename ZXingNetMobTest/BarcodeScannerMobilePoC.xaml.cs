using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarcodeScanner.Mobile.Core;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace ZXingNetMobTest
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class BarcodeScannerMobilePoC : ContentPage
  {
    public BarcodeScannerMobilePoC()
    {
      InitializeComponent();

      //Methods.SetSupportBarcodeFormat(BarcodeFormats.Code39 | BarcodeFormats.QRCode | BarcodeFormats.Code128 | BarcodeFormats.Ean13 | BarcodeFormats.Ean8);
      _ = On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
    }

    private async void CameraView_OnDetected(object sender, OnDetectedEventArg e)
    {
      List<BarcodeResult> obj = e.BarcodeResults;

      string result = string.Empty;
      for (int i = 0; i < obj.Count; i++)
      {
        result += $"Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
      }

      Device.BeginInvokeOnMainThread(async () =>
      {
        await DisplayAlert("Result", result, "OK");
        // If you want to start scanning again
        this.BarcodeScannCamera.IsScanning = true;
      });
    }

    protected override async void OnAppearing()
    {
      base.OnAppearing();

      bool allowed = await BarcodeScanner.Mobile.XamarinForms.Methods.AskForRequiredPermission();

      this.BarcodeScannCamera.CaptureQuality = CaptureQuality.Medium;

      this.BarcodeScannCamera.IsScanning = true;
    }

    protected override void OnDisappearing()
    {
      this.BarcodeScannCamera.IsScanning = false;

      base.OnDisappearing();
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
      await Navigation.PopModalAsync();
    }

    private void FlashlightButton_Clicked(object sender, EventArgs e)
    {
      BarcodeScannCamera.TorchOn = !BarcodeScannCamera.TorchOn;
    }

    private void SwitchCameraButton_Clicked(object sender, EventArgs e)
    {
      BarcodeScannCamera.CameraFacing = BarcodeScannCamera.CameraFacing == CameraFacing.Back
                                ? CameraFacing.Front
                                : CameraFacing.Back;
    }
  }
}