using Unity.Entities;
using Unity.Mathematics;

public struct MoveSpeed : IComponentData
{
  public float Value;
}

public struct Radius : IComponentData
{
  public float Value;
}

public struct CircleCenter : IComponentData
{
  public float3 Value;
}