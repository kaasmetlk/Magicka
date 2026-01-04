// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.SpawnEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct SpawnEvent
{
  public int Type;
  public Animations IdleAnimation;
  public Animations SpawnAnimation;
  public Order Order;
  public ReactTo ReactTo;
  public Order Reaction;
  public float Health;
  public float Rotation;
  public Vector3 Offset;

  public SpawnEvent(int iType)
  {
    this.Type = iType;
    this.IdleAnimation = Animations.None;
    this.SpawnAnimation = Animations.None;
    this.Order = Order.Attack;
    this.ReactTo = ReactTo.None;
    this.Reaction = Order.None;
    this.Health = 1f;
    this.Rotation = 0.0f;
    this.Offset = new Vector3();
  }

  public SpawnEvent(int iType, Order iOrder, ReactTo iReactTo, Order iReaction)
  {
    this.Type = iType;
    this.IdleAnimation = Animations.None;
    this.SpawnAnimation = Animations.None;
    this.Order = iOrder;
    this.ReactTo = iReactTo;
    this.Reaction = iReaction;
    this.Health = 1f;
    this.Rotation = 0.0f;
    this.Offset = new Vector3();
  }

  public SpawnEvent(int iType, Order iOrder, ReactTo iReactTo, Order iReaction, float iHealth)
  {
    this.Type = iType;
    this.IdleAnimation = Animations.None;
    this.SpawnAnimation = Animations.None;
    this.Order = iOrder;
    this.ReactTo = iReactTo;
    this.Reaction = iReaction;
    this.Health = iHealth;
    this.Rotation = 0.0f;
    this.Offset = new Vector3();
  }

  public SpawnEvent(ContentReader iInput)
  {
    string iString = iInput.ReadString();
    this.Type = iString.GetHashCodeCustom();
    this.IdleAnimation = (Animations) Enum.Parse(typeof (Animations), iInput.ReadString(), true);
    this.SpawnAnimation = (Animations) Enum.Parse(typeof (Animations), iInput.ReadString(), true);
    this.Health = iInput.ReadSingle();
    this.Order = (Order) iInput.ReadByte();
    this.ReactTo = (ReactTo) iInput.ReadByte();
    this.Reaction = (Order) iInput.ReadByte();
    this.Rotation = MathHelper.ToRadians(iInput.ReadSingle());
    this.Offset = iInput.ReadVector3();
    iInput.ContentManager.Load<CharacterTemplate>("Data/Characters/" + iString);
  }

  public void Execute(Entity iItem, Entity iTarget)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iItem.PlayState);
    CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.Type);
    Matrix result1 = iItem.Body.Orientation;
    Matrix result2;
    Matrix.CreateRotationY(this.Rotation, out result2);
    Matrix.Multiply(ref result2, ref result1, out result1);
    Vector3 result3 = iItem.Position;
    Vector3 result4;
    Vector3.TransformNormal(ref this.Offset, ref result1, out result4);
    Vector3.Add(ref result4, ref result3, out result3);
    if (NetworkManager.Instance.State == NetworkState.Server)
      NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref new TriggerActionMessage()
      {
        ActionType = TriggerActionType.SpawnNPC,
        Handle = instance.Handle,
        Template = this.Type,
        Id = 0,
        Position = result3,
        Direction = result1.Forward,
        Bool0 = false,
        Point0 = 0,
        Point1 = 0,
        Point2 = (int) this.SpawnAnimation,
        Point3 = (int) this.IdleAnimation
      });
    instance.Initialize(cachedTemplate, result3, 0);
    if (iItem.PlayState.Level.CurrentScene.RuleSet != null)
    {
      if (iItem is Character)
        instance.Faction = (iItem as Character).Faction;
      if (iItem.PlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
        (iItem.PlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, true);
    }
    instance.Body.Orientation = result1;
    instance.CharacterBody.DesiredDirection = result1.Forward;
    instance.HitPoints = this.Health * instance.MaxHitPoints;
    instance.AI.SetOrder(this.Order, this.ReactTo, this.Reaction, 0, 0, 0, (AIEvent[]) null);
    if (this.SpawnAnimation != Animations.None && this.SpawnAnimation != Animations.idle && this.SpawnAnimation != Animations.idle_agg)
    {
      instance.SpawnAnimation = this.SpawnAnimation;
      instance.ChangeState((BaseState) RessurectionState.Instance);
    }
    if (this.IdleAnimation != Animations.None)
      instance.SpecialIdleAnimation = this.IdleAnimation;
    iItem.PlayState.EntityManager.AddEntity((Entity) instance);
  }
}
