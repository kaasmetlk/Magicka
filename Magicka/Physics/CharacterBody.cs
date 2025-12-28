using System;
using JigLibX.Collision;
using JigLibX.Physics;
using Magicka.Achievements;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Levels;
using Microsoft.Xna.Framework;

namespace Magicka.Physics
{
	// Token: 0x02000622 RID: 1570
	internal class CharacterBody : Body
	{
		// Token: 0x06002EF2 RID: 12018 RVA: 0x0017CFAC File Offset: 0x0017B1AC
		public CharacterBody(Character iCharacter)
		{
			base.AllowFreezing = false;
			this.mOwner = new WeakReference(iCharacter);
			this.bodyInertia = default(Matrix);
			this.bodyInvInertia = default(Matrix);
		}

		// Token: 0x17000B0A RID: 2826
		// (get) Token: 0x06002EF3 RID: 12019 RVA: 0x0017D00E File Offset: 0x0017B20E
		// (set) Token: 0x06002EF4 RID: 12020 RVA: 0x0017D016 File Offset: 0x0017B216
		public Vector3 AdditionalForce
		{
			get
			{
				return this.mAdditionalForce;
			}
			set
			{
				if (!this.Owner.IsEntangled && this.mSpeedmultiplier > 1E-06f)
				{
					this.mAdditionalForce = value;
				}
			}
		}

		// Token: 0x06002EF5 RID: 12021 RVA: 0x0017D03C File Offset: 0x0017B23C
		public void AddImpulse(Vector3 iImpulse)
		{
			if (!this.Owner.IsEntangled)
			{
				Vector3.Multiply(ref iImpulse, base.InverseMass, out iImpulse);
				Vector3.Add(ref this.mImpulse, ref iImpulse, out this.mImpulse);
				this.mIsPushed = true;
				if (this.mImpulse.Y > 0.5f)
				{
					this.mIsLeavingGround = true;
				}
			}
		}

		// Token: 0x06002EF6 RID: 12022 RVA: 0x0017D098 File Offset: 0x0017B298
		public void AddJump(Vector3 iImpulse)
		{
			if (!this.Owner.IsEntangled)
			{
				if (!this.mIsTouchingGround | this.mIsLeavingGround | !this.mAllowMove)
				{
					return;
				}
				Vector3.Add(ref this.mImpulse, ref iImpulse, out this.mImpulse);
				this.mIsJumping = true;
				this.mIsLeavingGround = true;
				base.Velocity = default(Vector3);
			}
		}

		// Token: 0x06002EF7 RID: 12023 RVA: 0x0017D100 File Offset: 0x0017B300
		public void AddImpulseVelocity(ref Vector3 iImpulse)
		{
			if (!this.Owner.IsEntangled)
			{
				Vector3.Add(ref this.mImpulse, ref iImpulse, out this.mImpulse);
				this.mIsPushed = true;
				if (this.mImpulse.Y > 2.7f)
				{
					this.mIsLeavingGround = true;
				}
			}
		}

		// Token: 0x17000B0B RID: 2827
		// (get) Token: 0x06002EF8 RID: 12024 RVA: 0x0017D14C File Offset: 0x0017B34C
		// (set) Token: 0x06002EF9 RID: 12025 RVA: 0x0017D154 File Offset: 0x0017B354
		public bool RunBackward
		{
			get
			{
				return this.mRunBackwards;
			}
			set
			{
				this.mRunBackwards = value;
			}
		}

		// Token: 0x17000B0C RID: 2828
		// (get) Token: 0x06002EFA RID: 12026 RVA: 0x0017D15D File Offset: 0x0017B35D
		// (set) Token: 0x06002EFB RID: 12027 RVA: 0x0017D165 File Offset: 0x0017B365
		public bool IsOnGrease
		{
			get
			{
				return this.mIsOnGrease;
			}
			set
			{
				this.mIsOnGrease = value;
			}
		}

