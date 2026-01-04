// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.CameraShakeEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct CameraShakeEvent
{
  private float mTTL;
  private float mMagnitude;
  private bool mFromPosition;

  public CameraShakeEvent(float iTTL, float iMagnitude)
  {
    this.mTTL = iTTL;
    this.mMagnitude = iMagnitude;
    this.mFromPosition = false;
  }

  public CameraShakeEvent(float iTTL, float iMagnitude, bool iFromPosition)
  {
    this.mTTL = iTTL;
    this.mMagnitude = iMagnitude;
    this.mFromPosition = iFromPosition;
  }

  public CameraShakeEvent(ContentReader iInput)
  {
    this.mTTL = iInput.ReadSingle();
    this.mMagnitude = iInput.ReadSingle();
    this.mFromPosition = iInput.ReadBoolean();
  }

  public void Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
  {
    if (this.mFromPosition)
    {
      Vector3 position = iItem.Position;
      if (iPosition.HasValue)
        position = iPosition.Value;
      iItem.PlayState.Camera.CameraShake(position, this.mMagnitude, this.mTTL);
    }
    else
      iItem.PlayState.Camera.CameraShake(this.mMagnitude, this.mTTL);
  }
}
