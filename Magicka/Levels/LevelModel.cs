using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Magicka.PathFinding;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Lights;
using PolygonHead.Models;

namespace Magicka.Levels
{
	// Token: 0x02000389 RID: 905
	public class LevelModel : IDisposable
	{
		// Token: 0x06001BB3 RID: 7091 RVA: 0x000BE4EC File Offset: 0x000BC6EC
		public LevelModel(ContentReader iInput)
		{
			GraphicsDevice graphicsDevice = (iInput.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice;
			this.mModel = iInput.ReadObject<BiTreeModel>();
			int num = iInput.ReadInt32();
			this.mAnimatedLevelParts = new Dictionary<int, AnimatedLevelPart>(num);
			for (int i = 0; i < num; i++)
			{
				AnimatedLevelPart animatedLevelPart = new AnimatedLevelPart(iInput, this);
				this.mAnimatedLevelParts.Add(animatedLevelPart.ID, animatedLevelPart);
			}
			num = iInput.ReadInt32();
			this.mLights = new Dictionary<int, Light>(num);
			for (int j = 0; j < num; j++)
			{
				string text = iInput.ReadString();
				Vector3 position = iInput.ReadVector3();
				Vector3 vector = iInput.ReadVector3();
				LightType lightType = (LightType)iInput.ReadInt32();
				LightVariationType variationType = (LightVariationType)iInput.ReadInt32();
				float num2 = iInput.ReadSingle();
				bool useAttenuation = iInput.ReadBoolean();
				float cutoffAngle = iInput.ReadSingle();
				float sharpness = iInput.ReadSingle();
				Light light;
				switch (lightType)
				{
				case LightType.Point:
					light = new PointLight(graphicsDevice)
					{
						Position = position,
						Radius = num2
					};
					break;
				case LightType.Directional:
				{
					DirectionalLight directionalLight = new DirectionalLight(graphicsDevice, vector, false);
					light = directionalLight;
					break;
				}
				case LightType.Spot:
					light = new SpotLight(graphicsDevice)
					{
						Position = position,
						Range = num2,
						Direction = vector,
						CutoffAngle = cutoffAngle,
						Sharpness = sharpness,
						UseAttenuation = useAttenuation
					};
					break;
				default:
					throw new Exception("Unknown light type!");
				}
				light.Name = text;
				light.VariationType = variationType;
				light.DiffuseColor = iInput.ReadVector3();
				light.AmbientColor = iInput.ReadVector3();
				light.SpecularAmount = iInput.ReadSingle();
				light.VariationSpeed = iInput.ReadSingle();
				light.VariationAmount = iInput.ReadSingle();
				light.ShadowMapSize = GlobalSettings.Instance.ModShadowResolution(iInput.ReadInt32());
				light.CastShadows = iInput.ReadBoolean();
				this.mLights.Add(text.GetHashCodeCustom(), light);
			}
			num = iInput.ReadInt32();
			this.mEffects = new LevelModel.VisualEffectStorage[num];
			Vector3 vector2 = default(Vector3);
			vector2.Y = 1f;
			for (int k = 0; k < num; k++)
			{
				string iString = iInput.ReadString().ToLowerInvariant();
				LevelModel.VisualEffectStorage visualEffectStorage;
				visualEffectStorage.ID = iString.GetHashCodeCustom();
				Vector3 vector3 = iInput.ReadVector3();
				Vector3 vector4 = iInput.ReadVector3();
				visualEffectStorage.Range = iInput.ReadSingle();
				string iString2 = iInput.ReadString();
				visualEffectStorage.Effect = iString2.GetHashCodeCustom();
				Matrix.CreateWorld(ref vector3, ref vector4, ref vector2, out visualEffectStorage.Transform);
				this.mEffects[k] = visualEffectStorage;
			}
			num = iInput.ReadInt32();
			this.mPhysEntities = new LevelModel.PhysicsEntityStorage[num];
			for (int l = 0; l < num; l++)
			{
				LevelModel.PhysicsEntityStorage physicsEntityStorage;
				physicsEntityStorage.StartTransform = iInput.ReadMatrix();
				physicsEntityStorage.Template = iInput.ContentManager.Load<PhysicsEntityTemplate>("Data/PhysicsEntities/" + iInput.ReadString());
				this.mPhysEntities[l] = physicsEntityStorage;
			}
			num = iInput.ReadInt32();
			this.mWaters = new Liquid[num];
			for (int m = 0; m < num; m++)
			{
				this.mWaters[m] = Liquid.Read(iInput, this, null);
			}
			num = iInput.ReadInt32();
			this.mForceFields = new ForceField[num];
			for (int n = 0; n < num; n++)
			{
				this.mForceFields[n] = new ForceField(iInput, this);
			}
			this.mCollisionSkin = new CollisionSkin(null);
			for (int num3 = 0; num3 < 10; num3++)
			{
				TriangleMesh triangleMesh = new TriangleMesh();
				List<Vector3> vertices;
				List<TriangleVertexIndices> list;
				if (iInput.ReadBoolean())
				{
					vertices = iInput.ReadObject<List<Vector3>>();
					num = iInput.ReadInt32();
					list = new List<TriangleVertexIndices>(num);
					for (int num4 = 0; num4 < num; num4++)
					{
						TriangleVertexIndices item;
						item.I0 = iInput.ReadInt32();
						item.I1 = iInput.ReadInt32();
						item.I2 = iInput.ReadInt32();
						list.Add(item);
					}
				}
				else
				{
					vertices = new List<Vector3>();
					list = new List<TriangleVertexIndices>();
				}
				triangleMesh.CreateMesh(vertices, list, 16, 2f);
				this.mCollisionSkin.AddPrimitive(triangleMesh, 1, new MaterialProperties(1f, 1f, 1f));
			}
			this.mCollisionSkin.ApplyLocalTransform(Transform.Identity);
			this.mCollisionSkin.Tag = this;
			if (iInput.ReadBoolean())
			{
				this.mCameraMesh = new TriangleMesh();
				List<Vector3> vertices2 = iInput.ReadObject<List<Vector3>>();
				num = iInput.ReadInt32();
				List<TriangleVertexIndices> list2 = new List<TriangleVertexIndices>(num);
				for (int num5 = 0; num5 < num; num5++)
				{
					TriangleVertexIndices item2;
					item2.I0 = iInput.ReadInt32();
					item2.I1 = iInput.ReadInt32();
					item2.I2 = iInput.ReadInt32();
					list2.Add(item2);
				}
				this.mCameraMesh.CreateMesh(vertices2, list2, 4, 2f);
			}
			num = iInput.ReadInt32();
			for (int num6 = 0; num6 < num; num6++)
			{
				string text2 = iInput.ReadString();
				this.mTriggerAreas.Add(text2.ToLowerInvariant().GetHashCodeCustom(), TriggerArea.Read(iInput));
			}
			this.mTriggerAreas.Add(TriggerArea.ANYID, new AnyTriggerArea());
			num = iInput.ReadInt32();
			for (int num7 = 0; num7 < num; num7++)
			{
				string text3 = iInput.ReadString();
				Locator value = new Locator(text3, iInput);
				this.mLocators.Add(text3.GetHashCodeCustom(), value);
			}
			this.mNavMesh = new NavMesh(this, iInput);
		}

		// Token: 0x06001BB4 RID: 7092 RVA: 0x000BEA90 File Offset: 0x000BCC90
		public void UpdateAnimatedLevelParts(DataChannel iDataChannel, float iDeltaTime, GameScene iScene)
		{
			Matrix identity = Matrix.Identity;
			foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
			{
				animatedLevelPart.Update(iDataChannel, iDeltaTime, ref identity, iScene);
			}
		}

		// Token: 0x06001BB5 RID: 7093 RVA: 0x000BEAF4 File Offset: 0x000BCCF4
		public void Update(DataChannel iDataChannel, float iDeltaTime, GameScene iScene)
		{
			this.mModel.SwayEnabled = true;
			this.mModel.AddToScene(iDataChannel, iScene.PlayState.Scene);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				this.mNetworkUpdateTimer -= iDeltaTime;
				if (this.mNetworkUpdateTimer <= 0f)
				{
					this.mNetworkUpdateTimer = 0.1f;
					NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
					if (networkServer != null)
					{
						foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
						{
							animatedLevelPart.NetworkUpdate(networkServer);
						}
					}
				}
			}
			for (int i = 0; i < this.mWaters.Length; i++)
			{
				this.mWaters[i].Update(iDataChannel, iDeltaTime, iScene.PlayState.Scene);
			}
			for (int j = 0; j < this.mForceFields.Length; j++)
			{
				this.mForceFields[j].Update(iDataChannel, iDeltaTime);
			}
		}

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x06001BB6 RID: 7094 RVA: 0x000BEC08 File Offset: 0x000BCE08
		public Dictionary<int, AnimatedLevelPart> AnimatedLevelParts
		{
			get
			{
				return this.mAnimatedLevelParts;
			}
		}

