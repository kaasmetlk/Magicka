using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Magicka.Network
{
	// Token: 0x02000225 RID: 549
	public struct EntityUpdateMessage : ISendable
	{
		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x0600113C RID: 4412 RVA: 0x0006ABD3 File Offset: 0x00068DD3
		public PacketType PacketType
		{
			get
			{
				return PacketType.EntityUpdate;
			}
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x0006ABD8 File Offset: 0x00068DD8
		public unsafe void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Handle);
			iWriter.Write(this.UDPStamp);
			iWriter.Write((ushort)this.Features);
			if ((ushort)(this.Features & EntityFeatures.Position) != 0)
			{
				iWriter.Write(this.Position.X);
				iWriter.Write(this.Position.Y);
				iWriter.Write(this.Position.Z);
			}
			if ((ushort)(this.Features & EntityFeatures.Direction) != 0)
			{
				float num = MathHelper.WrapAngle(this.Direction);
				num /= 6.2831855f;
				num += 0.5f;
				num *= 65535f;
				iWriter.Write((ushort)num);
			}
			if ((ushort)(this.Features & EntityFeatures.Velocity) != 0)
			{
				iWriter.Write(new HalfSingle(this.Velocity.X).PackedValue);
				iWriter.Write(new HalfSingle(this.Velocity.Y).PackedValue);
				iWriter.Write(new HalfSingle(this.Velocity.Z).PackedValue);
			}
			if ((ushort)(this.Features & EntityFeatures.Orientation) != 0)
			{
				iWriter.Write(new HalfSingle(this.Orientation.X).PackedValue);
				iWriter.Write(new HalfSingle(this.Orientation.Y).PackedValue);
				iWriter.Write(new HalfSingle(this.Orientation.Z).PackedValue);
				iWriter.Write(new HalfSingle(this.Orientation.W).PackedValue);
			}
			if ((ushort)(this.Features & EntityFeatures.Character) != 0)
			{
				throw new NotImplementedException();
			}
			if ((ushort)(this.Features & EntityFeatures.Damageable) != 0)
			{
				iWriter.Write(this.HitPoints);
			}
			if ((ushort)(this.Features & EntityFeatures.StatusEffected) != 0)
			{
				iWriter.Write((short)this.StatusEffects);
				fixed (float* ptr = &this.StatusEffectMagnitude.FixedElementField)
				{
					fixed (float* ptr2 = &this.StatusEffectDPS.FixedElementField)
					{
						for (int i = 0; i < 9; i++)
						{
							StatusEffects statusEffects = StatusEffect.StatusFromIndex(i);
							if ((this.StatusEffects & statusEffects) == statusEffects)
							{
								iWriter.Write(new HalfSingle(ptr[i]).PackedValue);
								iWriter.Write(new HalfSingle(ptr2[i]).PackedValue);
							}
						}
					}
				}
			}
			if ((ushort)(this.Features & EntityFeatures.GenericBool) != 0)
			{
				iWriter.Write(this.GenericBool);
			}
			if ((ushort)(this.Features & EntityFeatures.GenericInt) != 0)
			{
				iWriter.Write(this.GenericInt);
			}
			if ((ushort)(this.Features & EntityFeatures.GenericFloat) != 0)
			{
				iWriter.Write(new HalfSingle(this.GenericFloat).PackedValue);
			}
			if ((ushort)(this.Features & EntityFeatures.WanderAngle) != 0)
			{
				float num2 = MathHelper.WrapAngle(this.WanderAngle);
				num2 /= 6.2831855f;
				num2 += 0.5f;
				num2 *= 256f;
				iWriter.Write((byte)num2);
			}
			if ((ushort)(this.Features & EntityFeatures.SelfShield) != 0)
			{
				iWriter.Write((byte)this.SelfShieldType);
				iWriter.Write(new HalfSingle(this.SelfShieldHealth).PackedValue);
			}
			if ((ushort)(this.Features & EntityFeatures.Etherealized) != 0)
			{
				iWriter.Write(this.EtherealState);
			}
			if ((ushort)(this.Features & EntityFeatures.GenericUShort) != 0)
			{
				iWriter.Write(this.GenericUShort);
			}
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x0006AF38 File Offset: 0x00069138
		public unsafe void Read(BinaryReader iReader)
		{
			HalfSingle halfSingle = default(HalfSingle);
			this.Handle = iReader.ReadUInt16();
			this.UDPStamp = iReader.ReadUInt16();
			this.Features = (EntityFeatures)iReader.ReadUInt16();
			if ((ushort)(this.Features & EntityFeatures.Position) != 0)
			{
				this.Position.X = iReader.ReadSingle();
				this.Position.Y = iReader.ReadSingle();
				this.Position.Z = iReader.ReadSingle();
			}
			if ((ushort)(this.Features & EntityFeatures.Direction) != 0)
			{
				float num = (float)iReader.ReadUInt16();
				num /= 65535f;
				num -= 0.5f;
				num *= 6.2831855f;
				this.Direction = num;
			}
			if ((ushort)(this.Features & EntityFeatures.Velocity) != 0)
			{
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Velocity.X = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Velocity.Y = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Velocity.Z = halfSingle.ToSingle();
			}
			if ((ushort)(this.Features & EntityFeatures.Orientation) != 0)
			{
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Orientation.X = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Orientation.Y = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Orientation.Z = halfSingle.ToSingle();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.Orientation.W = halfSingle.ToSingle();
			}
			if ((ushort)(this.Features & EntityFeatures.Character) != 0)
			{
				throw new NotImplementedException();
			}
			if ((ushort)(this.Features & EntityFeatures.Damageable) != 0)
			{
				this.HitPoints = iReader.ReadSingle();
			}
			if ((ushort)(this.Features & EntityFeatures.StatusEffected) != 0)
			{
				this.StatusEffects = (StatusEffects)iReader.ReadInt16();
				fixed (float* ptr = &this.StatusEffectMagnitude.FixedElementField)
				{
					fixed (float* ptr2 = &this.StatusEffectDPS.FixedElementField)
					{
						for (int i = 0; i < 9; i++)
						{
							StatusEffects statusEffects = StatusEffect.StatusFromIndex(i);
							if ((this.StatusEffects & statusEffects) == statusEffects)
							{
								halfSingle.PackedValue = iReader.ReadUInt16();
								ptr[i] = halfSingle.ToSingle();
								halfSingle.PackedValue = iReader.ReadUInt16();
								ptr2[i] = halfSingle.ToSingle();
							}
							else
							{
								ptr[i] = 0f;
								ptr2[i] = 0f;
							}
						}
					}
				}
			}
			if ((ushort)(this.Features & EntityFeatures.GenericBool) != 0)
			{
				this.GenericBool = iReader.ReadBoolean();
			}
			if ((ushort)(this.Features & EntityFeatures.GenericInt) != 0)
			{
				this.GenericInt = iReader.ReadInt32();
			}
			if ((ushort)(this.Features & EntityFeatures.GenericFloat) != 0)
			{
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.GenericFloat = halfSingle.ToSingle();
			}
			if ((ushort)(this.Features & EntityFeatures.WanderAngle) != 0)
			{
				float num2 = (float)iReader.ReadByte();
				num2 /= 256f;
				num2 -= 0.5f;
				num2 *= 6.2831855f;
				this.WanderAngle = num2;
			}
			if ((ushort)(this.Features & EntityFeatures.SelfShield) != 0)
			{
				this.SelfShieldType = (Character.SelfShieldType)iReader.ReadByte();
				halfSingle.PackedValue = iReader.ReadUInt16();
				this.SelfShieldHealth = halfSingle.ToSingle();
			}
			if ((ushort)(this.Features & EntityFeatures.Etherealized) != 0)
			{
				this.EtherealState = iReader.ReadBoolean();
			}
			if ((ushort)(this.Features & EntityFeatures.GenericUShort) != 0)
			{
				this.GenericUShort = iReader.ReadUInt16();
			}
		}

		// Token: 0x04001012 RID: 4114
		public ushort UDPStamp;

		// Token: 0x04001013 RID: 4115
		public ushort Handle;

		// Token: 0x04001014 RID: 4116
		public EntityFeatures Features;

		// Token: 0x04001015 RID: 4117
		public Vector3 Position;

		// Token: 0x04001016 RID: 4118
		public float Direction;

		// Token: 0x04001017 RID: 4119
		public Vector3 Velocity;

		// Token: 0x04001018 RID: 4120
		public Quaternion Orientation;

		// Token: 0x04001019 RID: 4121
		public float HitPoints;

		// Token: 0x0400101A RID: 4122
		public bool GenericBool;

		// Token: 0x0400101B RID: 4123
		public int GenericInt;

		// Token: 0x0400101C RID: 4124
		public ushort GenericUShort;

		// Token: 0x0400101D RID: 4125
		public float GenericFloat;

		// Token: 0x0400101E RID: 4126
		public float WanderAngle;

		// Token: 0x0400101F RID: 4127
		public bool EtherealState;

		// Token: 0x04001020 RID: 4128
		public Character.SelfShieldType SelfShieldType;

		// Token: 0x04001021 RID: 4129
		public float SelfShieldHealth;

		// Token: 0x04001022 RID: 4130
		public StatusEffects StatusEffects;

		// Token: 0x04001023 RID: 4131
		[FixedBuffer(typeof(float), 9)]
		public EntityUpdateMessage.<StatusEffectMagnitude>e__FixedBuffer3 StatusEffectMagnitude;

		// Token: 0x04001024 RID: 4132
		[FixedBuffer(typeof(float), 9)]
		public EntityUpdateMessage.<StatusEffectDPS>e__FixedBuffer4 StatusEffectDPS;

		// Token: 0x02000226 RID: 550
		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 36)]
		public struct <StatusEffectMagnitude>e__FixedBuffer3
		{
			// Token: 0x04001025 RID: 4133
			public float FixedElementField;
		}

		// Token: 0x02000227 RID: 551
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 36)]
		public struct <StatusEffectDPS>e__FixedBuffer4
		{
			// Token: 0x04001026 RID: 4134
			public float FixedElementField;
		}
	}
}
