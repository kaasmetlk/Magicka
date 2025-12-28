using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Graphics
{
	// Token: 0x0200062B RID: 1579
	public class TypingText : Text
	{
		// Token: 0x06002F96 RID: 12182 RVA: 0x0018122B File Offset: 0x0017F42B
		public TypingText(int iLength, BitmapFont iFont, TextAlign iAlign, bool iDynamic, float iTypeSpeed) : base(iLength, iFont, iAlign, iDynamic)
		{
			this.mNextChar = 0f;
			this.mCharIndex = -1;
			this.mVisibleCharacters = 0;
			this.mTypeSpeed = iTypeSpeed;
		}

		// Token: 0x06002F97 RID: 12183 RVA: 0x00181259 File Offset: 0x0017F459
		public override void SetText(string iText)
		{
			this.mNextChar = 0f;
			this.mCharIndex = -1;
			this.mVisibleCharacters = 0;
			base.SetText(iText);
		}

		// Token: 0x06002F98 RID: 12184 RVA: 0x0018127C File Offset: 0x0017F47C
		public void Update(float iDeltaTime)
		{
			this.mNextChar -= iDeltaTime;
			while (this.mNextChar < 0f && this.mVisibleCharacters < this.mPrimitiveCount / 2)
			{
				this.mCharIndex++;
				char c = this.mText[this.mCharIndex];
				if (c == '[')
				{
					this.mCharIndex++;
					c = this.mText[this.mCharIndex];
					if (c != '[')
					{
						if (char.ToLowerInvariant(c) == 'p')
						{
							this.mCharIndex++;
							c = this.mText[this.mCharIndex];
							if (c != '=')
							{
								throw new Exception("Syntax Error!");
							}
							float num = 0f;
							for (;;)
							{
								this.mCharIndex++;
								c = this.mText[this.mCharIndex];
								if (!char.IsDigit(c))
								{
									break;
								}
								num *= 10f;
								num += (float)(c - '0');
							}
							if (c == '.')
							{
								float num2 = 0.1f;
								for (;;)
								{
									this.mCharIndex++;
									c = this.mText[this.mCharIndex];
									if (!char.IsDigit(c))
									{
										break;
									}
									num += (float)(c - '0') * num2;
									num2 *= 0.1f;
								}
								if (c != ']')
								{
									throw new Exception("Syntax Error!");
								}
							}
							else if (c != ']')
							{
								throw new Exception("Syntax Error!");
							}
							this.mNextChar += num;
						}
						else
						{
							while (c != ']')
							{
								this.mCharIndex++;
								c = this.mText[this.mCharIndex];
							}
						}
					}
					else
					{
						this.mVisibleCharacters++;
						this.mNextChar += 1f / this.mTypeSpeed;
					}
				}
				else if (TypingText.IsPunctuation(c) & (this.mCharIndex + 1 < this.mText.Length && char.IsWhiteSpace(this.mText[this.mCharIndex + 1])))
				{
					this.mVisibleCharacters++;
					this.mNextChar += 0.25f;
				}
				else if (c != '\n')
				{
					this.mVisibleCharacters++;
					this.mNextChar += 1f / this.mTypeSpeed;
				}
			}
		}

		// Token: 0x06002F99 RID: 12185 RVA: 0x001814C4 File Offset: 0x0017F6C4
		private static bool IsPunctuation(char iChar)
		{
			return iChar == '!' | iChar == ',' | iChar == '.' | iChar == ':' | iChar == ';' | iChar == '?' | iChar == '…' | iChar == '‼';
		}

		// Token: 0x06002F9A RID: 12186 RVA: 0x001814FB File Offset: 0x0017F6FB
		public void Finish()
		{
			this.mCharIndex = base.Characters.Length;
			this.mVisibleCharacters = this.mPrimitiveCount / 2;
		}

		// Token: 0x17000B49 RID: 2889
		// (get) Token: 0x06002F9B RID: 12187 RVA: 0x00181519 File Offset: 0x0017F719
		public bool IsFinished
		{
			get
			{
				return this.mVisibleCharacters == this.mPrimitiveCount / 2;
			}
		}

		// Token: 0x06002F9C RID: 12188 RVA: 0x0018152C File Offset: 0x0017F72C
		public override void Draw(GUIBasicEffect iEffect, ref Matrix iTransform)
		{
			if (this.mDirty)
			{
				this.UpdateVertices();
			}
			int num = this.mVisibleCharacters;
			if (num <= 0)
			{
				return;
			}
			iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, this.mVertexStride);
			iEffect.GraphicsDevice.Indices = this.mIndices;
			iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			iEffect.Transform = iTransform;
			iEffect.Texture = this.mFont.Texture;
			iEffect.TextureEnabled = true;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.mPrimitiveCount * 2, 0, num * 2);
		}

		// Token: 0x04003390 RID: 13200
		private float mNextChar;

		// Token: 0x04003391 RID: 13201
		private int mVisibleCharacters;

		// Token: 0x04003392 RID: 13202
		private int mCharIndex;

		// Token: 0x04003393 RID: 13203
		private float mTypeSpeed;
	}
}
