// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.VladZap
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class VladZap : SpecialAbility
{
  private const float RANGE = 15f;
  private static List<VladZap> sCache;
  private HitList mHitList = new HitList(32 /*0x20*/);
  public static readonly int SOUND = "magick_thunderbolt".GetHashCodeCustom();

  public static VladZap GetInstance()
  {
    if (VladZap.sCache.Count <= 0)
      return new VladZap();
    VladZap instance = VladZap.sCache[VladZap.sCache.Count - 1];
    VladZap.sCache.RemoveAt(VladZap.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr)
  {
    VladZap.sCache = new List<VladZap>(iNr);
    for (int index = 0; index < iNr; ++index)
      VladZap.sCache.Add(new VladZap());
  }

  public VladZap(Magicka.Animations iAnimation)
    : base(iAnimation, "#specab_lightbolt".GetHashCodeCustom())
  {
  }

  private VladZap()
    : base(Magicka.Animations.cast_magick_direct, "#magick_vladzap".GetHashCodeCustom())
  {
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mHitList.Clear();
    if (!iOwner.HasStatus(StatusEffects.Wet))
      this.mHitList.Add(iOwner);
    DamageCollection5 iDamages = new DamageCollection5();
    iDamages.AddDamage(new Damage()
    {
      Amount = 850f,
      AttackProperty = AttackProperties.Damage,
      Element = Elements.Lightning,
      Magnitude = 1f
    });
    Vector3 position = iOwner.Position;
    List<Entity> entities = iOwner.PlayState.EntityManager.GetEntities(position, 12f, false);
    List<VladCharacter> vladCharacterList = new List<VladCharacter>();
    foreach (Entity entity in entities)
    {
      if (entity is VladCharacter vladCharacter)
        vladCharacterList.Add(vladCharacter);
    }
    int count = vladCharacterList.Count;
    if (count == 3)
    {
      for (int index1 = 0; index1 < count; ++index1)
      {
        VladCharacter vladCharacter = vladCharacterList[index1];
        int index2 = (index1 + 1) % count;
        VladCharacter iTarget = vladCharacterList[index2];
        LightningBolt.GetLightning().Cast(iOwner, vladCharacter.CastSource.Translation, (Entity) iTarget, this.mHitList, Spell.LIGHTNINGCOLOR * 2f, 1f, 15f, ref iDamages, iPlayState);
      }
    }
    iOwner.PlayState.EntityManager.ReturnEntityList(entities);
    AudioManager.Instance.PlayCue(Banks.Spells, VladZap.SOUND, iOwner.AudioEmitter);
    iPlayState.Camera.CameraShake(iOwner.Position, 1.5f, 0.333f);
    return true;
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState) => false;
}
