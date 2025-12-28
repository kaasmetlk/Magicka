using System;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x02000017 RID: 23
	public abstract class Condition
	{
		// Token: 0x060000AE RID: 174 RVA: 0x00006983 File Offset: 0x00004B83
		protected Condition(GameScene iScene)
		{
			this.mScene = iScene;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00006992 File Offset: 0x00004B92
		public virtual void Initialize()
		{
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00006994 File Offset: 0x00004B94
		protected static Type GetType(string name)
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].BaseType == typeof(Condition) && types[i].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return types[i];
				}
			}
			return null;
		}

		// Token: 0x060000B1 RID: 177
		protected abstract bool InternalMet(Character iSender);

		// Token: 0x060000B2 RID: 178 RVA: 0x000069E4 File Offset: 0x00004BE4
		public virtual bool IsMet(Character iSender)
		{
			return this.mInvert ^ this.InternalMet(iSender);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000069F4 File Offset: 0x00004BF4
		public static Condition Read(GameScene iParent, XmlNode iNode)
		{
			Type type = Condition.GetType(iNode.Name);
			ConstructorInfo constructor = type.GetConstructor(new Type[]
			{
				typeof(GameScene)
			});
			Condition condition = (Condition)constructor.Invoke(new object[]
			{
				iParent
			});
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
				if (propertyInfo != null)
				{
					if (propertyInfo.PropertyType.IsEnum)
					{
						propertyInfo.SetValue(condition, Enum.Parse(propertyInfo.PropertyType, xmlAttribute.Value, true), null);
					}
					else if (propertyInfo.PropertyType == typeof(bool))
					{
						propertyInfo.SetValue(condition, bool.Parse(xmlAttribute.Value), null);
					}
					else if (propertyInfo.PropertyType == typeof(int))
					{
						propertyInfo.SetValue(condition, int.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture), null);
					}
					else if (propertyInfo.PropertyType == typeof(float))
					{
						propertyInfo.SetValue(condition, float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture), null);
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
						propertyInfo.SetValue(condition, xmlAttribute.Value.ToLowerInvariant(), null);
					}
				}
			}
			return condition;
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x00006C1D File Offset: 0x00004E1D
		public GameScene Scene
		{
			get
			{
				return this.mScene;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00006C25 File Offset: 0x00004E25
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x00006C2D File Offset: 0x00004E2D
		public bool Invert
		{
			get
			{
				return this.mInvert;
			}
			set
			{
				this.mInvert = value;
			}
		}

		// Token: 0x0400008E RID: 142
		public static readonly int ANYID = "any".GetHashCodeCustom();

		// Token: 0x0400008F RID: 143
		private bool mInvert;

		// Token: 0x04000090 RID: 144
		private GameScene mScene;
	}
}
