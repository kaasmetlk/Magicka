// Decompiled with JetBrains decompiler
// Type: Magicka.Achievements.PdxWidgetWindow
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

#nullable disable
namespace Magicka.Achievements;

internal abstract class PdxWidgetWindow : AchievementWindow
{
  protected static Texture2D sTexture;
  protected static Vector2 sInvTextureSize;
  protected static Rectangle[] sRectangles;

  protected PdxWidgetWindow() => PdxWidgetWindow.EnsureInitialized();

  private static void EnsureInitialized()
  {
    if (PdxWidgetWindow.sTexture != null)
      return;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
      PdxWidgetWindow.sTexture = Texture2D.FromFile(graphicsDevice, "content/connectui/widget.png");
    PdxWidgetWindow.sInvTextureSize.X = 1f / (float) PdxWidgetWindow.sTexture.Width;
    PdxWidgetWindow.sInvTextureSize.Y = 1f / (float) PdxWidgetWindow.sTexture.Height;
    TextReader textReader = (TextReader) File.OpenText("content/connectui/widget.txt");
    int length = int.Parse(textReader.ReadLine());
    PdxWidgetWindow.sRectangles = 43 == length ? new Rectangle[length] : throw new Exception("Invalid nr of textures in Widget.png!");
    for (int index1 = 0; index1 < length; ++index1)
    {
      string str = textReader.ReadLine();
      PdxWidgetWindow.Textures index2 = (PdxWidgetWindow.Textures) Enum.Parse(typeof (PdxWidgetWindow.Textures), str.Substring(0, str.Length - 4), true);
      PdxWidgetWindow.sRectangles[(int) index2].X = int.Parse(textReader.ReadLine());
      PdxWidgetWindow.sRectangles[(int) index2].Y = int.Parse(textReader.ReadLine());
      PdxWidgetWindow.sRectangles[(int) index2].Width = int.Parse(textReader.ReadLine());
      PdxWidgetWindow.sRectangles[(int) index2].Height = int.Parse(textReader.ReadLine());
    }
  }

  protected static void CreateVertices(
    Vector4[] iVertices,
    ref int iIndex,
    PdxWidgetWindow.Textures iTexture)
  {
    PdxWidgetWindow.EnsureInitialized();
    PdxWidgetWindow.CreateVertices(iVertices, ref iIndex, ref PdxWidgetWindow.sRectangles[(int) iTexture]);
  }

  public static void CreateVertices(Vector4[] iVertices, ref int iIndex, ref Rectangle iSourceRect)
  {
    PdxWidgetWindow.EnsureInitialized();
    iVertices[iIndex].X = 0.0f;
    iVertices[iIndex].Y = 0.0f;
    iVertices[iIndex].Z = (float) iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
    iVertices[iIndex].W = (float) iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
    ++iIndex;
    iVertices[iIndex].X = (float) iSourceRect.Width;
    iVertices[iIndex].Y = 0.0f;
    iVertices[iIndex].Z = (float) (iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
    iVertices[iIndex].W = (float) iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
    ++iIndex;
    iVertices[iIndex].X = (float) iSourceRect.Width;
    iVertices[iIndex].Y = (float) iSourceRect.Height;
    iVertices[iIndex].Z = (float) (iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
    iVertices[iIndex].W = (float) (iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
    ++iIndex;
    iVertices[iIndex].X = 0.0f;
    iVertices[iIndex].Y = (float) iSourceRect.Height;
    iVertices[iIndex].Z = (float) iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
    iVertices[iIndex].W = (float) (iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
    ++iIndex;
  }

  public static void GetTexture(
    PdxWidgetWindow.Textures iTexture,
    out Texture2D oTexture,
    out Rectangle oRect)
  {
    PdxWidgetWindow.EnsureInitialized();
    oTexture = PdxWidgetWindow.sTexture;
    oRect = PdxWidgetWindow.sRectangles[(int) iTexture];
  }

  public abstract void OnMouseDown(ref Vector2 iMousePos);

  public abstract void OnMouseUp(ref Vector2 iMousePos);

  public abstract void OnMouseMove(ref Vector2 iMousePos);

  public abstract void OnKeyDown(KeyData iData);

  public abstract void OnKeyPress(char iChar);

  public enum Textures
  {
    frame_profilebox_middle,
    frame_profilebox_top,
    frame_profilebox_bottom,
    popup_info,
    progress_indicator_off,
    progress_indicator_on,
    popup_input_error,
    popup_input_info,
    badge_game,
    frame_points,
    badge_achievement_on,
    badge_achievement_off,
    textfield_focus,
    textfield_notfocus,
    menu_highlight_right,
    menu_right,
    menu_highlight_left,
    menu_left,
    button_big_empty_off,
    button_big_empty_on,
    button_big_empty_off_d,
    button_big_empty_on_d,
    button_arrow_left_off,
    button_arrow_left_on,
    button_arrow_right_off,
    button_arrow_right_on,
    button_arrow_left_off_d,
    button_arrow_left_on_d,
    button_arrow_right_off_d,
    button_arrow_right_on_d,
    points_number_0,
    points_number_1,
    points_number_2,
    points_number_3,
    points_number_4,
    points_number_5,
    points_number_6,
    points_number_7,
    points_number_8,
    points_number_9,
    button_empty_off,
    button_empty_on,
    textfield_cursor,
    NR_OF_TEXTURES,
  }
}
