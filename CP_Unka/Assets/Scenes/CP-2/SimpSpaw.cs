using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
  public GameObject prefab;
  public int count = 1000;
  public float spacing = 2f;
    
  void Start()
  {
    int gridDim = Mathf.CeilToInt(Mathf.Sqrt(count));
    float startX = -gridDim * spacing / 2f;
    float startZ = -gridDim * spacing / 2f;
        
    for (int i = 0; i < count; i++)
    {
      int x = i % gridDim;
      int z = i / gridDim;
            
      Vector3 pos = new Vector3(
        startX + x * spacing,
        0,
        startZ + z * spacing
      );
            
      Instantiate(prefab, pos, Quaternion.identity);
    }
        
    Debug.Log($"Spawned {count} cubes");
  }
}