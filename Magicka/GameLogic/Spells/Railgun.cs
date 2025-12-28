using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x0200039F RID: 927
	internal class Railgun : IAbilityEffect
	{
		// Token: 0x06001C65 RID: 7269 RVA: 0x000C3670 File Offset: 0x000C1870
		public static void InitializeCache(int iSize)
		{
			Railgun.sCache = new List<Railgun>(iSize);
			Railgun.sActiveRails = new List<Railgun>(iSize);
			for (int i = 0; i < iSize; i++)
			{
				Railgun.sCache.Add(new Railgun());
			}
		}

		// Token: 0x06001C66 RID: 7270 RVA: 0x000C36B0 File Offset: 0x000C18B0
		public static Railgun GetFromCache()
		{
			if (Railgun.sCache.Count > 0)
			{
				Railgun result = Railgun.sCache[Railgun.sCache.Count - 1];
				Railgun.sCache.RemoveAt(Railgun.sCache.Count - 1);
				return result;
			}
			return new Railgun();
		}

		// Token: 0x06001C67 RID: 7271 RVA: 0x000C3700 File Offset: 0x000C1900
		private Railgun()
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			ArcaneEffect arcaneEffect = RenderManager.Instance.GetEffect(ArcaneEffect.TYPEHASH) as ArcaneEffect;
			if (arcaneEffect == null)
			{
				lock (graphicsDevice)
				{
					arcaneEffect = new ArcaneEffect(graphicsDevice, Game.Instance.Content);
				}
				RenderManager.Instance.RegisterEffect(arcaneEffect);
			}
			this.mAdditionalElementCues = new List<Cue>(8);
			if (Railgun.sVertexBuffer == null)
			{
				Vector4[] array = new Vector4[512];
				for (int i = 0; i < 256; i++)
				{
					float num = (float)i / 255f;
					array[i * 2].Z = (float)i / 8f;
					array[i * 2 + 1].Z = (float)i / 8f;
					num *= num;
					array[i * 2].X = num;
					array[i * 2 + 1].X = num;
					array[i * 2].Y = 1f;
					array[i * 2 + 1].Y = -1f;
					array[i * 2].W = 0.5f;
					array[i * 2 + 1].W = 0.625f;
				}
				lock (graphicsDevice)
				{
					Railgun.sTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/Beams");
					Railgun.sVertexBuffer = new VertexBuffer(graphicsDevice, array.Length * 4 * 4, BufferUsage.WriteOnly);
					Railgun.sVertexBuffer.SetData<Vector4>(array);
					Railgun.sVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
					{
						new VertexElement(0, 0, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.Position, 0),
						new VertexElement(0, 4, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
						new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
					});
				}
				Railgun.sVertexBufferHash = Railgun.sVertexBuffer.GetHashCode();
				array = new Vector4[4];
				array[0].X = 1f;
				array[0].Y = 1f;
				array[0].Z = 1f;
				array[0].W = 0.125f;
				array[1].X = -1f;
				array[1].Y = 1f;
				array[1].Z = 1f;
				array[1].W = 0f;
				array[2].X = -1f;
				array[2].Y = 0f;
				array[2].Z = 0f;
				array[2].W = 0f;
				array[3].X = 1f;
				array[3].Y = 0f;
				array[3].Z = 0f;
				array[3].W = 0.125f;
				lock (graphicsDevice)
				{
					Railgun.sAdditionalVertexBuffer = new VertexBuffer(graphicsDevice, 64, BufferUsage.WriteOnly);
					Railgun.sAdditionalVertexBuffer.SetData<Vector4>(array);
					Railgun.sAdditionalVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
					{
						new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
						new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
					});
				}
				Railgun.sAdditionalVertexBufferHash = Railgun.sAdditionalVertexBuffer.GetHashCode();
			}
			this.mLight = new CapsuleLight(Game.Instance.Content);
			this.mRenderData = new Railgun.RenderData[3];
			this.mAdditionalRenderData = new Railgun.AdditionalRenderData[3];
			for (int j = 0; j < 3; j++)
			{
				Railgun.RenderData renderData = new Railgun.RenderData();
				this.mRenderData[j] = renderData;
				Railgun.AdditionalRenderData additionalRenderData = new Railgun.AdditionalRenderData();
				this.mAdditionalRenderData[j] = additionalRenderData;
			}
		}

		// Token: 0x06001C68 RID: 7272 RVA: 0x000C3B3C File Offset: 0x000C1D3C
		public void Initialize(ISpellCaster iOwner, Vector3 iPosition, Vector3 iDirection, Vector3 iColor, ref DamageCollection5 iDamages, ref Spell iSpell)
		{
			this.mOutPortal = null;
			this.mDepth = 0;
			this.mAdditionalElementCues.Clear();
			this.mTime = 0f;
			this.mLength = 0f;
			this.mDamageTimer = 0.25f;
			this.mCut = 0f;
			this.mImmaFirinMahLazer = false;
			this.mIsOposits = false;
			this.mWillExplode = false;
			this.mLocked = false;
			this.mChild = null;
			this.mParents.Clear();
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			Elements element = iSpell.Element;
			this.mArcane = ((element & Elements.Arcane) == Elements.Arcane);
			this.mAdditionalElements = (element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Steam | Elements.Poison));
			this.mAllCombinedElements = iSpell.Element;
			int iHash;
			int iHash2;
			if (this.mArcane)
			{
				iHash = Railgun.ARCANESOURCEEFFECTHASH;
				iHash2 = Railgun.ARCANEHITEFFECTHASH;
				this.mHitSoundHash = Railgun.ARCANESTAGESOUNDSHASH[1];
			}
			else
			{
				iHash = Railgun.LIFESOURCEEFFECTHASH;
				iHash2 = Railgun.LIFEHITEFFECTHASH;
				this.mHitSoundHash = Railgun.LIFESTAGESOUNDSHASH[1];
			}
			EffectManager.Instance.StartEffect(iHash, ref iPosition, ref iDirection, out this.mSourceEffect);
			EffectManager.Instance.StartEffect(iHash2, ref iPosition, ref iDirection, out this.mHitEffect);
			this.mDamages.Clear();
			this.mDamages.Add(iOwner.Handle, iDamages);
			this.mColor = iColor;
			for (int i = 0; i < 3; i++)
			{
				Railgun.RenderData renderData = this.mRenderData[i];
				renderData.ColorCenter.X = this.mColor.X * 0.666f;
				renderData.ColorCenter.Y = this.mColor.Y * 0.666f;
				renderData.ColorCenter.Z = this.mColor.Z * 0.666f;
				renderData.ColorEdge.X = this.mColor.X * 0.333f;
				renderData.ColorEdge.Y = this.mColor.Y * 0.333f;
				renderData.ColorEdge.Z = this.mColor.Z * 0.333f;
			}
			Railgun.sEmitter.Position = iPosition;
			Railgun.sEmitter.Forward = iDirection;
			SpellManager.Instance.AddSpellEffect(this);
			int iSoundIndex = this.mArcane ? Railgun.ARCANESTAGESOUNDSHASH[0] : Railgun.LIFESTAGESOUNDSHASH[0];
			this.mStageStartCue = AudioManager.Instance.PlayCue(Banks.Spells, iSoundIndex, Railgun.sEmitter);
			Spell spell;
			Spell.DefaultSpell(this.mAdditionalElements, out spell);
			foreach (Cue cue in spell.PlaySound(SpellType.Spray, CastType.Force))
			{
				if (cue != null)
				{
					this.mAdditionalElementCues.Add(cue);
				}
				cue.Apply3D(AudioManager.Instance.getListener(), Railgun.sEmitter);
				cue.Play();
			}
			this.mDead = false;
			Railgun.sEmitter.Up = Vector3.Up;
			this.mLight.Start = this.mPosition;
			this.mLight.End = this.mPosition;
			this.mLight.DiffuseColor = iColor * 1f;
			this.mLight.AmbientColor = iColor * 0f;
			this.mLight.Radius = 5f;
			this.mLight.Intensity = 0.6f;
			this.mLight.Enable(iOwner.PlayState.Scene);
			Railgun.sActiveRails.Add(this);
		}

		// Token: 0x06001C69 RID: 7273 RVA: 0x000C3ED4 File Offset: 0x000C20D4
		private void Initialize(Railgun iParentA, Railgun iParentB, ref Vector3 iPosition, ref Vector3 iDirection, int iDepth, Portal.PortalEntity iOutPortal)
		{
			this.mOutPortal = iOutPortal;
			this.mDepth = iDepth;
			this.mTime = 0f;
			this.mLength = 0f;
			this.mDamageTimer = 0.25f;
			this.mCut = 0f;
			this.mIsOposits = false;
			this.mWillExplode = false;
			this.mLocked = false;
			this.mTimeStamp = iParentA.mTimeStamp;
			this.mChild = null;
			this.mParents.Clear();
			this.mParents.Add(iParentA);
			if (iParentB != null)
			{
				this.mParents.Add(iParentB);
			}
			iParentA.mChild = this;
			this.mArcane = iParentA.mArcane;
			this.mAdditionalElements = iParentA.mAdditionalElements;
			this.mAllCombinedElements = iParentA.mAllCombinedElements;
			int iHash;
			if (this.mArcane)
			{
				iHash = Railgun.ARCANEHITEFFECTHASH;
				this.mHitSoundHash = Railgun.ARCANESTAGESOUNDSHASH[1];
			}
			else
			{
				iHash = Railgun.LIFEHITEFFECTHASH;
				this.mHitSoundHash = Railgun.LIFESTAGESOUNDSHASH[1];
			}
			EffectManager.Instance.StartEffect(iHash, ref iPosition, ref iDirection, out this.mHitEffect);
			this.mDamages.Clear();
			for (int i = 0; i < iParentA.mDamages.Count; i++)
			{
				if (!this.mDamages.ContainsKey(iParentA.mDamages.Keys[i]))
				{
					this.mDamages.Add(iParentA.mDamages.Keys[i], iParentA.mDamages.Values[i]);
				}
			}
			this.mColor = iParentA.mColor;
			if (iParentB != null)
			{
				iParentB.mChild = this;
				this.mAdditionalElements |= iParentB.mAdditionalElements;
				this.mAllCombinedElements |= iParentB.mAllCombinedElements;
				for (int j = 0; j < iParentB.mDamages.Count; j++)
				{
					if (!this.mDamages.ContainsKey(iParentB.mDamages.Keys[j]))
					{
						this.mDamages.Add(iParentB.mDamages.Keys[j], iParentB.mDamages.Values[j]);
					}
				}
				Vector3.Lerp(ref this.mColor, ref iParentB.mColor, 0.5f, out this.mColor);
				this.mImmaFirinMahLazer = true;
			}
			for (int k = 0; k < 3; k++)
			{
				Railgun.RenderData renderData = this.mRenderData[k];
				renderData.ColorCenter.X = this.mColor.X * 0.666f;
				renderData.ColorCenter.Y = this.mColor.Y * 0.666f;
				renderData.ColorCenter.Z = this.mColor.Z * 0.666f;
				renderData.ColorEdge.X = this.mColor.X * 0.333f;
				renderData.ColorEdge.Y = this.mColor.Y * 0.333f;
				renderData.ColorEdge.Z = this.mColor.Z * 0.333f;
			}
			Railgun.sEmitter.Position = iPosition;
			Railgun.sEmitter.Forward = iDirection;
			SpellManager.Instance.AddSpellEffect(this);
			int iSoundIndex = this.mArcane ? Railgun.ARCANESTAGESOUNDSHASH[0] : Railgun.LIFESTAGESOUNDSHASH[0];
			this.mStageStartCue = AudioManager.Instance.PlayCue(Banks.Spells, iSoundIndex, Railgun.sEmitter);
			Spell spell;
			Spell.DefaultSpell(this.mAdditionalElements, out spell);
			foreach (Cue cue in spell.PlaySound(SpellType.Spray, CastType.Force))
			{
				if (cue != null)
				{
					this.mAdditionalElementCues.Add(cue);
				}
				cue.Apply3D(AudioManager.Instance.getListener(), Railgun.sEmitter);
				cue.Play();
			}
			this.mDead = false;
			Railgun.sEmitter.Up = Vector3.Up;
			this.mLight.Start = this.mPosition;
			this.mLight.End = this.mPosition;
			this.mLight.DiffuseColor = this.mColor;
			this.mLight.AmbientColor = this.mColor * 0.5f;
			this.mLight.Radius = 5f;
			this.mLight.Intensity = 0.6f;
			this.mLight.Enable(Entity.GetFromHandle((int)this.mDamages.Keys[0]).PlayState.Scene);
			Railgun.sActiveRails.Add(this);
		}

		// Token: 0x06001C6A RID: 7274 RVA: 0x000C436C File Offset: 0x000C256C
		internal void Kill()
		{
			this.mDead = true;
			EffectManager.Instance.Stop(ref this.mSourceEffect);
		}

		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x06001C6B RID: 7275 RVA: 0x000C4385 File Offset: 0x000C2585
		// (set) Token: 0x06001C6C RID: 7276 RVA: 0x000C438D File Offset: 0x000C258D
		public Vector3 Position
		{
			get
			{
				return this.mPosition;
			}
			set
			{
				if (!this.mLocked)
				{
					this.mPosition = value;
				}
			}
		}

		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x06001C6D RID: 7277 RVA: 0x000C439E File Offset: 0x000C259E
		// (set) Token: 0x06001C6E RID: 7278 RVA: 0x000C43A6 File Offset: 0x000C25A6
		public Vector3 Direction
		{
			get
			{
				return this.mDirection;
			}
			set
			{
				if (!this.mLocked)
				{
					this.mDirection = value;
				}
			}
		}

		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x06001C6F RID: 7279 RVA: 0x000C43B7 File Offset: 0x000C25B7
		public bool IsDead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x000C43C0 File Offset: 0x000C25C0
		public unsafe void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.IsDead)
			{
				return;
			}
			bool flag = this.mParents.Count > 0;
			for (int i = 0; i < this.mParents.Count; i++)
			{
				if (!this.mParents[i].mDead)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this.Kill();
			}
			if (this.mParents.Count > 1)
			{
				Vector3 vector = default(Vector3);
				Vector3 vector2 = default(Vector3);
				for (int j = 0; j < this.mParents.Count; j++)
				{
					Vector3 vector3;
					Vector3.Multiply(ref this.mParents[j].mDirection, this.mParents[j].mLength, out vector3);
					Vector3.Add(ref this.mParents[j].mPosition, ref vector3, out vector3);
					Vector3.Add(ref vector, ref vector3, out vector);
					Vector3.Add(ref vector2, ref this.mParents[j].mDirection, out vector2);
				}
				Vector3.Divide(ref vector, (float)this.mParents.Count, out this.mPosition);
				Vector3.Normalize(ref vector2, out this.mDirection);
			}
			this.mTime += iDeltaTime * 30f;
			PlayState playState = Entity.GetFromHandle((int)this.mDamages.Keys[0]).PlayState;
			if (this.mWillExplode)
			{
				this.mImmaFirinMahLazer = false;
				this.mExplosionCountdown -= iDeltaTime;
				if (this.mExplosionCountdown <= 0f)
				{
					this.Explode(playState);
				}
			}
			else if (!this.mLocked)
			{
				if (!(this.mDead & this.mChild != null))
				{
					this.mLength = Math.Min(this.mLength + iDeltaTime * 30f * 3f, 75f);
				}
				Segment segment;
				segment.Origin = this.mPosition;
				Vector3.Multiply(ref this.mDirection, this.mLength, out segment.Delta);
				Segment seg;
				MagickaMath.SegmentNegate(ref segment, out seg);
				List<Entity> entities = playState.EntityManager.GetEntities(this.mPosition, this.mLength * 1.25f, false);
				float num = float.MaxValue;
				Entity entity = null;
				Vector3 iAttackPosition = this.mPosition;
				Vector3 vector4;
				float num2;
				Vector3 vector5;
				for (int k = 0; k < entities.Count; k++)
				{
					Entity entity2 = entities[k];
					if ((entity2 is IDamageable || (entity2 is Portal.PortalEntity && Portal.Instance.Connected)) && entity2 != this.mOutPortal && !((this.mDamages.Count == 1 & this.mDamages.Keys[0] == entity2.Handle) | entity2 is Grease.GreaseField | (entity2 is Barrier && !(entity2 as Barrier).Solid)))
					{
						IDamageable damageable = entity2 as IDamageable;
						if (!(damageable is CthulhuMist) || !(damageable as CthulhuMist).IgnoreElements(this.mAllCombinedElements))
						{
							Portal.PortalEntity portalEntity = entity2 as Portal.PortalEntity;
							if ((damageable != null && damageable.SegmentIntersect(out vector4, segment, 0.25f)) || (portalEntity != null && portalEntity.Body.CollisionSkin.SegmentIntersect(out num2, out vector4, out vector5, segment)))
							{
								float num3;
								Vector3.DistanceSquared(ref vector4, ref this.mPosition, out num3);
								if (num3 < num)
								{
									if (num3 > 1E-06f)
									{
										num = num3;
										entity = entity2;
										iAttackPosition = vector4;
									}
									else if ((damageable != null && damageable.SegmentIntersect(out vector4, segment, 0.25f)) || (portalEntity != null && portalEntity.Body.CollisionSkin.SegmentIntersect(out num2, out vector4, out vector5, segment)))
									{
										Vector3.DistanceSquared(ref vector4, ref this.mPosition, out num3);
										if (num3 > 1E-06f & num3 < num)
										{
											num = num3;
											entity = entity2;
											iAttackPosition = vector4;
										}
									}
								}
							}
						}
					}
				}
				playState.EntityManager.ReturnEntityList(entities);
				if (entity != null)
				{
					this.mLength = (float)Math.Sqrt((double)num);
				}
				else
				{
					this.mDamageTimer = 0.25f;
				}
				bool flag2 = entity != null;
				Vector3.Multiply(ref this.mDirection, this.mLength, out segment.Delta);
				Vector3 vector6 = default(Vector3);
				AnimatedLevelPart animatedLevelPart;
				int num4;
				if (playState.Level.CurrentScene.SegmentIntersect(out num2, out vector4, out vector5, out animatedLevelPart, out num4, segment))
				{
					float num5;
					Vector3.Dot(ref vector5, ref this.mDirection, out num5);
					if (num5 > 0f)
					{
						Vector3 vector7;
						Vector3.Multiply(ref this.mDirection, 0.1f, out vector7);
						Vector3.Add(ref segment.Origin, ref vector7, out segment.Origin);
						Vector3.Subtract(ref segment.Delta, ref vector7, out segment.Delta);
						if (playState.Level.CurrentScene.SegmentIntersect(out num2, out vector4, out vector5, out animatedLevelPart, out num4, segment))
						{
							flag2 = true;
							this.mLength *= num2;
							Vector3.Multiply(ref segment.Delta, num2, out segment.Delta);
							Vector3.Add(ref this.mPosition, ref segment.Delta, out vector6);
							entity = null;
						}
					}
					else
					{
						flag2 = true;
						this.mLength *= num2;
						Vector3.Multiply(ref segment.Delta, num2, out segment.Delta);
						Vector3.Add(ref this.mPosition, ref segment.Delta, out vector6);
						entity = null;
					}
				}
				Vector3 position = vector4;
				Vector3 vector8 = vector5;
				CollisionMaterials collisionMaterials = (CollisionMaterials)num4;
				if (animatedLevelPart != null)
				{
					collisionMaterials = animatedLevelPart.CollisionMaterial;
				}
				bool flag3 = flag2 && Vector3.Dot(vector8, this.mDirection) < 0f && collisionMaterials == CollisionMaterials.Reflect && vector8.LengthSquared() > 1E-06f;
				if (!this.mDead)
				{
					Railgun railgun = null;
					bool flag4 = false;
					for (int l = 0; l < Railgun.sActiveRails.Count; l++)
					{
						Railgun railgun2 = Railgun.sActiveRails[l];
						if (!(railgun2 == this | this.mParents.Contains(railgun2) | railgun2 == this.mChild | railgun2.mParents.Contains(this) | railgun2.mDead))
						{
							Segment seg2 = default(Segment);
							seg2.Origin = railgun2.mPosition;
							Vector3.Multiply(ref railgun2.mDirection, railgun2.mLength, out seg2.Delta);
							float num6;
							float num7;
							if (Distance.SegmentSegmentDistanceSq(out num6, out num7, segment, seg2) <= 1f & num6 > 1E-06f)
							{
								float num8 = flag4 ? ((this.mLength - 1f) / this.mLength) : 1f;
								float num9 = 1f / this.mLength;
								if (num6 <= num8 & num7 >= num9)
								{
									entity = null;
									flag4 = (num7 * railgun2.mLength < 1f);
									flag2 = false;
									this.mLength *= num6;
									Vector3.Multiply(ref segment.Delta, num6, out segment.Delta);
									railgun = railgun2;
								}
							}
						}
					}
					if (railgun != null)
					{
						bool flag5 = false;
						if (!flag4)
						{
							for (int m = 0; m < this.mDamages.Count; m++)
							{
								if (railgun.mDamages.ContainsKey(this.mDamages.Keys[m]))
								{
									flag5 = true;
								}
							}
						}
						flag5 |= SpellManager.InclusiveOpposites(this.mAdditionalElements, railgun.mAdditionalElements);
						flag5 |= (this.mArcane != railgun.mArcane);
						Vector3.Add(ref this.mPosition, ref segment.Delta, out vector6);
						if (flag5)
						{
							this.mIsOposits = true;
							this.mWillExplode = true;
							this.mExplosionCompanion = railgun;
							this.mExplosionCountdown = 0.25f;
							this.mExplosionPosition = vector6;
							Vector3.Distance(ref this.mPosition, ref vector6, out this.mLength);
							Railgun railgun3 = this;
							while (railgun3.mChild != null)
							{
								railgun3 = railgun3.mChild;
							}
							railgun3.LockAll();
						}
						else
						{
							if (railgun.mChild != this.mChild & railgun != this.mChild)
							{
								if (this.mChild != null)
								{
									this.mChild.Kill();
								}
								if (railgun.mChild != null && railgun.mChild.mParents.Count <= 2)
								{
									railgun.mChild.Kill();
								}
							}
							if ((flag4 & this.mChild == null) && !railgun.mParents.Contains(this))
							{
								for (int n = 0; n < this.mDamages.Count; n++)
								{
									ushort key = this.mDamages.Keys[n];
									if (railgun.mDamages.ContainsKey(key))
									{
										this.mWillExplode = true;
										this.mExplosionCompanion = railgun;
										this.mExplosionCountdown = 0.25f;
										this.mExplosionPosition = vector6;
										Vector3.Distance(ref this.mPosition, ref vector6, out this.mLength);
										Railgun railgun4 = railgun;
										while (railgun4.mChild != null)
										{
											railgun4 = railgun4.mChild;
										}
										railgun4.LockAll();
										break;
									}
									railgun.mDamages.Add(key, this.mDamages.Values[n]);
								}
								if (!this.mWillExplode)
								{
									this.mChild = railgun;
									railgun.mParents.Add(this);
								}
							}
							Vector3 vector9;
							Vector3.Lerp(ref this.mDirection, ref railgun.mDirection, 0.5f, out vector9);
							float num10 = vector9.Length();
							if (num10 < 0.2f)
							{
								this.mWillExplode = true;
								this.mExplosionCompanion = railgun;
								this.mExplosionCountdown = 0.25f;
								this.mExplosionPosition = vector6;
								Railgun railgun5 = this;
								while (railgun5.mChild != null)
								{
									railgun5 = railgun5.mChild;
								}
								railgun5.LockAll();
							}
							else if (this.mChild == null)
							{
								vector9.Normalize();
								int num11 = Math.Max(this.mDepth, railgun.mDepth);
								if (num11 < 6)
								{
									Railgun.GetFromCache().Initialize(this, railgun, ref vector6, ref vector9, num11 + 1, null);
								}
							}
						}
					}
					else if (this.mChild != null & (!(entity is Shield) && !(entity is Portal.PortalEntity) && (!(entity is Magicka.GameLogic.Entities.Character) || (entity as Magicka.GameLogic.Entities.Character).CurrentSelfShieldType != Magicka.GameLogic.Entities.Character.SelfShieldType.Shield) && !flag3))
					{
						this.mChild.Kill();
					}
				}
				if (entity != null)
				{
					Vector3.Multiply(ref this.mDirection, this.mLength, out vector6);
					Vector3.Add(ref this.mPosition, ref vector6, out vector6);
					if (!entity.Dead)
					{
						if (entity is Shield)
						{
							this.mDamageTimer = 0.25f;
							Shield shield = entity as Shield;
							Vector3.Multiply(ref this.mDirection, this.mLength + 1f, out segment.Delta);
							float num12;
							if (shield.Body.CollisionSkin.SegmentIntersect(out num12, out vector6, out vector5, segment))
							{
								if (num12 <= 1E-06f)
								{
									shield.Body.CollisionSkin.SegmentIntersect(out num12, out vector6, out vector5, seg);
								}
								vector5.Y = 0f;
								Vector3 direction;
								Vector3.Reflect(ref this.mDirection, ref vector5, out direction);
								direction.Normalize();
								float num5;
								Vector3.Dot(ref direction, ref this.mDirection, out num5);
								if (num5 < -0.99f)
								{
									this.mWillExplode = true;
									this.mExplosionCompanion = null;
									this.mExplosionCountdown = 0.25f;
									this.GetFirstJunction(out this.mExplosionPosition);
									Railgun railgun6 = this;
									while (railgun6.mChild != null)
									{
										railgun6 = railgun6.mChild;
									}
									railgun6.LockAll();
								}
								else
								{
									if (this.mChild != null && this.mChild.mParents.Count > 1)
									{
										this.mChild.Kill();
										this.mChild = null;
									}
									if (this.mChild == null && this.mDepth < 6)
									{
										Railgun.GetFromCache().Initialize(this, null, ref vector6, ref direction, this.mDepth + 1, null);
									}
									if (this.mChild != null)
									{
										this.mChild.Position = vector6;
										this.mChild.Direction = direction;
									}
								}
							}
							else if (this.mChild != null)
							{
								this.mChild.Kill();
							}
						}
						else if (entity is Magicka.GameLogic.Entities.Character && (entity as Magicka.GameLogic.Entities.Character).CurrentSelfShieldType == Magicka.GameLogic.Entities.Character.SelfShieldType.Shield)
						{
							this.mDamageTimer = 0.25f;
							Magicka.GameLogic.Entities.Character character = entity as Magicka.GameLogic.Entities.Character;
							Vector3.Multiply(ref this.mDirection, this.mLength + 1f, out segment.Delta);
							float num13;
							if (character.Body.CollisionSkin.SegmentIntersect(out num13, out vector6, out vector5, segment))
							{
								if (num13 <= 1E-06f)
								{
									character.Body.CollisionSkin.SegmentIntersect(out num13, out vector6, out vector5, seg);
								}
							}
							else
							{
								Vector3 position2 = character.Position;
								Distance.PointSegmentDistanceSq(out num13, position2, segment);
								segment.GetPoint(num13, out vector6);
								Vector3.Subtract(ref vector6, ref position2, out vector5);
							}
							if (num13 > 1E-06f)
							{
								vector5.Y = 0f;
								Vector3 direction2;
								Vector3.Reflect(ref this.mDirection, ref vector5, out direction2);
								direction2.Normalize();
								float num5;
								Vector3.Dot(ref direction2, ref this.mDirection, out num5);
								if (num5 < -0.99f)
								{
									this.mWillExplode = true;
									this.mExplosionCompanion = null;
									this.mExplosionCountdown = 0.25f;
									this.GetFirstJunction(out this.mExplosionPosition);
									Railgun railgun7 = this;
									while (railgun7.mChild != null)
									{
										railgun7 = railgun7.mChild;
									}
									railgun7.LockAll();
								}
								else
								{
									if (this.mChild != null && this.mChild.mParents.Count > 1)
									{
										this.mChild.Kill();
										this.mChild = null;
									}
									if (this.mChild == null && this.mDepth < 6)
									{
										Railgun.GetFromCache().Initialize(this, null, ref vector6, ref direction2, this.mDepth + 1, null);
									}
									if (this.mChild != null)
									{
										this.mChild.Position = vector6;
										this.mChild.Direction = direction2;
									}
								}
							}
							else if (this.mChild != null)
							{
								this.mChild.Kill();
							}
						}
						else if (entity is Portal.PortalEntity)
						{
							this.mDamageTimer = 0.25f;
							Portal.PortalEntity portalEntity2 = Portal.OtherPortal(entity as Portal.PortalEntity);
							Vector3 direction3 = this.mDirection;
							Vector3 position3 = entity.Position;
							Vector3.Subtract(ref vector6, ref position3, out position3);
							Vector3 position4 = portalEntity2.Position;
							Vector3.Add(ref position4, ref position3, out position4);
							this.mLength += 0.5f;
							if (this.mChild != null && this.mChild.mParents.Count > 1)
							{
								this.mChild.Kill();
								this.mChild = null;
							}
							if (this.mChild == null && this.mDepth < 6)
							{
								Railgun.GetFromCache().Initialize(this, null, ref position4, ref direction3, this.mDepth + 1, portalEntity2);
							}
							if (this.mChild != null)
							{
								this.mChild.Position = position4;
								this.mChild.Direction = direction3;
							}
						}
						else if (this.mDamageTimer <= 0f && entity is IDamageable)
						{
							this.mDamageTimer += 0.25f;
							for (int num14 = 0; num14 < this.mDamages.Count; num14++)
							{
								DamageCollection5 iDamage = this.mDamages.Values[num14];
								iDamage.MultiplyMagnitude(0.25f);
								Entity fromHandle = Entity.GetFromHandle((int)this.mDamages.Keys[num14]);
								(entity as IDamageable).Damage(iDamage, fromHandle, this.mTimeStamp, iAttackPosition);
								Magicka.GameLogic.Entities.Character character2 = entity as Magicka.GameLogic.Entities.Character;
								if (character2 != null)
								{
									Damage* ptr = &iDamage.A;
									for (int num15 = 0; num15 < 5; num15++)
									{
										if ((ptr[num15].Element & Elements.Beams) != Elements.None)
										{
											character2.AccumulateArcaneDamage(ptr[num15].Element, ptr[num15].Magnitude);
											if ((entity as IDamageable).HitPoints <= 0f & !character2.Bloating)
											{
												character2.BloatKill(ptr[num15].Element, fromHandle);
											}
										}
									}
								}
							}
						}
						this.mDamageTimer -= iDeltaTime;
					}
				}
				else if (flag3)
				{
					this.mDamageTimer = 0.25f;
					Vector3.Multiply(ref this.mDirection, this.mLength + 1f, out segment.Delta);
					Vector3 direction4;
					Vector3.Reflect(ref this.mDirection, ref vector8, out direction4);
					direction4.Normalize();
					float num5;
					Vector3.Dot(ref direction4, ref this.mDirection, out num5);
					if (num5 < -0.99f)
					{
						this.mWillExplode = true;
						this.mExplosionCompanion = null;
						this.mExplosionCountdown = 0.25f;
						this.GetFirstJunction(out this.mExplosionPosition);
						Railgun railgun8 = this;
						while (railgun8.mChild != null)
						{
							railgun8 = railgun8.mChild;
						}
						railgun8.LockAll();
					}
					else
					{
						if (this.mChild != null && this.mChild.mParents.Count > 1)
						{
							this.mChild.Kill();
							this.mChild = null;
						}
						if (this.mChild == null && this.mDepth < 6)
						{
							Railgun.GetFromCache().Initialize(this, null, ref position, ref direction4, this.mDepth + 1, null);
						}
						if (this.mChild != null)
						{
							this.mChild.Position = position;
							this.mChild.Direction = direction4;
						}
					}
				}
				if (flag2)
				{
					Railgun.sEmitter.Position = vector6;
					Railgun.sEmitter.Forward = this.mDirection;
					if (this.mStageHitCue == null)
					{
						this.mStageHitCue = AudioManager.Instance.PlayCue(Banks.Spells, this.mHitSoundHash, Railgun.sEmitter);
					}
					else
					{
						this.mStageHitCue.Apply3D(playState.Camera.Listener, Railgun.sEmitter);
					}
				}
				else
				{
					Vector3.Multiply(ref this.mDirection, this.mLength, out vector6);
					Vector3.Add(ref this.mPosition, ref vector6, out vector6);
					if (this.mStageHitCue != null)
					{
						this.mStageHitCue.Stop(AudioStopOptions.AsAuthored);
						this.mStageHitCue = null;
					}
				}
				if (this.mDead)
				{
					this.mCut += iDeltaTime / this.mLength * 30f * 3f;
				}
				EffectManager.Instance.UpdatePositionDirection(ref this.mSourceEffect, ref this.mPosition, ref this.mDirection);
				EffectManager.Instance.UpdatePositionDirection(ref this.mHitEffect, ref vector6, ref this.mDirection);
				Vector3 start;
				Vector3.Lerp(ref this.mPosition, ref vector6, this.mCut, out start);
				this.mLight.Start = start;
				this.mLight.End = vector6;
			}
			if (this.mAdditionalElements == Elements.None)
			{
				Railgun.RenderData renderData = this.mRenderData[(int)iDataChannel];
				if (this.mParents.Count > 0)
				{
					renderData.Nr = 2;
					for (int num16 = 0; num16 < this.mDamages.Count; num16++)
					{
						renderData.Nr += 2;
					}
				}
				else
				{
					renderData.Nr = 4;
				}
				renderData.Time = this.mTime;
				renderData.Cut = this.mCut;
				renderData.Position = this.mPosition;
				renderData.Direction = this.mDirection;
				renderData.Length = this.mLength;
				renderData.Branch = (this.mParents.Count > 0);
				playState.Scene.AddRenderableAdditiveObject(iDataChannel, renderData);
				return;
			}
			Railgun.AdditionalRenderData additionalRenderData = this.mAdditionalRenderData[(int)iDataChannel];
			additionalRenderData.Time = this.mTime;
			additionalRenderData.Cut = this.mCut;
			additionalRenderData.Position = this.mPosition;
			additionalRenderData.Direction = this.mDirection;
			additionalRenderData.Length = this.mLength;
			additionalRenderData.Elements = this.mAdditionalElements;
			playState.Scene.AddRenderableAdditiveObject(iDataChannel, additionalRenderData);
		}

		// Token: 0x06001C71 RID: 7281 RVA: 0x000C5768 File Offset: 0x000C3968
		private void Explode(PlayState iPlayState)
		{
			Vector3.Lerp(ref this.mExplosionPosition, ref this.mPosition, 0.0001f, out this.mExplosionPosition);
			Railgun railgun = this;
			while (railgun.mChild != null)
			{
				railgun = railgun.mChild;
			}
			int num = this.mDamages.Count;
			if (this.mExplosionCompanion != null)
			{
				num += this.mExplosionCompanion.mDamages.Count;
			}
			float iRadius = 3f + 1f * (float)num;
			railgun.KillAll();
			bool flag = false;
			DamageResult damageResult;
			for (int i = 0; i < this.mDamages.Count; i++)
			{
				damageResult = Blast.FullBlast(iPlayState, Entity.GetFromHandle((int)this.mDamages.Keys[i]), this.mTimeStamp, null, iRadius, this.mExplosionPosition, this.mDamages.Values[i]);
				if ((damageResult & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
				{
					flag = true;
				}
			}
			if (this.mExplosionCompanion != null)
			{
				for (int j = 0; j < this.mExplosionCompanion.mDamages.Count; j++)
				{
					damageResult = Blast.FullBlast(iPlayState, Entity.GetFromHandle((int)this.mExplosionCompanion.mDamages.Keys[j]), this.mTimeStamp, null, iRadius, this.mExplosionPosition, this.mExplosionCompanion.mDamages.Values[j]);
					if ((damageResult & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
					{
						flag = true;
					}
				}
			}
			Damage iDamage;
			iDamage.AttackProperty = AttackProperties.Knockback;
			iDamage.Element = Elements.None;
			iDamage.Amount = 500f;
			iDamage.Magnitude = 2f;
			damageResult = Blast.FullBlast(iPlayState, null, this.mTimeStamp, null, 5f, this.mExplosionPosition, iDamage);
			if ((damageResult & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
			{
				flag = true;
			}
			if (this.mIsOposits)
			{
				bool flag2 = false;
				foreach (ushort num2 in this.mDamages.Keys)
				{
					for (int k = 0; k < Game.Instance.Players.Length; k++)
					{
						if (Game.Instance.Players[k].Playing && num2 == Game.Instance.Players[k].Avatar.Handle && !(Game.Instance.Players[k].Gamer is NetworkGamer))
						{
							flag2 = true;
						}
					}
				}
				if (flag && flag2)
				{
					AchievementsManager.Instance.AwardAchievement(iPlayState, "nevercrossthebeams");
				}
			}
		}

		// Token: 0x06001C72 RID: 7282 RVA: 0x000C59E4 File Offset: 0x000C3BE4
		private void GetFirstJunction(out Vector3 mExplosionPosition)
		{
			if (this.mParents.Count == 1)
			{
				this.mParents[0].GetFirstJunction(out mExplosionPosition);
				return;
			}
			mExplosionPosition = this.mPosition;
		}

		// Token: 0x06001C73 RID: 7283 RVA: 0x000C5A14 File Offset: 0x000C3C14
		private void LockAll()
		{
			this.mLocked = true;
			for (int i = 0; i < this.mParents.Count; i++)
			{
				this.mParents[i].LockAll();
			}
		}

		// Token: 0x06001C74 RID: 7284 RVA: 0x000C5A50 File Offset: 0x000C3C50
		private void KillAll()
		{
			this.mDead = true;
			this.mCut = 1f;
			if (this.mChild != null)
			{
				throw new Exception();
			}
			for (int i = 0; i < this.mParents.Count; i++)
			{
				this.mParents[i].mChild = null;
				this.mParents[i].KillAll();
			}
			this.mParents.Clear();
		}

		// Token: 0x06001C75 RID: 7285 RVA: 0x000C5AC4 File Offset: 0x000C3CC4
		public void OnRemove()
		{
			this.mLight.Disable();
			if (this.mImmaFirinMahLazer)
			{
				bool flag = false;
				PlayState iPlayState = null;
				foreach (ushort num in this.mDamages.Keys)
				{
					for (int i = 0; i < Game.Instance.Players.Length; i++)
					{
						if (Game.Instance.Players[i].Playing && num == Game.Instance.Players[i].Avatar.Handle && !(Game.Instance.Players[i].Gamer is NetworkGamer))
						{
							flag = true;
							iPlayState = Game.Instance.Players[i].Avatar.PlayState;
						}
					}
				}
				if (flag)
				{
					AchievementsManager.Instance.AwardAchievement(iPlayState, "immafirinmahlazer");
				}
			}
			if (this.mChild != null)
			{
				this.mChild.Kill();
				this.mChild = null;
			}
			for (int j = 0; j < this.mParents.Count; j++)
			{
				this.mParents[j].mChild = null;
			}
			this.mParents.Clear();
			EffectManager.Instance.Stop(ref this.mSourceEffect);
			EffectManager.Instance.Stop(ref this.mHitEffect);
			this.mDamages.Clear();
			if (this.mStageStartCue != null)
			{
				this.mStageStartCue.Stop(AudioStopOptions.AsAuthored);
			}
			if (this.mStageHitCue != null)
			{
				this.mStageHitCue.Stop(AudioStopOptions.AsAuthored);
			}
			for (int k = 0; k < this.mAdditionalElementCues.Count; k++)
			{
				if (this.mAdditionalElementCues[k] != null)
				{
					this.mAdditionalElementCues[k].Stop(AudioStopOptions.AsAuthored);
				}
			}
			this.mAdditionalElementCues.Clear();
			Railgun.sActiveRails.Remove(this);
			Railgun.sCache.Add(this);
		}

		// Token: 0x04001E90 RID: 7824
		private const float EXPLODE_COOLDOWN = 0.25f;

		// Token: 0x04001E91 RID: 7825
		private const int MAX_RAY_DEPTH = 6;

		// Token: 0x04001E92 RID: 7826
		public const int VERTEXCOUNT = 512;

		// Token: 0x04001E93 RID: 7827
		public const int PRIMITIVECOUNT = 510;

		// Token: 0x04001E94 RID: 7828
		private const float SPEED = 30f;

		// Token: 0x04001E95 RID: 7829
		private static List<Railgun> sCache;

		// Token: 0x04001E96 RID: 7830
		private static List<Railgun> sActiveRails;

		// Token: 0x04001E97 RID: 7831
		private static readonly int ARCANEHITEFFECTHASH = "arcane_hit".GetHashCodeCustom();

		// Token: 0x04001E98 RID: 7832
		private static readonly int ARCANESOURCEEFFECTHASH = "arcane_source".GetHashCodeCustom();

		// Token: 0x04001E99 RID: 7833
		private static readonly int LIFEHITEFFECTHASH = "life_hit".GetHashCodeCustom();

		// Token: 0x04001E9A RID: 7834
		private static readonly int LIFESOURCEEFFECTHASH = "life_source".GetHashCodeCustom();

		// Token: 0x04001E9B RID: 7835
		public static readonly int[] LIFESTAGESOUNDSHASH = new int[]
		{
			"spell_life_ray_stage1".GetHashCodeCustom(),
			"spell_life_ray_stage2".GetHashCodeCustom(),
			"spell_life_ray_stage3".GetHashCodeCustom(),
			"spell_life_ray_stage4".GetHashCodeCustom()
		};

		// Token: 0x04001E9C RID: 7836
		public static readonly int[] ARCANESTAGESOUNDSHASH = new int[]
		{
			"spell_arcane_ray_stage1".GetHashCodeCustom(),
			"spell_arcane_ray_stage2".GetHashCodeCustom(),
			"spell_arcane_ray_stage3".GetHashCodeCustom(),
			"spell_arcane_ray_stage4".GetHashCodeCustom()
		};

		// Token: 0x04001E9D RID: 7837
		private static Random sRandom = new Random();

		// Token: 0x04001E9E RID: 7838
		private static AudioEmitter sEmitter = new AudioEmitter();

		// Token: 0x04001E9F RID: 7839
		private static Texture2D sTexture;

		// Token: 0x04001EA0 RID: 7840
		private static VertexBuffer sVertexBuffer;

		// Token: 0x04001EA1 RID: 7841
		private static int sVertexBufferHash;

		// Token: 0x04001EA2 RID: 7842
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x04001EA3 RID: 7843
		private static VertexBuffer sAdditionalVertexBuffer;

		// Token: 0x04001EA4 RID: 7844
		private static int sAdditionalVertexBufferHash;

		// Token: 0x04001EA5 RID: 7845
		private static VertexDeclaration sAdditionalVertexDeclaration;

		// Token: 0x04001EA6 RID: 7846
		public static readonly Vector2[] ELEMENT_OFFSET_LOOKUP = new Vector2[]
		{
			default(Vector2),
			new Vector2(0f, 0f),
			new Vector2(0f, 0.125f),
			new Vector2(0f, 0.25f),
			new Vector2(0f, 0.375f),
			new Vector2(0f, 0.5f),
			new Vector2(0f, 0.625f),
			default(Vector2),
			default(Vector2),
			new Vector2(0f, 0.75f),
			new Vector2(0f, 0.875f)
		};

		// Token: 0x04001EA7 RID: 7847
		private float mTime;

		// Token: 0x04001EA8 RID: 7848
		private float mLength;

		// Token: 0x04001EA9 RID: 7849
		private Railgun.RenderData[] mRenderData;

		// Token: 0x04001EAA RID: 7850
		private Railgun.AdditionalRenderData[] mAdditionalRenderData;

		// Token: 0x04001EAB RID: 7851
		private Vector3 mPosition;

		// Token: 0x04001EAC RID: 7852
		private Vector3 mDirection;

		// Token: 0x04001EAD RID: 7853
		private Railgun mChild;

		// Token: 0x04001EAE RID: 7854
		private List<Railgun> mParents = new List<Railgun>(8);

		// Token: 0x04001EAF RID: 7855
		private Vector3 mColor;

		// Token: 0x04001EB0 RID: 7856
		private bool mDead;

		// Token: 0x04001EB1 RID: 7857
		private float mCut;

		// Token: 0x04001EB2 RID: 7858
		private VisualEffectReference mSourceEffect;

		// Token: 0x04001EB3 RID: 7859
		private VisualEffectReference mHitEffect;

		// Token: 0x04001EB4 RID: 7860
		private SortedList<ushort, DamageCollection5> mDamages = new SortedList<ushort, DamageCollection5>(4);

		// Token: 0x04001EB5 RID: 7861
		private bool mArcane;

		// Token: 0x04001EB6 RID: 7862
		private Cue mStageStartCue;

		// Token: 0x04001EB7 RID: 7863
		private Cue mStageHitCue;

		// Token: 0x04001EB8 RID: 7864
		private List<Cue> mAdditionalElementCues;

		// Token: 0x04001EB9 RID: 7865
		private float mDamageTimer;

		// Token: 0x04001EBA RID: 7866
		private int mHitSoundHash;

		// Token: 0x04001EBB RID: 7867
		private CapsuleLight mLight;

		// Token: 0x04001EBC RID: 7868
		private Elements mAdditionalElements;

		// Token: 0x04001EBD RID: 7869
		private Elements mAllCombinedElements;

		// Token: 0x04001EBE RID: 7870
		private bool mImmaFirinMahLazer;

		// Token: 0x04001EBF RID: 7871
		private bool mIsOposits;

		// Token: 0x04001EC0 RID: 7872
		private bool mWillExplode;

		// Token: 0x04001EC1 RID: 7873
		private bool mLocked;

		// Token: 0x04001EC2 RID: 7874
		private Railgun mExplosionCompanion;

		// Token: 0x04001EC3 RID: 7875
		private float mExplosionCountdown;

		// Token: 0x04001EC4 RID: 7876
		private Vector3 mExplosionPosition;

		// Token: 0x04001EC5 RID: 7877
		private double mTimeStamp;

		// Token: 0x04001EC6 RID: 7878
		private int mDepth;

		// Token: 0x04001EC7 RID: 7879
		private Portal.PortalEntity mOutPortal;

		// Token: 0x020003A0 RID: 928
		protected class RenderData : IRenderableAdditiveObject
		{
			// Token: 0x170006FE RID: 1790
			// (get) Token: 0x06001C77 RID: 7287 RVA: 0x000C5EC4 File Offset: 0x000C40C4
			int IRenderableAdditiveObject.Effect
			{
				get
				{
					return ArcaneEffect.TYPEHASH;
				}
			}

			// Token: 0x170006FF RID: 1791
			// (get) Token: 0x06001C78 RID: 7288 RVA: 0x000C5ECB File Offset: 0x000C40CB
			int IRenderableAdditiveObject.Technique
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x17000700 RID: 1792
			// (get) Token: 0x06001C79 RID: 7289 RVA: 0x000C5ECE File Offset: 0x000C40CE
			VertexBuffer IRenderableAdditiveObject.Vertices
			{
				get
				{
					return Railgun.sVertexBuffer;
				}
			}

			// Token: 0x17000701 RID: 1793
			// (get) Token: 0x06001C7A RID: 7290 RVA: 0x000C5ED5 File Offset: 0x000C40D5
			int IRenderableAdditiveObject.VerticesHashCode
			{
				get
				{
					return Railgun.sVertexBufferHash;
				}
			}

			// Token: 0x17000702 RID: 1794
			// (get) Token: 0x06001C7B RID: 7291 RVA: 0x000C5EDC File Offset: 0x000C40DC
			int IRenderableAdditiveObject.VertexStride
			{
				get
				{
					return 16;
				}
			}

			// Token: 0x17000703 RID: 1795
			// (get) Token: 0x06001C7C RID: 7292 RVA: 0x000C5EE0 File Offset: 0x000C40E0
			IndexBuffer IRenderableAdditiveObject.Indices
			{
				get
				{
					return null;
				}
			}

			// Token: 0x17000704 RID: 1796
			// (get) Token: 0x06001C7D RID: 7293 RVA: 0x000C5EE3 File Offset: 0x000C40E3
			VertexDeclaration IRenderableAdditiveObject.VertexDeclaration
			{
				get
				{
					return Railgun.sVertexDeclaration;
				}
			}

			// Token: 0x06001C7E RID: 7294 RVA: 0x000C5EEA File Offset: 0x000C40EA
			bool IRenderableAdditiveObject.Cull(BoundingFrustum iViewFrustum)
			{
				return false;
			}

			// Token: 0x06001C7F RID: 7295 RVA: 0x000C5EF0 File Offset: 0x000C40F0
			void IRenderableAdditiveObject.Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				ArcaneEffect arcaneEffect = iEffect as ArcaneEffect;
				arcaneEffect.Origin = this.Position;
				arcaneEffect.Direction = this.Direction;
				arcaneEffect.Length = this.Length;
				arcaneEffect.ColorCenter = this.ColorCenter;
				arcaneEffect.ColorEdge = this.ColorEdge;
				arcaneEffect.Cut = this.Cut;
				arcaneEffect.Alpha = 1f;
				arcaneEffect.StartLength = 4f;
				arcaneEffect.Dropoff = 0.666f;
				arcaneEffect.MinRadius = 0f;
				arcaneEffect.MaxRadius = 0f;
				arcaneEffect.RayRadius = 0.333f;
				arcaneEffect.Texture = Railgun.sTexture;
				arcaneEffect.Time = this.Time;
				arcaneEffect.TextureScale = 0.1f;
				arcaneEffect.CommitChanges();
				arcaneEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 0, 510);
				float num = 0.5f;
				for (int i = 0; i < this.Nr; i++)
				{
					float num2 = (float)Math.Sqrt((double)num);
					arcaneEffect.Time = (this.Time + num * 10f) * num2;
					arcaneEffect.Clockwice = (i % 2 == 0);
					arcaneEffect.WaveScale = 4f * num2;
					arcaneEffect.MinRadius = 0.15f * num2;
					if (this.Branch)
					{
						arcaneEffect.MaxRadius = 0.15f * num2;
					}
					else
					{
						arcaneEffect.MaxRadius = 1.25f * num2;
					}
					arcaneEffect.RayRadius = 0.1f / num2;
					arcaneEffect.CommitChanges();
					arcaneEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 0, 510);
					num += 0.25f;
				}
			}

			// Token: 0x04001EC8 RID: 7880
			public Vector3 Position;

			// Token: 0x04001EC9 RID: 7881
			public Vector3 Direction;

			// Token: 0x04001ECA RID: 7882
			public float Length;

			// Token: 0x04001ECB RID: 7883
			public Vector3 ColorCenter;

			// Token: 0x04001ECC RID: 7884
			public Vector3 ColorEdge;

			// Token: 0x04001ECD RID: 7885
			public float Cut;

			// Token: 0x04001ECE RID: 7886
			public float Time;

			// Token: 0x04001ECF RID: 7887
			public int Nr;

			// Token: 0x04001ED0 RID: 7888
			public bool Branch;
		}

		// Token: 0x020003A1 RID: 929
		protected class AdditionalRenderData : IRenderableAdditiveObject, IPreRenderRenderer
		{
			// Token: 0x17000705 RID: 1797
			// (get) Token: 0x06001C81 RID: 7297 RVA: 0x000C6083 File Offset: 0x000C4283
			public int Effect
			{
				get
				{
					return AdditiveEffect.TYPEHASH;
				}
			}

			// Token: 0x17000706 RID: 1798
			// (get) Token: 0x06001C82 RID: 7298 RVA: 0x000C608A File Offset: 0x000C428A
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x17000707 RID: 1799
			// (get) Token: 0x06001C83 RID: 7299 RVA: 0x000C608D File Offset: 0x000C428D
			public VertexBuffer Vertices
			{
				get
				{
					return Railgun.sAdditionalVertexBuffer;
				}
			}

			// Token: 0x17000708 RID: 1800
			// (get) Token: 0x06001C84 RID: 7300 RVA: 0x000C6094 File Offset: 0x000C4294
			public int VerticesHashCode
			{
				get
				{
					return Railgun.sAdditionalVertexBufferHash;
				}
			}

			// Token: 0x17000709 RID: 1801
			// (get) Token: 0x06001C85 RID: 7301 RVA: 0x000C609B File Offset: 0x000C429B
			public int VertexStride
			{
				get
				{
					return 16;
				}
			}

			// Token: 0x1700070A RID: 1802
			// (get) Token: 0x06001C86 RID: 7302 RVA: 0x000C609F File Offset: 0x000C429F
			public IndexBuffer Indices
			{
				get
				{
					return null;
				}
			}

			// Token: 0x1700070B RID: 1803
			// (get) Token: 0x06001C87 RID: 7303 RVA: 0x000C60A2 File Offset: 0x000C42A2
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return Railgun.sAdditionalVertexDeclaration;
				}
			}

			// Token: 0x06001C88 RID: 7304 RVA: 0x000C60A9 File Offset: 0x000C42A9
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return false;
			}

			// Token: 0x06001C89 RID: 7305 RVA: 0x000C60AC File Offset: 0x000C42AC
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
				additiveEffect.VertexColorEnabled = false;
				additiveEffect.Texture = Railgun.sTexture;
				additiveEffect.TextureEnabled = true;
				Vector4 colorTint = default(Vector4);
				colorTint.W = 1f;
				Vector2 textureScale = new Vector2
				{
					X = this.Length * 0.0333f,
					Y = 1f
				};
				additiveEffect.TextureScale = textureScale;
				for (int i = 0; i < 11; i++)
				{
					Elements elements = (Elements)(1 << i);
					if ((elements & this.Elements) == elements)
					{
						if (elements == Elements.Lightning)
						{
							colorTint.X = Spell.LIGHTNINGCOLOR.X * 2f;
							colorTint.Y = Spell.LIGHTNINGCOLOR.Y * 2f;
							colorTint.Z = Spell.LIGHTNINGCOLOR.Z * 2f;
						}
						else
						{
							colorTint.X = (colorTint.Y = (colorTint.Z = 2f));
						}
						additiveEffect.ColorTint = colorTint;
						additiveEffect.World = this.mTransform;
						textureScale.Y = 1f;
						additiveEffect.TextureScale = textureScale;
						Vector2 textureOffset = Railgun.ELEMENT_OFFSET_LOOKUP[i];
						textureOffset.X = this.Time * -0.05f + this.Cut;
						additiveEffect.TextureOffset = textureOffset;
						additiveEffect.CommitChanges();
						additiveEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
						textureOffset.X = textureOffset.X * 1.3256675f + 0.3546517f;
						textureOffset.Y += 0.125f;
						additiveEffect.TextureOffset = textureOffset;
						textureScale.Y = -textureScale.Y;
						additiveEffect.TextureScale = textureScale;
						Matrix world = this.mTransform;
						world.M11 *= 0.666f;
						world.M12 *= 0.666f;
						world.M13 *= 0.666f;
						additiveEffect.World = world;
						additiveEffect.CommitChanges();
						additiveEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
					}
				}
			}

			// Token: 0x06001C8A RID: 7306 RVA: 0x000C62D0 File Offset: 0x000C44D0
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				Matrix.CreateConstrainedBillboard(ref this.Position, ref iCameraPosition, ref this.Direction, new Vector3?(iCameraDirection), null, out this.mTransform);
				float num = this.Length * (1f - this.Cut);
				Vector3 up = this.mTransform.Up;
				Vector3.Multiply(ref up, this.Length * this.Cut, out up);
				Vector3.Add(ref up, ref this.Position, out up);
				this.mTransform.Translation = up;
				this.mTransform.M21 = this.mTransform.M21 * num;
				this.mTransform.M22 = this.mTransform.M22 * num;
				this.mTransform.M23 = this.mTransform.M23 * num;
			}

			// Token: 0x04001ED1 RID: 7889
			public Vector3 Position;

			// Token: 0x04001ED2 RID: 7890
			public Vector3 Direction;

			// Token: 0x04001ED3 RID: 7891
			public float Length;

			// Token: 0x04001ED4 RID: 7892
			public float Cut;

			// Token: 0x04001ED5 RID: 7893
			public float Time;

			// Token: 0x04001ED6 RID: 7894
			public Elements Elements;

			// Token: 0x04001ED7 RID: 7895
			private Matrix mTransform;
		}

		// Token: 0x020003A2 RID: 930
		public enum RailStage
		{
			// Token: 0x04001ED9 RID: 7897
			Start,
			// Token: 0x04001EDA RID: 7898
			Hit,
			// Token: 0x04001EDB RID: 7899
			Bloat,
			// Token: 0x04001EDC RID: 7900
			Explode
		}
	}
}
