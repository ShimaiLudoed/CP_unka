using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Unity.Mathematics;

public class ArmySettings : MonoBehaviour
{
    public GameObject unitPrefab;
    public int objectCount = 50000;
    public float gridSpacing = 2f;
    public float gridSize = 500f;
    public float moveSpeed = 1f;
    public float radius = 5f;
    
    void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;
        
        var entities = new NativeArray<Entity>(objectCount, Allocator.Temp);
        
        Mesh mesh = unitPrefab.GetComponent<MeshFilter>().sharedMesh;
        Material material = unitPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        
        var archetype = entityManager.CreateArchetype(
            typeof(LocalTransform),
            typeof(MoveSpeed),
            typeof(Radius),
            typeof(CircleCenter),
            typeof(RenderMesh)
        );
        
        entityManager.CreateEntity(archetype, entities);
        
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
            
            entityManager.SetComponentData(entities[i], LocalTransform.FromPosition(position));
            entityManager.SetComponentData(entities[i], new MoveSpeed { Value = moveSpeed });
            entityManager.SetComponentData(entities[i], new Radius { Value = radius });
            entityManager.SetComponentData(entities[i], new CircleCenter { Value = position });
        }
        
        entities.Dispose();
        Debug.Log($"Spawned {objectCount} entities");
    }
}