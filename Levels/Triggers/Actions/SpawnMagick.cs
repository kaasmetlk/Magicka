// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SpawnMagick
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class SpawnMagick(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mArea;
  private int mAreaHash;
  private MagickType mMagick;
  private string mOnPickupTrigger;
  private int mOnPickUp;
  private string mUniqueName;
  private int mUniqueID;
  private bool mSpawnImmovable = true;
  private float mTimeOut;

  public override void Initialize() => base.Initialize();

  protected override void Execute()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Matrix oLocator;
    this.GameScene.GetLocator(this.mAreaHash, out oLocator);
    Vector3 translation = oLocator.Translation;
    Matrix matrix = oLocator with
    {
      Translation = new Vector3()
    };
    BookOfMagick instance = BookOfMagick.GetInstance(this.GameScene.PlayState);
    instance.Initialize(translation, matrix, this.mMagick, this.mSpawnImmovable, new Vector3(), this.mTimeOut, this.mUniqueID);
    instance.OnPickup = this.mOnPickUp;
    this.GameScene.PlayState.EntityManager.AddEntity((Entity) instance);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    TriggerActionMessage iMessage = new TriggerActionMessage();
    iMessage.ActionType = TriggerActionType.SpawnMagick;
    iMessage.Handle = instance.Handle;
    iMessage.Template = (int) this.mMagick;
    iMessage.Position = instance.Position;
    iMessage.Direction = new Vector3();
    iMessage.Time = this.mTimeOut;
    iMessage.Point0 = this.mUniqueID;
    iMessage.Bool0 = this.mSpawnImmovable;
    Quaternion.CreateFromRotationMatrix(ref matrix, out iMessage.Orientation);
    NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
  }

  public override void QuickExecute()
  {
  }

  public string Area
  {
    get => this.mArea;
    set
    {
      this.mArea = value;
      this.mAreaHash = this.mArea.GetHashCodeCustom();
    }
  }

  public bool Immovable
  {
    get => this.mSpawnImmovable;
    set => this.mSpawnImmovable = value;
  }

  public MagickType Magick
  {
    get => this.mMagick;
    set => this.mMagick = value;
  }

  public string OnPickup
  {
    get => this.mOnPickupTrigger;
    set
    {
      this.mOnPickupTrigger = value;
      this.mOnPickUp = this.mOnPickupTrigger.GetHashCodeCustom();
    }
  }

  public string ID
  {
    get => this.mUniqueName;
    set
    {
      this.mUniqueName = value;
      this.mUniqueID = this.mUniqueName.GetHashCodeCustom();
    }
  }

  public float TimeOut
  {
    get => this.mTimeOut;
    set => this.mTimeOut = value;
  }
}
