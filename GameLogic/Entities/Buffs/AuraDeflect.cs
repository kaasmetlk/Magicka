// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.AuraDeflect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct AuraDeflect
{
  private static readonly Random RANDOM = new Random();
  public float Strength;

  public AuraDeflect(float iStrength) => this.Strength = iStrength;

  public AuraDeflect(ContentReader iInput) => this.Strength = iInput.ReadSingle();

  public float Execute(
    Entity iOwner,
    float iDeltaTime,
    AuraTarget iAuraTarget,
    int iEffect,
    float iRadius)
  {
    Vector3 position1 = iOwner.Position;
    Vector3 up = Vector3.Up;
    List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(position1, this.Strength, false);
    foreach (Entity entity in entities)
    {
      if (entity is MissileEntity missileEntity && !missileEntity.Dead)
      {
        Vector3 position2 = missileEntity.Position;
        Vector3 result1;
        Vector3.Subtract(ref position2, ref position1, out result1);
        float num = result1.Length();
        result1.Y = 0.0f;
        result1.Normalize();
        Vector3 velocity = missileEntity.Body.Velocity;
        float angle = (float) (AuraDeflect.RANDOM.NextDouble() - 0.5) * 0.7853982f;
        Quaternion result2;
        Quaternion.CreateFromAxisAngle(ref up, angle, out result2);
        Vector3.Transform(ref result1, ref result2, out result1);
        Vector3.Multiply(ref result1, (float) ((double) velocity.Length() * (double) iDeltaTime * 100.0 * (1.0 - (double) num / (double) this.Strength)), out result1);
        Vector3.Add(ref result1, ref velocity, out result1);
        missileEntity.Body.Velocity = result1;
      }
    }
    iOwner.PlayState.EntityManager.ReturnEntityList(entities);
    return 1f;
  }
}
