// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.SpawnItemEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct SpawnItemEvent
{
  private static int RANDOM = "random".GetHashCodeCustom();
  public int Type;
  public float DespawnTime;

  public SpawnItemEvent(int iType)
  {
    this.Type = iType;
    this.DespawnTime = 0.0f;
  }

  public SpawnItemEvent(int iType, float iDespawnTime)
  {
    this.Type = iType;
    this.DespawnTime = iDespawnTime;
  }

  public SpawnItemEvent(ContentReader iInput)
  {
    string lowerInvariant = iInput.ReadString().ToLowerInvariant();
    this.Type = lowerInvariant.GetHashCodeCustom();
    if (this.Type != SpawnItemEvent.RANDOM)
      Item.CacheWeapon(this.Type, iInput.ContentManager.Load<Item>("Data/Items/Wizard/" + lowerInvariant));
    this.DespawnTime = 0.0f;
  }

  public void Execute(Entity iItem, Entity iTarget)
  {
    if (NetworkManager.Instance.State == NetworkState.Client || this.Type == SpawnItemEvent.RANDOM)
      return;
    Item pickableIntstance = Item.GetPickableIntstance();
    Item.GetCachedWeapon(this.Type, pickableIntstance);
    pickableIntstance.Body.SetActive();
    pickableIntstance.Detach();
    Vector3 position = iItem.Position;
    position.Y += (pickableIntstance.Body.CollisionSkin.GetPrimitiveLocal(0) as Box).SideLengths.Y;
    Matrix orientation = iItem.Body.Orientation;
    pickableIntstance.Body.MoveTo(position, orientation);
    Vector3 vector3 = new Vector3((float) (MagickaMath.Random.NextDouble() - 0.5) * 2f, (float) MagickaMath.Random.NextDouble() * 7f, (float) (MagickaMath.Random.NextDouble() - 0.5) * 2f);
    pickableIntstance.Body.Velocity = vector3;
    if ((double) this.DespawnTime > 0.0)
      pickableIntstance.Despawn(this.DespawnTime);
    iItem.PlayState.EntityManager.AddEntity((Entity) pickableIntstance);
    pickableIntstance.Body.EnableBody();
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    TriggerActionMessage iMessage = new TriggerActionMessage();
    iMessage.ActionType = TriggerActionType.SpawnItem;
    iMessage.Handle = pickableIntstance.Handle;
    iMessage.Template = this.Type;
    iMessage.Position = pickableIntstance.Position;
    iMessage.Direction = vector3;
    iMessage.Bool0 = true;
    iMessage.Point0 = 0;
    iMessage.Time = this.DespawnTime;
    Quaternion.CreateFromRotationMatrix(ref orientation, out iMessage.Orientation);
    NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
  }
}
