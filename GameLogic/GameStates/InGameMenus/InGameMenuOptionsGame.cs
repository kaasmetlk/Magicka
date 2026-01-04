// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenuOptionsGame
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using PolygonHead;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal class InGameMenuOptionsGame : InGameMenu
{
  private const string OPTION_BLOOD_GORE = "blood_and_gore";
  private const string OPTION_DAMAGE_NUMBERS = "damage_numbers";
  private const string OPTION_HEALTH_BARS = "health_bars";
  private const string OPTION_SPELL_WHEELS = "spell_wheels";
  private const string OPTION_BACK = "back";
  private static readonly string[] OPTION_STRINGS = new string[5]
  {
    "blood_and_gore",
    "damage_numbers",
    "health_bars",
    "spell_wheels",
    "back"
  };
  private static InGameMenuOptionsGame sSingelton;
  private static volatile object sSingeltonLock = new object();
  private List<MenuTextItem> mOptions;
  private SettingOptions mBloodAndGore;
  private SettingOptions mDamageNumbers;
  private SettingOptions mHealthBars;
  private SettingOptions mSpellWheel;
  private GlobalSettings mGlobalSettings = GlobalSettings.Instance;

  public static InGameMenuOptionsGame Instance
  {
    get
    {
      if (InGameMenuOptionsGame.sSingelton == null)
      {
        lock (InGameMenuOptionsGame.sSingeltonLock)
        {
          if (InGameMenuOptionsGame.sSingelton == null)
            InGameMenuOptionsGame.sSingelton = new InGameMenuOptionsGame();
        }
      }
      return InGameMenuOptionsGame.sSingelton;
    }
  }

  private InGameMenuOptionsGame()
  {
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    this.mOptions = new List<MenuTextItem>();
    this.mOptions.Add(new MenuTextItem(InGameMenu.GetSettingLoc(this.mGlobalSettings.BloodAndGore), new Vector2(), font, TextAlign.Left));
    this.AddMenuTextItem("#menu_opt_game_01".GetHashCodeCustom(), font, TextAlign.Right);
    this.mOptions.Add(new MenuTextItem(InGameMenu.GetSettingLoc(this.mGlobalSettings.DamageNumbers), new Vector2(), font, TextAlign.Left));
    this.AddMenuTextItem("#menu_opt_game_02".GetHashCodeCustom(), font, TextAlign.Right);
    this.mOptions.Add(new MenuTextItem(InGameMenu.GetSettingLoc(this.mGlobalSettings.HealthBars), new Vector2(), font, TextAlign.Left));
    this.AddMenuTextItem("#menu_opt_game_03".GetHashCodeCustom(), font, TextAlign.Right);
    this.mOptions.Add(new MenuTextItem(InGameMenu.GetSettingLoc(this.mGlobalSettings.SpellWheel), new Vector2(), font, TextAlign.Left));
    this.AddMenuTextItem("#menu_opt_game_04".GetHashCodeCustom(), font, TextAlign.Right);
    this.AddMenuTextItem("#menu_back".GetHashCodeCustom(), font, TextAlign.Center);
    this.mBackgroundSize = new Vector2(600f, 400f);
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    for (int index = 0; index < this.mOptions.Count; ++index)
      this.mOptions[index].LanguageChanged();
  }

  public override void UpdatePositions()
  {
    Vector2 vector2 = new Vector2();
    vector2.X = (float) ((double) InGameMenu.sScreenSize.X * 0.5 + 80.0);
    vector2.Y = 290f * InGameMenu.sScale;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      if (index == this.mMenuItems.Count - 1)
        vector2.X -= 80f;
      MenuItem mMenuItem = this.mMenuItems[index];
      mMenuItem.Scale = InGameMenu.sScale;
      mMenuItem.Position = vector2;
      vector2.Y += mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y;
    }
    for (int index = 0; index < this.mOptions.Count; ++index)
    {
      Vector2 position = this.mMenuItems[index].Position;
      position.X -= 15f * InGameMenu.sScale;
      this.mMenuItems[index].Position = position;
      position.X += 30f * InGameMenu.sScale;
      this.mOptions[index].Position = position;
      this.mOptions[index].Scale = InGameMenu.sScale;
    }
    this.mMenuItems[this.mMenuItems.Count - 1].Position += new Vector2(0.0f, 10f * InGameMenu.sScale);
  }

  protected override void IControllerSelect(Controller iSender)
  {
    GlobalSettings instance = GlobalSettings.Instance;
    switch (this.mSelectedItem)
    {
      case 0:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        switch (instance.BloodAndGore)
        {
          case SettingOptions.On:
            instance.BloodAndGore = SettingOptions.Off;
            break;
          default:
            instance.BloodAndGore = SettingOptions.On;
            break;
        }
        this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.BloodAndGore));
        break;
      case 1:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        switch (instance.DamageNumbers)
        {
          case SettingOptions.On:
            instance.DamageNumbers = SettingOptions.Off;
            break;
          default:
            instance.DamageNumbers = SettingOptions.On;
            break;
        }
        this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.DamageNumbers));
        break;
      case 2:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        switch (instance.HealthBars)
        {
          case SettingOptions.On:
            instance.HealthBars = SettingOptions.Off;
            break;
          case SettingOptions.Players_Only:
            instance.HealthBars = SettingOptions.On;
            break;
          default:
            instance.HealthBars = SettingOptions.Players_Only;
            break;
        }
        this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.HealthBars));
        break;
      case 3:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
        switch (instance.SpellWheel)
        {
          case SettingOptions.On:
            instance.SpellWheel = SettingOptions.Off;
            break;
          default:
            instance.SpellWheel = SettingOptions.On;
            break;
        }
        this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.SpellWheel));
        break;
      case 4:
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
        InGameMenu.PopMenu();
        break;
    }
  }

  protected override void IControllerBack(Controller iSender)
  {
    AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
    InGameMenu.PopMenu();
  }

  protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
  {
    base.IControllerMove(iSender, iDirection);
    GlobalSettings instance = GlobalSettings.Instance;
    switch (iDirection)
    {
      case ControllerDirection.Right:
        switch (this.mSelectedItem)
        {
          case 0:
            AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
            switch (instance.BloodAndGore)
            {
              case SettingOptions.On:
                instance.BloodAndGore = SettingOptions.Off;
                break;
              default:
                instance.BloodAndGore = SettingOptions.On;
                break;
            }
            this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.BloodAndGore));
            return;
          case 1:
            AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
            switch (instance.DamageNumbers)
            {
              case SettingOptions.On:
                instance.DamageNumbers = SettingOptions.Off;
                break;
              default:
                instance.DamageNumbers = SettingOptions.On;
                break;
            }
            this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.DamageNumbers));
            return;
          case 2:
            AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
            switch (instance.HealthBars)
            {
              case SettingOptions.On:
                instance.HealthBars = SettingOptions.Off;
                break;
              case SettingOptions.Players_Only:
                instance.HealthBars = SettingOptions.On;
                break;
              default:
                instance.HealthBars = SettingOptions.Players_Only;
                break;
            }
            this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.HealthBars));
            return;
          case 3:
            AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
            switch (instance.SpellWheel)
            {
              case SettingOptions.On:
                instance.SpellWheel = SettingOptions.Off;
                break;
              default:
                instance.SpellWheel = SettingOptions.On;
                break;
            }
            this.mOptions[this.mSelectedItem].SetText(InGameMenu.GetSettingLoc(instance.SpellWheel));
            return;
          default:
            return;
        }
      case ControllerDirection.Left:
        switch (this.mSelectedItem)
        {
          case 0:
            AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
            switch (instance.BloodAndGore)
            {
              case SettingOptions.On:
                instance.BloodAndGore = SettingOptions.Off;
                break;
              default:
                instance.BloodAndGore = SettingOptions.On;
                break;
            }
            this.mOptions[this.mSelectedItem].SetText(instance.BloodAndGore.ToString());
            return;
          case 1:
            AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
            switch (instance.DamageNumbers)
            {
              case SettingOptions.On:
                instance.DamageNumbers = SettingOptions.Off;
                break;
              default:
                instance.DamageNumbers = SettingOptions.On;
                break;
            }
            this.mOptions[this.mSelectedItem].SetText(instance.DamageNumbers.ToString());
            return;
          case 2:
            AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
            switch (instance.HealthBars)
            {
              case SettingOptions.On:
                instance.HealthBars = SettingOptions.Players_Only;
                break;
              case SettingOptions.Players_Only:
                instance.HealthBars = SettingOptions.Off;
                break;
              default:
                instance.HealthBars = SettingOptions.On;
                break;
            }
            this.mOptions[this.mSelectedItem].SetText(instance.HealthBars.ToString());
            return;
          case 3:
            AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
            switch (instance.SpellWheel)
            {
              case SettingOptions.On:
                instance.SpellWheel = SettingOptions.Off;
                break;
              default:
                instance.SpellWheel = SettingOptions.On;
                break;
            }
            this.mOptions[this.mSelectedItem].SetText(instance.SpellWheel.ToString());
            return;
          default:
            return;
        }
    }
  }

  protected override string IGetHighlightedButtonName()
  {
    return InGameMenuOptionsGame.OPTION_STRINGS[this.mSelectedItem];
  }

  protected override void OnEnter()
  {
    if (InGameMenu.sController is KeyboardMouseController)
      this.mSelectedItem = -1;
    else
      this.mSelectedItem = 0;
    this.mBloodAndGore = this.mGlobalSettings.BloodAndGore;
    this.mDamageNumbers = this.mGlobalSettings.DamageNumbers;
    this.mHealthBars = this.mGlobalSettings.HealthBars;
    this.mSpellWheel = this.mGlobalSettings.SpellWheel;
  }

  protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
  {
    base.IDraw(iDeltaTime, ref iBackgroundSize);
    Vector4 color = this.mMenuItems[0].Color;
    Vector4 colorSelected = this.mMenuItems[0].ColorSelected;
    Vector4 colorDisabled = this.mMenuItems[0].ColorDisabled;
    for (int index = 0; index < this.mOptions.Count; ++index)
    {
      MenuItem mOption = (MenuItem) this.mOptions[index];
      mOption.Color = color;
      mOption.ColorSelected = colorSelected;
      mOption.ColorDisabled = colorDisabled;
      mOption.Selected = this.mMenuItems[index].Selected;
      mOption.Enabled = this.mMenuItems[index].Enabled;
      mOption.Draw(InGameMenu.sEffect);
    }
  }

  protected override void OnExit()
  {
    if (!(this.mBloodAndGore != this.mGlobalSettings.BloodAndGore | this.mDamageNumbers != this.mGlobalSettings.DamageNumbers | this.mHealthBars != this.mGlobalSettings.HealthBars | this.mSpellWheel != this.mGlobalSettings.SpellWheel))
      return;
    SaveManager.Instance.SaveSettings();
  }
}