		// Token: 0x17000B0D RID: 2829
		// (get) Token: 0x06002EFC RID: 12028 RVA: 0x0017D16E File Offset: 0x0017B36E
		// (set) Token: 0x06002EFD RID: 12029 RVA: 0x0017D176 File Offset: 0x0017B376
		public bool IsFlying
		{
			get
			{
				return this.mIsFlying;
			}
			set
			{
				this.mIsFlying = value;
			}
		}

		// Token: 0x17000B0E RID: 2830
		// (get) Token: 0x06002EFE RID: 12030 RVA: 0x0017D17F File Offset: 0x0017B37F
		public bool IsInWater
		{
			get
			{
				return this.mIsInWater;
			}
		}

		// Token: 0x17000B0F RID: 2831
		// (get) Token: 0x06002EFF RID: 12031 RVA: 0x0017D187 File Offset: 0x0017B387
		// (set) Token: 0x06002F00 RID: 12032 RVA: 0x0017D190 File Offset: 0x0017B390
		public Vector3 Movement
		{
			get
			{
				return this.mMovement;
			}
			set
			{
				value.Y = 0f;
				float num = value.LengthSquared();
				if (num <= 1E-06f)
				{
					this.mMovement = default(Vector3);
					this.mMoving = false;
					return;
				}
				num = (float)Math.Sqrt((double)num);
				float num2 = 1f / num;
				this.mDesiredDirection.X = value.X * num2;
				this.mDesiredDirection.Y = value.Y * num2;
				this.mDesiredDirection.Z = value.Z * num2;
				if (num > 1f)
				{
					this.mMovement = this.mDesiredDirection;
					return;
				}
				this.mMovement = value;
			}
		}

		// Token: 0x17000B10 RID: 2832
		// (get) Token: 0x06002F01 RID: 12033 RVA: 0x0017D236 File Offset: 0x0017B436
		public Vector3 Direction
		{
			get
			{
				return this.transform.Orientation.Forward;
			}
		}

		// Token: 0x17000B11 RID: 2833
		// (get) Token: 0x06002F02 RID: 12034 RVA: 0x0017D248 File Offset: 0x0017B448
		// (set) Token: 0x06002F03 RID: 12035 RVA: 0x0017D250 File Offset: 0x0017B450
		public Vector3 DesiredDirection
		{
			get
			{
				return this.mDesiredDirection;
			}
			set
			{
				value.Y = 0f;
				float num = value.LengthSquared();
				if (num > 1E-06f)
				{
					num = (float)Math.Sqrt((double)num);
					Vector3.Divide(ref value, num, out this.mDesiredDirection);
				}
			}
		}

		// Token: 0x17000B12 RID: 2834
		// (get) Token: 0x06002F04 RID: 12036 RVA: 0x0017D290 File Offset: 0x0017B490
		// (set) Token: 0x06002F05 RID: 12037 RVA: 0x0017D2B2 File Offset: 0x0017B4B2
		public float NormalizedVelocity
		{
			get
			{
				return base.Velocity.Length() / this.MaxVelocity;
			}
			set
			{
				base.Velocity = this.Direction * (value * this.MaxVelocity);
			}
		}

		// Token: 0x17000B13 RID: 2835
		// (get) Token: 0x06002F06 RID: 12038 RVA: 0x0017D2CD File Offset: 0x0017B4CD
		// (set) Token: 0x06002F07 RID: 12039 RVA: 0x0017D2D5 File Offset: 0x0017B4D5
		public float MaxVelocity
		{
			get
			{
				return this.mMaxSpeed;
			}
			set
			{
				this.mMaxSpeed = value;
			}
		}

