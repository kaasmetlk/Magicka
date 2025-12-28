using System;
using System.Xml;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000241 RID: 577
	public class Kill : Action
	{
		// Token: 0x060011D5 RID: 4565 RVA: 0x0006CA63 File Offset: 0x0006AC63
		public Kill(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x0006CA70 File Offset: 0x0006AC70
		protected override void Execute()
		{
			if (this.mIsSpecific)
			{
				Entity byID = Entity.GetByID(this.mIDHash);
				if (byID != null)
				{
					if (byID is Character)
					{
						(byID as Character).CannotDieWithoutExplicitKill = false;
					}
					byID.Kill();
					return;
				}
			}
			else
			{
				TriggerArea triggerArea = base.GameScene.GetTriggerArea(this.mAreaHash);
				for (int i = 0; i < triggerArea.PresentCharacters.Count; i++)
				{
					Character character = triggerArea.PresentCharacters[i];
					if (character != null && (this.mTypeHash == Kill.ANYID || character.Type == this.mTypeHash || (character.GetOriginalFaction & this.mFactions) != Factions.NONE))
					{
						character.CannotDieWithoutExplicitKill = false;
						character.Kill();
					}
				}
			}
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x0006CB21 File Offset: 0x0006AD21
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x060011D8 RID: 4568 RVA: 0x0006CB29 File Offset: 0x0006AD29
		// (set) Token: 0x060011D9 RID: 4569 RVA: 0x0006CB31 File Offset: 0x0006AD31
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

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x060011DA RID: 4570 RVA: 0x0006CB3A File Offset: 0x0006AD3A
		// (set) Token: 0x060011DB RID: 4571 RVA: 0x0006CB42 File Offset: 0x0006AD42
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

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x060011DC RID: 4572 RVA: 0x0006CB7F File Offset: 0x0006AD7F
		// (set) Token: 0x060011DD RID: 4573 RVA: 0x0006CB87 File Offset: 0x0006AD87
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

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x060011DE RID: 4574 RVA: 0x0006CBA1 File Offset: 0x0006ADA1
		// (set) Token: 0x060011DF RID: 4575 RVA: 0x0006CBA9 File Offset: 0x0006ADA9
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

		// Token: 0x0400109C RID: 4252
		public static readonly int ANYID = "any".GetHashCodeCustom();

		// Token: 0x0400109D RID: 4253
		protected string mID;

		// Token: 0x0400109E RID: 4254
		protected int mIDHash;

		// Token: 0x0400109F RID: 4255
		protected string mType;

		// Token: 0x040010A0 RID: 4256
		protected Factions mFactions;

		// Token: 0x040010A1 RID: 4257
		protected int mTypeHash;

		// Token: 0x040010A2 RID: 4258
		protected string mArea;

		// Token: 0x040010A3 RID: 4259
		protected int mAreaHash;

		// Token: 0x040010A4 RID: 4260
		protected bool mIsSpecific;
	}
}
