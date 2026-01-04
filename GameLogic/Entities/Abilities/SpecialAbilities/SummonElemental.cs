// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SummonElemental
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class SummonElemental : SpecialAbility
{
  private static SummonElemental mSingelton;
  private static volatile object mSingeltonLock = new object();
  private static CharacterTemplate sTemplate;
  public static readonly int SOUND = "magick_summon_elemental".GetHashCodeCustom();
  public static readonly int EFFECT = "magick_summonelemental".GetHashCodeCustom();
  private static AudioEmitter sAudioEmitter = new AudioEmitter();

  public static SummonElemental Instance
  {
    get
    {
      if (SummonElemental.mSingelton == null)
      {
        lock (SummonElemental.mSingeltonLock)
        {
          if (SummonElemental.mSingelton == null)
            SummonElemental.mSingelton = new SummonElemental();
        }
      }
      return SummonElemental.mSingelton;
    }
  }

  private SummonElemental()
    : base(Magicka.Animations.cast_magick_direct, "#magick_selemental".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    Vector3 iDirection = new Vector3((float) SpecialAbility.RANDOM.NextDouble(), 0.0f, (float) SpecialAbility.RANDOM.NextDouble());
    iDirection.Normalize();
    return this.Execute(iPosition, iDirection, (ISpellCaster) null, iPlayState);
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    return this.Execute(iOwner.Position, iOwner.Direction, iOwner, iPlayState);
  }

  private bool Execute(
    Vector3 iPosition,
    Vector3 iDirection,
    ISpellCaster iOwner,
    PlayState iPlayState)
  {
    Vector3 result = iPosition;
    Vector3 iDirection1 = iDirection;
    Vector3 vector3;
    Vector3.Multiply(ref iDirection1, 3f, out vector3);
    Vector3.Add(ref result, ref vector3, out result);
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      ElementalEgg instance = ElementalEgg.GetInstance(iPlayState);
      double nearestPosition = (double) iPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result, out vector3, MovementProperties.Default);
      result = vector3;
      instance.Initialize(ref result, ref iDirection, 0);
      instance.SetSummoned(iOwner);
      iPlayState.EntityManager.AddEntity((Entity) instance);
      if (NetworkManager.Instance.State == NetworkState.Server && NetworkManager.Instance.State == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
        {
          ActionType = TriggerActionType.SpawnElemental,
          Handle = instance.Handle,
          Arg = (int) iOwner.Handle,
          Position = result,
          Direction = iDirection1
        });
    }
    EffectManager.Instance.StartEffect(SummonElemental.EFFECT, ref result, ref iDirection1, out VisualEffectReference _);
    SummonElemental.sAudioEmitter.Position = iPosition;
    SummonElemental.sAudioEmitter.Up = Vector3.Up;
    SummonElemental.sAudioEmitter.Forward = iDirection;
    AudioManager.Instance.PlayCue(Banks.Spells, SummonElemental.SOUND, SummonElemental.sAudioEmitter);
    return true;
  }

  internal void SetTemplate(CharacterTemplate iTemplate) => SummonElemental.sTemplate = iTemplate;
}
