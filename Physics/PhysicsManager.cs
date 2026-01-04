// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.PhysicsManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Physics;

public class PhysicsManager
{
  private PhysicsSystem mSimulator;
  private static PhysicsManager mSingelton;
  private static volatile object mSingeltonLock = new object();
  private bool mIsFrozen;

  public static PhysicsManager Instance
  {
    get
    {
      if (PhysicsManager.mSingelton == null)
      {
        lock (PhysicsManager.mSingeltonLock)
        {
          if (PhysicsManager.mSingelton == null)
            PhysicsManager.mSingelton = new PhysicsManager();
        }
      }
      return PhysicsManager.mSingelton;
    }
  }

  private PhysicsManager() => this.CreateSimulator();

  private void CreateSimulator()
  {
    this.mSimulator = new PhysicsSystem();
    this.mSimulator.Gravity = Vector3.Down * 9.82f * 2f;
    this.mSimulator.CollisionSystem = (CollisionSystem) new CollisionSystemSAP();
    this.mSimulator.CollisionSystem.RegisterCollDetectFunctor((DetectFunctor) new HollowSphereCapsule());
    this.mSimulator.CollisionSystem.RegisterCollDetectFunctor((DetectFunctor) new HollowSphereSphere());
    this.mSimulator.CollisionSystem.RegisterCollDetectFunctor((DetectFunctor) new HollowSphereBox());
    this.mSimulator.CollisionSystem.RegisterCollDetectFunctor((DetectFunctor) new HollowSphereStaticMesh());
    this.mSimulator.CollisionSystem.RegisterCollDetectFunctor((DetectFunctor) new CustomCapsuleStaticMesh());
    this.mSimulator.CollisionSystem.RegisterCollDetectFunctor((DetectFunctor) new CustomCapsulePlane());
    this.mSimulator.CollisionSystem.RegisterCollDetectFunctor((DetectFunctor) new CustomCapsuleCapsule());
    this.mSimulator.EnableFreezing = true;
    this.mSimulator.AllowedPenetration = 1f / 1000f;
    this.mSimulator.SolverType = PhysicsSystem.Solver.Normal;
    this.mSimulator.CollisionSystem.UseSweepTests = true;
    this.mSimulator.IsShockStepEnabled = false;
  }

  public void Update(float iDeltaTime)
  {
    if (this.mIsFrozen)
      return;
    this.mSimulator.Integrate(iDeltaTime);
  }

  public void Clear()
  {
    while (this.mSimulator.Bodies.Count > 0)
      this.mSimulator.Bodies[0].DisableBody();
    while (this.mSimulator.CollisionSystem.CollisionSkins.Count > 0)
      this.mSimulator.CollisionSystem.RemoveCollisionSkin(this.mSimulator.CollisionSystem.CollisionSkins[0]);
  }

  public void Freeze() => this.mIsFrozen = true;

  public void UnFreeze() => this.mIsFrozen = false;

  public PhysicsSystem Simulator => this.mSimulator;
}
