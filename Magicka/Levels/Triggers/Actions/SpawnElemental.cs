using System;
using System.Collections.Generic;
using Magicka.GameLogic.Entities;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020002C9 RID: 713
	internal class SpawnElemental : Action
	{
		// Token: 0x060015AA RID: 5546 RVA: 0x0008A95C File Offset: 0x00088B5C
		public SpawnElemental(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x0008A973 File Offset: 0x00088B73
		public override void Initialize()
		{
			base.Initialize();
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x0008A97C File Offset: 0x00088B7C
		protected override void Execute()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			for (int i = 0; i < this.mAmount; i++)
			{
				ElementalEgg instance = ElementalEgg.GetInstance(base.GameScene.PlayState);
				instance.Proximity = this.mProximity;
				Matrix matrix;
				base.GameScene.GetLocator(this.mAreaID, out matrix);
				if (this.mSnapToNavMesh)
				{
					Vector3 translation = matrix.Translation;
					Vector3 translation2;
					base.GameScene.LevelModel.NavMesh.GetNearestPosition(ref translation, out translation2, MovementProperties.Default);
					matrix.Translation = translation2;
				}
				Vector3 translation3 = matrix.Translation;
				Vector3 forward = matrix.Forward;
				instance.Initialize(ref translation3, ref forward, this.mUniqueID);
				base.GameScene.PlayState.EntityManager.AddEntity(instance);
				this.mSpawnedEntities.Add(instance.Handle);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnElemental;
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Time = this.mProximity;
					triggerActionMessage.Id = this.mUniqueID;
					triggerActionMessage.Position = translation3;
					triggerActionMessage.Direction = forward;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
		}

		// Token: 0x060015AD RID: 5549 RVA: 0x0008AAC4 File Offset: 0x00088CC4
		public override void QuickExecute()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			for (int i = 0; i < this.mSpawnedEntities.Count; i++)
			{
				ElementalEgg instance = ElementalEgg.GetInstance(base.GameScene.PlayState);
				Matrix matrix;
				base.GameScene.GetLocator(this.mAreaID, out matrix);
				if (this.mSnapToNavMesh)
				{
					Vector3 translation = matrix.Translation;
					Vector3 translation2;
					base.GameScene.LevelModel.NavMesh.GetNearestPosition(ref translation, out translation2, MovementProperties.Default);
					matrix.Translation = translation2;
				}
				Vector3 translation3 = matrix.Translation;
				Vector3 forward = matrix.Forward;
				instance.Initialize(ref translation3, ref forward, this.mUniqueID);
				base.GameScene.PlayState.EntityManager.AddEntity(instance);
				this.mSpawnedEntities[i] = instance.Handle;
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnElemental;
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Id = this.mUniqueID;
					triggerActionMessage.Position = translation3;
					triggerActionMessage.Direction = forward;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
		}

		// Token: 0x060015AE RID: 5550 RVA: 0x0008ABF8 File Offset: 0x00088DF8
		public override void Update(float iDeltaTime)
		{
			for (int i = 0; i < this.mSpawnedEntities.Count; i++)
			{
				ElementalEgg elementalEgg = Entity.GetFromHandle((int)this.mSpawnedEntities[i]) as ElementalEgg;
				if (elementalEgg.Dead)
				{
					this.mSpawnedEntities.RemoveAt(i--);
				}
			}
			base.Update(iDeltaTime);
		}

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x060015AF RID: 5551 RVA: 0x0008AC51 File Offset: 0x00088E51
		// (set) Token: 0x060015B0 RID: 5552 RVA: 0x0008AC59 File Offset: 0x00088E59
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

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x060015B1 RID: 5553 RVA: 0x0008AC73 File Offset: 0x00088E73
		// (set) Token: 0x060015B2 RID: 5554 RVA: 0x0008AC7B File Offset: 0x00088E7B
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaID = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x060015B3 RID: 5555 RVA: 0x0008AC95 File Offset: 0x00088E95
		// (set) Token: 0x060015B4 RID: 5556 RVA: 0x0008AC9D File Offset: 0x00088E9D
		public bool SnapToNavMesh
		{
			get
			{
				return this.mSnapToNavMesh;
			}
			set
			{
				this.mSnapToNavMesh = value;
			}
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x060015B5 RID: 5557 RVA: 0x0008ACA6 File Offset: 0x00088EA6
		// (set) Token: 0x060015B6 RID: 5558 RVA: 0x0008ACAE File Offset: 0x00088EAE
		public int Nr
		{
			get
			{
				return this.mAmount;
			}
			set
			{
				this.mAmount = value;
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x060015B7 RID: 5559 RVA: 0x0008ACB7 File Offset: 0x00088EB7
		// (set) Token: 0x060015B8 RID: 5560 RVA: 0x0008ACBF File Offset: 0x00088EBF
		public float Proximity
		{
			get
			{
				return this.mProximity;
			}
			set
			{
				this.mProximity = value;
			}
		}

		// Token: 0x04001702 RID: 5890
		private List<ushort> mSpawnedEntities = new List<ushort>(10);

		// Token: 0x04001703 RID: 5891
		private bool mSnapToNavMesh;

		// Token: 0x04001704 RID: 5892
		private string mArea;

		// Token: 0x04001705 RID: 5893
		private int mAreaID;

		// Token: 0x04001706 RID: 5894
		private int mAmount;

		// Token: 0x04001707 RID: 5895
		private string mUniqueName;

		// Token: 0x04001708 RID: 5896
		private int mUniqueID;

		// Token: 0x04001709 RID: 5897
		private float mProximity;
	}
}
