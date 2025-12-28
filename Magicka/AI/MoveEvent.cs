using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Magicka.Levels;
using Magicka.Levels.Triggers;
using Microsoft.Xna.Framework;

namespace Magicka.AI
{
	// Token: 0x02000193 RID: 403
	public struct MoveEvent
	{
		// Token: 0x06000C20 RID: 3104 RVA: 0x00048C1C File Offset: 0x00046E1C
		public MoveEvent(LevelModel iLevel, XmlNode iNode)
		{
			this = default(MoveEvent);
			this.Speed = 1f;
			foreach (object obj in iNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (xmlAttribute.Name.Equals("position", StringComparison.OrdinalIgnoreCase))
				{
					string[] array = xmlAttribute.Value.Split(new char[]
					{
						','
					});
					if (array.Length == 3)
					{
						this.Waypoint.X = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
						this.Waypoint.Y = float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
						this.Waypoint.Z = float.Parse(array[2], CultureInfo.InvariantCulture.NumberFormat);
					}
					else
					{
						this.WaypointID = xmlAttribute.Value.ToLowerInvariant().GetHashCodeCustom();
						Locator locator;
						TriggerArea triggerArea;
						if (iLevel.Locators.TryGetValue(this.WaypointID, out locator))
						{
							this.Waypoint = locator.Transform.Translation;
							this.Direction = locator.Transform.Forward;
						}
						else if (iLevel.TriggerAreas.TryGetValue(this.WaypointID, out triggerArea))
						{
							this.Waypoint = triggerArea.GetRandomLocation();
						}
					}
				}
				else if (xmlAttribute.Name.Equals("facingDirection", StringComparison.OrdinalIgnoreCase))
				{
					string[] array = xmlAttribute.Value.Split(new char[]
					{
						','
					});
					if (array.Length == 3)
					{
						this.FixedDirection = true;
						this.Direction.X = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
						this.Direction.Y = float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
						this.Direction.Z = float.Parse(array[2], CultureInfo.InvariantCulture.NumberFormat);
						this.Direction.Normalize();
					}
					else
					{
						this.FixedDirection = bool.Parse(xmlAttribute.Value);
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

		// Token: 0x06000C21 RID: 3105 RVA: 0x00048F08 File Offset: 0x00047108
		public MoveEvent(BinaryReader iReader)
		{
			this.Trigger = iReader.ReadInt32();
			this.Delay = (float)iReader.ReadInt32();
			this.Direction.X = iReader.ReadSingle();
			this.Direction.Y = iReader.ReadSingle();
			this.Direction.Z = iReader.ReadSingle();
			this.FixedDirection = iReader.ReadBoolean();
			this.Speed = iReader.ReadSingle();
			this.Waypoint.X = iReader.ReadSingle();
			this.Waypoint.Y = iReader.ReadSingle();
			this.Waypoint.Z = iReader.ReadSingle();
			this.WaypointID = iReader.ReadInt32();
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x00048FB8 File Offset: 0x000471B8
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.Trigger);
			iWriter.Write(this.Delay);
			iWriter.Write(this.Direction.X);
			iWriter.Write(this.Direction.Y);
			iWriter.Write(this.Direction.Z);
			iWriter.Write(this.FixedDirection);
			iWriter.Write(this.Speed);
			iWriter.Write(this.Waypoint.X);
			iWriter.Write(this.Waypoint.Y);
			iWriter.Write(this.Waypoint.Z);
			iWriter.Write(this.WaypointID);
		}

		// Token: 0x04000B49 RID: 2889
		public int Trigger;

		// Token: 0x04000B4A RID: 2890
		public float Delay;

		// Token: 0x04000B4B RID: 2891
		public float Speed;

		// Token: 0x04000B4C RID: 2892
		public Vector3 Waypoint;

		// Token: 0x04000B4D RID: 2893
		public readonly int WaypointID;

		// Token: 0x04000B4E RID: 2894
		public bool FixedDirection;

		// Token: 0x04000B4F RID: 2895
		public Vector3 Direction;
	}
}
