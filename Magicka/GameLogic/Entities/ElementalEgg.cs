using System;
using System.Collections.Generic;
using System.IO;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020003B5 RID: 949
	internal class ElementalEgg : Entity, IStatusEffected, IDamageable
	{
		// Token: 0x06001D3A RID: 7482 RVA: 0x000CF4D0 File Offset: 0x000CD6D0
		public static ElementalEgg GetInstance(PlayState iPlayState)
		{
			if (ElementalEgg.sCache.Count > 0)
			{
				ElementalEgg elementalEgg = ElementalEgg.sCache[0];
				ElementalEgg.sCache.RemoveAt(0);
				ElementalEgg.sCache.Add(elementalEgg);
				return elementalEgg;
			}
			throw new Exception("Cache was not initialized!");
		}

		// Token: 0x06001D3B RID: 7483 RVA: 0x000CF518 File Offset: 0x000CD718
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			ElementalEgg.sTemplateLookup = new Dictionary<Elements, CharacterTemplate>(8);
			for (int i = 0; i < 11; i++)
			{
				Elements elements = Spell.ElementFromIndex(i);
				if ((elements & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Arcane | Elements.Life | Elements.Steam | Elements.Poison)) == elements)
				{
					ElementalEgg.sTemplateLookup.Add(elements, iPlayState.Content.Load<CharacterTemplate>(string.Format("Data/Characters/Elemental_{0}", elements)));
				}
			}
			ElementalEgg.sCache = new List<ElementalEgg>(iNr);
			for (int j = 0; j < iNr; j++)
			{
				ElementalEgg.sCache.Add(new ElementalEgg(iPlayState));
			}
		}

		// Token: 0x06001D3C RID: 7484 RVA: 0x000CF59C File Offset: 0x000CD79C
		private ElementalEgg(PlayState iPlayState) : base(iPlayState)
		{
			SkinnedModel skinnedModel = null;
			lock (Game.Instance.GraphicsDevice)
			{
				this.mModel = iPlayState.Content.Load<SkinnedModel>("Models/Characters/Elemental/elemental_mesh");
				skinnedModel = iPlayState.Content.Load<SkinnedModel>("Models/Characters/Elemental/elemental_animation");
			}
			this.mBody = new Body();
			this.mCollision = new CollisionSkin(this.mBody);
			this.mBody.CollisionSkin = this.mCollision;
			this.mBody.AllowFreezing = false;
			this.mBody.ApplyGravity = false;
			this.mBody.Tag = this;
			this.mCollision.callbackFn += this.OnCollision;
			this.mCollision.postCollisionCallbackFn += this.PostCollision;
			this.mScale = ElementalEgg.sTemplateLookup[Elements.Fire].Models[0].Scale;
			this.mRadius = ElementalEgg.sTemplateLookup[Elements.Fire].Radius;
			float length = ElementalEgg.sTemplateLookup[Elements.Fire].Length;
			this.mHeightOffset = ElementalEgg.sTemplateLookup[Elements.Fire].Length * 0.5f + ElementalEgg.sTemplateLookup[Elements.Fire].Radius;
			this.mCollision.AddPrimitive(new Capsule(default(Vector3), Matrix.CreateRotationX(-1.5707964f), this.mRadius, length), 1, new MaterialProperties(0f, 0f, 0f));
			this.mController = new AnimationController();
			this.mController.Skeleton = this.mModel.SkeletonBones;
			this.mClip = skinnedModel.AnimationClips["spawn"];
			SkinnedModelDeferredBasicMaterial mMaterial;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out mMaterial);
			this.mRenderData = new ElementalEgg.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new ElementalEgg.RenderData();
				this.mRenderData[i].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
				this.mRenderData[i].mMaterial = mMaterial;
			}
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x000CF85C File Offset: 0x000CDA5C
		public float ResistanceAgainst(Elements iElement)
		{
			return 0f;
		}

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x06001D3E RID: 7486 RVA: 0x000CF863 File Offset: 0x000CDA63
		// (set) Token: 0x06001D3F RID: 7487 RVA: 0x000CF86B File Offset: 0x000CDA6B
		public float Proximity
		{
			get
			{
				return this.mProximity;
			}
			set
			{
				this.mProximity = value;
			}
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x000CF874 File Offset: 0x000CDA74
		private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			return false;
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x000CF877 File Offset: 0x000CDA77
		private void PostCollision(ref CollisionInfo iInfo)
		{
			if (iInfo.SkinInfo.Skin0 == this.mCollision)
			{
				iInfo.SkinInfo.IgnoreSkin0 = true;
				return;
			}
			iInfo.SkinInfo.IgnoreSkin1 = true;
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x000CF8A8 File Offset: 0x000CDAA8
		public void Initialize(ref Vector3 iPosition, ref Vector3 iDirection, int iUniqueID)
		{
			base.Initialize(iUniqueID);
			this.mHitpoints = 1f;
			this.mMaxHitPoints = 1f;
			Segment iSeg = new Segment(iPosition, new Vector3(0f, -10f, 0f));
			iSeg.Origin.Y = iSeg.Origin.Y + 1f;
			float num;
			Vector3 pos;
			Vector3 vector;
			if (!this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out pos, out vector, iSeg))
			{
				pos = iPosition;
			}
			pos.Y += this.mRadius;
			Vector3 up = Vector3.Up;
			Vector3 right;
			Vector3.Cross(ref iDirection, ref up, out right);
			Matrix orientation = default(Matrix);
			orientation.Forward = iDirection;
			orientation.Up = up;
			orientation.Right = right;
			this.mBody.MoveTo(pos, orientation);
			this.mDamageMemory.Clear();
			this.mSpawnElement = Elements.None;
			this.mSummoner = null;
			this.mProximityTimer = 0f;
			this.mController.StartClip(this.mClip, true);
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x000CF9C0 File Offset: 0x000CDBC0
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			Matrix orientation = this.mBody.Transform.Orientation;
			Matrix.Multiply(ref orientation, this.mScale, out orientation);
			Vector3 position = this.mBody.Transform.Position;
			orientation.Translation = position;
			this.mController.Update(iDeltaTime, ref orientation, true);
			this.mController.SkinnedBoneTransforms.CopyTo(this.mRenderData[(int)iDataChannel].mBones, 0);
			this.mProximityTimer += iDeltaTime;
			this.mSpawnElement = Elements.None;
			float num = float.MinValue;
			foreach (KeyValuePair<Elements, float> keyValuePair in this.mDamageMemory)
			{
				if (keyValuePair.Value > num)
				{
					this.mSpawnElement = keyValuePair.Key;
					num = keyValuePair.Value;
				}
			}
			this.mRenderData[(int)iDataChannel].mBoundingSphere.Center = orientation.Translation;
			this.mRenderData[(int)iDataChannel].mBoundingSphere.Radius = this.mRadius;
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			if (this.mBody.Velocity.LengthSquared() > 1E-06f)
			{
				this.mRestingTimer = 1f;
			}
			else
			{
				this.mRestingTimer -= iDeltaTime;
			}
			if (this.mProximity > 0f & this.mProximityTimer > 1f)
			{
				this.mProximityTimer -= 1f;
				List<Entity> entities = this.mPlayState.EntityManager.GetEntities(position, this.mProximity, false, false);
				for (int i = 0; i < entities.Count; i++)
				{
					if (entities[i] is Avatar)
					{
						Vector3 position2 = entities[i].Position;
						float num2;
						Vector3.DistanceSquared(ref position, ref position2, out num2);
						if (num2 <= this.mProximity * this.mProximity)
						{
							this.mSpawnElement = ElementalEgg.RANDOM_LOOKUP[ElementalEgg.sRandom.Next(ElementalEgg.RANDOM_LOOKUP.Length)];
							break;
						}
					}
				}
				this.mPlayState.EntityManager.ReturnEntityList(entities);
			}
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x000CFC08 File Offset: 0x000CDE08
		public void SetSummoned(ISpellCaster iCharacter)
		{
			this.mSummoner = iCharacter;
		}

		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x06001D45 RID: 7493 RVA: 0x000CFC11 File Offset: 0x000CDE11
		public bool IsSummoned
		{
			get
			{
				return this.mSummoner != null;
			}
		}

		// Token: 0x06001D46 RID: 7494 RVA: 0x000CFC1F File Offset: 0x000CDE1F
		public override void Deinitialize()
		{
			base.Deinitialize();
			if ((this.mSpawnElement & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Arcane | Elements.Life | Elements.Steam | Elements.Poison)) != Elements.None)
			{
				this.Spawn();
			}
		}

		// Token: 0x06001D47 RID: 7495 RVA: 0x000CFC3C File Offset: 0x000CDE3C
		private void Spawn()
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				Vector3 position = this.Position;
				Matrix orientation = this.mBody.Orientation;
				CharacterTemplate iTemplate = ElementalEgg.sTemplateLookup[this.mSpawnElement];
				NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
				instance.Initialize(iTemplate, position, 0, 0f);
				instance.CharacterBody.Orientation = orientation;
				instance.CharacterBody.DesiredDirection = orientation.Forward;
				instance.AI.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, -1, -1, 0, null);
				instance.SpawnAnimation = Animations.special0;
				instance.ChangeState(RessurectionState.Instance);
				if (this.mSummoner is Character)
				{
					instance.Summoned(this.mSummoner as Character);
				}
				this.mPlayState.EntityManager.AddEntity(instance);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
					triggerActionMessage.Handle = instance.Handle;
					if (this.mSummoner != null)
					{
						triggerActionMessage.Scene = (int)this.mSummoner.Handle;
					}
					triggerActionMessage.Template = instance.Type;
					triggerActionMessage.Position = instance.Position;
					triggerActionMessage.Direction = orientation.Forward;
					triggerActionMessage.Point1 = 170;
					triggerActionMessage.Point2 = 170;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
		}

		// Token: 0x06001D48 RID: 7496 RVA: 0x000CFDA5 File Offset: 0x000CDFA5
		public bool HasStatus(StatusEffects iStatus)
		{
			return false;
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x000CFDA8 File Offset: 0x000CDFA8
		public StatusEffect[] GetStatusEffects()
		{
			return null;
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x000CFDAB File Offset: 0x000CDFAB
		public float StatusMagnitude(StatusEffects iStatus)
		{
			return 0f;
		}

		// Token: 0x06001D4B RID: 7499 RVA: 0x000CFDB2 File Offset: 0x000CDFB2
		public void Damage(float iDamage, Elements iElement)
		{
		}

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x06001D4C RID: 7500 RVA: 0x000CFDB4 File Offset: 0x000CDFB4
		public float Volume
		{
			get
			{
				return this.mCollision.GetPrimitiveLocal(0).GetVolume();
			}
		}

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x06001D4D RID: 7501 RVA: 0x000CFDC7 File Offset: 0x000CDFC7
		public float HitPoints
		{
			get
			{
				return this.mHitpoints;
			}
		}

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x06001D4E RID: 7502 RVA: 0x000CFDCF File Offset: 0x000CDFCF
		public float MaxHitPoints
		{
			get
			{
				return this.mMaxHitPoints;
			}
		}

		// Token: 0x06001D4F RID: 7503 RVA: 0x000CFDD8 File Offset: 0x000CDFD8
		public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			Segment seg = default(Segment);
			seg.Origin = (this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Position;
			seg.Delta.Y = (this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Length;
			float num2;
			float t;
			float num = Distance.SegmentSegmentDistanceSq(out num2, out t, seg, iSeg);
			iSeg.GetPoint(t, out oPosition);
			float num3 = iSegmentRadius + this.mRadius;
			return num <= num3 * num3;
		}

		// Token: 0x06001D50 RID: 7504 RVA: 0x000CFE58 File Offset: 0x000CE058
		public DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			damageResult |= this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			return damageResult | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x000CFED8 File Offset: 0x000CE0D8
		public DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if ((iDamage.Element & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Arcane | Elements.Life | Elements.Steam | Elements.Poison)) != Elements.None)
			{
				int num = Helper.CountSetBits((uint)iDamage.Element);
				float value = iDamage.Amount * iDamage.Magnitude;
				if (num > 1)
				{
					for (int i = 0; i < 11; i++)
					{
						Elements elements = Spell.ElementFromIndex(i);
						if ((elements & iDamage.Element) == elements && (elements & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Arcane | Elements.Life | Elements.Steam | Elements.Poison)) == elements)
						{
							this.mDamageMemory[elements] = value;
						}
					}
				}
				else
				{
					this.mDamageMemory[iDamage.Element] = value;
				}
				return DamageResult.Hit;
			}
			return DamageResult.None;
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x000CFF64 File Offset: 0x000CE164
		public void OverKill()
		{
			this.Kill();
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x000CFF6C File Offset: 0x000CE16C
		public void Electrocute(IDamageable iTarget, float iMultiplyer)
		{
		}

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x06001D54 RID: 7508 RVA: 0x000CFF6E File Offset: 0x000CE16E
		public override bool Dead
		{
			get
			{
				return this.mSpawnElement != Elements.None;
			}
		}

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x06001D55 RID: 7509 RVA: 0x000CFF7C File Offset: 0x000CE17C
		public override bool Removable
		{
			get
			{
				return this.mSpawnElement != Elements.None;
			}
		}

		// Token: 0x06001D56 RID: 7510 RVA: 0x000CFF8A File Offset: 0x000CE18A
		public override void Kill()
		{
			if (this.mSpawnElement == Elements.None)
			{
				this.mSpawnElement = Elements.Shield;
			}
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x06001D57 RID: 7511 RVA: 0x000CFF9F File Offset: 0x000CE19F
		public bool Resting
		{
			get
			{
				return this.mRestingTimer < 0f;
			}
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x000CFFAE File Offset: 0x000CE1AE
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			if (!this.Resting)
			{
				oMsg.Features |= EntityFeatures.Position;
				oMsg.Position = this.Position;
			}
		}

		// Token: 0x06001D59 RID: 7513 RVA: 0x000CFFDA File Offset: 0x000CE1DA
		internal override float GetDanger()
		{
			return 0f;
		}

		// Token: 0x04001FD3 RID: 8147
		private const Elements ACCEPTABLE_ELEMENTS = Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Arcane | Elements.Life | Elements.Steam | Elements.Poison;

		// Token: 0x04001FD4 RID: 8148
		private static List<ElementalEgg> sCache;

		// Token: 0x04001FD5 RID: 8149
		private float mHitpoints;

		// Token: 0x04001FD6 RID: 8150
		private float mMaxHitPoints;

		// Token: 0x04001FD7 RID: 8151
		private static readonly Elements[] RANDOM_LOOKUP = new Elements[]
		{
			Elements.Arcane,
			Elements.Cold,
			Elements.Poison,
			Elements.Fire,
			Elements.Life,
			Elements.Water,
			Elements.Steam,
			Elements.Lightning
		};

		// Token: 0x04001FD8 RID: 8152
		private Dictionary<Elements, float> mDamageMemory = new Dictionary<Elements, float>(1);

		// Token: 0x04001FD9 RID: 8153
		private static readonly Random sRandom = new Random();

		// Token: 0x04001FDA RID: 8154
		private ElementalEgg.RenderData[] mRenderData;

		// Token: 0x04001FDB RID: 8155
		private SkinnedModel mModel;

		// Token: 0x04001FDC RID: 8156
		private AnimationController mController;

		// Token: 0x04001FDD RID: 8157
		private AnimationClip mClip;

		// Token: 0x04001FDE RID: 8158
		private Elements mSpawnElement;

		// Token: 0x04001FDF RID: 8159
		private static Dictionary<Elements, CharacterTemplate> sTemplateLookup;

		// Token: 0x04001FE0 RID: 8160
		private ISpellCaster mSummoner;

		// Token: 0x04001FE1 RID: 8161
		private float mProximity;

		// Token: 0x04001FE2 RID: 8162
		private float mProximityTimer;

		// Token: 0x04001FE3 RID: 8163
		private float mHeightOffset;

		// Token: 0x04001FE4 RID: 8164
		private float mScale;

		// Token: 0x04001FE5 RID: 8165
		protected float mRestingTimer = 1f;

		// Token: 0x020003B6 RID: 950
		public struct State
		{
			// Token: 0x06001D5B RID: 7515 RVA: 0x000D0033 File Offset: 0x000CE233
			public State(BinaryReader iReader)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001D5C RID: 7516 RVA: 0x000D003A File Offset: 0x000CE23A
			public State(ElementalEgg iEgg)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001D5D RID: 7517 RVA: 0x000D0041 File Offset: 0x000CE241
			public void ApplyTo(ElementalEgg iEgg)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001D5E RID: 7518 RVA: 0x000D0048 File Offset: 0x000CE248
			public void Write(BinaryWriter iWriter)
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x020003B7 RID: 951
		private class RenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
		{
			// Token: 0x06001D5F RID: 7519 RVA: 0x000D004F File Offset: 0x000CE24F
			public RenderData()
			{
				this.mBones = new Matrix[80];
			}

			// Token: 0x06001D60 RID: 7520 RVA: 0x000D0064 File Offset: 0x000CE264
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mBones;
				base.Draw(iEffect, iViewFrustum);
			}

			// Token: 0x06001D61 RID: 7521 RVA: 0x000D008C File Offset: 0x000CE28C
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mBones;
				base.DrawShadow(iEffect, iViewFrustum);
			}

			// Token: 0x04001FE6 RID: 8166
			public Matrix[] mBones;
		}
	}
}
