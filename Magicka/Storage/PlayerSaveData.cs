using System;
using System.IO;

namespace Magicka.Storage
{
	// Token: 0x02000204 RID: 516
	public class PlayerSaveData
	{
		// Token: 0x060010EA RID: 4330 RVA: 0x000699C4 File Offset: 0x00067BC4
		public PlayerSaveData()
		{
			this.Staff = "";
			this.Weapon = "";
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x000699E2 File Offset: 0x00067BE2
		public void Write(BinaryWriter iWriter)
		{
			if (this.Staff == null)
			{
				this.Staff = "";
			}
			if (this.Weapon == null)
			{
				this.Weapon = "";
			}
			iWriter.Write(this.Staff);
			iWriter.Write(this.Weapon);
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x00069A24 File Offset: 0x00067C24
		public static PlayerSaveData Read(BinaryReader iReader)
		{
			return new PlayerSaveData
			{
				Staff = iReader.ReadString(),
				Weapon = iReader.ReadString()
			};
		}

		// Token: 0x04000F72 RID: 3954
		public string Staff;

		// Token: 0x04000F73 RID: 3955
		public string Weapon;
	}
}
