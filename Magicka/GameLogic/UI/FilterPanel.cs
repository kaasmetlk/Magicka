using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x0200029B RID: 667
	public class FilterPanel : MenuItem
	{
		// Token: 0x060013D8 RID: 5080 RVA: 0x00078DBC File Offset: 0x00076FBC
		public void CreateFilterPanel()
		{
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			this.mFilterDropDownBoxes = new DropDownBox[2];
			this.mFilterCheckBoxes = new CheckBox[3];
			this.mFilterTitles = new MenuTextItem[5];
			this.mToolTips = this.CreateToolTips();
			Texture2D iTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, 64, BufferUsage.WriteOnly);
			VertexDeclaration iDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
			});
			this.SetVertices(iTexture, 64, 64, new Point(1600, 98), new Point(64, 64), vertexBuffer);
			this.mRefreshButton = new MenuImageItem(default(Vector2), iTexture, vertexBuffer, iDeclaration, 0f, 0, 16, 64f, 64f);
			BitmapFont iFont = font;
			Scope[] array = new Scope[3];
			array[0] = Scope.FriendsOnly;
			array[1] = Scope.LAN;
			DropDownBox<Scope> dropDownBox = new DropDownBox<Scope>(iFont, array, new int?[]
			{
				new int?(SubMenuOnline.LOC_FRIENDS),
				new int?(SubMenuOnline.LOC_LAN),
				new int?(SubMenuOnline.LOC_ONLINE)
			}, 180);
			dropDownBox.ValueChanged += new Action<DropDownBox, Scope>(this.ScopeValueChanged);
			this.mFilterDropDownBoxes[0] = dropDownBox;
			this.mFilterTitles[0] = new MenuTextItem(SubMenuOnline.LOC_SEARCH_SCOPE, default(Vector2), font, TextAlign.Left);
			this.mLatencyLookup[0] = LanguageManager.Instance.GetString(SubMenu.LOC_ANY);
			DropDownBox<string> dropDownBox2 = new DropDownBox<string>(font, this.mLatencyLookup, null, 100);
			dropDownBox2.ValueChanged += new Action<DropDownBox, string>(this.LatencyValueChanged);
			this.mFilterDropDownBoxes[1] = dropDownBox2;
			this.mFilterTitles[1] = new MenuTextItem(SubMenuOnline.LOC_LATENCY, default(Vector2), font, TextAlign.Left);
			this.mFilterCheckBoxes[2] = new CheckBox(false);
			this.mFilterCheckBoxes[2].OnCheckedChanged += this.VACProtectionValueChanged;
			this.mFilterTitles[4] = new MenuTextItem(SubMenuOnline.LOC_VAC_ONLY, default(Vector2), font, TextAlign.Left);
			this.mFilterCheckBoxes[0] = new CheckBox(false);
			this.mFilterCheckBoxes[0].OnCheckedChanged += this.PasswordValueChanged;
			this.mFilterTitles[2] = new MenuTextItem(SubMenuOnline.LOC_PRIVATE, default(Vector2), font, TextAlign.Left);
			this.mFilterCheckBoxes[1] = new CheckBox(false);
			this.mFilterCheckBoxes[1].OnCheckedChanged += this.FilterPlayingValueChanged;
			this.mFilterTitles[3] = new MenuTextItem(SubMenuOnline.LOC_PLAYING, default(Vector2), font, TextAlign.Left);
			this.InitializeFilters();
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x000790A4 File Offset: 0x000772A4
		public void AddToMenu(List<MenuItem> iMenuItems)
		{
			iMenuItems.Add(this.mFilterCheckBoxes[0]);
			iMenuItems.Add(this.mFilterCheckBoxes[1]);
			iMenuItems.Add(this.mFilterCheckBoxes[2]);
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x000790D0 File Offset: 0x000772D0
		private string[] CreateToolTips()
		{
			return new string[]
			{
				LanguageManager.Instance.GetString(SubMenuOnline.LOC_SEARCH_SCOPE),
				LanguageManager.Instance.GetString(SubMenuOnline.LOC_LATENCY),
				LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_HIDE_PRIVATE_GAMES),
				LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_HIDE_GAMES_IN_PROGRESS),
				LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_SHOW_ONLY_GAMES_PROTECTED_BY_VAC),
				LanguageManager.Instance.GetString(SubMenuOnline.LOC_REFRESH)
			};
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x00079154 File Offset: 0x00077354
		public override void LanguageChanged()
		{
			this.mToolTips = this.CreateToolTips();
			for (int i = 0; i < 5; i++)
			{
				if (i < 2)
				{
					this.mFilterDropDownBoxes[i].LanguageChanged();
				}
				else
				{
					this.mFilterCheckBoxes[i - 2].LanguageChanged();
				}
				this.mFilterTitles[i].LanguageChanged();
			}
			this.mRefreshButton.LanguageChanged();
		}

		// Token: 0x060013DC RID: 5084 RVA: 0x000791B4 File Offset: 0x000773B4
		private void SetVertices(Texture2D iTexture, int iWidth, int iHeight, Point iTextureOffset, Point iTextureSize, VertexBuffer iVertexBuffer)
		{
			Vector4[] array = new Vector4[4];
			QuadHelper.CreateQuadFan(array, 0, new Vector2((float)iWidth / 2f, (float)iHeight / 2f), new Vector2((float)iWidth, (float)iHeight), new Vector2((float)iTextureOffset.X / (float)iTexture.Width, (float)iTextureOffset.Y / (float)iTexture.Height), new Vector2((float)iTextureSize.X / (float)iTexture.Width, (float)iTextureSize.Y / (float)iTexture.Height));
			iVertexBuffer.SetData<Vector4>(array);
		}

		// Token: 0x060013DD RID: 5085 RVA: 0x00079240 File Offset: 0x00077440
		public void DoLayout(Vector2 iBackgroundPos, Vector2 iBackgroundSize)
		{
			int num = (int)iBackgroundPos.X;
			int num2 = (int)(iBackgroundPos.Y + iBackgroundSize.Y + 5f);
			Text text = this.mFilterTitles[2].Text;
			int num3 = text.Font.LineHeight / 2;
			int num4 = 40;
			int num5 = (int)MenuTextButtonItem.DEFAULT_SIZE.X * 2;
			this.mFilterDropDownBoxes[0].Position = new Vector2((float)(num + num5) - this.mFilterDropDownBoxes[0].Size.X, (float)num2);
			this.mFilterTitles[0].Position = new Vector2((float)num, (float)(num2 + num3));
			this.mFilterDropDownBoxes[1].Position = new Vector2((float)(num + num5) - this.mFilterDropDownBoxes[1].Size.X, (float)(num2 + num4));
			this.mFilterTitles[1].Position = new Vector2((float)num, (float)(num2 + num3 + num4));
			num = (int)(iBackgroundPos.X + iBackgroundSize.X * 0.5f - (float)num5 / 2f);
			this.mFilterCheckBoxes[0].Position = new Vector2((float)num, (float)num2);
			this.mFilterTitles[2].Position = new Vector2((float)(num + this.mFilterCheckBoxes[0].Width), (float)(num2 + num3));
			this.mFilterCheckBoxes[1].Position = new Vector2((float)num, (float)(num2 + num4));
			this.mFilterTitles[3].Position = new Vector2((float)(num + this.mFilterCheckBoxes[1].Width), (float)(num2 + num3 + num4));
			num = (int)(iBackgroundPos.X + iBackgroundSize.X - (float)num5);
			this.mFilterCheckBoxes[2].Position = new Vector2((float)num, (float)num2);
			this.mFilterTitles[4].Position = new Vector2((float)(num + this.mFilterCheckBoxes[2].Width), (float)(num2 + num3));
			int num6 = (int)(this.mRefreshButton.BottomRight.X - this.mRefreshButton.TopLeft.X);
			num = (int)(iBackgroundPos.X + iBackgroundSize.X - (float)num6);
			this.mRefreshButton.Position = new Vector2((float)(num + num6 / 2), (float)(num2 + num4));
		}

		// Token: 0x060013DE RID: 5086 RVA: 0x0007946F File Offset: 0x0007766F
		private void VACProtectionValueChanged(CheckBox iSender)
		{
			this.mFilter.VACOnly = iSender.Checked;
			if (this.FilterDataChanged != null)
			{
				this.FilterDataChanged(this.mFilter);
			}
		}

		// Token: 0x060013DF RID: 5087 RVA: 0x0007949B File Offset: 0x0007769B
		private void PasswordValueChanged(CheckBox iSender)
		{
			this.mFilter.FilterPassword = iSender.Checked;
			if (this.FilterDataChanged != null)
			{
				this.FilterDataChanged(this.mFilter);
			}
		}

		// Token: 0x060013E0 RID: 5088 RVA: 0x000794C7 File Offset: 0x000776C7
		private void FilterPlayingValueChanged(CheckBox iSender)
		{
			this.mFilter.FilterPlaying = iSender.Checked;
			if (this.FilterDataChanged != null)
			{
				this.FilterDataChanged(this.mFilter);
			}
		}

		// Token: 0x060013E1 RID: 5089 RVA: 0x000794F3 File Offset: 0x000776F3
		private void ScopeValueChanged(DropDownBox iSender, Scope iValue)
		{
			this.mFilter.Scope = iValue;
			if (this.RefreshRequest != null)
			{
				this.RefreshRequest(this.mFilter);
			}
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x0007951C File Offset: 0x0007771C
		private void LatencyValueChanged(DropDownBox iSender, string iValue)
		{
			int num = 0;
			if (iValue.Contains("<"))
			{
				num = int.Parse(iValue.Substring(2, iValue.Length - 2));
			}
			this.mFilter.MaxLatency = (short)num;
			if (this.FilterDataChanged != null)
			{
				this.FilterDataChanged(this.mFilter);
			}
		}

		// Token: 0x060013E3 RID: 5091 RVA: 0x00079574 File Offset: 0x00077774
		public void InitializeFilters()
		{
			this.mFilter = GlobalSettings.Instance.Filter;
			this.mFilterCheckBoxes[2].Checked = this.mFilter.VACOnly;
			this.mFilterCheckBoxes[0].Checked = this.mFilter.FilterPassword;
			this.mFilterCheckBoxes[1].Checked = this.mFilter.FilterPlaying;
			(this.mFilterDropDownBoxes[0] as DropDownBox<Scope>).SetNewValue(this.mFilter.Scope);
			(this.mFilterDropDownBoxes[1] as DropDownBox<string>).SetNewValue(this.mLatencyLookup[(int)(this.mFilter.MaxLatency / 100)]);
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x00079620 File Offset: 0x00077820
		internal void DeselectAll()
		{
			for (int i = 0; i < 5; i++)
			{
				this.mFilterTitles[i].Selected = false;
				if (i < 2)
				{
					this.mFilterDropDownBoxes[i].Selected = false;
				}
				else
				{
					this.mFilterCheckBoxes[i - 2].SetMouseHover(false);
				}
			}
			this.mRefreshButton.Selected = false;
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x00079678 File Offset: 0x00077878
		internal void ControllExit()
		{
			int num = this.mSelectedIndex;
			if (num < 2)
			{
				this.mFilterDropDownBoxes[num].Selected = false;
				this.mFilterTitles[num].Selected = false;
			}
			else if (num < 5)
			{
				this.mFilterCheckBoxes[num - 2].SetMouseHover(false);
				this.mFilterTitles[num].Selected = false;
			}
			else if (num == 5)
			{
				this.mRefreshButton.Selected = false;
			}
			this.mSelectedIndex = num;
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x000796EC File Offset: 0x000778EC
		internal void ControllEnter(int iEntranceIndex)
		{
			this.DeselectAll();
			if (iEntranceIndex < 2)
			{
				this.mFilterDropDownBoxes[iEntranceIndex].Selected = true;
				this.mFilterTitles[iEntranceIndex].Selected = true;
			}
			else if (iEntranceIndex < 5)
			{
				this.mFilterCheckBoxes[iEntranceIndex - 2].SetMouseHover(true);
				this.mFilterTitles[iEntranceIndex].Selected = true;
			}
			else if (iEntranceIndex == 5)
			{
				this.mRefreshButton.Selected = true;
			}
			this.mSelectedIndex = iEntranceIndex;
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x00079760 File Offset: 0x00077960
		internal bool ControllUpOver()
		{
			if (this.mActiveChild != null)
			{
				this.mActiveChild.NewSelection--;
				if (this.mActiveChild.NewSelection == -1)
				{
					this.mActiveChild.NewSelection = this.mActiveChild.Count - 1;
				}
				return false;
			}
			if (this.mSelectedIndex >= 5)
			{
				this.ControllExit();
				return true;
			}
			if (this.mSelectedIndex % 2 == 0)
			{
				this.ControllExit();
				return true;
			}
			this.ControllEnter(this.mSelectedIndex - 1);
			return false;
		}

		// Token: 0x060013E8 RID: 5096 RVA: 0x000797E4 File Offset: 0x000779E4
		internal int ControllDownOver()
		{
			if (this.mActiveChild != null)
			{
				this.mActiveChild.NewSelection++;
				if (this.mActiveChild.NewSelection == this.mActiveChild.Count)
				{
					this.mActiveChild.NewSelection = 0;
				}
				return -1;
			}
			if (this.mSelectedIndex >= 4)
			{
				this.ControllExit();
				return this.mSelectedIndex;
			}
			if (this.mSelectedIndex % 2 == 1)
			{
				this.ControllExit();
				return this.mSelectedIndex;
			}
			this.ControllEnter(this.mSelectedIndex + 1);
			return -1;
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x00079874 File Offset: 0x00077A74
		internal void ControllLeftOver()
		{
			if (this.mActiveChild != null)
			{
				return;
			}
			if (this.mSelectedIndex < 5)
			{
				this.mSelectedIndex -= 2;
				if (this.mSelectedIndex < 0)
				{
					this.mSelectedIndex += 2;
				}
			}
			else
			{
				this.mSelectedIndex--;
			}
			this.ControllEnter(this.mSelectedIndex);
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x000798D4 File Offset: 0x00077AD4
		internal void ControllRightOver()
		{
			if (this.mActiveChild != null)
			{
				return;
			}
			switch (this.mSelectedIndex)
			{
			case 0:
			case 1:
			case 2:
				this.mSelectedIndex += 2;
				break;
			case 3:
			case 4:
				this.mSelectedIndex++;
				break;
			}
			this.ControllEnter(this.mSelectedIndex);
		}

		// Token: 0x060013EB RID: 5099 RVA: 0x0007993C File Offset: 0x00077B3C
		internal bool ControllB()
		{
			bool flag = this.mActiveChild != null;
			if (flag)
			{
				this.mActiveChild.IsDown = false;
				this.mActiveChild = null;
				this.ControllEnter(this.mSelectedIndex);
			}
			return flag;
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x0007997C File Offset: 0x00077B7C
		internal void ControllA()
		{
			if (this.mActiveChild != null)
			{
				this.mActiveChild.SelectedIndex = this.mActiveChild.NewSelection;
				this.mActiveChild.IsDown = false;
				this.mActiveChild = null;
				this.ControllEnter(this.mSelectedIndex);
				return;
			}
			int num = this.mSelectedIndex;
			if (num < 2)
			{
				this.mFilterDropDownBoxes[num].IsDown = true;
				this.mActiveChild = this.mFilterDropDownBoxes[num];
				this.mFilterDropDownBoxes[num].NewSelection = this.mFilterDropDownBoxes[num].SelectedIndex;
				this.mFilterTitles[num].Selected = false;
				return;
			}
			if (num < 5)
			{
				this.mFilterCheckBoxes[num - 2].Toggle();
				return;
			}
			if (num == 5 && this.RefreshRequest != null)
			{
				this.RefreshRequest(this.mFilter);
			}
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x00079A48 File Offset: 0x00077C48
		internal string GetToolTip(out Vector2 oPosition)
		{
			if (this.mActiveChild == null && this.mSelectedIndex >= 0)
			{
				if (this.mSelectedIndex < 2)
				{
					oPosition = this.mFilterDropDownBoxes[this.mSelectedIndex].Position;
				}
				else if (this.mSelectedIndex < 5)
				{
					oPosition = this.mFilterCheckBoxes[this.mSelectedIndex - 2].Position;
				}
				else
				{
					oPosition = this.mRefreshButton.Position;
				}
				return this.mToolTips[this.mSelectedIndex];
			}
			oPosition = default(Vector2);
			return null;
		}

		// Token: 0x060013EE RID: 5102 RVA: 0x00079AD8 File Offset: 0x00077CD8
		internal bool CheckMouseMove(ref Vector2 iMousePos, ref string iToolTip)
		{
			bool flag = false;
			for (int i = 0; i < this.mFilterDropDownBoxes.Length; i++)
			{
				if (this.mFilterDropDownBoxes[i].IsDown)
				{
					this.mFilterDropDownBoxes[i].NewSelection = this.mFilterDropDownBoxes[i].GetHitIndex(ref iMousePos);
					flag = true;
				}
			}
			if (!flag)
			{
				for (int j = 0; j < this.mFilterDropDownBoxes.Length; j++)
				{
					this.mFilterDropDownBoxes[j].Selected = (this.mFilterDropDownBoxes[j].InsideBounds(ref iMousePos) || this.mFilterTitles[j].InsideBounds(ref iMousePos));
					this.mFilterTitles[j].Selected = this.mFilterDropDownBoxes[j].Selected;
					if (this.mFilterDropDownBoxes[j].Selected)
					{
						iToolTip = this.mToolTips[j];
						this.mSelectedIndex = j;
					}
				}
				for (int k = 0; k < this.mFilterCheckBoxes.Length; k++)
				{
					bool flag2 = this.mFilterCheckBoxes[k].InsideBounds(ref iMousePos);
					if (!flag2)
					{
						flag2 = this.mFilterTitles[k + 2].InsideBounds(ref iMousePos);
					}
					this.mFilterCheckBoxes[k].SetMouseHover(flag2);
					this.mFilterTitles[k + 2].Selected = flag2;
					if (flag2)
					{
						iToolTip = this.mToolTips[k + 2];
						this.mSelectedIndex = k + 2;
					}
				}
				this.mRefreshButton.Selected = this.mRefreshButton.InsideBounds(ref iMousePos);
				if (this.mRefreshButton.Selected)
				{
					iToolTip = "Refresh";
					this.mSelectedIndex = 5;
				}
			}
			return flag || iToolTip != null;
		}

		// Token: 0x060013EF RID: 5103 RVA: 0x00079C58 File Offset: 0x00077E58
		internal bool CheckMouseAction(ref Vector2 iMousePos)
		{
			bool flag = false;
			int num = 0;
			while (num < this.mFilterDropDownBoxes.Length && !flag)
			{
				if (!this.mFilterDropDownBoxes[num].IsDown)
				{
					if (this.mFilterDropDownBoxes[num].InsideBounds(ref iMousePos))
					{
						this.mActiveChild = this.mFilterDropDownBoxes[num];
						this.mFilterDropDownBoxes[num].IsDown = true;
						this.mFilterTitles[num].Selected = false;
						flag = true;
					}
				}
				else if (this.mFilterDropDownBoxes[num].IsDown)
				{
					this.mFilterDropDownBoxes[num].SelectedIndex = this.mFilterDropDownBoxes[num].GetHitIndex(ref iMousePos);
					this.mFilterDropDownBoxes[num].IsDown = false;
					flag = true;
					this.mFilterDropDownBoxes[num].Selected = true;
					this.mFilterTitles[num].Selected = true;
					this.mActiveChild = null;
				}
				num++;
			}
			if (!flag)
			{
				for (int i = 0; i < this.mFilterCheckBoxes.Length; i++)
				{
					if (this.mFilterCheckBoxes[i].InsideBounds(ref iMousePos) || this.mFilterTitles[i + 2].InsideBounds(ref iMousePos))
					{
						this.mFilterCheckBoxes[i].DoMouseClick();
						flag = true;
					}
				}
				if (this.mRefreshButton.InsideBounds(ref iMousePos))
				{
					flag = true;
					if (this.RefreshRequest != null)
					{
						this.RefreshRequest(this.mFilter);
					}
				}
			}
			return flag;
		}

		// Token: 0x060013F0 RID: 5104 RVA: 0x00079DA4 File Offset: 0x00077FA4
		internal void Update(float iDeltaTime)
		{
			for (int i = 0; i < this.mFilterDropDownBoxes.Length; i++)
			{
				this.mFilterDropDownBoxes[i].Update(iDeltaTime);
			}
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x00079DD2 File Offset: 0x00077FD2
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, 1f);
		}

		// Token: 0x060013F2 RID: 5106 RVA: 0x00079DE0 File Offset: 0x00077FE0
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			for (int i = 2; i < this.mFilterTitles.Length; i++)
			{
				this.mFilterTitles[i].Draw(iEffect);
			}
			for (int j = 0; j < this.mFilterCheckBoxes.Length; j++)
			{
				this.mFilterCheckBoxes[j].Draw(iEffect);
			}
			int num = -1;
			for (int k = 0; k < this.mFilterDropDownBoxes.Length; k++)
			{
				if (this.mFilterDropDownBoxes[k].IsDown)
				{
					num = k;
				}
				else
				{
					this.mFilterDropDownBoxes[k].Draw(iEffect, this.mFilterDropDownBoxes[k].Scale, 1f);
					iEffect.Color = (this.mFilterDropDownBoxes[k].Enabled ? ((this.mFilterDropDownBoxes[k].Selected & !this.mFilterDropDownBoxes[k].IsDown) ? this.mFilterDropDownBoxes[k].ColorSelected : this.mFilterDropDownBoxes[k].Color) : this.mFilterDropDownBoxes[k].ColorDisabled);
					this.mFilterTitles[k].Draw(iEffect);
				}
			}
			if (num != -1)
			{
				this.mFilterDropDownBoxes[num].Draw(iEffect, this.mFilterDropDownBoxes[num].Scale, 1f);
				iEffect.Color = (this.mFilterDropDownBoxes[num].Enabled ? ((this.mFilterDropDownBoxes[num].Selected & !this.mFilterDropDownBoxes[num].IsDown) ? this.mFilterDropDownBoxes[num].ColorSelected : this.mFilterDropDownBoxes[num].Color) : this.mFilterDropDownBoxes[num].ColorDisabled);
				this.mFilterTitles[num].Draw(iEffect);
			}
			this.mRefreshButton.Draw(iEffect);
		}

		// Token: 0x060013F3 RID: 5107 RVA: 0x00079F8F File Offset: 0x0007818F
		protected override void UpdateBoundingBox()
		{
		}

		// Token: 0x0400154F RID: 5455
		private const int NR_FILTERS = 5;

		// Token: 0x04001550 RID: 5456
		private const int NR_DROP_DOWN = 2;

		// Token: 0x04001551 RID: 5457
		private const int NR_CHECKBOXES = 3;

		// Token: 0x04001552 RID: 5458
		private const int SCOPE = 0;

		// Token: 0x04001553 RID: 5459
		private const int LATENCY = 1;

		// Token: 0x04001554 RID: 5460
		private const int FILTER_PRIVATE = 0;

		// Token: 0x04001555 RID: 5461
		private const int FILTER_PLAYING = 1;

		// Token: 0x04001556 RID: 5462
		private const int VAC = 2;

		// Token: 0x04001557 RID: 5463
		private string[] mToolTips;

		// Token: 0x04001558 RID: 5464
		private DropDownBox[] mFilterDropDownBoxes;

		// Token: 0x04001559 RID: 5465
		private MenuTextItem[] mFilterTitles;

		// Token: 0x0400155A RID: 5466
		private FilterData mFilter;

		// Token: 0x0400155B RID: 5467
		private CheckBox[] mFilterCheckBoxes;

		// Token: 0x0400155C RID: 5468
		private MenuImageItem mRefreshButton;

		// Token: 0x0400155D RID: 5469
		private int mSelectedIndex;

		// Token: 0x0400155E RID: 5470
		private DropDownBox mActiveChild;

		// Token: 0x0400155F RID: 5471
		public string[] mLatencyLookup = new string[]
		{
			"∞",
			"< 100",
			"< 200",
			"< 300",
			"< 400",
			"< 500"
		};

		// Token: 0x04001560 RID: 5472
		public Action<FilterData> FilterDataChanged;

		// Token: 0x04001561 RID: 5473
		public Action<FilterData> RefreshRequest;
	}
}
