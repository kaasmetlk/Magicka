// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.TypingText
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.Graphics;

public class TypingText : Text
{
  private float mNextChar;
  private int mVisibleCharacters;
  private int mCharIndex;
  private float mTypeSpeed;

  public TypingText(
    int iLength,
    BitmapFont iFont,
    TextAlign iAlign,
    bool iDynamic,
    float iTypeSpeed)
    : base(iLength, iFont, iAlign, iDynamic)
  {
    this.mNextChar = 0.0f;
    this.mCharIndex = -1;
    this.mVisibleCharacters = 0;
    this.mTypeSpeed = iTypeSpeed;
  }

  public override void SetText(string iText)
  {
    this.mNextChar = 0.0f;
    this.mCharIndex = -1;
    this.mVisibleCharacters = 0;
    base.SetText(iText);
  }

  public void Update(float iDeltaTime)
  {
    this.mNextChar -= iDeltaTime;
label_25:
    while ((double) this.mNextChar < 0.0 && this.mVisibleCharacters < this.mPrimitiveCount / 2)
    {
      ++this.mCharIndex;
      char iChar = this.mText[this.mCharIndex];
      if (iChar == '[')
      {
        ++this.mCharIndex;
        char c1 = this.mText[this.mCharIndex];
        if (c1 != '[')
        {
          if (char.ToLowerInvariant(c1) == 'p')
          {
            ++this.mCharIndex;
            if (this.mText[this.mCharIndex] != '=')
              throw new Exception("Syntax Error!");
            float num1 = 0.0f;
            char c2;
            while (true)
            {
              ++this.mCharIndex;
              c2 = this.mText[this.mCharIndex];
              if (char.IsDigit(c2))
                num1 = num1 * 10f + (float) ((int) c2 - 48 /*0x30*/);
              else
                break;
            }
            if (c2 == '.')
            {
              float num2 = 0.1f;
              char c3;
              while (true)
              {
                ++this.mCharIndex;
                c3 = this.mText[this.mCharIndex];
                if (char.IsDigit(c3))
                {
                  num1 += (float) ((int) c3 - 48 /*0x30*/) * num2;
                  num2 *= 0.1f;
                }
                else
                  break;
              }
              if (c3 != ']')
                throw new Exception("Syntax Error!");
            }
            else if (c2 != ']')
              throw new Exception("Syntax Error!");
            this.mNextChar += num1;
          }
          else
          {
            while (true)
            {
              if (c1 != ']')
              {
                ++this.mCharIndex;
                c1 = this.mText[this.mCharIndex];
              }
              else
                goto label_25;
            }
          }
        }
        else
        {
          ++this.mVisibleCharacters;
          this.mNextChar += 1f / this.mTypeSpeed;
        }
      }
      else if (((TypingText.IsPunctuation(iChar) ? 1 : 0) & (this.mCharIndex + 1 >= this.mText.Length ? 0 : (char.IsWhiteSpace(this.mText[this.mCharIndex + 1]) ? 1 : 0))) != 0)
      {
        ++this.mVisibleCharacters;
        this.mNextChar += 0.25f;
      }
      else if (iChar != '\n')
      {
        ++this.mVisibleCharacters;
        this.mNextChar += 1f / this.mTypeSpeed;
      }
    }
  }

  private static bool IsPunctuation(char iChar)
  {
    return iChar == '!' | iChar == ',' | iChar == '.' | iChar == ':' | iChar == ';' | iChar == '?' | iChar == '…' | iChar == '‼';
  }

  public void Finish()
  {
    this.mCharIndex = this.Characters.Length;
    this.mVisibleCharacters = this.mPrimitiveCount / 2;
  }

  public bool IsFinished => this.mVisibleCharacters == this.mPrimitiveCount / 2;

  public override void Draw(GUIBasicEffect iEffect, ref Matrix iTransform)
  {
    if (this.mDirty)
      this.UpdateVertices();
    int visibleCharacters = this.mVisibleCharacters;
    if (visibleCharacters <= 0)
      return;
    iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, this.mVertexStride);
    iEffect.GraphicsDevice.Indices = this.mIndices;
    iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    iEffect.Transform = iTransform;
    iEffect.Texture = (Texture) this.mFont.Texture;
    iEffect.TextureEnabled = true;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.mPrimitiveCount * 2, 0, visibleCharacters * 2);
  }
}
