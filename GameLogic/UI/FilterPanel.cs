// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.FilterPanel
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.UI;

public class FilterPanel : MenuItem
{
  private const int NR_FILTERS = 5;
  private const int NR_DROP_DOWN = 2;
  private const int NR_CHECKBOXES = 3;
  private const int SCOPE = 0;
  private const int LATENCY = 1;
  private const int FILTER_PRIVATE = 0;
  private const int FILTER_PLAYING = 1;
  private const int VAC = 2;
  private string[] mToolTips;
  private DropDownBox[] mFilterDropDownBoxes;
  private MenuTextItem[] mFilterTitles;
  private FilterData mFilter;
  private CheckBox[] mFilterCheckBoxes;
  private MenuImageItem mRefreshButton;
  private int mSelectedIndex;
  private DropDownBox mActiveChild;
  public string[] mLatencyLookup = new string[6]
  {
    "∞",
    "< 100",
    "< 200",
    "< 300",
    "< 400",
    "< 500"
  };
  public Action<FilterData> FilterDataChanged;
  public Action<FilterData> RefreshRequest;

  public void CreateFilterPanel()
  {
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    this.mFilterDropDownBoxes = new DropDownBox[2];
    this.mFilterCheckBoxes = new CheckBox[3];
    this.mFilterTitles = new MenuTextItem[5];
    this.mToolTips = this.CreateToolTips();
    Texture2D iTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
    VertexBuffer vertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 64 /*0x40*/, BufferUsage.WriteOnly);
    VertexDeclaration vertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[2]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
    });
    this.SetVertices(iTexture, 64 /*0x40*/, 64 /*0x40*/, new Point(1600, 98), new Point(64 /*0x40*/, 64 /*0x40*/), vertexBuffer);
    this.mRefreshButton = new MenuImageItem(new Vector2(), iTexture, vertexBuffer, vertexDeclaration, 0.0f, 0, 16 /*0x10*/, 64f, 64f);
    DropDownBox<Scope> dropDownBox1 = new DropDownBox<Scope>(font, new Scope[3]
    {
      Scope.FriendsOnly,
      Scope.LAN,
      Scope.WAN
    }, new int?[3]
    {
      new int?(SubMenuOnline.LOC_FRIENDS),
      new int?(SubMenuOnline.LOC_LAN),
      new int?(SubMenuOnline.LOC_ONLINE)
    }, 180);
    dropDownBox1.ValueChanged += new Action<DropDownBox, Scope>(this.ScopeValueChanged);
    this.mFilterDropDownBoxes[0] = (DropDownBox) dropDownBox1;
    this.mFilterTitles[0] = new MenuTextItem(SubMenuOnline.LOC_SEARCH_SCOPE, new Vector2(), font, TextAlign.Left);
    this.mLatencyLookup[0] = LanguageManager.Instance.GetString(SubMenu.LOC_ANY);
    DropDownBox<string> dropDownBox2 = new DropDownBox<string>(font, this.mLatencyLookup, (int?[]) null, 100);
    dropDownBox2.ValueChanged += new Action<DropDownBox, string>(this.LatencyValueChanged);
    this.mFilterDropDownBoxes[1] = (DropDownBox) dropDownBox2;
    this.mFilterTitles[1] = new MenuTextItem(SubMenuOnline.LOC_LATENCY, new Vector2(), font, TextAlign.Left);
    this.mFilterCheckBoxes[2] = new CheckBox(false);
    this.mFilterCheckBoxes[2].OnCheckedChanged += new Action<CheckBox>(this.VACProtectionValueChanged);
    this.mFilterTitles[4] = new MenuTextItem(SubMenuOnline.LOC_VAC_ONLY, new Vector2(), font, TextAlign.Left);
    this.mFilterCheckBoxes[0] = new CheckBox(false);
    this.mFilterCheckBoxes[0].OnCheckedChanged += new Action<CheckBox>(this.PasswordValueChanged);
    this.mFilterTitles[2] = new MenuTextItem(SubMenuOnline.LOC_PRIVATE, new Vector2(), font, TextAlign.Left);
    this.mFilterCheckBoxes[1] = new CheckBox(false);
    this.mFilterCheckBoxes[1].OnCheckedChanged += new Action<CheckBox>(this.FilterPlayingValueChanged);
    this.mFilterTitles[3] = new MenuTextItem(SubMenuOnline.LOC_PLAYING, new Vector2(), font, TextAlign.Left);
    this.InitializeFilters();
  }

  public void AddToMenu(List<MenuItem> iMenuItems)
  {
    iMenuItems.Add((MenuItem) this.mFilterCheckBoxes[0]);
    iMenuItems.Add((MenuItem) this.mFilterCheckBoxes[1]);
    iMenuItems.Add((MenuItem) this.mFilterCheckBoxes[2]);
  }

  private string[] CreateToolTips()
  {
    return new string[6]
    {
      LanguageManager.Instance.GetString(SubMenuOnline.LOC_SEARCH_SCOPE),
      LanguageManager.Instance.GetString(SubMenuOnline.LOC_LATENCY),
      LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_HIDE_PRIVATE_GAMES),
      LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_HIDE_GAMES_IN_PROGRESS),
      LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_SHOW_ONLY_GAMES_PROTECTED_BY_VAC),
      LanguageManager.Instance.GetString(SubMenuOnline.LOC_REFRESH)
    };
  }

  public override void LanguageChanged()
  {
    this.mToolTips = this.CreateToolTips();
    for (int index = 0; index < 5; ++index)
    {
      if (index < 2)
        this.mFilterDropDownBoxes[index].LanguageChanged();
      else
        this.mFilterCheckBoxes[index - 2].LanguageChanged();
      this.mFilterTitles[index].LanguageChanged();
    }
    this.mRefreshButton.LanguageChanged();
  }

  private void SetVertices(
    Texture2D iTexture,
    int iWidth,
    int iHeight,
    Point iTextureOffset,
    Point iTextureSize,
    VertexBuffer iVertexBuffer)
  {
    Vector4[] vector4Array = new Vector4[4];
    QuadHelper.CreateQuadFan(vector4Array, 0, new Vector2((float) iWidth / 2f, (float) iHeight / 2f), new Vector2((float) iWidth, (float) iHeight), new Vector2((float) iTextureOffset.X / (float) iTexture.Width, (float) iTextureOffset.Y / (float) iTexture.Height), new Vector2((float) iTextureSize.X / (float) iTexture.Width, (float) iTextureSize.Y / (float) iTexture.Height));
    iVertexBuffer.SetData<Vector4>(vector4Array);
  }

  public void DoLayout(Vector2 iBackgroundPos, Vector2 iBackgroundSize)
  {
    int x1 = (int) iBackgroundPos.X;
    int y = (int) ((double) iBackgroundPos.Y + (double) iBackgroundSize.Y + 5.0);
    int num1 = this.mFilterTitles[2].Text.Font.LineHeight / 2;
    int num2 = 40;
    int num3 = (int) MenuTextButtonItem.DEFAULT_SIZE.X * 2;
    this.mFilterDropDownBoxes[0].Position = new Vector2((float) (x1 + num3) - this.mFilterDropDownBoxes[0].Size.X, (float) y);
    this.mFilterTitles[0].Position = new Vector2((float) x1, (float) (y + num1));
    this.mFilterDropDownBoxes[1].Position = new Vector2((float) (x1 + num3) - this.mFilterDropDownBoxes[1].Size.X, (float) (y + num2));
    this.mFilterTitles[1].Position = new Vector2((float) x1, (float) (y + num1 + num2));
    int x2 = (int) ((double) iBackgroundPos.X + (double) iBackgroundSize.X * 0.5 - (double) num3 / 2.0);
    this.mFilterCheckBoxes[0].Position = new Vector2((float) x2, (float) y);
    this.mFilterTitles[2].Position = new Vector2((float) (x2 + this.mFilterCheckBoxes[0].Width), (float) (y + num1));
    this.mFilterCheckBoxes[1].Position = new Vector2((float) x2, (float) (y + num2));
    this.mFilterTitles[3].Position = new Vector2((float) (x2 + this.mFilterCheckBoxes[1].Width), (float) (y + num1 + num2));
    int x3 = (int) ((double) iBackgroundPos.X + (double) iBackgroundSize.X - (double) num3);
    this.mFilterCheckBoxes[2].Position = new Vector2((float) x3, (float) y);
    this.mFilterTitles[4].Position = new Vector2((float) (x3 + this.mFilterCheckBoxes[2].Width), (float) (y + num1));
    int num4 = (int) ((double) this.mRefreshButton.BottomRight.X - (double) this.mRefreshButton.TopLeft.X);
    this.mRefreshButton.Position = new Vector2((float) ((int) ((double) iBackgroundPos.X + (double) iBackgroundSize.X - (double) num4) + num4 / 2), (float) (y + num2));
  }

  private void VACProtectionValueChanged(CheckBox iSender)
  {
    this.mFilter.VACOnly = iSender.Checked;
    if (this.FilterDataChanged == null)
      return;
    this.FilterDataChanged(this.mFilter);
  }

  private void PasswordValueChanged(CheckBox iSender)
  {
    this.mFilter.FilterPassword = iSender.Checked;
    if (this.FilterDataChanged == null)
      return;
    this.FilterDataChanged(this.mFilter);
  }

  private void FilterPlayingValueChanged(CheckBox iSender)
  {
    this.mFilter.FilterPlaying = iSender.Checked;
    if (this.FilterDataChanged == null)
      return;
    this.FilterDataChanged(this.mFilter);
  }

  private void ScopeValueChanged(DropDownBox iSender, Scope iValue)
  {
    this.mFilter.Scope = iValue;
    if (this.RefreshRequest == null)
      return;
    this.RefreshRequest(this.mFilter);
  }

  private void LatencyValueChanged(DropDownBox iSender, string iValue)
  {
    int num = 0;
    if (iValue.Contains("<"))
      num = int.Parse(iValue.Substring(2, iValue.Length - 2));
    this.mFilter.MaxLatency = (short) num;
    if (this.FilterDataChanged == null)
      return;
    this.FilterDataChanged(this.mFilter);
  }

  public void InitializeFilters()
  {
    this.mFilter = GlobalSettings.Instance.Filter;
    this.mFilterCheckBoxes[2].Checked = this.mFilter.VACOnly;
    this.mFilterCheckBoxes[0].Checked = this.mFilter.FilterPassword;
    this.mFilterCheckBoxes[1].Checked = this.mFilter.FilterPlaying;
    (this.mFilterDropDownBoxes[0] as DropDownBox<Scope>).SetNewValue(this.mFilter.Scope);
    (this.mFilterDropDownBoxes[1] as DropDownBox<string>).SetNewValue(this.mLatencyLookup[(int) this.mFilter.MaxLatency / 100]);
  }

  internal void DeselectAll()
  {
    for (int index = 0; index < 5; ++index)
    {
      this.mFilterTitles[index].Selected = false;
      if (index < 2)
        this.mFilterDropDownBoxes[index].Selected = false;
      else
        this.mFilterCheckBoxes[index - 2].SetMouseHover(false);
    }
    this.mRefreshButton.Selected = false;
  }

  internal void ControllExit()
  {
    int mSelectedIndex = this.mSelectedIndex;
    if (mSelectedIndex < 2)
    {
      this.mFilterDropDownBoxes[mSelectedIndex].Selected = false;
      this.mFilterTitles[mSelectedIndex].Selected = false;
    }
    else if (mSelectedIndex < 5)
    {
      this.mFilterCheckBoxes[mSelectedIndex - 2].SetMouseHover(false);
      this.mFilterTitles[mSelectedIndex].Selected = false;
    }
    else if (mSelectedIndex == 5)
      this.mRefreshButton.Selected = false;
    this.mSelectedIndex = mSelectedIndex;
  }

  internal void ControllEnter(int iEntranceIndex)
  {
    this.DeselectAll();
    int index = iEntranceIndex;
    if (index < 2)
    {
      this.mFilterDropDownBoxes[index].Selected = true;
      this.mFilterTitles[index].Selected = true;
    }
    else if (index < 5)
    {
      this.mFilterCheckBoxes[index - 2].SetMouseHover(true);
      this.mFilterTitles[index].Selected = true;
    }
    else if (index == 5)
      this.mRefreshButton.Selected = true;
    this.mSelectedIndex = index;
  }

  internal bool ControllUpOver()
  {
    if (this.mActiveChild != null)
    {
      --this.mActiveChild.NewSelection;
      if (this.mActiveChild.NewSelection == -1)
        this.mActiveChild.NewSelection = this.mActiveChild.Count - 1;
      return false;
    }
    if (this.mSelectedIndex < 5)
    {
      if (this.mSelectedIndex % 2 == 0)
      {
        this.ControllExit();
        return true;
      }
      this.ControllEnter(this.mSelectedIndex - 1);
      return false;
    }
    this.ControllExit();
    return true;
  }

  internal int ControllDownOver()
  {
    if (this.mActiveChild != null)
    {
      ++this.mActiveChild.NewSelection;
      if (this.mActiveChild.NewSelection == this.mActiveChild.Count)
        this.mActiveChild.NewSelection = 0;
      return -1;
    }
    if (this.mSelectedIndex < 4)
    {
      if (this.mSelectedIndex % 2 == 1)
      {
        this.ControllExit();
        return this.mSelectedIndex;
      }
      this.ControllEnter(this.mSelectedIndex + 1);
      return -1;
    }
    this.ControllExit();
    return this.mSelectedIndex;
  }

  internal void ControllLeftOver()
  {
    if (this.mActiveChild != null)
      return;
    if (this.mSelectedIndex < 5)
    {
      this.mSelectedIndex -= 2;
      if (this.mSelectedIndex < 0)
        this.mSelectedIndex += 2;
    }
    else
      --this.mSelectedIndex;
    this.ControllEnter(this.mSelectedIndex);
  }

  internal void ControllRightOver()
  {
    if (this.mActiveChild != null)
      return;
    switch (this.mSelectedIndex)
    {
      case 0:
      case 1:
      case 2:
        this.mSelectedIndex += 2;
        break;
      case 3:
      case 4:
        ++this.mSelectedIndex;
        break;
    }
    this.ControllEnter(this.mSelectedIndex);
  }

  internal bool ControllB()
  {
    bool flag = this.mActiveChild != null;
    if (flag)
    {
      this.mActiveChild.IsDown = false;
      this.mActiveChild = (DropDownBox) null;
      this.ControllEnter(this.mSelectedIndex);
    }
    return flag;
  }

  internal void ControllA()
  {
    if (this.mActiveChild != null)
    {
      this.mActiveChild.SelectedIndex = this.mActiveChild.NewSelection;
      this.mActiveChild.IsDown = false;
      this.mActiveChild = (DropDownBox) null;
      this.ControllEnter(this.mSelectedIndex);
    }
    else
    {
      int mSelectedIndex = this.mSelectedIndex;
      if (mSelectedIndex < 2)
      {
        this.mFilterDropDownBoxes[mSelectedIndex].IsDown = true;
        this.mActiveChild = this.mFilterDropDownBoxes[mSelectedIndex];
        this.mFilterDropDownBoxes[mSelectedIndex].NewSelection = this.mFilterDropDownBoxes[mSelectedIndex].SelectedIndex;
        this.mFilterTitles[mSelectedIndex].Selected = false;
      }
      else if (mSelectedIndex < 5)
      {
        this.mFilterCheckBoxes[mSelectedIndex - 2].Toggle();
      }
      else
      {
        if (mSelectedIndex != 5 || this.RefreshRequest == null)
          return;
        this.RefreshRequest(this.mFilter);
      }
    }
  }

  internal string GetToolTip(out Vector2 oPosition)
  {
    if (this.mActiveChild == null && this.mSelectedIndex >= 0)
    {
      oPosition = this.mSelectedIndex >= 2 ? (this.mSelectedIndex >= 5 ? this.mRefreshButton.Position : this.mFilterCheckBoxes[this.mSelectedIndex - 2].Position) : this.mFilterDropDownBoxes[this.mSelectedIndex].Position;
      return this.mToolTips[this.mSelectedIndex];
    }
    oPosition = new Vector2();
    return (string) null;
  }

  internal bool CheckMouseMove(ref Vector2 iMousePos, ref string iToolTip)
  {
    bool flag = false;
    for (int index = 0; index < this.mFilterDropDownBoxes.Length; ++index)
    {
      if (this.mFilterDropDownBoxes[index].IsDown)
      {
        this.mFilterDropDownBoxes[index].NewSelection = this.mFilterDropDownBoxes[index].GetHitIndex(ref iMousePos);
        flag = true;
      }
    }
    if (!flag)
    {
      for (int index = 0; index < this.mFilterDropDownBoxes.Length; ++index)
      {
        this.mFilterDropDownBoxes[index].Selected = this.mFilterDropDownBoxes[index].InsideBounds(ref iMousePos) || this.mFilterTitles[index].InsideBounds(ref iMousePos);
        this.mFilterTitles[index].Selected = this.mFilterDropDownBoxes[index].Selected;
        if (this.mFilterDropDownBoxes[index].Selected)
        {
          iToolTip = this.mToolTips[index];
          this.mSelectedIndex = index;
        }
      }
      for (int index = 0; index < this.mFilterCheckBoxes.Length; ++index)
      {
        bool iHover = this.mFilterCheckBoxes[index].InsideBounds(ref iMousePos);
        if (!iHover)
          iHover = this.mFilterTitles[index + 2].InsideBounds(ref iMousePos);
        this.mFilterCheckBoxes[index].SetMouseHover(iHover);
        this.mFilterTitles[index + 2].Selected = iHover;
        if (iHover)
        {
          iToolTip = this.mToolTips[index + 2];
          this.mSelectedIndex = index + 2;
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

  internal bool CheckMouseAction(ref Vector2 iMousePos)
  {
    bool flag = false;
    for (int index = 0; index < this.mFilterDropDownBoxes.Length && !flag; ++index)
    {
      if (!this.mFilterDropDownBoxes[index].IsDown)
      {
        if (this.mFilterDropDownBoxes[index].InsideBounds(ref iMousePos))
        {
          this.mActiveChild = this.mFilterDropDownBoxes[index];
          this.mFilterDropDownBoxes[index].IsDown = true;
          this.mFilterTitles[index].Selected = false;
          flag = true;
        }
      }
      else if (this.mFilterDropDownBoxes[index].IsDown)
      {
        this.mFilterDropDownBoxes[index].SelectedIndex = this.mFilterDropDownBoxes[index].GetHitIndex(ref iMousePos);
        this.mFilterDropDownBoxes[index].IsDown = false;
        flag = true;
        this.mFilterDropDownBoxes[index].Selected = true;
        this.mFilterTitles[index].Selected = true;
        this.mActiveChild = (DropDownBox) null;
      }
    }
    if (!flag)
    {
      for (int index = 0; index < this.mFilterCheckBoxes.Length; ++index)
      {
        if (this.mFilterCheckBoxes[index].InsideBounds(ref iMousePos) || this.mFilterTitles[index + 2].InsideBounds(ref iMousePos))
        {
          this.mFilterCheckBoxes[index].DoMouseClick();
          flag = true;
        }
      }
      if (this.mRefreshButton.InsideBounds(ref iMousePos))
      {
        flag = true;
        if (this.RefreshRequest != null)
          this.RefreshRequest(this.mFilter);
      }
    }
    return flag;
  }

  internal void Update(float iDeltaTime)
  {
    for (int index = 0; index < this.mFilterDropDownBoxes.Length; ++index)
      this.mFilterDropDownBoxes[index].Update(iDeltaTime);
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, 1f);

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    for (int index = 2; index < this.mFilterTitles.Length; ++index)
      this.mFilterTitles[index].Draw(iEffect);
    for (int index = 0; index < this.mFilterCheckBoxes.Length; ++index)
      this.mFilterCheckBoxes[index].Draw(iEffect);
    int index1 = -1;
    for (int index2 = 0; index2 < this.mFilterDropDownBoxes.Length; ++index2)
    {
      if (this.mFilterDropDownBoxes[index2].IsDown)
      {
        index1 = index2;
      }
      else
      {
        this.mFilterDropDownBoxes[index2].Draw(iEffect, this.mFilterDropDownBoxes[index2].Scale, 1f);
        iEffect.Color = this.mFilterDropDownBoxes[index2].Enabled ? (this.mFilterDropDownBoxes[index2].Selected & !this.mFilterDropDownBoxes[index2].IsDown ? this.mFilterDropDownBoxes[index2].ColorSelected : this.mFilterDropDownBoxes[index2].Color) : this.mFilterDropDownBoxes[index2].ColorDisabled;
        this.mFilterTitles[index2].Draw(iEffect);
      }
    }
    if (index1 != -1)
    {
      this.mFilterDropDownBoxes[index1].Draw(iEffect, this.mFilterDropDownBoxes[index1].Scale, 1f);
      iEffect.Color = this.mFilterDropDownBoxes[index1].Enabled ? (this.mFilterDropDownBoxes[index1].Selected & !this.mFilterDropDownBoxes[index1].IsDown ? this.mFilterDropDownBoxes[index1].ColorSelected : this.mFilterDropDownBoxes[index1].Color) : this.mFilterDropDownBoxes[index1].ColorDisabled;
      this.mFilterTitles[index1].Draw(iEffect);
    }
    this.mRefreshButton.Draw(iEffect);
  }

  protected override void UpdateBoundingBox()
  {
  }
}
