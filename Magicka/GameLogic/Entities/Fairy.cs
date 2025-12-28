using System;
using JigLibX.Collision;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Network;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020004D4 RID: 1236
	internal class Fairy : Entity
	{
		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x060024C1 RID: 9409 RVA: 0x00109A4B File Offset: 0x00107C4B
		// (set) Token: 0x060024C2 RID: 9410 RVA: 0x00109A53 File Offset: 0x00107C53
		public bool Active
		{
			get
			{
				return this.mActive;
			}
			private set
			{
				this.mActive = value;
			}
		}

		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x060024C3 RID: 9411 RVA: 0x00109A5C File Offset: 0x00107C5C
		// (set) Token: 0x060024C4 RID: 9412 RVA: 0x00109A64 File Offset: 0x00107C64
		public float WaitBeforeRevive
		{
			get
			{
				return this.mWaitBeforeRevive;
			}
			set
			{
				this.mWaitBeforeRevive = value;
			}
		}

		// Token: 0x060024C5 RID: 9413 RVA: 0x00109A70 File Offset: 0x00107C70
		public static Fairy MakeFairy(PlayState iPlayState, Character iAvatar)
		{
			return new Fairy(iPlayState, iAvatar);
		}

		// Token: 0x060024C6 RID: 9414 RVA: 0x00109A88 File Offset: 0x00107C88
		private Fairy(PlayState iPlayState, Character iOwner) : base(iPlayState)
		{
			this.mOwner = iOwner;
			this.Active = false;
			this.mBody = new Body();
			this.mBody.ApplyGravity = false;
			this.mBody.AllowFreezing = false;
			this.mCollision = new CollisionSkin(this.mBody);
			this.mBody.CollisionSkin = this.mCollision;
		}

		// Token: 0x060024C7 RID: 9415 RVA: 0x00109B14 File Offset: 0x00107D14
		public void Initialize(PlayState iPlayState, bool iShowGreetingDialog)
		{
			base.Initialize("#ent_fairy".GetHashCodeCustom());
			this.mPlayState = iPlayState;
			this.mPlayState.EntityManager.AddEntity(this);
			if (!this.Active && iShowGreetingDialog)
			{
				this.ShowGreetingDialog();
			}
			this.Active = true;
			this.Play();
			if (Fairy.sDialogTimer <= 0f)
			{
				Fairy.sDialogTimer = Fairy.sTimeUntilNextDialog;
			}
			this.mDeathsoundPlayed = false;
		}

		// Token: 0x060024C8 RID: 9416 RVA: 0x00109B84 File Offset: 0x00107D84
		private void Play()
		{
			this.mCurrentState = Fairy.FairyState.Orbiting;
			if (EffectManager.Instance.IsActive(ref this.mEffectRef))
			{
				return;
			}
			Matrix matrix = Matrix.CreateTranslation(this.mOwner.Position);
			EffectManager.Instance.StartEffect("special_fairy".GetHashCodeCustom(), ref matrix, out this.mEffectRef);
		}

		// Token: 0x060024C9 RID: 9417 RVA: 0x00109BD9 File Offset: 0x00107DD9
		private void Stop()
		{
			EffectManager.Instance.Stop(ref this.mEffectRef);
		}

		// Token: 0x060024CA RID: 9418 RVA: 0x00109BEC File Offset: 0x00107DEC
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (!this.Active)
			{
				return;
			}
			base.Update(iDataChannel, iDeltaTime);
			bool flag = this.CheckDialogs(iDeltaTime);
			if (!this.mFirstCheckAfterCheckFail && !flag)
			{
				this.mFirstCheckAfterCheckFail = true;
			}
			switch (this.mCurrentState)
			{
			case Fairy.FairyState.Orbiting:
				if (this.mOwner == null || !this.mOwner.Dead)
				{
					this.OrbitAround(iDeltaTime);
					return;
				}
				if (this.mWaitBeforeRevive > 0f)
				{
					this.mWaitBeforeRevive -= iDeltaTime;
				}
				if (this.mWaitBeforeRevive <= 0f)
				{
					this.mWaitBeforeRevive = 0f;
					this.FindRevivePosition();
					return;
				}
				this.OrbitAround(iDeltaTime);
				return;
			case Fairy.FairyState.FlyToRevivePosition:
				this.FlyTowardsTarget(iDeltaTime);
				return;
			case Fairy.FairyState.Reviving:
				this.ReviveAvatar();
				return;
			case Fairy.FairyState.WaitingForAvatar:
				if (this.mOwner is Avatar && !(this.mOwner as Avatar).Player.Avatar.Dead)
				{
					this.Active = false;
					(this.mOwner as Avatar).Player.Avatar.TimedEthereal(0f, true);
					(this.mOwner as Avatar).Player.Avatar.TimedEthereal(5f, false);
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x060024CB RID: 9419 RVA: 0x00109D20 File Offset: 0x00107F20
		private void FindRevivePosition()
		{
			if (this.mPlayState.Camera.CurrentBehaviour == CameraBehaviour.FollowPlayers)
			{
				this.mPlayState.Camera.Follow(this);
			}
			this.mAnimPosition = 0f;
			this.mDeathPos = this.mBody.Position;
			this.mRevivalTarget = this.mOwner.CharacterBody.LastPositionOnGround;
			NavMesh navMesh = this.mPlayState.Level.CurrentScene.NavMesh;
			Vector3 vector;
			navMesh.GetNearestPosition(ref this.mRevivalTarget, out vector, MovementProperties.Default);
			this.mRevivalTarget = vector;
			this.mCurrentState = Fairy.FairyState.FlyToRevivePosition;
			this.mOrbitRadius = this.mDefaultOrbitRadius * 1f;
		}

		// Token: 0x060024CC RID: 9420 RVA: 0x00109DC8 File Offset: 0x00107FC8
		private void OrbitAround(float iDeltaTime)
		{
			this.mDefaultOrbitRadius = 0.8f;
			this.mOrbitRadius = this.mDefaultOrbitRadius;
			float num = 0.25f;
			float num2 = 6.2831855f * num * this.mTime;
			float num3 = this.mOrbitRadius * (float)Math.Cos((double)num2);
			float num4 = this.mOrbitRadius * (float)Math.Sin((double)num2);
			float num5 = 0.25f;
			float num6 = 0.35f;
			num2 = 6.2831855f * num6 * this.mTime;
			float num7 = num5 * (float)Math.Sin((double)num2);
			float num8 = 1f;
			num7 += num8;
			Vector3 position = this.mOwner.Position;
			Vector3 vector = new Vector3(position.X + num3, position.Y + num7, position.Z + num4);
			Vector3 direction = this.mOwner.Direction;
			this.mTime += iDeltaTime;
			this.mBody.Position = this.mOwner.Position;
			EffectManager.Instance.UpdatePositionDirection(ref this.mEffectRef, ref vector, ref direction);
		}

		// Token: 0x060024CD RID: 9421 RVA: 0x00109ED8 File Offset: 0x001080D8
		private void FlyTowardsTarget(float iDeltaTime)
		{
			this.mAnimPosition += iDeltaTime;
			float num = 2.1f;
			if (num - this.mAnimPosition < 1.4f && !this.mDeathsoundPlayed)
			{
				this.mDeathsoundPlayed = true;
				AudioManager.Instance.PlayCue(Banks.Additional, "fairy_death".GetHashCodeCustom(), base.AudioEmitter);
			}
			if (this.mAnimPosition >= num)
			{
				this.mCurrentState = Fairy.FairyState.Reviving;
				return;
			}
			Vector3 position = default(Vector3);
			float num2;
			if (this.mAnimPosition < 1f)
			{
				num2 = this.mAnimPosition / 1f;
			}
			else
			{
				num2 = 1f - (this.mAnimPosition - 1f) / 1f;
			}
			num2 *= 1.5f;
			float num3 = (this.mAnimPosition - 1f) * 12.566371f;
			float num4;
			if (this.mAnimPosition > 2f)
			{
				num4 = 1f - (this.mAnimPosition - 1f - 1f) / 0.1f;
			}
			else if (this.mAnimPosition > 1f)
			{
				num4 = (this.mAnimPosition - 1f) / 1f;
			}
			else
			{
				num4 = 0f;
			}
			num4 *= 2f;
			position.Y = this.mRevivalTarget.Y + num4;
			position.X = this.mRevivalTarget.X + (float)Math.Cos((double)num3) * num2;
			position.Z = this.mRevivalTarget.Z + (float)Math.Sin((double)num3) * num2;
			if (this.mAnimPosition < 1f)
			{
				Vector3.SmoothStep(ref this.mDeathPos, ref position, this.mAnimPosition / 1f, out position);
			}
			Vector3 forward = Vector3.Forward;
			EffectManager.Instance.UpdatePositionDirection(ref this.mEffectRef, ref position, ref forward);
			if (this.mAnimPosition < 1f)
			{
				Vector3.SmoothStep(ref this.mDeathPos, ref this.mRevivalTarget, this.mAnimPosition / 1f, out position);
			}
			else
			{
				position = this.mRevivalTarget;
			}
			this.mBody.Position = position;
		}

		// Token: 0x060024CE RID: 9422 RVA: 0x0010A0D8 File Offset: 0x001082D8
		private void ReviveAvatar()
		{
			EffectManager.Instance.Stop(ref this.mEffectRef);
			Matrix matrix = Matrix.CreateTranslation(this.mRevivalTarget);
			EffectManager.Instance.StartEffect("special_fairy_crash".GetHashCodeCustom(), ref matrix, out this.mCrashEffectRef);
			if (this.mPlayState.Camera.CurrentBehaviour == CameraBehaviour.FollowEntity && this.mPlayState.Camera.TargetEntity == this)
			{
				this.mPlayState.Camera.Release_NoMagReset(2f);
			}
			Revive instance = Revive.GetInstance();
			instance.Execute(this.mRevivalTarget, this.mPlayState);
			this.mCurrentState = Fairy.FairyState.WaitingForAvatar;
			Damage damage = new Damage(AttackProperties.Knockback, Elements.Earth, 100f, 2f);
			Vector3 position = this.mBody.Position;
			Helper.CircleDamage(this.mPlayState, this, this.mPlayState.PlayTime, this, ref position, 3f, ref damage);
			if (this.mOwner is Avatar)
			{
				this.mPlayState.RemoveFairyFrom(this.mOwner as Avatar);
			}
			this.mDeathsoundPlayed = false;
		}

		// Token: 0x060024CF RID: 9423 RVA: 0x0010A1EC File Offset: 0x001083EC
		private void ShowGreetingDialog()
		{
			int num = MagickaMath.Random.Next(Fairy.GREETING_DIALOG_ID.Length);
			int iDialog = Fairy.GREETING_DIALOG_ID[num];
			DialogManager.Instance.StartDialog(iDialog, this, null);
		}

		// Token: 0x060024D0 RID: 9424 RVA: 0x0010A220 File Offset: 0x00108420
		private void ShowRandomDialog()
		{
			int num;
			for (num = this.mLastDialogGreeting; num == this.mLastDialogGreeting; num = MagickaMath.Random.Next(Fairy.DIALOG_ID.Length))
			{
			}
			int num2;
			for (num2 = this.mLastDialogTip; num2 == this.mLastDialogTip; num2 = MagickaMath.Random.Next(Fairy.DIALOG_ID[0].Length))
			{
			}
			this.mLastDialogGreeting = num;
			this.mLastDialogTip = num2;
			int iDialog = Fairy.DIALOG_ID[num][num2];
			DialogManager.Instance.StartDialog(iDialog, this, null);
		}

		// Token: 0x060024D1 RID: 9425 RVA: 0x0010A29C File Offset: 0x0010849C
		private bool CheckDialogs(float iDeltaTime)
		{
			if (this.mPlayState.IsInCutscene || this.mCurrentState != Fairy.FairyState.Orbiting)
			{
				if (this.mLastDialogGreeting != -1 && this.mLastDialogTip != -1)
				{
					int iDialog = Fairy.DIALOG_ID[this.mLastDialogGreeting][this.mLastDialogTip];
					if (DialogManager.Instance.DialogActive(iDialog))
					{
						DialogManager.Instance.End(iDialog);
					}
				}
				return false;
			}
			if (DialogManager.Instance.MessageBoxActive | DialogManager.Instance.IsDialogActive | DialogManager.Instance.AwaitingInput)
			{
				return false;
			}
			if (ControlManager.Instance.IsInputLimited)
			{
				return false;
			}
			if (AudioManager.Instance.Threat)
			{
				return false;
			}
			if (BossFight.Instance.IsRunning)
			{
				return false;
			}
			if (Credits.Instance.IsActive)
			{
				return false;
			}
			if (this.mFirstCheckAfterCheckFail)
			{
				Fairy.sDialogTimer = Math.Max(Fairy.sDialogTimer, 15f);
				this.mFirstCheckAfterCheckFail = false;
			}
			Fairy.sDialogTimer -= iDeltaTime;
			if (Fairy.sDialogTimer <= 0f)
			{
				this.ShowRandomDialog();
				if (Fairy.sTimeUntilNextDialog < 60f)
				{
					Fairy.sTimeUntilNextDialog += 5f;
				}
				Fairy.sDialogTimer = Fairy.sTimeUntilNextDialog;
			}
			return true;
		}

		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x060024D2 RID: 9426 RVA: 0x0010A3C2 File Offset: 0x001085C2
		public override bool Dead
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x060024D3 RID: 9427 RVA: 0x0010A3C5 File Offset: 0x001085C5
		public override bool Removable
		{
			get
			{
				return !this.mActive;
			}
		}

		// Token: 0x060024D4 RID: 9428 RVA: 0x0010A3D0 File Offset: 0x001085D0
		public override void Kill()
		{
		}

		// Token: 0x060024D5 RID: 9429 RVA: 0x0010A3D2 File Offset: 0x001085D2
		internal override bool SendsNetworkUpdate(NetworkState iState)
		{
			return false;
		}

		// Token: 0x060024D6 RID: 9430 RVA: 0x0010A3D5 File Offset: 0x001085D5
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
		}

		// Token: 0x04002825 RID: 10277
		private const float TIME_UNTIL_NEXT_DIALOG_CAP = 60f;

		// Token: 0x04002826 RID: 10278
		private Character mOwner;

		// Token: 0x04002827 RID: 10279
		private Fairy.FairyState mCurrentState;

		// Token: 0x04002828 RID: 10280
		private Vector3 mDeathPos;

		// Token: 0x04002829 RID: 10281
		private Vector3 mRevivalTarget;

		// Token: 0x0400282A RID: 10282
		private float mAnimPosition;

		// Token: 0x0400282B RID: 10283
		private VisualEffectReference mEffectRef;

		// Token: 0x0400282C RID: 10284
		private VisualEffectReference mCrashEffectRef;

		// Token: 0x0400282D RID: 10285
		private float mTime;

		// Token: 0x0400282E RID: 10286
		private float mDefaultOrbitRadius = 0.5f;

		// Token: 0x0400282F RID: 10287
		private float mOrbitRadius = 0.5f;

		// Token: 0x04002830 RID: 10288
		private float mWaitBeforeRevive;

		// Token: 0x04002831 RID: 10289
		private bool mDeathsoundPlayed;

		// Token: 0x04002832 RID: 10290
		private static float sTimeUntilNextDialog = 30f;

		// Token: 0x04002833 RID: 10291
		private static float sDialogTimer;

		// Token: 0x04002834 RID: 10292
		private int mLastDialogGreeting = -1;

		// Token: 0x04002835 RID: 10293
		private int mLastDialogTip = -1;

		// Token: 0x04002836 RID: 10294
		private bool mFirstCheckAfterCheckFail;

		// Token: 0x04002837 RID: 10295
		private static readonly int[] GREETING_DIALOG_ID = new int[]
		{
			"fairy_greet0".GetHashCodeCustom(),
			"fairy_greet1".GetHashCodeCustom()
		};

		// Token: 0x04002838 RID: 10296
		private static readonly int[][] DIALOG_ID = new int[][]
		{
			new int[]
			{
				"fairy20".GetHashCodeCustom(),
				"fairy21".GetHashCodeCustom(),
				"fairy22".GetHashCodeCustom(),
				"fairy23".GetHashCodeCustom(),
				"fairy24".GetHashCodeCustom()
			},
			new int[]
			{
				"fairy30".GetHashCodeCustom(),
				"fairy31".GetHashCodeCustom(),
				"fairy32".GetHashCodeCustom(),
				"fairy33".GetHashCodeCustom(),
				"fairy34".GetHashCodeCustom()
			}
		};

		// Token: 0x04002839 RID: 10297
		private bool mActive;

		// Token: 0x020004D5 RID: 1237
		private enum FairyState
		{
			// Token: 0x0400283B RID: 10299
			Orbiting,
			// Token: 0x0400283C RID: 10300
			FlyToRevivePosition,
			// Token: 0x0400283D RID: 10301
			Reviving,
			// Token: 0x0400283E RID: 10302
			WaitingForAvatar
		}
	}
}
