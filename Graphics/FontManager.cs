// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.FontManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Localization;
using Microsoft.Xna.Framework.Content;
using PolygonHead;
using System.IO;

#nullable disable
namespace Magicka.Graphics;

internal class FontManager
{
  private ContentManager mContent;
  private BitmapFont[] mFonts;
  private static FontManager mSingelton;
  private static volatile object mSingeltonLock = new object();

  public static FontManager Instance
  {
    get
    {
      if (FontManager.mSingelton == null)
      {
        lock (FontManager.mSingeltonLock)
        {
          if (FontManager.mSingelton == null)
            FontManager.mSingelton = new FontManager();
        }
      }
      return FontManager.mSingelton;
    }
  }

  private FontManager()
  {
    this.mContent = new ContentManager(Game.Instance.Content.ServiceProvider, "content");
    this.mFonts = new BitmapFont[16 /*0x10*/];
    this.mFonts[0] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Maiandra14");
    this.mFonts[1] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Maiandra16");
    this.mFonts[1].Spacing = 1;
    this.mFonts[2] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Maiandra18");
    this.mFonts[2].Spacing = 1;
    this.mFonts[3] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Stonecross28");
    this.mFonts[3].Spacing = 1;
    this.mFonts[4] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Stonecross36");
    this.mFonts[4].Spacing = 1;
    this.mFonts[5] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Stonecross50");
    this.mFonts[5].Spacing = 1;
    this.mFonts[9] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Versustext");
    this.mFonts[9].Spacing = 1;
    this.mFonts[6] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/MenuDefault");
    this.mFonts[6].Spacing = 1;
    this.mFonts[7] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/MenuOption");
    this.mFonts[8] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/MenuTitle");
    this.mFonts[10] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXUIFont");
    this.mFonts[10].Spacing = 1;
    this.mFonts[11] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXUISmallFont");
    this.mFonts[11].Spacing = 1;
    this.mFonts[12] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXUISmallHeaderFont");
    this.mFonts[13] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXUIBoldFont");
    this.mFonts[13].Spacing = 1;
    this.mFonts[14] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXPointsFont");
    this.mFonts[15] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXEditFont");
  }

  public void LoadFonts(Language iLanguage)
  {
    if (Directory.Exists($"content/Languages/{iLanguage.ToString()}/font"))
    {
      lock (Game.Instance.GraphicsDevice)
      {
        this.mContent.Unload();
        this.mFonts[0].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/Maiandra14"));
        this.mFonts[1].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/Maiandra16"));
        this.mFonts[1].Spacing = 1;
        this.mFonts[2].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/Maiandra18"));
        this.mFonts[2].Spacing = 1;
        this.mFonts[3].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/Stonecross28"));
        this.mFonts[3].Spacing = 1;
        this.mFonts[4].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/Stonecross36"));
        this.mFonts[4].Spacing = 1;
        this.mFonts[5].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/Stonecross50"));
        this.mFonts[5].Spacing = 1;
        this.mFonts[9].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/Versustext"));
        this.mFonts[9].Spacing = 1;
        this.mFonts[6].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/MenuDefault"));
        this.mFonts[6].Spacing = 1;
        this.mFonts[7].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/MenuOption"));
        this.mFonts[8].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/MenuTitle"));
        this.mFonts[10].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/PDXUIFont"));
        this.mFonts[10].Spacing = 1;
        this.mFonts[11].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/PDXUISmallFont"));
        this.mFonts[11].Spacing = 1;
        this.mFonts[12].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/PDXUISmallHeaderFont"));
        this.mFonts[13].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/PDXUIBoldFont"));
        this.mFonts[13].Spacing = 1;
        this.mFonts[14].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/PDXPointsFont"));
        this.mFonts[15].Read(this.mContent.Load<BitmapFont>($"Languages/{iLanguage.ToString()}/Font/PDXEditFont"));
      }
    }
    else
      this.LoadFonts(Language.eng);
  }

  public BitmapFont GetFont(MagickaFont iFont) => this.mFonts[(int) iFont];
}
