using System;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x020005A1 RID: 1441
	public class InventoryBox
	{
		// Token: 0x06002B17 RID: 11031 RVA: 0x0015240C File Offset: 0x0015060C
		public InventoryBox()
		{
			this.mShow = false;
			IndexBuffer indexBuffer;
			VertexBuffer vertexBuffer;
			VertexDeclaration iDeclaration;
			TextBoxEffect textBoxEffect;
			lock (Game.Instance.GraphicsDevice)
			{
				indexBuffer = new IndexBuffer(Game.Instance.GraphicsDevice, 108, BufferUsage.None, IndexElementSize.SixteenBits);
				vertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, 16 * VertexPositionNormalTexture.SizeInBytes, BufferUsage.None);
				iDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
				indexBuffer.SetData<ushort>(TextBox.INDICES);
				vertexBuffer.SetData<VertexPositionNormalTexture>(TextBox.VERTICES);
				textBoxEffect = new TextBoxEffect(Game.Instance.GraphicsDevice, Game.Instance.Content);
			}
			Point screenSize = RenderManager.Instance.ScreenSize;
			textBoxEffect.BorderSize = 32f;
			textBoxEffect.Texture = Game.Instance.Content.Load<Texture2D>("UI/HUD/Dialog_Say");
			textBoxEffect.ScreenSize = new Vector2((float)screenSize.X, (float)screenSize.Y);
			GUIBasicEffect guibasicEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			guibasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
			guibasicEffect.VertexColorEnabled = true;
			guibasicEffect.TextureEnabled = true;
			guibasicEffect.Color = Vector4.One;
			this.mText = new Text(1024, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Left, false);
			this.mText.DefaultColor = Defines.DIALOGUE_COLOR_DEFAULT;
			this.mText.SetText("");
			Texture2D texture2D = Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks");
			Vector2 vector = new Vector2(880f / (float)texture2D.Width, 1260f / (float)texture2D.Height);
			Vector2 vector2 = new Vector2(250f / (float)texture2D.Width, 230f / (float)texture2D.Height);
			VertexPositionColorTexture[] array = new VertexPositionColorTexture[4];
			array[0].TextureCoordinate = new Vector2(vector.X + vector2.X, vector.Y + vector2.Y);
			array[0].Position = new Vector3(250f, 230f, 0f);
			array[0].Color = Color.White;
			array[1].TextureCoordinate = new Vector2(vector.X, vector.Y + vector2.Y);
			array[1].Position = new Vector3(0f, 230f, 0f);
			array[1].Color = Color.White;
			array[2].TextureCoordinate = new Vector2(vector.X, vector.Y);
			array[2].Position = new Vector3(0f, 0f, 0f);
			array[2].Color = Color.White;
			array[3].TextureCoordinate = new Vector2(vector.X + vector2.X, vector.Y);
			array[3].Position = new Vector3(250f, 0f, 0f);
			array[3].Color = Color.White;
			VertexBuffer vertexBuffer2;
			VertexDeclaration iImageDeclaration;
			lock (Game.Instance.GraphicsDevice)
			{
				vertexBuffer2 = new VertexBuffer(Game.Instance.GraphicsDevice, 4 * VertexPositionColorTexture.SizeInBytes, BufferUsage.WriteOnly);
				vertexBuffer2.SetData<VertexPositionColorTexture>(array);
				iImageDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionColorTexture.VertexElements);
			}
			this.mRenderData = new InventoryBox.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new InventoryBox.RenderData(vertexBuffer, indexBuffer, iDeclaration, vertexBuffer2, iImageDeclaration, texture2D);
				this.mRenderData[i].mTextBoxEffect = textBoxEffect;
				this.mRenderData[i].mGUIBasicEffect = guibasicEffect;
				this.mRenderData[i].mText = this.mText;
				this.mRenderData[i].mTextOffset = 0f;
			}
		}

		// Token: 0x06002B18 RID: 11032 RVA: 0x0015283C File Offset: 0x00150A3C
		public void ShowInventory(Item iWeapon, Item iStaff, Magicka.GameLogic.Entities.Character iOwner)
		{
			this.mOwner = iOwner;
			this.mShow = true;
			if (this.mWeaponID == iWeapon.DisplayName & this.mStaffID == iStaff.DisplayName)
			{
				return;
			}
			this.mWeaponID = iWeapon.DisplayName;
			if (iWeapon.DisplayName == 0)
			{
				this.mWeaponName = "";
			}
			else
			{
				this.mWeaponName = LanguageManager.Instance.GetString(iWeapon.DisplayName);
			}
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
			if (iStaff.DisplayName == 0)
			{
				this.mStaffName = "";
			}
			else
			{
				this.mStaffName = LanguageManager.Instance.GetString(iStaff.DisplayName);
			}
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
			int num = 9;
			char[] array = this.mWeaponName.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				this.mText.Characters[num + i] = array[i];
			}
			this.mText.Characters[num + array.Length] = '[';
			this.mText.Characters[num + array.Length + 1] = '/';
			this.mText.Characters[num + array.Length + 2] = 'c';
			this.mText.Characters[num + array.Length + 3] = ']';
			this.mText.Characters[num + array.Length + 4] = '\n';
			this.mWeaponDesc = this.mText.Font.Wrap(this.mWeaponDesc, 350, true);
			num += array.Length + 4 + 1;
			char[] array2 = this.mWeaponDesc.ToCharArray();
			for (int j = 0; j < array2.Length; j++)
			{
				this.mText.Characters[num + j] = array2[j];
			}
			num += array2.Length;
			this.mText.Characters[num] = '\n';
			this.mText.Characters[num + 1] = '\n';
			num += 2;
			this.mText.Characters[num] = '[';
			this.mText.Characters[num + 1] = 'c';
			this.mText.Characters[num + 2] = '=';
			this.mText.Characters[num + 3] = '1';
			this.mText.Characters[num + 4] = ',';
			this.mText.Characters[num + 5] = '1';
			this.mText.Characters[num + 6] = ',';
			this.mText.Characters[num + 7] = '1';
			this.mText.Characters[num + 8] = ']';
			num += 9;
			array = this.mStaffName.ToCharArray();
			for (int k = 0; k < array.Length; k++)
			{
				this.mText.Characters[num + k] = array[k];
			}
			num += array.Length;
			this.mText.Characters[num] = '[';
			this.mText.Characters[num + 1] = '/';
			this.mText.Characters[num + 2] = 'c';
			this.mText.Characters[num + 3] = ']';
			this.mText.Characters[num + 4] = '\n';
			this.mStaffDesc = this.mText.Font.Wrap(this.mStaffDesc, 350, true);
			num += 5;
			array2 = this.mStaffDesc.ToCharArray();
			for (int l = 0; l < array2.Length; l++)
			{
				this.mText.Characters[num + l] = array2[l];
			}
			this.mText.Characters[num + array2.Length] = '\0';
			this.mText.MarkAsDirty();
			float y = this.mText.Font.MeasureText(this.mText.Characters, true).Y;
			this.mBox.Height = (int)Math.Max(y, 230f);
		}

		// Token: 0x17000A1F RID: 2591
		// (get) Token: 0x06002B19 RID: 11033 RVA: 0x00152CF3 File Offset: 0x00150EF3
		public bool Active
		{
			get
			{
				return this.mShow | this.mScale > 0f;
			}
		}

		// Token: 0x06002B1A RID: 11034 RVA: 0x00152D09 File Offset: 0x00150F09
		public void SetPosition(Vector3 iPosition)
		{
			this.mWorldPosition = iPosition;
		}

		// Token: 0x17000A20 RID: 2592
		// (get) Token: 0x06002B1B RID: 11035 RVA: 0x00152D12 File Offset: 0x00150F12
		public Magicka.GameLogic.Entities.Character Owner
		{
			get
			{
				return this.mOwner;
			}
		}

		// Token: 0x06002B1C RID: 11036 RVA: 0x00152D1A File Offset: 0x00150F1A
		public void Close(Magicka.GameLogic.Entities.Character iOwner)
		{
			if (iOwner == this.mOwner | iOwner == null)
			{
				this.mShow = false;
				this.mOwner = null;
			}
		}

		// Token: 0x06002B1D RID: 11037 RVA: 0x00152D3C File Offset: 0x00150F3C
		public void Update(float iDeltaTime, DataChannel iDataChannel)
		{
			if (this.mShow)
			{
				this.mScale = Math.Min(this.mScale + iDeltaTime * 4f, 1f);
			}
			else
			{
				if (this.mScale <= 0f)
				{
					return;
				}
				this.mScale = Math.Max(this.mScale - iDeltaTime * 4f, 0f);
			}
			this.mRenderData[(int)iDataChannel].mScale = this.mScale;
			this.mRenderData[(int)iDataChannel].mSize.X = (float)this.mBox.Width;
			this.mRenderData[(int)iDataChannel].mSize.Y = (float)this.mBox.Height;
			GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
		}

		// Token: 0x04002E3D RID: 11837
		private Rectangle mBox;

		// Token: 0x04002E3E RID: 11838
		private float mScale;

		// Token: 0x04002E3F RID: 11839
		private bool mShow;

		// Token: 0x04002E40 RID: 11840
		private Text mText;

		// Token: 0x04002E41 RID: 11841
		private Magicka.GameLogic.Entities.Character mOwner;

		// Token: 0x04002E42 RID: 11842
		private int mWeaponID;

		// Token: 0x04002E43 RID: 11843
		private int mStaffID;

		// Token: 0x04002E44 RID: 11844
		private string mWeaponName;

		// Token: 0x04002E45 RID: 11845
		private string mWeaponDesc;

		// Token: 0x04002E46 RID: 11846
		private string mStaffName;

		// Token: 0x04002E47 RID: 11847
		private string mStaffDesc;

		// Token: 0x04002E48 RID: 11848
		private InventoryBox.RenderData[] mRenderData;

		// Token: 0x04002E49 RID: 11849
		private Vector3 mWorldPosition;

		// Token: 0x020005A2 RID: 1442
		protected class RenderData : IRenderableGUIObject, IPreRenderRenderer
		{
			// Token: 0x06002B1E RID: 11038 RVA: 0x00152E09 File Offset: 0x00151009
			public RenderData(VertexBuffer iVertices, IndexBuffer iIndices, VertexDeclaration iDeclaration, VertexBuffer iImageVertices, VertexDeclaration iImageDeclaration, Texture2D iImageTexture)
			{
				this.mVertexBuffer = iVertices;
				this.mIndexBuffer = iIndices;
				this.mVertexDeclaration = iDeclaration;
				this.mImageTexture = iImageTexture;
				this.mImageVertexBuffer = iImageVertices;
				this.mImageDeclaration = iImageDeclaration;
			}

			// Token: 0x06002B1F RID: 11039 RVA: 0x00152E40 File Offset: 0x00151040
			public void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				this.mPosition.X = (float)screenSize.X * 0.5f;
				this.mPosition.Y = (float)screenSize.Y * 0.5f;
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
				this.mGUIBasicEffect.Texture = this.mImageTexture;
				this.mGUIBasicEffect.TextureEnabled = true;
				Matrix identity = Matrix.Identity;
				identity.M11 = this.mScale;
				identity.M22 = this.mScale;
				identity.M41 = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
				identity.M42 = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
				this.mGUIBasicEffect.Transform = identity;
				this.mGUIBasicEffect.Begin();
				this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
				this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				this.mText.Draw(this.mGUIBasicEffect, this.mPosition.X - (this.mSize.X * 0.5f - 250f) * this.mScale, this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale, this.mScale);
				this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
				this.mGUIBasicEffect.End();
			}

			// Token: 0x17000A21 RID: 2593
			// (get) Token: 0x06002B20 RID: 11040 RVA: 0x0015313D File Offset: 0x0015133D
			public int ZIndex
			{
				get
				{
					return 150;
				}
			}

			// Token: 0x06002B21 RID: 11041 RVA: 0x00153144 File Offset: 0x00151344
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				Vector2 screenSize2 = default(Vector2);
				screenSize2.X = (float)screenSize.X;
				screenSize2.Y = (float)screenSize.Y;
				this.mTextBoxEffect.ScreenSize = screenSize2;
				Vector4 vector;
				Vector4.Transform(ref this.mWorldPosition, ref iViewProjectionMatrix, out vector);
				this.mPosition.X = (vector.X / vector.W * 0.5f + 0.5f) * screenSize2.X;
				this.mPosition.X = this.mPosition.X - (this.mSize.X * 0.5f + 64f) / 3f * this.mScale;
				this.mPosition.Y = (vector.Y / vector.W * -0.5f + 0.5f) * screenSize2.Y;
				this.mPosition.Y = this.mPosition.Y - (this.mSize.Y * 0.5f + 64f) * this.mScale;
				if (this.mScale > 0.999f)
				{
					this.mPosition.X = (float)Math.Floor((double)this.mPosition.X);
					if (this.mSize.X % 2f > 0.5f)
					{
						this.mPosition.X = this.mPosition.X + 0.5f;
					}
					this.mPosition.Y = (float)Math.Floor((double)this.mPosition.Y);
					if (this.mSize.Y % 2f > 0.5f)
					{
						this.mPosition.Y = this.mPosition.Y + 0.5f;
					}
				}
			}

			// Token: 0x04002E4A RID: 11850
			public TextBoxEffect mTextBoxEffect;

			// Token: 0x04002E4B RID: 11851
			public GUIBasicEffect mGUIBasicEffect;

			// Token: 0x04002E4C RID: 11852
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04002E4D RID: 11853
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04002E4E RID: 11854
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04002E4F RID: 11855
			protected Texture2D mImageTexture;

			// Token: 0x04002E50 RID: 11856
			protected VertexBuffer mImageVertexBuffer;

			// Token: 0x04002E51 RID: 11857
			protected VertexDeclaration mImageDeclaration;

			// Token: 0x04002E52 RID: 11858
			public Vector3 mWorldPosition;

			// Token: 0x04002E53 RID: 11859
			private Vector2 mPosition;

			// Token: 0x04002E54 RID: 11860
			public bool mShowName;

			// Token: 0x04002E55 RID: 11861
			public float mTextOffset;

			// Token: 0x04002E56 RID: 11862
			public Text mText;

			// Token: 0x04002E57 RID: 11863
			public Vector2 mSize;

			// Token: 0x04002E58 RID: 11864
			public float mScale;
		}
	}
}
