// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.MoveAbilities.MoveAbility
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Microsoft.Xna.Framework.Content;
using System;
using System.Reflection;

#nullable disable
namespace Magicka.GameLogic.Entities.MoveAbilities;

public abstract class MoveAbility
{
  protected Animations[] mAnimationKeys;

  protected MoveAbility(ContentReader iInput)
  {
    this.mAnimationKeys = new Animations[iInput.ReadInt32()];
    for (int index = 0; index < this.mAnimationKeys.Length; ++index)
      this.mAnimationKeys[index] = (Animations) Enum.Parse(typeof (Animations), iInput.ReadString(), true);
  }

  public abstract void Execute(Agent iAgent);

  public abstract void Update(Agent iAgent, float iDeltaTime);

  protected static Type GetType(string name)
  {
    Type[] types = Assembly.GetExecutingAssembly().GetTypes();
    for (int index = 0; index < types.Length; ++index)
    {
      if (types[index].BaseType == typeof (MoveAbility) && types[index].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
        return types[index];
    }
    return (Type) null;
  }

  public static MoveAbility Read(ContentReader iInput)
  {
    return (MoveAbility) MoveAbility.GetType(iInput.ReadString()).GetConstructor(new Type[1]
    {
      typeof (ContentReader)
    }).Invoke(new object[1]{ (object) iInput });
  }

  public virtual float GetFuzzyWeight(Agent iAgent) => 0.0f;

  public abstract float GetMaxRange();

  public abstract float GetMinRange();

  public abstract float GetAngle();

  public abstract float GetForceMultiplier();
}
