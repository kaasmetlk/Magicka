// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Reflection;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public abstract class SpecialAbility
{
  private Magicka.Animations mAnimation;
  public static readonly int SOUND_MAGICK_FAIL = "magick_fail".GetHashCodeCustom();
  protected static readonly Random RANDOM = new Random();
  protected double mTimeStamp;
  private long mLastExecute = DateTime.Now.Ticks;
  private readonly int mDisplayName;

  public SpecialAbility(Magicka.Animations iAnimation, int iDisplayName)
  {
    this.mAnimation = iAnimation;
    this.mDisplayName = iDisplayName;
  }

  public Magicka.Animations Animation => this.mAnimation;

  public virtual bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (iOwner != null)
      this.mTimeStamp = iOwner.PlayState.PlayTime;
    return true;
  }

  public virtual bool Execute(ISpellCaster iOwner, Elements elements, PlayState iPlayState)
  {
    if (iOwner != null)
      this.mTimeStamp = iOwner.PlayState.PlayTime;
    return true;
  }

  public virtual bool Execute(Vector3 iPosition, PlayState iPlayState) => true;

  private static Type GetAbilityType(string iName)
  {
    Type[] types = Assembly.GetExecutingAssembly().GetTypes();
    for (int index = 0; index < types.Length; ++index)
    {
      if (types[index].IsSubclassOf(typeof (SpecialAbility)) && types[index].Name.Equals(iName, StringComparison.OrdinalIgnoreCase))
        return types[index];
    }
    return (Type) null;
  }

  public static SpecialAbility Read(ContentReader iInput)
  {
    Type abilityType = SpecialAbility.GetAbilityType(iInput.ReadString());
    string str1 = iInput.ReadString();
    string str2 = iInput.ReadString();
    Magicka.Animations animations = Magicka.Animations.cast_self;
    if (!string.IsNullOrEmpty(str1))
      animations = (Magicka.Animations) Enum.Parse(typeof (Magicka.Animations), str1, true);
    Elements[] elementsArray = new Elements[iInput.ReadInt32()];
    Elements elements = Elements.None;
    if (elementsArray.Length > 0)
    {
      try
      {
        for (int index = 0; index < elementsArray.Length; ++index)
        {
          elementsArray[index] = (Elements) iInput.ReadInt32();
          elements |= elementsArray[index];
        }
        if (!string.IsNullOrEmpty(str2))
        {
          int hashCodeCustom = str2.ToLowerInvariant().GetHashCodeCustom();
          return (SpecialAbility) abilityType.GetConstructor(new Type[3]
          {
            typeof (Magicka.Animations),
            typeof (Elements[]),
            typeof (int)
          }).Invoke(new object[3]
          {
            (object) animations,
            (object) elementsArray,
            (object) hashCodeCustom
          });
        }
        return (SpecialAbility) abilityType.GetConstructor(new Type[2]
        {
          typeof (Magicka.Animations),
          typeof (Elements[])
        }).Invoke(new object[2]
        {
          (object) animations,
          (object) elementsArray
        });
      }
      catch (Exception ex)
      {
      }
    }
    if (!string.IsNullOrEmpty(str2))
    {
      int hashCodeCustom = str2.ToLowerInvariant().GetHashCodeCustom();
      return (SpecialAbility) abilityType.GetConstructor(new Type[2]
      {
        typeof (Magicka.Animations),
        typeof (int)
      }).Invoke(new object[2]
      {
        (object) animations,
        (object) hashCodeCustom
      });
    }
    return (SpecialAbility) abilityType.GetConstructor(new Type[1]
    {
      typeof (Magicka.Animations)
    }).Invoke(new object[1]{ (object) animations });
  }

  public int DisplayName => this.mDisplayName;
}
