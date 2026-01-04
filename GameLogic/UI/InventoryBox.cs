// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.InventoryBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

public class InventoryBox
{
  private Rectangle mBox;
  private float mScale;
  private bool mShow;
  private Text mText;
  private Magicka.GameLogic.Entities.Character mOwner;
  private int mWeaponID;
  private int mStaffID;
  private string mWeaponName;
  private string mWeaponDesc;
  private string mStaffName;
  private string mStaffDesc;
  private InventoryBox.RenderData[] mRenderData;
  private Vector3 mWorldPosition;

  public InventoryBox()
  {
    this.mShow = false;
    IndexBuffer iIndices;
    VertexBuffer iVertices;
    VertexDeclaration iDeclaration;
    TextBoxEffect textBoxEffect;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      iIndices = new IndexBuffer(Magicka.Game.Instance.GraphicsDevice, 108, BufferUsage.None, IndexElementSize.SixteenBits);
      iVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 16 /*0x10*/ * VertexPositionNormalTexture.SizeInBytes, BufferUsage.None);
      iDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
      iIndices.SetData<ushort>(TextBox.INDICES);
      iVertices.SetData<VertexPositionNormalTexture>(TextBox.VERTICES);
      textBoxEffect = new TextBoxEffect(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
    }
    Point screenSize = RenderManager.Instance.ScreenSize;
    textBoxEffect.BorderSize = 32f;
    textBoxEffect.Texture = (Texture) Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/Dialog_Say");
    textBoxEffect.ScreenSize = new Vector2((float) screenSize.X, (float) screenSize.Y);
    GUIBasicEffect guiBasicEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    guiBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
    guiBasicEffect.VertexColorEnabled = true;
    guiBasicEffect.TextureEnabled = true;
    guiBasicEffect.Color = Vector4.One;
    this.mText = new Text(1024 /*0x0400*/, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Left, false);
    this.mText.DefaultColor = Defines.DIALOGUE_COLOR_DEFAULT;
    this.mText.SetText("");
    Texture2D iImageTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks");
    Vector2 vector2_1 = new Vector2(880f / (float) iImageTexture.Width, 1260f / (float) iImageTexture.Height);
    Vector2 vector2_2 = new Vector2(250f / (float) iImageTexture.Width, 230f / (float) iImageTexture.Height);
    VertexPositionColorTexture[] data = new VertexPositionColorTexture[4];
    data[0].TextureCoordinate = new Vector2(vector2_1.X + vector2_2.X, vector2_1.Y + vector2_2.Y);
    data[0].Position = new Vector3(250f, 230f, 0.0f);
    data[0].Color = Color.White;
    data[1].TextureCoordinate = new Vector2(vector2_1.X, vector2_1.Y + vector2_2.Y);
    data[1].Position = new Vector3(0.0f, 230f, 0.0f);
    data[1].Color = Color.White;
    data[2].TextureCoordinate = new Vector2(vector2_1.X, vector2_1.Y);
    data[2].Position = new Vector3(0.0f, 0.0f, 0.0f);
    data[2].Color = Color.White;
    data[3].TextureCoordinate = new Vector2(vector2_1.X + vector2_2.X, vector2_1.Y);
    data[3].Position = new Vector3(250f, 0.0f, 0.0f);
    data[3].Color = Color.White;
    VertexBuffer iImageVertices;
    VertexDeclaration iImageDeclaration;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      iImageVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 4 * VertexPositionColorTexture.SizeInBytes, BufferUsage.WriteOnly);
      iImageVertices.SetData<VertexPositionColorTexture>(data);
      iImageDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionColorTexture.VertexElements);
    }
    this.mRenderData = new InventoryBox.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new InventoryBox.RenderData(iVertices, iIndices, iDeclaration, iImageVertices, iImageDeclaration, iImageTexture);
      this.mRenderData[index].mTextBoxEffect = textBoxEffect;
      this.mRenderData[index].mGUIBasicEffect = guiBasicEffect;
      this.mRenderData[index].mText = this.mText;
      this.mRenderData[index].mTextOffset = 0.0f;
    }
  }

  public void ShowInventory(Item iWeapon, Item iStaff, Magicka.GameLogic.Entities.Character iOwner)
  {
    this.mOwner = iOwner;
    this.mShow = true;
    if (this.mWeaponID == iWeapon.DisplayName & this.mStaffID == iStaff.DisplayName)
      return;
    this.mWeaponID = iWeapon.DisplayName;
    this.mWeaponName = iWeapon.DisplayName != 0 ? LanguageManager.Instance.GetString(iWeapon.DisplayName) : "";
    if (iWeapon.Description == 0)
    {
      this.mWeaponDesc = "";
    }
    else
    {
      this.mWeaponDesc = LanguageManager.Instance.GetString(iWeapon.Description);
      this.mWeaponDesc = LanguageManager.Instance.ParseReferences(this.mWeaponDesc);
    }
    this.mStaffID = iStaff.DisplayName;
    this.mStaffName = iStaff.DisplayName != 0 ? LanguageManager.Instance.GetString(iStaff.DisplayName) : "";
    if (iStaff.Description == 0)
    {
      this.mStaffDesc = "";
    }
    else
    {
      this.mStaffDesc = LanguageManager.Instance.GetString(iStaff.Description);
      this.mStaffDesc = LanguageManager.Instance.ParseReferences(this.mStaffDesc);
    }
    this.mBox.Width = 600;
    this.mText.Characters[0] = '[';
    this.mText.Characters[1] = 'c';
    this.mText.Characters[2] = '=';
    this.mText.Characters[3] = '1';
    this.mText.Characters[4] = ',';
    this.mText.Characters[5] = '1';
    this.mText.Characters[6] = ',';
    this.mText.Characters[7] = '1';
    this.mText.Characters[8] = ']';
    int num1 = 9;
    char[] charArray1 = this.mWeaponName.ToCharArray();
    for (int index = 0; index < charArray1.Length; ++index)
      this.mText.Characters[num1 + index] = charArray1[index];
    this.mText.Characters[num1 + charArray1.Length] = '[';
    this.mText.Characters[num1 + charArray1.Length + 1] = '/';
    this.mText.Characters[num1 + charArray1.Length + 2] = 'c';
    this.mText.Characters[num1 + charArray1.Length + 3] = ']';
    this.mText.Characters[num1 + charArray1.Length + 4] = '\n';
    this.mWeaponDesc = this.mText.Font.Wrap(this.mWeaponDesc, 350, true);
    int num2 = num1 + (charArray1.Length + 4 + 1);
    char[] charArray2 = this.mWeaponDesc.ToCharArray();
    for (int index = 0; index < charArray2.Length; ++index)
      this.mText.Characters[num2 + index] = charArray2[index];
    int index1 = num2 + charArray2.Length;
    this.mText.Characters[index1] = '\n';
    this.mText.Characters[index1 + 1] = '\n';
    int index2 = index1 + 2;
    this.mText.Characters[index2] = '[';
    this.mText.Characters[index2 + 1] = 'c';
    this.mText.Characters[index2 + 2] = '=';
    this.mText.Characters[index2 + 3] = '1';
    this.mText.Characters[index2 + 4] = ',';
    this.mText.Characters[index2 + 5] = '1';
    this.mText.Characters[index2 + 6] = ',';
    this.mText.Characters[index2 + 7] = '1';
    this.mText.Characters[index2 + 8] = ']';
    int num3 = index2 + 9;
    char[] charArray3 = this.mStaffName.ToCharArray();
    for (int index3 = 0; index3 < charArray3.Length; ++index3)
      this.mText.Characters[num3 + index3] = charArray3[index3];
    int index4 = num3 + charArray3.Length;
    this.mText.Characters[index4] = '[';
    this.mText.Characters[index4 + 1] = '/';
    this.mText.Characters[index4 + 2] = 'c';
    this.mText.Characters[index4 + 3] = ']';
    this.mText.Characters[index4 + 4] = '\n';
    this.mStaffDesc = this.mText.Font.Wrap(this.mStaffDesc, 350, true);
    int num4 = index4 + 5;
    char[] charArray4 = this.mStaffDesc.ToCharArray();
    for (int index5 = 0; index5 < charArray4.Length; ++index5)
      this.mText.Characters[num4 + index5] = charArray4[index5];
    this.mText.Characters[num4 + charArray4.Length] = char.MinValue;
    this.mText.MarkAsDirty();
    this.mBox.Height = (int) Math.Max(this.mText.Font.MeasureText(this.mText.Characters, true).Y, 230f);
  }

  public bool Active => this.mShow | (double) this.mScale > 0.0;

  public void SetPosition(Vector3 iPosition) => this.mWorldPosition = iPosition;

  public Magicka.GameLogic.Entities.Character Owner => this.mOwner;

  public void Close(Magicka.GameLogic.Entities.Character iOwner)
  {
    if (!(iOwner == this.mOwner | iOwner == null))
      return;
    this.mShow = false;
    this.mOwner = (Magicka.GameLogic.Entities.Character) null;
  }

  public void Update(float iDeltaTime, DataChannel iDataChannel)
  {
    if (this.mShow)
    {
      this.mScale = Math.Min(this.mScale + iDeltaTime * 4f, 1f);
    }
    else
    {
      if ((double) this.mScale <= 0.0)
        return;
      this.mScale = Math.Max(this.mScale - iDeltaTime * 4f, 0.0f);
    }
    this.mRenderData[(int) iDataChannel].mScale = this.mScale;
    this.mRenderData[(int) iDataChannel].mSize.X = (float) this.mBox.Width;
    this.mRenderData[(int) iDataChannel].mSize.Y = (float) this.mBox.Height;
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) this.mRenderData[(int) iDataChannel]);
  }

  protected class RenderData : IRenderableGUIObject, IPreRenderRenderer
  {
    public TextBoxEffect mTextBoxEffect;
    public GUIBasicEffect mGUIBasicEffect;
    protected VertexBuffer mVertexBuffer;
    protected VertexDeclaration mVertexDeclaration;
    protected IndexBuffer mIndexBuffer;
    protected Texture2D mImageTexture;
    protected VertexBuffer mImageVertexBuffer;
    protected VertexDeclaration mImageDeclaration;
    public Vector3 mWorldPosition;
    private Vector2 mPosition;
    public bool mShowName;
    public float mTextOffset;
    public Text mText;
    public Vector2 mSize;
    public float mScale;

    public RenderData(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      VertexDeclaration iDeclaration,
      VertexBuffer iImageVertices,
      VertexDeclaration iImageDeclaration,
      Texture2D iImageTexture)
    {
      this.mVertexBuffer = iVertices;
      this.mIndexBuffer = iIndices;
      this.mVertexDeclaration = iDeclaration;
      this.mImageTexture = iImageTexture;
      this.mImageVertexBuffer = iImageVertices;
      this.mImageDeclaration = iImageDeclaration;
    }

    public void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      this.mPosition.X = (float) screenSize.X * 0.5f;
      this.mPosition.Y = (float) screenSize.Y * 0.5f;
      this.mTextBoxEffect.Color = Vector4.One;
      this.mTextBoxEffect.Position = this.mPosition;
      this.mTextBoxEffect.Size = this.mSize;
      this.mTextBoxEffect.Scale = this.mScale;
      this.mTextBoxEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
      this.mTextBoxEffect.GraphicsDevice.Indices = this.mIndexBuffer;
      this.mTextBoxEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mTextBoxEffect.Begin();
      this.mTextBoxEffect.CurrentTechnique.Passes[0].Begin();
      this.mTextBoxEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, 18);
      this.mTextBoxEffect.CurrentTechnique.Passes[0].End();
      this.mTextBoxEffect.End();
      this.mGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
      this.mGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(this.mImageVertexBuffer, 0, VertexPositionColorTexture.SizeInBytes);
      this.mGUIBasicEffect.GraphicsDevice.VertexDeclaration = this.mImageDeclaration;
      this.mGUIBasicEffect.Texture = (Texture) this.mImageTexture;
      this.mGUIBasicEffect.TextureEnabled = true;
      this.mGUIBasicEffect.Transform = Matrix.Identity with
      {
        M11 = this.mScale,
        M22 = this.mScale,
        M41 = this.mPosition.X - this.mSize.X * 0.5f * this.mScale,
        M42 = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale
      };
      this.mGUIBasicEffect.Begin();
      this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
      this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      this.mText.Draw(this.mGUIBasicEffect, this.mPosition.X - (float) ((double) this.mSize.X * 0.5 - 250.0) * this.mScale, this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale, this.mScale);
      this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
      this.mGUIBasicEffect.End();
    }

    public int ZIndex => 150;

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      Vector2 vector2 = new Vector2();
      vector2.X = (float) screenSize.X;
      vector2.Y = (float) screenSize.Y;
      this.mTextBoxEffect.ScreenSize = vector2;
      Vector4 result;
      Vector4.Transform(ref this.mWorldPosition, ref iViewProjectionMatrix, out result);
      this.mPosition.X = (float) ((double) result.X / (double) result.W * 0.5 + 0.5) * vector2.X;
      this.mPosition.X -= (float) (((double) this.mSize.X * 0.5 + 64.0) / 3.0) * this.mScale;
      this.mPosition.Y = (float) ((double) result.Y / (double) result.W * -0.5 + 0.5) * vector2.Y;
      this.mPosition.Y -= (float) ((double) this.mSize.Y * 0.5 + 64.0) * this.mScale;
      if ((double) this.mScale <= 0.99900001287460327)
        return;
      this.mPosition.X = (float) Math.Floor((double) this.mPosition.X);
      if ((double) this.mSize.X % 2.0 > 0.5)
        this.mPosition.X += 0.5f;
      this.mPosition.Y = (float) Math.Floor((double) this.mPosition.Y);
      if ((double) this.mSize.Y % 2.0 <= 0.5)
        return;
      this.mPosition.Y += 0.5f;
    }
  }
}