		// Token: 0x17000B14 RID: 2836
		// (get) Token: 0x06002F08 RID: 12040 RVA: 0x0017D2DE File Offset: 0x0017B4DE
		// (set) Token: 0x06002F09 RID: 12041 RVA: 0x0017D2E6 File Offset: 0x0017B4E6
		public float SpeedMultiplier
		{
			get
			{
				return this.mSpeedmultiplier;
			}
			set
			{
				value = Math.Min(value, 100f);
				this.mSpeedmultiplier = Math.Max(value, 0f);
			}
		}

		// Token: 0x17000B15 RID: 2837
		// (get) Token: 0x06002F0A RID: 12042 RVA: 0x0017D306 File Offset: 0x0017B506
		public bool Moving
		{
			get
			{
				return this.mMoving;
			}
		}

		// Token: 0x17000B16 RID: 2838
		// (get) Token: 0x06002F0B RID: 12043 RVA: 0x0017D30E File Offset: 0x0017B50E
		// (set) Token: 0x06002F0C RID: 12044 RVA: 0x0017D316 File Offset: 0x0017B516
		public bool AllowMove
		{
			get
			{
				return this.mAllowMove;
			}
			set
			{
				this.mAllowMove = value;
			}
		}

		// Token: 0x17000B17 RID: 2839
		// (get) Token: 0x06002F0D RID: 12045 RVA: 0x0017D31F File Offset: 0x0017B51F
		// (set) Token: 0x06002F0E RID: 12046 RVA: 0x0017D327 File Offset: 0x0017B527
		public bool AllowRotate
		{
			get
			{
				return this.mAllowRotate;
			}
			set
			{
				this.mAllowRotate = value;
			}
		}

		// Token: 0x17000B18 RID: 2840
		// (get) Token: 0x06002F0F RID: 12047 RVA: 0x0017D330 File Offset: 0x0017B530
		public bool IsLeavingGround
		{
			get
			{
				return this.mIsLeavingGround;
			}
		}

		// Token: 0x17000B19 RID: 2841
		// (get) Token: 0x06002F10 RID: 12048 RVA: 0x0017D338 File Offset: 0x0017B538
		public Vector3 Impulse
		{
			get
			{
				return this.mImpulse;
			}
		}

		// Token: 0x17000B1A RID: 2842
		// (get) Token: 0x06002F11 RID: 12049 RVA: 0x0017D340 File Offset: 0x0017B540
		public bool IsPushed
		{
			get
			{
				return this.mIsPushed;
			}
		}

		// Token: 0x17000B1B RID: 2843
		// (get) Token: 0x06002F12 RID: 12050 RVA: 0x0017D348 File Offset: 0x0017B548
		// (set) Token: 0x06002F13 RID: 12051 RVA: 0x0017D350 File Offset: 0x0017B550
		public bool IsJumping
		{
			get
			{
				return this.mIsJumping;
			}
			set
			{
				this.mIsJumping = value;
			}
		}

		// Token: 0x17000B1C RID: 2844
		// (get) Token: 0x06002F14 RID: 12052 RVA: 0x0017D359 File Offset: 0x0017B559
		public bool IsTouchingGround
		{
			get
			{
				return this.mIsTouchingGround;
			}
		}

		// Token: 0x17000B1D RID: 2845
		// (get) Token: 0x06002F15 RID: 12053 RVA: 0x0017D361 File Offset: 0x0017B561
		public bool IsTouchingSolidGround
		{
			get
			{
				return this.mIsTouchingGround && !this.mIsOnIce;
			}
		}

		// Token: 0x17000B1E RID: 2846
		// (get) Token: 0x06002F16 RID: 12054 RVA: 0x0017D376 File Offset: 0x0017B576
		public CollisionMaterials GroundMaterial
		{
			get
			{
				return this.mGroundMaterial;
			}
		}

		// Token: 0x17000B1F RID: 2847
		// (get) Token: 0x06002F17 RID: 12055 RVA: 0x0017D37E File Offset: 0x0017B57E
		public Character Owner
		{
			get
			{
				return this.mOwner.Target as Character;
			}
		}

