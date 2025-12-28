using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using JigLibX.Geometry;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200055D RID: 1373
	public class Damage : Action
	{
		// Token: 0x060028E9 RID: 10473 RVA: 0x00141AC8 File Offset: 0x0013FCC8
		public Damage(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
			List<Damage> list = new List<Damage>();
			foreach (object obj in iNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlComment) && xmlNode.Name.Equals("damage", StringComparison.OrdinalIgnoreCase))
				{
					Damage item = default(Damage);
					foreach (object obj2 in xmlNode.Attributes)
					{
						XmlAttribute xmlAttribute = (XmlAttribute)obj2;
						if (xmlAttribute.Name.Equals("attackProperty", StringComparison.OrdinalIgnoreCase))
						{
							item.AttackProperty = (AttackProperties)Enum.Parse(typeof(AttackProperties), xmlAttribute.Value, true);
						}
						else if (xmlAttribute.Name.Equals("element", StringComparison.OrdinalIgnoreCase))
						{
							item.Element = (Elements)Enum.Parse(typeof(Elements), xmlAttribute.Value, true);
						}
						else if (xmlAttribute.Name.Equals("amount", StringComparison.OrdinalIgnoreCase))
						{
							item.Amount = float.Parse(xmlAttribute.Value);
						}
						else if (xmlAttribute.Name.Equals("magnitude", StringComparison.OrdinalIgnoreCase))
						{
							item.Magnitude = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture.NumberFormat);
						}
					}
					list.Add(item);
				}
			}
			this.mDamages = list.ToArray();
		}

		// Token: 0x060028EA RID: 10474 RVA: 0x00141CA8 File Offset: 0x0013FEA8
		protected override void Execute()
		{
			Vector3 vector = default(Vector3);
			if (this.mSourceID != 0)
			{
				Matrix matrix;
				base.GameScene.GetLocator(this.mSourceID, out matrix);
				vector = matrix.Translation;
			}
			if (this.mIsSpecific)
			{
				IDamageable damageable = Entity.GetByID(this.mIDHash) as IDamageable;
				if (damageable != null)
				{
					for (int i = 0; i < this.mDamages.Length; i++)
					{
						damageable.Damage(this.mDamages[i], null, 0.0, vector);
					}
					return;
				}
			}
			else
			{
				TriggerArea triggerArea = base.GameScene.GetTriggerArea(this.mAreaHash);
				for (int j = 0; j < triggerArea.PresentCharacters.Count; j++)
				{
					Character character = triggerArea.PresentCharacters[j];
					if (character != null && (this.mTypeHash == Damage.ANYID || character.Type == this.mTypeHash || (character.GetOriginalFaction & this.mFactions) != Factions.NONE))
					{
						if (!this.mIgnoreShields)
						{
							if (this.mSourceID == 0)
							{
								Shield shield;
								if (base.GameScene.PlayState.EntityManager.IsProtectedByShield(character, out shield))
								{
									goto IL_1E6;
								}
							}
							else
							{
								Segment iSeg;
								iSeg.Origin = vector;
								iSeg.Delta = character.Position;
								Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
								List<Shield> shields = base.GameScene.PlayState.EntityManager.Shields;
								bool flag = false;
								foreach (Shield shield2 in shields)
								{
									Vector3 vector2;
									if (shield2.SegmentIntersect(out vector2, iSeg, 0.25f))
									{
										flag = true;
										break;
									}
								}
								if (flag)
								{
									goto IL_1E6;
								}
							}
						}
						for (int k = 0; k < this.mDamages.Length; k++)
						{
							character.Damage(this.mDamages[k], null, 0.0, vector);
						}
					}
					IL_1E6:;
				}
			}
		}

		// Token: 0x060028EB RID: 10475 RVA: 0x00141EC4 File Offset: 0x001400C4
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170009A0 RID: 2464
		// (get) Token: 0x060028EC RID: 10476 RVA: 0x00141ECC File Offset: 0x001400CC
		// (set) Token: 0x060028ED RID: 10477 RVA: 0x00141ED4 File Offset: 0x001400D4
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

		// Token: 0x170009A1 RID: 2465
		// (get) Token: 0x060028EE RID: 10478 RVA: 0x00141EDD File Offset: 0x001400DD
		// (set) Token: 0x060028EF RID: 10479 RVA: 0x00141EE5 File Offset: 0x001400E5
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

		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x060028F0 RID: 10480 RVA: 0x00141F22 File Offset: 0x00140122
		// (set) Token: 0x060028F1 RID: 10481 RVA: 0x00141F2A File Offset: 0x0014012A
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

		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x060028F2 RID: 10482 RVA: 0x00141F44 File Offset: 0x00140144
		// (set) Token: 0x060028F3 RID: 10483 RVA: 0x00141F4C File Offset: 0x0014014C
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

		// Token: 0x170009A4 RID: 2468
		// (get) Token: 0x060028F4 RID: 10484 RVA: 0x00141F66 File Offset: 0x00140166
		// (set) Token: 0x060028F5 RID: 10485 RVA: 0x00141F6E File Offset: 0x0014016E
		public string Source
		{
			get
			{
				return this.mSource;
			}
			set
			{
				this.mSource = value;
				this.mSourceID = this.mSource.GetHashCodeCustom();
			}
		}

		// Token: 0x170009A5 RID: 2469
		// (get) Token: 0x060028F6 RID: 10486 RVA: 0x00141F88 File Offset: 0x00140188
		// (set) Token: 0x060028F7 RID: 10487 RVA: 0x00141F90 File Offset: 0x00140190
		public bool IgnoreShields
		{
			get
			{
				return this.mIgnoreShields;
			}
			set
			{
				this.mIgnoreShields = value;
			}
		}

		// Token: 0x04002C55 RID: 11349
		public static readonly int ANYID = "any".GetHashCodeCustom();

		// Token: 0x04002C56 RID: 11350
		protected string mID;

		// Token: 0x04002C57 RID: 11351
		protected int mIDHash;

		// Token: 0x04002C58 RID: 11352
		protected string mType;

		// Token: 0x04002C59 RID: 11353
		protected Factions mFactions;

		// Token: 0x04002C5A RID: 11354
		protected int mTypeHash;

		// Token: 0x04002C5B RID: 11355
		protected string mArea;

		// Token: 0x04002C5C RID: 11356
		protected int mAreaHash;

		// Token: 0x04002C5D RID: 11357
		protected string mSource;

		// Token: 0x04002C5E RID: 11358
		protected int mSourceID;

		// Token: 0x04002C5F RID: 11359
		protected bool mIgnoreShields = true;

		// Token: 0x04002C60 RID: 11360
		private Damage[] mDamages;

		// Token: 0x04002C61 RID: 11361
		protected bool mIsSpecific;
	}
}
