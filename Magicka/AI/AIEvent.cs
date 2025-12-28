using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using Magicka.Levels;

namespace Magicka.AI
{
	// Token: 0x02000192 RID: 402
	[StructLayout(LayoutKind.Explicit)]
	public struct AIEvent
	{
		// Token: 0x06000C1D RID: 3101 RVA: 0x00048A74 File Offset: 0x00046C74
		public AIEvent(LevelModel iLevel, XmlNode iNode)
		{
			this = default(AIEvent);
			this.EventType = (AIEventType)Enum.Parse(typeof(AIEventType), iNode.Name, true);
			switch (this.EventType)
			{
			case AIEventType.Move:
				this.MoveEvent = new MoveEvent(iLevel, iNode);
				return;
			case AIEventType.Animation:
				this.AnimationEvent = new AnimationEvent(iLevel, iNode);
				return;
			case AIEventType.Face:
				this.FaceEvent = new FaceEvent(iLevel, iNode);
				return;
			case AIEventType.Kill:
				this.KillEvent = new KillEvent(iLevel, iNode);
				return;
			case AIEventType.Loop:
				this.LoopEvent = new LoopEvent(iLevel, iNode);
				return;
			default:
				throw new Exception("Invalid EventType!");
			}
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x00048B1C File Offset: 0x00046D1C
		public AIEvent(BinaryReader iReader)
		{
			this = default(AIEvent);
			this.EventType = (AIEventType)iReader.ReadByte();
			switch (this.EventType)
			{
			case AIEventType.Move:
				this.MoveEvent = new MoveEvent(iReader);
				return;
			case AIEventType.Animation:
				this.AnimationEvent = new AnimationEvent(iReader);
				return;
			case AIEventType.Face:
				this.FaceEvent = new FaceEvent(iReader);
				return;
			case AIEventType.Kill:
				this.KillEvent = new KillEvent(iReader);
				return;
			case AIEventType.Loop:
				this.LoopEvent = new LoopEvent(iReader);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x00048BA0 File Offset: 0x00046DA0
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write((byte)this.EventType);
			switch (this.EventType)
			{
			case AIEventType.Move:
				this.MoveEvent.Write(iWriter);
				return;
			case AIEventType.Animation:
				this.AnimationEvent.Write(iWriter);
				return;
			case AIEventType.Face:
				this.FaceEvent.Write(iWriter);
				return;
			case AIEventType.Kill:
				this.KillEvent.Write(iWriter);
				return;
			case AIEventType.Loop:
				this.LoopEvent.Write(iWriter);
				return;
			default:
				return;
			}
		}

		// Token: 0x04000B43 RID: 2883
		[FieldOffset(0)]
		public AIEventType EventType;

		// Token: 0x04000B44 RID: 2884
		[FieldOffset(4)]
		public MoveEvent MoveEvent;

		// Token: 0x04000B45 RID: 2885
		[FieldOffset(4)]
		public AnimationEvent AnimationEvent;

		// Token: 0x04000B46 RID: 2886
		[FieldOffset(4)]
		public FaceEvent FaceEvent;

		// Token: 0x04000B47 RID: 2887
		[FieldOffset(4)]
		public KillEvent KillEvent;

		// Token: 0x04000B48 RID: 2888
		[FieldOffset(4)]
		public LoopEvent LoopEvent;
	}
}
