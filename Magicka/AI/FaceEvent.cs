using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Magicka.Levels;
using Microsoft.Xna.Framework;

namespace Magicka.AI
{
	// Token: 0x02000194 RID: 404
	public struct FaceEvent
	{
		// Token: 0x06000C23 RID: 3107 RVA: 0x00049068 File Offset: 0x00047268
		public FaceEvent(LevelModel iLevel, XmlNode iNode)
		{
			this = default(FaceEvent);
			this.Speed = 1f;
			foreach (object obj in iNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (xmlAttribute.Name.Equals("facingDirection", StringComparison.OrdinalIgnoreCase) || xmlAttribute.Name.Equals("target", StringComparison.OrdinalIgnoreCase))
				{
					string[] array = xmlAttribute.Value.Split(new char[]
					{
						','
					});
					if (array.Length == 3)
					{
						this.Direction.X = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
						this.Direction.Y = float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
						this.Direction.Z = float.Parse(array[2], CultureInfo.InvariantCulture.NumberFormat);
						this.Direction.Normalize();
					}
					else
					{
						this.TargetID = xmlAttribute.Value.ToLowerInvariant().GetHashCodeCustom();
					}
				}
				else if (xmlAttribute.Name.Equals("delay", StringComparison.OrdinalIgnoreCase))
				{
					this.Delay = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture.NumberFormat);
				}
				else if (xmlAttribute.Name.Equals("speed", StringComparison.OrdinalIgnoreCase))
				{
					this.Speed = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture.NumberFormat);
				}
				else
				{
					if (!xmlAttribute.Name.Equals("trigger", StringComparison.OrdinalIgnoreCase))
					{
						throw new Exception(string.Format("Invalid attribute \"{0}\" in \"Event\"!", xmlAttribute.Name));
					}
					this.Trigger = xmlAttribute.Value.ToLowerInvariant().GetHashCodeCustom();
				}
			}
		}

		// Token: 0x06000C24 RID: 3108 RVA: 0x0004924C File Offset: 0x0004744C
		public FaceEvent(BinaryReader iReader)
		{
			this.Trigger = iReader.ReadInt32();
			this.Delay = (float)iReader.ReadInt32();
			this.Speed = iReader.ReadSingle();
			this.TargetID = iReader.ReadInt32();
			this.Direction.X = iReader.ReadSingle();
			this.Direction.Y = iReader.ReadSingle();
			this.Direction.Z = iReader.ReadSingle();
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x000492C0 File Offset: 0x000474C0
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Trigger);
			iWriter.Write(this.Delay);
			iWriter.Write(this.Speed);
			iWriter.Write(this.TargetID);
			iWriter.Write(this.Direction.X);
			iWriter.Write(this.Direction.Y);
			iWriter.Write(this.Direction.Z);
		}

		// Token: 0x04000B50 RID: 2896
		public int Trigger;

		// Token: 0x04000B51 RID: 2897
		public float Delay;

		// Token: 0x04000B52 RID: 2898
		public float Speed;

		// Token: 0x04000B53 RID: 2899
		public int TargetID;

		// Token: 0x04000B54 RID: 2900
		public Vector3 Direction;
	}
}
