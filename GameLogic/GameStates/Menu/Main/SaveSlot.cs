// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SaveSlot
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

public class SaveSlot : MenuItem
{
  private const float BORDERWIDTH = 1f;
  public const float SLOTWIDTH = 768f;
  public const float SLOTHEIGHT = 256f;
  private static VertexBuffer sVertices;
  private static VertexDeclaration sVertexDeclaration;
  private static Texture2D sTexture;
  private Text mName;
  private int mNameHash;
  private Text mTitle;
  private int mTitleHash;
  private Text mTime;
  private Text mUnlockedMagicks;
  private bool mLooped;
  private bool mEmptySlot;
  private float mLineHeight;
  public static readonly Vector2 sBackgroundSize = new Vector2(784f, 240f);
  private static readonly Vector2 sLoopedSize = new Vector2(64f, 64f);
  private static readonly Vector2 sLoopedOffset = new Vector2(23f / 32f, 3f / 32f);
  private static readonly Vector2 sTimeSize = new Vector2(64f, 64f);
  private static readonly Vector2 sTimeOffset = new Vector2(11f / 16f, 3f / 32f);
  private static readonly Vector2 sMagickSize = new Vector2(64f, 64f);
  private static readonly Vector2 sMagickOffset = new Vector2(21f / 32f, 3f / 32f);
  private bool mMythos;

