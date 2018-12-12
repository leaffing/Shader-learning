using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class MovementSystem : ComponentSystem
{
    //这里声明一个结构, 其中包含我们定义的过滤条件, 也就是必须拥有CameraComponent组件才会被注入。
    public struct Group
    {
        public readonly int Length;

        public ComponentDataArray<VelocityComponent> Velocities;

        public ComponentDataArray<Position> Positions;
    }

    //然后声明结构类型的字段, 并且加上[Inject]
    [Inject] Group data;

    public struct GameObject
    {
        public readonly int Length;

        public ComponentArray<Transform> Transforms;

        public ComponentDataArray<VelocityComponent> Velocities;
    }

    [Inject] GameObject go;

    protected override void OnUpdate()
    {
        float deltaTime = Time.deltaTime;

        for (int i = 0; i < go.Length; i++)
        {
            float3 pos = go.Transforms[i].position; //Read
            float3 vector = go.Velocities[i].moveDir * 0.1f; //Read

            pos += vector * deltaTime; //Move

            go.Transforms[i].position = pos; //Write
        }

        for (int i = 0; i < data.Length; i++)
        {
            float3 pos = data.Positions[i].Value;             //Read
            float3 vector = data.Velocities[i].moveDir * 10f;       //Read

            pos += vector * deltaTime; //Move

            data.Positions[i] = new Position { Value = pos }; //Write
        }
    }

    //unsafe struct Group
    //{
    //    public Transform Transform;
    //    public Rotation* Camera;
    //}

    //protected override unsafe void OnUpdate()
    //{
    //    foreach (var item in GetEntities<Group>())
    //    {
    //        var cam = *item.Camera;
    //        Debug.Log(item.Transform + "" + cam);
    //    }
    //}
}