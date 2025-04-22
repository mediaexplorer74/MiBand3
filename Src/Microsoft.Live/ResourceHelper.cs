// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.ResourceHelper
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System.Collections.Generic;
using Windows.ApplicationModel.Resources.Core;

#nullable disable
namespace Microsoft.Live
{
  internal static class ResourceHelper
  {
    private static readonly Dictionary<string, string> ErrorMappings = new Dictionary<string, string>();

    static ResourceHelper()
    {
      ResourceHelper.ErrorMappings["AsyncOperationInProgress"] = "Another async operation is already in progress.";
      ResourceHelper.ErrorMappings["BackgroundTransferServiceAddError"] = "An error occurred while adding the request to the BackgroundTransferService.";
      ResourceHelper.ErrorMappings["BackgroundTransferServiceRemoveError"] = "An error occurred while removing a request to the BackgroundTransferService.";
      ResourceHelper.ErrorMappings["BackgroundUploadResponseHandlerError"] = "An error occurred while reading the response of the BackgroundUpload.";
      ResourceHelper.ErrorMappings["ConnectionError"] = "A connection to the server could not be established.";
      ResourceHelper.ErrorMappings["CantLogout"] = "Log out is not supported because the user is logged in to this PC with a Microsoft account.";
      ResourceHelper.ErrorMappings["ConsentNotGranted"] = "User has not granted the application consent to access data in Windows Live.";
      ResourceHelper.ErrorMappings["FileNameInvalid"] = "Input parameter '{0}' is invalid.  '{0}' contains invalid characters.";
      ResourceHelper.ErrorMappings["InvalidAuthClient"] = "The app is not configured correctly to use Live Connect services. To configure your app, please follow the instructions on http://go.microsoft.com/fwlink/?LinkId=220871.";
      ResourceHelper.ErrorMappings["InvalidNullOrEmptyParameter"] = "Input parameter '{0}' is invalid.  '{0}' cannot be null or empty.";
      ResourceHelper.ErrorMappings["InvalidNullParameter"] = "Input parameter '{0}' is invalid.  '{0}' cannot be null.";
      ResourceHelper.ErrorMappings["LoginPopupAlreadyOpen"] = "The login page is already open.";
      ResourceHelper.ErrorMappings["NoResponseData"] = "There was no response from the server for this request.";
      ResourceHelper.ErrorMappings["NotOnUiThread"] = "The method '{0}' must be called from the application's UI thread.";
      ResourceHelper.ErrorMappings["NoUploadLinkFound"] = "Could not determine the upload location.  Make sure the path points to a file resource id.";
      ResourceHelper.ErrorMappings["RelativeUrlRequired"] = "Input parameter '{0}' is invalid.  '{0}' must be a relative url.";
      ResourceHelper.ErrorMappings["RootVisualNotRendered"] = "Can not invoke the login page before the application root visual is rendered.";
      ResourceHelper.ErrorMappings["ServerError"] = "An error occurred while performing the operation. Please try again later.";
      ResourceHelper.ErrorMappings["ServerErrorWithStatus"] = "An error occurred while performing the operation. Server returned a response with status {0}.";
      ResourceHelper.ErrorMappings["StreamNotReadable"] = "Input parameter '{0}' is invalid.  Stream is not readable.";
      ResourceHelper.ErrorMappings["StreamNotWritable"] = "Input parameter '{0}' is invalid.  Stream is not writable.";
      ResourceHelper.ErrorMappings["UriMissingFileName"] = "Input parameter '{0}' is invalid. '{0}' must contain a filename.";
      ResourceHelper.ErrorMappings["UriMustBeRootedInSharedTransfers"] = "Input parameter '{0}' is invalid. '{0}' must be rooted in \\shared\\transfers.";
      ResourceHelper.ErrorMappings["UrlInvalid"] = "Input parameter '{0}' is invalid.  '{0}' must be a valid URI.";
      ResourceHelper.ErrorMappings["UserNotLoggedIn"] = "User did not log in.  Call LiveAuthClient.LoginAsync to log in.";
    }

    public static string GetString(string name)
    {
      return ResourceHelper.ErrorMappings[name] != null ? ResourceHelper.ErrorMappings[name] : ResourceManager.Current.MainResourceMap.GetValue("ms-resource:///Microsoft.Live/Resources/" + name).ValueAsString;
    }
  }
}
