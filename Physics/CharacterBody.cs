// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.CharacterBody
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Physics;
using Magicka.Achievements;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.Physics;

internal class CharacterBody : Body
{
  protected const float MAXTIMESKYWALKER = 0.2f;
  protected float mFlyHeight = 2f;
  protected bool mIsFlying;
  protected Vector3 mAdditionalForce;
  protected Vector3 mLastPositionOnGround;
  protected Vector3 mGroundContactNormal;
  protected float mTimeSinceGroundContact;
  protected bool mIsTouchingGround;
  protected bool mIsLeavingGround;
  protected bool mIsPushed;
  protected bool mImpulseApplyed;
  protected Vector3 mImpulse;
  protected bool mAllowMove = true;
  protected bool mAllowRotate = true;
  protected bool mMoving;
  protected bool mIsOnIce;
  protected bool mIsInWater;
  protected bool mIsOnGrease;
  protected bool mIsJumping;
  public float Slow;
  protected Vector3 mMovement;
  protected Vector3 mDesiredDirection = Vector3.Forward;
  protected float mMaxSpeed;
  protected float mSpeedmultiplier;
  protected WeakReference mOwner;
  protected CollisionMaterials mGroundMaterial;
  protected bool mRunBackwards;

  public CharacterBody(Character iCharacter)
  {
    this.AllowFreezing = false;
    this.mOwner = new WeakReference((object) iCharacter);
    this.bodyInertia = new Matrix();
    this.bodyInvInertia = new Matrix();
  }

  public Vector3 AdditionalForce
  {
    get => this.mAdditionalForce;
    set
    {
      if (this.Owner.IsEntangled || (double) this.mSpeedmultiplier <= 9.9999999747524271E-07)
        return;
      this.mAdditionalForce = value;
    }
  }

  public void AddImpulse(Vector3 iImpulse)
  {
    if (this.Owner.IsEntangled)
      return;
    Vector3.Multiply(ref iImpulse, this.InverseMass, out iImpulse);
    Vector3.Add(ref this.mImpulse, ref iImpulse, out this.mImpulse);
    this.mIsPushed = true;
    if ((double) this.mImpulse.Y <= 0.5)
      return;
    this.mIsLeavingGround = true;
  }

  public void AddJump(Vector3 iImpulse)
  {
    if (this.Owner.IsEntangled || !this.mIsTouchingGround | this.mIsLeavingGround | !this.mAllowMove)
      return;
    Vector3.Add(ref this.mImpulse, ref iImpulse, out this.mImpulse);
    this.mIsJumping = true;
    this.mIsLeavingGround = true;
    this.Velocity = new Vector3();
  }

  public void AddImpulseVelocity(ref Vector3 iImpulse)
  {
    if (this.Owner.IsEntangled)
      return;
    Vector3.Add(ref this.mImpulse, ref iImpulse, out this.mImpulse);
    this.mIsPushed = true;
    if ((double) this.mImpulse.Y <= 2.7000000476837158)
      return;
    this.mIsLeavingGround = true;
  }

  public bool RunBackward
  {
    get => this.mRunBackwards;
    set => this.mRunBackwards = value;
  }

  public bool IsOnGrease
  {
    get => this.mIsOnGrease;
    set => this.mIsOnGrease = value;
  }

  public bool IsFlying
  {
    get => this.mIsFlying;
    set => this.mIsFlying = value;
  }

  public bool IsInWater => this.mIsInWater;

  public Vector3 Movement
  {
    get => this.mMovement;
    set
    {
      value.Y = 0.0f;
      float d = value.LengthSquared();
      if ((double) d > 9.9999999747524271E-07)
      {
        float num1 = (float) Math.Sqrt((double) d);
        float num2 = 1f / num1;
        this.mDesiredDirection.X = value.X * num2;
        this.mDesiredDirection.Y = value.Y * num2;
        this.mDesiredDirection.Z = value.Z * num2;
        if ((double) num1 > 1.0)
          this.mMovement = this.mDesiredDirection;
        else
          this.mMovement = value;
      }
      else
      {
        this.mMovement = new Vector3();
        this.mMoving = false;
      }
    }
  }

  public Vector3 Direction => this.transform.Orientation.Forward;

