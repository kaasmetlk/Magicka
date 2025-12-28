using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Magicka
{
	// Token: 0x020005C7 RID: 1479
	public class SharedContentManager : ContentManager
	{
		// Token: 0x06002C4C RID: 11340 RVA: 0x0015CA10 File Offset: 0x0015AC10
		public SharedContentManager(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			if (SharedContentManager.sCommon == null)
			{
				SharedContentManager.sCommon = new SharedContentManager.CommonContentManager(base.ServiceProvider, "content");
			}
			SharedContentManager.sCommon.mManagers++;
			this.mLoadedAssets = new Dictionary<string, int>();
		}

		// Token: 0x06002C4D RID: 11341 RVA: 0x0015CA5D File Offset: 0x0015AC5D
		private static string GetCleanPath(string iPath)
		{
			if (!Path.IsPathRooted(iPath))
			{
				iPath = Path.Combine(SharedContentManager.sCommon.RootDirectory, iPath);
			}
			return Path.GetFullPath(iPath).ToLowerInvariant();
		}

		// Token: 0x06002C4E RID: 11342 RVA: 0x0015CA84 File Offset: 0x0015AC84
		public override T Load<T>(string assetName)
		{
			assetName = SharedContentManager.GetCleanPath(assetName);
			T result = SharedContentManager.sCommon.Load<T>(assetName);
			int num;
			if (!this.mLoadedAssets.TryGetValue(assetName, out num))
			{
				num = 0;
			}
			this.mLoadedAssets[assetName] = num + 1;
			return result;
		}

		// Token: 0x06002C4F RID: 11343 RVA: 0x0015CAC7 File Offset: 0x0015ACC7
		public override void Unload()
		{
			if (this.mLoadedAssets == null)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			SharedContentManager.sCommon.Unload(this);
			this.mLoadedAssets.Clear();
			base.Unload();
		}

		// Token: 0x06002C50 RID: 11344 RVA: 0x0015CB00 File Offset: 0x0015AD00
		protected override void Dispose(bool disposing)
		{
			this.Unload();
			base.Dispose(disposing);
			this.mLoadedAssets = null;
			SharedContentManager.sCommon.mManagers--;
			if (SharedContentManager.sCommon.mManagers == 0)
			{
				SharedContentManager.sCommon.Dispose();
				SharedContentManager.sCommon = null;
			}
		}

		// Token: 0x04002FED RID: 12269
		private static SharedContentManager.CommonContentManager sCommon;

		// Token: 0x04002FEE RID: 12270
		private Dictionary<string, int> mLoadedAssets;

		// Token: 0x020005C8 RID: 1480
		private class CommonContentManager : ContentManager
		{
			// Token: 0x06002C51 RID: 11345 RVA: 0x0015CB4F File Offset: 0x0015AD4F
			public CommonContentManager(IServiceProvider serviceProvider, string iRootDirectory) : base(serviceProvider, iRootDirectory)
			{
				this.mReferences = new Dictionary<string, SharedContentManager.CommonContentManager.ReferencedAsset>();
			}

			// Token: 0x06002C52 RID: 11346 RVA: 0x0015CB74 File Offset: 0x0015AD74
			public override T Load<T>(string assetName)
			{
				assetName = SharedContentManager.GetCleanPath(assetName);
				SharedContentManager.CommonContentManager.ReferencedAsset referencedAsset;
				if (!this.mReferences.TryGetValue(assetName, out referencedAsset))
				{
					referencedAsset = new SharedContentManager.CommonContentManager.ReferencedAsset(assetName);
					this.mLoadStack.Push(referencedAsset);
					try
					{
						referencedAsset.Asset = base.ReadAsset<T>(assetName, null);
					}
					finally
					{
						this.mLoadStack.Pop();
					}
					if (!this.mReferences.ContainsKey(assetName))
					{
						this.mReferences.Add(assetName, referencedAsset);
					}
				}
				if (this.mLoadStack.Count > 0)
				{
					this.mLoadStack.Peek().Children.Add(referencedAsset);
				}
				referencedAsset.References++;
				return (T)((object)referencedAsset.Asset);
			}

			// Token: 0x06002C53 RID: 11347 RVA: 0x0015CC38 File Offset: 0x0015AE38
			public void Unload(SharedContentManager container)
			{
				foreach (KeyValuePair<string, int> keyValuePair in container.mLoadedAssets)
				{
					SharedContentManager.CommonContentManager.ReferencedAsset iAsset = this.mReferences[keyValuePair.Key];
					this.Dec(iAsset, keyValuePair.Value);
				}
			}

			// Token: 0x06002C54 RID: 11348 RVA: 0x0015CCA8 File Offset: 0x0015AEA8
			private void Dec(SharedContentManager.CommonContentManager.ReferencedAsset iAsset, int iDec)
			{
				iAsset.References -= iDec;
				if (iAsset.References == 0)
				{
					for (int i = 0; i < iAsset.Children.Count; i++)
					{
						this.Dec(iAsset.Children[i], 1);
					}
					if (iAsset.Asset is IDisposable)
					{
						(iAsset.Asset as IDisposable).Dispose();
					}
					this.mReferences.Remove(iAsset.Name);
				}
			}

			// Token: 0x04002FEF RID: 12271
			public int mManagers;

			// Token: 0x04002FF0 RID: 12272
			private readonly Dictionary<string, SharedContentManager.CommonContentManager.ReferencedAsset> mReferences;

			// Token: 0x04002FF1 RID: 12273
			private Stack<SharedContentManager.CommonContentManager.ReferencedAsset> mLoadStack = new Stack<SharedContentManager.CommonContentManager.ReferencedAsset>(16);

			// Token: 0x020005C9 RID: 1481
			private class ReferencedAsset
			{
				// Token: 0x06002C55 RID: 11349 RVA: 0x0015CD23 File Offset: 0x0015AF23
				public ReferencedAsset(string iName)
				{
					this.Name = iName;
					this.Children = new List<SharedContentManager.CommonContentManager.ReferencedAsset>();
					this.Asset = null;
					this.References = 0;
				}

				// Token: 0x04002FF2 RID: 12274
				public string Name;

				// Token: 0x04002FF3 RID: 12275
				public object Asset;

				// Token: 0x04002FF4 RID: 12276
				public int References;

				// Token: 0x04002FF5 RID: 12277
				public List<SharedContentManager.CommonContentManager.ReferencedAsset> Children;
			}
		}
	}
}
