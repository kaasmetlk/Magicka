using System;
using Magicka.Network;
using PolygonHead;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000016 RID: 22
	public class ChangeScene : Action
	{
		// Token: 0x0600009F RID: 159 RVA: 0x00006761 File Offset: 0x00004961
		public ChangeScene(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00006780 File Offset: 0x00004980
		protected unsafe override void Execute()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.ChangeScene;
				SpawnPoint spawnPoint = this.mSpawnPoint;
				triggerActionMessage.Scene = spawnPoint.Scene;
				triggerActionMessage.Point0 = *(&spawnPoint.Locations.FixedElementField);
				triggerActionMessage.Point1 = (&spawnPoint.Locations.FixedElementField)[1];
				triggerActionMessage.Point2 = (&spawnPoint.Locations.FixedElementField)[2];
				triggerActionMessage.Point3 = (&spawnPoint.Locations.FixedElementField)[3];
				triggerActionMessage.Bool0 = spawnPoint.SpawnPlayers;
				triggerActionMessage.Bool1 = this.mSaveNPCs;
				triggerActionMessage.Template = (int)this.mTransition;
				triggerActionMessage.Time = this.mTransitionTime;
				(NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
			base.GameScene.Level.GoToScene(this.mSpawnPoint, this.mTransition, this.mTransitionTime, this.mSaveNPCs, null);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000068B1 File Offset: 0x00004AB1
		public override void QuickExecute()
		{
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x000068B3 File Offset: 0x00004AB3
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x000068BB File Offset: 0x00004ABB
		public string Scene
		{
			get
			{
				return this.mSceneString;
			}
			set
			{
				this.mSceneString = value;
				this.mSpawnPoint.Scene = this.mSceneString.GetHashCodeCustom();
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x000068DA File Offset: 0x00004ADA
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x000068E2 File Offset: 0x00004AE2
		public Transitions Transition
		{
			get
			{
				return this.mTransition;
			}
			set
			{
				this.mTransition = value;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x000068EB File Offset: 0x00004AEB
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x000068F3 File Offset: 0x00004AF3
		public float TransitionTime
		{
			get
			{
				return this.mTransitionTime;
			}
			set
			{
				this.mTransitionTime = value;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x000068FC File Offset: 0x00004AFC
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x00006909 File Offset: 0x00004B09
		public bool SpawnPlayers
		{
			get
			{
				return this.mSpawnPoint.SpawnPlayers;
			}
			set
			{
				this.mSpawnPoint.SpawnPlayers = value;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00006917 File Offset: 0x00004B17
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00006920 File Offset: 0x00004B20
		public unsafe string SpawnPoint
		{
			get
			{
				return this.mSpawnPointName;
			}
			set
			{
				this.mSpawnPointName = value;
				fixed (int* ptr = &this.mSpawnPoint.Locations.FixedElementField)
				{
					for (int i = 0; i < 4; i++)
					{
						ptr[i] = (this.mSpawnPointName + i).GetHashCodeCustom();
					}
				}
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000AC RID: 172 RVA: 0x00006972 File Offset: 0x00004B72
		// (set) Token: 0x060000AD RID: 173 RVA: 0x0000697A File Offset: 0x00004B7A
		public bool SaveNPCs
		{
			get
			{
				return this.mSaveNPCs;
			}
			set
			{
				this.mSaveNPCs = value;
			}
		}

		// Token: 0x04000088 RID: 136
		private string mSceneString;

		// Token: 0x04000089 RID: 137
		private Transitions mTransition = Transitions.Fade;

		// Token: 0x0400008A RID: 138
		private float mTransitionTime = 1f;

		// Token: 0x0400008B RID: 139
		private string mSpawnPointName;

		// Token: 0x0400008C RID: 140
		private SpawnPoint mSpawnPoint;

		// Token: 0x0400008D RID: 141
		private bool mSaveNPCs;
	}
}
