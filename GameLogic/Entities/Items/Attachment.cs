// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.Attachment
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public class Attachment
{
  private int mAttachIndex;
  private Matrix mBindBose;
  private Item mItem;

  public Attachment(PlayState iPlayState, Character iOwner)
  {
    this.mAttachIndex = -1;
    this.mBindBose = new Matrix();
    this.mItem = new Item(iPlayState, iOwner);
  }

  public Attachment(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
  {
    string str = iInput.ReadString();
    SkinnedModelBone skinnedModelBone = (SkinnedModelBone) null;
    for (int index = 0; index < iSkeleton.Count; ++index)
    {
      if (iSkeleton[index].Name.Equals(str, StringComparison.OrdinalIgnoreCase))
      {
        skinnedModelBone = iSkeleton[index];
        this.mAttachIndex = index;
        break;
      }
    }
    this.mBindBose = skinnedModelBone != null ? skinnedModelBone.InverseBindPoseTransform : throw new Exception("Invalid attach point: " + str);
    Matrix.Invert(ref this.mBindBose, out this.mBindBose);
    Vector3 vector3 = iInput.ReadVector3();
    Matrix result1;
    Matrix.CreateRotationX(vector3.X, out result1);
    Matrix result2;
    Matrix.CreateRotationY(vector3.Y + 3.14159274f, out result2);
    Matrix result3;
    Matrix.CreateRotationX(vector3.Z, out result3);
    Matrix result4;
    Matrix.Multiply(ref result1, ref result2, out result4);
    Matrix.Multiply(ref result4, ref result3, out result4);
    Matrix.Multiply(ref result4, ref this.mBindBose, out this.mBindBose);
    this.mItem = iInput.ReadExternalReference<Item>();
  }

  public void TransformBindPose(ref Matrix iTransform)
  {
    Matrix.Multiply(ref iTransform, ref this.mBindBose, out this.mBindBose);
  }

  public int AttachIndex
  {
    get => this.mAttachIndex;
    set => this.mAttachIndex = value;
  }

  public Matrix BindBose
  {
    get => this.mBindBose;
    set => this.mBindBose = value;
  }

  public Item Item
  {
    get => this.mItem;
    set => this.mItem = value;
  }

  public void CopyToInstance(Attachment iTarget)
  {
    iTarget.mAttachIndex = this.mAttachIndex;
    iTarget.mBindBose = this.mBindBose;
    this.mItem.Copy(iTarget.mItem);
  }

  internal void Set(Item iItem, SkinnedModelBone iBone, Vector3? iRotation)
  {
    this.mAttachIndex = (int) iBone.Index;
    this.mBindBose = iBone.InverseBindPoseTransform;
    Matrix.Invert(ref this.mBindBose, out this.mBindBose);
    Matrix result1;
    if (iRotation.HasValue)
    {
      Matrix result2;
      Matrix.CreateRotationX(iRotation.Value.X, out result2);
      Matrix result3;
      Matrix.CreateRotationY(iRotation.Value.Y + 3.14159274f, out result3);
      Matrix result4;
      Matrix.CreateRotationX(iRotation.Value.Z, out result4);
      Matrix.Multiply(ref result2, ref result3, out result1);
      Matrix.Multiply(ref result1, ref result4, out result1);
    }
    else
      Matrix.CreateRotationY(3.14159274f, out result1);
    Matrix.Multiply(ref result1, ref this.mBindBose, out this.mBindBose);
    this.mItem.Deinitialize();
    iItem.Copy(this.mItem);
  }

  public void Release(PlayState iPlayState)
  {
    if (!this.mItem.Attached || this.mItem.Model == null)
      return;
    Item obj = (Item) null;
    Quaternion rotation;
    Vector3 translation;
    this.Item.Transform.Decompose(out Vector3 _, out rotation, out translation);
    Matrix result;
    Matrix.CreateFromQuaternion(ref rotation, out result);
    result.Translation = translation;
    this.mItem.Transform = result;
    if (this.mItem.IsPickable)
    {
      if (NetworkManager.Instance.State != NetworkState.Client)
      {
        obj = Item.GetPickableIntstance();
        this.mItem.Copy(obj);
        obj.Transform = this.mItem.Transform;
      }
      this.mItem.Detach();
    }
    else
      obj = this.mItem;
    if (obj == null)
      return;
    obj.Detach();
    if (obj.Dead)
      return;
    iPlayState.EntityManager.AddEntity((Entity) obj);
    obj.Body.EnableBody();
    obj.Body.SetActive();
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    TriggerActionMessage iMessage = new TriggerActionMessage();
    iMessage.ActionType = TriggerActionType.SpawnItem;
    iMessage.Handle = obj.Handle;
    iMessage.Template = obj.Type;
    iMessage.Position = obj.Position;
    iMessage.Direction = obj.Body.Velocity;
    iMessage.Bool0 = true;
    iMessage.Point0 = 0;
    Matrix orientation = obj.Body.Orientation;
    Quaternion.CreateFromRotationMatrix(ref orientation, out iMessage.Orientation);
    NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
  }

  public void ReAttach(Character iOwner) => this.mItem.Deinitialize();
}