		// Token: 0x06001BB7 RID: 7095 RVA: 0x000BEC10 File Offset: 0x000BCE10
		public AnimatedLevelPart GetAnimatedLevelPart(int iId)
		{
			return this.mAnimatedLevelParts[iId];
		}

		// Token: 0x06001BB8 RID: 7096 RVA: 0x000BEC20 File Offset: 0x000BCE20
		public void Initialize(PlayState iPlayState)
		{
			Matrix identity = Matrix.Identity;
			foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
			{
				animatedLevelPart.Initialize(ref identity);
			}
			for (int i = 0; i < this.mForceFields.Length; i++)
			{
				this.mForceFields[i].Initialize(iPlayState);
			}
		}

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x000BECA0 File Offset: 0x000BCEA0
		public Dictionary<int, Light> Lights
		{
			get
			{
				return this.mLights;
			}
		}

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x06001BBA RID: 7098 RVA: 0x000BECA8 File Offset: 0x000BCEA8
		public Liquid[] Waters
		{
			get
			{
				return this.mWaters;
			}
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x06001BBB RID: 7099 RVA: 0x000BECB0 File Offset: 0x000BCEB0
		public Dictionary<int, TriggerArea> TriggerAreas
		{
			get
			{
				return this.mTriggerAreas;
			}
		}

		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x06001BBC RID: 7100 RVA: 0x000BECB8 File Offset: 0x000BCEB8
		public Dictionary<int, Locator> Locators
		{
			get
			{
				return this.mLocators;
			}
		}

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x06001BBD RID: 7101 RVA: 0x000BECC0 File Offset: 0x000BCEC0
		public CollisionSkin CollisionSkin
		{
			get
			{
				return this.mCollisionSkin;
			}
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x06001BBE RID: 7102 RVA: 0x000BECC8 File Offset: 0x000BCEC8
		public TriangleMesh CameraMesh
		{
			get
			{
				return this.mCameraMesh;
			}
		}

		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x06001BBF RID: 7103 RVA: 0x000BECD0 File Offset: 0x000BCED0
		public BiTreeModel Model
		{
			get
			{
				return this.mModel;
			}
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x06001BC0 RID: 7104 RVA: 0x000BECD8 File Offset: 0x000BCED8
		internal NavMesh NavMesh
		{
			get
			{
				return this.mNavMesh;
			}
		}

		// Token: 0x06001BC1 RID: 7105 RVA: 0x000BECE0 File Offset: 0x000BCEE0
		public void RegisterCollisionSkin()
		{
			if (!PhysicsManager.Instance.Simulator.CollisionSystem.CollisionSkins.Contains(this.mCollisionSkin))
			{
				PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mCollisionSkin);
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
			{
				animatedLevelPart.RegisterCollisionSkin();
			}
		}

		// Token: 0x06001BC2 RID: 7106 RVA: 0x000BED74 File Offset: 0x000BCF74
		public void CreatePhysicsEntities(List<Entity> iEntities, PlayState iPlayState)
		{
			for (int i = 0; i < this.mPhysEntities.Length; i++)
			{
				PhysicsEntity physicsEntity;
				if (this.mPhysEntities[i].Template.MaxHitpoints > 0)
				{
					physicsEntity = new DamageablePhysicsEntity(iPlayState);
				}
				else
				{
					physicsEntity = new PhysicsEntity(iPlayState);
				}
				physicsEntity.Initialize(this.mPhysEntities[i].Template, this.mPhysEntities[i].StartTransform, 0);
				iEntities.Add(physicsEntity);
			}
		}

		// Token: 0x06001BC3 RID: 7107 RVA: 0x000BEDF0 File Offset: 0x000BCFF0
		public void Dispose()
		{
			foreach (Light light in this.mLights.Values)
			{
				light.DisposeShadowMap();
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
			{
				animatedLevelPart.Dispose();
			}
			this.mModel.Dispose();
			this.mNavMesh = null;
		}

		// Token: 0x06001BC4 RID: 7108 RVA: 0x000BEEA0 File Offset: 0x000BD0A0
		internal bool SegmentIntersect(out float oFrac, Segment iSeg)
		{
			Vector3 vector;
			Vector3 vector2;
			AnimatedLevelPart animatedLevelPart;
			int num;
			return this.SegmentIntersect(out oFrac, out vector, out vector2, out animatedLevelPart, out num, iSeg, false);
		}

		// Token: 0x06001BC5 RID: 7109 RVA: 0x000BEEBE File Offset: 0x000BD0BE
		internal bool SegmentIntersect(out float oFrac, out Vector3 oPos, out Vector3 oNrm, out AnimatedLevelPart oAnimatedLevelPart, out int oPrim, Segment iSeg)
		{
			return this.SegmentIntersect(out oFrac, out oPos, out oNrm, out oAnimatedLevelPart, out oPrim, iSeg, false);
		}

		// Token: 0x06001BC6 RID: 7110 RVA: 0x000BEED0 File Offset: 0x000BD0D0
		internal bool SegmentIntersect(out float oFrac, out Vector3 oPos, out Vector3 oNrm, out AnimatedLevelPart oAnimatedLevelPart, out int oPrim, Segment iSeg, bool iIgnoreBackfaces)
		{
			oFrac = float.MaxValue;
			oPos = default(Vector3);
			oNrm = default(Vector3);
			oAnimatedLevelPart = null;
			oPrim = 0;
			float num;
			Vector3 vector;
			Vector3 vector2;
			if (this.mCollisionSkin.SegmentIntersect(out num, out vector, out vector2, out oPrim, iIgnoreBackfaces, iSeg))
			{
				oFrac = num;
				oPos = vector;
				oNrm = vector2;
			}
			for (int i = 0; i < this.mForceFields.Length; i++)
			{
				if (this.mForceFields[i].CollisionSkin.SegmentIntersect(out num, out vector, out vector2, iSeg) && num < oFrac)
				{
					oFrac = num;
					oPos = vector;
					oNrm = vector2;
					oPrim = 0;
				}
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
			{
				AnimatedLevelPart animatedLevelPart2;
				if (animatedLevelPart.SegmentIntersect(out num, out vector, out vector2, out animatedLevelPart2, iSeg) && num < oFrac)
				{
					oFrac = num;
					oPos = vector;
					oNrm = vector2;
					oAnimatedLevelPart = animatedLevelPart2;
					oPrim = (int)oAnimatedLevelPart.CollisionMaterial;
				}
			}
			return oFrac <= 1f;
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x000BEFF8 File Offset: 0x000BD1F8
		public void GetAllEffects(SortedList<int, GameScene.EffectStorage> iEffects)
		{
			for (int i = 0; i < this.mEffects.Length; i++)
			{
				GameScene.EffectStorage value;
				value.Effect = EffectManager.Instance.GetEffect(this.mEffects[i].Effect);
				value.Transform = this.mEffects[i].Transform;
				value.Animation = null;
				value.Range = this.mEffects[i].Range;
				value.Effect.Start(ref value.Transform);
				iEffects.Add(this.mEffects[i].ID, value);
			}
			foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
			{
				animatedLevelPart.GetAllEffects(iEffects);
			}
		}

		// Token: 0x04001E0F RID: 7695
		private const float NETWORK_UPDATE_PERIOD = 0.1f;

		// Token: 0x04001E10 RID: 7696
		protected float mNetworkUpdateTimer;

		// Token: 0x04001E11 RID: 7697
		private BiTreeModel mModel;

		// Token: 0x04001E12 RID: 7698
		private Dictionary<int, AnimatedLevelPart> mAnimatedLevelParts;

		// Token: 0x04001E13 RID: 7699
		private Dictionary<int, Light> mLights;

		// Token: 0x04001E14 RID: 7700
		private LevelModel.VisualEffectStorage[] mEffects;

		// Token: 0x04001E15 RID: 7701
		private LevelModel.PhysicsEntityStorage[] mPhysEntities;

		// Token: 0x04001E16 RID: 7702
		private Liquid[] mWaters;

		// Token: 0x04001E17 RID: 7703
		private ForceField[] mForceFields;

		// Token: 0x04001E18 RID: 7704
		private CollisionSkin mCollisionSkin;

		// Token: 0x04001E19 RID: 7705
		private Dictionary<int, TriggerArea> mTriggerAreas = new Dictionary<int, TriggerArea>();

		// Token: 0x04001E1A RID: 7706
		private Dictionary<int, Locator> mLocators = new Dictionary<int, Locator>();

		// Token: 0x04001E1B RID: 7707
		private NavMesh mNavMesh;

		// Token: 0x04001E1C RID: 7708
		private TriangleMesh mCameraMesh;

		// Token: 0x0200038A RID: 906
		protected struct PhysicsEntityStorage
		{
			// Token: 0x04001E1D RID: 7709
			public Matrix StartTransform;

			// Token: 0x04001E1E RID: 7710
			public PhysicsEntityTemplate Template;
		}

		// Token: 0x0200038B RID: 907
		internal struct VisualEffectStorage
		{
			// Token: 0x04001E1F RID: 7711
			public int ID;

			// Token: 0x04001E20 RID: 7712
			public Matrix Transform;

			// Token: 0x04001E21 RID: 7713
			public float Range;

			// Token: 0x04001E22 RID: 7714
			public int Effect;
		}
	}
}
