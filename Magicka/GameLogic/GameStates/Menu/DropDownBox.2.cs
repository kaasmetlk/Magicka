using System;
using Magicka.Localization;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x02000519 RID: 1305
	internal class DropDownBox<T> : DropDownBox
	{
		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06002750 RID: 10064 RVA: 0x0011E942 File Offset: 0x0011CB42
		// (remove) Token: 0x06002751 RID: 10065 RVA: 0x0011E95B File Offset: 0x0011CB5B
		public event Action<DropDownBox, T> ValueChanged;

		// Token: 0x06002752 RID: 10066 RVA: 0x0011E974 File Offset: 0x0011CB74
		public DropDownBox(BitmapFont iFont, T[] iValues, int?[] iNames, int iWidth) : base(iFont, new string[iValues.Length], iWidth)
		{
			if (iNames != null && iNames.Length != iValues.Length)
			{
				throw new ArgumentException("iNames must contain the same number of elements os iValues! If no localization is to be used, pass null instead.", "iNames");
			}
			this.mValues = iValues;
			this.mNameIDs = iNames;
			this.LanguageChanged();
		}

		// Token: 0x06002753 RID: 10067 RVA: 0x0011E9C4 File Offset: 0x0011CBC4
		public override void LanguageChanged()
		{
			if (this.mNameIDs != null)
			{
				LanguageManager instance = LanguageManager.Instance;
				for (int i = 0; i < this.mNames.Length; i++)
				{
					if (this.mNameIDs[i] != null)
					{
						this.mNames[i] = instance.GetString(this.mNameIDs[i].Value);
					}
					else
					{
						this.mNames[i] = this.mValues[i].ToString();
					}
				}
			}
			else
			{
				for (int j = 0; j < this.mNames.Length; j++)
				{
					this.mNames[j] = this.mValues[j].ToString();
				}
			}
			base.LanguageChanged();
		}

		// Token: 0x06002754 RID: 10068 RVA: 0x0011EA83 File Offset: 0x0011CC83
		protected override void OnSelectedIndexChanged()
		{
			base.OnSelectedIndexChanged();
			if (this.ValueChanged != null)
			{
				this.ValueChanged.Invoke(this, this.mValues[this.mSelectedIndex]);
			}
		}

		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x06002755 RID: 10069 RVA: 0x0011EAB0 File Offset: 0x0011CCB0
		public T SelectedValue
		{
			get
			{
				return this.mValues[this.mSelectedIndex];
			}
		}

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x06002756 RID: 10070 RVA: 0x0011EAC3 File Offset: 0x0011CCC3
		public T[] Values
		{
			get
			{
				return this.mValues;
			}
		}

		// Token: 0x06002757 RID: 10071 RVA: 0x0011EACC File Offset: 0x0011CCCC
		public void SetNewValue(T iNewValue)
		{
			for (int i = 0; i < this.mValues.Length; i++)
			{
				if (this.mValues[i].Equals(iNewValue))
				{
					base.SelectedIndex = i;
					return;
				}
			}
		}

		// Token: 0x04002A8A RID: 10890
		private int?[] mNameIDs;

		// Token: 0x04002A8B RID: 10891
		private T[] mValues;
	}
}
