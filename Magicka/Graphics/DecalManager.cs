using System;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Graphics
{
	// Token: 0x02000415 RID: 1045
	public sealed class DecalManager
	{
		// Token: 0x170007E9 RID: 2025
		// (get) Token: 0x06002059 RID: 8281 RVA: 0x000E5914 File Offset: 0x000E3B14
		public static DecalManager Instance
		{
			get
			{
				if (DecalManager.mSingelton == null)
				{
					lock (DecalManager.mSingeltonLock)
					{
						if (DecalManager.mSingelton == null)
						{
							DecalManager.mSingelton = new DecalManager();
						}
					}
				}
				return DecalManager.mSingelton;
			}
		}

		// Token: 0x0600205B RID: 8283 RVA: 0x000E598C File Offset: 0x000E3B8C
		private DecalManager()
		{
			HardwareInstancedProjectionEffect iEffect = new HardwareInstancedProjectionEffect(Game.Instance.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
			RenderManager.Instance.RegisterEffect(iEffect);
			this.mAlphaDecals = Game.Instance.Content.Load<Texture2D>("EffectTextures/decals");
			this.mAlphaBlendedRenderData = new DecalManager.RenderData[3];
		}

		// Token: 0x0600205C RID: 8284 RVA: 0x000E5A04 File Offset: 0x000E3C04
		public void Initialize(Scene iScene, int iMaxDecals)
		{
			this.mScene = iScene;
			if (iScene == null)
			{
				return;
			}
			this.mLastAlphaBlendedDecal = -1;
			this.mFreeAlphaBlendedDecals = new IntHeap(iMaxDecals);
			this.mAlphaUniqueIDs = new int[iMaxDecals];
			this.Clear();
			this.mAlphaTransforms = new Matrix[iMaxDecals];
			this.mAlphaInvTransforms = new Matrix[iMaxDecals];
			this.mAlphaArgs = new Vector3[iMaxDecals];
			this.mAlphaColors = new Vector4[iMaxDecals];
			this.mAlphaUniqueIDs = new int[iMaxDecals];
			for (int i = 0; i < 3; i++)
			{
				DecalManager.RenderData renderData = new DecalManager.RenderData(iMaxDecals);
				this.mAlphaBlendedRenderData[i] = renderData;
				renderData.mTextureScale = DecalManager.sDecals;
				renderData.mTexture = this.mAlphaDecals;
			}
			this.CreateVertexBuffer();
		}

		// Token: 0x0600205D RID: 8285 RVA: 0x000E5AB8 File Offset: 0x000E3CB8
		internal void Clear()
		{
			this.mLastAlphaBlendedDecal = -1;
			this.mFreeAlphaBlendedDecals.Clear();
			for (int i = 0; i < this.mAlphaUniqueIDs.Length; i++)
			{
				this.mFreeAlphaBlendedDecals.Push(i);
				this.mAlphaUniqueIDs[i] = -1;
			}
		}

		// Token: 0x170007EA RID: 2026
		// (get) Token: 0x0600205E RID: 8286 RVA: 0x000E5AFF File Offset: 0x000E3CFF
		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return this.mVertexDeclaration;
			}
		}

		// Token: 0x170007EB RID: 2027
		// (get) Token: 0x0600205F RID: 8287 RVA: 0x000E5B07 File Offset: 0x000E3D07
		public Scene Scene
		{
			get
			{
				return this.mScene;
			}
		}

		// Token: 0x06002060 RID: 8288 RVA: 0x000E5B10 File Offset: 0x000E3D10
		private void CreateVertexBuffer()
		{
			if (this.mVertices != null && !this.mVertices.IsDisposed)
			{
				this.mVertices.Dispose();
			}
			if (this.mVertexDeclaration != null && !this.mVertexDeclaration.IsDisposed)
			{
				this.mVertexDeclaration.Dispose();
			}
			if (this.mIndices != null && !this.mIndices.IsDisposed)
			{
				this.mIndices.Dispose();
			}
			DecalManager.Vertex[] array = new DecalManager.Vertex[240];
			ushort[] array2 = new ushort[1080];
			for (int i = 0; i < 30; i++)
			{
				int num = i * 8;
				for (int j = 0; j < 8; j++)
				{
					array[num + j].Position.X = ((j % 2 < 1) ? -0.5f : 0.5f);
					array[num + j].Position.Y = ((j % 4 < 2) ? -0.5f : 0.5f);
					array[num + j].Position.Z = ((j % 8 < 4) ? -0.5f : 0.5f);
					array[num + j].Instance = (float)i;
				}
				int num2 = i * 36;
				array2[num2++] = (ushort)num;
				array2[num2++] = (ushort)(num + 2);
				array2[num2++] = (ushort)(num + 1);
				array2[num2++] = (ushort)(num + 1);
				array2[num2++] = (ushort)(num + 2);
				array2[num2++] = (ushort)(num + 3);
				array2[num2++] = (ushort)(num + 1);
				array2[num2++] = (ushort)num;
				array2[num2++] = (ushort)(num + 4);
				array2[num2++] = (ushort)(num + 4);
				array2[num2++] = (ushort)(num + 5);
				array2[num2++] = (ushort)(num + 1);
				array2[num2++] = (ushort)num;
				array2[num2++] = (ushort)(num + 2);
				array2[num2++] = (ushort)(num + 4);
				array2[num2++] = (ushort)(num + 4);
				array2[num2++] = (ushort)(num + 2);
				array2[num2++] = (ushort)(num + 6);
				array2[num2++] = (ushort)(num + 4);
				array2[num2++] = (ushort)(num + 6);
				array2[num2++] = (ushort)(num + 5);
				array2[num2++] = (ushort)(num + 5);
				array2[num2++] = (ushort)(num + 6);
				array2[num2++] = (ushort)(num + 7);
				array2[num2++] = (ushort)(num + 1);
				array2[num2++] = (ushort)(num + 5);
				array2[num2++] = (ushort)(num + 3);
				array2[num2++] = (ushort)(num + 3);
				array2[num2++] = (ushort)(num + 5);
				array2[num2++] = (ushort)(num + 7);
				array2[num2++] = (ushort)(num + 2);
				array2[num2++] = (ushort)(num + 3);
				array2[num2++] = (ushort)(num + 6);
				array2[num2++] = (ushort)(num + 6);
				array2[num2++] = (ushort)(num + 3);
				array2[num2++] = (ushort)(num + 7);
			}
			this.mVertices = new VertexBuffer(Game.Instance.GraphicsDevice, 3840, BufferUsage.WriteOnly);
			this.mVertices.SetData<DecalManager.Vertex>(array);
			this.mIndices = new IndexBuffer(Game.Instance.GraphicsDevice, 2160, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
			this.mIndices.SetData<ushort>(array2);
			this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, DecalManager.Vertex.VertexElements);
			for (int k = 0; k < 3; k++)
			{
				DecalManager.RenderData renderData = this.mAlphaBlendedRenderData[k];
				renderData.mVertices = this.mVertices;
				renderData.mVerticesHash = this.mVertices.GetHashCode();
				renderData.mIndices = this.mIndices;
				renderData.mVertexDeclaration = this.mVertexDeclaration;
			}
		}

		// Token: 0x06002061 RID: 8289 RVA: 0x000E5ED8 File Offset: 0x000E40D8
		public void AddAlphaBlendedDecal(Decal iDecal, AnimatedLevelPart iAnimation, float iScale, ref Vector3 iPosition, ref Vector3 iNormal, float iTTL)
		{
			Vector2 vector = default(Vector2);
			vector.Y = iScale;
			vector.X = iScale;
			DecalManager.DecalReference decalReference;
			this.AddAlphaBlendedDecal(iDecal, iAnimation, ref vector, ref iPosition, null, ref iNormal, iTTL, 1f, out decalReference);
		}

		// Token: 0x06002062 RID: 8290 RVA: 0x000E5F20 File Offset: 0x000E4120
		public void AddAlphaBlendedDecal(Decal iDecal, AnimatedLevelPart iAnimation, ref Vector2 iScale, ref Vector3 iPosition, Vector3? iDirection, ref Vector3 iNormal, float iTTL)
		{
			DecalManager.DecalReference decalReference;
			this.AddAlphaBlendedDecal(iDecal, iAnimation, ref iScale, ref iPosition, iDirection, ref iNormal, iTTL, 1f, out decalReference);
		}

		// Token: 0x06002063 RID: 8291 RVA: 0x000E5F48 File Offset: 0x000E4148
		public void AddAlphaBlendedDecal(Decal iDecal, AnimatedLevelPart iAnimation, ref Vector2 iScale, ref Vector3 iPosition, Vector3? iDirection, ref Vector3 iNormal, float iTTL, float iAlpha)
		{
			DecalManager.DecalReference decalReference;
			this.AddAlphaBlendedDecal(iDecal, iAnimation, ref iScale, ref iPosition, iDirection, ref iNormal, iTTL, iAlpha, out decalReference);
		}

		// Token: 0x06002064 RID: 8292 RVA: 0x000E5F6C File Offset: 0x000E416C
		public void AddAlphaBlendedDecal(Decal iDecal, AnimatedLevelPart iAnimation, ref Vector2 iScale, ref Vector3 iPosition, Vector3? iDirection, ref Vector3 iNormal, float iTTL, float iAlpha, out DecalManager.DecalReference oReference)
		{
			Vector4 vector = default(Vector4);
			vector.X = (vector.Y = (vector.Z = 1f));
			vector.W = iAlpha;
			this.AddAlphaBlendedDecal(iDecal, iAnimation, ref iScale, ref iPosition, iDirection, ref iNormal, iTTL, ref vector, out oReference);
		}

		// Token: 0x06002065 RID: 8293 RVA: 0x000E5FC0 File Offset: 0x000E41C0
		public void AddAlphaBlendedDecal(Decal iDecal, AnimatedLevelPart iAnimation, ref Vector2 iScale, ref Vector3 iPosition, Vector3? iDirection, ref Vector3 iNormal, float iTTL, ref Vector4 iColor, out DecalManager.DecalReference oReference)
		{
			int num2;
			if (this.mFreeAlphaBlendedDecals.IsEmpty)
			{
				float num = float.MaxValue;
				num2 = -1;
				for (int i = 0; i < this.mAlphaArgs.Length; i++)
				{
					float z = this.mAlphaArgs[i].Z;
					if (z < num)
					{
						num = z;
						num2 = i;
					}
				}
				if (num2 < 0)
				{
					oReference = default(DecalManager.DecalReference);
					oReference.Index = -1;
					return;
				}
			}
			else
			{
				num2 = this.mFreeAlphaBlendedDecals.Pop();
			}
			Matrix matrix = default(Matrix);
			if (iDirection != null)
			{
				Vector3 value = iDirection.Value;
				Vector3 right;
				Vector3.Cross(ref value, ref iNormal, out right);
				right.Normalize();
				Vector3.Cross(ref iNormal, ref right, out value);
				matrix.Backward = iNormal;
				matrix.Right = right;
				matrix.Up = value;
			}
			else
			{
				Vector3 up = default(Vector3);
				float num3 = 1f;
				while (num3 > 0.99f | num3 < -0.99f | float.IsNaN(num3))
				{
					up.X = (float)this.mRandom.NextDouble() - 0.5f;
					up.Y = (float)this.mRandom.NextDouble() - 0.5f;
					up.Z = (float)this.mRandom.NextDouble() - 0.5f;
					up.Normalize();
					Vector3.Dot(ref up, ref iNormal, out num3);
				}
				Vector3 right;
				Vector3.Cross(ref up, ref iNormal, out right);
				right.Normalize();
				Vector3.Cross(ref iNormal, ref right, out up);
				matrix.Backward = iNormal;
				matrix.Right = right;
				matrix.Up = up;
			}
			matrix.M11 *= iScale.X;
			matrix.M12 *= iScale.X;
			matrix.M13 *= iScale.X;
			matrix.M21 *= iScale.Y;
			matrix.M22 *= iScale.Y;
			matrix.M23 *= iScale.Y;
			matrix.M44 = 1f;
			matrix.Translation = iPosition;
			this.mAlphaTransforms[num2] = matrix;
			Matrix.Invert(ref matrix, out this.mAlphaInvTransforms[num2]);
			this.mAlphaArgs[num2] = new Vector3((float)(iDecal % Decal.BloodBlack0), (float)(iDecal / Decal.BloodBlack0), iTTL);
			this.mAlphaColors[num2] = iColor;
			if (num2 > this.mLastAlphaBlendedDecal)
			{
				this.mLastAlphaBlendedDecal = num2;
			}
			oReference.Index = num2;
			this.mAlphaUniqueIDs[num2] = (int)DateTime.Now.Ticks;
			if (this.mAlphaUniqueIDs[num2] == -1)
			{
				this.mAlphaUniqueIDs[num2] = 0;
			}
			oReference.Hash = this.mAlphaUniqueIDs[num2];
			if (iAnimation != null)
			{
				iAnimation.AddDecal(ref oReference);
			}
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x000E62A4 File Offset: 0x000E44A4
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mLastAlphaBlendedDecal >= 0)
			{
				DecalManager.RenderData renderData = this.mAlphaBlendedRenderData[(int)iDataChannel];
				renderData.mNrOfDecals = this.mLastAlphaBlendedDecal + 1;
				Array.Copy(this.mAlphaTransforms, 0, renderData.mTransforms, 0, renderData.mNrOfDecals);
				Array.Copy(this.mAlphaInvTransforms, 0, renderData.mInvTransforms, 0, renderData.mNrOfDecals);
				Array.Copy(this.mAlphaArgs, 0, renderData.mArgs, 0, renderData.mNrOfDecals);
				Array.Copy(this.mAlphaColors, 0, renderData.mColors, 0, renderData.mNrOfDecals);
				this.mScene.AddProjection(iDataChannel, renderData);
			}
			float num = (float)Math.Pow((double)(1f - (float)this.mFreeAlphaBlendedDecals.Count / (float)this.mAlphaUniqueIDs.Length), 8.0);
			num *= 19f;
			float num2 = iDeltaTime * (1f + num);
			for (int i = 0; i <= this.mLastAlphaBlendedDecal; i++)
			{
				float z = this.mAlphaArgs[i].Z;
				float num3 = z - num2;
				this.mAlphaArgs[i].Z = num3;
				if (num3 < 1E-45f && z >= 1E-45f)
				{
					this.mAlphaUniqueIDs[i] = -1;
					this.mFreeAlphaBlendedDecals.Push(i);
				}
			}
			while (this.mLastAlphaBlendedDecal >= 0 && this.mAlphaArgs[this.mLastAlphaBlendedDecal].Z < 0f)
			{
				this.mLastAlphaBlendedDecal--;
			}
		}

		// Token: 0x06002067 RID: 8295 RVA: 0x000E6420 File Offset: 0x000E4620
		public bool AddAlphaBlendedDecalAlpha(ref DecalManager.DecalReference iReference, float iAdditionalAlpha)
		{
			if (iReference.Index < 0)
			{
				return false;
			}
			if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
			{
				this.mAlphaColors[iReference.Index].W = MathHelper.Clamp(this.mAlphaColors[iReference.Index].W + iAdditionalAlpha, 0f, 1f);
				return true;
			}
			iReference.Index = -1;
			return false;
		}

		// Token: 0x06002068 RID: 8296 RVA: 0x000E64A4 File Offset: 0x000E46A4
		public bool SetDecalTTL(ref DecalManager.DecalReference iReference, float iTTL)
		{
			if (iReference.Index < 0)
			{
				return false;
			}
			if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
			{
				this.mAlphaArgs[iReference.Index].Z = iTTL;
				return true;
			}
			iReference.Index = -1;
			return false;
		}

		// Token: 0x06002069 RID: 8297 RVA: 0x000E6504 File Offset: 0x000E4704
		public bool SetDecal(ref DecalManager.DecalReference iReference, float iTTL, ref Matrix iTransform)
		{
			if (iReference.Index < 0)
			{
				return false;
			}
			if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
			{
				this.mAlphaTransforms[iReference.Index] = iTransform;
				Matrix.Invert(ref iTransform, out this.mAlphaInvTransforms[iReference.Index]);
				this.mAlphaArgs[iReference.Index].Z = iTTL;
				return true;
			}
			iReference.Index = -1;
			return false;
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x000E6598 File Offset: 0x000E4798
		public bool SetDecal(ref DecalManager.DecalReference iReference, float iTTL, ref Matrix iTransform, float iAlpha)
		{
			if (iReference.Index < 0)
			{
				return false;
			}
			if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
			{
				this.mAlphaTransforms[iReference.Index] = iTransform;
				Matrix.Invert(ref iTransform, out this.mAlphaInvTransforms[iReference.Index]);
				this.mAlphaArgs[iReference.Index].Z = iTTL;
				this.mAlphaColors[iReference.Index].W = iAlpha;
				return true;
			}
			iReference.Index = -1;
			return false;
		}

		// Token: 0x0600206B RID: 8299 RVA: 0x000E6644 File Offset: 0x000E4844
		public bool SetDecal(ref DecalManager.DecalReference iReference, ref Matrix iTransform, ref Vector4 iColor)
		{
			if (iReference.Index < 0)
			{
				return false;
			}
			if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
			{
				this.mAlphaTransforms[iReference.Index] = iTransform;
				Matrix.Invert(ref iTransform, out this.mAlphaInvTransforms[iReference.Index]);
				this.mAlphaColors[iReference.Index] = iColor;
				return true;
			}
			iReference.Index = -1;
			return false;
		}

		// Token: 0x0600206C RID: 8300 RVA: 0x000E66DC File Offset: 0x000E48DC
		public bool GetDecalTTL(ref DecalManager.DecalReference iReference, out float oTTL)
		{
			oTTL = 0f;
			if (iReference.Index < 0)
			{
				return false;
			}
			if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
			{
				oTTL = this.mAlphaArgs[iReference.Index].Z;
				return true;
			}
			return false;
		}

		// Token: 0x0600206D RID: 8301 RVA: 0x000E673C File Offset: 0x000E493C
		public bool GetDecalAlpha(ref DecalManager.DecalReference iReference, out float oAlpha)
		{
			oAlpha = 0f;
			if (iReference.Index < 0)
			{
				return false;
			}
			if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
			{
				oAlpha = this.mAlphaColors[iReference.Index].W;
				return true;
			}
			return false;
		}

		// Token: 0x0600206E RID: 8302 RVA: 0x000E679C File Offset: 0x000E499C
		public bool TransformDecal(ref DecalManager.DecalReference iReference, ref Matrix iTransform)
		{
			if (iReference.Index < 0)
			{
				return false;
			}
			if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
			{
				Matrix.Multiply(ref this.mAlphaTransforms[iReference.Index], ref iTransform, out this.mAlphaTransforms[iReference.Index]);
				Matrix.Invert(ref this.mAlphaTransforms[iReference.Index], out this.mAlphaInvTransforms[iReference.Index]);
				return true;
			}
			return false;
		}

		// Token: 0x040022D0 RID: 8912
		private static DecalManager mSingelton;

		// Token: 0x040022D1 RID: 8913
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040022D2 RID: 8914
		private Matrix[] mAlphaTransforms;

		// Token: 0x040022D3 RID: 8915
		private Matrix[] mAlphaInvTransforms;

		// Token: 0x040022D4 RID: 8916
		private Vector3[] mAlphaArgs;

		// Token: 0x040022D5 RID: 8917
		private Vector4[] mAlphaColors;

		// Token: 0x040022D6 RID: 8918
		private int[] mAlphaUniqueIDs;

		// Token: 0x040022D7 RID: 8919
		private int mLastAlphaBlendedDecal = -1;

		// Token: 0x040022D8 RID: 8920
		private VertexBuffer mVertices;

		// Token: 0x040022D9 RID: 8921
		private IndexBuffer mIndices;

		// Token: 0x040022DA RID: 8922
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x040022DB RID: 8923
		private IntHeap mFreeAlphaBlendedDecals;

		// Token: 0x040022DC RID: 8924
		private Random mRandom = new Random();

		// Token: 0x040022DD RID: 8925
		private DecalManager.RenderData[] mAlphaBlendedRenderData;

		// Token: 0x040022DE RID: 8926
		private Scene mScene;

		// Token: 0x040022DF RID: 8927
		private Texture2D mAlphaDecals;

		// Token: 0x040022E0 RID: 8928
		private static readonly Vector2 sDecals = new Vector2(8f, 8f);

		// Token: 0x02000416 RID: 1046
		public struct DecalReference
		{
			// Token: 0x040022E1 RID: 8929
			public int Index;

			// Token: 0x040022E2 RID: 8930
			public int Hash;
		}

		// Token: 0x02000417 RID: 1047
		private struct Vertex
		{
			// Token: 0x040022E3 RID: 8931
			public const int SIZEINBYTES = 16;

			// Token: 0x040022E4 RID: 8932
			public Vector3 Position;

			// Token: 0x040022E5 RID: 8933
			public float Instance;

			// Token: 0x040022E6 RID: 8934
			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 12, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.BlendIndices, 0)
			};
		}

		// Token: 0x02000418 RID: 1048
		private class RenderData : IProjectionObject
		{
			// Token: 0x06002070 RID: 8304 RVA: 0x000E6878 File Offset: 0x000E4A78
			public RenderData(int iMaxDecals)
			{
				this.mTransforms = new Matrix[iMaxDecals];
				this.mInvTransforms = new Matrix[iMaxDecals];
				this.mArgs = new Vector3[iMaxDecals];
				this.mColors = new Vector4[iMaxDecals];
			}

			// Token: 0x170007EC RID: 2028
			// (get) Token: 0x06002071 RID: 8305 RVA: 0x000E68EF File Offset: 0x000E4AEF
			public int Effect
			{
				get
				{
					return HardwareInstancedProjectionEffect.TYPEHASH;
				}
			}

			// Token: 0x170007ED RID: 2029
			// (get) Token: 0x06002072 RID: 8306 RVA: 0x000E68F6 File Offset: 0x000E4AF6
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x170007EE RID: 2030
			// (get) Token: 0x06002073 RID: 8307 RVA: 0x000E68F9 File Offset: 0x000E4AF9
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertices;
				}
			}

			// Token: 0x170007EF RID: 2031
			// (get) Token: 0x06002074 RID: 8308 RVA: 0x000E6901 File Offset: 0x000E4B01
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndices;
				}
			}

			// Token: 0x170007F0 RID: 2032
			// (get) Token: 0x06002075 RID: 8309 RVA: 0x000E6909 File Offset: 0x000E4B09
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x170007F1 RID: 2033
			// (get) Token: 0x06002076 RID: 8310 RVA: 0x000E6911 File Offset: 0x000E4B11
			public int VertexStride
			{
				get
				{
					return 16;
				}
			}

			// Token: 0x170007F2 RID: 2034
			// (get) Token: 0x06002077 RID: 8311 RVA: 0x000E6915 File Offset: 0x000E4B15
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06002078 RID: 8312 RVA: 0x000E691D File Offset: 0x000E4B1D
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return false;
			}

			// Token: 0x06002079 RID: 8313 RVA: 0x000E6920 File Offset: 0x000E4B20
			public void Draw(Effect iEffect, Texture2D iDepthMap)
			{
				HardwareInstancedProjectionEffect hardwareInstancedProjectionEffect = iEffect as HardwareInstancedProjectionEffect;
				hardwareInstancedProjectionEffect.DepthMap = iDepthMap;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
				hardwareInstancedProjectionEffect.PixelSize = new Vector2(1f / (float)iDepthMap.Width, 1f / (float)iDepthMap.Height);
				hardwareInstancedProjectionEffect.Texture = this.mTexture;
				hardwareInstancedProjectionEffect.TextureScale = this.mTextureScale;
				for (int i = 0; i < this.mNrOfDecals; i += 30)
				{
					int num = Math.Min(Math.Min(this.mTransforms.Length - i, 30), this.mNrOfDecals - i);
					Array.Copy(this.mTransforms, i, this.mBatchTransforms, 0, num);
					Array.Copy(this.mInvTransforms, i, this.mBatchInvTransforms, 0, num);
					Array.Copy(this.mArgs, i, this.mBatchArgs, 0, num);
					Array.Copy(this.mColors, i, this.mBatchColors, 0, num);
					hardwareInstancedProjectionEffect.WorldTransforms = this.mBatchTransforms;
					hardwareInstancedProjectionEffect.InvWorldTransforms = this.mBatchInvTransforms;
					hardwareInstancedProjectionEffect.Args = this.mBatchArgs;
					hardwareInstancedProjectionEffect.Colors = this.mBatchColors;
					hardwareInstancedProjectionEffect.CommitChanges();
					hardwareInstancedProjectionEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, num * 8, 0, num * 12);
				}
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
			}

			// Token: 0x040022E7 RID: 8935
			public VertexBuffer mVertices;

			// Token: 0x040022E8 RID: 8936
			public IndexBuffer mIndices;

			// Token: 0x040022E9 RID: 8937
			public VertexDeclaration mVertexDeclaration;

			// Token: 0x040022EA RID: 8938
			public Texture2D mTexture;

			// Token: 0x040022EB RID: 8939
			public Vector2 mTextureScale;

			// Token: 0x040022EC RID: 8940
			public int mNrOfDecals;

			// Token: 0x040022ED RID: 8941
			public int mVerticesHash;

			// Token: 0x040022EE RID: 8942
			public Matrix[] mTransforms;

			// Token: 0x040022EF RID: 8943
			public Matrix[] mInvTransforms;

			// Token: 0x040022F0 RID: 8944
			public Vector3[] mArgs;

			// Token: 0x040022F1 RID: 8945
			public Vector4[] mColors;

			// Token: 0x040022F2 RID: 8946
			private Matrix[] mBatchTransforms = new Matrix[30];

			// Token: 0x040022F3 RID: 8947
			private Matrix[] mBatchInvTransforms = new Matrix[30];

			// Token: 0x040022F4 RID: 8948
			private Vector3[] mBatchArgs = new Vector3[30];

			// Token: 0x040022F5 RID: 8949
			private Vector4[] mBatchColors = new Vector4[30];
		}
	}
}
