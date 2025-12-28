using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.Levels
{
	// Token: 0x0200032D RID: 813
	public struct Locator
	{
		// Token: 0x060018D2 RID: 6354 RVA: 0x000A3D6D File Offset: 0x000A1F6D
		public Locator(string iName, ContentReader iInput)
		{
			this.Name = iName;
			this.Transform = iInput.ReadMatrix();
			this.Radius = iInput.ReadSingle();
		}

		// Token: 0x060018D3 RID: 6355 RVA: 0x000A3D8E File Offset: 0x000A1F8E
		public override string ToString()
		{
			return this.Name;
		}

		// Token: 0x04001AA9 RID: 6825
		public static readonly string DISTANCE_VAR_NAME = "Distance";

		// Token: 0x04001AAA RID: 6826
		public string Name;

		// Token: 0x04001AAB RID: 6827
		public Matrix Transform;

		// Token: 0x04001AAC RID: 6828
		public float Radius;
	}
}
