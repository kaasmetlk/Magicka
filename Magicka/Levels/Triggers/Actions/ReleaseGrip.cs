using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020003E7 RID: 999
	internal class ReleaseGrip : Action
	{
		// Token: 0x06001E9F RID: 7839 RVA: 0x000D5FE6 File Offset: 0x000D41E6
		public ReleaseGrip(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001EA0 RID: 7840 RVA: 0x000D5FF0 File Offset: 0x000D41F0
		protected override void Execute()
		{
			if (this.mIsSpecific)
			{
				NonPlayerCharacter nonPlayerCharacter = Entity.GetByID(this.mIDHash) as NonPlayerCharacter;
				if (nonPlayerCharacter != null)
				{
					nonPlayerCharacter.ReleaseAttachedCharacter();
					if (nonPlayerCharacter.Gripper != null)
					{
						nonPlayerCharacter.Gripper.ReleaseAttachedCharacter();
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
					if (nonPlayerCharacter2 != null && (this.mTypeHash == ReleaseGrip.ANYID || nonPlayerCharacter2.Type == this.mTypeHash || (nonPlayerCharacter2.GetOriginalFaction & this.mFactions) != Factions.NONE))
					{
						nonPlayerCharacter2.ReleaseAttachedCharacter();
						if (nonPlayerCharacter2.Gripper != null)
						{
							nonPlayerCharacter2.Gripper.ReleaseAttachedCharacter();
						}
					}
				}
			}
		}

		// Token: 0x06001EA1 RID: 7841 RVA: 0x000D60B9 File Offset: 0x000D42B9
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x1700077D RID: 1917
		// (get) Token: 0x06001EA2 RID: 7842 RVA: 0x000D60C1 File Offset: 0x000D42C1
		// (set) Token: 0x06001EA3 RID: 7843 RVA: 0x000D60C9 File Offset: 0x000D42C9
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

		// Token: 0x1700077E RID: 1918
		// (get) Token: 0x06001EA4 RID: 7844 RVA: 0x000D60D2 File Offset: 0x000D42D2
		// (set) Token: 0x06001EA5 RID: 7845 RVA: 0x000D60DA File Offset: 0x000D42DA
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

		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x06001EA6 RID: 7846 RVA: 0x000D6117 File Offset: 0x000D4317
		// (set) Token: 0x06001EA7 RID: 7847 RVA: 0x000D611F File Offset: 0x000D431F
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

		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x06001EA8 RID: 7848 RVA: 0x000D6139 File Offset: 0x000D4339
		// (set) Token: 0x06001EA9 RID: 7849 RVA: 0x000D6141 File Offset: 0x000D4341
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

		// Token: 0x040020F0 RID: 8432
		public static readonly int ANYID = "any".GetHashCodeCustom();

		// Token: 0x040020F1 RID: 8433
		protected Factions mFactions;

		// Token: 0x040020F2 RID: 8434
		protected string mType;

		// Token: 0x040020F3 RID: 8435
		protected int mTypeHash;

		// Token: 0x040020F4 RID: 8436
		protected string mID;

		// Token: 0x040020F5 RID: 8437
		protected int mIDHash;

		// Token: 0x040020F6 RID: 8438
		protected string mArea;

		// Token: 0x040020F7 RID: 8439
		protected int mAreaHash;

		// Token: 0x040020F8 RID: 8440
		protected bool mIsSpecific;
	}
}
