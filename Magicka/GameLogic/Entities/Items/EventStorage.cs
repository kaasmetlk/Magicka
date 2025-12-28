using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x02000498 RID: 1176
	[StructLayout(LayoutKind.Explicit)]
	public struct EventStorage
	{
		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x060023AF RID: 9135 RVA: 0x00100FB4 File Offset: 0x000FF1B4
		public EventType EventType
		{
			get
			{
				return this.mEventType;
			}
		}

		// Token: 0x060023B0 RID: 9136 RVA: 0x00100FBC File Offset: 0x000FF1BC
		public EventStorage(DamageEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.Damage;
			this.DamageEvent = iEvent;
		}

		// Token: 0x060023B1 RID: 9137 RVA: 0x00100FD3 File Offset: 0x000FF1D3
		public EventStorage(SplashEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.Splash;
			this.SplashEvent = iEvent;
		}

		// Token: 0x060023B2 RID: 9138 RVA: 0x00100FEA File Offset: 0x000FF1EA
		public EventStorage(RemoveEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.Remove;
			this.RemoveProjectileEvent = iEvent;
		}

		// Token: 0x060023B3 RID: 9139 RVA: 0x00101001 File Offset: 0x000FF201
		public EventStorage(PlayEffectEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.Effect;
			this.PlayEffectEvent = iEvent;
		}

		// Token: 0x060023B4 RID: 9140 RVA: 0x00101018 File Offset: 0x000FF218
		public EventStorage(PlaySoundEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.Sound;
			this.PlaySoundEvent = iEvent;
		}

		// Token: 0x060023B5 RID: 9141 RVA: 0x0010102F File Offset: 0x000FF22F
		public EventStorage(SpawnDecalEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.Decal;
			this.SpawnDecalEvent = iEvent;
		}

		// Token: 0x060023B6 RID: 9142 RVA: 0x00101046 File Offset: 0x000FF246
		public EventStorage(CameraShakeEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.CameraShake;
			this.CameraShakeEvent = iEvent;
		}

		// Token: 0x060023B7 RID: 9143 RVA: 0x0010105D File Offset: 0x000FF25D
		public EventStorage(BlastEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.Blast;
			this.BlastEvent = iEvent;
		}

		// Token: 0x060023B8 RID: 9144 RVA: 0x00101074 File Offset: 0x000FF274
		public EventStorage(SpawnEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.Spawn;
			this.SpawnEvent = iEvent;
		}

		// Token: 0x060023B9 RID: 9145 RVA: 0x0010108B File Offset: 0x000FF28B
		public EventStorage(OverKillEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.Overkill;
			this.OverKillEvent = iEvent;
		}

		// Token: 0x060023BA RID: 9146 RVA: 0x001010A3 File Offset: 0x000FF2A3
		public EventStorage(SpawnItemEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.SpawnItem;
			this.SpawnItemEvent = iEvent;
		}

		// Token: 0x060023BB RID: 9147 RVA: 0x001010BB File Offset: 0x000FF2BB
		public EventStorage(SpawnMagickEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.SpawnMagick;
			this.SpawnMagickEvent = iEvent;
		}

		// Token: 0x060023BC RID: 9148 RVA: 0x001010D3 File Offset: 0x000FF2D3
		public EventStorage(LightEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.Light;
			this.LightEvent = iEvent;
		}

		// Token: 0x060023BD RID: 9149 RVA: 0x001010EB File Offset: 0x000FF2EB
		public EventStorage(CastMagickEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.CastMagick;
			this.CastMagickEvent = iEvent;
		}

		// Token: 0x060023BE RID: 9150 RVA: 0x00101103 File Offset: 0x000FF303
		public EventStorage(CallbackEvent iEvent)
		{
			this = default(EventStorage);
			this.mEventType = EventType.Callback;
			this.CallbackEvent = iEvent;
		}

		// Token: 0x060023BF RID: 9151 RVA: 0x0010111C File Offset: 0x000FF31C
		public DamageResult Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
		{
			DamageResult damageResult = DamageResult.None;
			switch (this.mEventType)
			{
			case EventType.Damage:
				damageResult |= this.DamageEvent.Execute(iItem, iTarget);
				break;
			case EventType.Splash:
				damageResult |= this.SplashEvent.Execute(iItem, iTarget, ref iPosition);
				break;
			case EventType.Sound:
				this.PlaySoundEvent.Execute(iItem, iTarget, ref iPosition);
				break;
			case EventType.Effect:
				this.PlayEffectEvent.Execute(iItem, iTarget, ref iPosition);
				break;
			case EventType.Remove:
				this.RemoveProjectileEvent.Execute(iItem, iTarget);
				break;
			case EventType.CameraShake:
				this.CameraShakeEvent.Execute(iItem, iTarget, ref iPosition);
				break;
			case EventType.Decal:
				this.SpawnDecalEvent.Execute(iItem, iTarget, ref iPosition);
				break;
			case EventType.Blast:
				damageResult |= this.BlastEvent.Execute(iItem, iTarget, ref iPosition);
				break;
			case EventType.Spawn:
				this.SpawnEvent.Execute(iItem, iTarget);
				break;
			case EventType.Overkill:
				this.OverKillEvent.Execute(iItem, iTarget);
				break;
			case EventType.SpawnGibs:
				this.SpawnGibsEvent.Execute(iItem, iTarget);
				break;
			case EventType.SpawnItem:
				this.SpawnItemEvent.Execute(iItem, iTarget);
				break;
			case EventType.SpawnMagick:
				this.SpawnMagickEvent.Execute(iItem, iTarget);
				break;
			case EventType.SpawnMissile:
				this.SpawnMissileEvent.Execute(iItem, iTarget);
				break;
			case EventType.Light:
				this.LightEvent.Execute(iItem, iTarget);
				break;
			case EventType.CastMagick:
				this.CastMagickEvent.Execute(iItem, iTarget);
				break;
			case EventType.DamageOwner:
				this.DamageOwnerEvent.Execute(iItem, iTarget);
				break;
			case EventType.Callback:
				this.CallbackEvent.Execute(iItem, iTarget);
				break;
			}
			return damageResult;
		}

		// Token: 0x060023C0 RID: 9152 RVA: 0x001012C0 File Offset: 0x000FF4C0
		public EventStorage(ContentReader iInput)
		{
			this = default(EventStorage);
			this.mEventType = (EventType)iInput.ReadByte();
			switch (this.mEventType)
			{
			case EventType.Damage:
				this.DamageEvent = new DamageEvent(iInput);
				return;
			case EventType.Splash:
				this.SplashEvent = new SplashEvent(iInput);
				return;
			case EventType.Sound:
				this.PlaySoundEvent = new PlaySoundEvent(iInput);
				return;
			case EventType.Effect:
				this.PlayEffectEvent = new PlayEffectEvent(iInput);
				return;
			case EventType.Remove:
				this.RemoveProjectileEvent = new RemoveEvent(iInput);
				return;
			case EventType.CameraShake:
				this.CameraShakeEvent = new CameraShakeEvent(iInput);
				return;
			case EventType.Decal:
				this.SpawnDecalEvent = new SpawnDecalEvent(iInput);
				return;
			case EventType.Blast:
				this.BlastEvent = new BlastEvent(iInput);
				return;
			case EventType.Spawn:
				this.SpawnEvent = new SpawnEvent(iInput);
				return;
			case EventType.Overkill:
				this.OverKillEvent = new OverKillEvent(iInput);
				return;
			case EventType.SpawnGibs:
				this.SpawnGibsEvent = new SpawnGibsEvent(iInput);
				return;
			case EventType.SpawnItem:
				this.SpawnItemEvent = new SpawnItemEvent(iInput);
				return;
			case EventType.SpawnMagick:
				this.SpawnMagickEvent = new SpawnMagickEvent(iInput);
				return;
			case EventType.SpawnMissile:
				this.SpawnMissileEvent = new SpawnMissileEvent(iInput);
				return;
			case EventType.Light:
				this.LightEvent = new LightEvent(iInput);
				return;
			case EventType.CastMagick:
				this.CastMagickEvent = new CastMagickEvent(iInput);
				return;
			case EventType.DamageOwner:
				this.DamageOwnerEvent = new DamageOwnerEvent(iInput);
				return;
			case EventType.Callback:
				this.CallbackEvent = new CallbackEvent(iInput);
				return;
			default:
				throw new Exception("No event specified");
			}
		}

		// Token: 0x040026D0 RID: 9936
		[FieldOffset(0)]
		private EventType mEventType;

		// Token: 0x040026D1 RID: 9937
		[FieldOffset(4)]
		public DamageEvent DamageEvent;

		// Token: 0x040026D2 RID: 9938
		[FieldOffset(4)]
		public SplashEvent SplashEvent;

		// Token: 0x040026D3 RID: 9939
		[FieldOffset(4)]
		public RemoveEvent RemoveProjectileEvent;

		// Token: 0x040026D4 RID: 9940
		[FieldOffset(4)]
		public PlayEffectEvent PlayEffectEvent;

		// Token: 0x040026D5 RID: 9941
		[FieldOffset(4)]
		public PlaySoundEvent PlaySoundEvent;

		// Token: 0x040026D6 RID: 9942
		[FieldOffset(4)]
		public SpawnDecalEvent SpawnDecalEvent;

		// Token: 0x040026D7 RID: 9943
		[FieldOffset(4)]
		public CameraShakeEvent CameraShakeEvent;

		// Token: 0x040026D8 RID: 9944
		[FieldOffset(4)]
		public BlastEvent BlastEvent;

		// Token: 0x040026D9 RID: 9945
		[FieldOffset(4)]
		public SpawnEvent SpawnEvent;

		// Token: 0x040026DA RID: 9946
		[FieldOffset(4)]
		public OverKillEvent OverKillEvent;

		// Token: 0x040026DB RID: 9947
		[FieldOffset(4)]
		public SpawnGibsEvent SpawnGibsEvent;

		// Token: 0x040026DC RID: 9948
		[FieldOffset(4)]
		public SpawnItemEvent SpawnItemEvent;

		// Token: 0x040026DD RID: 9949
		[FieldOffset(4)]
		public SpawnMagickEvent SpawnMagickEvent;

		// Token: 0x040026DE RID: 9950
		[FieldOffset(4)]
		public SpawnMissileEvent SpawnMissileEvent;

		// Token: 0x040026DF RID: 9951
		[FieldOffset(4)]
		public LightEvent LightEvent;

		// Token: 0x040026E0 RID: 9952
		[FieldOffset(8)]
		public CastMagickEvent CastMagickEvent;

		// Token: 0x040026E1 RID: 9953
		[FieldOffset(4)]
		public DamageOwnerEvent DamageOwnerEvent;

		// Token: 0x040026E2 RID: 9954
		[FieldOffset(4)]
		public CallbackEvent CallbackEvent;
	}
}
