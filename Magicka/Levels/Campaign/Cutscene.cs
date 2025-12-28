using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Magicka.Audio;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Campaign
{
	// Token: 0x020002CD RID: 717
	internal class Cutscene
	{
		// Token: 0x060015F3 RID: 5619 RVA: 0x0008B268 File Offset: 0x00089468
		public Cutscene(XmlNode iNode)
		{
			List<Cutscene.TimeFloat> list = new List<Cutscene.TimeFloat>();
			List<Cutscene.TimeVector2> list2 = new List<Cutscene.TimeVector2>();
			for (int i = 0; i < iNode.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = iNode.ChildNodes[i];
				if (!(xmlNode is XmlComment))
				{
					if (xmlNode.Name.Equals("camera", StringComparison.OrdinalIgnoreCase))
					{
						for (int j = 0; j < xmlNode.ChildNodes.Count; j++)
						{
							XmlNode xmlNode2 = xmlNode.ChildNodes[j];
							if (!(xmlNode2 is XmlComment))
							{
								if (xmlNode2.Name.Equals("zoom", StringComparison.OrdinalIgnoreCase))
								{
									Cutscene.TimeFloat item = default(Cutscene.TimeFloat);
									for (int k = 0; k < xmlNode2.Attributes.Count; k++)
									{
										XmlAttribute xmlAttribute = xmlNode2.Attributes[k];
										if (xmlAttribute.Name.Equals("time", StringComparison.OrdinalIgnoreCase))
										{
											item.Time = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture.NumberFormat);
										}
									}
									item.Value = float.Parse(xmlNode2.InnerText, CultureInfo.InvariantCulture.NumberFormat);
									list.Add(item);
								}
								else if (xmlNode2.Name.Equals("position", StringComparison.OrdinalIgnoreCase))
								{
									Cutscene.TimeVector2 item2 = default(Cutscene.TimeVector2);
									for (int l = 0; l < xmlNode2.Attributes.Count; l++)
									{
										XmlAttribute xmlAttribute2 = xmlNode2.Attributes[l];
										if (xmlAttribute2.Name.Equals("time", StringComparison.OrdinalIgnoreCase))
										{
											item2.Time = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture.NumberFormat);
										}
									}
									string[] array = xmlNode2.InnerText.Split(new char[]
									{
										','
									});
									item2.Value.X = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
									item2.Value.Y = float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
									list2.Add(item2);
								}
							}
						}
					}
					else if (xmlNode.Name.Equals("dialog", StringComparison.OrdinalIgnoreCase))
					{
						for (int m = 0; m < xmlNode.Attributes.Count; m++)
						{
							XmlAttribute xmlAttribute3 = xmlNode.Attributes[m];
							if (xmlAttribute3.Name.Equals("subtitles", StringComparison.OrdinalIgnoreCase))
							{
								this.mSubTitles = xmlAttribute3.Value.ToLowerInvariant();
								this.mSubTitlesHash = this.mSubTitles.GetHashCodeCustom();
							}
						}
						this.mDialog = xmlNode.InnerText.ToLowerInvariant();
						string[] array2 = this.mDialog.Split(new char[]
						{
							'/'
						});
						if (array2.Length == 1)
						{
							this.mDialogBank = Banks.WaveBank;
							this.mDialogHash = this.mDialog.GetHashCodeCustom();
						}
						else
						{
							if (array2.Length != 2)
							{
								throw new Exception("Invalid sound description: " + this.mDialog);
							}
							this.mDialogBank = (Banks)Enum.Parse(typeof(Banks), array2[0], true);
							this.mDialogHash = array2[1].GetHashCodeCustom();
						}
					}
				}
			}
			list.Sort();
			this.mZoomKeys = list.ToArray();
			list2.Sort();
			this.mPositionKeys = list2.ToArray();
			this.mDuration = Math.Max(this.mPositionKeys[this.mPositionKeys.Length - 1].Time, this.mZoomKeys[this.mZoomKeys.Length - 1].Time);
		}

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x060015F4 RID: 5620 RVA: 0x0008B611 File Offset: 0x00089811
		public int SubTitles
		{
			get
			{
				return this.mSubTitlesHash;
			}
		}

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x060015F5 RID: 5621 RVA: 0x0008B619 File Offset: 0x00089819
		public Banks DialogBank
		{
			get
			{
				return this.mDialogBank;
			}
		}

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x060015F6 RID: 5622 RVA: 0x0008B621 File Offset: 0x00089821
		public int Dialog
		{
			get
			{
				return this.mDialogHash;
			}
		}

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x060015F7 RID: 5623 RVA: 0x0008B629 File Offset: 0x00089829
		public float Duration
		{
			get
			{
				return this.mDuration;
			}
		}

		// Token: 0x060015F8 RID: 5624 RVA: 0x0008B631 File Offset: 0x00089831
		public void GetCamera(float iTime, out Vector2 oPosition, out float oZoom)
		{
			Cutscene.TimeFloat.Lerp(this.mZoomKeys, iTime, out oZoom);
			Cutscene.TimeVector2.Lerp(this.mPositionKeys, iTime, out oPosition);
		}

		// Token: 0x0400172A RID: 5930
		private Cutscene.TimeFloat[] mZoomKeys;

		// Token: 0x0400172B RID: 5931
		private Cutscene.TimeVector2[] mPositionKeys;

		// Token: 0x0400172C RID: 5932
		private string mDialog;

		// Token: 0x0400172D RID: 5933
		private Banks mDialogBank = Banks.WaveBank;

		// Token: 0x0400172E RID: 5934
		private int mDialogHash;

		// Token: 0x0400172F RID: 5935
		private string mSubTitles;

		// Token: 0x04001730 RID: 5936
		private int mSubTitlesHash;

		// Token: 0x04001731 RID: 5937
		private float mDuration;

		// Token: 0x020002CE RID: 718
		private struct TimeVector2 : IComparable<Cutscene.TimeVector2>
		{
			// Token: 0x060015F9 RID: 5625 RVA: 0x0008B64D File Offset: 0x0008984D
			public static void Lerp(ref Vector2 iA, ref Vector2 iB, float iAmount, out Vector2 oValue)
			{
				Vector2.SmoothStep(ref iA, ref iB, iAmount, out oValue);
			}

			// Token: 0x060015FA RID: 5626 RVA: 0x0008B658 File Offset: 0x00089858
			public static void Lerp(Cutscene.TimeVector2[] iKeys, float iTime, out Vector2 oValue)
			{
				int num = iKeys.Length;
				int i = 0;
				while (i < num)
				{
					if (iTime < iKeys[i].Time)
					{
						if (i == 0)
						{
							oValue = iKeys[0].Value;
							return;
						}
						float iAmount = (iTime - iKeys[i - 1].Time) / (iKeys[i].Time - iKeys[i - 1].Time);
						Cutscene.TimeVector2.Lerp(ref iKeys[i - 1].Value, ref iKeys[i].Value, iAmount, out oValue);
						return;
					}
					else
					{
						i++;
					}
				}
				num--;
				oValue = iKeys[num].Value;
			}

			// Token: 0x060015FB RID: 5627 RVA: 0x0008B6FF File Offset: 0x000898FF
			public int CompareTo(Cutscene.TimeVector2 other)
			{
				if (this.Time < other.Time)
				{
					return -1;
				}
				if (this.Time > other.Time)
				{
					return 1;
				}
				return 0;
			}

			// Token: 0x04001732 RID: 5938
			public float Time;

			// Token: 0x04001733 RID: 5939
			public Vector2 Value;
		}

		// Token: 0x020002CF RID: 719
		private struct TimeFloat : IComparable<Cutscene.TimeFloat>
		{
			// Token: 0x060015FC RID: 5628 RVA: 0x0008B724 File Offset: 0x00089924
			public static void Lerp(float iA, float iB, float iAmount, out float oValue)
			{
				oValue = MathHelper.SmoothStep(iA, iB, iAmount);
			}

			// Token: 0x060015FD RID: 5629 RVA: 0x0008B730 File Offset: 0x00089930
			public static void Lerp(Cutscene.TimeFloat[] iKeys, float iTime, out float oValue)
			{
				int num = iKeys.Length;
				int i = 0;
				while (i < num)
				{
					if (iTime < iKeys[i].Time)
					{
						if (i == 0)
						{
							oValue = iKeys[0].Value;
							return;
						}
						float iAmount = (iTime - iKeys[i - 1].Time) / (iKeys[i].Time - iKeys[i - 1].Time);
						Cutscene.TimeFloat.Lerp(iKeys[i - 1].Value, iKeys[i].Value, iAmount, out oValue);
						return;
					}
					else
					{
						i++;
					}
				}
				num--;
				oValue = iKeys[num].Value;
			}

			// Token: 0x060015FE RID: 5630 RVA: 0x0008B7CF File Offset: 0x000899CF
			public int CompareTo(Cutscene.TimeFloat other)
			{
				if (this.Time < other.Time)
				{
					return -1;
				}
				if (this.Time > other.Time)
				{
					return 1;
				}
				return 0;
			}

			// Token: 0x04001734 RID: 5940
			public float Time;

			// Token: 0x04001735 RID: 5941
			public float Value;
		}
	}
}
