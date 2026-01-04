// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.SpawnMissileEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct SpawnMissileEvent
{
  public bool Facing;
  public int Type;
  public Vector3 Velocity;

  public SpawnMissileEvent(ContentReader iInput)
  {
    this.Type = iInput.ReadString().ToLowerInvariant().GetHashCodeCustom();
    this.Velocity = iInput.ReadVector3();
    this.Facing = iInput.ReadBoolean();
  }

  public void Execute(Entity iItem, Entity iTarget)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Item cachedWeapon = Item.GetCachedWeapon(this.Type);
    MissileEntity iMissile = (MissileEntity) null;
    this.SpawnMissile(ref iMissile, this.Velocity, cachedWeapon, iItem.PlayState, iItem);
    if (iMissile == null)
      return;
    iItem.PlayState.EntityManager.AddEntity((Entity) iMissile);
  }

  public void SpawnMissile(
    ref MissileEntity iMissile,
    Vector3 iVelocity,
    Item iItem,
    PlayState iPlayState,
    Entity iOwner)
  {
    if (iMissile == null & NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (iMissile == null)
      iMissile = MissileEntity.GetInstance(iPlayState);
    Vector3 iVelocity1 = iVelocity;
    Vector3 position = iOwner.Position;
    if (iItem.ProjectileModel != null)
      iMissile.Initialize(iOwner, iItem.ProjectileModel.Meshes[0].BoundingSphere.Radius, ref position, ref iVelocity1, iItem.ProjectileModel, iItem.RangedConditions, false);
    else
      iMissile.Initialize(iOwner, 0.75f, ref position, ref iVelocity1, iItem.ProjectileModel, iItem.RangedConditions, false);
    iMissile.Danger = iItem.Danger;
    iMissile.Homing = iItem.Homing;
    iMissile.FacingVelocity = iItem.Facing;
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref new SpawnMissileMessage()
    {
      Type = SpawnMissileMessage.MissileType.Item,
      Item = iItem.Handle,
      Owner = iOwner.Handle,
      Handle = iMissile.Handle,
      Position = iMissile.Position,
      Velocity = iMissile.Body.Velocity
    });
  }
}
