using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class GizmosSystem : SystemBase
{
  public bool showGizmos = true;
  public int maxGizmos = 1000;
    
  protected override void OnUpdate()
  {
    if (!showGizmos) return;
        
    int count = 0;
        
    foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>())
    {
      if (count >= maxGizmos) break;
            
      float3 pos = transform.ValueRO.Position;
      UnityEngine.Debug.DrawLine(pos, pos + new float3(0, 2, 0), Color.red);
            
      count++;
    }
  }
}