using BulletSharp;
using OpenTK.Mathematics;

using GameEngineAPI;

namespace GameEngineAPI.Components
{
    public class RigidTransform3D : Transform
    {
        public string componentID { get => "Transform"; }

        public GameObject? owner { get; set; }

        // transform
        public OpenTK.Mathematics.Vector3 Position { get { return new OpenTK.Mathematics.Vector3(_position.X, _position.Y, _position.Z); } set { _position = new BulletSharp.Vector3(value.X, value.Y, value.Z); } }
        public OpenTK.Mathematics.Quaternion Rotation { get; set; }
        public OpenTK.Mathematics.Vector3 Scale { get; set; }

        // bs transform
        BulletSharp.Quaternion _quaternion = new BulletSharp.Quaternion();
        BulletSharp.Vector3 _position = new BulletSharp.Vector3();

        //collision
        public string shapeType = "Box";
        public static OpenTK.Mathematics.Vector3 size = new OpenTK.Mathematics.Vector3(1, 1, 1);
        public static CollisionShape shape;
        MotionState motion = new DefaultMotionState();
        public static RigidBodyConstructionInfo rbCI;
        static RigidBody body;

        public RigidTransform3D(string ShapeType, float Mass)
        {
            this.shapeType = ShapeType;
            if (shapeType == "Box")
            {
                BulletSharp.Vector3 colShapeSize = new BulletSharp.Vector3(size.X, size.Y, size.Z);
                CollisionShape boxShape = new BulletSharp.BoxShape(colShapeSize);

                shape = boxShape;
            }

            BulletSharp.Vector3 interna = shape.CalculateLocalInertia(Mass);

            rbCI = new RigidBodyConstructionInfo(Mass, motion, shape, interna);

            body = new RigidBody(rbCI);

            body.SetMassProps(Mass, interna);

            Console.WriteLine(Mass);

            Phys.world.AddRigidBody(body);
        }

        public void OnUpdate()
        {
            if (shapeType == "Box")
            {
                shape = new BoxShape(new BulletSharp.Vector3(size.X, size.Y, size.Z));
            }

            body.LinearVelocity = new BulletSharp.Vector3(0, -1, 0);

            _position = body.MotionState.WorldTransform.Origin;

            //Console.WriteLine(Position);
        }
    }
}
