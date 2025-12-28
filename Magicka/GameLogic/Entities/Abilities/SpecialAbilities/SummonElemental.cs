using System;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000177 RID: 375
	public class SummonElemental : SpecialAbility
	{
		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06000B6A RID: 2922 RVA: 0x000442E0 File Offset: 0x000424E0
		public static SummonElemental Instance
		{
			get
			{
				if (SummonElemental.mSingelton == null)
				{
					lock (SummonElemental.mSingeltonLock)
					{
						if (SummonElemental.mSingelton == null)
						{
							SummonElemental.mSingelton = new SummonElemental();
						}
					}
				}
				return SummonElemental.mSingelton;
			}
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x0004436A File Offset: 0x0004256A
		private SummonElemental() : base(Animations.cast_magick_direct, "#magick_selemental".GetHashCodeCustom())
		{
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x00044380 File Offset: 0x00042580
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			Vector3 iDirection = new Vector3((float)SpecialAbility.RANDOM.NextDouble(), 0f, (float)SpecialAbility.RANDOM.NextDouble());
			iDirection.Normalize();
			return this.Execute(iPosition, iDirection, null, iPlayState);
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x000443C0 File Offset: 0x000425C0
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			return this.Execute(iOwner.Position, iOwner.Direction, iOwner, iPlayState);
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x000443E0 File Offset: 0x000425E0
		private bool Execute(Vector3 iPosition, Vector3 iDirection, ISpellCaster iOwner, PlayState iPlayState)
		{
			Vector3 position = iPosition;
			Vector3 direction = iDirection;
			Vector3 vector;
			Vector3.Multiply(ref direction, 3f, out vector);
			Vector3.Add(ref position, ref vector, out position);
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				ElementalEgg instance = ElementalEgg.GetInstance(iPlayState);
				iPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref position, out vector, MovementProperties.Default);
				position = vector;
				instance.Initialize(ref position, ref iDirection, 0);
				instance.SetSummoned(iOwner);
				iPlayState.EntityManager.AddEntity(instance);
				if (NetworkManager.Instance.State == NetworkState.Server && NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnElemental;
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Arg = (int)iOwner.Handle;
					triggerActionMessage.Position = position;
					triggerActionMessage.Direction = direction;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(SummonElemental.EFFECT, ref position, ref direction, out visualEffectReference);
			SummonElemental.sAudioEmitter.Position = iPosition;
			SummonElemental.sAudioEmitter.Up = Vector3.Up;
			SummonElemental.sAudioEmitter.Forward = iDirection;
			AudioManager.Instance.PlayCue(Banks.Spells, SummonElemental.SOUND, SummonElemental.sAudioEmitter);
			return true;
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x00044517 File Offset: 0x00042717
		internal void SetTemplate(CharacterTemplate iTemplate)
		{
			SummonElemental.sTemplate = iTemplate;
		}

		// Token: 0x04000A5B RID: 2651
		private static SummonElemental mSingelton;

		// Token: 0x04000A5C RID: 2652
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04000A5D RID: 2653
		private static CharacterTemplate sTemplate;

		// Token: 0x04000A5E RID: 2654
		public static readonly int SOUND = "magick_summon_elemental".GetHashCodeCustom();

		// Token: 0x04000A5F RID: 2655
		public static readonly int EFFECT = "magick_summonelemental".GetHashCodeCustom();

		// Token: 0x04000A60 RID: 2656
		private static AudioEmitter sAudioEmitter = new AudioEmitter();
	}
}
