using System;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020002C5 RID: 709
	public class SpawnMagick : Action
	{
		// Token: 0x06001591 RID: 5521 RVA: 0x0008A6AC File Offset: 0x000888AC
		public SpawnMagick(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x0008A6BD File Offset: 0x000888BD
		public override void Initialize()
		{
			base.Initialize();
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x0008A6C8 File Offset: 0x000888C8
		protected override void Execute()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			Matrix matrix;
			base.GameScene.GetLocator(this.mAreaHash, out matrix);
			Vector3 translation = matrix.Translation;
			Matrix iOrientation = matrix;
			iOrientation.Translation = default(Vector3);
			BookOfMagick instance = BookOfMagick.GetInstance(base.GameScene.PlayState);
			instance.Initialize(translation, iOrientation, this.mMagick, this.mSpawnImmovable, default(Vector3), this.mTimeOut, this.mUniqueID);
			instance.OnPickup = this.mOnPickUp;
			base.GameScene.PlayState.EntityManager.AddEntity(instance);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.SpawnMagick;
				triggerActionMessage.Handle = instance.Handle;
				triggerActionMessage.Template = (int)this.mMagick;
				triggerActionMessage.Position = instance.Position;
				triggerActionMessage.Direction = default(Vector3);
				triggerActionMessage.Time = this.mTimeOut;
				triggerActionMessage.Point0 = this.mUniqueID;
				triggerActionMessage.Bool0 = this.mSpawnImmovable;
				Quaternion.CreateFromRotationMatrix(ref iOrientation, out triggerActionMessage.Orientation);
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x0008A808 File Offset: 0x00088A08
		public override void QuickExecute()
		{
		}

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06001595 RID: 5525 RVA: 0x0008A80A File Offset: 0x00088A0A
		// (set) Token: 0x06001596 RID: 5526 RVA: 0x0008A812 File Offset: 0x00088A12
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaHash = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06001597 RID: 5527 RVA: 0x0008A82C File Offset: 0x00088A2C
		// (set) Token: 0x06001598 RID: 5528 RVA: 0x0008A834 File Offset: 0x00088A34
		public bool Immovable
		{
			get
			{
				return this.mSpawnImmovable;
			}
			set
			{
				this.mSpawnImmovable = value;
			}
		}

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06001599 RID: 5529 RVA: 0x0008A83D File Offset: 0x00088A3D
		// (set) Token: 0x0600159A RID: 5530 RVA: 0x0008A845 File Offset: 0x00088A45
		public MagickType Magick
		{
			get
			{
				return this.mMagick;
			}
			set
			{
				this.mMagick = value;
			}
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x0600159B RID: 5531 RVA: 0x0008A84E File Offset: 0x00088A4E
		// (set) Token: 0x0600159C RID: 5532 RVA: 0x0008A856 File Offset: 0x00088A56
		public string OnPickup
		{
			get
			{
				return this.mOnPickupTrigger;
			}
			set
			{
				this.mOnPickupTrigger = value;
				this.mOnPickUp = this.mOnPickupTrigger.GetHashCodeCustom();
			}
		}

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x0600159D RID: 5533 RVA: 0x0008A870 File Offset: 0x00088A70
		// (set) Token: 0x0600159E RID: 5534 RVA: 0x0008A878 File Offset: 0x00088A78
		public string ID
		{
			get
			{
				return this.mUniqueName;
			}
			set
			{
				this.mUniqueName = value;
				this.mUniqueID = this.mUniqueName.GetHashCodeCustom();
			}
		}

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x0600159F RID: 5535 RVA: 0x0008A892 File Offset: 0x00088A92
		// (set) Token: 0x060015A0 RID: 5536 RVA: 0x0008A89A File Offset: 0x00088A9A
		public float TimeOut
		{
			get
			{
				return this.mTimeOut;
			}
			set
			{
				this.mTimeOut = value;
			}
		}

		// Token: 0x040016F7 RID: 5879
		private string mArea;

		// Token: 0x040016F8 RID: 5880
		private int mAreaHash;

		// Token: 0x040016F9 RID: 5881
		private MagickType mMagick;

		// Token: 0x040016FA RID: 5882
		private string mOnPickupTrigger;

		// Token: 0x040016FB RID: 5883
		private int mOnPickUp;

		// Token: 0x040016FC RID: 5884
		private string mUniqueName;

		// Token: 0x040016FD RID: 5885
		private int mUniqueID;

		// Token: 0x040016FE RID: 5886
		private bool mSpawnImmovable = true;

		// Token: 0x040016FF RID: 5887
		private float mTimeOut;
	}
}