		// Token: 0x17000B20 RID: 2848
		// (get) Token: 0x06002F18 RID: 12056 RVA: 0x0017D390 File Offset: 0x0017B590
		public Vector3 LastPositionOnGround
		{
			get
			{
				return this.mLastPositionOnGround;
			}
		}

		// Token: 0x06002F19 RID: 12057 RVA: 0x0017D398 File Offset: 0x0017B598
		public override void EnableBody()
		{
			this.mMovement = default(Vector3);
			this.mMoving = false;
			this.mIsTouchingGround = true;
			this.mIsLeavingGround = false;
			this.mIsPushed = false;
			this.mImpulseApplyed = false;
			this.mImpulse = default(Vector3);
			this.mAllowMove = true;
			this.mAllowRotate = true;
			this.mMoving = false;
			this.mIsOnIce = false;
			this.mIsOnGrease = false;
			this.mIsJumping = false;
			base.EnableBody();
		}

		// Token: 0x06002F1A RID: 12058 RVA: 0x0017D410 File Offset: 0x0017B610
		private void Turn(ref Vector3 iNewDirection, float iTurnSpeed, float iDeltaTime)
		{
			Matrix identity = Matrix.Identity;
			Vector3 up = Vector3.Up;
			iNewDirection.Y = 0f;
			Vector3 right;
			Vector3.Cross(ref iNewDirection, ref up, out right);
			identity.Forward = iNewDirection;
			identity.Up = up;
			identity.Right = right;
			Quaternion quaternion;
			Quaternion.CreateFromRotationMatrix(ref this.transform.Orientation, out quaternion);
			Quaternion quaternion2;
			Quaternion.CreateFromRotationMatrix(ref identity, out quaternion2);
			Quaternion.Lerp(ref quaternion, ref quaternion2, MathHelper.Clamp(iDeltaTime * iTurnSpeed, 0f, 1f), out quaternion2);
			Matrix.CreateFromQuaternion(ref quaternion2, out this.transform.Orientation);
		}

