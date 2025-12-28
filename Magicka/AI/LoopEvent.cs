using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Magicka.Levels;

namespace Magicka.AI
{
	// Token: 0x02000197 RID: 407
	public struct LoopEvent
	{
		// Token: 0x06000C2C RID: 3116 RVA: 0x0004968C File Offset: 0x0004788C
		public LoopEvent(LevelModel iLevel, XmlNode iNode)
		{
			this = default(LoopEvent);
			foreach (object obj in iNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (xmlAttribute.Name.Equals("type", StringComparison.OrdinalIgnoreCase))
				{
					this.Type = (LoopType)Enum.Parse(typeof(LoopType), xmlAttribute.Value, true);
				}
				else if (xmlAttribute.Name.Equals("delay", StringComparison.OrdinalIgnoreCase))
				{
					this.Delay = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture.NumberFormat);
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

		// Token: 0x06000C2D RID: 3117 RVA: 0x00049794 File Offset: 0x00047994
		public LoopEvent(BinaryReader iReader)
		{
			this.Trigger = iReader.ReadInt32();
			this.Delay = (float)iReader.ReadInt32();
			this.Type = (LoopType)iReader.ReadByte();
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x000497BB File Offset: 0x000479BB
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Trigger);
			iWriter.Write(this.Delay);
			iWriter.Write((byte)this.Type);
		}

		// Token: 0x04000B5D RID: 2909
		public int Trigger;

		// Token: 0x04000B5E RID: 2910
		public float Delay;

		// Token: 0x04000B5F RID: 2911
		public LoopType Type;
	}
}
