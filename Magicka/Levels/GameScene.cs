using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels.Triggers;
using Magicka.Levels.Triggers.Actions;
using Magicka.Levels.Versus;
using Magicka.Network;
using Magicka.PathFinding;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using PolygonHead.Models;
using PolygonHead.ParticleEffects;

namespace Magicka.Levels
{
	// Token: 0x02000554 RID: 1364
	public class GameScene : IPreRenderRenderer, IDisposable
	{
		// Token: 0x06002885 RID: 10373 RVA: 0x0013DA58 File Offset: 0x0013BC58
		internal GameScene(Level iLevel, string iFileName, XmlDocument iInput, ContentManager iContent, VersusRuleset.Settings iSettings)
		{
			this.mContent = new SharedContentManager(iContent.ServiceProvider);
			try
			{
				using (FileStream fileStream = File.OpenRead(iFileName))
				{
					this.mShaHash = SHA256.Create().ComputeHash(fileStream);
				}
			}
			catch (Exception)
			{
				this.mShaHash = new byte[32];
			}
			this.mPlayState = iLevel.PlayState;
			this.mLevel = iLevel;
			this.mLastGeneratedEffectID = 0;
			float num = -50f;
			string directoryName = Path.GetDirectoryName(iFileName);
			this.mName = Path.GetFileNameWithoutExtension(iFileName).ToLowerInvariant();
			this.mId = this.mName.GetHashCodeCustom();
			XmlNode xmlNode = null;
			for (int i = 0; i < iInput.ChildNodes.Count; i++)
			{
				if (iInput.ChildNodes[i].Name.Equals("Scene", StringComparison.OrdinalIgnoreCase))
				{
					for (int j = 0; j < iInput.ChildNodes[i].Attributes.Count; j++)
					{
						XmlAttribute xmlAttribute = iInput.ChildNodes[i].Attributes[j];
						if (xmlAttribute.Name.Equals("forcenavmesh", StringComparison.OrdinalIgnoreCase))
						{
							this.mForceNavMesh = bool.Parse(xmlAttribute.Value);
						}
						else if (xmlAttribute.Name.Equals("forcecamera", StringComparison.OrdinalIgnoreCase))
						{
							this.mForceCamera = bool.Parse(xmlAttribute.Value);
						}
					}
					xmlNode = iInput.ChildNodes[i];
					break;
				}
			}
			if (xmlNode == null)
			{
				throw new Exception("No Scene node found in scene XML!");
			}
			this.mTriggers = new SortedList<int, Trigger>();
			for (int k = 0; k < xmlNode.ChildNodes.Count; k++)
			{
				XmlNode xmlNode2 = xmlNode.ChildNodes[k];
				if (!(xmlNode2 is XmlComment))
				{
					if (xmlNode2.Name.Equals("Indoor", StringComparison.OrdinalIgnoreCase))
					{
						this.mIndoor = bool.Parse(xmlNode2.InnerText);
					}
					else if (xmlNode2.Name.Equals("Magnify", StringComparison.OrdinalIgnoreCase))
					{
						iLevel.PlayState.Camera.DefaultMagnification = float.Parse(xmlNode2.InnerText, CultureInfo.InvariantCulture.NumberFormat);
					}
					else if (xmlNode2.Name.Equals("reverb", StringComparison.OrdinalIgnoreCase))
					{
						for (int l = 0; l < xmlNode2.Attributes.Count; l++)
						{
							XmlAttribute xmlAttribute2 = xmlNode2.Attributes[l];
							if (xmlAttribute2.Name.Equals("roomType", StringComparison.OrdinalIgnoreCase))
							{
								this.mRoomType = (RoomType)Enum.Parse(typeof(RoomType), xmlAttribute2.Value, true);
							}
							else if (xmlAttribute2.Name.Equals("mix", StringComparison.OrdinalIgnoreCase))
							{
								this.mReverbMix = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
						}
					}
					else if (xmlNode2.Name.Equals("Ruleset", StringComparison.OrdinalIgnoreCase))
					{
						for (int m = 0; m < xmlNode2.Attributes.Count; m++)
						{
							XmlAttribute xmlAttribute3 = xmlNode2.Attributes[m];
							string a;
							if ((a = xmlAttribute3.Name.ToLowerInvariant()) != null && a == "type")
							{
								if (xmlAttribute3.Value.Equals("survival", StringComparison.OrdinalIgnoreCase))
								{
									this.mRuleset = new SurvivalRuleset(this, xmlNode2);
								}
								else if (xmlAttribute3.Value.Equals("timedobjective", StringComparison.OrdinalIgnoreCase))
								{
									this.mRuleset = new TimedObjectiveRuleset(this, xmlNode2);
								}
								else if (xmlAttribute3.Value.Equals("versus", StringComparison.OrdinalIgnoreCase))
								{
									if (iSettings is DeathMatch.Settings)
									{
										this.mRuleset = new DeathMatch(this, xmlNode2, iSettings as DeathMatch.Settings);
									}
									else if (iSettings is Brawl.Settings)
									{
										this.mRuleset = new Brawl(this, xmlNode2, iSettings as Brawl.Settings);
									}
									else if (iSettings is Pyrite.Settings)
									{
										this.mRuleset = new Pyrite(this, xmlNode2, iSettings as Pyrite.Settings);
									}
									else if (iSettings is Krietor.Settings)
									{
										this.mRuleset = new Krietor(this, xmlNode2, iSettings as Krietor.Settings);
									}
									else if (iSettings is King.Settings)
									{
										this.mRuleset = new King(this, xmlNode2, iSettings as King.Settings);
									}
								}
							}
						}
					}
					else if (this.RuleSet != null && xmlNode2.Name.Equals("Wave", StringComparison.OrdinalIgnoreCase) && this.mRuleset is SurvivalRuleset)
					{
						((SurvivalRuleset)this.mRuleset).ReadWave(xmlNode2);
					}
					else if (xmlNode2.Name.Equals("Fog", StringComparison.OrdinalIgnoreCase))
					{
						this.mFog.Enabled = true;
						for (int n = 0; n < xmlNode2.Attributes.Count; n++)
						{
							XmlAttribute xmlAttribute4 = xmlNode2.Attributes[n];
							if (xmlAttribute4.Name.Equals("Start", StringComparison.OrdinalIgnoreCase))
							{
								this.mFog.Start = float.Parse(xmlAttribute4.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
							if (xmlAttribute4.Name.Equals("End", StringComparison.OrdinalIgnoreCase))
							{
								this.mFog.End = float.Parse(xmlAttribute4.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
							if (xmlAttribute4.Name.Equals("Color", StringComparison.OrdinalIgnoreCase))
							{
								string[] array = xmlAttribute4.Value.Split(new char[]
								{
									','
								});
								this.mFog.Color.X = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
								this.mFog.Color.Y = float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
								this.mFog.Color.Z = float.Parse(array[2], CultureInfo.InvariantCulture.NumberFormat);
								if (array.Length > 3)
								{
									this.mFog.Color.W = float.Parse(array[3], CultureInfo.InvariantCulture.NumberFormat);
								}
								else
								{
									this.mFog.Color.W = 1f;
								}
							}
						}
					}
					else if (xmlNode2.Name.Equals("Filter", StringComparison.OrdinalIgnoreCase))
					{
						for (int num2 = 0; num2 < xmlNode2.Attributes.Count; num2++)
						{
							XmlAttribute xmlAttribute5 = xmlNode2.Attributes[num2];
							if (xmlAttribute5.Name.Equals("Brightness", StringComparison.OrdinalIgnoreCase))
							{
								this.mBrightness = float.Parse(xmlAttribute5.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
							if (xmlAttribute5.Name.Equals("Contrast", StringComparison.OrdinalIgnoreCase))
							{
								this.mContrast = float.Parse(xmlAttribute5.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
							if (xmlAttribute5.Name.Equals("Saturation", StringComparison.OrdinalIgnoreCase))
							{
								this.mSaturation = float.Parse(xmlAttribute5.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
						}
					}
					else if (xmlNode2.Name.Equals("Bloom", StringComparison.OrdinalIgnoreCase))
					{
						for (int num3 = 0; num3 < xmlNode2.Attributes.Count; num3++)
						{
							XmlAttribute xmlAttribute6 = xmlNode2.Attributes[num3];
							if (xmlAttribute6.Name.Equals("Threshold", StringComparison.OrdinalIgnoreCase))
							{
								this.mBloomThreshold = float.Parse(xmlAttribute6.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
							if (xmlAttribute6.Name.Equals("Multiplyer", StringComparison.OrdinalIgnoreCase) || xmlAttribute6.Name.Equals("Multiplier", StringComparison.OrdinalIgnoreCase))
							{
								this.mBloomMultiplier = float.Parse(xmlAttribute6.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
							if (xmlAttribute6.Name.Equals("Blur", StringComparison.OrdinalIgnoreCase))
							{
								this.mBlurSigma = float.Parse(xmlAttribute6.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
						}
					}
					else if (xmlNode2.Name.Equals("SkyMap", StringComparison.OrdinalIgnoreCase))
					{
						this.mSkyMapColor = new Vector3(1f);
						for (int num4 = 0; num4 < xmlNode2.Attributes.Count; num4++)
						{
							XmlAttribute xmlAttribute7 = xmlNode2.Attributes[num4];
							if (xmlAttribute7.Name.Equals("color"))
							{
								string[] array2 = xmlAttribute7.Value.Split(new char[]
								{
									','
								});
								this.mSkyMapColor.X = float.Parse(array2[0], CultureInfo.InvariantCulture.NumberFormat);
								this.mSkyMapColor.Y = float.Parse(array2[1], CultureInfo.InvariantCulture.NumberFormat);
								this.mSkyMapColor.Z = float.Parse(array2[2], CultureInfo.InvariantCulture.NumberFormat);
							}
						}
						this.mSkyMapFileName = Path.Combine(directoryName, xmlNode2.InnerText);
					}
					else if (xmlNode2.Name.Equals("RimLight", StringComparison.OrdinalIgnoreCase))
					{
						for (int num5 = 0; num5 < xmlNode2.Attributes.Count; num5++)
						{
							XmlAttribute xmlAttribute8 = xmlNode2.Attributes[num5];
							if (xmlAttribute8.Name.Equals("Glow", StringComparison.OrdinalIgnoreCase))
							{
								this.mRimLightGlow = float.Parse(xmlAttribute8.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
							if (xmlAttribute8.Name.Equals("Power", StringComparison.OrdinalIgnoreCase))
							{
								this.mRimLightPower = float.Parse(xmlAttribute8.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
							if (xmlAttribute8.Name.Equals("Bias", StringComparison.OrdinalIgnoreCase))
							{
								this.mRimLightBias = float.Parse(xmlAttribute8.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
						}
					}
					else if (xmlNode2.Name.Equals("Clouds", StringComparison.OrdinalIgnoreCase))
					{
						for (int num6 = 0; num6 < xmlNode2.Attributes.Count; num6++)
						{
							XmlAttribute xmlAttribute9 = xmlNode2.Attributes[num6];
							if (xmlAttribute9.Name.Equals("Intensity", StringComparison.OrdinalIgnoreCase))
							{
								this.mCloudIntensity = float.Parse(xmlAttribute9.Value, CultureInfo.InvariantCulture.NumberFormat);
							}
							else if (xmlAttribute9.Name.Equals("Scale", StringComparison.OrdinalIgnoreCase))
							{
								string[] array3 = xmlAttribute9.Value.Split(new char[]
								{
									','
								});
								if (array3.Length == 1)
								{
									this.mCloudScale = new Vector2(float.Parse(array3[0], CultureInfo.InvariantCulture.NumberFormat));
								}
								else
								{
									if (array3.Length != 2)
									{
										throw new Exception("Invalid Syntax in; Clouds, Scale!");
									}
									this.mCloudScale = new Vector2(float.Parse(array3[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(array3[1], CultureInfo.InvariantCulture.NumberFormat));
								}
							}
							else if (xmlAttribute9.Name.Equals("Movement", StringComparison.OrdinalIgnoreCase))
							{
								string[] array4 = xmlAttribute9.Value.Split(new char[]
								{
									','
								});
								if (array4.Length != 2)
								{
									throw new Exception("Invalid Syntax in; Clouds, Scale!");
								}
								this.mCloudMovement = new Vector2(float.Parse(array4[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(array4[1], CultureInfo.InvariantCulture.NumberFormat));
							}
							else if (xmlAttribute9.Name.Equals("Texture", StringComparison.OrdinalIgnoreCase))
							{
								this.mCloudTexture = iContent.Load<Texture2D>(Path.Combine(directoryName, xmlAttribute9.Value));
							}
							else if (xmlAttribute9.Name.Equals("WorldRelative", StringComparison.InvariantCultureIgnoreCase))
							{
								this.mCloudWorldRelative = bool.Parse(xmlAttribute9.Value);
							}
						}
					}
					else if (xmlNode2.Name.Equals("Model", StringComparison.OrdinalIgnoreCase))
					{
						this.mModelName = Path.Combine(directoryName, xmlNode2.InnerText);
					}
					else if (xmlNode2.Name.Equals("Trigger", StringComparison.OrdinalIgnoreCase))
					{
						Trigger trigger = Trigger.Read(this, xmlNode2);
						this.mTriggers.Add(trigger.ID, trigger);
					}
					else if (xmlNode2.Name.Equals("OnInteract", StringComparison.OrdinalIgnoreCase))
					{
						Interactable interactable = new Interactable(xmlNode2, this);
						this.mTriggers.Add(interactable.ID, interactable);
					}
					else
					{
						if (!xmlNode2.Name.Equals("KillPlanePosition", StringComparison.OrdinalIgnoreCase))
						{
							throw new NotImplementedException(xmlNode2.Name);
						}
						num = float.Parse(xmlNode2.InnerText, CultureInfo.InvariantCulture.NumberFormat);
					}
				}
			}
			this.mTriggeredActions = new List<Action>();
			this.mSwaySurfaceFormat = SurfaceFormat.HalfVector4;
			this.mUseVertexTexturing = Game.Instance.GraphicsDevice.CreationParameters.Adapter.CheckDeviceFormat(Game.Instance.GraphicsDevice.GraphicsDeviceCapabilities.DeviceType, Game.Instance.GraphicsDevice.DisplayMode.Format, TextureUsage.None, QueryUsages.VertexTexture, ResourceType.Texture2D, this.mSwaySurfaceFormat);
			if (!this.mUseVertexTexturing)
			{
				this.mSwaySurfaceFormat = SurfaceFormat.Vector4;
				this.mUseVertexTexturing = Game.Instance.GraphicsDevice.CreationParameters.Adapter.CheckDeviceFormat(Game.Instance.GraphicsDevice.GraphicsDeviceCapabilities.DeviceType, Game.Instance.GraphicsDevice.DisplayMode.Format, TextureUsage.None, QueryUsages.VertexTexture, ResourceType.Texture2D, this.mSwaySurfaceFormat);
			}
			if (this.mUseVertexTexturing)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					this.mSwayTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/SwayTexture");
					this.mCharacterDisplacementTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/CharacterDisplacement");
					this.mSwayEffect = new SwayEffect(Game.Instance.GraphicsDevice, Game.Instance.Content);
					this.mSwayVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
				}
			}
			this.mKillPlane = new CollisionSkin();
			Primitive prim = new JigLibX.Geometry.Plane(Vector3.Up, -num);
			this.mKillPlane.AddPrimitive(prim, 1, default(MaterialProperties));
			this.mKillPlane.callbackFn += this.mKillPlane_callbackFn;
		}

		// Token: 0x1700097F RID: 2431
		// (get) Token: 0x06002886 RID: 10374 RVA: 0x0013E9FC File Offset: 0x0013CBFC
		public byte[] ShaHash
		{
			get
			{
				return this.mShaHash;
			}
		}

		// Token: 0x06002887 RID: 10375 RVA: 0x0013EA04 File Offset: 0x0013CC04
		internal void AddEffect(int iId, ref LevelModel.VisualEffectStorage iEffect)
		{
			if (this.mEffects.ContainsKey(iId))
			{
				VisualEffect effect = this.mEffects[iId].Effect;
				if (effect.IsActive)
				{
					return;
				}
			}
			GameScene.EffectStorage value;
			value.Effect = EffectManager.Instance.GetEffect(iEffect.Effect);
			default(Vector3).Y = 1f;
			value.Transform = iEffect.Transform;
			value.Effect.Start(ref value.Transform);
			value.Animation = null;
			value.Range = iEffect.Range;
			this.mEffects[iId] = value;
		}

		// Token: 0x06002888 RID: 10376 RVA: 0x0013EAA8 File Offset: 0x0013CCA8
		internal int GetGeneratedEffectID()
		{
			int num = this.mLastGeneratedEffectID + 1;
			while (this.mEffects.ContainsKey(num) && num != 0)
			{
				num++;
			}
			this.mLastGeneratedEffectID = num;
			return num;
		}

		// Token: 0x06002889 RID: 10377 RVA: 0x0013EAE0 File Offset: 0x0013CCE0
		public void StartEffect(int iId)
		{
			GameScene.EffectStorage value;
			if (!this.mEffects.TryGetValue(iId, out value))
			{
				return;
			}
			if (value.Effect.IsActive)
			{
				return;
			}
			value.Effect.Start(ref value.Effect.Transform);
			this.mEffects[iId] = value;
		}

		// Token: 0x0600288A RID: 10378 RVA: 0x0013EB34 File Offset: 0x0013CD34
		public bool StopEffect(int iId)
		{
			GameScene.EffectStorage value;
			if (!this.mEffects.TryGetValue(iId, out value))
			{
				return false;
			}
			value.Effect.Stop();
			this.mEffects[iId] = value;
			return true;
		}

		// Token: 0x0600288B RID: 10379 RVA: 0x0013EB70 File Offset: 0x0013CD70
		private bool mKillPlane_callbackFn(CollisionSkin skin0, int prim0, CollisionSkin skin1, int prim1)
		{
			if (skin1.Owner != null)
			{
				if (skin1.Owner.Tag is Magicka.GameLogic.Entities.Character)
				{
					(skin1.Owner.Tag as Magicka.GameLogic.Entities.Character).Terminate(true, true, false);
				}
				else if (skin1.Owner.Tag is Entity)
				{
					(skin1.Owner.Tag as Entity).Kill();
				}
			}
			return false;
		}

		// Token: 0x17000980 RID: 2432
		// (get) Token: 0x0600288C RID: 10380 RVA: 0x0013EBD9 File Offset: 0x0013CDD9
		// (set) Token: 0x0600288D RID: 10381 RVA: 0x0013EBE1 File Offset: 0x0013CDE1
		public float LightTargetIntensity
		{
			get
			{
				return this.mTargetLightIntensity;
			}
			set
			{
				this.mTargetLightIntensity = value;
			}
		}

		// Token: 0x17000981 RID: 2433
		// (get) Token: 0x0600288E RID: 10382 RVA: 0x0013EBEA File Offset: 0x0013CDEA
		public GameScene.LightSettings[] DirectionalLightSettings
		{
			get
			{
				return this.mDirectionalLightSettings;
			}
		}

		// Token: 0x0600288F RID: 10383 RVA: 0x0013EBF2 File Offset: 0x0013CDF2
		public TriggerArea GetTriggerArea(int iAreaID)
		{
			return this.mModel.TriggerAreas[iAreaID];
		}

		// Token: 0x06002890 RID: 10384 RVA: 0x0013EC05 File Offset: 0x0013CE05
		public bool TryGetTriggerArea(int iAreaID, out TriggerArea oArea)
		{
			return this.mModel.TriggerAreas.TryGetValue(iAreaID, out oArea);
		}

		// Token: 0x06002891 RID: 10385 RVA: 0x0013EC1C File Offset: 0x0013CE1C
		public bool LiquidSegmentIntersect(out float frac, out Vector3 pos, out Vector3 nrm, ref Segment seg, bool ignoreBackfaces, bool ignoreWater, bool ignoreIce)
		{
			frac = 0f;
			pos = default(Vector3);
			nrm = default(Vector3);
			foreach (Liquid liquid in this.Liquids)
			{
				if (liquid.SegmentIntersect(out frac, out pos, out nrm, ref seg, ignoreBackfaces, ignoreWater, ignoreIce))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002892 RID: 10386 RVA: 0x0013EC74 File Offset: 0x0013CE74
		public bool SegmentIntersect(out float oFrac, out Vector3 oPos, out Vector3 oNrm, Segment iSeg)
		{
			AnimatedLevelPart animatedLevelPart;
			return this.SegmentIntersect(out oFrac, out oPos, out oNrm, out animatedLevelPart, iSeg);
		}

		// Token: 0x06002893 RID: 10387 RVA: 0x0013EC90 File Offset: 0x0013CE90
		public bool SegmentIntersect(out float oFrac, out Vector3 oPos, out Vector3 oNrm, out AnimatedLevelPart oAnimatedLevelPart, Segment iSeg)
		{
			int num;
			return this.SegmentIntersect(out oFrac, out oPos, out oNrm, out oAnimatedLevelPart, out num, iSeg);
		}

		// Token: 0x06002894 RID: 10388 RVA: 0x0013ECAC File Offset: 0x0013CEAC
		public bool SegmentIntersect(out float oFrac, out Vector3 oPos, out Vector3 oNrm, out AnimatedLevelPart oAnimatedLevelPart, out int oPrim, Segment iSeg)
		{
			if (this.mModel != null)
			{
				return this.mModel.SegmentIntersect(out oFrac, out oPos, out oNrm, out oAnimatedLevelPart, out oPrim, iSeg);
			}
			oFrac = float.MaxValue;
			oPos = default(Vector3);
			oNrm = default(Vector3);
			oAnimatedLevelPart = null;
			oPrim = 0;
			return false;
		}

		// Token: 0x06002895 RID: 10389 RVA: 0x0013ECE9 File Offset: 0x0013CEE9
		public void UpdateAnimatedLevelParts(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mModel.UpdateAnimatedLevelParts(iDataChannel, iDeltaTime, this);
		}

		// Token: 0x06002896 RID: 10390 RVA: 0x0013ECFC File Offset: 0x0013CEFC
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mTargetLightIntensity < this.mLightIntensity)
			{
				this.mLightIntensity -= Math.Max(iDeltaTime, this.mTargetLightIntensity - this.mLightIntensity);
			}
			else
			{
				this.mLightIntensity += Math.Min(iDeltaTime, this.mTargetLightIntensity - this.mLightIntensity);
			}
			for (int i = 0; i < this.mDirectionalLightSettings.Length; i++)
			{
				this.mDirectionalLightSettings[i].Assign(this.mLightIntensity);
			}
			for (int j = 0; j < this.mSounds.Count; j++)
			{
				this.mSounds.Values[j].Update(this);
				Cue cue = this.mSounds.Values[j].Cue;
				if (cue.IsStopped || cue.IsStopping)
				{
					this.mSounds.RemoveAt(j);
					j--;
				}
			}
			for (int k = 0; k < this.mTriggers.Count; k++)
			{
				this.mTriggers.Values[k].Update(iDeltaTime);
			}
			if (this.mRuleset != null)
			{
				if (NetworkManager.Instance.State == NetworkState.Client)
				{
					this.mRuleset.LocalUpdate(iDeltaTime, iDataChannel);
				}
				else
				{
					this.mRuleset.Update(iDeltaTime, iDataChannel);
				}
			}
			if (iDataChannel != DataChannel.None)
			{
				this.PlayState.Scene.AddPreRenderRenderers(iDataChannel, this);
			}
			this.mModel.Update(iDataChannel, iDeltaTime, this);
			for (int l = 0; l < this.mEffects.Count; l++)
			{
				GameScene.EffectStorage value = this.mEffects.Values[l];
				if (value.Effect.IsActive)
				{
					float num = 1f;
					Matrix transform;
					if (value.Animation != null)
					{
						transform = value.Animation.AbsoluteTransform;
						Matrix.Multiply(ref value.Transform, ref transform, out transform);
					}
					else
					{
						transform = value.Transform;
					}
					if (value.Range > 0f)
					{
						Segment segment;
						segment.Origin = transform.Translation;
						Vector3 forward = transform.Forward;
						Vector3.Multiply(ref forward, value.Range, out segment.Delta);
						float num2;
						if (this.mModel.SegmentIntersect(out num2, segment))
						{
							num = num2;
						}
						List<Shield> shields = this.PlayState.EntityManager.Shields;
						foreach (Shield shield in shields)
						{
							Vector3 vector;
							Vector3 vector2;
							if (shield.Body.CollisionSkin.SegmentIntersect(out num2, out vector, out vector2, segment) && num2 < num)
							{
								num = num2;
							}
						}
						MagickaMath.UniformMatrixScale(ref transform, num);
					}
					value.Effect.Transform = transform;
					value.Effect.Update(iDeltaTime);
					this.mEffects[this.mEffects.Keys[l]] = value;
				}
			}
			this.ClearTriggerAreas();
		}

		// Token: 0x06002897 RID: 10391 RVA: 0x0013F000 File Offset: 0x0013D200
		public void ChangeAmbientSoundVolume(int iID, float iVolume)
		{
			AudioLocator audioLocator;
			if (this.mSounds.TryGetValue(iID, out audioLocator))
			{
				audioLocator.Cue.SetVariable(GameScene.VOLUME_VAR_NAME, iVolume);
				return;
			}
			GameScene.GlobalAudio value;
			if (!this.mGlobalSounds.TryGetValue(iID, out value))
			{
				return;
			}
			value.SetVolume(iVolume);
			this.mGlobalSounds[iID] = value;
		}

		// Token: 0x06002898 RID: 10392 RVA: 0x0013F058 File Offset: 0x0013D258
		public void PlayAmbientSound(int iID, Banks iBank, int iCue, float iVolume, int iLocator, float iRadius, bool iApply3D)
		{
			if (!this.mSounds.ContainsKey(iID))
			{
				AudioLocator value = new AudioLocator(iID, iBank, iCue, iVolume, iLocator, iRadius, iApply3D);
				value.Play();
				this.mSounds.Add(value.ID, value);
			}
		}

		// Token: 0x06002899 RID: 10393 RVA: 0x0013F0A0 File Offset: 0x0013D2A0
		public void PlayAmbientSound(int iID, Banks iBank, int iCue, float iVolume)
		{
			GameScene.GlobalAudio value;
			if (this.mGlobalSounds.TryGetValue(iID, out value))
			{
				value.SetVolume(iVolume);
				value.Play();
				this.mGlobalSounds[iID] = value;
				return;
			}
			value = new GameScene.GlobalAudio(iBank, iCue, iVolume);
			value.Play();
			this.mGlobalSounds.Add(iID, value);
		}

		// Token: 0x0600289A RID: 10394 RVA: 0x0013F100 File Offset: 0x0013D300
		public void StopSound(int iID, bool mInstant)
		{
			AudioLocator audioLocator;
			if (this.mSounds.TryGetValue(iID, out audioLocator))
			{
				if (mInstant)
				{
					audioLocator.Stop(AudioStopOptions.Immediate);
				}
				else
				{
					audioLocator.Stop(AudioStopOptions.AsAuthored);
				}
				this.mSounds.Remove(iID);
				return;
			}
			GameScene.GlobalAudio globalAudio;
			if (!this.mGlobalSounds.TryGetValue(iID, out globalAudio))
			{
				return;
			}
			if (mInstant)
			{
				globalAudio.Stop(AudioStopOptions.Immediate);
				return;
			}
			globalAudio.Stop(AudioStopOptions.AsAuthored);
		}

		// Token: 0x0600289B RID: 10395 RVA: 0x0013F168 File Offset: 0x0013D368
		private void ClearTriggerAreas()
		{
			foreach (TriggerArea triggerArea in this.mModel.TriggerAreas.Values)
			{
				triggerArea.Reset();
			}
			(this.mModel.TriggerAreas[TriggerArea.ANYID] as AnyTriggerArea).Count(this.PlayState.EntityManager);
		}

		// Token: 0x0600289C RID: 10396 RVA: 0x0013F1F0 File Offset: 0x0013D3F0
		public void GetLocator(int iId, out Locator oLocator)
		{
			if (iId == GameScene.ANYAREA)
			{
				iId = this.mRuleset.GetAnyArea();
			}
			if (this.mModel.Locators.TryGetValue(iId, out oLocator))
			{
				return;
			}
			Matrix identity = Matrix.Identity;
			foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
			{
				if (animatedLevelPart.TryGetLocator(iId, ref identity, out oLocator))
				{
					return;
				}
			}
			oLocator = default(Locator);
			oLocator.Transform = Matrix.Identity;
		}

		// Token: 0x0600289D RID: 10397 RVA: 0x0013F298 File Offset: 0x0013D498
		public void GetLocator(int iId, out Matrix oLocator)
		{
			if (iId == GameScene.ANYAREA)
			{
				iId = this.mRuleset.GetAnyArea();
			}
			Locator locator;
			if (this.mModel.Locators.TryGetValue(iId, out locator))
			{
				oLocator = locator.Transform;
				return;
			}
			TriggerArea triggerArea;
			if (this.mModel.TriggerAreas.TryGetValue(iId, out triggerArea))
			{
				Matrix.CreateRotationY(MagickaMath.RandomBetween(0f, 6.2831855f), out oLocator);
				oLocator.Translation = triggerArea.GetRandomLocation();
				return;
			}
			Matrix identity = Matrix.Identity;
			foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
			{
				if (animatedLevelPart.TryGetLocator(iId, ref identity, out locator))
				{
					oLocator = locator.Transform;
					return;
				}
			}
			throw new Exception(string.Concat(new object[]
			{
				"No area with key \"",
				iId,
				"\" in scene ",
				this.mName
			}));
		}

		// Token: 0x0600289E RID: 10398 RVA: 0x0013F3BC File Offset: 0x0013D5BC
		public bool TryGetLocator(int iId, out Matrix oLocator)
		{
			Locator locator;
			if (this.mModel.Locators.TryGetValue(iId, out locator))
			{
				oLocator = locator.Transform;
				return true;
			}
			Matrix identity = Matrix.Identity;
			foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
			{
				if (animatedLevelPart.TryGetLocator(iId, ref identity, out locator))
				{
					oLocator = locator.Transform;
					return true;
				}
			}
			oLocator = Matrix.Identity;
			return false;
		}

		// Token: 0x17000982 RID: 2434
		// (get) Token: 0x0600289F RID: 10399 RVA: 0x0013F46C File Offset: 0x0013D66C
		public CollisionSkin CollisionSkin
		{
			get
			{
				return this.mModel.CollisionSkin;
			}
		}

		// Token: 0x17000983 RID: 2435
		// (get) Token: 0x060028A0 RID: 10400 RVA: 0x0013F479 File Offset: 0x0013D679
		public bool ForceNavMesh
		{
			get
			{
				return this.mForceNavMesh;
			}
		}

		// Token: 0x17000984 RID: 2436
		// (get) Token: 0x060028A1 RID: 10401 RVA: 0x0013F481 File Offset: 0x0013D681
		public bool ForceCamera
		{
			get
			{
				return this.mForceCamera;
			}
		}

		// Token: 0x17000985 RID: 2437
		// (get) Token: 0x060028A2 RID: 10402 RVA: 0x0013F489 File Offset: 0x0013D689
		public bool Indoors
		{
			get
			{
				return this.mIndoor;
			}
		}

		// Token: 0x17000986 RID: 2438
		// (get) Token: 0x060028A3 RID: 10403 RVA: 0x0013F491 File Offset: 0x0013D691
		// (set) Token: 0x060028A4 RID: 10404 RVA: 0x0013F499 File Offset: 0x0013D699
		public Level Level
		{
			get
			{
				return this.mLevel;
			}
			set
			{
				this.mLevel = value;
			}
		}

		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x060028A5 RID: 10405 RVA: 0x0013F4A2 File Offset: 0x0013D6A2
		public Scene Scene
		{
			get
			{
				return this.mLevel.PlayState.Scene;
			}
		}

		// Token: 0x17000988 RID: 2440
		// (get) Token: 0x060028A6 RID: 10406 RVA: 0x0013F4B4 File Offset: 0x0013D6B4
		public IRuleset RuleSet
		{
			get
			{
				return this.mRuleset;
			}
		}

		// Token: 0x17000989 RID: 2441
		// (get) Token: 0x060028A7 RID: 10407 RVA: 0x0013F4BC File Offset: 0x0013D6BC
		public string Name
		{
			get
			{
				return this.mName;
			}
		}

		// Token: 0x1700098A RID: 2442
		// (get) Token: 0x060028A8 RID: 10408 RVA: 0x0013F4C4 File Offset: 0x0013D6C4
		public int ID
		{
			get
			{
				return this.mId;
			}
		}

		// Token: 0x1700098B RID: 2443
		// (get) Token: 0x060028A9 RID: 10409 RVA: 0x0013F4CC File Offset: 0x0013D6CC
		public BiTreeModel Model
		{
			get
			{
				return this.mModel.Model;
			}
		}

		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x060028AA RID: 10410 RVA: 0x0013F4D9 File Offset: 0x0013D6D9
		public LevelModel LevelModel
		{
			get
			{
				return this.mModel;
			}
		}

		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x060028AB RID: 10411 RVA: 0x0013F4E1 File Offset: 0x0013D6E1
		public Liquid[] Liquids
		{
			get
			{
				return this.mLiquids;
			}
		}

		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x060028AC RID: 10412 RVA: 0x0013F4E9 File Offset: 0x0013D6E9
		public SortedList<int, Trigger> Triggers
		{
			get
			{
				return this.mTriggers;
			}
		}

		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x060028AD RID: 10413 RVA: 0x0013F4F1 File Offset: 0x0013D6F1
		internal NavMesh NavMesh
		{
			get
			{
				return this.mModel.NavMesh;
			}
		}

		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x060028AE RID: 10414 RVA: 0x0013F4FE File Offset: 0x0013D6FE
		public PlayState PlayState
		{
			get
			{
				return this.mPlayState;
			}
		}

		// Token: 0x17000991 RID: 2449
		// (get) Token: 0x060028AF RID: 10415 RVA: 0x0013F506 File Offset: 0x0013D706
		public RoomType RoomType
		{
			get
			{
				return this.mRoomType;
			}
		}

		// Token: 0x17000992 RID: 2450
		// (get) Token: 0x060028B0 RID: 10416 RVA: 0x0013F50E File Offset: 0x0013D70E
		public float ReverbMix
		{
			get
			{
				return this.mReverbMix;
			}
		}

		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x060028B1 RID: 10417 RVA: 0x0013F516 File Offset: 0x0013D716
		public float Brightness
		{
			get
			{
				return this.mBrightness;
			}
		}

		// Token: 0x17000994 RID: 2452
		// (get) Token: 0x060028B2 RID: 10418 RVA: 0x0013F51E File Offset: 0x0013D71E
		public float Contrast
		{
			get
			{
				return this.mContrast;
			}
		}

		// Token: 0x17000995 RID: 2453
		// (get) Token: 0x060028B3 RID: 10419 RVA: 0x0013F526 File Offset: 0x0013D726
		public float Saturation
		{
			get
			{
				return this.mSaturation;
			}
		}

		// Token: 0x17000996 RID: 2454
		// (get) Token: 0x060028B4 RID: 10420 RVA: 0x0013F52E File Offset: 0x0013D72E
		public Texture2D SkyMap
		{
			get
			{
				return this.mSkyMap;
			}
		}

		// Token: 0x17000997 RID: 2455
		// (get) Token: 0x060028B5 RID: 10421 RVA: 0x0013F536 File Offset: 0x0013D736
		public Vector3 SkyMapColor
		{
			get
			{
				return this.mSkyMapColor;
			}
		}

		// Token: 0x060028B6 RID: 10422 RVA: 0x0013F540 File Offset: 0x0013D740
		public void Destroy(bool iSaveNPCs)
		{
			TutorialManager.Instance.HideAll();
			EffectManager.Instance.Clear();
			PhysicsManager.Instance.Clear();
			AudioManager.Instance.ClearMusicFocus();
			foreach (AudioLocator audioLocator in this.mSounds.Values)
			{
				if (iSaveNPCs)
				{
					audioLocator.Pause();
				}
				else
				{
					audioLocator.Stop(AudioStopOptions.AsAuthored);
				}
			}
			foreach (GameScene.GlobalAudio globalAudio in this.mGlobalSounds.Values)
			{
				if (iSaveNPCs)
				{
					globalAudio.Pause();
				}
				else
				{
					globalAudio.Stop(AudioStopOptions.AsAuthored);
				}
			}
			this.mSavedAnimations.Clear();
			foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
			{
				animatedLevelPart.AddStateTo(this.mSavedAnimations);
			}
			this.mSavedEntities.Clear();
			this.PlayState.EntityManager.ClearAndStore(iSaveNPCs ? this.mSavedEntities : null);
			if (this.mSwayTarget != null && !this.mSwayTarget.IsDisposed)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					this.mSwayTarget.Dispose();
				}
			}
			lock (Game.Instance.GraphicsDevice)
			{
				foreach (Light light in this.mModel.Lights.Values)
				{
					light.Disable();
					light.DisposeShadowMap();
				}
				Vector3 vector = default(Vector3);
				Vector3 vector2 = default(Vector3);
				this.mPlayState.Scene.UpdateLights(DataChannel.None, 0f, ref vector, ref vector2);
			}
		}

		// Token: 0x060028B7 RID: 10423 RVA: 0x0013F790 File Offset: 0x0013D990
		public void UnloadContent()
		{
			this.mContent.Unload();
		}

		// Token: 0x060028B8 RID: 10424 RVA: 0x0013F7A0 File Offset: 0x0013D9A0
		public void LoadLevel()
		{
			AnimatedLevelPart.ClearHandles();
			this.mModel = this.mContent.Load<LevelModel>(this.mModelName);
			List<Liquid> list = new List<Liquid>();
			list.AddRange(this.mModel.Waters);
			foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
			{
				animatedLevelPart.GetLiquids(list);
			}
			this.mLiquids = list.ToArray();
			if (!string.IsNullOrEmpty(this.mSkyMapFileName))
			{
				this.mSkyMap = this.mContent.Load<Texture2D>(this.mSkyMapFileName);
			}
		}

		// Token: 0x060028B9 RID: 10425 RVA: 0x0013F860 File Offset: 0x0013DA60
		public void Initialize(SpawnPoint iSpawnPoint, bool iClearAudio)
		{
			this.Initialize(iSpawnPoint, iClearAudio, null);
		}

		// Token: 0x060028BA RID: 10426 RVA: 0x0013F86C File Offset: 0x0013DA6C
		public unsafe void Initialize(SpawnPoint iSpawnPoint, bool iClearAudio, Action<float> reportbackAction)
		{
			this.mModel.Initialize(this.PlayState);
			ParticleSystem.Instance.Clear();
			ParticleLightBatcher.Instance.Clear();
			PointLightBatcher.Instance.Clear();
			foreach (Trigger trigger in this.mTriggers.Values)
			{
				trigger.Initialize();
			}
			this.RestoreSavedAnimations();
			List<DirectionalLight> list = new List<DirectionalLight>();
			foreach (Light light in this.mModel.Lights.Values)
			{
				DirectionalLight directionalLight = light as DirectionalLight;
				if (directionalLight != null)
				{
					list.Add(directionalLight);
				}
			}
			this.mDirectionalLightSettings = new GameScene.LightSettings[list.Count];
			for (int i = 0; i < this.mDirectionalLightSettings.Length; i++)
			{
				this.mDirectionalLightSettings[i].GetFromLight(list[i]);
			}
			if (this.mCloudIntensity > 1E-45f)
			{
				foreach (Light light2 in this.mModel.Lights.Values)
				{
					DirectionalLight directionalLight2 = light2 as DirectionalLight;
					if (directionalLight2 != null)
					{
						directionalLight2.ProjectedTextureIntensity = this.mCloudIntensity;
						directionalLight2.ProjectedTextureScale = this.mCloudScale;
						directionalLight2.ProjectedTexture = this.mCloudTexture;
					}
				}
			}
			DecalManager.Instance.Clear();
			ShadowBlobs.Instance.Initialize(this.PlayState.Scene);
			this.mModel.RegisterCollisionSkin();
			if (this.mRuleset != null)
			{
				this.mRuleset.Initialize();
			}
			PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.PlayState.Camera.CollisionSkin);
			RenderManager.Instance.Fog = this.mFog;
			RenderManager.Instance.SkyMap = this.mSkyMap;
			RenderManager.Instance.SkyMapColor = this.mSkyMapColor;
			RenderManager.Instance.Brightness = this.mBrightness;
			RenderManager.Instance.Contrast = this.mContrast;
			RenderManager.Instance.Saturation = this.mSaturation;
			RenderManager.Instance.BloomThreshold = this.mBloomThreshold;
			RenderManager.Instance.BloomMultiplier = this.mBloomMultiplier;
			RenderManager.Instance.BlurSigma = this.mBlurSigma;
			SkinnedModelDeferredEffect skinnedModelDeferredEffect = RenderManager.Instance.GetEffect(SkinnedModelDeferredEffect.TYPEHASH) as SkinnedModelDeferredEffect;
			skinnedModelDeferredEffect.RimLightGlow = this.mRimLightGlow;
			skinnedModelDeferredEffect.RimLightPower = this.mRimLightPower;
			skinnedModelDeferredEffect.RimLightBias = this.mRimLightBias;
			PlayState playState = this.PlayState;
			if (this.mSwayTarget == null || this.mSwayTarget.IsDisposed)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					this.mSwayTarget = new RenderTarget2D(Game.Instance.GraphicsDevice, 512, 512, 1, this.mSwaySurfaceFormat);
				}
			}
			foreach (AudioLocator audioLocator in this.mSounds.Values)
			{
				if (iClearAudio)
				{
					audioLocator.Stop(AudioStopOptions.Immediate);
				}
				else
				{
					audioLocator.Play();
				}
			}
			foreach (GameScene.GlobalAudio globalAudio in this.mGlobalSounds.Values)
			{
				if (iClearAudio)
				{
					globalAudio.Stop(AudioStopOptions.Immediate);
				}
				else
				{
					globalAudio.Play();
				}
			}
			if (iClearAudio)
			{
				this.mSounds.Clear();
				this.mGlobalSounds.Clear();
			}
			Vector2 vector = this.mVindDirection;
			vector.X *= 35.35534f;
			vector.Y *= 35.35534f;
			Vector2 vector2 = vector;
			vector2.X = vector2.Y;
			vector2.Y = -vector2.X;
			VertexPositionTexture[] array = new VertexPositionTexture[8];
			VertexPositionTexture vertexPositionTexture = default(VertexPositionTexture);
			vertexPositionTexture.Position.X = -35.35534f;
			vertexPositionTexture.Position.Z = -35.35534f;
			vertexPositionTexture.TextureCoordinate.X = 0f;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[0] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 35.35534f;
			vertexPositionTexture.Position.Z = -35.35534f;
			vertexPositionTexture.TextureCoordinate.X = 0.25f;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[1] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 35.35534f;
			vertexPositionTexture.Position.Z = 35.35534f;
			vertexPositionTexture.TextureCoordinate.X = 0.25f;
			vertexPositionTexture.TextureCoordinate.Y = 1f;
			array[2] = vertexPositionTexture;
			vertexPositionTexture.Position.X = -35.35534f;
			vertexPositionTexture.Position.Z = 35.35534f;
			vertexPositionTexture.TextureCoordinate.X = 0f;
			vertexPositionTexture.TextureCoordinate.Y = 1f;
			array[3] = vertexPositionTexture;
			vertexPositionTexture.Position.X = -1f;
			vertexPositionTexture.Position.Z = -1f;
			vertexPositionTexture.TextureCoordinate.X = 0f;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[4] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 1f;
			vertexPositionTexture.Position.Z = -1f;
			vertexPositionTexture.TextureCoordinate.X = 1f;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[5] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 1f;
			vertexPositionTexture.Position.Z = 1f;
			vertexPositionTexture.TextureCoordinate.X = 1f;
			vertexPositionTexture.TextureCoordinate.Y = 1f;
			array[6] = vertexPositionTexture;
			vertexPositionTexture.Position.X = -1f;
			vertexPositionTexture.Position.Z = 1f;
			vertexPositionTexture.TextureCoordinate.X = 0f;
			vertexPositionTexture.TextureCoordinate.Y = 1f;
			array[7] = vertexPositionTexture;
			if (this.mSwayVertices == null || this.mSwayVertices.IsDisposed)
			{
				this.mSwayVertices = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
			}
			lock (Game.Instance.GraphicsDevice)
			{
				this.mSwayVertices.SetData<VertexPositionTexture>(array);
			}
			if (this.mFirstStart)
			{
				this.mFirstStart = false;
				this.mModel.CreatePhysicsEntities(this.mSavedEntities, this.PlayState);
			}
			if (!this.mIsInitialized)
			{
				this.mModel.GetAllEffects(this.mEffects);
			}
			this.AddSavedEntities();
			this.PlayState.Camera.SnapPrimitive = this.mModel.CameraMesh;
			this.PlayState.Camera.Release(0f);
			for (int j = 0; j < this.mModel.Waters.Length; j++)
			{
				this.mModel.Waters[j].Initialize();
			}
			foreach (Light light3 in this.mModel.Lights.Values)
			{
				light3.CreateShadowMap();
				light3.Enable(playState.Scene);
			}
			foreach (Trigger trigger2 in this.mTriggers.Values)
			{
				trigger2.ResetTimers();
			}
			foreach (TriggerArea triggerArea in this.mModel.TriggerAreas.Values)
			{
				triggerArea.Register();
			}
			PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mKillPlane);
			if (iSpawnPoint.SpawnPlayers)
			{
				Vector3 vector3 = Vector3.Zero;
				for (int k = 0; k < Game.Instance.Players.Length; k++)
				{
					Matrix orientation = Matrix.Identity;
					Vector3 vector4 = default(Vector3);
					int l;
					for (l = k; l >= 0; l--)
					{
						Locator locator;
						if (this.mModel.Locators.TryGetValue((&iSpawnPoint.Locations.FixedElementField)[l], out locator))
						{
							orientation = locator.Transform;
							vector4 = locator.Transform.Translation;
							orientation.Translation = default(Vector3);
							break;
						}
					}
					vector4.X += 2f * (float)(k - l);
					vector4.Y += 1f;
					vector3 += vector4;
					Player player = Game.Instance.Players[k];
					if (player.Playing && player.Avatar != null && !player.Avatar.Dead)
					{
						Segment iSeg = default(Segment);
						iSeg.Delta.Y = -10f;
						iSeg.Origin = vector4;
						iSeg.Origin.Y = iSeg.Origin.Y + 1f;
						float num;
						Vector3 vector5;
						Vector3 vector6;
						if (this.SegmentIntersect(out num, out vector5, out vector6, iSeg))
						{
							vector4 = vector5;
							vector4.Y += player.Avatar.Capsule.Length * 0.5f + player.Avatar.Capsule.Radius;
						}
						player.Avatar.CharacterBody.MoveTo(vector4, orientation);
						player.Avatar.CharacterBody.DesiredDirection = orientation.Forward;
						player.Avatar.Events = null;
						player.Avatar.Path.Clear();
						player.Avatar.CharacterBody.Movement = default(Vector3);
						playState.EntityManager.AddEntity(player.Avatar);
						player.Avatar.Body.EnableBody();
					}
				}
				vector3 /= (float)Game.Instance.Players.Length;
				this.PlayState.Camera.SetPosition(vector3, true);
				playState.SpawnFairies();
			}
			this.RunStartupActions(reportbackAction);
			this.mIsInitialized = true;
		}

		// Token: 0x060028BB RID: 10427 RVA: 0x001403EC File Offset: 0x0013E5EC
		public void RestoreSavedAnimations()
		{
			if (this.mSavedAnimations.Count > 0)
			{
				foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
				{
					animatedLevelPart.RestoreStateFrom(this.mSavedAnimations);
					Matrix identity = Matrix.Identity;
					animatedLevelPart.Update(DataChannel.None, 0f, ref identity, this);
				}
			}
		}

		// Token: 0x060028BC RID: 10428 RVA: 0x00140474 File Offset: 0x0013E674
		public void RestoreDynamicLights()
		{
			this.mLevel.PlayState.Scene.ClearLights();
			foreach (Light light in this.mModel.Lights.Values)
			{
				light.CreateShadowMap();
				light.Enable(this.mLevel.PlayState.Scene);
			}
		}

		// Token: 0x060028BD RID: 10429 RVA: 0x001404FC File Offset: 0x0013E6FC
		public void AddSavedEntities()
		{
			for (int i = 0; i < this.mSavedEntities.Count; i++)
			{
				Entity entity = this.mSavedEntities[i];
				entity.Body.EnableBody();
				NonPlayerCharacter nonPlayerCharacter = entity as NonPlayerCharacter;
				if (nonPlayerCharacter != null)
				{
					nonPlayerCharacter.AI.Enable();
				}
				this.mPlayState.EntityManager.AddEntity(entity);
			}
			this.mSavedEntities.Clear();
		}

		// Token: 0x060028BE RID: 10430 RVA: 0x00140568 File Offset: 0x0013E768
		public int GetNumStartupActions()
		{
			if (this.mStartupActions == null)
			{
				return 0;
			}
			int result = 0;
			lock (this.mStartupActions)
			{
				result = this.mStartupActions.Count;
			}
			return result;
		}

		// Token: 0x060028BF RID: 10431 RVA: 0x001405B4 File Offset: 0x0013E7B4
		public void RunStartupActions()
		{
			this.RunStartupActions(null);
		}

		// Token: 0x060028C0 RID: 10432 RVA: 0x001405C0 File Offset: 0x0013E7C0
		public void RunStartupActions(Action<float> reportBackAction)
		{
			if (NetworkManager.Instance.State != NetworkState.Offline)
			{
				NetworkManager.Instance.Interface.Sync();
			}
			float num = (float)this.mStartupActions.Count;
			lock (this.mStartupActions)
			{
				for (int i = 0; i < this.mStartupActions.Count; i++)
				{
					this.mStartupActions[i].QuickExecute();
					float obj2 = ((float)i + 1f) / num;
					if (reportBackAction != null)
					{
						reportBackAction(obj2);
					}
				}
				this.mStartupActions.Clear();
			}
		}

		// Token: 0x060028C1 RID: 10433 RVA: 0x00140668 File Offset: 0x0013E868
		public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
		{
			if (this.mCloudIntensity > 1E-45f)
			{
				Vector2 vector;
				Vector2.Multiply(ref this.mCloudMovement, iDeltaTime, out vector);
				Vector2.Add(ref this.mCloudOffset, ref vector, out this.mCloudOffset);
				foreach (Light light in this.mModel.Lights.Values)
				{
					DirectionalLight directionalLight = light as DirectionalLight;
					if (directionalLight != null)
					{
						if (this.mCloudWorldRelative)
						{
							Vector3 vector2 = default(Vector3);
							vector2.X = this.mCloudOffset.X;
							vector2.Z = this.mCloudOffset.Y;
							Matrix lightOrientation = directionalLight.LightOrientation;
							Vector3 vector3;
							Vector3.Transform(ref vector2, ref lightOrientation, out vector3);
							directionalLight.ProjectedTextureOffset = new Vector2
							{
								X = -vector3.X,
								Y = vector3.Y
							};
						}
						else
						{
							directionalLight.ProjectedTextureOffset = this.mCloudOffset;
						}
					}
				}
			}
			EntityManager entityManager = this.PlayState.EntityManager;
			lock (this)
			{
				if (entityManager != null)
				{
					float scaleFactor = 224f;
					Vector3 vector4 = Vector3.Up;
					Vector3 vector5;
					Vector3.Multiply(ref iCameraDirection, scaleFactor, out vector5);
					Vector3.Add(ref vector5, ref iCameraPosition, out vector5);
					Vector3 vector6;
					Vector3.Multiply(ref vector4, 10f, out vector6);
					Vector3.Add(ref vector5, ref vector6, out vector6);
					vector4 = Vector3.Forward;
					Matrix matrix;
					Matrix.CreateLookAt(ref vector6, ref vector5, ref vector4, out matrix);
					Matrix matrix2;
					Matrix.CreateOrthographic(50f, 50f, 0f, 20f, out matrix2);
					Matrix.Multiply(ref matrix, ref matrix2, out this.mVisualAidViewProjection);
					GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
					DepthStencilBuffer depthStencilBuffer = graphicsDevice.DepthStencilBuffer;
					if (this.mUseVertexTexturing)
					{
						this.DrawSway(iDeltaTime, ref vector5, entityManager);
					}
					graphicsDevice.SetRenderTarget(0, null);
					graphicsDevice.DepthStencilBuffer = depthStencilBuffer;
					if (this.mUseVertexTexturing && this.mSwayTarget != null && !this.mSwayTarget.IsDisposed)
					{
						RenderDeferredEffect renderDeferredEffect = RenderManager.Instance.GetEffect(RenderDeferredEffect.TYPEHASH) as RenderDeferredEffect;
						renderDeferredEffect.SwayTexture = this.mSwayTarget.GetTexture();
						renderDeferredEffect.SwayProjection = this.mVisualAidViewProjection;
					}
				}
			}
		}

		// Token: 0x060028C2 RID: 10434 RVA: 0x001408DC File Offset: 0x0013EADC
		private void DrawSway(float iDeltaTime, ref Vector3 iOrigo, EntityManager iEntityManager)
		{
			if (this.mSwayTarget == null || this.mSwayTarget.IsDisposed)
			{
				return;
			}
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			Matrix identity;
			Matrix.CreateRotationY(1f, out identity);
			this.mWindPosition += iDeltaTime * this.mVindSpeed;
			Vector3 translation = default(Vector3);
			translation.X = this.mWindPosition;
			Vector3.Transform(ref translation, ref identity, out translation);
			identity.Translation = translation;
			identity.M41 -= iOrigo.X * 0.014142136f * 0.25f;
			identity.M42 -= iOrigo.Z * 0.014142136f;
			this.mSwayEffect.TextureTransform = identity;
			identity = Matrix.Identity;
			identity.M41 = iOrigo.X;
			identity.M42 = iOrigo.Y;
			identity.M43 = iOrigo.Z;
			this.mSwayEffect.World = identity;
			this.mSwayEffect.ViewProjection = this.mVisualAidViewProjection;
			this.mSwayEffect.Texture = this.mSwayTexture;
			this.mSwayEffect.SetTechnique(SwayEffect.Technique.Sway);
			graphicsDevice.SetRenderTarget(0, this.mSwayTarget);
			graphicsDevice.DepthStencilBuffer = null;
			graphicsDevice.VertexDeclaration = this.mSwayVertexDeclaration;
			graphicsDevice.RenderState.CullMode = CullMode.None;
			graphicsDevice.RenderState.DepthBufferEnable = false;
			graphicsDevice.RenderState.AlphaBlendEnable = false;
			graphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;
			graphicsDevice.Vertices[0].SetSource(this.mSwayVertices, 0, VertexPositionTexture.SizeInBytes);
			graphicsDevice.VertexDeclaration = this.mSwayVertexDeclaration;
			this.mSwayEffect.Begin();
			this.mSwayEffect.CurrentTechnique.Passes[0].Begin();
			graphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
			this.mSwayEffect.CurrentTechnique.Passes[0].End();
			this.mSwayEffect.End();
			this.mSwayEffect.Texture = this.mCharacterDisplacementTexture;
			this.mSwayEffect.SetTechnique(SwayEffect.Technique.CharacterOffset);
			this.mSwayEffect.Begin();
			this.mSwayEffect.CurrentTechnique.Passes[0].Begin();
			List<Magicka.GameLogic.Entities.Character> list = new List<Magicka.GameLogic.Entities.Character>();
			iEntityManager.GetCharacters(ref list);
			lock (list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					Magicka.GameLogic.Entities.Character character = list[i];
					Vector3 position = character.Position;
					float radius = character.Radius;
					identity.M41 = position.X;
					identity.M42 = iOrigo.Y;
					identity.M43 = position.Z;
					identity.M11 = radius;
					identity.M22 = radius;
					identity.M33 = radius;
					this.mSwayEffect.World = identity;
					this.mSwayEffect.CommitChanges();
					graphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 4, 2);
				}
			}
			this.mSwayEffect.CurrentTechnique.Passes[0].End();
			this.mSwayEffect.End();
			graphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
		}

		// Token: 0x060028C3 RID: 10435 RVA: 0x00140C10 File Offset: 0x0013EE10
		public void ActionExecute(Action iAction)
		{
			if (iAction is Spawn)
			{
				this.mTriggeredActions.Remove(iAction);
			}
			this.mTriggeredActions.Add(iAction);
		}

		// Token: 0x060028C4 RID: 10436 RVA: 0x00140C34 File Offset: 0x0013EE34
		public void Dispose()
		{
			lock (this)
			{
				if (this.mModel != null)
				{
					foreach (GameScene.GlobalAudio globalAudio in this.mGlobalSounds.Values)
					{
						globalAudio.Stop(AudioStopOptions.Immediate);
					}
					this.mGlobalSounds = null;
					foreach (AudioLocator audioLocator in this.mSounds.Values)
					{
						audioLocator.Stop(AudioStopOptions.Immediate);
					}
					this.mSounds = null;
					this.mModel.Dispose();
					this.mModel = null;
					this.mLiquids = null;
				}
				if (this.mSwayTarget != null)
				{
					this.mSwayTarget.Dispose();
					this.mSwayTarget = null;
				}
				if (this.mRuleset != null)
				{
					this.mRuleset.DeInitialize();
				}
			}
			this.mContent.Dispose();
		}

		// Token: 0x060028C5 RID: 10437 RVA: 0x00140D58 File Offset: 0x0013EF58
		public void ExecuteTrigger(int iTrigger, Magicka.GameLogic.Entities.Character iArg, bool iIgnoreConditions)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mTriggers[iTrigger].Execute(iArg, iIgnoreConditions);
			}
		}

		// Token: 0x04002BF8 RID: 11256
		private const float PROJECTIONWIDTH = 50f;

		// Token: 0x04002BF9 RID: 11257
		private const float OFFSETLENGTH = 35.35534f;

		// Token: 0x04002BFA RID: 11258
		private const float OFFSETDEVISOR = 0.014142136f;

		// Token: 0x04002BFB RID: 11259
		private static readonly string VOLUME_VAR_NAME = "Volume";

		// Token: 0x04002BFC RID: 11260
		public static readonly int ANYAREA = "any".GetHashCodeCustom();

		// Token: 0x04002BFD RID: 11261
		private SharedContentManager mContent;

		// Token: 0x04002BFE RID: 11262
		private PlayState mPlayState;

		// Token: 0x04002BFF RID: 11263
		private Level mLevel;

		// Token: 0x04002C00 RID: 11264
		private int mLastGeneratedEffectID;

		// Token: 0x04002C01 RID: 11265
		private List<Action> mTriggeredActions;

		// Token: 0x04002C02 RID: 11266
		private List<Action> mStartupActions = new List<Action>(64);

		// Token: 0x04002C03 RID: 11267
		private SortedList<int, GameScene.GlobalAudio> mGlobalSounds = new SortedList<int, GameScene.GlobalAudio>();

		// Token: 0x04002C04 RID: 11268
		private SortedList<int, AudioLocator> mSounds = new SortedList<int, AudioLocator>();

		// Token: 0x04002C05 RID: 11269
		private SortedList<int, Trigger> mTriggers;

		// Token: 0x04002C06 RID: 11270
		private string mModelName;

		// Token: 0x04002C07 RID: 11271
		private LevelModel mModel;

		// Token: 0x04002C08 RID: 11272
		private Liquid[] mLiquids;

		// Token: 0x04002C09 RID: 11273
		private string mName;

		// Token: 0x04002C0A RID: 11274
		private IRuleset mRuleset;

		// Token: 0x04002C0B RID: 11275
		private bool mIndoor;

		// Token: 0x04002C0C RID: 11276
		private bool mForceNavMesh;

		// Token: 0x04002C0D RID: 11277
		private bool mForceCamera;

		// Token: 0x04002C0E RID: 11278
		private bool mFirstStart = true;

		// Token: 0x04002C0F RID: 11279
		private bool mIsInitialized;

		// Token: 0x04002C10 RID: 11280
		private List<Entity> mSavedEntities = new List<Entity>();

		// Token: 0x04002C11 RID: 11281
		private Dictionary<int, AnimatedLevelPart.AnimationState> mSavedAnimations = new Dictionary<int, AnimatedLevelPart.AnimationState>();

		// Token: 0x04002C12 RID: 11282
		private Fog mFog;

		// Token: 0x04002C13 RID: 11283
		private float mBrightness = 1f;

		// Token: 0x04002C14 RID: 11284
		private float mContrast = 1f;

		// Token: 0x04002C15 RID: 11285
		private float mSaturation = 1f;

		// Token: 0x04002C16 RID: 11286
		private float mRimLightGlow = 0.8f;

		// Token: 0x04002C17 RID: 11287
		private float mRimLightPower = 1.5f;

		// Token: 0x04002C18 RID: 11288
		private float mRimLightBias = 0.2f;

		// Token: 0x04002C19 RID: 11289
		private float mBloomThreshold = 0.8f;

		// Token: 0x04002C1A RID: 11290
		private float mBloomMultiplier = 1f;

		// Token: 0x04002C1B RID: 11291
		private float mBlurSigma = 2.5f;

		// Token: 0x04002C1C RID: 11292
		private Matrix mVisualAidViewProjection;

		// Token: 0x04002C1D RID: 11293
		private bool mUseVertexTexturing;

		// Token: 0x04002C1E RID: 11294
		private SurfaceFormat mSwaySurfaceFormat;

		// Token: 0x04002C1F RID: 11295
		private RenderTarget2D mSwayTarget;

		// Token: 0x04002C20 RID: 11296
		private Texture2D mSwayTexture;

		// Token: 0x04002C21 RID: 11297
		private Texture2D mCharacterDisplacementTexture;

		// Token: 0x04002C22 RID: 11298
		private SwayEffect mSwayEffect;

		// Token: 0x04002C23 RID: 11299
		private VertexDeclaration mSwayVertexDeclaration;

		// Token: 0x04002C24 RID: 11300
		private VertexBuffer mSwayVertices;

		// Token: 0x04002C25 RID: 11301
		private float mWindPosition;

		// Token: 0x04002C26 RID: 11302
		private Vector2 mVindDirection = Vector2.Normalize(new Vector2(1f, 1f));

		// Token: 0x04002C27 RID: 11303
		private float mVindSpeed = 0.05f;

		// Token: 0x04002C28 RID: 11304
		private float mReverbMix;

		// Token: 0x04002C29 RID: 11305
		private RoomType mRoomType;

		// Token: 0x04002C2A RID: 11306
		private bool mCloudWorldRelative;

		// Token: 0x04002C2B RID: 11307
		private float mCloudIntensity;

		// Token: 0x04002C2C RID: 11308
		private Vector2 mCloudScale;

		// Token: 0x04002C2D RID: 11309
		private Vector2 mCloudMovement;

		// Token: 0x04002C2E RID: 11310
		private Vector2 mCloudOffset;

		// Token: 0x04002C2F RID: 11311
		private Texture2D mCloudTexture;

		// Token: 0x04002C30 RID: 11312
		private Texture2D mSkyMap;

		// Token: 0x04002C31 RID: 11313
		private string mSkyMapFileName;

		// Token: 0x04002C32 RID: 11314
		private Vector3 mSkyMapColor;

		// Token: 0x04002C33 RID: 11315
		private float mLightIntensity = 1f;

		// Token: 0x04002C34 RID: 11316
		private float mTargetLightIntensity = 1f;

		// Token: 0x04002C35 RID: 11317
		private Vector3 mLightDiffuse = new Vector3(1f);

		// Token: 0x04002C36 RID: 11318
		private Vector3 mTargetLightDiffuse = new Vector3(1f);

		// Token: 0x04002C37 RID: 11319
		private GameScene.LightSettings[] mDirectionalLightSettings;

		// Token: 0x04002C38 RID: 11320
		private SortedList<int, GameScene.EffectStorage> mEffects = new SortedList<int, GameScene.EffectStorage>();

		// Token: 0x04002C39 RID: 11321
		private int mId;

		// Token: 0x04002C3A RID: 11322
		private CollisionSkin mKillPlane;

		// Token: 0x04002C3B RID: 11323
		private byte[] mShaHash;

		// Token: 0x02000555 RID: 1365
		public struct GlobalAudio
		{
			// Token: 0x060028C7 RID: 10439 RVA: 0x00140D98 File Offset: 0x0013EF98
			public GlobalAudio(Banks iBank, int iCue, float iVolume)
			{
				this.mBank = iBank;
				this.mCueID = iCue;
				this.mCue = AudioManager.Instance.GetCue(this.mBank, this.mCueID);
				this.mVolume = iVolume;
				this.mCue.SetVariable(GameScene.VOLUME_VAR_NAME, this.mVolume);
			}

			// Token: 0x17000998 RID: 2456
			// (get) Token: 0x060028C8 RID: 10440 RVA: 0x00140DEC File Offset: 0x0013EFEC
			public Banks Bank
			{
				get
				{
					return this.mBank;
				}
			}

			// Token: 0x17000999 RID: 2457
			// (get) Token: 0x060028C9 RID: 10441 RVA: 0x00140DF4 File Offset: 0x0013EFF4
			public int CueID
			{
				get
				{
					return this.mCueID;
				}
			}

			// Token: 0x1700099A RID: 2458
			// (get) Token: 0x060028CA RID: 10442 RVA: 0x00140DFC File Offset: 0x0013EFFC
			public float Volume
			{
				get
				{
					return this.mVolume;
				}
			}

			// Token: 0x060028CB RID: 10443 RVA: 0x00140E04 File Offset: 0x0013F004
			public void Play()
			{
				if (this.mCue == null || this.mCue.IsStopped || this.mCue.IsStopping)
				{
					this.mCue = AudioManager.Instance.GetCue(this.mBank, this.mCueID);
					this.mCue.SetVariable(GameScene.VOLUME_VAR_NAME, this.mVolume);
				}
				if (this.mCue.IsPaused)
				{
					this.mCue.Resume();
					return;
				}
				if (!this.mCue.IsPlaying)
				{
					this.mCue.Play();
				}
			}

			// Token: 0x060028CC RID: 10444 RVA: 0x00140E96 File Offset: 0x0013F096
			public void Pause()
			{
				if (this.mCue != null && this.mCue.IsPlaying)
				{
					this.mCue.Pause();
				}
			}

			// Token: 0x060028CD RID: 10445 RVA: 0x00140EB8 File Offset: 0x0013F0B8
			public void Stop(AudioStopOptions iOptions)
			{
				if (this.mCue != null && !this.mCue.IsStopped && !this.mCue.IsStopping)
				{
					this.mCue.Stop(iOptions);
				}
			}

			// Token: 0x060028CE RID: 10446 RVA: 0x00140EE8 File Offset: 0x0013F0E8
			public void SetVolume(float iValue)
			{
				this.mVolume = iValue;
				this.mCue.SetVariable(GameScene.VOLUME_VAR_NAME, iValue);
			}

			// Token: 0x04002C3C RID: 11324
			private Cue mCue;

			// Token: 0x04002C3D RID: 11325
			private Banks mBank;

			// Token: 0x04002C3E RID: 11326
			private int mCueID;

			// Token: 0x04002C3F RID: 11327
			private float mVolume;
		}

		// Token: 0x02000556 RID: 1366
		public struct LightSettings
		{
			// Token: 0x060028CF RID: 10447 RVA: 0x00140F02 File Offset: 0x0013F102
			public void Assign()
			{
				this.Light.AmbientColor = this.AmbientColor;
				this.Light.DiffuseColor = this.DiffuseColor;
				this.Light.SpecularAmount = this.SpecularAmount;
			}

			// Token: 0x060028D0 RID: 10448 RVA: 0x00140F38 File Offset: 0x0013F138
			public void Assign(float iIntensity)
			{
				Vector3 vector;
				Vector3.Multiply(ref this.AmbientColor, iIntensity, out vector);
				this.Light.AmbientColor = vector;
				Vector3.Multiply(ref this.DiffuseColor, iIntensity, out vector);
				this.Light.DiffuseColor = vector;
				this.Light.SpecularAmount = this.SpecularAmount * iIntensity;
			}

			// Token: 0x060028D1 RID: 10449 RVA: 0x00140F8C File Offset: 0x0013F18C
			public void GetFromLight(Light iLight)
			{
				this.Light = iLight;
				this.AmbientColor = this.Light.AmbientColor;
				this.DiffuseColor = this.Light.DiffuseColor;
				this.SpecularAmount = this.Light.SpecularAmount;
			}

			// Token: 0x04002C40 RID: 11328
			public Light Light;

			// Token: 0x04002C41 RID: 11329
			public Vector3 AmbientColor;

			// Token: 0x04002C42 RID: 11330
			public Vector3 DiffuseColor;

			// Token: 0x04002C43 RID: 11331
			public float SpecularAmount;
		}

		// Token: 0x02000557 RID: 1367
		public struct EffectStorage
		{
			// Token: 0x04002C44 RID: 11332
			public VisualEffect Effect;

			// Token: 0x04002C45 RID: 11333
			public Matrix Transform;

			// Token: 0x04002C46 RID: 11334
			public AnimatedLevelPart Animation;

			// Token: 0x04002C47 RID: 11335
			public float Range;
		}

		// Token: 0x02000558 RID: 1368
		public class State
		{
			// Token: 0x060028D2 RID: 10450 RVA: 0x00140FC8 File Offset: 0x0013F1C8
			public State(GameScene iScene)
			{
				this.mScene = iScene;
				this.mSavedEntities = new EntityStateStorage(iScene.PlayState);
				this.mTriggers = new Trigger.State[this.mScene.mTriggers.Count];
				int num = 0;
				foreach (Trigger trigger in this.mScene.mTriggers.Values)
				{
					this.mTriggers[num++] = trigger.GetState();
				}
				this.mTriggeredActions = new List<Action>(iScene.mTriggeredActions.Capacity);
				this.UpdateState();
			}

			// Token: 0x060028D3 RID: 10451 RVA: 0x001410A4 File Offset: 0x0013F2A4
			public void UpdateState()
			{
				this.mFirstStart = this.mScene.mFirstStart;
				this.mSavedEntities.Clear();
				if (this.mScene.Level.CurrentScene == this.mScene)
				{
					this.mSavedEntities.Store(this.mScene.PlayState.EntityManager.Entities);
				}
				else
				{
					this.mSavedEntities.Store(this.mScene.mSavedEntities);
				}
				this.mTriggeredActions.Clear();
				this.mTriggeredActions.AddRange(this.mScene.mTriggeredActions);
				this.mGlobalAudio.Clear();
				foreach (KeyValuePair<int, GameScene.GlobalAudio> keyValuePair in this.mScene.mGlobalSounds)
				{
					this.mGlobalAudio.Add(keyValuePair.Key, keyValuePair.Value);
				}
				this.mAmbientAudio.Clear();
				foreach (KeyValuePair<int, AudioLocator> keyValuePair2 in this.mScene.mSounds)
				{
					this.mAmbientAudio.Add(keyValuePair2.Key, keyValuePair2.Value);
				}
				this.mAnimations.Clear();
				if (this.mScene.Level.CurrentScene == this.mScene)
				{
					using (Dictionary<int, AnimatedLevelPart>.ValueCollection.Enumerator enumerator3 = this.mScene.mModel.AnimatedLevelParts.Values.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							AnimatedLevelPart animatedLevelPart = enumerator3.Current;
							animatedLevelPart.AddStateTo(this.mAnimations);
						}
						goto IL_1F7;
					}
				}
				foreach (KeyValuePair<int, AnimatedLevelPart.AnimationState> keyValuePair3 in this.mScene.mSavedAnimations)
				{
					this.mAnimations.Add(keyValuePair3.Key, keyValuePair3.Value);
				}
				IL_1F7:
				for (int i = 0; i < this.mTriggers.Length; i++)
				{
					this.mTriggers[i].UpdateState();
				}
			}

			// Token: 0x060028D4 RID: 10452 RVA: 0x00141304 File Offset: 0x0013F504
			public void ApplyState(List<int> iIgnoredTriggers)
			{
				this.mScene.mFirstStart = this.mFirstStart;
				this.mScene.mSavedEntities.Clear();
				this.mSavedEntities.Restore(this.mScene.mSavedEntities);
				this.mScene.mGlobalSounds.Clear();
				foreach (KeyValuePair<int, GameScene.GlobalAudio> keyValuePair in this.mGlobalAudio)
				{
					this.mScene.mGlobalSounds.Add(keyValuePair.Key, keyValuePair.Value);
				}
				this.mScene.mSounds.Clear();
				foreach (KeyValuePair<int, AudioLocator> keyValuePair2 in this.mAmbientAudio)
				{
					this.mScene.mSounds.Add(keyValuePair2.Key, keyValuePair2.Value);
				}
				this.mScene.mSavedAnimations.Clear();
				foreach (KeyValuePair<int, AnimatedLevelPart.AnimationState> keyValuePair3 in this.mAnimations)
				{
					this.mScene.mSavedAnimations.Add(keyValuePair3.Key, keyValuePair3.Value);
				}
				this.mScene.mTriggeredActions.Clear();
				this.mScene.mTriggeredActions.AddRange(this.mTriggeredActions);
				if (iIgnoredTriggers != null)
				{
					for (int i = 0; i < this.mTriggers.Length; i++)
					{
						if (iIgnoredTriggers.Contains(this.mTriggers[i].Trigger.ID))
						{
							this.mTriggers[i].ResetState();
						}
						else
						{
							this.mTriggers[i].ApplyState();
						}
					}
					this.mScene.mStartupActions.Clear();
					for (int j = 0; j < this.mTriggeredActions.Count; j++)
					{
						if (!iIgnoredTriggers.Contains(this.mTriggeredActions[j].Trigger.ID))
						{
							this.mScene.mStartupActions.Add(this.mTriggeredActions[j]);
						}
					}
					return;
				}
				for (int k = 0; k < this.mTriggers.Length; k++)
				{
					this.mTriggers[k].ApplyState();
				}
				this.mScene.mStartupActions.Clear();
				this.mScene.mStartupActions.AddRange(this.mTriggeredActions);
			}

			// Token: 0x1700099B RID: 2459
			// (get) Token: 0x060028D5 RID: 10453 RVA: 0x001415B4 File Offset: 0x0013F7B4
			public GameScene Scene
			{
				get
				{
					return this.mScene;
				}
			}

			// Token: 0x060028D6 RID: 10454 RVA: 0x001415BC File Offset: 0x0013F7BC
			internal void Write(BinaryWriter iWriter)
			{
				iWriter.Write(this.mFirstStart);
				this.mSavedEntities.Write(iWriter);
				iWriter.Write(this.mGlobalAudio.Count);
				foreach (KeyValuePair<int, GameScene.GlobalAudio> keyValuePair in this.mGlobalAudio)
				{
					iWriter.Write(keyValuePair.Key);
					iWriter.Write((ushort)keyValuePair.Value.Bank);
					iWriter.Write(keyValuePair.Value.CueID);
					iWriter.Write(keyValuePair.Value.Volume);
				}
				iWriter.Write(this.mAmbientAudio.Count);
				foreach (AudioLocator audioLocator in this.mAmbientAudio.Values)
				{
					iWriter.Write(audioLocator.ID);
					iWriter.Write((ushort)audioLocator.Bank);
					iWriter.Write(audioLocator.CueID);
					iWriter.Write(audioLocator.Volume);
					iWriter.Write(audioLocator.Locator);
					iWriter.Write(audioLocator.Radius);
					iWriter.Write(audioLocator.Apply3D);
				}
				iWriter.Write(this.mAnimations.Count);
				foreach (KeyValuePair<int, AnimatedLevelPart.AnimationState> keyValuePair2 in this.mAnimations)
				{
					iWriter.Write(keyValuePair2.Key);
					keyValuePair2.Value.Write(iWriter);
				}
				for (int i = 0; i < this.mTriggers.Length; i++)
				{
					this.mTriggers[i].Write(iWriter);
				}
				iWriter.Write(this.mTriggeredActions.Count);
				for (int j = 0; j < this.mTriggeredActions.Count; j++)
				{
					iWriter.Write(this.mTriggeredActions[j].Handle);
				}
			}

			// Token: 0x060028D7 RID: 10455 RVA: 0x00141800 File Offset: 0x0013FA00
			internal void Read(BinaryReader iReader)
			{
				this.mFirstStart = iReader.ReadBoolean();
				this.mSavedEntities.Clear();
				this.mSavedEntities.Read(iReader);
				this.mGlobalAudio.Clear();
				int num = iReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					int key = iReader.ReadInt32();
					Banks iBank = (Banks)iReader.ReadUInt16();
					int iCue = iReader.ReadInt32();
					float iVolume = iReader.ReadSingle();
					GameScene.GlobalAudio value = new GameScene.GlobalAudio(iBank, iCue, iVolume);
					this.mGlobalAudio.Add(key, value);
				}
				this.mAmbientAudio.Clear();
				num = iReader.ReadInt32();
				for (int j = 0; j < num; j++)
				{
					int num2 = iReader.ReadInt32();
					Banks iBank2 = (Banks)iReader.ReadUInt16();
					int iCue2 = iReader.ReadInt32();
					float iVolume2 = iReader.ReadSingle();
					int iLocator = iReader.ReadInt32();
					float iRadius = iReader.ReadSingle();
					bool iApply3D = iReader.ReadBoolean();
					AudioLocator value2 = new AudioLocator(num2, iBank2, iCue2, iVolume2, iLocator, iRadius, iApply3D);
					this.mAmbientAudio.Add(num2, value2);
				}
				this.mAnimations.Clear();
				num = iReader.ReadInt32();
				for (int k = 0; k < num; k++)
				{
					int key2 = iReader.ReadInt32();
					AnimatedLevelPart.AnimationState value3 = new AnimatedLevelPart.AnimationState(iReader);
					this.mAnimations.Add(key2, value3);
				}
				for (int l = 0; l < this.mTriggers.Length; l++)
				{
					this.mTriggers[l].Read(iReader);
				}
				this.mTriggeredActions.Clear();
				num = iReader.ReadInt32();
				for (int m = 0; m < num; m++)
				{
					this.mTriggeredActions.Add(Action.GetByHandle(iReader.ReadUInt16()));
				}
			}

			// Token: 0x04002C48 RID: 11336
			private GameScene mScene;

			// Token: 0x04002C49 RID: 11337
			private Trigger.State[] mTriggers;

			// Token: 0x04002C4A RID: 11338
			private bool mFirstStart;

			// Token: 0x04002C4B RID: 11339
			private EntityStateStorage mSavedEntities;

			// Token: 0x04002C4C RID: 11340
			private List<Action> mTriggeredActions;

			// Token: 0x04002C4D RID: 11341
			private Dictionary<int, GameScene.GlobalAudio> mGlobalAudio = new Dictionary<int, GameScene.GlobalAudio>();

			// Token: 0x04002C4E RID: 11342
			private Dictionary<int, AudioLocator> mAmbientAudio = new Dictionary<int, AudioLocator>();

			// Token: 0x04002C4F RID: 11343
			private Dictionary<int, AnimatedLevelPart.AnimationState> mAnimations = new Dictionary<int, AnimatedLevelPart.AnimationState>();
		}
	}
}
