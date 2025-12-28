using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000013 RID: 19
	public abstract class Action
	{
		// Token: 0x06000070 RID: 112 RVA: 0x00005C40 File Offset: 0x00003E40
		protected Action(Trigger iTrigger, GameScene iScene)
		{
			this.mHandle = (ushort)Action.sInstances.Count;
			if (Action.sInstances.Count >= 65535)
			{
				throw new Exception("To many actions! Max allowed per level is " + ushort.MaxValue);
			}
			Action.sInstances.Add(this);
			this.mTrigger = iTrigger;
			this.mScene = iScene;
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00005CA8 File Offset: 0x00003EA8
		public ushort Handle
		{
			get
			{
				return this.mHandle;
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00005CB0 File Offset: 0x00003EB0
		public static void ClearInstances()
		{
			Action.sInstances.Clear();
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00005CBC File Offset: 0x00003EBC
		public static Action GetByHandle(ushort iHandel)
		{
			if ((int)iHandel < Action.sInstances.Count)
			{
				return Action.sInstances[(int)iHandel];
			}
			return null;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00005CD8 File Offset: 0x00003ED8
		public virtual void Initialize()
		{
		}

		// Token: 0x06000075 RID: 117
		protected abstract void Execute();

		// Token: 0x06000076 RID: 118
		public abstract void QuickExecute();

		// Token: 0x06000077 RID: 119 RVA: 0x00005CDC File Offset: 0x00003EDC
		protected static Type GetType(string name)
		{
			Type[] array = null;
			try
			{
				Module module = Assembly.GetExecutingAssembly().GetModules()[0];
				array = module.GetTypes();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].BaseType == typeof(Action) && array[i].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return array[i];
				}
			}
			return null;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00005D4C File Offset: 0x00003F4C
		public virtual void OnTrigger(Character iArg)
		{
			if (this.mQueue <= 0)
			{
				this.mQueue = 1;
				this.mDelayCountdown = this.mDelay;
				return;
			}
			this.mQueue++;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00005D79 File Offset: 0x00003F79
		public bool HasFinishedExecuting()
		{
			return this.mQueue == 0;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005D84 File Offset: 0x00003F84
		public virtual void Update(float iDeltaTime)
		{
			this.mDelayCountdown -= iDeltaTime;
			while (this.mDelayCountdown <= 0f && this.mQueue > 0)
			{
				this.mQueue--;
				this.mDelayCountdown += this.mDelay;
				this.Execute();
				this.GameScene.ActionExecute(this);
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00005DEA File Offset: 0x00003FEA
		public void ClearDelayed()
		{
			this.mQueue = 0;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00005DF4 File Offset: 0x00003FF4
		public static Action Read(GameScene iScene, Trigger iTrigger, XmlNode iNode)
		{
			Type type = Action.GetType(iNode.Name);
			ConstructorInfo constructor = type.GetConstructor(new Type[]
			{
				typeof(Trigger),
				typeof(GameScene),
				typeof(XmlNode)
			});
			Action action;
			if (constructor == null)
			{
				constructor = type.GetConstructor(new Type[]
				{
					typeof(Trigger),
					typeof(GameScene)
				});
				action = (Action)constructor.Invoke(new object[]
				{
					iTrigger,
					iScene
				});
			}
			else
			{
				action = (Action)constructor.Invoke(new object[]
				{
					iTrigger,
					iScene,
					iNode
				});
			}
			PropertyInfo[] properties = type.GetProperties();
			for (int i = 0; i < iNode.Attributes.Count; i++)
			{
				XmlAttribute xmlAttribute = iNode.Attributes[i];
				PropertyInfo propertyInfo = null;
				for (int j = 0; j < properties.Length; j++)
				{
					if (properties[j].CanWrite && properties[j].Name.Equals(xmlAttribute.Name, StringComparison.OrdinalIgnoreCase))
					{
						propertyInfo = properties[j];
						break;
					}
				}
				if (propertyInfo != null && propertyInfo != null)
				{
					if (propertyInfo.PropertyType.IsEnum)
					{
						propertyInfo.SetValue(action, Enum.Parse(propertyInfo.PropertyType, xmlAttribute.Value, true), null);
					}
					else if (propertyInfo.PropertyType == typeof(bool))
					{
						propertyInfo.SetValue(action, bool.Parse(xmlAttribute.Value), null);
					}
					else if (propertyInfo.PropertyType == typeof(int))
					{
						propertyInfo.SetValue(action, int.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture), null);
					}
					else if (propertyInfo.PropertyType == typeof(float))
					{
						propertyInfo.SetValue(action, float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture), null);
					}
					else if (propertyInfo.PropertyType == typeof(Vector2))
					{
						string[] array = xmlAttribute.Value.Split(new char[]
						{
							','
						});
						propertyInfo.SetValue(action, new Vector2
						{
							X = float.Parse(array[0], CultureInfo.InvariantCulture),
							Y = float.Parse(array[1], CultureInfo.InvariantCulture)
						}, null);
					}
					else if (propertyInfo.PropertyType == typeof(Vector3))
					{
						string[] array2 = xmlAttribute.Value.Split(new char[]
						{
							','
						});
						propertyInfo.SetValue(action, new Vector3
						{
							X = float.Parse(array2[0], CultureInfo.InvariantCulture),
							Y = float.Parse(array2[1], CultureInfo.InvariantCulture),
							Z = float.Parse(array2[2], CultureInfo.InvariantCulture)
						}, null);
					}
					else if (propertyInfo.PropertyType == typeof(Vector4))
					{
						string[] array3 = xmlAttribute.Value.Split(new char[]
						{
							','
						});
						propertyInfo.SetValue(action, new Vector4
						{
							X = float.Parse(array3[0], CultureInfo.InvariantCulture),
							Y = float.Parse(array3[1], CultureInfo.InvariantCulture),
							Z = float.Parse(array3[2], CultureInfo.InvariantCulture),
							W = float.Parse(array3[3], CultureInfo.InvariantCulture)
						}, null);
					}
					else if (propertyInfo.PropertyType == typeof(Color))
					{
						string[] array4 = xmlAttribute.Value.Split(new char[]
						{
							','
						});
						Color color = new Color(new Vector4
						{
							X = float.Parse(array4[0], CultureInfo.InvariantCulture),
							Y = float.Parse(array4[1], CultureInfo.InvariantCulture),
							Z = float.Parse(array4[2], CultureInfo.InvariantCulture),
							W = float.Parse(array4[3], CultureInfo.InvariantCulture)
						});
						propertyInfo.SetValue(action, color, null);
					}
					else
					{
						if (propertyInfo.PropertyType != typeof(string))
						{
							throw new NotSupportedException(string.Concat(new string[]
							{
								"Invalid type \"",
								propertyInfo.PropertyType.Name,
								"\" trying to parse attribute \"",
								xmlAttribute.Name,
								"\" in XML node \"",
								iNode.Name,
								"\"!"
							}));
						}
						propertyInfo.SetValue(action, xmlAttribute.Value.ToLowerInvariant(), null);
					}
				}
			}
			return action;
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600007D RID: 125 RVA: 0x000062F4 File Offset: 0x000044F4
		// (set) Token: 0x0600007E RID: 126 RVA: 0x000062FC File Offset: 0x000044FC
		public float Delay
		{
			get
			{
				return this.mDelay;
			}
			set
			{
				this.mDelay = value;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00006305 File Offset: 0x00004505
		public GameScene GameScene
		{
			get
			{
				return this.mScene;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000080 RID: 128 RVA: 0x0000630D File Offset: 0x0000450D
		public Trigger Trigger
		{
			get
			{
				return this.mTrigger;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00006315 File Offset: 0x00004515
		// (set) Token: 0x06000082 RID: 130 RVA: 0x0000631D File Offset: 0x0000451D
		protected virtual object Tag { get; set; }

		// Token: 0x06000083 RID: 131 RVA: 0x00006326 File Offset: 0x00004526
		protected virtual void WriteTag(BinaryWriter iWriter, object mTag)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000632D File Offset: 0x0000452D
		protected virtual object ReadTag(BinaryReader iReader)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0400006F RID: 111
		protected float mDelay;

		// Token: 0x04000070 RID: 112
		protected float mDelayCountdown;

		// Token: 0x04000071 RID: 113
		protected int mQueue;

		// Token: 0x04000072 RID: 114
		protected Trigger mTrigger;

		// Token: 0x04000073 RID: 115
		protected GameScene mScene;

		// Token: 0x04000074 RID: 116
		private static List<Action> sInstances = new List<Action>();

		// Token: 0x04000075 RID: 117
		private readonly ushort mHandle;

		// Token: 0x02000014 RID: 20
		public struct State
		{
			// Token: 0x06000086 RID: 134 RVA: 0x00006340 File Offset: 0x00004540
			public State(Action iAction)
			{
				this.mDelayCountdown = iAction.mDelayCountdown;
				this.mQueue = iAction.mQueue;
				this.mTag = iAction.Tag;
			}

			// Token: 0x06000087 RID: 135 RVA: 0x00006366 File Offset: 0x00004566
			public State(BinaryReader iReader, Action iAction)
			{
				this.mDelayCountdown = iReader.ReadSingle();
				this.mQueue = iReader.ReadInt32();
				this.mTag = null;
				if (iReader.ReadBoolean())
				{
					this.mTag = iAction.ReadTag(iReader);
				}
			}

			// Token: 0x06000088 RID: 136 RVA: 0x0000639C File Offset: 0x0000459C
			public void AssignToAction(Action iAction)
			{
				iAction.mDelayCountdown = this.mDelayCountdown;
				iAction.mQueue = this.mQueue;
				iAction.Tag = this.mTag;
			}

			// Token: 0x06000089 RID: 137 RVA: 0x000063C2 File Offset: 0x000045C2
			public void Reset(Action iAction)
			{
				iAction.mDelayCountdown = 0f;
				iAction.mQueue = 0;
			}

			// Token: 0x0600008A RID: 138 RVA: 0x000063D8 File Offset: 0x000045D8
			internal void Write(BinaryWriter iWriter, Action iAction)
			{
				iWriter.Write(this.mDelayCountdown);
				iWriter.Write(this.mQueue);
				iWriter.Write(this.mTag != null);
				if (this.mTag != null)
				{
					iAction.WriteTag(iWriter, this.mTag);
				}
			}

			// Token: 0x04000077 RID: 119
			private float mDelayCountdown;

			// Token: 0x04000078 RID: 120
			private int mQueue;

			// Token: 0x04000079 RID: 121
			private object mTag;
		}
	}
}
