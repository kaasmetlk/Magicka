// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SummonFlamer
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

internal class SummonFlamer(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_tsal_blaze"))
{
  private static CharacterTemplate sTemplate;
  private PlayState mPlayState;
  private ISpellCaster mOwner;
  private Vector3 mPosition;
  private Vector3 mDirection;

  internal static void InitializeCache(PlayState iPlayState)
  {
    SummonFlamer.sTemplate = iPlayState.Content.Load<CharacterTemplate>("data/characters/conscious_blaze");
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    this.mOwner = (ISpellCaster) null;
    this.mPlayState = iPlayState;
    return this.Execute(iPosition, new Vector3((float) (MagickaMath.Random.NextDouble() - 0.5) * 2f, 0.0f, (float) (MagickaMath.Random.NextDouble() - 0.5) * 2f));
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    base.Execute(iOwner, iPlayState);
    this.mOwner = iOwner;
    this.mPlayState = iPlayState;
    Vector3 result1 = iOwner.Position;
    Vector3 result2 = iOwner.Direction;
    Vector3.Multiply(ref result2, 4f, out result2);
    Vector3.Add(ref result1, ref result2, out result1);
    return this.Execute(result1, iOwner.Direction);
  }

  private bool Execute(Vector3 iPosition, Vector3 iDirection)
  {
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      this.mDirection = iDirection;
      this.mPosition = iPosition;
      NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
      if (instance == null)
        return false;
      Vector3 iPoint = iPosition;
      iPoint.Y += 3f;
      double nearestPosition = (double) this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPoint, out this.mPosition, MovementProperties.Default);
      Vector3 oPos;
      if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, new Segment()
      {
        Origin = this.mPosition,
        Delta = {
          Y = -10f
        }
      }))
        this.mPosition = oPos;
      instance.Initialize(SummonFlamer.sTemplate, this.mPosition, 0);
      if (this.mOwner is Character)
        instance.Summoned(this.mOwner as Character, false, true);
      instance.ForceAnimation(Magicka.Animations.spawn);
      Agent ai = instance.AI;
      ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      ai.AlertRadius = 12f;
      Vector3 up = new Vector3();
      up.Y = 1f;
      Vector3 position = new Vector3();
      Matrix result;
      Matrix.CreateWorld(ref position, ref this.mDirection, ref up, out result);
      instance.CharacterBody.DesiredDirection = this.mDirection;
      instance.Body.Orientation = result;
      this.mPlayState.EntityManager.AddEntity((Entity) instance);
      ai.Owner.SpawnAnimation = Magicka.Animations.spawn;
      ai.Owner.ChangeState((BaseState) RessurectionState.Instance);
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        TriggerActionMessage iMessage = new TriggerActionMessage();
        iMessage.ActionType = TriggerActionType.SpawnNPC;
        iMessage.Handle = instance.Handle;
        iMessage.Template = instance.Type;
        iMessage.Id = instance.UniqueID;
        iMessage.Position = instance.Position;
        iMessage.Direction = result.Forward;
        if (this.mOwner != null)
          iMessage.Scene = (int) this.mOwner.Handle;
        iMessage.Bool0 = false;
        iMessage.Point1 = 170;
        iMessage.Point2 = 170;
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
      }
    }
    return true;
  }
}