		// Token: 0x06002F1B RID: 12059 RVA: 0x0017D4A8 File Offset: 0x0017B6A8
		public override void ProcessCollisionPoints(float dt)
		{
			base.ProcessCollisionPoints(dt);
			float num = 0f;
			Liquid liquid = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < base.CollisionSkin.Collisions.Count; i++)
			{
				CollisionInfo collisionInfo = base.CollisionSkin.Collisions[i];
				if (collisionInfo.SkinInfo.Skin0 != null && collisionInfo.SkinInfo.Skin1 != null)
				{
					Vector3 dirToBody = collisionInfo.DirToBody0;
					Vector3 velocity = base.Velocity;
					float num2;
					CollisionSkin collisionSkin;
					Body owner;
					CharacterBody characterBody;
					if (collisionInfo.SkinInfo.Skin0.Owner == this)
					{
						Vector3.Negate(ref dirToBody, out dirToBody);
						Vector3.Dot(ref velocity, ref dirToBody, out num2);
						collisionSkin = collisionInfo.SkinInfo.Skin1;
						owner = collisionSkin.Owner;
						characterBody = (owner as CharacterBody);
						if (characterBody != null)
						{
							if (!(characterBody.Owner is Avatar) && characterBody.Mass < base.Mass * 0.5f)
							{
								collisionInfo.SkinInfo.IgnoreSkin0 = true;
								if (num2 > 0f)
								{
									characterBody.Owner.KnockDown();
								}
							}
							else if (this.Owner.IsInvisibile && characterBody.Owner != null && (characterBody.Owner.Faction & this.Owner.Faction) == Factions.NONE)
							{
								this.Owner.SetInvisible(0f);
							}
						}
					}
					else
					{
						Vector3.Dot(ref velocity, ref dirToBody, out num2);
						collisionSkin = collisionInfo.SkinInfo.Skin0;
						owner = collisionSkin.Owner;
						characterBody = (owner as CharacterBody);
						if (characterBody != null && !(characterBody.Owner is Avatar) && characterBody.Mass < base.Mass * 0.5f)
						{
							collisionInfo.SkinInfo.IgnoreSkin1 = true;
							if (num2 > 0f)
							{
								characterBody.Owner.KnockDown();
							}
						}
					}
					int num3;
					if (collisionInfo.SkinInfo.Skin0.Owner == this)
					{
						num3 = collisionInfo.SkinInfo.IndexPrim1;
					}
					else
					{
						num3 = collisionInfo.SkinInfo.IndexPrim0;
					}
					bool flag4 = collisionSkin.GetPrimitiveLocal(num3) is WaterMesh;
					bool flag5 = collisionSkin.Tag is LevelModel;
					bool flag6 = collisionSkin.GetPrimitiveLocal(num3) is IceMesh;
					bool flag7 = collisionSkin.Owner is PhysicsObjectBody;
					if (flag4)
					{
						flag = true;
						if (collisionSkin.Tag is Lava)
						{
							this.mGroundMaterial = CollisionMaterials.Lava;
						}
						else
						{
							this.mGroundMaterial = CollisionMaterials.Water;
						}
						collisionInfo.SkinInfo.IgnoreSkin0 = true;
						collisionInfo.SkinInfo.IgnoreSkin0 = true;
						base.CollisionSkin.Collisions[i] = collisionInfo;
						Vector3 r = collisionInfo.PointInfo[0].info.R1;
						Vector3 position = base.Position;
						position.Y += this.Owner.HeightOffset;
						r.X = (position.X = (r.Z = (position.Z = 0f)));
						float num4 = 0f;
						Vector3.Distance(ref position, ref r, out num4);
						if (num4 > num)
						{
							num = num4;
							this.Owner.WaterDepth = num4;
							liquid = (collisionSkin.Tag as Liquid);
						}
						if (this.Owner.WaterDepth >= Defines.FOOTSTEP_WATER_OFFSET && (!this.Owner.IsSelfShielded || this.Owner.IsSolidSelfShielded))
						{
							if (collisionSkin.Tag is Lava)
							{
								this.Owner.AddStatusEffect(new StatusEffect(StatusEffects.Burning, 100f, this.Owner.WaterDepth * dt * 20f, this.Owner.Capsule.Length, this.Owner.Capsule.Radius));
							}
							else if (collisionSkin.Tag is Water)
							{
								this.Owner.AddStatusEffect(new StatusEffect(StatusEffects.Wet, 0f, this.Owner.WaterDepth * dt, this.Owner.Capsule.Length, this.Owner.Capsule.Radius));
							}
						}
					}
					if (flag5 || flag6)
					{
						if (!this.mIsTouchingGround && !this.IsJumping && !this.Owner.IgnoreCollisionDamage && this.Owner.PlayState.Level.CurrentScene != null && num2 > 0f)
						{
							Damage iDamage = default(Damage);
							float num5 = 2.7f;
							float num6 = 100f / (float)Math.Pow(10.0, (double)num5);
							iDamage.Amount = (float)Math.Pow((double)num2, (double)num5) * num6;
							bool flag8 = (short)(iDamage.AttackProperty | AttackProperties.Knockback) == 6;
							flag8 |= ((short)(iDamage.AttackProperty | AttackProperties.Damage) == 1 && iDamage.Amount > 50f);
							if (flag8)
							{
								if (flag3)
								{
									goto IL_7F8;
								}
								flag3 = true;
							}
							if (iDamage.Amount > 50f)
							{
								iDamage.AttackProperty = AttackProperties.Damage;
								if (iDamage.Amount > 100f)
								{
									iDamage.AttackProperty |= AttackProperties.Knockdown;
								}
								iDamage.Element = Elements.Earth;
								iDamage.Magnitude = 1f;
								Avatar avatar = this.Owner as Avatar;
								if (avatar != null && avatar.HitPoints > 0f && !(avatar.Player.Gamer is NetworkGamer))
								{
									DamageResult damageResult = this.Owner.Damage(iDamage, this.Owner.LastAttacker, this.Owner.PlayState.PlayTime, base.Position);
									if ((damageResult & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
									{
										AchievementsManager.Instance.AwardAchievement(this.Owner.PlayState, "ohgravitythouartahea");
									}
								}
								else
								{
									this.Owner.Damage(iDamage, this.Owner.LastAttacker, this.Owner.PlayState.PlayTime, base.Position);
								}
							}
						}
						if (dirToBody.Y < -0.7f)
						{
							this.mTimeSinceGroundContact = 0f;
							if (this.Owner.HitPoints > 0f)
							{
								this.mLastPositionOnGround = base.Position;
							}
							this.mGroundContactNormal = collisionInfo.DirToBody0;
							if (!flag4 && !flag)
							{
								this.mGroundMaterial = (CollisionMaterials)num3;
							}
							flag2 = (flag6 & collisionSkin.Tag is Water);
						}
					}
					else if (flag7)
					{
						float num7 = collisionInfo.DirToBody0.Y;
						if (this == collisionInfo.SkinInfo.Skin1.Owner)
						{
							num7 = -num7;
						}
						if (num7 > 0.7f)
						{
							this.mTimeSinceGroundContact = 0f;
							if (this.Owner.HitPoints > 0f)
							{
								this.mLastPositionOnGround = base.Position;
							}
							this.mGroundMaterial = CollisionMaterials.Generic;
						}
					}
					if (this.mIsPushed || this.mIsLeavingGround)
					{
						if (characterBody != null && (num2 > 0f || characterBody.Owner.IsKnockedDown))
						{
							if (owner.Mass <= base.Mass)
							{
								characterBody.Owner.KnockDown();
								if (collisionInfo.SkinInfo.Skin0.Owner == this)
								{
									collisionInfo.SkinInfo.IgnoreSkin0 = true;
								}
								else
								{
									collisionInfo.SkinInfo.IgnoreSkin1 = true;
								}
							}
							else if (this.mTimeSinceGroundContact <= 0.2f)
							{
								this.Owner.KnockDown();
							}
						}
						else if (num2 > 0f && collisionSkin.Tag is LevelModel && Math.Abs(collisionInfo.DirToBody0.Y) < 0.3f)
						{
							this.Owner.KnockDown();
						}
					}
					else
					{
						velocity.Y = 0f;
						if (this.mIsTouchingGround & velocity.LengthSquared() < 1E-06f & characterBody != null)
						{
							if (collisionInfo.SkinInfo.Skin0.Owner == this)
							{
								collisionInfo.SkinInfo.IgnoreSkin0 = true;
							}
							else
							{
								collisionInfo.SkinInfo.IgnoreSkin1 = true;
							}
						}
					}
				}
				IL_7F8:;
			}
			this.mIsOnIce = flag2;
			float num8 = -this.Owner.HeightOffset;
			if (this.Owner.HitPoints > 0f && flag && this.Owner.WaterDepth >= num8)
			{
				if (liquid.Parent != null)
				{
					liquid.Parent.AddEntity(this.Owner);
				}
				this.Owner.Drown();
			}
			this.mIsInWater = flag;
			if (flag)
			{
				float num9 = (float)Math.Pow((double)Math.Max(1f - this.Owner.WaterDepth, 0f), 100.0);
				num9 = Math.Max(num9, 0.1f);
				num9 = (float)Math.Pow((double)num9, (double)dt);
				Vector3 velocity2 = base.Velocity;
				Vector3.Multiply(ref velocity2, num9, out velocity2);
				base.Velocity = velocity2;
			}
		}

		// Token: 0x06002F1C RID: 12060 RVA: 0x0017DD94 File Offset: 0x0017BF94
		public override void AddExternalForces(float dt)
		{
			base.ClearForces();
			Vector3 vector = base.Velocity;
			if (this.mIsFlying)
			{
				this.mTimeSinceGroundContact = 0f;
				Vector3 position = base.Position;
				Vector3 position2 = base.Position;
				this.Owner.PlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref position, out position2, MovementProperties.Default);
				position2.Y += this.mFlyHeight;
				float num = 0.2f * (position2.Y - position.Y) - 0.02f * vector.Y;
				vector.Y += num;
			}
			else
			{
				this.mTimeSinceGroundContact += dt;
			}
			this.mIsTouchingGround = (this.mTimeSinceGroundContact <= 0.2f || this.Owner.IsLevitating);
			bool flag = false;
			Vector3.Add(ref vector, ref this.mImpulse, out vector);
			this.mImpulse = Vector3.Zero;
			base.Velocity = vector;
			if (this.mIsLeavingGround)
			{
				this.mTimeSinceGroundContact = 0.3f;
				this.mIsTouchingGround = false;
				this.mIsLeavingGround = false;
			}
			else if (this.mIsPushed)
			{
				if (this.mIsTouchingGround)
				{
					if (this.Owner.IsKnockedDown)
					{
						this.mIsPushed = false;
						vector = default(Vector3);
					}
					else
					{
						double x = 0.3;
						if (this.mIsTouchingGround)
						{
							if (this.mIsOnGrease || this.mIsOnIce || this.Owner.IsLevitating)
							{
								x = 0.1;
							}
							else
							{
								x = 0.02;
							}
						}
						Vector3.Multiply(ref vector, (float)Math.Pow(x, (double)dt), out vector);
						if (vector.Y > 0f)
						{
							vector.Y = 0f;
						}
						if (vector.LengthSquared() < this.MaxVelocity * this.MaxVelocity * 0.33f)
						{
							this.mIsPushed = false;
						}
					}
				}
				base.Velocity = vector;
				if (vector.LengthSquared() > 1E-06f && vector.X + vector.Z > 1E-45f)
				{
					Vector3.Negate(ref vector, out vector);
					vector.Normalize();
					this.Turn(ref vector, this.Owner.TurnSpeed, dt);
				}
			}
			else if (this.mIsTouchingGround)
			{
				if (!this.mIsOnIce && !this.mIsOnGrease && !this.Owner.IsLevitating)
				{
					vector.X = 0f;
					vector.Z = 0f;
				}
				if (!this.mIsFlying & !this.mIsJumping & vector.Y > 0f)
				{
					vector.Y *= 0.8f;
				}
				this.mIsJumping = false;
				this.mIsPushed = false;
				Vector3 vector2 = this.mMovement;
				if (this.mIsInWater)
				{
					float scaleFactor = MathHelper.Clamp(1f - this.Owner.WaterDepth * 0.5f, 0.5f, 1f);
					Vector3.Multiply(ref vector2, scaleFactor, out vector2);
				}
				Vector3 vector3;
				Vector3.Multiply(ref vector2, this.mMaxSpeed * this.mSpeedmultiplier, out vector3);
				Vector3.Subtract(ref vector3, ref vector, out vector3);
				vector3.Y = 0f;
				if (this.mIsOnIce || this.mIsOnGrease || this.Owner.IsLevitating)
				{
					vector3.X *= Math.Min(dt * 1f, 1f);
					vector3.Z *= Math.Min(dt * 1f, 1f);
				}
				vector3.X *= this.Slow;
				vector3.Z *= this.Slow;
				vector3.Y = 0f;
				if (this.mAllowMove)
				{
					Vector3.Add(ref vector3, ref vector, out vector);
					flag = true;
					Vector3 vector4 = vector;
					vector4.Y = 0f;
					if (vector4.LengthSquared() < 1E-06f)
					{
						flag = false;
					}
				}
				if (this.mAllowRotate)
				{
					Vector3 vector5;
					if (this.mRunBackwards)
					{
						Vector3.Negate(ref this.mDesiredDirection, out vector5);
					}
					else
					{
						vector5 = this.mDesiredDirection;
					}
					this.Turn(ref vector5, this.Owner.TurnSpeed, dt);
				}
				base.Velocity = vector;
			}
			if (this.mAdditionalForce.LengthSquared() > 1E-06f)
			{
				vector = base.Velocity;
				float num2 = vector.LengthSquared();
				if (num2 > 1E-06f)
				{
					float value = this.mAdditionalForce.Length();
					Vector3 vector6;
					Vector3.Divide(ref vector, value, out vector6);
					Vector3 vector7;
					Vector3.Divide(ref this.mAdditionalForce, value, out vector7);
					float num3;
					Vector3.Dot(ref vector7, ref vector6, out num3);
					Vector3.Multiply(ref this.mAdditionalForce, MathHelper.Clamp(1f - num3, 0f, 1f), out this.mAdditionalForce);
				}
				Vector3.Add(ref vector, ref this.mAdditionalForce, out vector);
				base.Velocity = vector;
				this.mAdditionalForce = default(Vector3);
			}
			this.Slow = 1f;
			this.mMoving = flag;
			base.AddGravityToExternalForce();
			this.mIsOnGrease = false;
		}