  public Vector3 DesiredDirection
  {
    get => this.mDesiredDirection;
    set
    {
      value.Y = 0.0f;
      float d = value.LengthSquared();
      if ((double) d <= 9.9999999747524271E-07)
        return;
      float num = (float) Math.Sqrt((double) d);
      Vector3.Divide(ref value, num, out this.mDesiredDirection);
    }
  }

  public float NormalizedVelocity
  {
    get => this.Velocity.Length() / this.MaxVelocity;
    set => this.Velocity = this.Direction * (value * this.MaxVelocity);
  }

  public float MaxVelocity
  {
    get => this.mMaxSpeed;
    set => this.mMaxSpeed = value;
  }

  public float SpeedMultiplier
  {
    get => this.mSpeedmultiplier;
    set
    {
      value = Math.Min(value, 100f);
      this.mSpeedmultiplier = Math.Max(value, 0.0f);
    }
  }

  public bool Moving => this.mMoving;

  public bool AllowMove
  {
    get => this.mAllowMove;
    set => this.mAllowMove = value;
  }

  public bool AllowRotate
  {
    get => this.mAllowRotate;
    set => this.mAllowRotate = value;
  }

  public bool IsLeavingGround => this.mIsLeavingGround;

  public Vector3 Impulse => this.mImpulse;

  public bool IsPushed => this.mIsPushed;

  public bool IsJumping
  {
    get => this.mIsJumping;
    set => this.mIsJumping = value;
  }

  public bool IsTouchingGround => this.mIsTouchingGround;

  public bool IsTouchingSolidGround => this.mIsTouchingGround && !this.mIsOnIce;

  public CollisionMaterials GroundMaterial => this.mGroundMaterial;

  public Character Owner => this.mOwner.Target as Character;

  public Vector3 LastPositionOnGround => this.mLastPositionOnGround;

  public override void EnableBody()
  {
    this.mMovement = new Vector3();
    this.mMoving = false;
    this.mIsTouchingGround = true;
    this.mIsLeavingGround = false;
    this.mIsPushed = false;
    this.mImpulseApplyed = false;
    this.mImpulse = new Vector3();
    this.mAllowMove = true;
    this.mAllowRotate = true;
    this.mMoving = false;
    this.mIsOnIce = false;
    this.mIsOnGrease = false;
    this.mIsJumping = false;
    base.EnableBody();
  }

  private void Turn(ref Vector3 iNewDirection, float iTurnSpeed, float iDeltaTime)
  {
    Matrix identity = Matrix.Identity;
    Vector3 up = Vector3.Up;
    iNewDirection.Y = 0.0f;
    Vector3 result1;
    Vector3.Cross(ref iNewDirection, ref up, out result1);
    identity.Forward = iNewDirection;
    identity.Up = up;
    identity.Right = result1;
    Quaternion result2;
    Quaternion.CreateFromRotationMatrix(ref this.transform.Orientation, out result2);
    Quaternion result3;
    Quaternion.CreateFromRotationMatrix(ref identity, out result3);
    Quaternion.Lerp(ref result2, ref result3, MathHelper.Clamp(iDeltaTime * iTurnSpeed, 0.0f, 1f), out result3);
    Matrix.CreateFromQuaternion(ref result3, out this.transform.Orientation);
  }

