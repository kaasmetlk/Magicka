using System;
using System.Xml;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200032F RID: 815
	public class Remove : Action
	{
		// Token: 0x060018DF RID: 6367 RVA: 0x000A3EC7 File Offset: 0x000A20C7
		public Remove(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060018E0 RID: 6368 RVA: 0x000A3ED4 File Offset: 0x000A20D4
		protected override void Execute()
		{
			if (this.mIsSpecific)
			{
				Entity byID = Entity.GetByID(this.mIDHash);
				Character character = byID as Character;
				if (character != null)
				{
					if (this.Force)
					{
						character.CannotDieWithoutExplicitKill = false;
					}
					character.Kill();
					character.Terminate(true, false);
					return;
				}
				if (byID != null)
				{
					byID.Kill();
					return;
				}
			}
			else
			{
				TriggerArea triggerArea = base.GameScene.GetTriggerArea(this.mAreaHash);
				for (int i = 0; i < triggerArea.PresentCharacters.Count; i++)
				{
					Character character2 = triggerArea.PresentCharacters[i];
					if (character2 != null && (this.mTypeHash == Remove.ANYID || character2.Type == this.mTypeHash || (character2.GetOriginalFaction & this.mFactions) != Factions.NONE))
					{
						character2.Kill();
						character2.Terminate(true, false);
					}
				}
			}
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x000A3F9D File Offset: 0x000A219D
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x060018E2 RID: 6370 RVA: 0x000A3FA5 File Offset: 0x000A21A5
		// (set) Token: 0x060018E3 RID: 6371 RVA: 0x000A3FAD File Offset: 0x000A21AD
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

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x060018E4 RID: 6372 RVA: 0x000A3FB6 File Offset: 0x000A21B6
		// (set) Token: 0x060018E5 RID: 6373 RVA: 0x000A3FBE File Offset: 0x000A21BE
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

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x060018E6 RID: 6374 RVA: 0x000A3FFB File Offset: 0x000A21FB
		// (set) Token: 0x060018E7 RID: 6375 RVA: 0x000A4003 File Offset: 0x000A2203
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

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x060018E8 RID: 6376 RVA: 0x000A401D File Offset: 0x000A221D
		// (set) Token: 0x060018E9 RID: 6377 RVA: 0x000A4025 File Offset: 0x000A2225
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

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x060018EA RID: 6378 RVA: 0x000A403F File Offset: 0x000A223F
		// (set) Token: 0x060018EB RID: 6379 RVA: 0x000A4047 File Offset: 0x000A2247
		public bool Force { get; set; }

		// Token: 0x04001AB2 RID: 6834
		public static readonly int ANYID = "any".GetHashCodeCustom();

		// Token: 0x04001AB3 RID: 6835
		protected string mID;

		// Token: 0x04001AB4 RID: 6836
		protected int mIDHash;

		// Token: 0x04001AB5 RID: 6837
		protected string mType;

		// Token: 0x04001AB6 RID: 6838
		protected Factions mFactions;

		// Token: 0x04001AB7 RID: 6839
		protected int mTypeHash;

		// Token: 0x04001AB8 RID: 6840
		protected string mArea;

		// Token: 0x04001AB9 RID: 6841
		protected int mAreaHash;

		// Token: 0x04001ABA RID: 6842
		protected bool mIsSpecific;
	}
}
