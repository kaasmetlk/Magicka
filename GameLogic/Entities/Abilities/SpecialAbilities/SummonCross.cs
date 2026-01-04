// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SummonCross
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class SummonCross : SpecialAbility, IAbilityEffect
{
  public static readonly int EFFECT_BUBBLE = "paladin_bubble".GetHashCodeCustom();
  private static List<SummonCross> sCache;
  private PlayState mPlayState;
  private static CharacterTemplate sTemplate;
  private NonPlayerCharacter mCross;
  private ISpellCaster mOwner;
  private Vector3 mPosition;

  public static SummonCross GetInstance()
  {
    if (SummonCross.sCache.Count <= 0)
      return new SummonCross();
    SummonCross instance = SummonCross.sCache[SummonCross.sCache.Count - 1];
    SummonCross.sCache.RemoveAt(SummonCross.sCache.Count - 1);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    SummonCross.sTemplate = iPlayState.Content.Load<CharacterTemplate>("data/characters/cross");
    SummonCross.sCache = new List<SummonCross>(iNr);
    for (int index = 0; index < iNr; ++index)
      SummonCross.sCache.Add(new SummonCross());
  }

  private SummonCross()
    : base(Magicka.Animations.cast_magick_global, "#magick_cross".GetHashCodeCustom())
  {
  }

  public SummonCross(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_cross".GetHashCodeCustom())
  {
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mPosition = iPosition;
    return this.Execute();
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    this.mPlayState = iPlayState;
    this.mOwner = iOwner;
    this.mPosition = this.mOwner.Position;
    return this.Execute();
  }

  private bool Execute()
  {
    this.mCross = NonPlayerCharacter.GetInstance(this.mPlayState);
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      if (this.mCross == null)
        return false;
      Vector3 mPosition = this.mPosition;
      mPosition.Y += 3f;
      double nearestPosition = (double) this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref mPosition, out this.mPosition, MovementProperties.Default);
      this.mCross.Initialize(SummonCross.sTemplate, this.mPosition, 0);
      if (this.mOwner is Magicka.GameLogic.Entities.Character)
        this.mCross.Summoned(this.mOwner as Magicka.GameLogic.Entities.Character);
      Agent ai = this.mCross.AI;
      ai.SetOrder(Order.None, ReactTo.None, Order.None, 0, 0, 0, (AIEvent[]) null);
      ai.AlertRadius = 0.0f;
      Matrix result;
      Matrix.CreateRotationY((float) SpecialAbility.RANDOM.NextDouble() * 6.28318548f, out result);
      this.mCross.Body.Orientation = result;
      this.mPlayState.EntityManager.AddEntity((Entity) this.mCross);
      ai.Owner.SpawnAnimation = Magicka.Animations.spawn;
      ai.Owner.ChangeState((BaseState) RessurectionState.Instance);
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        TriggerActionMessage iMessage = new TriggerActionMessage();
        iMessage.ActionType = TriggerActionType.SpawnNPC;
        iMessage.Handle = this.mCross.Handle;
        iMessage.Template = this.mCross.Type;
        iMessage.Id = this.mCross.UniqueID;
        iMessage.Position = this.mCross.Position;
        iMessage.Direction = result.Forward;
        if (this.mOwner != null)
          iMessage.Scene = (int) this.mOwner.Handle;
        iMessage.Bool0 = false;
        iMessage.Point1 = 170;
        iMessage.Point2 = 170;
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
      }
    }
    (this.mOwner as Magicka.GameLogic.Entities.Character).AddBubbleShield(new AuraStorage(new AuraBuff(new BuffStorage(new BuffResistance(new Resistance()
    {
      ResistanceAgainst = Elements.All,
      Multiplier = 0.0f
    }), VisualCategory.Defensive, Spell.SHIELDCOLOR)), AuraTarget.Self, AuraType.Buff, SummonCross.EFFECT_BUBBLE, 1E+08f, 0.0f, VisualCategory.Special, Spell.SHIELDCOLOR, (int[]) null, Factions.NONE));
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    return true;
  }

  public bool IsDead => this.mCross.Dead;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
  }

  public void OnRemove()
  {
  }
}
