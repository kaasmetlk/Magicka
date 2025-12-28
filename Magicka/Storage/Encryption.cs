using System;

namespace Magicka.Storage
{
	// Token: 0x02000328 RID: 808
	public static class Encryption
	{
		// Token: 0x060018C0 RID: 6336 RVA: 0x000A3520 File Offset: 0x000A1720
		public static string Vigenere(string iInput, string iKeyword, bool iDecode)
		{
			int num = 0;
			string text = string.Empty;
			for (int i = 0; i < iInput.Length; i++)
			{
				if (iDecode)
				{
					text += iInput[i] - iKeyword[num];
				}
				else
				{
					text += iInput[i] + iKeyword[num];
				}
				num++;
				if (num >= iKeyword.Length)
				{
					num = 0;
				}
			}
			return text;
		}
	}
}
