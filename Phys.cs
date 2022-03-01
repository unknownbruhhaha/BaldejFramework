using BulletSharp;

using GameEngineAPI.Components;

namespace GameEngineAPI
{
    public static class Phys
    {
        public static CollisionConfiguration collisionConfiguration = new DefaultCollisionConfiguration();
        public static Dispatcher dispatcher = new CollisionDispatcher(collisionConfiguration);
        public static BroadphaseInterface broadphase = new DbvtBroadphase();
        public static ConstraintSolver solver = new SequentialImpulseConstraintSolver();
        public static DynamicsWorld world = new DiscreteDynamicsWorld(dispatcher, broadphase, solver, collisionConfiguration);
        public static Vector3 gravityVector = new Vector3(0, -9.8f, 0);

        public static void Init()
        {
            world.Gravity = gravityVector;
        }

        public static void Update()
        {
            world.StepSimulation(Render.Render.deltaTime);
        }
    }
}
