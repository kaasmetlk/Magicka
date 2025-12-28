using System;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main.Options;
using Magicka.Graphics;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x0200063D RID: 1597
	internal class GamePadConfigMessageBox : MessageBox
	{
		// Token: 0x17000B65 RID: 2917
		// (get) Token: 0x0600302C RID: 12332 RVA: 0x0018A600 File Offset: 0x00188800
		public static GamePadConfigMessageBox Instance
		{
			get
			{
				if (GamePadConfigMessageBox.sSingelton == null)
				{
					lock (GamePadConfigMessageBox.sSingeltonLock)
					{
						if (GamePadConfigMessageBox.sSingelton == null)
						{
							GamePadConfigMessageBox.sSingelton = new GamePadConfigMessageBox();
						}
					}
				}
				return GamePadConfigMessageBox.sSingelton;
			}
		}

		// Token: 0x0600302D RID: 12333 RVA: 0x0018A654 File Offset: 0x00188854
		private GamePadConfigMessageBox()
		{
			ControllerFunction[][] array = new ControllerFunction[20][];
			array[0] = new ControllerFunction[]
			{
				ControllerFunction.Move_Up
			};
			array[1] = new ControllerFunction[]
			{
				ControllerFunction.Move_Down
			};
			array[2] = new ControllerFunction[]
			{
				ControllerFunction.Move_Left
			};
			ControllerFunction[][] array2 = array;
			int num = 3;
			ControllerFunction[] array3 = new ControllerFunction[1];
			array2[num] = array3;
			array[4] = new ControllerFunction[]
			{
				ControllerFunction.Spell_Up
			};
			array[5] = new ControllerFunction[]
			{
				ControllerFunction.Spell_Down
			};
			array[6] = new ControllerFunction[]
			{
				ControllerFunction.Spell_Left
			};
			array[7] = new ControllerFunction[]
			{
				ControllerFunction.Spell_Right
			};
			array[8] = new ControllerFunction[]
			{
				ControllerFunction.Spell_Wheel
			};
			array[9] = new ControllerFunction[]
			{
				ControllerFunction.Menu_Select,
				ControllerFunction.Attack
			};
			array[10] = new ControllerFunction[]
			{
				ControllerFunction.Menu_Back,
				ControllerFunction.Boost,
				ControllerFunction.Cast_Magick
			};
			array[11] = new ControllerFunction[]
			{
				ControllerFunction.Interact
			};
			array[12] = new ControllerFunction[]
			{
				ControllerFunction.Special,
				ControllerFunction.Cast_Self
			};
			array[13] = new ControllerFunction[]
			{
				ControllerFunction.Block
			};
			array[14] = new ControllerFunction[]
			{
				ControllerFunction.Cast_Force
			};
			array[15] = new ControllerFunction[]
			{
				ControllerFunction.Cast_Area
			};
			array[16] = new ControllerFunction[]
			{
				ControllerFunction.Magick_Next
			};
			array[17] = new ControllerFunction[]
			{
				ControllerFunction.Magick_Prev
			};
			array[18] = new ControllerFunction[]
			{
				ControllerFunction.Pause
			};
			array[19] = new ControllerFunction[]
			{
				ControllerFunction.Inventory
			};
			this.mFunctions = array;
			base..ctor("Configure GamePad (NonLoc)");
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/GamePadConfig");
			}
			this.mSize = new Vector2(640f, 512f);
			Vector4[] array4 = new Vector4[72];
			Vector2 vector = default(Vector2);
			vector.X = 1f / (float)this.mTexture.Width;
			vector.Y = 1f / (float)this.mTexture.Height;
			int num2 = 0;
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(0, 0, 640, 448), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(0, 448, 64, 64), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(64, 448, 64, 64), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(384, 448, 64, 64), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(448, 448, 64, 64), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(128, 448, 128, 64), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(256, 448, 128, 64), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(640, 0, 128, 128), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(640, 128, 128, 128), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(768, 0, 128, 128), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(896, 0, 128, 128), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(768, 128, 128, 128), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(896, 128, 128, 128), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(640, 256, 128, 128), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(768, 256, 128, 128), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(896, 256, 128, 128), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(768, 384, 128, 128), ref vector);
			GamePadConfigMessageBox.MakeQuad(array4, ref num2, new Rectangle(896, 384, 128, 128), ref vector);
			lock (graphicsDevice)
			{
				this.mVertices = new VertexBuffer(graphicsDevice, 16 * array4.Length, BufferUsage.WriteOnly);
				this.mVertices.SetData<Vector4>(array4);
				this.mVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
				});
			}
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			this.mAction = new Text(128, font, TextAlign.Center, false);
			this.mBack = new MenuTextItem(SubMenu.LOC_BACK, default(Vector2), font, TextAlign.Left);
			this.mBack.ColorDisabled = Defines.DIALOGUE_COLOR_DEFAULT * 0.5f;
			this.mBack.Color = Defines.DIALOGUE_COLOR_DEFAULT;
			this.mBack.ColorSelected = Vector4.One;
			this.mCancel = new MenuTextItem(SubMenu.LOC_CANCEL, default(Vector2), font, TextAlign.Right);
			this.mCancel.ColorDisabled = Defines.DIALOGUE_COLOR_DEFAULT * 0.5f;
			this.mCancel.Color = Defines.DIALOGUE_COLOR_DEFAULT;
			this.mCancel.ColorSelected = Vector4.One;
		}

		// Token: 0x0600302E RID: 12334 RVA: 0x0018AC54 File Offset: 0x00188E54
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			this.mBack.LanguageChanged();
			this.mCancel.LanguageChanged();
		}

		// Token: 0x0600302F RID: 12335 RVA: 0x0018AC74 File Offset: 0x00188E74
		private static void MakeQuad(Vector4[] iVertices, ref int iIndex, Rectangle iRect, ref Vector2 iUVScale)
		{
			iVertices[iIndex].X = (float)(-(float)(iRect.Width / 2));
			iVertices[iIndex].Y = (float)(-(float)(iRect.Height / 2));
			iVertices[iIndex].Z = (float)iRect.X * iUVScale.X;
			iVertices[iIndex].W = (float)iRect.Y * iUVScale.Y;
			iIndex++;
			iVertices[iIndex].X = (float)(iRect.Width / 2);
			iVertices[iIndex].Y = (float)(-(float)(iRect.Height / 2));
			iVertices[iIndex].Z = (float)(iRect.X + iRect.Width) * iUVScale.X;
			iVertices[iIndex].W = (float)iRect.Y * iUVScale.Y;
			iIndex++;
			iVertices[iIndex].X = (float)(iRect.Width / 2);
			iVertices[iIndex].Y = (float)(iRect.Height / 2);
			iVertices[iIndex].Z = (float)(iRect.X + iRect.Width) * iUVScale.X;
			iVertices[iIndex].W = (float)(iRect.Y + iRect.Height) * iUVScale.Y;
			iIndex++;
			iVertices[iIndex].X = (float)(-(float)(iRect.Width / 2));
			iVertices[iIndex].Y = (float)(iRect.Height / 2);
			iVertices[iIndex].Z = (float)iRect.X * iUVScale.X;
			iVertices[iIndex].W = (float)(iRect.Y + iRect.Height) * iUVScale.Y;
			iIndex++;
		}

		// Token: 0x17000B66 RID: 2918
		// (get) Token: 0x06003030 RID: 12336 RVA: 0x0018AE55 File Offset: 0x00189055
		// (set) Token: 0x06003031 RID: 12337 RVA: 0x0018AE5D File Offset: 0x0018905D
		public DirectInputController GamePad
		{
			get
			{
				return this.mGamePad;
			}
			set
			{
				this.mGamePad = value;
			}
		}

		// Token: 0x06003032 RID: 12338 RVA: 0x0018AE66 File Offset: 0x00189066
		public override void OnTextInput(char iChar)
		{
		}

		// Token: 0x06003033 RID: 12339 RVA: 0x0018AE68 File Offset: 0x00189068
		public override void OnMove(Controller iSender, ControllerDirection iDirection)
		{
		}

		// Token: 0x06003034 RID: 12340 RVA: 0x0018AE6C File Offset: 0x0018906C
		public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
		{
			this.mBack.Selected = this.mBack.InsideBounds((float)iNewState.X, (float)iNewState.Y);
			this.mCancel.Selected = this.mCancel.InsideBounds((float)iNewState.X, (float)iNewState.Y);
		}

		// Token: 0x06003035 RID: 12341 RVA: 0x0018AEC8 File Offset: 0x001890C8
		public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
		{
			if (this.mCancel.InsideBounds((float)iNewState.X, (float)iNewState.Y) && iNewState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed)
			{
				this.mGamePad.Configured = true;
				this.Kill();
				return;
			}
			if (iNewState.LeftButton == ButtonState.Released & this.mFunction > 0 & this.mBack.InsideBounds((float)iNewState.X, (float)iNewState.Y))
			{
				this.mFunction--;
				this.UpdateAction();
			}
		}

		// Token: 0x06003036 RID: 12342 RVA: 0x0018AF5E File Offset: 0x0018915E
		public override void OnSelect(Controller iSender)
		{
		}

		// Token: 0x06003037 RID: 12343 RVA: 0x0018AF60 File Offset: 0x00189160
		public override void Show()
		{
			this.mFunction = 0;
			this.UpdateAction();
			this.mGamePad.OnChange += this.OnGamePadChange;
			base.Show();
		}

		// Token: 0x06003038 RID: 12344 RVA: 0x0018AF8C File Offset: 0x0018918C
		private void UpdateAction()
		{
			ControllerFunction[] array = this.mFunctions[this.mFunction];
			string text = string.Format("({0}/{1})\n{2}", this.mFunction + 1, this.mFunctions.Length, array[0].ToString());
			for (int i = 1; i < array.Length; i++)
			{
				text = text + "/" + array[i].ToString();
			}
			this.mAction.SetText(text);
		}

		// Token: 0x06003039 RID: 12345 RVA: 0x0018B00C File Offset: 0x0018920C
		private void OnGamePadChange(DirectInputController.Binding iBinding)
		{
			if (this.mFunction < this.mFunctions.Length)
			{
				ControllerFunction[] array = this.mFunctions[this.mFunction];
				for (int i = 0; i < array.Length; i++)
				{
					this.mGamePad.Bindings[(int)array[i]] = iBinding;
				}
			}
			this.mFunction++;
			if (this.mFunction >= this.mFunctions.Length)
			{
				this.mGamePad.Configured = true;
				SaveManager.Instance.SaveSettings();
				this.Kill();
				if (SubMenuOptionsGamepad.Instance.Controller == this.mGamePad)
				{
					SubMenuOptionsGamepad.Instance.Controller = this.mGamePad;
					return;
				}
			}
			else
			{
				this.UpdateAction();
				this.mGamePad.OnChange += this.OnGamePadChange;
			}
		}

		// Token: 0x0600303A RID: 12346 RVA: 0x0018B0D8 File Offset: 0x001892D8
		public override void Draw(float iDeltaTime)
		{
			base.Draw(iDeltaTime);
			float num = this.mAlpha * this.mAlpha;
			this.mBack.Enabled = (this.mFunction > 0);
			MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 16);
			MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			Point screenSize = RenderManager.Instance.ScreenSize;
			Vector2 vector = default(Vector2);
			vector.X = (float)screenSize.X;
			vector.Y = (float)screenSize.Y;
			Matrix transform = default(Matrix);
			transform.M11 = (transform.M22 = 1f);
			transform.M44 = 1f;
			transform.M41 = vector.X * 0.5f;
			transform.M42 = vector.Y * 0.5f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.Texture = this.mTexture;
			MessageBox.sGUIBasicEffect.TextureEnabled = true;
			MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
			Vector4 color = default(Vector4);
			color.X = (color.Y = (color.Z = 1f));
			color.W = num;
			MessageBox.sGUIBasicEffect.Color = color;
			MessageBox.sGUIBasicEffect.CommitChanges();
			MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			transform.M41 = vector.X * 0.5f + 32f;
			transform.M42 = vector.Y * 0.5f + 64f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 18)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 16, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 12, 2);
			}
			transform.M41 = vector.X * 0.5f - 32f;
			transform.M42 = vector.Y * 0.5f + 64f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 19)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 16, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 12, 2);
			}
			transform.M41 = vector.X * 0.5f + 112f;
			transform.M42 = vector.Y * 0.5f + 48f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 9)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
			}
			transform.M41 = vector.X * 0.5f + 160f;
			transform.M42 = vector.Y * 0.5f - 0f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 10)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
			}
			transform.M41 = vector.X * 0.5f + 64f;
			transform.M42 = vector.Y * 0.5f - 0f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 11)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
			}
			transform.M41 = vector.X * 0.5f + 112f;
			transform.M42 = vector.Y * 0.5f - 48f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 12)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
			}
			transform.M41 = vector.X * 0.5f - 112f;
			transform.M42 = vector.Y * 0.5f - 112f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == -1)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 24, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
			}
			transform.M41 = vector.X * 0.5f - 96f;
			transform.M42 = vector.Y * 0.5f - 160f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 15)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 24, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
			}
			transform.M41 = vector.X * 0.5f + 112f;
			transform.M42 = vector.Y * 0.5f - 112f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 13)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 24, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
			}
			transform.M41 = vector.X * 0.5f + 96f;
			transform.M42 = vector.Y * 0.5f - 160f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 14)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 24, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
			}
			transform.M41 = vector.X * 0.5f - 118f;
			transform.M42 = vector.Y * 0.5f - 0f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 17)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
			}
			else if (this.mFunction == 16)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 60, 2);
			}
			else if (this.mFunction == -1)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 64, 2);
			}
			else if (this.mFunction == -1)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 68, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 52, 2);
			}
			transform.M41 = vector.X * 0.5f - 64f;
			transform.M42 = vector.Y * 0.5f + 142f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 0)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 36, 2);
			}
			else if (this.mFunction == 1)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 40, 2);
			}
			else if (this.mFunction == 2)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 44, 2);
			}
			else if (this.mFunction == 3)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 48, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 28, 2);
			}
			transform.M41 = vector.X * 0.5f + 64f;
			transform.M42 = vector.Y * 0.5f + 142f;
			MessageBox.sGUIBasicEffect.Transform = transform;
			MessageBox.sGUIBasicEffect.CommitChanges();
			if (this.mFunction == 4)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 36, 2);
			}
			else if (this.mFunction == 5)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 40, 2);
			}
			else if (this.mFunction == 6)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 44, 2);
			}
			else if (this.mFunction == 7)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 48, 2);
			}
			else if (this.mFunction == 8)
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 32, 2);
			}
			else
			{
				MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 28, 2);
			}
			color = MenuItem.COLOR;
			color.W *= num;
			MessageBox.sGUIBasicEffect.Color = color;
			this.mAction.Draw(MessageBox.sGUIBasicEffect, vector.X * 0.5f, vector.Y * 0.5f + (210f - 0.5f * (float)this.mAction.Font.LineHeight));
			Vector4 color_DISABLED = MenuItem.COLOR_DISABLED;
			color_DISABLED.W *= num;
			Vector4 color_SELECTED = MenuItem.COLOR_SELECTED;
			color_SELECTED.W *= num;
			this.mBack.Color = color;
			this.mBack.ColorSelected = color_SELECTED;
			this.mBack.ColorDisabled = color_DISABLED;
			this.mBack.Position = new Vector2(vector.X * 0.5f - 180f, vector.Y * 0.5f + 220f);
			this.mBack.Draw(MessageBox.sGUIBasicEffect, 1f);
			this.mCancel.Color = color;
			this.mCancel.ColorSelected = color_SELECTED;
			this.mCancel.ColorDisabled = color_DISABLED;
			this.mCancel.Position = new Vector2(vector.X * 0.5f + 180f, vector.Y * 0.5f + 220f);
			this.mCancel.Draw(MessageBox.sGUIBasicEffect, 1f);
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
			MessageBox.sGUIBasicEffect.End();
		}

		// Token: 0x04003462 RID: 13410
		private static GamePadConfigMessageBox sSingelton;

		// Token: 0x04003463 RID: 13411
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04003464 RID: 13412
		private DirectInputController mGamePad;

		// Token: 0x04003465 RID: 13413
		private int mFunction;

		// Token: 0x04003466 RID: 13414
		private ControllerFunction[][] mFunctions;

		// Token: 0x04003467 RID: 13415
		private Texture2D mTexture;

		// Token: 0x04003468 RID: 13416
		private VertexBuffer mVertices;

		// Token: 0x04003469 RID: 13417
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x0400346A RID: 13418
		private MenuTextItem mBack;

		// Token: 0x0400346B RID: 13419
		private MenuTextItem mCancel;

		// Token: 0x0400346C RID: 13420
		private Text mAction;
	}
}
