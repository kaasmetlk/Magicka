// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SummonBug
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

internal class SummonBug(Magicka.Animations iAnimation) : SpecialAbility(iAnimation, Helper.GetHashCodeCustom("#specab_swarm"))
{
  private static CharacterTemplate sTemplate;

  internal static void InitialzeCache(PlayState iPlayState)
  {
    SummonBug.sTemplate = iPlayState.Content.Load<CharacterTemplate>("data/characters/bugswarm");
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iPlayState);
      if (instance == null)
        return false;
      Vector3 result1 = iOwner.Direction;
      Vector3 result2 = iOwner.Position;
      Vector3.Multiply(ref result1, 4f, out result1);
      Vector3.Add(ref result2, ref result1, out result2);
      result1 = iOwner.Direction;
      Vector3 oPoint;
      double nearestPosition = (double) iPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref result2, out oPoint, MovementProperties.Default);
      Segment iSeg = new Segment();
      iSeg.Origin = oPoint;
      ++iSeg.Origin.Y;
      iSeg.Delta.Y = -10f;
      Vector3 oPos;
      if (iPlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
        result2 = oPos;
      instance.Initialize(SummonBug.sTemplate, result2, 0);
      if (iOwner is Character)
        instance.Summoned(iOwner as Character);
      instance.ForceAnimation(Magicka.Animations.spawn);
      Agent ai = instance.AI;
      ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, (AIEvent[]) null);
      ai.AlertRadius = 12f;
      Vector3 up = new Vector3();
      up.Y = 1f;
      Vector3 position = new Vector3();
      Matrix result3;
      Matrix.CreateWorld(ref position, ref result1, ref up, out result3);
      instance.CharacterBody.DesiredDirection = result1;
      instance.Body.Orientation = result3;
      iPlayState.EntityManager.AddEntity((Entity) instance);
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
        iMessage.Direction = result3.Forward;
        if (iOwner != null)
          iMessage.Scene = (int) iOwner.Handle;
        iMessage.Bool0 = false;
        iMessage.Point1 = 170;
        iMessage.Point2 = 170;
        NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
      }
    }
    return true;
  }
}
