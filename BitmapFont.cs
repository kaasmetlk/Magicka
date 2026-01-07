// Decompiled with JetBrains decompiler
// Type: PolygonHead.BitmapFont
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace PolygonHead;

public class BitmapFont
{
  private Texture2D mTexture;
  private Dictionary<char, Glyph> mGlyphs;
  private Dictionary<char, Character> mCharacters;
  private Dictionary<char, Dictionary<char, int>> mKerning;
  private int mBaseLine;
  private int mLineHeight;
  private char mDefaultCharacter;
  private int mSpacing;

  internal BitmapFont()
  {
  }

  internal void Read(ContentReader iInput)
  {
    this.mSpacing = 0;
    this.mLineHeight = iInput.ReadInt32();
    this.mBaseLine = iInput.ReadInt32();
    this.mDefaultCharacter = iInput.ReadChar();
    GraphicsDevice graphicsDevice = (GraphicsDevice) null;
    do
    {
      try
      {
        graphicsDevice = (iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice;
      }
      catch
      {
        Thread.Sleep(1);
      }
    }
    while (graphicsDevice == null);
    lock (graphicsDevice)
      this.mTexture = iInput.ReadObject<Texture2D>();
    int capacity = iInput.ReadInt32();
    this.mGlyphs = new Dictionary<char, Glyph>(capacity);
    this.mCharacters = new Dictionary<char, Character>(capacity);
    for (int index = 0; index < capacity; ++index)
    {
      Glyph iGlyph = Glyph.Read(iInput);
      this.mGlyphs.Add(iGlyph.Character, iGlyph);
      Character character = Character.FromGlyph(iGlyph, Vector4.One, this.mTexture.Width, (float) this.mTexture.Height);
      this.mCharacters.Add(iGlyph.Character, character);
    }
    int num1 = iInput.ReadInt32();
    this.mKerning = new Dictionary<char, Dictionary<char, int>>();
    for (int index = 0; index < num1; ++index)
    {
      char key1 = iInput.ReadChar();
      char key2 = iInput.ReadChar();
      int num2 = iInput.ReadInt32();
      Dictionary<char, int> dictionary;
      if (!this.mKerning.TryGetValue(key1, out dictionary))
      {
        dictionary = new Dictionary<char, int>();
        this.mKerning.Add(key1, dictionary);
      }
      dictionary.Add(key2, num2);
    }
  }

  public void Read(BitmapFont iBitmapFont)
  {
    this.mSpacing = 0;
    this.mLineHeight = iBitmapFont.mLineHeight;
    this.mBaseLine = iBitmapFont.mBaseLine;
    this.mDefaultCharacter = iBitmapFont.mDefaultCharacter;
    this.mTexture = iBitmapFont.mTexture;
    this.mGlyphs.Clear();
    foreach (KeyValuePair<char, Glyph> mGlyph in iBitmapFont.mGlyphs)
      this.mGlyphs.Add(mGlyph.Key, mGlyph.Value);
    this.mCharacters.Clear();
    foreach (KeyValuePair<char, Character> mCharacter in iBitmapFont.mCharacters)
      this.mCharacters.Add(mCharacter.Key, mCharacter.Value);
    this.mKerning.Clear();
    foreach (KeyValuePair<char, Dictionary<char, int>> keyValuePair1 in iBitmapFont.mKerning)
    {
      Dictionary<char, int> dictionary = new Dictionary<char, int>();
      foreach (KeyValuePair<char, int> keyValuePair2 in keyValuePair1.Value)
        dictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
      this.mKerning.Add(keyValuePair1.Key, dictionary);
    }
  }

  public Texture2D Texture => this.mTexture;

  public int Baseline => this.mBaseLine;

  public int Spacing
  {
    get => this.mSpacing;
    set => this.mSpacing = value;
  }

  public int LineHeight => this.mLineHeight;

  public bool GetCharacter(char iChar, out Character oChar)
  {
    if (this.mCharacters.TryGetValue(iChar, out oChar))
      return true;
    if (!this.mCharacters.TryGetValue(this.mDefaultCharacter, out oChar))
    {
      Dictionary<char, Character>.ValueCollection.Enumerator enumerator = this.mCharacters.Values.GetEnumerator();
      enumerator.MoveNext();
      oChar = enumerator.Current;
    }
    return false;
  }

  public bool GetGlyph(char iChar, out Glyph oGlyph)
  {
    if (this.mGlyphs.TryGetValue(iChar, out oGlyph))
      return true;
    if (!this.mGlyphs.TryGetValue(this.mDefaultCharacter, out oGlyph))
    {
      Dictionary<char, Glyph>.ValueCollection.Enumerator enumerator = this.mGlyphs.Values.GetEnumerator();
      enumerator.MoveNext();
      oGlyph = enumerator.Current;
    }
    return false;
  }

  public string Wrap(string iText, int iTargetLineWidth, bool iKern)
  {
    return this.Wrap(iText, iTargetLineWidth, iKern, out iTargetLineWidth);
  }

  public string Wrap(string iText, int iTargetLineWidth, bool iKern, out int oLineWidth)
  {
    bool flag1 = false;
    string str = "";
    while (!flag1)
    {
      str = "";
      flag1 = true;
      int num1;
      for (int startIndex = 0; startIndex < iText.Length; startIndex = num1 + 1)
      {
        num1 = iText.Length;
        int num2 = 0;
        int num3 = 0;
        char iLeft = char.MinValue;
        bool flag2 = false;
        int index;
        for (index = startIndex; index < iText.Length; ++index)
        {
          char ch = iText[index];
          if (flag2)
          {
            switch (ch)
            {
              case '[':
                Glyph oGlyph1;
                this.GetGlyph(ch, out oGlyph1);
                if (iKern)
                {
                  num3 += this.CalcKern(iLeft, ch);
                  iLeft = ch;
                }
                num3 += oGlyph1.AdvanceWidth + this.mSpacing;
                flag2 = false;
                break;
              case ']':
                flag2 = false;
                break;
            }
          }
          else
          {
            if (ch == ' ' && (num3 <= iTargetLineWidth || index < num1))
            {
              num1 = index;
              num2 = num3;
            }
            switch (ch)
            {
              case char.MinValue:
                goto label_21;
              case '\n':
                num1 = index;
                goto label_21;
              default:
                if (ch == '[' && !flag2)
                {
                  flag2 = true;
                  break;
                }
                Glyph oGlyph2;
                this.GetGlyph(ch, out oGlyph2);
                if (iKern)
                {
                  num3 += this.CalcKern(iLeft, ch);
                  iLeft = ch;
                }
                num3 += oGlyph2.AdvanceWidth + this.mSpacing;
                break;
            }
          }
          if (num3 > iTargetLineWidth && index >= num1)
            break;
        }
label_21:
        if (index == iText.Length && num3 <= iTargetLineWidth)
        {
          num2 = num3;
          num1 = index;
        }
        if (num2 > iTargetLineWidth)
        {
          flag1 = false;
          iTargetLineWidth = num2;
          break;
        }
        str += iText.Substring(startIndex, num1 - startIndex);
        if (num1 < iText.Length)
          str += (string) (object) '\n';
      }
    }
    oLineWidth = iTargetLineWidth;
    return str;
  }

  public Vector2 MeasureText(char[] iText, bool iKern)
  {
    return this.MeasureText(iText, iKern, iText.Length);
  }

  public Vector2 MeasureText(char[] iText, bool iKern, int iLength)
  {
    Vector2 vector2 = new Vector2();
    vector2.Y = (float) this.mLineHeight;
    int val2 = 0;
    char iLeft = char.MinValue;
    bool flag = false;
    iLength = Math.Min(iText.Length, iLength);
    for (int index = 0; index < iLength; ++index)
    {
      char ch = iText[index];
      if (flag)
      {
        switch (ch)
        {
          case '[':
            Glyph oGlyph1;
            this.GetGlyph(ch, out oGlyph1);
            if (iKern)
            {
              val2 += this.CalcKern(iLeft, ch);
              iLeft = ch;
            }
            val2 += oGlyph1.AdvanceWidth + this.mSpacing;
            flag = false;
            continue;
          case ']':
            flag = false;
            continue;
          default:
            continue;
        }
      }
      else
      {
        switch (ch)
        {
          case char.MinValue:
            goto label_16;
          case '\n':
            vector2.Y += (float) this.mLineHeight;
            vector2.X = Math.Max(vector2.X, (float) val2);
            val2 = 0;
            break;
        }
        if (ch == '[' && !flag)
        {
          flag = true;
        }
        else
        {
          Glyph oGlyph2;
          this.GetGlyph(ch, out oGlyph2);
          if (iKern)
          {
            val2 += this.CalcKern(iLeft, ch);
            iLeft = ch;
          }
          val2 += oGlyph2.AdvanceWidth + this.mSpacing;
        }
      }
    }
label_16:
    vector2.X = Math.Max(vector2.X, (float) val2);
    if ((double) vector2.X > 1.0)
      vector2.X -= (float) this.mSpacing;
    return vector2;
  }

  public Vector2 MeasureText(string iText, bool iKern)
  {
    return this.MeasureText(iText, iKern, iText.Length);
  }

  public Vector2 MeasureText(string iText, bool iKern, int iLength)
  {
    Vector2 vector2 = new Vector2();
    vector2.Y = (float) this.mLineHeight;
    int val2 = 0;
    char iLeft = char.MinValue;
    bool flag = false;
    iLength = Math.Min(iText.Length, iLength);
    for (int index = 0; index < iLength; ++index)
    {
      char ch = iText[index];
      if (flag)
      {
        switch (ch)
        {
          case '[':
            Glyph oGlyph1;
            this.GetGlyph(ch, out oGlyph1);
            if (iKern)
            {
              val2 += this.CalcKern(iLeft, ch);
              iLeft = ch;
            }
            val2 += oGlyph1.AdvanceWidth + this.mSpacing;
            flag = false;
            continue;
          case ']':
            flag = false;
            continue;
          default:
            continue;
        }
      }
      else
      {
        switch (ch)
        {
          case char.MinValue:
            goto label_16;
          case '\n':
            vector2.Y += (float) this.mLineHeight;
            vector2.X = Math.Max(vector2.X, (float) val2);
            val2 = 0;
            break;
        }
        if (ch == '[' && !flag)
        {
          flag = true;
        }
        else
        {
          Glyph oGlyph2;
          this.GetGlyph(ch, out oGlyph2);
          if (iKern)
          {
            val2 += this.CalcKern(iLeft, ch);
            iLeft = ch;
          }
          val2 += oGlyph2.AdvanceWidth + this.mSpacing;
        }
      }
    }
label_16:
    vector2.X = Math.Max(vector2.X, (float) val2);
    if ((double) vector2.X > 1.0)
      vector2.X -= (float) this.mSpacing;
    return vector2;
  }

  public int CalcKern(char iLeft, char iRight)
  {
    Dictionary<char, int> dictionary;
    int num;
    return this.mKerning.TryGetValue(iLeft, out dictionary) && dictionary.TryGetValue(iRight, out num) ? num : 0;
  }
}
