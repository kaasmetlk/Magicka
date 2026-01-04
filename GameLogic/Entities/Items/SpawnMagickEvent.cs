// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.SpawnMagickEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct SpawnMagickEvent
{
  public MagickType Type;
  public float DespawnTime;

  public SpawnMagickEvent(MagickType iType)
  {
    this.Type = iType;
    this.DespawnTime = 0.0f;
  }

  public SpawnMagickEvent(MagickType iType, float iDespawnTime)
  {
    this.Type = iType;
    this.DespawnTime = iDespawnTime;
  }

  public SpawnMagickEvent(ContentReader iInput)
  {
    this.DespawnTime = 0.0f;
    string lowerInvariant = iInput.ReadString().ToLowerInvariant();
    if (lowerInvariant.Equals("random", StringComparison.InvariantCultureIgnoreCase))
      this.Type = MagickType.Revive;
    else
      this.Type = (MagickType) Enum.Parse(typeof (MagickType), lowerInvariant, true);
  }

  public void Execute(Entity iItem, Entity iTarget)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    BookOfMagick instance = BookOfMagick.GetInstance(iItem.PlayState);
    Matrix orientation = iItem.Body.Orientation;
    Vector3 position = iItem.Position;
    position.Y += iItem.Radius;
    Vector3 iVelocity = new Vector3((float) (MagickaMath.Random.NextDouble() - 0.5) * 3f, (float) MagickaMath.Random.NextDouble() * 7f, (float) (MagickaMath.Random.NextDouble() - 0.5) * 3f);
    instance.Initialize(position, orientation, this.Type, false, iVelocity, this.DespawnTime, 0);
    iItem.PlayState.EntityManager.AddEntity((Entity) instance);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    TriggerActionMessage iMessage = new TriggerActionMessage();
    iMessage.ActionType = TriggerActionType.SpawnMagick;
    iMessage.Handle = instance.Handle;
    iMessage.Template = (int) this.Type;
    iMessage.Position = instance.Position;
    iMessage.Direction = iVelocity;
    iMessage.Time = this.DespawnTime;
    iMessage.Point0 = 0;
    iMessage.Bool0 = false;
    Quaternion.CreateFromRotationMatrix(ref orientation, out iMessage.Orientation);
    NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
  }
}
