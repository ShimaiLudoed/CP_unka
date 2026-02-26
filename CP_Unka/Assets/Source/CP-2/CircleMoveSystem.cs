using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct MovementSystem : ISystem
{
  [BurstCompile]
  public void OnUpdate(ref SystemState state)
  {
    float time = (float)SystemAPI.Time.ElapsedTime;
        
    new MovementJob
    {
      Time = time
    }.ScheduleParallel();
  }
    
  [BurstCompile]
  private partial struct MovementJob : IJobEntity
  {
    public float Time;
        
    private void Execute(ref LocalTransform transform, in MoveSpeed speed, in Radius radius, in CircleCenter center)
    {
      float angle = Time * speed.Value;
            
      float x = center.Value.x + math.cos(angle) * radius.Value;
      float z = center.Value.z + math.sin(angle) * radius.Value;
            
      transform.Position = new float3(x, transform.Position.y, z);
    }
  }
}