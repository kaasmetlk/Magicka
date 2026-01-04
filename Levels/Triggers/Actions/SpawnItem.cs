// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SpawnItem
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.Network;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class SpawnItem(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mArea;
  private int mAreaHash;
  private string mItem;
  private int mItemHash;
  private string mOnPickupTrigger;
  private int mOnPickUp;
  private int mNr = 1;
  private bool mPhysicsEnabled;
  private bool mIgnoreTractorPull;
  private int mAttachToAnimatedLevelPartID;
  private string mAttachToAnimatedLevelPart;
  private Magicka.GameLogic.Entities.Items.Item mPlaceholder;
  private string mIDString;
  private int mIDHash;

  public override void Initialize()
  {
    base.Initialize();
    this.mPlaceholder = new Magicka.GameLogic.Entities.Items.Item(this.GameScene.PlayState, (Character) null);
    this.GameScene.PlayState.Content.Load<Magicka.GameLogic.Entities.Items.Item>("Data/Items/Wizard/" + this.mItem);
  }

  protected override void Execute()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Matrix oLocator;
    this.GameScene.GetLocator(this.mAreaHash, out oLocator);
    Magicka.GameLogic.Entities.Items.Item.GetCachedWeapon(this.mItemHash, this.mPlaceholder);
    this.mPlaceholder.OnPickup = this.mOnPickUp;
    this.mPlaceholder.Detach();
    Vector3 translation = oLocator.Translation;
    Matrix matrix = oLocator with
    {
      Translation = new Vector3()
    };
    this.mPlaceholder.Body.MoveTo(translation, matrix);
    this.GameScene.PlayState.EntityManager.AddEntity((Entity) this.mPlaceholder);
    this.mPlaceholder.Body.Immovable = !this.mPhysicsEnabled;
    this.mPlaceholder.IgnoreTractorPull = this.mIgnoreTractorPull;
    if (!string.IsNullOrEmpty(this.mIDString))
      this.mPlaceholder.SetUniqueID(this.mIDHash);
    this.mPlaceholder.Body.EnableBody();
    AnimatedLevelPart animatedLevelPart;
    if (this.mAttachToAnimatedLevelPartID != 0 && this.mScene.Level.CurrentScene.LevelModel.AnimatedLevelParts.TryGetValue(this.mAttachToAnimatedLevelPartID, out animatedLevelPart))
    {
      animatedLevelPart.AddEntity((Entity) this.mPlaceholder);
      this.mPlaceholder.AnimatedLevelPartID = this.mAttachToAnimatedLevelPartID;
    }
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    TriggerActionMessage iMessage = new TriggerActionMessage();
    iMessage.ActionType = TriggerActionType.SpawnItem;
    iMessage.Handle = this.mPlaceholder.Handle;
    iMessage.Template = this.mItemHash;
    iMessage.Position = this.mPlaceholder.Position;
    iMessage.Bool0 = this.mPhysicsEnabled;
    iMessage.Bool1 = this.mIgnoreTractorPull;
    iMessage.Point0 = this.mOnPickUp;
    iMessage.Point1 = this.mAttachToAnimatedLevelPartID;
    iMessage.Direction = new Vector3();
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

  public string Item
  {
    get => this.mItem;
    set
    {
      this.mItem = value;
      this.mItemHash = this.mItem.GetHashCodeCustom();
    }
  }

  public bool IgnoreTractorPull
  {
    get => this.mIgnoreTractorPull;
    set => this.mIgnoreTractorPull = value;
  }

  public string AttachTo
  {
    get => this.mAttachToAnimatedLevelPart;
    set
    {
      this.mAttachToAnimatedLevelPart = value;
      this.mAttachToAnimatedLevelPartID = this.mAttachToAnimatedLevelPart.GetHashCodeCustom();
    }
  }

  public int Nr
  {
    get => this.mNr;
    set => this.mNr = value;
  }

  public bool PhysicsEnabled
  {
    get => this.mPhysicsEnabled;
    set => this.mPhysicsEnabled = value;
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
    get => this.mIDString;
    set
    {
      this.mIDString = value;
      this.mIDHash = this.mIDString.GetHashCodeCustom();
    }
  }
}
