
// Type: MiBandApp.Services.EmailComposer
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBandApp.Tools;
using System;
using System.Text;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Streams;

#nullable disable
namespace MiBandApp.Services
{
  public class EmailComposer
  {
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private readonly BandController _bandController;
    private readonly MiBandApp.Storage.Settings.Settings _settings;

    public EmailComposer(BandController bandController, MiBandApp.Storage.Settings.Settings settings)
    {
      this._bandController = bandController;
      this._settings = settings;
    }

    public async void ComposeEmail(IStorageFile file = null, string theme = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(theme ?? "Bind Mi Band Support");
      stringBuilder.Append(string.Format(" (User {0} Version {1}", (object) this._settings.GetUserId(), 
          (object) this._settings.AppVersion));
      if (this._bandController.DeviceInfo.Value != null)
        stringBuilder.Append(string.Format(" {0} {1}", (object) this._bandController.DeviceInfo.Value.Model, 
            (object) this._bandController.DeviceInfo.Value.FirmwareVersion));
      if (SystemInfo.SystemVersion != null)
        stringBuilder.Append(string.Format(" {0}", (object) SystemInfo.SystemVersion));
      stringBuilder.Append(")");
      EmailMessage emailMessage = new EmailMessage();
      emailMessage.Subject = stringBuilder.ToString();
      
      emailMessage.To.Add(new EmailRecipient(
          "fakemail@fakemail777777777777777777777777777777777777777777.com",//"support@bindmiband.com", 
          "Bind Mi Band Support"
          ));
      if (file != null)
      {
        emailMessage.Attachments.Add(new EmailAttachment(((IStorageItem) file).Name, 
            (IRandomAccessStreamReference) file));
        emailMessage.Body = (string.Format("<{0}>\n\n", 
            (object) this._resourceLoader.GetString("AboutPage_DiagnosticPackageMessage_EmailHint")));
      }
      await EmailManager.ShowComposeNewEmailAsync(emailMessage);
    }
  }
}
