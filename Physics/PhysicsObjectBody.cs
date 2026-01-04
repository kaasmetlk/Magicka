// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.PhysicsObjectBody
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Physics;

#nullable disable
namespace Magicka.Physics;

public class PhysicsObjectBody : Body
{
  private bool mReactToCharacters;

  public override void ProcessCollisionPoints(float dt)
  {
    if (!this.mReactToCharacters)
    {
      for (int index = 0; index < this.CollisionSkin.Collisions.Count; ++index)
      {
        CollisionInfo collision = this.CollisionSkin.Collisions[index];
        if (collision.SkinInfo.Skin0 != null && collision.SkinInfo.Skin1 != null)
        {
          if (collision.SkinInfo.Skin0.Owner is CharacterBody)
            collision.SkinInfo.IgnoreSkin1 = true;
          if (collision.SkinInfo.Skin1.Owner is CharacterBody)
            collision.SkinInfo.IgnoreSkin0 = true;
        }
      }
    }
    base.ProcessCollisionPoints(dt);
  }

  public bool ReactToCharacters
  {
    get => this.mReactToCharacters;
    set => this.mReactToCharacters = value;
  }
}
