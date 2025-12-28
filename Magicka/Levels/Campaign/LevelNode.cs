using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace Magicka.Levels.Campaign
{
	// Token: 0x0200028D RID: 653
	internal class LevelNode
	{
		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x0600133B RID: 4923 RVA: 0x000763F3 File Offset: 0x000745F3
		public string FullFileName
		{
			get
			{
				return this.mFullFileName;
			}
		}

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x0600133C RID: 4924 RVA: 0x000763FB File Offset: 0x000745FB
		public string FileName
		{
			get
			{
				return this.mFileName;
			}
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x0600133D RID: 4925 RVA: 0x00076403 File Offset: 0x00074603
		public string LoadingImage
		{
			get
			{
				return this.mLoadingImage;
			}
		}

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x0600133E RID: 4926 RVA: 0x0007640B File Offset: 0x0007460B
		public string Name
		{
			get
			{
				return this.mName;
			}
		}

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x0600133F RID: 4927 RVA: 0x00076413 File Offset: 0x00074613
		public int Description
		{
			get
			{
				return this.mDescription;
			}
		}

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06001340 RID: 4928 RVA: 0x0007641B File Offset: 0x0007461B
		public int ShortDescription
		{
			get
			{
				return this.mShortDescription;
			}
		}

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06001341 RID: 4929 RVA: 0x00076423 File Offset: 0x00074623
		public byte[] HashSum
		{
			get
			{
				return this.mHashSum;
			}
		}

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06001342 RID: 4930 RVA: 0x0007642B File Offset: 0x0007462B
		public LevelNode.SceneNode[] Scenes
		{
			get
			{
				return this.mScenes;
			}
		}

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06001343 RID: 4931 RVA: 0x00076433 File Offset: 0x00074633
		public string PreferredAvatar
		{
			get
			{
				return this.mPreferredAvatar;
			}
		}

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06001344 RID: 4932 RVA: 0x0007643B File Offset: 0x0007463B
		public List<string> AllowedAvatars
		{
			get
			{
				return this.mAllowedAvatars;
			}
		}

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06001345 RID: 4933 RVA: 0x00076444 File Offset: 0x00074644
		public Rulesets RulesetType
		{
			get
			{
				if (this.mScenes != null && this.mScenes.Length > 0)
				{
					for (int i = 0; i < this.mScenes.Length; i++)
					{
						if (this.mScenes[i].RulesetType != Rulesets.None)
						{
							return this.mScenes[i].RulesetType;
						}
					}
				}
				return Rulesets.None;
			}
		}

		// Token: 0x06001346 RID: 4934 RVA: 0x00076498 File Offset: 0x00074698
		public LevelNode(string iPath, string iFileName)
		{
			this.mFullFileName = iPath + iFileName;
			string path = "content/Levels/" + this.mFullFileName;
			FileStream fileStream = File.OpenRead(path);
			this.mHashSum = null;
			this.mPreferredAvatar = "wizard";
			this.mAllowedAvatars = new List<string>();
			XmlTextReader xmlTextReader = new XmlTextReader(fileStream);
			List<LevelNode.SceneNode> list = new List<LevelNode.SceneNode>();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			while (xmlTextReader.Read())
			{
				if (xmlTextReader.NodeType == XmlNodeType.Element)
				{
					if (xmlTextReader.Name.Equals("Name", StringComparison.OrdinalIgnoreCase))
					{
						if (xmlTextReader.IsEmptyElement)
						{
							throw new Exception("\"Name\" tag is empty in \"" + iFileName + '"');
						}
						while (xmlTextReader.Read())
						{
							if (xmlTextReader.NodeType == XmlNodeType.Text)
							{
								this.mName = xmlTextReader.Value.ToLowerInvariant();
								flag = true;
								break;
							}
							if (xmlTextReader.NodeType == XmlNodeType.EndElement && xmlTextReader.Name.Equals("Name"))
							{
								throw new Exception("\"Name\" tag is empty in \"" + iFileName + '"');
							}
						}
					}
					else if (xmlTextReader.Name.Equals("Description", StringComparison.OrdinalIgnoreCase))
					{
						if (xmlTextReader.IsEmptyElement)
						{
							throw new Exception("\"Description\" tag is empty in \"" + iFileName + '"');
						}
						while (xmlTextReader.Read())
						{
							if (xmlTextReader.NodeType == XmlNodeType.Text)
							{
								this.mDescription = xmlTextReader.Value.ToLowerInvariant().GetHashCodeCustom();
								if (!flag4)
								{
									this.mShortDescription = this.mDescription;
								}
								flag2 = true;
								break;
							}
							if (xmlTextReader.NodeType == XmlNodeType.Element && xmlTextReader.Name.Equals("Description"))
							{
								throw new Exception("\"Description\" tag is empty in \"" + iFileName + '"');
							}
						}
					}
					else if (xmlTextReader.Name.Equals("ShortDescription", StringComparison.OrdinalIgnoreCase))
					{
						if (xmlTextReader.IsEmptyElement)
						{
							this.mShortDescription = this.mDescription;
						}
						else
						{
							while (xmlTextReader.Read())
							{
								if (xmlTextReader.NodeType == XmlNodeType.Text)
								{
									this.mShortDescription = xmlTextReader.Value.ToLowerInvariant().GetHashCodeCustom();
									flag4 = true;
									break;
								}
								if (xmlTextReader.NodeType == XmlNodeType.Element && xmlTextReader.Name.Equals("ShortDescription"))
								{
									this.mShortDescription = this.mDescription;
									break;
								}
							}
						}
					}
					else if (xmlTextReader.Name.Equals("Loading", StringComparison.OrdinalIgnoreCase))
					{
						if (xmlTextReader.IsEmptyElement)
						{
							throw new Exception("\"Loading\" tag is empty in \"" + iFileName + '"');
						}
						while (xmlTextReader.Read())
						{
							if (xmlTextReader.NodeType == XmlNodeType.Text)
							{
								this.mLoadingImage = xmlTextReader.Value;
								flag3 = true;
								break;
							}
							if (xmlTextReader.NodeType == XmlNodeType.Element && xmlTextReader.Name.Equals("Loading"))
							{
								throw new Exception("\"Loading\" tag is empty in \"" + iFileName + '"');
							}
						}
					}
					else if (xmlTextReader.Name.Equals("Scenes", StringComparison.OrdinalIgnoreCase))
					{
						if (xmlTextReader.IsEmptyElement)
						{
							throw new Exception("\"Scenes\" tag is empty in \"" + iFileName + '"');
						}
						while (xmlTextReader.Read())
						{
							if (xmlTextReader.Name.Equals("Scene", StringComparison.OrdinalIgnoreCase))
							{
								if (xmlTextReader.IsEmptyElement)
								{
									throw new Exception("\"Scene\" tag is empty in \"" + iFileName + '"');
								}
								while (xmlTextReader.Read())
								{
									if (xmlTextReader.NodeType == XmlNodeType.Text)
									{
										list.Add(new LevelNode.SceneNode(Path.Combine(Path.GetDirectoryName(path), xmlTextReader.Value)));
										break;
									}
								}
							}
						}
					}
					else if (xmlTextReader.Name.Equals("PreferredAvatar", StringComparison.OrdinalIgnoreCase))
					{
						if (xmlTextReader.IsEmptyElement)
						{
							throw new Exception("\"PreferredAvatar\" tag is empty in \"" + iFileName + '"');
						}
						while (xmlTextReader.Read())
						{
							if (xmlTextReader.NodeType == XmlNodeType.Text)
							{
								this.mPreferredAvatar = xmlTextReader.Value.ToLowerInvariant();
								break;
							}
							if (xmlTextReader.NodeType == XmlNodeType.Element && xmlTextReader.Name.Equals("PreferredAvatar"))
							{
								throw new Exception("\"PreferredAvatar\" tag is empty in \"" + iFileName + '"');
							}
						}
					}
					else if (xmlTextReader.Name.Equals("AllowedAvatars", StringComparison.OrdinalIgnoreCase))
					{
						if (xmlTextReader.IsEmptyElement)
						{
							throw new Exception("\"AllowedAvatars\" tag is empty in \"" + iFileName + '"');
						}
						while (xmlTextReader.Read())
						{
							if (xmlTextReader.Name.Equals("Avatar", StringComparison.OrdinalIgnoreCase))
							{
								if (xmlTextReader.IsEmptyElement)
								{
									throw new Exception("\"Avatar\" tag is empty in \"" + iFileName + '"');
								}
								while (xmlTextReader.Read())
								{
									if (xmlTextReader.NodeType == XmlNodeType.Text)
									{
										this.mAllowedAvatars.Add(xmlTextReader.Value.ToLowerInvariant());
										break;
									}
									if (xmlTextReader.NodeType == XmlNodeType.EndElement)
									{
										break;
									}
								}
							}
							if (xmlTextReader.NodeType == XmlNodeType.EndElement)
							{
								break;
							}
						}
					}
				}
				if (flag && flag2 && flag3 && list.Count > 0)
				{
					break;
				}
			}
			xmlTextReader.Close();
			fileStream.Close();
			fileStream.Dispose();
			if (!flag)
			{
				throw new Exception("No \"Name\" tag found in \"" + iFileName + '"');
			}
			if (!flag2)
			{
				throw new Exception("No \"Description\" tag found in \"" + iFileName + '"');
			}
			if (!flag3)
			{
				throw new Exception("No \"Loading\" tag found in \"" + iFileName + '"');
			}
			if (list.Count == 0)
			{
				throw new Exception("No \"Scene\" tag found in \"" + iFileName + '"');
			}
			this.mScenes = list.ToArray();
			this.mFileName = Path.GetFileNameWithoutExtension(iFileName);
		}

		// Token: 0x06001347 RID: 4935 RVA: 0x00076A24 File Offset: 0x00074C24
		internal void ComputeHashSums(SHA256 iSHA)
		{
			for (int i = 0; i < this.mScenes.Length; i++)
			{
				this.mScenes[i].ComputeHashSums(iSHA);
			}
			FileStream fileStream = File.OpenRead("content/Levels/" + this.mFullFileName);
			this.mHashSum = iSHA.ComputeHash(fileStream);
			fileStream.Close();
		}

		// Token: 0x06001348 RID: 4936 RVA: 0x00076A7C File Offset: 0x00074C7C
		public byte[] GetCombinedHash()
		{
			if (this.mHashSum == null)
			{
				return null;
			}
			byte[] array = this.mHashSum.Clone() as byte[];
			for (int i = 0; i < this.mScenes.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					byte[] array2 = array;
					int num = j;
					array2[num] ^= this.mScenes[i].ScriptHashSum[j];
					byte[] array3 = array;
					int num2 = j;
					array3[num2] ^= this.mScenes[i].ModelHashSum[j];
				}
			}
			return array;
		}

		// Token: 0x040014E1 RID: 5345
		private string mFileName;

		// Token: 0x040014E2 RID: 5346
		private string mName;

		// Token: 0x040014E3 RID: 5347
		private string mFullFileName;

		// Token: 0x040014E4 RID: 5348
		private int mDescription;

		// Token: 0x040014E5 RID: 5349
		private int mShortDescription;

		// Token: 0x040014E6 RID: 5350
		private string mLoadingImage;

		// Token: 0x040014E7 RID: 5351
		private byte[] mHashSum;

		// Token: 0x040014E8 RID: 5352
		private LevelNode.SceneNode[] mScenes;

		// Token: 0x040014E9 RID: 5353
		private string mPreferredAvatar;

		// Token: 0x040014EA RID: 5354
		private List<string> mAllowedAvatars;

		// Token: 0x0200028E RID: 654
		public class SceneNode
		{
			// Token: 0x06001349 RID: 4937 RVA: 0x00076B0C File Offset: 0x00074D0C
			public SceneNode(string iFilename)
			{
				this.ScriptFilename = iFilename;
				FileStream fileStream = File.OpenRead(iFilename);
				this.ScriptHashSum = null;
				XmlTextReader xmlTextReader = new XmlTextReader(fileStream);
				while (xmlTextReader.Read())
				{
					if (xmlTextReader.NodeType == XmlNodeType.Element)
					{
						if (xmlTextReader.Name.Equals("model", StringComparison.OrdinalIgnoreCase))
						{
							if (xmlTextReader.IsEmptyElement)
							{
								throw new Exception("\"model\" tag is empty in \"" + iFilename + '"');
							}
							while (xmlTextReader.Read())
							{
								if (xmlTextReader.NodeType == XmlNodeType.Text)
								{
									this.ModelFilename = xmlTextReader.Value.ToLowerInvariant() + ".xnb";
									this.ModelFilename = Path.Combine(Path.GetDirectoryName(iFilename), this.ModelFilename);
									this.ModelHashSum = null;
									break;
								}
								if (xmlTextReader.NodeType == XmlNodeType.EndElement && xmlTextReader.Name.Equals("model"))
								{
									throw new Exception("\"model\" tag is empty in \"" + iFilename + '"');
								}
							}
						}
						else if (xmlTextReader.Name.Equals("ruleset", StringComparison.OrdinalIgnoreCase))
						{
							string attribute = xmlTextReader.GetAttribute("type");
							if (attribute.Equals("versus", StringComparison.OrdinalIgnoreCase))
							{
								this.mRulesetType = Rulesets.None;
							}
							else if (!string.IsNullOrEmpty(attribute))
							{
								this.mRulesetType = (Rulesets)Enum.Parse(typeof(Rulesets), attribute, true);
							}
						}
					}
				}
				if (string.IsNullOrEmpty(this.ModelFilename))
				{
					throw new Exception("\"model\" tag missing in \"" + iFilename + '"');
				}
				fileStream.Close();
			}

			// Token: 0x0600134A RID: 4938 RVA: 0x00076CA0 File Offset: 0x00074EA0
			internal void ComputeHashSums(SHA256 iSHA)
			{
				FileStream fileStream = File.OpenRead(this.ScriptFilename);
				this.ScriptHashSum = iSHA.ComputeHash(fileStream);
				fileStream.Close();
				fileStream = File.OpenRead(this.ModelFilename);
				this.ModelHashSum = iSHA.ComputeHash(fileStream);
				fileStream.Close();
			}

			// Token: 0x170004EB RID: 1259
			// (get) Token: 0x0600134B RID: 4939 RVA: 0x00076CEB File Offset: 0x00074EEB
			// (set) Token: 0x0600134C RID: 4940 RVA: 0x00076CF3 File Offset: 0x00074EF3
			public string ScriptFilename { get; private set; }

			// Token: 0x170004EC RID: 1260
			// (get) Token: 0x0600134D RID: 4941 RVA: 0x00076CFC File Offset: 0x00074EFC
			// (set) Token: 0x0600134E RID: 4942 RVA: 0x00076D04 File Offset: 0x00074F04
			public byte[] ScriptHashSum { get; private set; }

			// Token: 0x170004ED RID: 1261
			// (get) Token: 0x0600134F RID: 4943 RVA: 0x00076D0D File Offset: 0x00074F0D
			// (set) Token: 0x06001350 RID: 4944 RVA: 0x00076D15 File Offset: 0x00074F15
			public string ModelFilename { get; private set; }

			// Token: 0x170004EE RID: 1262
			// (get) Token: 0x06001351 RID: 4945 RVA: 0x00076D1E File Offset: 0x00074F1E
			// (set) Token: 0x06001352 RID: 4946 RVA: 0x00076D26 File Offset: 0x00074F26
			public byte[] ModelHashSum { get; private set; }

			// Token: 0x170004EF RID: 1263
			// (get) Token: 0x06001353 RID: 4947 RVA: 0x00076D2F File Offset: 0x00074F2F
			public Rulesets RulesetType
			{
				get
				{
					return this.mRulesetType;
				}
			}

			// Token: 0x040014EB RID: 5355
			private Rulesets mRulesetType;
		}
	}
}
