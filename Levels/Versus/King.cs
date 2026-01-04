// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Versus.King
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Xml;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.Levels.Versus;

internal class King : VersusRuleset
{
  private King.TorchRenderData[] mTorchRenderData;
  private King.HillRenderData[] mHillRenderData;
  private AnimationController mAnimationController;
  private AnimationClip mAnimationClip;
  private Body mBody;
  private CollisionSkin mCollisionSkin;
  private byte[] mScores = new byte[4];
  private float[] mRespawnTimers = new float[4];
  private float mTimeLimitTimer;
  private float mTimeLimitNetworkUpdate;
  private float mTimeLimitTarget;
  private float mTimeLimit;
  private int mScoreLimit;
  private float mRespawnTime;
  private bool mTeams;
  private King.Settings mSettings;
  private Matrix mOrientation;
  private Vector3 mBodyOffScreenPosition = new Vector3(0.0f, -100f, 0.0f);

  public King(GameScene iScene, XmlNode iNode, King.Settings iSettings)
    : base(iScene, iNode)
  {
    this.mSettings = iSettings;
    SkinnedModel skinnedModel1;
    SkinnedModel skinnedModel2;
    Model model;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      skinnedModel1 = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthbarrier_animation");
      skinnedModel2 = Magicka.Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthbarrier4_mesh");
      model = Magicka.Game.Instance.Content.Load<Model>("Models/Effects/torch");
    }
    Matrix rotationX = Matrix.CreateRotationX(-1.57079637f);
    this.mBody = new Body();
    this.mCollisionSkin = new CollisionSkin(this.mBody);
    this.mCollisionSkin.AddPrimitive((Primitive) new Capsule(new Vector3(0.0f, 0.5f, 0.0f), rotationX, 0.5f, 2f), 1, new MaterialProperties(0.0f, 0.0f, 0.0f));
    this.mBody.CollisionSkin = this.mCollisionSkin;
    this.mBody.Mass = 50f;
    this.mBody.Immovable = true;
    this.mBody.Tag = (object) this;
    this.mAnimationController = new AnimationController();
    this.mAnimationController.Skeleton = skinnedModel1.SkeletonBones;
    this.mAnimationClip = skinnedModel1.AnimationClips["emerge4"];
    SkinnedModelDeferredBasicMaterial oMaterial;
    Helper.SkinnedModelDeferredMaterialFromBasicEffect(skinnedModel2.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out oMaterial);
    this.mHillRenderData = new King.HillRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mHillRenderData[index] = new King.HillRenderData();
      this.mHillRenderData[index].mMaterial = oMaterial;
      this.mHillRenderData[index].SetMesh(skinnedModel2.Model.Meshes[0].VertexBuffer, skinnedModel2.Model.Meshes[0].IndexBuffer, skinnedModel2.Model.Meshes[0].MeshParts[0], 0, 3, 4);
    }
    this.mTorchRenderData = new King.TorchRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mTorchRenderData[index] = new King.TorchRenderData();
      this.mTorchRenderData[index].SetMesh(model.Meshes[0], model.Meshes[0].MeshParts[0], 4, 0, 5);
    }
    this.mOrientation = Matrix.Identity;
  }

  public override void OnPlayerDeath(Magicka.GameLogic.Player iPlayer)
  {
    if (this.mScene.PlayState.IsGameEnded && (double) this.mRespawnTimers[iPlayer.ID] <= 0.0 || (double) this.mRespawnTimers[iPlayer.ID] > 0.0)
      return;
    this.mRespawnTimers[iPlayer.ID] = this.mRespawnTime;
  }

  private void ActivateHill(ref Vector3 iPosition)
  {
    this.mAnimationController.StartClip(this.mAnimationClip, false);
  }

  public override void Initialize()
  {
    base.Initialize();
    this.mTimeLimit = (float) this.mSettings.TimeLimit * 60f;
    this.mTimeLimitTimer = this.mTimeLimit;
    this.mTimeLimitTarget = this.mTimeLimit;
    this.mScoreLimit = this.mSettings.ScoreLimit;
    this.mRespawnTime = 5f;
    this.mTeams = this.mSettings.TeamsEnabled;
    if (this.mTeams)
    {
      this.mScoreUIs.Add(new VersusRuleset.Score(true));
      this.mScoreUIs.Add(new VersusRuleset.Score(false));
      int index = 0;
      int num = 0;
      for (; index < this.mPlayers.Length; ++index)
      {
        if (this.mPlayers[index].Playing)
        {
          if (num % 2 == 0)
          {
            this.mPlayers[index].Team |= Factions.TEAM_RED;
            this.mPlayers[index].Team &= ~Factions.TEAM_BLUE;
          }
          else
          {
            this.mPlayers[index].Team |= Factions.TEAM_BLUE;
            this.mPlayers[index].Team &= ~Factions.TEAM_RED;
          }
          ++num;
        }
      }
      for (int key = 0; key < this.mPlayers.Length; ++key)
      {
        if (this.mPlayers[key].Playing)
        {
          Texture2D portrait = this.mPlayers[key].Gamer.Avatar.Portrait;
          this.mPlayers[key].Avatar.Faction &= ~Factions.FRIENDLY;
          if ((this.mPlayers[key].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
          {
            this.mIDToScoreUILookUp[key] = 0;
            this.mScoreUIs[0].AddPlayer(this.mPlayers[key].GamerTag, this.mPlayers[key].ID, portrait, Defines.PLAYERCOLORS[(int) this.mPlayers[key].Color]);
          }
          else
          {
            this.mIDToScoreUILookUp[key] = 1;
            this.mScoreUIs[1].AddPlayer(this.mPlayers[key].GamerTag, this.mPlayers[key].ID, portrait, Defines.PLAYERCOLORS[(int) this.mPlayers[key].Color]);
          }
        }
      }
    }
    else
    {
      int key = 0;
      int num = 0;
      for (; key < this.mPlayers.Length; ++key)
      {
        if (this.mPlayers[key].Playing)
        {
          this.mPlayers[key].Avatar.Faction &= ~Factions.FRIENDLY;
          Texture2D portrait = this.mPlayers[key].Gamer.Avatar.Portrait;
          VersusRuleset.Score score = new VersusRuleset.Score(num % 2 == 0);
          score.AddPlayer(this.mPlayers[key].GamerTag, this.mPlayers[key].ID, portrait, Defines.PLAYERCOLORS[(int) this.mPlayers[key].Color]);
          this.mIDToScoreUILookUp[key] = this.mScoreUIs.Count;
          this.mScoreUIs.Add(score);
          ++num;
        }
      }
    }
    for (int index = 0; index < this.mScores.Length; ++index)
      this.mScores[index] = (byte) 0;
    for (int index = 0; index < this.mRespawnTimers.Length; ++index)
      this.mRespawnTimers[index] = 0.0f;
    this.mBody.MoveTo(this.mBodyOffScreenPosition, Matrix.Identity);
    this.mBody.EnableBody();
  }

  public override void DeInitialize() => this.mBody.DisableBody();

  public override void Update(float iDeltaTime, DataChannel iDataChannel)
  {
    if (iDataChannel == DataChannel.None)
      return;
    if (this.mTeams)
    {
      for (int iID = 0; iID < this.mRespawnTimers.Length; ++iID)
      {
        float num1 = Math.Max(this.mRespawnTimers[iID] - iDeltaTime, 0.0f);
        if ((double) this.mRespawnTimers[iID] > 0.0 && (double) num1 <= 0.0 && this.mPlayers[iID].Playing)
        {
          int teamArea = this.GetTeamArea(this.mPlayers[iID].Team);
          Matrix oOrientation;
          this.GetMatrix(teamArea, out oOrientation);
          int num2 = (int) this.RevivePlayer(iID, teamArea, ref oOrientation, new ushort?());
        }
        this.mRespawnTimers[iID] = num1;
      }
    }
    else
    {
      for (int iID = 0; iID < this.mRespawnTimers.Length; ++iID)
      {
        float num3 = Math.Max(this.mRespawnTimers[iID] - iDeltaTime, 0.0f);
        if ((double) this.mRespawnTimers[iID] > 0.0 && (double) num3 <= 0.0 && this.mPlayers[iID].Playing)
        {
          int teamArea = this.GetTeamArea(this.mPlayers[iID].Team);
          Matrix oOrientation;
          this.GetMatrix(teamArea, out oOrientation);
          int num4 = (int) this.RevivePlayer(iID, teamArea, ref oOrientation, new ushort?());
        }
        this.mRespawnTimers[iID] = num3;
      }
    }
    for (int index1 = 0; index1 < this.mRespawnTimers.Length; ++index1)
    {
      int index2;
      if (this.mIDToScoreUILookUp.TryGetValue(index1, out index2) && index2 != -1)
        this.mScoreUIs[index2].SetTimer(index1, (int) this.mRespawnTimers[index1]);
    }
    if ((double) this.mTimeLimit > 0.0)
    {
      if (!this.mScene.PlayState.IsGameEnded)
      {
        this.mTimeLimitTimer -= iDeltaTime;
        if ((double) this.mTimeLimitTimer <= 0.0)
        {
          this.EndGame();
          this.mTimeLimitTimer = this.mTimeLimit;
        }
      }
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        if ((double) this.mTimeLimitNetworkUpdate > 1.0)
        {
          NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref new RulesetMessage()
          {
            Type = this.RulesetType,
            Byte01 = (byte) 0,
            Float01 = this.mTimeLimitTimer
          });
          this.mTimeLimitNetworkUpdate = 0.0f;
        }
        this.mTimeLimitNetworkUpdate += iDeltaTime;
      }
    }
    Matrix mOrientation = this.mOrientation with
    {
      Translation = new Vector3()
    };
    this.mAnimationController.Update(iDeltaTime, ref mOrientation, true);
    if (this.mAnimationController.HasFinished)
    {
      if (this.mAnimationController.PlaybackMode == PlaybackMode.Forward)
      {
        this.mAnimationController.PlaybackMode = PlaybackMode.Backward;
        this.mAnimationController.StartClip(this.mAnimationClip, false);
      }
      else
      {
        this.mAnimationController.PlaybackMode = PlaybackMode.Forward;
        this.mAnimationController.StartClip(this.mAnimationClip, false);
      }
    }
    this.mBody.MoveTo(new Vector3(), this.mOrientation);
    King.HillRenderData iObject1 = this.mHillRenderData[(int) iDataChannel];
    this.mAnimationController.SkinnedBoneTransforms.CopyTo((Array) iObject1.Bones, 0);
    this.mScene.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject1);
    King.TorchRenderData iObject2 = this.mTorchRenderData[(int) iDataChannel];
    iObject2.Transformation = mOrientation;
    this.mScene.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject2);
    VersusRuleset.RenderData renderData = this.mRenderData[(int) iDataChannel];
    renderData.DrawTime = (double) this.mTimeLimit > 0.0;
    renderData.TimeLimit = this.mTimeLimit;
    renderData.Time = this.mTimeLimitTimer;
    if ((double) this.mTimeLimitTimer <= 10.0)
      renderData.SetTimeText((int) this.mTimeLimitTimer);
    else
      renderData.SetTimeText(0);
    base.Update(iDeltaTime, iDataChannel);
  }

  public override void LocalUpdate(float iDeltaTime, DataChannel iDataChannel)
  {
    if (iDataChannel == DataChannel.None)
      return;
    for (int iID = 0; iID < this.mRespawnTimers.Length; ++iID)
    {
      this.mRespawnTimers[iID] = Math.Max(this.mRespawnTimers[iID] - iDeltaTime, 0.0f);
      for (int index = 0; index < this.mScoreUIs.Count; ++index)
        this.mScoreUIs[index].SetTimer(iID, (int) this.mRespawnTimers[iID]);
    }
    VersusRuleset.RenderData renderData = this.mRenderData[(int) iDataChannel];
    this.mTimeLimitTimer += (this.mTimeLimitTarget - this.mTimeLimitTimer) * iDeltaTime;
    renderData.DrawTime = (double) this.mTimeLimit > 0.0;
    renderData.TimeLimit = this.mTimeLimit;
    renderData.Time = this.mTimeLimitTimer;
    if ((double) this.mTimeLimitTimer <= 10.0)
      renderData.SetTimeText((int) this.mTimeLimitTimer);
    else
      renderData.SetTimeText(0);
    base.LocalUpdate(iDeltaTime, iDataChannel);
  }

  public override void NetworkUpdate(ref RulesetMessage iMsg)
  {
    if (iMsg.Byte01 == (byte) 0)
    {
      float num = NetworkManager.Instance.Interface.GetLatency(0) * 0.5f;
      this.mTimeLimitTarget = iMsg.Float01 - num;
    }
    else
      base.NetworkUpdate(ref iMsg);
  }

  public override Rulesets RulesetType => Rulesets.King;

  public override bool CanRevive(Magicka.GameLogic.Player iReviver, Magicka.GameLogic.Player iRevivee)
  {
    return this.mTeams && (iReviver.Team & iRevivee.Team) != Factions.NONE;
  }

  internal override short[] GetScores() => (short[]) null;

  internal override bool Teams => false;

  internal override short[] GetTeamScores() => (short[]) null;

  private class HillRenderData : 
    RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
  {
    public Matrix[] Bones;

    public HillRenderData() => this.Bones = new Matrix[80 /*0x50*/];

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelDeferredEffect).Bones = this.Bones;
      base.Draw(iEffect, iViewFrustum);
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      (iEffect as SkinnedModelDeferredEffect).Bones = this.Bones;
      base.DrawShadow(iEffect, iViewFrustum);
    }
  }

  private class TorchRenderData : RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>
  {
    public Matrix Transformation;

    public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      float num1 = 0.0f;
      int num2 = 0;
      while (num2 < 8)
      {
        Matrix transformation = this.Transformation;
        Vector3 result1 = transformation.Translation;
        Matrix result2;
        Matrix.CreateRotationY((float) (6.2831854820251465 * ((double) num1 / 8.0)), out result2);
        Vector3 result3 = result2.Forward;
        Vector3.Multiply(ref result3, 5.5f, out result3);
        Vector3.Add(ref result3, ref result1, out result1);
        transformation.Translation = result1;
        this.mMaterial.WorldTransform = transformation;
        base.Draw(iEffect, iViewFrustum);
        ++num2;
        ++num1;
      }
    }

    public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      float num1 = 0.0f;
      int num2 = 0;
      while (num2 < 8)
      {
        Matrix transformation = this.Transformation;
        Vector3 result1 = transformation.Translation;
        Matrix result2;
        Matrix.CreateRotationY((float) (6.2831854820251465 * ((double) num1 / 8.0)), out result2);
        Vector3 result3 = result2.Forward;
        Vector3.Multiply(ref result3, 5.5f, out result3);
        Vector3.Add(ref result3, ref result1, out result1);
        transformation.Translation = result1;
        this.mMaterial.WorldTransform = transformation;
        base.DrawShadow(iEffect, iViewFrustum);
        ++num2;
        ++num1;
      }
    }
  }

  internal new class Settings : VersusRuleset.Settings
  {
    private DropDownBox<int> mTimeLimit;
    private DropDownBox<int> mScoreLimit;
    private DropDownBox<bool> mTeams;

    public Settings()
    {
      this.mTimeLimit = this.AddOption<int>(this.LOC_TIME_LIMIT, this.LOC_TT_TIME, new int[5]
      {
        0,
        5,
        10,
        30,
        50
      }, new int?[5]
      {
        new int?(this.LOC_UNLIMITED),
        new int?(),
        new int?(),
        new int?(),
        new int?()
      });
      this.mTimeLimit.SelectedIndex = 0;
      this.mScoreLimit = this.AddOption<int>(this.LOC_SCORE_LIMIT, this.LOC_TT_SCORE, new int[5]
      {
        0,
        5,
        10,
        20,
        50
      }, new int?[5]
      {
        new int?(this.LOC_UNLIMITED),
        new int?(),
        new int?(),
        new int?(),
        new int?()
      });
      this.mScoreLimit.SelectedIndex = 3;
      this.mTeams = this.AddOption<bool>(this.LOC_TEAMS, this.LOC_TT_TEAMS, new bool[2]
      {
        false,
        true
      }, new int?[2]
      {
        new int?(this.LOC_NO),
        new int?(this.LOC_YES)
      });
      this.mTeams.SelectedIndex = 0;
    }

    public int TimeLimit => this.mTimeLimit.SelectedValue;

    public int ScoreLimit => this.mScoreLimit.SelectedValue;

    public override bool TeamsEnabled => this.mTeams.SelectedValue;
  }
}
