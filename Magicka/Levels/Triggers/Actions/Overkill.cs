using System;
using System.Xml;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200001B RID: 27
	public class Overkill : Action
	{
		// Token: 0x060000CC RID: 204 RVA: 0x00006D71 File Offset: 0x00004F71
		public Overkill(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00006D7C File Offset: 0x00004F7C
		protected override void Execute()
		{
			if (this.mIsSpecific)
			{
				IDamageable damageable = Entity.GetByID(this.mIDHash) as IDamageable;
				if (damageable != null)
				{
					damageable.OverKill();
					return;
				}
			}
			else
			{
				TriggerArea triggerArea = base.GameScene.GetTriggerArea(this.mAreaHash);
				for (int i = 0; i < triggerArea.PresentCharacters.Count; i++)
				{
					Character character = triggerArea.PresentCharacters[i];
					if (character != null && (this.mTypeHash == Overkill.ANYID || character.Type == this.mTypeHash || (character.GetOriginalFaction & this.mFactions) != Factions.NONE))
					{
						character.OverKill();
					}
				}
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00006E14 File Offset: 0x00005014
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00006E1C File Offset: 0x0000501C
		// (set) Token: 0x060000D0 RID: 208 RVA: 0x00006E24 File Offset: 0x00005024
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

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00006E2D File Offset: 0x0000502D
		// (set) Token: 0x060000D2 RID: 210 RVA: 0x00006E35 File Offset: 0x00005035
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

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00006E72 File Offset: 0x00005072
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x00006E7A File Offset: 0x0000507A
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

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00006E94 File Offset: 0x00005094
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x00006E9C File Offset: 0x0000509C
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

		// Token: 0x04000098 RID: 152
		public static readonly int ANYID = "any".GetHashCodeCustom();

		// Token: 0x04000099 RID: 153
		protected string mID;

		// Token: 0x0400009A RID: 154
		protected int mIDHash;

		// Token: 0x0400009B RID: 155
		protected string mType;

		// Token: 0x0400009C RID: 156
		protected Factions mFactions;

		// Token: 0x0400009D RID: 157
		protected int mTypeHash;

		// Token: 0x0400009E RID: 158
		protected string mArea;

		// Token: 0x0400009F RID: 159
		protected int mAreaHash;

		// Token: 0x040000A0 RID: 160
		protected bool mIsSpecific;
	}
}