		// Token: 0x0400332F RID: 13103
		protected const float MAXTIMESKYWALKER = 0.2f;

		// Token: 0x04003330 RID: 13104
		protected float mFlyHeight = 2f;

		// Token: 0x04003331 RID: 13105
		protected bool mIsFlying;

		// Token: 0x04003332 RID: 13106
		protected Vector3 mAdditionalForce;

		// Token: 0x04003333 RID: 13107
		protected Vector3 mLastPositionOnGround;

		// Token: 0x04003334 RID: 13108
		protected Vector3 mGroundContactNormal;

		// Token: 0x04003335 RID: 13109
		protected float mTimeSinceGroundContact;

		// Token: 0x04003336 RID: 13110
		protected bool mIsTouchingGround;

		// Token: 0x04003337 RID: 13111
		protected bool mIsLeavingGround;

		// Token: 0x04003338 RID: 13112
		protected bool mIsPushed;

		// Token: 0x04003339 RID: 13113
		protected bool mImpulseApplyed;

		// Token: 0x0400333A RID: 13114
		protected Vector3 mImpulse;

		// Token: 0x0400333B RID: 13115
		protected bool mAllowMove = true;

		// Token: 0x0400333C RID: 13116
		protected bool mAllowRotate = true;

		// Token: 0x0400333D RID: 13117
		protected bool mMoving;

		// Token: 0x0400333E RID: 13118
		protected bool mIsOnIce;

		// Token: 0x0400333F RID: 13119
		protected bool mIsInWater;

		// Token: 0x04003340 RID: 13120
		protected bool mIsOnGrease;

		// Token: 0x04003341 RID: 13121
		protected bool mIsJumping;

		// Token: 0x04003342 RID: 13122
		public float Slow;

		// Token: 0x04003343 RID: 13123
		protected Vector3 mMovement;

		// Token: 0x04003344 RID: 13124
		protected Vector3 mDesiredDirection = Vector3.Forward;

		// Token: 0x04003345 RID: 13125
		protected float mMaxSpeed;

		// Token: 0x04003346 RID: 13126
		protected float mSpeedmultiplier;

		// Token: 0x04003347 RID: 13127
		protected WeakReference mOwner;

		// Token: 0x04003348 RID: 13128
		protected CollisionMaterials mGroundMaterial;

		// Token: 0x04003349 RID: 13129
		protected bool mRunBackwards;
	}
}
