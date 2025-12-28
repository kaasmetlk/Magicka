using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Magicka.Levels;

namespace Magicka.AI
{
	// Token: 0x02000195 RID: 405
	public struct AnimationEvent
	{
		// Token: 0x06000C26 RID: 3110 RVA: 0x00049330 File Offset: 0x00047530
		public AnimationEvent(LevelModel iLevel, XmlNode iNode)
		{
			this = default(AnimationEvent);
			foreach (object obj in iNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (xmlAttribute.Name.Equals("animation", StringComparison.OrdinalIgnoreCase))
				{
					this.Animation = (Animations)Enum.Parse(typeof(Animations), xmlAttribute.Value, true);
				}
				else if (xmlAttribute.Name.Equals("idleanimation", StringComparison.OrdinalIgnoreCase))
				{
					this.IdleAnimation = (Animations)Enum.Parse(typeof(Animations), xmlAttribute.Value, true);
				}
				else if (xmlAttribute.Name.Equals("delay", StringComparison.OrdinalIgnoreCase))
				{
					this.Delay = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture.NumberFormat);
				}
				else if (xmlAttribute.Name.Equals("blendTime", StringComparison.OrdinalIgnoreCase))
				{
					this.BlendTime = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture.NumberFormat);
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

		// Token: 0x06000C27 RID: 3111 RVA: 0x000494B0 File Offset: 0x000476B0
		public AnimationEvent(BinaryReader iReader)
		{
			this.Trigger = iReader.ReadInt32();
			this.Delay = (float)iReader.ReadInt32();
			this.Animation = (Animations)iReader.ReadInt32();
			this.BlendTime = iReader.ReadSingle();
			this.IdleAnimation = (Animations)iReader.ReadInt32();
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x000494FC File Offset: 0x000476FC
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Trigger);
			iWriter.Write(this.Delay);
			iWriter.Write((int)this.Animation);
			iWriter.Write(this.BlendTime);
			iWriter.Write((int)this.IdleAnimation);
		}

		// Token: 0x04000B55 RID: 2901
		public int Trigger;

		// Token: 0x04000B56 RID: 2902
		public float Delay;

		// Token: 0x04000B57 RID: 2903
		public Animations Animation;

		// Token: 0x04000B58 RID: 2904
		public float BlendTime;

		// Token: 0x04000B59 RID: 2905
		public Animations IdleAnimation;
	}
}
