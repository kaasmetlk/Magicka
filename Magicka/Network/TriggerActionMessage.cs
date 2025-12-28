using System;
using System.IO;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x02000413 RID: 1043
	internal struct TriggerActionMessage : ISendable
	{
		// Token: 0x170007E8 RID: 2024
		// (get) Token: 0x06002053 RID: 8275 RVA: 0x000E4449 File Offset: 0x000E2649
		public PacketType PacketType
		{
			get
			{
				return PacketType.TriggerAction;
			}
		}

		// Token: 0x06002054 RID: 8276 RVA: 0x000E4450 File Offset: 0x000E2650
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write((byte)this.ActionType);
			switch (this.ActionType)
			{
			case TriggerActionType.TriggerExecute:
				iWriter.Write(this.Scene);
				iWriter.Write(this.Id);
				iWriter.Write(this.Handle);
				iWriter.Write(this.Arg);
				return;
			case TriggerActionType.SpawnNPC:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Template);
				iWriter.Write(this.Id);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write(new Normalized101010(this.Direction).PackedValue);
				iWriter.Write(this.Point0);
				iWriter.Write(this.Point1);
				iWriter.Write(this.Point2);
				iWriter.Write(this.Point3);
				iWriter.Write(this.Bool0);
				iWriter.Write(this.Arg);
				iWriter.Write(this.Scene);
				iWriter.Write(new HalfSingle(this.Color.X).PackedValue);
				iWriter.Write(new HalfSingle(this.Color.Y).PackedValue);
				iWriter.Write(new HalfSingle(this.Color.Z).PackedValue);
				return;
			case TriggerActionType.SpawnElemental:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Time);
				iWriter.Write(this.Id);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write(new Normalized101010(this.Direction).PackedValue);
				return;
			case TriggerActionType.SpawnLuggage:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Template);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write(new Normalized101010(this.Direction).PackedValue);
				iWriter.Write(this.Point0);
				return;
			case TriggerActionType.SpawnFood:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write(new Normalized101010(this.Orientation.X, this.Orientation.Y, this.Orientation.Z).PackedValue);
				iWriter.Write(new HalfSingle(this.Orientation.W).PackedValue);
				return;
			case TriggerActionType.SpawnItem:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Template);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write(this.Time);
				iWriter.Write(new Normalized101010(this.Direction).PackedValue);
				iWriter.Write(new Normalized101010(this.Orientation.X, this.Orientation.Y, this.Orientation.Z).PackedValue);
				iWriter.Write(new HalfSingle(this.Orientation.W).PackedValue);
				iWriter.Write(this.Point0);
				iWriter.Write(this.Point1);
				iWriter.Write(this.Bool0);
				iWriter.Write(this.Bool1);
				return;
			case TriggerActionType.SpawnMagick:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Template);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write(this.Time);
				iWriter.Write(new Normalized101010(this.Direction).PackedValue);
				iWriter.Write(new Normalized101010(this.Orientation.X, this.Orientation.Y, this.Orientation.Z).PackedValue);
				iWriter.Write(new HalfSingle(this.Orientation.W).PackedValue);
				iWriter.Write(this.Point0);
				iWriter.Write(this.Bool0);
				return;
			case TriggerActionType.ChangeScene:
				iWriter.Write(this.Scene);
				iWriter.Write(this.Point0);
				iWriter.Write(this.Point1);
				iWriter.Write(this.Point2);
				iWriter.Write(this.Point3);
				iWriter.Write(this.Bool0);
				iWriter.Write(this.Bool1);
				iWriter.Write(this.Template);
				iWriter.Write(this.Time);
				return;
			case TriggerActionType.EnterScene:
				iWriter.Write(this.Scene);
				iWriter.Write(this.Point0);
				iWriter.Write(this.Point1);
				iWriter.Write(this.Point2);
				iWriter.Write(this.Point3);
				iWriter.Write(this.Target0);
				iWriter.Write(this.Target1);
				iWriter.Write(this.Target2);
				iWriter.Write(this.Target3);
				iWriter.Write(this.Arg);
				iWriter.Write(this.Bool0);
				iWriter.Write(this.Bool1);
				iWriter.Write(this.Bool2);
				iWriter.Write(this.Template);
				iWriter.Write(this.Time);
				return;
			case TriggerActionType.ThunderBolt:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Id);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				return;
			case TriggerActionType.LightningBolt:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Id);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write((ushort)this.Spell.Element);
				for (int i = 0; i < 11; i++)
				{
					Elements elements = Defines.ElementFromIndex(i);
					if ((elements & this.Spell.Element) == elements)
					{
						iWriter.Write(new HalfSingle(this.Spell[elements]).PackedValue);
					}
				}
				iWriter.Write(new HalfSingle(this.Splash).PackedValue);
				return;
			case TriggerActionType.SpawnGrease:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Arg);
				iWriter.Write(this.Id);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write(new Normalized101010(this.Direction).PackedValue);
				return;
			case TriggerActionType.Nullify:
				iWriter.Write(this.Bool0);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				return;
			case TriggerActionType.SpawnTornado:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Id);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write(new Normalized101010(this.Orientation.X, this.Orientation.Y, this.Orientation.Z).PackedValue);
				iWriter.Write(new HalfSingle(this.Orientation.W).PackedValue);
				return;
			case TriggerActionType.NapalmStrike:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Arg);
				iWriter.Write(this.Id);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write(new Normalized101010(this.Direction).PackedValue);
				iWriter.Write(this.TimeStamp);
				return;
			case TriggerActionType.EarthQuake:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				iWriter.Write(this.Time);
				return;
			case TriggerActionType.Charm:
				iWriter.Write(this.Handle);
				iWriter.Write((ushort)this.Id);
				iWriter.Write(this.Time);
				iWriter.Write(this.Arg);
				return;
			case TriggerActionType.Starfall:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
				return;
			case TriggerActionType.OtherworldlyDischarge:
			case TriggerActionType.StarGaze:
				iWriter.Write(this.Handle);
				iWriter.Write((ushort)this.Arg);
				return;
			case TriggerActionType.OtherworldlyBoltDestroyed:
				iWriter.Write(this.Handle);
				iWriter.Write((ushort)this.Arg);
				iWriter.Write(this.Bool0);
				iWriter.Write(this.Bool1);
				return;
			case TriggerActionType.Confuse:
				iWriter.Write(this.Handle);
				iWriter.Write(this.Bool0);
				return;
			default:
				throw new NotImplementedException();
			}
		}

		// Token: 0x06002055 RID: 8277 RVA: 0x000E4E94 File Offset: 0x000E3094
		public void Read(BinaryReader iReader)
		{
			this.ActionType = (TriggerActionType)iReader.ReadByte();
			switch (this.ActionType)
			{
			case TriggerActionType.TriggerExecute:
				this.Scene = iReader.ReadInt32();
				this.Id = iReader.ReadInt32();
				this.Handle = iReader.ReadUInt16();
				this.Arg = iReader.ReadInt32();
				return;
			case TriggerActionType.SpawnNPC:
			{
				this.Handle = iReader.ReadUInt16();
				this.Template = iReader.ReadInt32();
				this.Id = iReader.ReadInt32();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				Normalized101010 normalized = default(Normalized101010);
				normalized.PackedValue = iReader.ReadUInt32();
				this.Direction = normalized.ToVector3();
				this.Point0 = iReader.ReadInt32();
				this.Point1 = iReader.ReadInt32();
				this.Point2 = iReader.ReadInt32();
				this.Point3 = iReader.ReadInt32();
				this.Bool0 = iReader.ReadBoolean();
				this.Arg = iReader.ReadInt32();
				this.Scene = iReader.ReadInt32();
				HalfSingle halfSingle = default(HalfSingle);
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Color.X = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Color.Y = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Color.Z = halfSingle.ToSingle();
				return;
			}
			case TriggerActionType.SpawnElemental:
			{
				this.Handle = iReader.ReadUInt16();
				this.Time = iReader.ReadSingle();
				this.Id = iReader.ReadInt32();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				Normalized101010 normalized2 = default(Normalized101010);
				normalized2.PackedValue = iReader.ReadUInt32();
				this.Direction = normalized2.ToVector3();
				return;
			}
			case TriggerActionType.SpawnLuggage:
			{
				this.Handle = iReader.ReadUInt16();
				this.Template = iReader.ReadInt32();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				Normalized101010 normalized3 = default(Normalized101010);
				normalized3.PackedValue = iReader.ReadUInt32();
				this.Direction = normalized3.ToVector3();
				this.Point0 = iReader.ReadInt32();
				return;
			}
			case TriggerActionType.SpawnFood:
			{
				this.Handle = iReader.ReadUInt16();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				Normalized101010 normalized4 = default(Normalized101010);
				normalized4.PackedValue = iReader.ReadUInt32();
				HalfSingle halfSingle2 = default(HalfSingle);
				halfSingle2.PackedValue = iReader.ReadUInt16();
				this.Orientation = new Quaternion(normalized4.ToVector3(), halfSingle2.ToSingle());
				return;
			}
			case TriggerActionType.SpawnItem:
			{
				this.Handle = iReader.ReadUInt16();
				this.Template = iReader.ReadInt32();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				this.Time = iReader.ReadSingle();
				Normalized101010 normalized5 = default(Normalized101010);
				normalized5.PackedValue = iReader.ReadUInt32();
				this.Direction = normalized5.ToVector3();
				normalized5.PackedValue = iReader.ReadUInt32();
				HalfSingle halfSingle3 = default(HalfSingle);
				halfSingle3.PackedValue = iReader.ReadUInt16();
				this.Orientation = new Quaternion(normalized5.ToVector3(), halfSingle3.ToSingle());
				this.Point0 = iReader.ReadInt32();
				this.Point1 = iReader.ReadInt32();
				this.Bool0 = iReader.ReadBoolean();
				this.Bool1 = iReader.ReadBoolean();
				return;
			}
			case TriggerActionType.SpawnMagick:
			{
				this.Handle = iReader.ReadUInt16();
				this.Template = iReader.ReadInt32();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				this.Time = iReader.ReadSingle();
				Normalized101010 normalized6 = default(Normalized101010);
				normalized6.PackedValue = iReader.ReadUInt32();
				this.Direction = normalized6.ToVector3();
				normalized6.PackedValue = iReader.ReadUInt32();
				HalfSingle halfSingle4 = default(HalfSingle);
				halfSingle4.PackedValue = iReader.ReadUInt16();
				this.Orientation = new Quaternion(normalized6.ToVector3(), halfSingle4.ToSingle());
				this.Point0 = iReader.ReadInt32();
				this.Bool0 = iReader.ReadBoolean();
				return;
			}
			case TriggerActionType.ChangeScene:
				this.Scene = iReader.ReadInt32();
				this.Point0 = iReader.ReadInt32();
				this.Point1 = iReader.ReadInt32();
				this.Point2 = iReader.ReadInt32();
				this.Point3 = iReader.ReadInt32();
				this.Bool0 = iReader.ReadBoolean();
				this.Bool1 = iReader.ReadBoolean();
				this.Template = iReader.ReadInt32();
				this.Time = iReader.ReadSingle();
				return;
			case TriggerActionType.EnterScene:
				this.Scene = iReader.ReadInt32();
				this.Point0 = iReader.ReadInt32();
				this.Point1 = iReader.ReadInt32();
				this.Point2 = iReader.ReadInt32();
				this.Point3 = iReader.ReadInt32();
				this.Target0 = iReader.ReadInt32();
				this.Target1 = iReader.ReadInt32();
				this.Target2 = iReader.ReadInt32();
				this.Target3 = iReader.ReadInt32();
				this.Arg = iReader.ReadInt32();
				this.Bool0 = iReader.ReadBoolean();
				this.Bool1 = iReader.ReadBoolean();
				this.Bool2 = iReader.ReadBoolean();
				this.Template = iReader.ReadInt32();
				this.Time = iReader.ReadSingle();
				return;
			case TriggerActionType.ThunderBolt:
				this.Handle = iReader.ReadUInt16();
				this.Id = iReader.ReadInt32();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				return;
			case TriggerActionType.LightningBolt:
			{
				HalfSingle halfSingle5 = default(HalfSingle);
				this.Handle = iReader.ReadUInt16();
				this.Id = iReader.ReadInt32();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				this.Spell.Element = (Elements)iReader.ReadUInt16();
				for (int i = 0; i < (int)this.Spell.Element; i++)
				{
					Elements elements = Defines.ElementFromIndex(i);
					if ((elements & this.Spell.Element) == elements)
					{
						halfSingle5.PackedValue = iReader.ReadUInt16();
						this.Spell[elements] = halfSingle5.ToSingle();
					}
				}
				halfSingle5.PackedValue = iReader.ReadUInt16();
				this.Splash = halfSingle5.ToSingle();
				return;
			}
			case TriggerActionType.SpawnGrease:
			{
				this.Handle = iReader.ReadUInt16();
				this.Arg = iReader.ReadInt32();
				this.Id = iReader.ReadInt32();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				Normalized101010 normalized7 = default(Normalized101010);
				normalized7.PackedValue = iReader.ReadUInt32();
				this.Direction = normalized7.ToVector3();
				return;
			}
			case TriggerActionType.Nullify:
				this.Bool0 = iReader.ReadBoolean();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				return;
			case TriggerActionType.SpawnTornado:
			{
				this.Handle = iReader.ReadUInt16();
				this.Id = iReader.ReadInt32();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				Normalized101010 normalized8 = default(Normalized101010);
				normalized8.PackedValue = iReader.ReadUInt32();
				HalfSingle halfSingle6 = default(HalfSingle);
				halfSingle6.PackedValue = iReader.ReadUInt16();
				this.Orientation = new Quaternion(normalized8.ToVector3(), halfSingle6.ToSingle());
				return;
			}
			case TriggerActionType.NapalmStrike:
			{
				this.Handle = iReader.ReadUInt16();
				this.Arg = iReader.ReadInt32();
				this.Id = iReader.ReadInt32();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				Normalized101010 normalized9 = default(Normalized101010);
				normalized9.PackedValue = iReader.ReadUInt32();
				this.Direction = normalized9.ToVector3();
				this.TimeStamp = iReader.ReadDouble();
				return;
			}
			case TriggerActionType.EarthQuake:
				this.Handle = iReader.ReadUInt16();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				this.Time = iReader.ReadSingle();
				return;
			case TriggerActionType.Charm:
				this.Handle = iReader.ReadUInt16();
				this.Id = (int)iReader.ReadUInt16();
				this.Time = iReader.ReadSingle();
				this.Arg = iReader.ReadInt32();
				return;
			case TriggerActionType.Starfall:
				this.Handle = iReader.ReadUInt16();
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
				this.Time = iReader.ReadSingle();
				return;
			case TriggerActionType.OtherworldlyDischarge:
			case TriggerActionType.StarGaze:
				this.Handle = iReader.ReadUInt16();
				this.Arg = (int)iReader.ReadUInt16();
				return;
			case TriggerActionType.OtherworldlyBoltDestroyed:
				this.Handle = iReader.ReadUInt16();
				this.Arg = (int)iReader.ReadUInt16();
				this.Bool0 = iReader.ReadBoolean();
				this.Bool1 = iReader.ReadBoolean();
				return;
			case TriggerActionType.Confuse:
				this.Handle = iReader.ReadUInt16();
				this.Bool0 = iReader.ReadBoolean();
				return;
			default:
				throw new NotImplementedException();
			}
		}

		// Token: 0x040022B7 RID: 8887
		public TriggerActionType ActionType;

		// Token: 0x040022B8 RID: 8888
		public ushort Handle;

		// Token: 0x040022B9 RID: 8889
		public int Template;

		// Token: 0x040022BA RID: 8890
		public int Scene;

		// Token: 0x040022BB RID: 8891
		public int Arg;

		// Token: 0x040022BC RID: 8892
		public int Id;

		// Token: 0x040022BD RID: 8893
		public Vector3 Position;

		// Token: 0x040022BE RID: 8894
		public Vector3 Direction;

		// Token: 0x040022BF RID: 8895
		public Quaternion Orientation;

		// Token: 0x040022C0 RID: 8896
		public int Point0;

		// Token: 0x040022C1 RID: 8897
		public int Point1;

		// Token: 0x040022C2 RID: 8898
		public int Point2;

		// Token: 0x040022C3 RID: 8899
		public int Point3;

		// Token: 0x040022C4 RID: 8900
		public int Target0;

		// Token: 0x040022C5 RID: 8901
		public int Target1;

		// Token: 0x040022C6 RID: 8902
		public int Target2;

		// Token: 0x040022C7 RID: 8903
		public int Target3;

		// Token: 0x040022C8 RID: 8904
		public bool Bool0;

		// Token: 0x040022C9 RID: 8905
		public bool Bool1;

		// Token: 0x040022CA RID: 8906
		public bool Bool2;

		// Token: 0x040022CB RID: 8907
		public float Time;

		// Token: 0x040022CC RID: 8908
		public Vector3 Color;

		// Token: 0x040022CD RID: 8909
		public double TimeStamp;

		// Token: 0x040022CE RID: 8910
		public Spell Spell;

		// Token: 0x040022CF RID: 8911
		public float Splash;
	}
}
