// Decompiled with JetBrains decompiler
// Type: Magicka.Misc.Singleton`1
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.Misc;

public class Singleton<T> where T : class, new()
{
  private const string EXCEPTION_TEXT = "Cannot create a new instance of {0} outside of Instance property.";
  private static bool sAllowInstancing = false;
  private static T sInstance;
  private static volatile object sLock = new object();

  public static T Instance
  {
    get
    {
      if ((object) Singleton<T>.sInstance == null)
      {
        lock (Singleton<T>.sLock)
        {
          if ((object) Singleton<T>.sInstance == null)
          {
            Singleton<T>.sAllowInstancing = true;
            Singleton<T>.sInstance = new T();
            Singleton<T>.sAllowInstancing = false;
          }
        }
      }
      return Singleton<T>.sInstance;
    }
  }

  public static bool HasInstance => (object) Singleton<T>.sInstance != null;

  public Singleton()
  {
    if (!Singleton<T>.sAllowInstancing)
      throw new Exception($"Cannot create a new instance of {typeof (T).Name} outside of Instance property.");
  }
}
