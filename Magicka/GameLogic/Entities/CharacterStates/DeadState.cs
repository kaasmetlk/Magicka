using System;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Statistics;
using Magicka.Network;
using Magicka.WebTools.Paradox.Telemetry;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020003FD RID: 1021
	public class DeadState : BaseState
	{
		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x06001F76 RID: 8054 RVA: 0x000DD75C File Offset: 0x000DB95C
		public static DeadState Instance
		{
			get
			{
				if (DeadState.mSingelton == null)
				{
					lock (DeadState.mSingeltonLock)
					{
						if (DeadState.mSingelton == null)
						{
							DeadState.mSingelton = new DeadState();
						}
					}
				}
				return DeadState.mSingelton;
			}
		}

		// Token: 0x06001F77 RID: 8055 RVA: 0x000DD7B0 File Offset: 0x000DB9B0
		public override void OnEnter(Character iOwner)
		{
			if (iOwner.Gripper != null)
			{
				iOwner.Gripper.ReleaseAttachedCharacter();
			}
			iOwner.ReleaseAttachedCharacter();
			iOwner.ReleaseEntanglement();
			if (!iOwner.Undying)
			{
				if (iOwner is NonPlayerCharacter)
				{
					(iOwner as NonPlayerCharacter).AI.Disable();
				}
				for (int i = 0; i < iOwner.Equipment.Length; i++)
				{
					if (iOwner is Avatar)
					{
						iOwner.Equipment[i].Item.PreviousOwner = iOwner;
					}
					else
					{
						iOwner.Equipment[i].Item.PreviousOwner = null;
					}
					iOwner.Equipment[i].Item.StopEffects();
					iOwner.Equipment[i].Release(iOwner.PlayState);
					iOwner.Equipment[i].Item.Body.SetActive();
				}
			}
			iOwner.CharacterBody.AllowMove = false;
			iOwner.CharacterBody.AllowRotate = false;
			double num = MagickaMath.Random.NextDouble();
			if (iOwner.Drowning)
			{
				if (iOwner.HasAnimation(Animations.die_drown1))
				{
					if (iOwner.HasAnimation(Animations.die_drown2))
					{
						if (num > 0.6660000085830688)
						{
							iOwner.ForceAnimation(Animations.die_drown2);
						}
						else if (num > 0.3330000042915344)
						{
							iOwner.ForceAnimation(Animations.die_drown1);
						}
						else
						{
							iOwner.ForceAnimation(Animations.die_drown);
						}
					}
					else if (num > 0.5)
					{
						iOwner.ForceAnimation(Animations.die_drown1);
					}
					else
					{
						iOwner.ForceAnimation(Animations.die_drown);
					}
				}
				else
				{
					iOwner.ForceAnimation(Animations.die_drown);
				}
			}
			else if (iOwner.HasAnimation(Animations.die1) && num > 0.5)
			{
				iOwner.GoToAnimation(Animations.die1, 0.05f);
			}
			else
			{
				iOwner.GoToAnimation(Animations.die0, 0.05f);
			}
			Avatar avatar = iOwner as Avatar;
			if (avatar != null && avatar.Player != null && avatar.Player == ControlManager.Instance.MenuController.Player)
			{
				TelemetryUtils.SendPlayerDeath(iOwner);
			}
		}

		// Token: 0x06001F78 RID: 8056 RVA: 0x000DD980 File Offset: 0x000DBB80
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			if (iOwner.HitPoints > 0f && !float.IsNaN(iOwner.UndyingTimer) && !iOwner.AnimationController.IsPlaying)
			{
				if (iOwner.Template.AnimationClips[0][150] == null)
				{
					iOwner.SpawnAnimation = Animations.hit;
				}
				else
				{
					iOwner.SpawnAnimation = Animations.revive;
				}
				return RessurectionState.Instance;
			}
			if (iOwner.AnimationController.HasFinished && !iOwner.Dead)
			{
				NetworkState state = NetworkManager.Instance.State;
				if (state == NetworkState.Server)
				{
					CharacterDieMessage characterDieMessage;
					characterDieMessage.Handle = iOwner.Handle;
					characterDieMessage.Drown = iOwner.Drowning;
					characterDieMessage.Overkill = false;
					if (iOwner.LastAttacker != null)
					{
						characterDieMessage.KillerHandle = iOwner.LastAttacker.Handle;
					}
					else
					{
						characterDieMessage.KillerHandle = ushort.MaxValue;
					}
					NetworkManager.Instance.Interface.SendMessage<CharacterDieMessage>(ref characterDieMessage);
				}
				if (state != NetworkState.Client)
				{
					if (!iOwner.NotedKilledEvent)
					{
						StatisticsManager.Instance.AddKillEvent(iOwner.PlayState, iOwner, iOwner.LastAttacker);
						iOwner.NotedKilledEvent = true;
					}
					iOwner.Die();
				}
			}
			else if (iOwner.Overkilled && !iOwner.CannotDieWithoutExplicitKill)
			{
				NetworkState state2 = NetworkManager.Instance.State;
				if (state2 == NetworkState.Server)
				{
					CharacterDieMessage characterDieMessage2;
					characterDieMessage2.Handle = iOwner.Handle;
					characterDieMessage2.Drown = false;
					characterDieMessage2.Overkill = true;
					if (iOwner.LastAttacker != null)
					{
						characterDieMessage2.KillerHandle = iOwner.LastAttacker.Handle;
					}
					else
					{
						characterDieMessage2.KillerHandle = ushort.MaxValue;
					}
					NetworkManager.Instance.Interface.SendMessage<CharacterDieMessage>(ref characterDieMessage2);
				}
				if (state2 != NetworkState.Client)
				{
					if (!iOwner.NotedKilledEvent)
					{
						StatisticsManager.Instance.AddKillEvent(iOwner.PlayState, iOwner, iOwner.LastAttacker);
						iOwner.NotedKilledEvent = true;
					}
					if (!iOwner.mDead)
					{
						iOwner.Die();
					}
					iOwner.RemoveAfterDeath = true;
					if (iOwner.HasGibs())
					{
						iOwner.SpawnGibs();
						AudioManager.Instance.PlayCue(Banks.Misc, DeadState.SOUND_GIB, iOwner.AudioEmitter);
					}
					else if (iOwner.BloatKilled)
					{
						iOwner.Terminate(false, false);
					}
				}
			}
			return null;
		}

		// Token: 0x06001F79 RID: 8057 RVA: 0x000DDB97 File Offset: 0x000DBD97
		public override void OnExit(Character iOwner)
		{
		}

		// Token: 0x040021E3 RID: 8675
		private static DeadState mSingelton;

		// Token: 0x040021E4 RID: 8676
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040021E5 RID: 8677
		public static readonly int SOUND_GIB = "misc_gib".GetHashCodeCustom();
	}
}
