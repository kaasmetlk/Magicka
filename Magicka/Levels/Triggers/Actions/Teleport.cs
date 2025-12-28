using System;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.Graphics;
using Magicka.Physics;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020000BA RID: 186
	internal class Teleport : Action
	{
		// Token: 0x06000565 RID: 1381 RVA: 0x0002019B File Offset: 0x0001E39B
		public Teleport(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x000201A8 File Offset: 0x0001E3A8
		protected override void Execute()
		{
			if (this.mIDHash == 0)
			{
				return;
			}
			if (this.mEffectHash == 0)
			{
				this.mEffectHash = Teleport.TELEPORT_EFFECT_APPEAR;
			}
			if (this.mSoundHash == 0)
			{
				this.mSoundHash = Teleport.TELEPORT_SOUND_DESTINATION;
			}
			Entity byID = Entity.GetByID(this.mIDHash);
			if (byID == null)
			{
				return;
			}
			Vector3 position = byID.Position;
			Vector3 right = Vector3.Right;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(this.mEffectHash, ref position, ref right, out visualEffectReference);
			Matrix orientation;
			base.GameScene.PlayState.Level.CurrentScene.GetLocator(this.mTargetHash, out orientation);
			EffectManager.Instance.StartEffect(this.mEffectHash, ref orientation, out visualEffectReference);
			Vector3 vector = orientation.Translation;
			orientation.Translation = Vector3.Zero;
			Segment iSeg = default(Segment);
			iSeg.Origin = vector;
			iSeg.Delta.Y = iSeg.Delta.Y - 4f;
			float num;
			Vector3 vector2;
			Vector3 vector3;
			if (base.GameScene.PlayState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, iSeg))
			{
				vector = vector2;
			}
			if (byID.Body.CollisionSkin.GetPrimitiveLocal(0) is Sphere)
			{
				vector.Y += (byID.Body.CollisionSkin.GetPrimitiveLocal(0) as Sphere).Radius;
			}
			else if (byID.Body.CollisionSkin.GetPrimitiveLocal(0) is Capsule)
			{
				vector.Y += (byID.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Radius + (byID.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Length * 0.5f;
			}
			vector.Y += 0.1f;
			byID.Body.MoveTo(vector, orientation);
			if (byID.Body is CharacterBody)
			{
				(byID.Body as CharacterBody).DesiredDirection = orientation.Forward;
			}
			AudioManager.Instance.PlayCue(Banks.Spells, this.mSoundHash, byID.AudioEmitter);
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x000203BA File Offset: 0x0001E5BA
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x000203C2 File Offset: 0x0001E5C2
		// (set) Token: 0x06000569 RID: 1385 RVA: 0x000203CA File Offset: 0x0001E5CA
		public string Effect
		{
			get
			{
				return this.mEffect;
			}
			set
			{
				this.mEffect = value;
				this.mEffectHash = this.mEffect.GetHashCodeCustom();
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x0600056A RID: 1386 RVA: 0x000203E4 File Offset: 0x0001E5E4
		// (set) Token: 0x0600056B RID: 1387 RVA: 0x000203EC File Offset: 0x0001E5EC
		public string Sound
		{
			get
			{
				return this.mSound;
			}
			set
			{
				this.mSound = value;
				this.mSoundHash = this.mSound.GetHashCodeCustom();
			}
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600056C RID: 1388 RVA: 0x00020406 File Offset: 0x0001E606
		// (set) Token: 0x0600056D RID: 1389 RVA: 0x0002040E File Offset: 0x0001E60E
		public string ID
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
				this.mIDHash = this.mID.GetHashCodeCustom();
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x00020428 File Offset: 0x0001E628
		// (set) Token: 0x0600056F RID: 1391 RVA: 0x00020430 File Offset: 0x0001E630
		public string Target
		{
			get
			{
				return this.Target;
			}
			set
			{
				this.mTarget = value;
				this.mTargetHash = this.mTarget.GetHashCodeCustom();
			}
		}

		// Token: 0x0400042E RID: 1070
		public static readonly int ANYID = "any".GetHashCodeCustom();

		// Token: 0x0400042F RID: 1071
		protected string mID;

		// Token: 0x04000430 RID: 1072
		protected int mIDHash;

		// Token: 0x04000431 RID: 1073
		protected string mTarget;

		// Token: 0x04000432 RID: 1074
		protected int mTargetHash;

		// Token: 0x04000433 RID: 1075
		protected string mEffect;

		// Token: 0x04000434 RID: 1076
		protected int mEffectHash;

		// Token: 0x04000435 RID: 1077
		protected string mSound;

		// Token: 0x04000436 RID: 1078
		protected int mSoundHash;
	}
}
