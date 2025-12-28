using System;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x0200016B RID: 363
	public struct ChantSpells
	{
		// Token: 0x06000B07 RID: 2823 RVA: 0x00042550 File Offset: 0x00040750
		public ChantSpells(Elements iElement, Character iOwner)
		{
			this.Active = false;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mMergeTarget = 0;
			this.mSourcePosition = iOwner.Position;
			if (iOwner.SpellQueue.Count >= 5)
			{
				this.State = ChantSpellState.Escaping;
			}
			else
			{
				this.State = ChantSpellState.Orbiting;
			}
			this.Index = 0;
			this.mElement = iElement;
			this.mNewElement = Elements.None;
			this.mTargetPosition = iOwner.CastSource.Translation;
			if (this.mTargetPosition.LengthSquared() > 1E-45f)
			{
				this.mPosition = iOwner.CastSource.Translation;
			}
			else
			{
				this.mPosition = iOwner.Position;
			}
			this.mHorizontalTime = 0f;
			this.mVerticalTime = 0f;
			this.mHorizontalOffset = (float)MagickaMath.Random.NextDouble() * 0.5f + 0.75f;
			this.mVerticalOffset = (float)MagickaMath.Random.NextDouble();
			this.mHorizontalSpeed = (float)MagickaMath.Random.NextDouble() + 0.5f;
			this.mVerticalSpeed = ((float)MagickaMath.Random.NextDouble() - 0.5f) * 2f;
			if (this.mVerticalSpeed < 0f)
			{
				this.mHorizontalSpeed *= -1f;
			}
			this.mTargetPosition = this.mPosition + new Vector3((float)MagickaMath.Random.NextDouble() - 0.5f, 0f, (float)MagickaMath.Random.NextDouble() - 0.5f);
			this.Owner = iOwner;
			Vector3 vector = default(Vector3);
			if (iElement <= Elements.Arcane)
			{
				if (iElement <= Elements.Fire)
				{
					switch (iElement)
					{
					case Elements.Water:
						vector = Spell.WATERCOLOR;
						break;
					case Elements.Earth | Elements.Water:
						break;
					case Elements.Cold:
						vector = Spell.COLDCOLOR;
						break;
					default:
						if (iElement == Elements.Fire)
						{
							vector = Spell.FIRECOLOR;
						}
						break;
					}
				}
				else if (iElement != Elements.Lightning)
				{
					if (iElement == Elements.Arcane)
					{
						vector = Spell.ARCANECOLOR;
					}
				}
				else
				{
					vector = Spell.LIGHTNINGCOLOR;
				}
			}
			else if (iElement <= Elements.Shield)
			{
				if (iElement != Elements.Life)
				{
					if (iElement == Elements.Shield)
					{
						vector = Spell.SHIELDCOLOR;
					}
				}
				else
				{
					vector = Spell.LIFECOLOR;
				}
			}
			else if (iElement != Elements.Ice)
			{
				if (iElement == Elements.Poison)
				{
					vector = Spell.LIFECOLOR;
				}
			}
			else
			{
				vector = Spell.ICECOLOR;
			}
			vector *= 0.75f;
			if (vector.LengthSquared() > 0.1f)
			{
				Vector3 ambientColor;
				Vector3.Multiply(ref vector, 0.25f, out ambientColor);
				this.mLight = DynamicLight.GetCachedLight();
				this.mLight.Initialize(this.mPosition, vector, 0.5f, 7f, 5f, 1f);
				this.mLight.AmbientColor = ambientColor;
				this.mLight.Enable();
			}
			else
			{
				this.mLight = null;
			}
			Vector3 direction = iOwner.Direction;
			int num = Defines.ElementIndex(iElement);
			int iHash = Defines.ChantEffects[num];
			EffectManager.Instance.StartEffect(iHash, ref this.mPosition, ref direction, out this.mEffect);
			this.mTTL = 1f + (float)MagickaMath.Random.NextDouble();
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x0004285C File Offset: 0x00040A5C
		public void Update(float iDeltaTime)
		{
			switch (this.State)
			{
			case ChantSpellState.Orbiting:
				if (this.Owner == null || this.Owner.Dead || this.Owner.Polymorphed)
				{
					this.Owner = null;
					this.State = ChantSpellState.Escaping;
					this.mTTL = 1f;
				}
				else if ((this.mElement == Elements.Lightning & this.Owner.HasStatus(StatusEffects.Wet)) && !this.Owner.HasPassiveAbility(Item.PassiveAbilities.WetLightning))
				{
					Spell spell;
					Spell.DefaultSpell(Elements.Lightning, out spell);
					DamageCollection5 iDamage;
					spell.CalculateDamage(SpellType.Lightning, CastType.Self, out iDamage);
					iDamage.MultiplyMagnitude(0.5f);
					this.Owner.Damage(iDamage, this.Owner, this.mTimeStamp, this.mPosition);
					Vector3 position = this.Owner.Position;
					Vector3.Subtract(ref position, ref this.mPosition, out position);
					LightningBolt lightning = LightningBolt.GetLightning();
					lightning.InitializeEffect(ref this.mPosition, position, Spell.LIGHTNINGCOLOR, false, 1f, 1f, this.Owner.PlayState);
					for (int i = 0; i < this.Owner.SpellQueue.Count; i++)
					{
						if ((this.Owner.SpellQueue[i].Element & Elements.Lightning) == Elements.Lightning)
						{
							this.Owner.SpellQueue.RemoveAt(i);
							break;
						}
					}
					if (this.Owner is Avatar)
					{
						(this.Owner as Avatar).Player.IconRenderer.ClearElements(Elements.Lightning);
					}
					this.State = ChantSpellState.MoveToTarget;
					this.mTTL = 0f;
				}
				else
				{
					this.mSourcePosition = this.Owner.Position;
					if (!this.Owner.Chanting)
					{
						this.Stop();
					}
					else
					{
						this.mVerticalTime = MathHelper.WrapAngle(this.mVerticalTime + iDeltaTime * 4f * this.mVerticalSpeed);
						this.mHorizontalTime = MathHelper.WrapAngle(this.mHorizontalTime + iDeltaTime * 5f * this.mHorizontalSpeed);
						Vector3 zero = Vector3.Zero;
						MathApproximation.FastSinCos(this.mHorizontalTime, out zero.X, out zero.Z);
						MathApproximation.FastSin(this.mVerticalTime, out zero.Y);
						zero.X *= this.mHorizontalOffset;
						zero.Z *= this.mHorizontalOffset;
						zero.Y *= this.mVerticalOffset;
						Vector3.Add(ref this.mSourcePosition, ref zero, out this.mTargetPosition);
					}
				}
				break;
			case ChantSpellState.Escaping:
			{
				this.mTTL -= iDeltaTime;
				float num = (float)Math.Sin((double)(this.mTTL * 3.1415927f));
				Vector3 value = this.mTargetPosition - this.mPosition;
				value.Normalize();
				this.mTargetPosition += value + new Vector3(num + 0.4f, num * 0.2f + 0.4f, num * -1.2f - 0.4f);
				if (this.mTTL < 0f)
				{
					this.Stop();
				}
				break;
			}
			case ChantSpellState.Merging:
				this.mTargetPosition = ChantSpellManager.GetChantSpell(this.mMergeTarget).mPosition;
				if ((this.mPosition - ChantSpellManager.GetChantSpell(this.mMergeTarget).mPosition).LengthSquared() < 0.02f)
				{
					ChantSpellManager.GetChantSpell(this.mMergeTarget).Stop();
					if (this.mNewElement != Elements.None)
					{
						this.Reinitialize();
						return;
					}
					this.Stop();
					return;
				}
				break;
			case ChantSpellState.MoveToTarget:
				if (this.mTTL <= 0f)
				{
					this.Stop();
				}
				break;
			}
			this.mPosition += Vector3.Normalize(this.mTargetPosition - this.mPosition) * iDeltaTime * 5f;
			Vector3 forward = Vector3.Forward;
			EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref this.mPosition, ref forward);
			if (this.mLight != null)
			{
				this.mLight.Position = this.mPosition;
			}
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x00042C88 File Offset: 0x00040E88
		public void Reinitialize()
		{
			EffectManager.Instance.Stop(ref this.mEffect);
			if (this.mLight != null)
			{
				this.mLight.Stop(false);
				this.mLight = null;
			}
			this.State = ChantSpellState.Orbiting;
			this.mElement = this.mNewElement;
			this.mNewElement = Elements.None;
			this.mHorizontalTime = 0f;
			this.mVerticalTime = 0f;
			this.mHorizontalOffset = (float)MagickaMath.Random.NextDouble() * 0.5f + 0.75f;
			this.mVerticalOffset = (float)MagickaMath.Random.NextDouble();
			this.mHorizontalSpeed = (float)MagickaMath.Random.NextDouble() + 0.5f;
			this.mVerticalSpeed = ((float)MagickaMath.Random.NextDouble() - 0.5f) * 2f;
			if (this.mVerticalSpeed < 0f)
			{
				this.mHorizontalSpeed *= -1f;
			}
			this.mTargetPosition = this.mPosition + new Vector3((float)MagickaMath.Random.NextDouble() - 0.5f, 0f, (float)MagickaMath.Random.NextDouble() - 0.5f);
			Vector3 vector = default(Vector3);
			Elements elements = this.mElement;
			if (elements <= Elements.Arcane)
			{
				if (elements <= Elements.Fire)
				{
					switch (elements)
					{
					case Elements.Water:
						vector = Spell.WATERCOLOR;
						break;
					case Elements.Earth | Elements.Water:
						break;
					case Elements.Cold:
						vector = Spell.COLDCOLOR;
						break;
					default:
						if (elements == Elements.Fire)
						{
							vector = Spell.FIRECOLOR;
						}
						break;
					}
				}
				else if (elements != Elements.Lightning)
				{
					if (elements == Elements.Arcane)
					{
						vector = Spell.ARCANECOLOR;
					}
				}
				else
				{
					vector = Spell.LIGHTNINGCOLOR;
				}
			}
			else if (elements <= Elements.Shield)
			{
				if (elements != Elements.Life)
				{
					if (elements == Elements.Shield)
					{
						vector = Spell.SHIELDCOLOR;
					}
				}
				else
				{
					vector = Spell.LIFECOLOR;
				}
			}
			else if (elements != Elements.Ice)
			{
				if (elements == Elements.Poison)
				{
					vector = Spell.LIFECOLOR;
				}
			}
			else
			{
				vector = Spell.ICECOLOR;
			}
			vector *= 0.75f;
			if (vector.LengthSquared() > 0.1f)
			{
				Vector3 ambientColor;
				Vector3.Multiply(ref vector, 0.25f, out ambientColor);
				this.mLight = DynamicLight.GetCachedLight();
				this.mLight.Initialize(this.mPosition, vector, 0.5f, 7f, 5f, 1f);
				this.mLight.AmbientColor = ambientColor;
				this.mLight.Enable();
			}
			else
			{
				this.mLight = null;
			}
			if (this.Owner.SpellQueue.Count >= 5)
			{
				this.State = ChantSpellState.Escaping;
			}
			Vector3 direction = this.Owner.Direction;
			int num = Defines.ElementIndex(this.mElement);
			int iHash = Defines.ChantEffects[num];
			EffectManager.Instance.StartEffect(iHash, ref this.mPosition, ref direction, out this.mEffect);
			this.mTTL = 1f + (float)MagickaMath.Random.NextDouble();
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x00042F50 File Offset: 0x00041150
		public void Stop()
		{
			EffectManager.Instance.Stop(ref this.mEffect);
			if (this.mLight != null)
			{
				this.mLight.Stop(false);
				this.mLight = null;
			}
			ChantSpellManager.Remove(ref this);
		}

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06000B0B RID: 2827 RVA: 0x00042F83 File Offset: 0x00041183
		public Elements Element
		{
			get
			{
				return this.mElement;
			}
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x00042F8C File Offset: 0x0004118C
		public void MergeWith(ChantSpells iSpell, Elements iNewElement)
		{
			this.State = ChantSpellState.Merging;
			this.mMergeTarget = iSpell.Index;
			if (iNewElement != Elements.None)
			{
				this.mNewElement = iNewElement;
				iSpell.mNewElement = iNewElement;
			}
			iSpell.mMergeTarget = this.Index;
			iSpell.State = ChantSpellState.Merging;
			ChantSpellManager.Set(iSpell);
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06000B0D RID: 2829 RVA: 0x00042FDA File Offset: 0x000411DA
		// (set) Token: 0x06000B0E RID: 2830 RVA: 0x00042FE2 File Offset: 0x000411E2
		public float TTL
		{
			get
			{
				return this.mTTL;
			}
			set
			{
				this.mTTL = value;
			}
		}

		// Token: 0x04000A0C RID: 2572
		public ChantSpellState State;

		// Token: 0x04000A0D RID: 2573
		private Elements mElement;

		// Token: 0x04000A0E RID: 2574
		private Elements mNewElement;

		// Token: 0x04000A0F RID: 2575
		public bool Active;

		// Token: 0x04000A10 RID: 2576
		public int Index;

		// Token: 0x04000A11 RID: 2577
		private float mHorizontalTime;

		// Token: 0x04000A12 RID: 2578
		private float mVerticalTime;

		// Token: 0x04000A13 RID: 2579
		private float mHorizontalOffset;

		// Token: 0x04000A14 RID: 2580
		private float mVerticalOffset;

		// Token: 0x04000A15 RID: 2581
		private float mHorizontalSpeed;

		// Token: 0x04000A16 RID: 2582
		private float mVerticalSpeed;

		// Token: 0x04000A17 RID: 2583
		private Vector3 mTargetPosition;

		// Token: 0x04000A18 RID: 2584
		private Vector3 mSourcePosition;

		// Token: 0x04000A19 RID: 2585
		private Vector3 mPosition;

		// Token: 0x04000A1A RID: 2586
		private VisualEffectReference mEffect;

		// Token: 0x04000A1B RID: 2587
		private DynamicLight mLight;

		// Token: 0x04000A1C RID: 2588
		public Character Owner;

		// Token: 0x04000A1D RID: 2589
		private int mMergeTarget;

		// Token: 0x04000A1E RID: 2590
		private double mTimeStamp;

		// Token: 0x04000A1F RID: 2591
		private float mTTL;
	}
}
