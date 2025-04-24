
// Type: MiBandApp.Tools.TaskExtensions
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable disable
namespace MiBandApp.Tools
{
  public static class TaskExtensions
  {
    public static void NoAwait(this ConfiguredTaskAwaitable task)
    {
    }

    public static void NoAwait(this Task task)
    {
    }
  }
}
