using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class Spawner : SystemBase
{
    private bool _spawned;
    
    public GameObject unitPrefab;
    public int objectCount = 50000;
    public float gridSpacing = 2f;
    public float gridSize = 500f;
    
    protected override void OnUpdate()
    {
        if (_spawned || unitPrefab == null) return;
        
        var entities = new NativeArray<Entity>(objectCount, Allocator.Temp);
        
        for (int i = 0; i < objectCount; i++)
        {
            entities[i] = EntityManager.CreateEntity();
            
            EntityManager.AddComponentData(entities[i], new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            
            EntityManager.AddComponentData(entities[i], new MoveSpeed { Value = 1f });
            EntityManager.AddComponentData(entities[i], new Radius { Value = 5f });
            EntityManager.AddComponentData(entities[i], new CircleCenter());
        }
        
        int gridDim = (int)math.ceil(math.sqrt(objectCount));
        float startX = -gridSize / 2f;
        float startZ = -gridSize / 2f;
        
        for (int i = 0; i < objectCount; i++)
        {
            int x = i % gridDim;
            int z = i / gridDim;
            
            float3 position = new float3(
                startX + x * gridSpacing,
                0,
                startZ + z * gridSpacing
            );
            
            EntityManager.SetComponentData(entities[i], LocalTransform.FromPosition(position));
            EntityManager.SetComponentData(entities[i], new CircleCenter { Value = position });
        }
        
        entities.Dispose();
        _spawned = true;
        Enabled = false;
        
        Debug.Log($"Spawned {objectCount} entities");
    }
}