  public SaveSlot(
    Vector2 iPos,
    int iName,
    int iDesc,
    int iCurrentTime,
    bool iLooped,
    ulong iMagicks,
    bool iMythos)
  {
    this.mPosition = iPos;
    this.mMythos = iMythos;
    this.mEmptySlot = iName == SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP;
    this.mNameHash = iName;
    this.mTitleHash = iDesc;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    this.mLineHeight = (float) font.LineHeight;
    this.UpdateBoundingBox();
    this.mName = new Text(64 /*0x40*/, font, TextAlign.Left, false);
    this.mTitle = new Text(64 /*0x40*/, font, TextAlign.Left, false);
    if (this.mEmptySlot)
    {
      this.mName.SetText(LanguageManager.Instance.GetString(iName));
    }
    else
    {
      this.mName.SetText(LanguageManager.Instance.GetString(iName) + ": ");
      this.mTitle.SetText(LanguageManager.Instance.GetString(iDesc));
    }
    this.mTime = new Text(8, font, TextAlign.Left, false);
    this.SetTimeText(this.mTime, iCurrentTime);
    this.mUnlockedMagicks = new Text(6, font, TextAlign.Left, false);
    this.mLooped = iLooped;
    if (SaveSlot.sVertices != null)
      return;
    SaveSlot.sTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
    SaveSlot.sVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * Defines.QUAD_TEX_VERTS_TL.Length, BufferUsage.None);
    SaveSlot.sVertices.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_TL);
    SaveSlot.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
  }

  private void SetTimeText(Text iText, int iTime)
  {
    if (iTime >= 359999)
    {
      iText.Characters[0] = '9';
      iText.Characters[1] = '9';
      iText.Characters[2] = ':';
      iText.Characters[3] = '5';
      iText.Characters[4] = '9';
      iText.Characters[5] = ':';
      iText.Characters[6] = '5';
      iText.Characters[7] = '9';
    }
    else
    {
      int index1 = 7;
      for (int index2 = 0; index2 < 3; ++index2)
      {
        int num = iTime % 60;
        iTime /= 60;
        iText.Characters[index1] = (char) (48 /*0x30*/ + num % 10);
        int index3 = index1 - 1;
        iText.Characters[index3] = (char) (48 /*0x30*/ + num / 10);
        index1 = index3 - 1;
        if (index1 > 0)
        {
          iText.Characters[index1] = '.';
          --index1;
        }
      }
    }
    iText.MarkAsDirty();
  }

  public static void GetPlayTime(char[] iText, int iIndex, int iPlayTime)
  {
    if (iPlayTime >= 362439)
    {
      iText[iIndex] = '9';
      iText[iIndex + 1] = '9';
      iText[iIndex + 2] = ':';
      iText[iIndex + 3] = '9';
      iText[iIndex + 4] = '9';
      iText[iIndex + 5] = ':';
      iText[iIndex + 6] = '9';
      iText[iIndex + 7] = '9';
    }
    else
    {
      int index1 = iIndex + 7;
      for (int index2 = 0; index2 < 3; ++index2)
      {
        int num = iPlayTime % 60;
        iPlayTime /= 60;
        iText[index1] = (char) (48 /*0x30*/ + num % 10);
        int index3 = index1 - 1;
        iText[index3] = (char) (48 /*0x30*/ + num / 10);
        index1 = index3 - 1;
        if (index1 > 0)
        {
          iText[index1] = ':';
          --index1;
        }
      }
    }
  }

  public static void GetPlayTime(out string oText, int iPlayTime)
  {
    if (iPlayTime >= 362439)
      oText = "99:99:99";
    else
      oText = $"{iPlayTime / 3600 % 60:D2}:{iPlayTime / 60 % 60:D2}:{iPlayTime % 60:D2}";
  }

  public void Set(int iName, int iDesc, int iTime, bool iLooped, ulong iMagicks, bool iMythos)
  {
    SaveSlot.GetPlayTime(this.mTime.Characters, 0, iTime);
    this.mTime.MarkAsDirty();
    this.mMythos = iMythos;
    this.mEmptySlot = iName == SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP;
    this.mNameHash = iName;
    if (this.mEmptySlot)
    {
      this.mName.SetText(LanguageManager.Instance.GetString(iName));
    }
    else
    {
      this.mName.SetText(LanguageManager.Instance.GetString(iName) + ": ");
      this.mTitle.SetText(LanguageManager.Instance.GetString(iDesc));
      this.mTitleHash = iDesc;
    }
    this.mLooped = iLooped;
    if (this.mMythos)
    {
      int num = Helper.CountSetBits(1040187402UL /*0x3E00000A*/);
      this.mUnlockedMagicks.SetText($"{(object) Helper.CountSetBits(iMagicks)}/{(object) num}");
    }
    else
    {
      int num = Helper.CountSetBits(8384510UL);
      this.mUnlockedMagicks.SetText($"{(object) Helper.CountSetBits(iMagicks)}/{(object) num}");
    }
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, 1f);

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    iEffect.VertexColorEnabled = false;
    iEffect.GraphicsDevice.Vertices[0].SetSource(SaveSlot.sVertices, 0, VertexPositionTexture.SizeInBytes);
    iEffect.GraphicsDevice.VertexDeclaration = SaveSlot.sVertexDeclaration;
    iEffect.TextureEnabled = true;
    iEffect.TextureOffset = Vector2.Zero;
    iEffect.TextureScale = new Vector2(SaveSlot.sBackgroundSize.X / (float) SaveSlot.sTexture.Width, SaveSlot.sBackgroundSize.Y / (float) SaveSlot.sTexture.Height);
    iEffect.Texture = (Texture) SaveSlot.sTexture;
    iEffect.Color = this.mSelected ? new Vector4(1.1f) : Vector4.One;
    Matrix identity = Matrix.Identity with
    {
      M11 = SaveSlot.sBackgroundSize.X,
      M22 = SaveSlot.sBackgroundSize.Y,
      M41 = this.mPosition.X - 384f,
      M42 = this.mPosition.Y - 128f
    };
    iEffect.Saturation = this.mEmptySlot ? 0.0f : (this.mSelected ? 1.2f : 1f);
    iEffect.Transform = identity;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    if (!this.mEmptySlot)
    {
      if (this.mLooped)
      {
        iEffect.TextureOffset = SaveSlot.sLoopedOffset;
        iEffect.TextureScale = new Vector2(SaveSlot.sLoopedSize.X / (float) SaveSlot.sTexture.Width, SaveSlot.sLoopedSize.Y / (float) SaveSlot.sTexture.Height);
        identity.M11 = SaveSlot.sLoopedSize.X;
        identity.M22 = SaveSlot.sLoopedSize.Y;
        identity.M41 = this.mPosition.X + 256f;
        identity.M42 = (float) ((double) this.mPosition.Y - 32.0 - 4.0);
        iEffect.Transform = identity;
        iEffect.Saturation = this.mLooped ? 1f : 0.0f;
        iEffect.CommitChanges();
        iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
        iEffect.Saturation = 1f;
      }
      iEffect.TextureOffset = SaveSlot.sMagickOffset;
      iEffect.TextureScale = new Vector2(SaveSlot.sMagickSize.X / (float) SaveSlot.sTexture.Width, SaveSlot.sMagickSize.Y / (float) SaveSlot.sTexture.Height);
      identity.M11 = SaveSlot.sMagickSize.X;
      identity.M22 = SaveSlot.sMagickSize.Y;
      identity.M41 = (float) ((double) this.mPosition.X - 256.0 - 32.0);
      identity.M42 = (float) ((double) this.mPosition.Y - 32.0 + 64.0 - 10.0);
      iEffect.Transform = identity;
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      iEffect.TextureOffset = SaveSlot.sTimeOffset;
      iEffect.TextureScale = new Vector2(SaveSlot.sTimeSize.X / (float) SaveSlot.sTexture.Width, SaveSlot.sTimeSize.Y / (float) SaveSlot.sTexture.Height);
      identity.M11 = SaveSlot.sTimeSize.X;
      identity.M22 = SaveSlot.sTimeSize.Y;
      identity.M41 += 192f;
      iEffect.Transform = identity;
      iEffect.Saturation = this.mLooped ? 1f : 0.0f;
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      iEffect.Color = this.mSelected ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
      this.mUnlockedMagicks.Draw(iEffect, (float) ((double) this.mPosition.X - 256.0 + 64.0 - 16.0), (float) ((double) this.mPosition.Y + 48.0 - 8.0));
      this.mTime.Draw(iEffect, (float) ((double) this.mPosition.X - 256.0 + 64.0 + 128.0 + 48.0 - 16.0), (float) ((double) this.mPosition.Y + 48.0 - 8.0));
      this.mTitle.Draw(iEffect, (float) ((double) this.Position.X - 256.0 - 32.0 + 64.0), (float) ((double) this.mPosition.Y - 8.0 - 4.0));
    }
    else
      iEffect.Color = this.mSelected ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
    this.mName.Draw(iEffect, (float) ((double) this.mPosition.X - 256.0 - 32.0), (float) ((double) this.mPosition.Y - 48.0 - 4.0));
  }

  protected override void UpdateBoundingBox()
  {
    this.mTopLeft.X = this.mPosition.X - 384f;
    this.mTopLeft.Y = this.mPosition.Y - 128f;
    this.mBottomRight.X = this.mPosition.X + 384f;
    this.mBottomRight.Y = this.mPosition.Y + 128f;
  }

  public override void LanguageChanged()
  {
    if (this.mEmptySlot)
    {
      this.mName.SetText(LanguageManager.Instance.GetString(this.mNameHash));
    }
    else
    {
      this.mName.SetText(LanguageManager.Instance.GetString(this.mNameHash) + ": ");
      this.mTitle.SetText(LanguageManager.Instance.GetString(this.mTitleHash));
    }
  }

  internal bool EmptySlot => this.mEmptySlot;
}
