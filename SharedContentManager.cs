// Decompiled with JetBrains decompiler
// Type: Magicka.SharedContentManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka;

public class SharedContentManager : ContentManager
{
  private static SharedContentManager.CommonContentManager sCommon;
  private Dictionary<string, int> mLoadedAssets;

  public SharedContentManager(IServiceProvider serviceProvider)
    : base(serviceProvider)
  {
    if (SharedContentManager.sCommon == null)
      SharedContentManager.sCommon = new SharedContentManager.CommonContentManager(this.ServiceProvider, "content");
    ++SharedContentManager.sCommon.mManagers;
    this.mLoadedAssets = new Dictionary<string, int>();
  }

  private new static string GetCleanPath(string iPath)
  {
    if (!Path.IsPathRooted(iPath))
      iPath = Path.Combine(SharedContentManager.sCommon.RootDirectory, iPath);
    return Path.GetFullPath(iPath).ToLowerInvariant();
  }

  public override T Load<T>(string assetName)
  {
    assetName = SharedContentManager.GetCleanPath(assetName);
    T obj = SharedContentManager.sCommon.Load<T>(assetName);
    int num;
    if (!this.mLoadedAssets.TryGetValue(assetName, out num))
      num = 0;
    this.mLoadedAssets[assetName] = num + 1;
    return obj;
  }

  public override void Unload()
  {
    if (this.mLoadedAssets == null)
      throw new ObjectDisposedException(this.GetType().Name);
    SharedContentManager.sCommon.Unload(this);
    this.mLoadedAssets.Clear();
    base.Unload();
  }

  protected override void Dispose(bool disposing)
  {
    this.Unload();
    base.Dispose(disposing);
    this.mLoadedAssets = (Dictionary<string, int>) null;
    --SharedContentManager.sCommon.mManagers;
    if (SharedContentManager.sCommon.mManagers != 0)
      return;
    SharedContentManager.sCommon.Dispose();
    SharedContentManager.sCommon = (SharedContentManager.CommonContentManager) null;
  }

  private class CommonContentManager : ContentManager
  {
    public int mManagers;
    private readonly Dictionary<string, SharedContentManager.CommonContentManager.ReferencedAsset> mReferences;
    private Stack<SharedContentManager.CommonContentManager.ReferencedAsset> mLoadStack = new Stack<SharedContentManager.CommonContentManager.ReferencedAsset>(16 /*0x10*/);

    public CommonContentManager(IServiceProvider serviceProvider, string iRootDirectory)
      : base(serviceProvider, iRootDirectory)
    {
      this.mReferences = new Dictionary<string, SharedContentManager.CommonContentManager.ReferencedAsset>();
    }

    public override T Load<T>(string assetName)
    {
      assetName = SharedContentManager.GetCleanPath(assetName);
      SharedContentManager.CommonContentManager.ReferencedAsset referencedAsset;
      if (!this.mReferences.TryGetValue(assetName, out referencedAsset))
      {
        referencedAsset = new SharedContentManager.CommonContentManager.ReferencedAsset(assetName);
        this.mLoadStack.Push(referencedAsset);
        try
        {
          referencedAsset.Asset = (object) this.ReadAsset<T>(assetName, (Action<IDisposable>) null);
        }
        finally
        {
          this.mLoadStack.Pop();
        }
        if (!this.mReferences.ContainsKey(assetName))
          this.mReferences.Add(assetName, referencedAsset);
      }
      if (this.mLoadStack.Count > 0)
        this.mLoadStack.Peek().Children.Add(referencedAsset);
      ++referencedAsset.References;
      return (T) referencedAsset.Asset;
    }

    public void Unload(SharedContentManager container)
    {
      foreach (KeyValuePair<string, int> mLoadedAsset in container.mLoadedAssets)
        this.Dec(this.mReferences[mLoadedAsset.Key], mLoadedAsset.Value);
    }

    private void Dec(
      SharedContentManager.CommonContentManager.ReferencedAsset iAsset,
      int iDec)
    {
      iAsset.References -= iDec;
      if (iAsset.References != 0)
        return;
      for (int index = 0; index < iAsset.Children.Count; ++index)
        this.Dec(iAsset.Children[index], 1);
      if (iAsset.Asset is IDisposable)
        (iAsset.Asset as IDisposable).Dispose();
      this.mReferences.Remove(iAsset.Name);
    }

    private class ReferencedAsset
    {
      public string Name;
      public object Asset;
      public int References;
      public List<SharedContentManager.CommonContentManager.ReferencedAsset> Children;

      public ReferencedAsset(string iName)
      {
        this.Name = iName;
        this.Children = new List<SharedContentManager.CommonContentManager.ReferencedAsset>();
        this.Asset = (object) null;
        this.References = 0;
      }
    }
  }
}
