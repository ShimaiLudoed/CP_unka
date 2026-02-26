using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class ArmyAuto : MonoBehaviour
{
  public float moveSpeed = 1f;
  public float radius = 5f;
  public Mesh mesh;
  public Material material;
    
  private class Baker : Baker<ArmyAuto>
  {
    public override void Bake(ArmyAuto authoring)
    {
      var entity = GetEntity(TransformUsageFlags.Dynamic);
            
      AddComponent(entity, new LocalTransform
      {
        Position = float3.zero,
        Rotation = quaternion.identity,
        Scale = 1f
      });
            
      AddComponent(entity, new MoveSpeed { Value = authoring.moveSpeed });
      AddComponent(entity, new Radius { Value = authoring.radius });
      AddComponent(entity, new CircleCenter { Value = float3.zero });
    }
  }
}