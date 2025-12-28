using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Magicka.Levels;

namespace Magicka.AI
{
	// Token: 0x02000196 RID: 406
	public struct KillEvent
	{
		// Token: 0x06000C29 RID: 3113 RVA: 0x0004953C File Offset: 0x0004773C
		public KillEvent(LevelModel iLevel, XmlNode iNode)
		{
			this = default(KillEvent);
			this.Remove = true;
			foreach (object obj in iNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (xmlAttribute.Name.Equals("remove", StringComparison.OrdinalIgnoreCase))
				{
					this.Remove = bool.Parse(xmlAttribute.Value);
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

		// Token: 0x06000C2A RID: 3114 RVA: 0x0004963C File Offset: 0x0004783C
		public KillEvent(BinaryReader iReader)
		{
			this.Trigger = iReader.ReadInt32();
			this.Delay = (float)iReader.ReadInt32();
			this.Remove = iReader.ReadBoolean();
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x00049663 File Offset: 0x00047863
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Trigger);
			iWriter.Write(this.Delay);
			iWriter.Write(this.Remove);
		}

		// Token: 0x04000B5A RID: 2906
		public int Trigger;

		// Token: 0x04000B5B RID: 2907
		public float Delay;

		// Token: 0x04000B5C RID: 2908
		public bool Remove;
	}
}
