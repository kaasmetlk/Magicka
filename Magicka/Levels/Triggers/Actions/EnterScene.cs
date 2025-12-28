using System;
using Magicka.AI;
using Magicka.GameLogic;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000395 RID: 917
	public class EnterScene : Action
	{
		// Token: 0x06001C13 RID: 7187 RVA: 0x000BF92A File Offset: 0x000BDB2A
		public EnterScene(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001C14 RID: 7188 RVA: 0x000BF954 File Offset: 0x000BDB54
		protected unsafe override void Execute()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			this.mSpawnPoint.SpawnPlayers = true;
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.EnterScene;
				SpawnPoint spawnPoint = this.mSpawnPoint;
				triggerActionMessage.Scene = spawnPoint.Scene;
				triggerActionMessage.Point0 = *(&spawnPoint.Locations.FixedElementField);
				triggerActionMessage.Point1 = (&spawnPoint.Locations.FixedElementField)[1];
				triggerActionMessage.Point2 = (&spawnPoint.Locations.FixedElementField)[2];
				triggerActionMessage.Point3 = (&spawnPoint.Locations.FixedElementField)[3];
				triggerActionMessage.Target0 = this.mMoveTargetIDs[0];
				triggerActionMessage.Target1 = this.mMoveTargetIDs[1];
				triggerActionMessage.Target2 = this.mMoveTargetIDs[2];
				triggerActionMessage.Target3 = this.mMoveTargetIDs[3];
				triggerActionMessage.Bool0 = this.mIgnoreTriggers;
				triggerActionMessage.Bool1 = this.mSaveNPCs;
				triggerActionMessage.Bool2 = this.mFacingDirection;
				triggerActionMessage.Arg = this.mMoveTriggerID;
				triggerActionMessage.Template = (int)this.mTransition;
				triggerActionMessage.Time = this.mTransitionTime;
				(NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
			base.GameScene.Level.GoToScene(this.mSpawnPoint, this.mTransition, this.mTransitionTime, this.mSaveNPCs, new Action(this.OnSceneChanged));
		}

		// Token: 0x06001C15 RID: 7189 RVA: 0x000BFAF4 File Offset: 0x000BDCF4
		private void OnSceneChanged()
		{
			if (!string.IsNullOrEmpty(this.mMoveTargetName))
			{
				GameScene currentScene = base.GameScene.Level.CurrentScene;
				AIEvent aievent = default(AIEvent);
				aievent.EventType = AIEventType.Move;
				aievent.MoveEvent.Delay = 0f;
				aievent.MoveEvent.Speed = 1f;
				aievent.MoveEvent.FixedDirection = this.mFacingDirection;
				aievent.MoveEvent.Trigger = this.mMoveTriggerID;
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					Player player = players[i];
					Matrix matrix;
					if (player.Playing && player.Avatar != null && currentScene.TryGetLocator(this.mMoveTargetIDs[i], out matrix))
					{
						aievent.MoveEvent.Waypoint = matrix.Translation;
						aievent.MoveEvent.Direction = matrix.Forward;
						player.Avatar.Events = new AIEvent[]
						{
							aievent
						};
						player.Avatar.IgnoreTriggers = this.mIgnoreTriggers;
					}
				}
			}
			if (!string.IsNullOrEmpty(this.mInstantTrigger))
			{
				base.GameScene.Level.CurrentScene.ExecuteTrigger(this.mInstantTriggerID, null, false);
			}
		}

		// Token: 0x06001C16 RID: 7190 RVA: 0x000BFC46 File Offset: 0x000BDE46
		public override void QuickExecute()
		{
		}

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x06001C17 RID: 7191 RVA: 0x000BFC48 File Offset: 0x000BDE48
		// (set) Token: 0x06001C18 RID: 7192 RVA: 0x000BFC50 File Offset: 0x000BDE50
		public string Scene
		{
			get
			{
				return this.mScene;
			}
			set
			{
				this.mScene = value;
				this.mSpawnPoint.Scene = this.mScene.GetHashCodeCustom();
			}
		}

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x06001C19 RID: 7193 RVA: 0x000BFC6F File Offset: 0x000BDE6F
		// (set) Token: 0x06001C1A RID: 7194 RVA: 0x000BFC77 File Offset: 0x000BDE77
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

		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x06001C1B RID: 7195 RVA: 0x000BFC80 File Offset: 0x000BDE80
		// (set) Token: 0x06001C1C RID: 7196 RVA: 0x000BFC88 File Offset: 0x000BDE88
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

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x06001C1D RID: 7197 RVA: 0x000BFC91 File Offset: 0x000BDE91
		// (set) Token: 0x06001C1E RID: 7198 RVA: 0x000BFC9C File Offset: 0x000BDE9C
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

		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x06001C1F RID: 7199 RVA: 0x000BFCEE File Offset: 0x000BDEEE
		// (set) Token: 0x06001C20 RID: 7200 RVA: 0x000BFCF8 File Offset: 0x000BDEF8
		public string MoveTarget
		{
			get
			{
				return this.mMoveTargetName;
			}
			set
			{
				this.mMoveTargetName = value;
				for (int i = 0; i < this.mMoveTargetIDs.Length; i++)
				{
					this.mMoveTargetIDs[i] = (value + i).GetHashCodeCustom();
				}
			}
		}

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x06001C21 RID: 7201 RVA: 0x000BFD38 File Offset: 0x000BDF38
		// (set) Token: 0x06001C22 RID: 7202 RVA: 0x000BFD40 File Offset: 0x000BDF40
		public bool FacingDirection
		{
			get
			{
				return this.mFacingDirection;
			}
			set
			{
				this.mFacingDirection = value;
			}
		}

		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x06001C23 RID: 7203 RVA: 0x000BFD49 File Offset: 0x000BDF49
		// (set) Token: 0x06001C24 RID: 7204 RVA: 0x000BFD51 File Offset: 0x000BDF51
		public string InstantTrigger
		{
			get
			{
				return this.mInstantTrigger;
			}
			set
			{
				this.mInstantTrigger = value;
				this.mInstantTriggerID = value.GetHashCodeCustom();
			}
		}

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x06001C25 RID: 7205 RVA: 0x000BFD66 File Offset: 0x000BDF66
		// (set) Token: 0x06001C26 RID: 7206 RVA: 0x000BFD6E File Offset: 0x000BDF6E
		public new string Trigger
		{
			get
			{
				return this.mMoveTrigger;
			}
			set
			{
				this.mMoveTrigger = value;
				this.mMoveTriggerID = value.GetHashCodeCustom();
			}
		}

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x06001C27 RID: 7207 RVA: 0x000BFD83 File Offset: 0x000BDF83
		// (set) Token: 0x06001C28 RID: 7208 RVA: 0x000BFD8B File Offset: 0x000BDF8B
		public bool IgnoreTriggers
		{
			get
			{
				return this.mIgnoreTriggers;
			}
			set
			{
				this.mIgnoreTriggers = value;
			}
		}

		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x06001C29 RID: 7209 RVA: 0x000BFD94 File Offset: 0x000BDF94
		// (set) Token: 0x06001C2A RID: 7210 RVA: 0x000BFD9C File Offset: 0x000BDF9C
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

		// Token: 0x04001E49 RID: 7753
		private new string mScene;

		// Token: 0x04001E4A RID: 7754
		private Transitions mTransition = Transitions.Fade;

		// Token: 0x04001E4B RID: 7755
		private float mTransitionTime = 1f;

		// Token: 0x04001E4C RID: 7756
		private string mSpawnPointName;

		// Token: 0x04001E4D RID: 7757
		private SpawnPoint mSpawnPoint;

		// Token: 0x04001E4E RID: 7758
		private string mMoveTargetName;

		// Token: 0x04001E4F RID: 7759
		private int[] mMoveTargetIDs = new int[4];

		// Token: 0x04001E50 RID: 7760
		private bool mFacingDirection;

		// Token: 0x04001E51 RID: 7761
		private string mInstantTrigger;

		// Token: 0x04001E52 RID: 7762
		private int mInstantTriggerID;

		// Token: 0x04001E53 RID: 7763
		private string mMoveTrigger;

		// Token: 0x04001E54 RID: 7764
		private int mMoveTriggerID;

		// Token: 0x04001E55 RID: 7765
		private bool mIgnoreTriggers;

		// Token: 0x04001E56 RID: 7766
		private bool mSaveNPCs;
	}
}
