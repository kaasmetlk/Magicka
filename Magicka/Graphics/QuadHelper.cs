using System;
using Microsoft.Xna.Framework;

namespace Magicka.Graphics
{
	// Token: 0x02000502 RID: 1282
	internal static class QuadHelper
	{
		// Token: 0x06002603 RID: 9731 RVA: 0x0011279B File Offset: 0x0011099B
		public static int CreateQuadFan(Vector4[] iVertices, int iIndex, Vector2 iOrigin, Vector2 iSize)
		{
			return QuadHelper.CreateQuadFan(iVertices, iIndex, iOrigin, iSize, Vector2.Zero, Vector2.One);
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x001127B0 File Offset: 0x001109B0
		public static int CreateQuadFan(Vector4[] iVertices, int iIndex, Vector2 iOrigin, Vector2 iSize, Vector2 iUVOffset, Vector2 iUVSize)
		{
			iVertices[iIndex].X = 0f - iOrigin.X;
			iVertices[iIndex].Y = 0f - iOrigin.Y;
			iVertices[iIndex].Z = iUVOffset.X;
			iVertices[iIndex].W = iUVOffset.Y;
			iIndex++;
			iVertices[iIndex].X = 0f - iOrigin.X + iSize.X;
			iVertices[iIndex].Y = 0f - iOrigin.Y;
			iVertices[iIndex].Z = iUVOffset.X + iUVSize.X;
			iVertices[iIndex].W = iUVOffset.Y;
			iIndex++;
			iVertices[iIndex].X = 0f - iOrigin.X + iSize.X;
			iVertices[iIndex].Y = 0f - iOrigin.Y + iSize.Y;
			iVertices[iIndex].Z = iUVOffset.X + iUVSize.X;
			iVertices[iIndex].W = iUVOffset.Y + iUVSize.Y;
			iIndex++;
			iVertices[iIndex].X = 0f - iOrigin.X;
			iVertices[iIndex].Y = 0f - iOrigin.Y + iSize.Y;
			iVertices[iIndex].Z = iUVOffset.X;
			iVertices[iIndex].W = iUVOffset.Y + iUVSize.Y;
			return 4;
		}

		// Token: 0x06002605 RID: 9733 RVA: 0x0011296D File Offset: 0x00110B6D
		public static int CreateQuadList(Vector4[] iVertices, int iIndex, Vector2 iOrigin, Vector2 iSize)
		{
			return QuadHelper.CreateQuadList(iVertices, iIndex, iOrigin, iSize, Vector2.Zero, Vector2.One);
		}

		// Token: 0x06002606 RID: 9734 RVA: 0x00112984 File Offset: 0x00110B84
		public static int CreateQuadList(Vector4[] iVertices, int iIndex, Vector2 iOrigin, Vector2 iSize, Vector2 iUVOffset, Vector2 iUVSize)
		{
			iVertices[iIndex].X = 0f - iOrigin.X;
			iVertices[iIndex].Y = 0f - iOrigin.Y;
			iVertices[iIndex].Z = iUVOffset.X;
			iVertices[iIndex].W = iUVOffset.Y;
			iIndex++;
			iVertices[iIndex].X = 0f - iOrigin.X + iSize.X;
			iVertices[iIndex].Y = 0f - iOrigin.Y;
			iVertices[iIndex].Z = iUVOffset.X + iUVSize.X;
			iVertices[iIndex].W = iUVOffset.Y;
			iIndex++;
			iVertices[iIndex].X = 0f - iOrigin.X + iSize.X;
			iVertices[iIndex].Y = 0f - iOrigin.Y + iSize.Y;
			iVertices[iIndex].Z = iUVOffset.X + iUVSize.X;
			iVertices[iIndex].W = iUVOffset.Y + iUVSize.Y;
			iIndex++;
			iVertices[iIndex].X = 0f - iOrigin.X;
			iVertices[iIndex].Y = 0f - iOrigin.Y;
			iVertices[iIndex].Z = iUVOffset.X;
			iVertices[iIndex].W = iUVOffset.Y;
			iIndex++;
			iVertices[iIndex].X = 0f - iOrigin.X + iSize.X;
			iVertices[iIndex].Y = 0f - iOrigin.Y + iSize.Y;
			iVertices[iIndex].Z = iUVOffset.X + iUVSize.X;
			iVertices[iIndex].W = iUVOffset.Y + iUVSize.Y;
			iIndex++;
			iVertices[iIndex].X = 0f - iOrigin.X;
			iVertices[iIndex].Y = 0f - iOrigin.Y + iSize.Y;
			iVertices[iIndex].Z = iUVOffset.X;
			iVertices[iIndex].W = iUVOffset.Y + iUVSize.Y;
			return 6;
		}
	}
}
