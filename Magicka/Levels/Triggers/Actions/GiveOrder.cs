using System;
using System.Collections.Generic;
using System.Xml;
using Magicka.AI;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Network;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020000AA RID: 170
	public class GiveOrder : Action
	{
		// Token: 0x060004D4 RID: 1236 RVA: 0x0001BDF4 File Offset: 0x00019FF4
		public GiveOrder(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
			if (GiveOrder.sPlayState != iScene.PlayState)
			{
				GiveOrder.sPlayState = iScene.PlayState;
				GiveOrder.sInstances.Clear();
			}
			this.mHandle = (ushort)GiveOrder.sInstances.Count;
			GiveOrder.sInstances.Add(this);
			this.mNode = iNode;
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0001BE68 File Offset: 0x0001A068
		public override void Initialize()
		{
			base.Initialize();
			LevelModel levelModel = base.GameScene.LevelModel;
			List<AIEvent> list = new List<AIEvent>();
			foreach (object obj in this.mNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlComment))
				{
					list.Add(new AIEvent(levelModel, xmlNode));
				}
			}
			this.mEvents = list.ToArray();
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x0001BEFC File Offset: 0x0001A0FC
		protected override void Execute()
		{
			this.Exec();
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x0001BF04 File Offset: 0x0001A104
		public static void ExecuteByHandle(ushort iHandle)
		{
			GiveOrder.sInstances[(int)iHandle].Exec();
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x0001BF18 File Offset: 0x0001A118
		private void Exec()
		{
			if (this.mIsSpecific)
			{
				NonPlayerCharacter nonPlayerCharacter = Entity.GetByID(this.mIDHash) as NonPlayerCharacter;
				if (nonPlayerCharacter != null)
				{
					nonPlayerCharacter.ReleaseAttachedCharacter();
					if (nonPlayerCharacter.IsGripped)
					{
						nonPlayerCharacter.Gripper.ReleaseAttachedCharacter();
					}
					nonPlayerCharacter.AI.WanderSpeed = this.mWanderSpeed;
					nonPlayerCharacter.AI.TargetArea = this.mTargetAreaHash;
					nonPlayerCharacter.AI.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
					if (this.mIdleAnimation != Animations.None)
					{
						nonPlayerCharacter.SpecialIdleAnimation = this.mIdleAnimation;
					}
					if (this.mAnimation != Animations.None)
					{
						nonPlayerCharacter.GoToAnimation(this.mAnimation, 0.2f);
					}
					if (this.mOrder == Order.Panic)
					{
						nonPlayerCharacter.OrderPanic();
					}
					if ((this.mAnimation != Animations.None | this.mIdleAnimation != Animations.None) && NetworkManager.Instance.State != NetworkState.Offline)
					{
						CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
						characterActionMessage.Handle = nonPlayerCharacter.Handle;
						characterActionMessage.Action = ActionType.EventAnimation;
						characterActionMessage.Param0I = (int)this.mAnimation;
						characterActionMessage.Param1F = 0.2f;
						characterActionMessage.Param2I = (int)this.mIdleAnimation;
						NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
						return;
					}
				}
			}
			else
			{
				TriggerArea triggerArea = base.GameScene.GetTriggerArea(this.mAreaHash);
				for (int i = 0; i < triggerArea.PresentCharacters.Count; i++)
				{
					NonPlayerCharacter nonPlayerCharacter2 = triggerArea.PresentCharacters[i] as NonPlayerCharacter;
					if (nonPlayerCharacter2 != null && (this.mTypeHash == GiveOrder.ANYID || nonPlayerCharacter2.Type == this.mTypeHash || (!this.mExplicitFaction && (nonPlayerCharacter2.GetOriginalFaction & this.mFactions) != Factions.NONE) || (this.mExplicitFaction && nonPlayerCharacter2.GetOriginalFaction == this.mFactions)))
					{
						nonPlayerCharacter2.ReleaseAttachedCharacter();
						if (nonPlayerCharacter2.IsGripped)
						{
							nonPlayerCharacter2.Gripper.ReleaseAttachedCharacter();
						}
						nonPlayerCharacter2.AI.WanderSpeed = this.mWanderSpeed;
						nonPlayerCharacter2.AI.TargetArea = this.mTargetAreaHash;
						nonPlayerCharacter2.AI.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
						if (this.mIdleAnimation != Animations.None)
						{
							nonPlayerCharacter2.SpecialIdleAnimation = this.mIdleAnimation;
						}
						if (this.mAnimation != Animations.None)
						{
							nonPlayerCharacter2.GoToAnimation(this.mAnimation, 0.2f);
						}
						if ((this.mAnimation != Animations.None | this.mIdleAnimation != Animations.None) && NetworkManager.Instance.State != NetworkState.Offline)
						{
							CharacterActionMessage characterActionMessage2 = default(CharacterActionMessage);
							characterActionMessage2.Handle = nonPlayerCharacter2.Handle;
							characterActionMessage2.Action = ActionType.EventAnimation;
							characterActionMessage2.Param0I = (int)this.mAnimation;
							characterActionMessage2.Param1F = 0.2f;
							characterActionMessage2.Param2I = (int)this.mIdleAnimation;
							NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage2);
						}
					}
				}
			}
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x0001C234 File Offset: 0x0001A434
		public override void QuickExecute()
		{
			if (this.mAreaHash != 0)
			{
				TriggerArea triggerArea = base.GameScene.GetTriggerArea(this.mAreaHash);
				triggerArea.UpdatePresent(base.GameScene.PlayState.EntityManager);
			}
			this.Execute();
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060004DA RID: 1242 RVA: 0x0001C277 File Offset: 0x0001A477
		// (set) Token: 0x060004DB RID: 1243 RVA: 0x0001C27F File Offset: 0x0001A47F
		public Order Order
		{
			get
			{
				return this.mOrder;
			}
			set
			{
				this.mOrder = value;
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x060004DC RID: 1244 RVA: 0x0001C288 File Offset: 0x0001A488
		// (set) Token: 0x060004DD RID: 1245 RVA: 0x0001C290 File Offset: 0x0001A490
		public ReactTo ReactTo
		{
			get
			{
				return this.mReactTo;
			}
			set
			{
				this.mReactTo = value;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x060004DE RID: 1246 RVA: 0x0001C299 File Offset: 0x0001A499
		// (set) Token: 0x060004DF RID: 1247 RVA: 0x0001C2A1 File Offset: 0x0001A4A1
		public new string Trigger
		{
			get
			{
				return this.mTrigger;
			}
			set
			{
				this.mTrigger = value;
				this.mTriggerID = this.mTrigger.GetHashCodeCustom();
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060004E0 RID: 1248 RVA: 0x0001C2BB File Offset: 0x0001A4BB
		// (set) Token: 0x060004E1 RID: 1249 RVA: 0x0001C2C3 File Offset: 0x0001A4C3
		public Order Reaction
		{
			get
			{
				return this.mReaction;
			}
			set
			{
				this.mReaction = value;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060004E2 RID: 1250 RVA: 0x0001C2CC File Offset: 0x0001A4CC
		// (set) Token: 0x060004E3 RID: 1251 RVA: 0x0001C2D4 File Offset: 0x0001A4D4
		public Animations Animation
		{
			get
			{
				return this.mAnimation;
			}
			set
			{
				this.mAnimation = value;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060004E4 RID: 1252 RVA: 0x0001C2DD File Offset: 0x0001A4DD
		// (set) Token: 0x060004E5 RID: 1253 RVA: 0x0001C2E5 File Offset: 0x0001A4E5
		public Animations IdleAnimation
		{
			get
			{
				return this.mIdleAnimation;
			}
			set
			{
				this.mIdleAnimation = value;
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060004E6 RID: 1254 RVA: 0x0001C2EE File Offset: 0x0001A4EE
		// (set) Token: 0x060004E7 RID: 1255 RVA: 0x0001C2F6 File Offset: 0x0001A4F6
		public Factions Factions
		{
			get
			{
				return this.mFactions;
			}
			set
			{
				this.mFactions = value;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060004E8 RID: 1256 RVA: 0x0001C2FF File Offset: 0x0001A4FF
		// (set) Token: 0x060004E9 RID: 1257 RVA: 0x0001C307 File Offset: 0x0001A507
		public string ID
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
				if (!string.IsNullOrEmpty(this.mID))
				{
					this.mIsSpecific = true;
					this.mIDHash = this.mID.GetHashCodeCustom();
					return;
				}
				this.mIsSpecific = false;
				this.mIDHash = 0;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060004EA RID: 1258 RVA: 0x0001C344 File Offset: 0x0001A544
		// (set) Token: 0x060004EB RID: 1259 RVA: 0x0001C34C File Offset: 0x0001A54C
		public string Type
		{
			get
			{
				return this.mType;
			}
			set
			{
				this.mType = value;
				this.mTypeHash = this.mType.GetHashCodeCustom();
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060004EC RID: 1260 RVA: 0x0001C366 File Offset: 0x0001A566
		// (set) Token: 0x060004ED RID: 1261 RVA: 0x0001C36E File Offset: 0x0001A56E
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

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060004EE RID: 1262 RVA: 0x0001C388 File Offset: 0x0001A588
		// (set) Token: 0x060004EF RID: 1263 RVA: 0x0001C390 File Offset: 0x0001A590
		public string TargetArea
		{
			get
			{
				return this.mTargetArea;
			}
			set
			{
				this.mTargetArea = value;
				this.mTargetAreaHash = this.mTargetArea.GetHashCodeCustom();
				if (this.mTargetAreaHash == GiveOrder.ANYID)
				{
					this.mTargetAreaHash = 0;
				}
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060004F0 RID: 1264 RVA: 0x0001C3BE File Offset: 0x0001A5BE
		// (set) Token: 0x060004F1 RID: 1265 RVA: 0x0001C3C6 File Offset: 0x0001A5C6
		public float Speed
		{
			get
			{
				return this.mWanderSpeed;
			}
			set
			{
				this.mWanderSpeed = value;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (set) Token: 0x060004F2 RID: 1266 RVA: 0x0001C3CF File Offset: 0x0001A5CF
		public string PriorityTarget
		{
			set
			{
				this.mPriorityTarget = value;
				this.mPriorityTargetHash = this.mPriorityTarget.GetHashCodeCustom();
			}
		}

		// Token: 0x170000C7 RID: 199
		// (set) Token: 0x060004F3 RID: 1267 RVA: 0x0001C3E9 File Offset: 0x0001A5E9
		public int PriorityAbility
		{
			set
			{
				this.mPriorityAbility = value;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (set) Token: 0x060004F4 RID: 1268 RVA: 0x0001C3F2 File Offset: 0x0001A5F2
		public bool Explicit
		{
			set
			{
				this.mExplicitFaction = value;
			}
		}

		// Token: 0x04000380 RID: 896
		public static readonly int ANYID = "any".GetHashCodeCustom();

		// Token: 0x04000381 RID: 897
		private static PlayState sPlayState;

		// Token: 0x04000382 RID: 898
		private static List<GiveOrder> sInstances = new List<GiveOrder>();

		// Token: 0x04000383 RID: 899
		private ushort mHandle;

		// Token: 0x04000384 RID: 900
		private XmlNode mNode;

		// Token: 0x04000385 RID: 901
		protected Order mOrder;

		// Token: 0x04000386 RID: 902
		protected ReactTo mReactTo;

		// Token: 0x04000387 RID: 903
		protected Order mReaction = Order.Attack;

		// Token: 0x04000388 RID: 904
		protected new string mTrigger;

		// Token: 0x04000389 RID: 905
		protected int mTriggerID;

		// Token: 0x0400038A RID: 906
		protected Animations mAnimation;

		// Token: 0x0400038B RID: 907
		protected Animations mIdleAnimation;

		// Token: 0x0400038C RID: 908
		protected string mID;

		// Token: 0x0400038D RID: 909
		protected int mIDHash;

		// Token: 0x0400038E RID: 910
		protected Factions mFactions;

		// Token: 0x0400038F RID: 911
		protected string mType;

		// Token: 0x04000390 RID: 912
		protected int mTypeHash;

		// Token: 0x04000391 RID: 913
		protected string mArea;

		// Token: 0x04000392 RID: 914
		protected int mAreaHash;

		// Token: 0x04000393 RID: 915
		protected string mTargetArea;

		// Token: 0x04000394 RID: 916
		protected int mTargetAreaHash;

		// Token: 0x04000395 RID: 917
		protected float mWanderSpeed = 1f;

		// Token: 0x04000396 RID: 918
		protected bool mIsSpecific;

		// Token: 0x04000397 RID: 919
		internal AIEvent[] mEvents;

		// Token: 0x04000398 RID: 920
		private string mPriorityTarget;

		// Token: 0x04000399 RID: 921
		private int mPriorityTargetHash;

		// Token: 0x0400039A RID: 922
		private int mPriorityAbility = -1;

		// Token: 0x0400039B RID: 923
		private bool mExplicitFaction;
	}
}
