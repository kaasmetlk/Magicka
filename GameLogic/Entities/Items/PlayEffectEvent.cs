// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.PlayEffectEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct PlayEffectEvent
{
  private int mEffectHash;
  private bool mFollow;
  private bool mWorldAlign;

  public PlayEffectEvent(int iHash)
  {
    this.mFollow = false;
    this.mEffectHash = iHash;
    this.mWorldAlign = false;
  }

  public PlayEffectEvent(string iName)
  {
    this.mEffectHash = iName.GetHashCodeCustom();
    this.mFollow = false;
    this.mWorldAlign = false;
  }

  public PlayEffectEvent(int iHash, bool iFollow)
  {
    this.mEffectHash = iHash;
    this.mFollow = iFollow;
    this.mWorldAlign = false;
  }

  public PlayEffectEvent(int iHash, bool iFollow, bool iWorldAlign)
  {
    this.mEffectHash = iHash;
    this.mFollow = iFollow;
    this.mWorldAlign = iWorldAlign;
  }

  public PlayEffectEvent(string iName, bool iFollow)
  {
    this.mEffectHash = iName.GetHashCodeCustom();
    this.mFollow = iFollow;
    this.mWorldAlign = false;
  }

  public PlayEffectEvent(ContentReader iInput)
  {
    this.mFollow = iInput.ReadBoolean();
    this.mWorldAlign = iInput.ReadBoolean();
    this.mEffectHash = iInput.ReadString().ToLowerInvariant().GetHashCodeCustom();
  }

  public void Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
  {
    Vector3 position = iItem.Position;
    if (iPosition.HasValue)
      position = iPosition.Value;
    Vector3 iDirection = new Vector3(0.0f, 1f, 0.0f);
    if (!this.mWorldAlign)
      iDirection = iItem.GetOrientation().Forward;
    VisualEffectReference oRef;
    EffectManager.Instance.StartEffect(this.mEffectHash, ref position, ref iDirection, out oRef);
    if (!this.mFollow)
      return;
    switch (iItem)
    {
      case MissileEntity _:
        (iItem as MissileEntity).AddEffectReference(this.mEffectHash, oRef);
        break;
      case Item _:
        (iItem as Item).AddEffectReference(this.mEffectHash, oRef);
        break;
    }
  }

  public int EffectHash => this.mEffectHash;
}
