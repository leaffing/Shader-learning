#if ECS
namespace MultiThread
{
    using UnityEngine;
    using UnityEngine.UI;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Burst;
    using Unity.Rendering;
    using Unity.Transforms;
    using Unity.Mathematics;
    using Unity.Collections;
    using Random = UnityEngine.Random;

    public struct PlayerInput : IComponentData
    {
        public float3 Vector;
    }

    public struct EnemyComponent : IComponentData
    {
    }

    public struct CameraComponent : IComponentData
    {
    }

    public struct Health : IComponentData
    {
        public int Value;
    }

    public struct Velocity : IComponentData
    {
        public float Value;
    }

    public class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Start()
        {
            EntityManager manager = World.Active.GetOrCreateManager<EntityManager>();

            GameObject player = GameObject.FindWithTag("Player");
            GameObject enemy = GameObject.FindWithTag("Enemy");
            GameObject camera = GameObject.FindWithTag("MainCamera");
            Text enemyCount = GameObject.Find("EnemyCountText").GetComponent<Text>();

            //获取Player MeshInstanceRenderer
            MeshInstanceRenderer playerRenderer = player.GetComponent<MeshInstanceRendererComponent>().Value;
            Object.Destroy(player);
            //获取Enemy MeshInstanceRenderer
            MeshInstanceRenderer enemyRenderer = enemy.GetComponent<MeshInstanceRendererComponent>().Value;
            Object.Destroy(enemy);
            //初始化玩家实体
            Entity entity = manager.CreateEntity();
            manager.AddComponentData(entity, new PlayerInput { });
            manager.AddComponentData(entity, new Position { Value = new float3(0, 0, 0) });
            manager.AddComponentData(entity, new Velocity { Value = 7 });
            manager.AddSharedComponentData(entity, playerRenderer);
            //初始化摄像机实体
            GameObjectEntity gameObjectEntity = camera.AddComponent<GameObjectEntity>();
            manager.AddComponentData(gameObjectEntity.Entity, new CameraComponent());
            //初始化UI系统
            UISystem uISystem = World.Active.GetOrCreateManager<UISystem>();
            uISystem.Init(enemyCount);
            //初始化敌人生成系统
            EnemySpawnSystem enemySpawnSystem = World.Active.GetOrCreateManager<EnemySpawnSystem>();
            enemySpawnSystem.Init(manager, enemyRenderer);
            //初始化敌人碰撞系统
            EnemyCollisionSystem collisionSystem = World.Active.GetOrCreateManager<EnemyCollisionSystem>();
            collisionSystem.Init(playerRenderer.mesh.bounds.size.x / 2, enemyRenderer.mesh.bounds.size.x / 2);
        }
    }

    public class PlayerInputSystem : ComponentSystem
    {
        struct Player
        {
            public readonly int Length;
            public ComponentDataArray<PlayerInput> playerInput;
        }

        [Inject] Player player;

        protected override void OnUpdate()
        {
            for (int i = 0; i < player.Length; i++)
            {
                float3 normalized = new float3();
                float x = Input.GetAxisRaw("Horizontal");
                float y = Input.GetAxisRaw("Vertical");
                if (x != 0 || y != 0)
                    normalized = math.normalize(new float3(x, y, 0));
                //归一化
                player.playerInput[i] = new PlayerInput { Vector = normalized };
            }
        }
    }

    public class PlayerMoveSystem : ComponentSystem
    {
        struct Player
        {
            public readonly int Length;
            public ComponentDataArray<Position> positions;
            public ComponentDataArray<PlayerInput> playerInput;
            public ComponentDataArray<Velocity> velocities;
        }

        [Inject] Player player;

        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < player.Length; i++)
            {
                //Read
                Position position = player.positions[i];
                PlayerInput input = player.playerInput[i];
                Velocity velocity = player.velocities[i];

                position.Value += new float3(input.Vector * velocity.Value * deltaTime);
                //Write
                player.positions[i] = position;
            }
        }
    }

    [UpdateAfter(typeof(PlayerMoveSystem))]
    public class CameraMoveSystem : ComponentSystem
    {
        struct Player
        {
            public readonly int Length;
            public ComponentDataArray<PlayerInput> playerInputs;
            public ComponentDataArray<Position> positions;
        }
        struct Cam
        {
            public ComponentDataArray<CameraComponent> cameras;
            public ComponentArray<Transform> transforms;
        }
        [Inject] Player player;
        [Inject] Cam cam;

        protected override void OnUpdate()
        {
            if (player.Length == 0) //玩家死亡
                return;
            float3 pos = player.positions[0].Value;
            //相机跟随
            cam.transforms[0].position = new Vector3(pos.x, pos.y, cam.transforms[0].position.z);
        }
    }

    [AlwaysUpdateSystem]
    public class UISystem : ComponentSystem
    {
        Text enemyCount;

        public void Init(Text enemyCount) => this.enemyCount = enemyCount;

        struct Player
        {
            public readonly int Length;
            public ComponentDataArray<PlayerInput> playerInputs;
        }
        struct Enemy
        {
            public readonly int Length;
            public ComponentDataArray<EnemyComponent> enemies;
        }
        [Inject] Player player;
        [Inject] Enemy enemy;

        protected override void OnUpdate()
        {
            if (player.Length == 0) //玩家死亡
                return;

            enemyCount.text = "敌人数量:" + enemy.Length;
        }
    }

    public class EnemySpawnSystem : ComponentSystem
    {
        EntityManager manager;
        MeshInstanceRenderer enemyLook;
        float timer;

        public void Init(EntityManager manager, MeshInstanceRenderer enemyLook)
        {
            this.manager = manager;
            this.enemyLook = enemyLook;
            timer = 0;
        }

        struct Player
        {
            public readonly int Length;
            public ComponentDataArray<PlayerInput> playerInputs;
            public ComponentDataArray<Position> positions;
        }

        [Inject] Player player;

        protected override void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= 0.1f)
            {
                timer = 0;
                CreatEnemy();
            }
        }

        void CreatEnemy()
        {
            if (player.Length == 0) //玩家死亡
                return;
            float3 playerPos = player.positions[0].Value;

            for (int i = 0; i < 50; i++)
            {
                Entity entity = manager.CreateEntity();

                int angle = Random.Range(1, 360);        //在玩家什么角度刷出来
                float distance = Random.Range(15f, 25f); //距离玩家多远刷出来
                float y = Mathf.Sin(angle) * distance;
                float x = y / Mathf.Tan(angle);
                float3 positon = new float3(playerPos.x + x, playerPos.y + y, 0);
                //初始化敌人及属性
                manager.AddComponentData(entity, new EnemyComponent { });
                manager.AddComponentData(entity, new Health { Value = 1 });
                manager.AddComponentData(entity, new Position { Value = positon });
                manager.AddComponentData(entity, new Velocity { Value = 1 });
                manager.AddSharedComponentData(entity, enemyLook);
            }
        }
    }

    //最关键的并行系统：EnemyMove跟EnemyCollision还没有实现。
    //难点中的难点来了，EnemyMoveSystem需要继承JobComponent系统来实现并行
    public class EnemyMoveSystem : JobComponentSystem
    {
        ComponentGroup enemyGroup; //由一系列组件组成
        ComponentGroup playerGroup;

        //系统创建时调用
        protected override void OnCreateManager()
        {
            //声明该组所需的组件，包括读写依赖
            enemyGroup = GetComponentGroup
            (
                ComponentType.ReadOnly(typeof(Velocity)),
                ComponentType.ReadOnly(typeof(EnemyComponent)),
                typeof(Position)
            );
            playerGroup = GetComponentGroup
            (
                ComponentType.ReadOnly(typeof(PlayerInput)),
                ComponentType.ReadOnly(typeof(Position))
            );
        }

        [BurstCompile]//使用Burst编译
        struct EnemyMoveJob : IJobParallelFor //继承该接口实现并行
        {
            public float deltaTime;
            public float3 playerPos;
            //记得声明读写关系
            public ComponentDataArray<Position> positions;
            [ReadOnly] public ComponentDataArray<Velocity> velocities;

            public void Execute(int i) //会被不同的线程调用，所以方法中不能存在引用类型。
            {
                //Read
                float3 position = positions[i].Value;
                float speed = velocities[i].Value;
                //算出朝向玩家的向量
                float3 vector = playerPos - position;
                vector = math.normalize(vector);

                float3 newPos = position + vector * speed * deltaTime;
                //Wirte
                positions[i] = new Position { Value = newPos };
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) //每帧调用
        {
            if (playerGroup.CalculateLength() == 0) //玩家死亡
                return base.OnUpdate(inputDeps);

            float3 playerPos = playerGroup.GetComponentDataArray<Position>()[0].Value;

            EnemyMoveJob job = new EnemyMoveJob
            {
                deltaTime = Time.deltaTime,
                playerPos = playerPos,
                positions = enemyGroup.GetComponentDataArray<Position>(),  //声明了组件后，Get时会进行组件的获取
                velocities = enemyGroup.GetComponentDataArray<Velocity>()
            };
            return job.Schedule(enemyGroup.CalculateLength(), 64, inputDeps);//第一个参数意味着每个job.Execute的执行次数
        }
    }

    [UpdateAfter(typeof(PlayerMoveSystem))] //逻辑上依赖于玩家移动系统，所以声明更新时序
    public class EnemyCollisionSystem : JobComponentSystem
    {
        float playerRadius;
        float enemyRadius;
        public void Init(float playerRadius, float enemyRadius)
        {
            this.playerRadius = playerRadius;
            this.enemyRadius = enemyRadius;
        }

        ComponentGroup enemyGroup;
        ComponentGroup playerGroup;

        protected override void OnCreateManager()
        {
            enemyGroup = GetComponentGroup
            (
                ComponentType.ReadOnly(typeof(EnemyComponent)),
                typeof(Health),
                ComponentType.ReadOnly(typeof(Position))
            );
            playerGroup = GetComponentGroup
            (
                ComponentType.ReadOnly(typeof(PlayerInput)),
                ComponentType.ReadOnly(typeof(Position))
            );
        }

        [BurstCompile]
        struct EnemyCollisionJob : IJobParallelFor
        {
            public int collisionDamage; //碰撞对双方造成的伤害
            public float playerRadius;
            public float enemyRadius;
            public float3 playerPos;
            [ReadOnly] public ComponentDataArray<Position> positions;
            public ComponentDataArray<Health> enemies;

            public void Execute(int i)
            {
                float3 position = positions[i].Value;
                float x = math.abs(position.x - playerPos.x);
                float y = math.abs(position.y - playerPos.y);
                //距离
                float magnitude = math.sqrt(x * x + y * y);

                //圆形碰撞检测
                if (magnitude < playerRadius + enemyRadius)
                {
                    //Read
                    int health = enemies[i].Value;
                    //Write
                    enemies[i] = new Health { Value = health - collisionDamage };
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (playerGroup.CalculateLength() == 0) //玩家死亡
                return base.OnUpdate(inputDeps);

            float3 playerPos = playerGroup.GetComponentDataArray<Position>()[0].Value;

            EnemyCollisionJob job = new EnemyCollisionJob
            {
                collisionDamage = 1,
                playerRadius = this.playerRadius,
                enemyRadius = this.enemyRadius,
                playerPos = playerPos,
                positions = enemyGroup.GetComponentDataArray<Position>(),
                enemies = enemyGroup.GetComponentDataArray<Health>()
            };
            return job.Schedule(enemyGroup.CalculateLength(), 64, inputDeps);
        }
    }

    public class RemoveDeadBarrier : BarrierSystem
    {
    }
    public class RemoveDeadSystem : JobComponentSystem
    {
        struct Player
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<PlayerInput> PlayerInputs;
        }
        [Inject] Player player;
        [Inject] RemoveDeadBarrier barrier;

        [BurstCompile]
        struct RemoveDeadJob : IJobProcessComponentDataWithEntity<Health>
        {
            public bool PlayerDead;
            public EntityCommandBuffer Command;

            //该方法会获取所有带有Health组件的实体。
            public void Execute(Entity entity, int index, [ReadOnly] ref Health health)
            {
                if (health.Value <= 0 || PlayerDead)
                    Command.DestroyEntity(entity);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            bool playerDead = player.Length == 0;

            RemoveDeadJob job = new RemoveDeadJob
            {
                PlayerDead = playerDead,
                Command = barrier.CreateCommandBuffer(),
            };
            return job.ScheduleSingle(this, inputDeps);
        }
    }
}
#endif