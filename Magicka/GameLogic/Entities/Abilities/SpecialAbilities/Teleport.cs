using System;
using JigLibX.Geometry;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000540 RID: 1344
	public class Teleport : SpecialAbility
	{
		// Token: 0x17000958 RID: 2392
		// (get) Token: 0x060027F8 RID: 10232 RVA: 0x00123EC0 File Offset: 0x001220C0
		public static Teleport Instance
		{
			get
			{
				if (Teleport.mSingelton == null)
				{
					lock (Teleport.mSingeltonLock)
					{
						if (Teleport.mSingelton == null)
						{
							Teleport.mSingelton = new Teleport();
						}
					}
				}
				return Teleport.mSingelton;
			}
		}

		// Token: 0x060027F9 RID: 10233 RVA: 0x00123F14 File Offset: 0x00122114
		public Teleport(Animations iAnimation) : base(iAnimation, "#magick_teleport".GetHashCodeCustom())
		{
		}

		// Token: 0x060027FA RID: 10234 RVA: 0x00123F48 File Offset: 0x00122148
		private Teleport() : base(Animations.cast_magick_self, "#magick_teleport".GetHashCodeCustom())
		{
		}

		// Token: 0x060027FB RID: 10235 RVA: 0x00123F7D File Offset: 0x0012217D
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Teleport must be called by an entity!");
		}

		// Token: 0x060027FC RID: 10236 RVA: 0x00123F8C File Offset: 0x0012218C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				base.Execute(iOwner, iPlayState);
				Vector3 direction = iOwner.Direction;
				Vector3 position = iOwner.Position;
				float scaleFactor = 10f;
				Vector3.Multiply(ref direction, scaleFactor, out direction);
				Vector3.Add(ref direction, ref position, out position);
				return this.DoTeleport(iOwner, position, iOwner.Direction, Teleport.TeleportType.Regular);
			}
			return true;
		}

		// Token: 0x060027FD RID: 10237 RVA: 0x00123FEC File Offset: 0x001221EC
		public bool DoTeleport(ISpellCaster iOwner, Vector3 iPosition, Vector3 iDirection, Teleport.TeleportType iTeleportType)
		{
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
				characterActionMessage.Handle = iOwner.Handle;
				characterActionMessage.Param0F = iPosition.X;
				characterActionMessage.Param1F = iPosition.Y;
				characterActionMessage.Param2F = iPosition.Z;
				characterActionMessage.Action = ActionType.Magick;
				characterActionMessage.Param3I = 5;
				characterActionMessage.Param4I = (int)iTeleportType;
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
			}
			PlayState playState = iOwner.PlayState;
			Vector3 position = iOwner.Position;
			Vector3 forward = iDirection;
			Cue cue = null;
			switch (iTeleportType)
			{
			case Teleport.TeleportType.Regular:
			{
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_DISAPPEAR, ref position, ref forward, out visualEffectReference);
				cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_ORIGIN);
				break;
			}
			case Teleport.TeleportType.SmokeBomb:
			{
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Teleport.SMOKEBOMB_EFFECT_DISAPPEAR, ref position, ref forward, out visualEffectReference);
				cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.SMOKEBOMB_SOUND);
				break;
			}
			}
			this.mOriginEmitter.Up = Vector3.Up;
			this.mOriginEmitter.Position = position;
			this.mOriginEmitter.Forward = forward;
			cue.Apply3D(playState.Camera.Listener, this.mOriginEmitter);
			cue.Play();
			Vector3 vector;
			playState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out vector, MovementProperties.Water);
			iPosition = vector;
			Segment segment = default(Segment);
			segment.Origin = vector;
			segment.Delta.Y = -4f;
			float num;
			Vector3 vector2;
			Vector3 vector3;
			if (playState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, segment))
			{
				iPosition = vector2;
				iPosition.Y += 0.1f;
			}
			if (iOwner is Character)
			{
				Character character = iOwner as Character;
				iPosition.Y -= character.HeightOffset;
				character.ReleaseEntanglement();
				character.ReleaseAttachedCharacter();
				if (character.Gripper != null)
				{
					character.Gripper.ReleaseAttachedCharacter();
				}
			}
			else
			{
				iPosition.Y += iOwner.Radius;
			}
			switch (iTeleportType)
			{
			case Teleport.TeleportType.Regular:
			{
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Teleport.TELEPORT_EFFECT_APPEAR, ref iPosition, ref forward, out visualEffectReference);
				cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.TELEPORT_SOUND_DESTINATION);
				break;
			}
			case Teleport.TeleportType.SmokeBomb:
			{
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Teleport.SMOKEBOMB_EFFECT_DISAPPEAR, ref iPosition, ref forward, out visualEffectReference);
				cue = AudioManager.Instance.GetCue(Banks.Spells, Teleport.SMOKEBOMB_SOUND);
				break;
			}
			}
			this.mDestinationEmitter.Up = Vector3.Up;
			this.mDestinationEmitter.Position = iPosition;
			this.mDestinationEmitter.Forward = forward;
			cue.Apply3D(playState.Camera.Listener, this.mDestinationEmitter);
			cue.Play();
			if (iOwner is Entity)
			{
				Entity iEntity = iOwner as Entity;
				segment.Origin = position;
				Vector3.Subtract(ref iPosition, ref segment.Origin, out segment.Delta);
				foreach (TriggerArea triggerArea in playState.Level.CurrentScene.LevelModel.TriggerAreas.Values)
				{
					float num2;
					Vector3 vector4;
					Vector3 vector5;
					if (!(triggerArea is AnyTriggerArea) && triggerArea.CollisionSkin.SegmentIntersect(out num2, out vector4, out vector5, segment))
					{
						triggerArea.AddEntity(iEntity);
					}
				}
			}
			Matrix orientation = iOwner.Body.Orientation;
			if (iOwner is Avatar)
			{
				Avatar avatar = iOwner as Avatar;
				Segment iSeg = default(Segment);
				iSeg.Origin = iOwner.Position;
				iSeg.Origin.Y = iSeg.Origin.Y + 0.5f;
				iSeg.Delta.Y = -15.5f;
				bool flag = true;
				float num3;
				Vector3 vector6;
				Vector3 vector7;
				for (int i = 0; i < playState.Level.CurrentScene.Liquids.Length; i++)
				{
					if (playState.Level.CurrentScene.Liquids[i].SegmentIntersect(out num3, out vector6, out vector7, ref iSeg, true, false, false))
					{
						flag = false;
					}
				}
				if (!(avatar.Player.Gamer is NetworkGamer) && !playState.Level.CurrentScene.SegmentIntersect(out num3, out vector6, out vector7, iSeg) && flag)
				{
					AchievementsManager.Instance.AwardAchievement(iOwner.PlayState, "101stairborne");
				}
				iOwner.Body.MoveTo(iPosition, orientation);
				avatar.ResetAfterImages();
			}
			else
			{
				iOwner.Body.MoveTo(iPosition, orientation);
			}
			return true;
		}

		// Token: 0x04002B85 RID: 11141
		private static Teleport mSingelton;

		// Token: 0x04002B86 RID: 11142
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04002B87 RID: 11143
		public static readonly int TELEPORT_EFFECT_DISAPPEAR = "magick_teleport_disappear".GetHashCodeCustom();

		// Token: 0x04002B88 RID: 11144
		public static readonly int TELEPORT_EFFECT_APPEAR = "magick_teleport_appear".GetHashCodeCustom();

		// Token: 0x04002B89 RID: 11145
		public static readonly int TELEPORT_SOUND_ORIGIN = "magick_teleporta".GetHashCodeCustom();

		// Token: 0x04002B8A RID: 11146
		public static readonly int TELEPORT_SOUND_DESTINATION = "magick_teleportb".GetHashCodeCustom();

		// Token: 0x04002B8B RID: 11147
		public static readonly int SMOKEBOMB_EFFECT_DISAPPEAR = "smoke_bomb".GetHashCodeCustom();

		// Token: 0x04002B8C RID: 11148
		public static readonly int SMOKEBOMB_EFFECT_APPEAR = "smoke_bomb".GetHashCodeCustom();

		// Token: 0x04002B8D RID: 11149
		public static readonly int SMOKEBOMB_SOUND = "spell_fire_self".GetHashCodeCustom();

		// Token: 0x04002B8E RID: 11150
		private AudioEmitter mOriginEmitter = new AudioEmitter();

		// Token: 0x04002B8F RID: 11151
		private AudioEmitter mDestinationEmitter = new AudioEmitter();

		// Token: 0x04002B90 RID: 11152
		private Random mRandom = new Random();

		// Token: 0x02000541 RID: 1345
		public enum TeleportType
		{
			// Token: 0x04002B92 RID: 11154
			Regular = 1,
			// Token: 0x04002B93 RID: 11155
			SmokeBomb
		}
	}
}