  public override void ProcessCollisionPoints(float dt)
  {
    base.ProcessCollisionPoints(dt);
    float num1 = 0.0f;
    Liquid liquid = (Liquid) null;
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    for (int index = 0; index < this.CollisionSkin.Collisions.Count; ++index)
    {
      CollisionInfo collision = this.CollisionSkin.Collisions[index];
      if (collision.SkinInfo.Skin0 != null && collision.SkinInfo.Skin1 != null)
      {
        Vector3 result1 = collision.DirToBody0;
        Vector3 velocity = this.Velocity;
        float result2;
        CollisionSkin collisionSkin;
        Body owner1;
        if (collision.SkinInfo.Skin0.Owner == this)
        {
          Vector3.Negate(ref result1, out result1);
          Vector3.Dot(ref velocity, ref result1, out result2);
          collisionSkin = collision.SkinInfo.Skin1;
          owner1 = collisionSkin.Owner;
          if (owner1 is CharacterBody characterBody)
          {
            if (!(characterBody.Owner is Avatar) && (double) characterBody.Mass < (double) this.Mass * 0.5)
            {
              collision.SkinInfo.IgnoreSkin0 = true;
              if ((double) result2 > 0.0)
                characterBody.Owner.KnockDown();
            }
            else if (this.Owner.IsInvisibile && characterBody.Owner != null && (characterBody.Owner.Faction & this.Owner.Faction) == Factions.NONE)
              this.Owner.SetInvisible(0.0f);
          }
        }
        else
        {
          Vector3.Dot(ref velocity, ref result1, out result2);
          collisionSkin = collision.SkinInfo.Skin0;
          owner1 = collisionSkin.Owner;
          if (owner1 is CharacterBody characterBody && !(characterBody.Owner is Avatar) && (double) characterBody.Mass < (double) this.Mass * 0.5)
          {
            collision.SkinInfo.IgnoreSkin1 = true;
            if ((double) result2 > 0.0)
              characterBody.Owner.KnockDown();
          }
        }
        int prim = collision.SkinInfo.Skin0.Owner != this ? collision.SkinInfo.IndexPrim0 : collision.SkinInfo.IndexPrim1;
        bool flag4 = collisionSkin.GetPrimitiveLocal(prim) is WaterMesh;
        bool flag5 = collisionSkin.Tag is LevelModel;
        bool flag6 = collisionSkin.GetPrimitiveLocal(prim) is IceMesh;
        bool flag7 = collisionSkin.Owner is PhysicsObjectBody;
        if (flag4)
        {
          flag1 = true;
          this.mGroundMaterial = !(collisionSkin.Tag is Lava) ? CollisionMaterials.Water : CollisionMaterials.Lava;
          collision.SkinInfo.IgnoreSkin0 = true;
          collision.SkinInfo.IgnoreSkin0 = true;
          this.CollisionSkin.Collisions[index] = collision;
          Vector3 r1 = collision.PointInfo[0].info.R1;
          Vector3 position = this.Position;
          position.Y += this.Owner.HeightOffset;
          r1.X = position.X = r1.Z = position.Z = 0.0f;
          float result3 = 0.0f;
          Vector3.Distance(ref position, ref r1, out result3);
          if ((double) result3 > (double) num1)
          {
            num1 = result3;
            this.Owner.WaterDepth = result3;
            liquid = collisionSkin.Tag as Liquid;
          }
          if ((double) this.Owner.WaterDepth >= (double) Defines.FOOTSTEP_WATER_OFFSET && (!this.Owner.IsSelfShielded || this.Owner.IsSolidSelfShielded))
          {
            if (collisionSkin.Tag is Lava)
            {
              int num2 = (int) this.Owner.AddStatusEffect(new StatusEffect(StatusEffects.Burning, 100f, (float) ((double) this.Owner.WaterDepth * (double) dt * 20.0), this.Owner.Capsule.Length, this.Owner.Capsule.Radius));
            }
            else if (collisionSkin.Tag is Water)
            {
              int num3 = (int) this.Owner.AddStatusEffect(new StatusEffect(StatusEffects.Wet, 0.0f, this.Owner.WaterDepth * dt, this.Owner.Capsule.Length, this.Owner.Capsule.Radius));
            }
          }
        }
        if (flag5 || flag6)
        {
          if (!this.mIsTouchingGround && !this.IsJumping && !this.Owner.IgnoreCollisionDamage && this.Owner.PlayState.Level.CurrentScene != null && (double) result2 > 0.0)
          {
            Damage iDamage = new Damage();
            float y = 2.7f;
            float num4 = 100f / (float) Math.Pow(10.0, (double) y);
            iDamage.Amount = (float) Math.Pow((double) result2, (double) y) * num4;
            if ((((iDamage.AttackProperty | AttackProperties.Knockback) == AttackProperties.Knockback ? 1 : 0) | ((iDamage.AttackProperty | AttackProperties.Damage) != AttackProperties.Damage ? 0 : ((double) iDamage.Amount > 50.0 ? 1 : 0))) != 0)
            {
              if (!flag3)
                flag3 = true;
              else
                continue;
            }
            if ((double) iDamage.Amount > 50.0)
            {
              iDamage.AttackProperty = AttackProperties.Damage;
              if ((double) iDamage.Amount > 100.0)
                iDamage.AttackProperty |= AttackProperties.Knockdown;
              iDamage.Element = Elements.Earth;
              iDamage.Magnitude = 1f;
              if (this.Owner is Avatar owner2 && (double) owner2.HitPoints > 0.0 && !(owner2.Player.Gamer is NetworkGamer))
              {
                if ((this.Owner.Damage(iDamage, this.Owner.LastAttacker, this.Owner.PlayState.PlayTime, this.Position) & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
                  AchievementsManager.Instance.AwardAchievement(this.Owner.PlayState, "ohgravitythouartahea");
              }
              else
              {
                int num5 = (int) this.Owner.Damage(iDamage, this.Owner.LastAttacker, this.Owner.PlayState.PlayTime, this.Position);
              }
            }
          }
          if ((double) result1.Y < -0.699999988079071)
          {
            this.mTimeSinceGroundContact = 0.0f;
            if ((double) this.Owner.HitPoints > 0.0)
              this.mLastPositionOnGround = this.Position;
            this.mGroundContactNormal = collision.DirToBody0;
            if (!flag4 && !flag1)
              this.mGroundMaterial = (CollisionMaterials) prim;
            flag2 = flag6 & collisionSkin.Tag is Water;
          }
        }
        else if (flag7)
        {
          float num6 = collision.DirToBody0.Y;
          if (this == collision.SkinInfo.Skin1.Owner)
            num6 = -num6;
          if ((double) num6 > 0.699999988079071)
          {
            this.mTimeSinceGroundContact = 0.0f;
            if ((double) this.Owner.HitPoints > 0.0)
              this.mLastPositionOnGround = this.Position;
            this.mGroundMaterial = CollisionMaterials.Generic;
          }
        }
        if (this.mIsPushed || this.mIsLeavingGround)
        {
          if (characterBody != null && ((double) result2 > 0.0 || characterBody.Owner.IsKnockedDown))
          {
            if ((double) owner1.Mass <= (double) this.Mass)
            {
              characterBody.Owner.KnockDown();
              if (collision.SkinInfo.Skin0.Owner == this)
                collision.SkinInfo.IgnoreSkin0 = true;
              else
                collision.SkinInfo.IgnoreSkin1 = true;
            }
            else if ((double) this.mTimeSinceGroundContact <= 0.20000000298023224)
              this.Owner.KnockDown();
          }
          else if ((double) result2 > 0.0 && collisionSkin.Tag is LevelModel && (double) Math.Abs(collision.DirToBody0.Y) < 0.30000001192092896)
            this.Owner.KnockDown();
        }
        else
        {
          velocity.Y = 0.0f;
          if (this.mIsTouchingGround & (double) velocity.LengthSquared() < 9.9999999747524271E-07 & characterBody != null)
          {
            if (collision.SkinInfo.Skin0.Owner == this)
              collision.SkinInfo.IgnoreSkin0 = true;
            else
              collision.SkinInfo.IgnoreSkin1 = true;
          }
        }
      }
    }
    this.mIsOnIce = flag2;
    float num7 = -this.Owner.HeightOffset;
    if ((double) this.Owner.HitPoints > 0.0 && flag1 && (double) this.Owner.WaterDepth >= (double) num7)
    {
      if (liquid.Parent != null)
        liquid.Parent.AddEntity((Entity) this.Owner);
      this.Owner.Drown();
    }
    this.mIsInWater = flag1;
    if (!flag1)
      return;
    float scaleFactor = (float) Math.Pow((double) Math.Max((float) Math.Pow((double) Math.Max(1f - this.Owner.WaterDepth, 0.0f), 100.0), 0.1f), (double) dt);
    Vector3 result = this.Velocity;
    Vector3.Multiply(ref result, scaleFactor, out result);
    this.Velocity = result;
  }

  public override void AddExternalForces(float dt)
  {
    this.ClearForces();
    Vector3 result1 = this.Velocity;
    if (this.mIsFlying)
    {
      this.mTimeSinceGroundContact = 0.0f;
      Vector3 position = this.Position;
      Vector3 oPoint = this.Position;
      double nearestPosition = (double) this.Owner.PlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref position, out oPoint, MovementProperties.Default);
      oPoint.Y += this.mFlyHeight;
      float num = (float) (0.20000000298023224 * ((double) oPoint.Y - (double) position.Y) - 0.019999999552965164 * (double) result1.Y);
      result1.Y += num;
    }
    else
      this.mTimeSinceGroundContact += dt;
    this.mIsTouchingGround = (double) this.mTimeSinceGroundContact <= 0.20000000298023224 || this.Owner.IsLevitating;
    bool flag = false;
    Vector3.Add(ref result1, ref this.mImpulse, out result1);
    this.mImpulse = Vector3.Zero;
    this.Velocity = result1;
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
          result1 = new Vector3();
        }
        else
        {
          double x = 0.3;
          if (this.mIsTouchingGround)
            x = this.mIsOnGrease || this.mIsOnIce || this.Owner.IsLevitating ? 0.1 : 0.02;
          Vector3.Multiply(ref result1, (float) Math.Pow(x, (double) dt), out result1);
          if ((double) result1.Y > 0.0)
            result1.Y = 0.0f;
          if ((double) result1.LengthSquared() < (double) this.MaxVelocity * (double) this.MaxVelocity * 0.33000001311302185)
            this.mIsPushed = false;
        }
      }
      this.Velocity = result1;
      if ((double) result1.LengthSquared() > 9.9999999747524271E-07 && (double) result1.X + (double) result1.Z > 1.4012984643248171E-45)
      {
        Vector3.Negate(ref result1, out result1);
        result1.Normalize();
        this.Turn(ref result1, this.Owner.TurnSpeed, dt);
      }
    }
    else if (this.mIsTouchingGround)
    {
      if (!this.mIsOnIce && !this.mIsOnGrease && !this.Owner.IsLevitating)
      {
        result1.X = 0.0f;
        result1.Z = 0.0f;
      }
      if (!this.mIsFlying & !this.mIsJumping & (double) result1.Y > 0.0)
        result1.Y *= 0.8f;
      this.mIsJumping = false;
      this.mIsPushed = false;
      Vector3 result2 = this.mMovement;
      if (this.mIsInWater)
      {
        float scaleFactor = MathHelper.Clamp((float) (1.0 - (double) this.Owner.WaterDepth * 0.5), 0.5f, 1f);
        Vector3.Multiply(ref result2, scaleFactor, out result2);
      }
      Vector3 result3;
      Vector3.Multiply(ref result2, this.mMaxSpeed * this.mSpeedmultiplier, out result3);
      Vector3.Subtract(ref result3, ref result1, out result3);
      result3.Y = 0.0f;
      if (this.mIsOnIce || this.mIsOnGrease || this.Owner.IsLevitating)
      {
        result3.X *= Math.Min(dt * 1f, 1f);
        result3.Z *= Math.Min(dt * 1f, 1f);
      }
      result3.X *= this.Slow;
      result3.Z *= this.Slow;
      result3.Y = 0.0f;
      if (this.mAllowMove)
      {
        Vector3.Add(ref result3, ref result1, out result1);
        flag = true;
        if ((double) (result1 with { Y = 0.0f }).LengthSquared() < 9.9999999747524271E-07)
          flag = false;
      }
      if (this.mAllowRotate)
      {
        Vector3 result4;
        if (this.mRunBackwards)
          Vector3.Negate(ref this.mDesiredDirection, out result4);
        else
          result4 = this.mDesiredDirection;
        this.Turn(ref result4, this.Owner.TurnSpeed, dt);
      }
      this.Velocity = result1;
    }
    if ((double) this.mAdditionalForce.LengthSquared() > 9.9999999747524271E-07)
    {
      result1 = this.Velocity;
      if ((double) result1.LengthSquared() > 9.9999999747524271E-07)
      {
        float num = this.mAdditionalForce.Length();
        Vector3 result5;
        Vector3.Divide(ref result1, num, out result5);
        Vector3 result6;
        Vector3.Divide(ref this.mAdditionalForce, num, out result6);
        float result7;
        Vector3.Dot(ref result6, ref result5, out result7);
        Vector3.Multiply(ref this.mAdditionalForce, MathHelper.Clamp(1f - result7, 0.0f, 1f), out this.mAdditionalForce);
      }
      Vector3.Add(ref result1, ref this.mAdditionalForce, out result1);
      this.Velocity = result1;
      this.mAdditionalForce = new Vector3();
    }
    this.Slow = 1f;
    this.mMoving = flag;
    this.AddGravityToExternalForce();
    this.mIsOnGrease = false;
  }
}
