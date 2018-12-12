#if ECS_F
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

public class Bootstrap
{
    private static EntityManager entityManager;     //所有实体的管理器, 提供操作Entity的API

    private static EntityArchetype playerArchetype; //Entity原型, 可以看成由组件组成的数组

    [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Awake()
    {
        entityManager = World.Active.GetOrCreateManager<EntityManager>();

        //下面的的Position类型需要引入Unity.Transforms命名空间
        playerArchetype = entityManager.CreateArchetype(typeof(Position));
    }

    [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Start()
    {
        //获取Cube
        GameObjectEntity cubeEntity = GameObject.Find("Cube").AddComponent<GameObjectEntity>();

        //添加Velocity组件
        entityManager.AddComponentData(cubeEntity.Entity, new VelocityComponent
        { moveDir = new Unity.Mathematics.float3(0, 1, 0) });


        //把GameObect.Find放在这里因为场景加载完成前无法获取游戏物体。
        GameObject playerGo = GameObject.Find("Player");

        //下面的类型是一个Struct, 需要引入Unity.Rendering命名空间
        MeshInstanceRenderer playerRenderer =
            playerGo.GetComponent<MeshInstanceRendererComponent>().Value;

        Object.Destroy(playerGo); //获取到渲染数据后可以销毁空物体

        Entity player = entityManager.CreateEntity(playerArchetype);    // Position
        //添加PlayerComponent组件
        entityManager.AddComponentData(player, new PlayerComponent());  // PlayerComponent
        entityManager.AddComponentData(player, new VelocityComponent());// VelocityComponent
        entityManager.AddComponentData(player, new InputComponent());   // InputComponent
        // 向实体添加共享的数据
        entityManager.AddSharedComponentData(player, playerRenderer);   // MeshInstanceRenderer

        //修改实体的Position组件
        entityManager.SetComponentData(player, new Position { Value = new Unity.Mathematics.float3(0, 0.5f, 0) });
    }
}
#endif