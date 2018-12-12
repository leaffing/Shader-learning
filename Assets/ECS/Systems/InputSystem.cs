using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class InputSystem : ComponentSystem
{
    public struct Group
    {
        public readonly int Length;

        public ComponentDataArray<PlayerComponent> Players;

        public ComponentDataArray<InputComponent> Inputs;

        public ComponentDataArray<VelocityComponent> Velocities;
    }

    [Inject] Group data;

    protected override void OnUpdate()
    {
        for (int i = 0; i < data.Length; i++)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            float3 normalized = new float3();

            if (x != 0 || z != 0)
                normalized = math.normalize(new float3(x, 0, z));

            //Write
            data.Velocities[i] = new VelocityComponent { moveDir = normalized };
        }
    }
}