using TMPro;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

public partial class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int objectCount = 1000;
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float circleRadius = 10f;
    [SerializeField] private float logCalculationInterval = 3f;
    [SerializeField] private TMP_Text performanceText;
    
    private TransformAccessArray _transformAccessArray;
    private NativeArray<float> _angles;
    private NativeArray<float> _logResults;
    
    private MoveJob _moveJob;
    private JobHandle _moveJobHandle;
    
    private float _nextLogTime;
    private float _deltaTime;
    
    private struct MoveJob : IJobParallelForTransform
    {
        public NativeArray<float> Angles;
        public float DeltaTime;
        public float Speed;
        public float Radius;
        public float CenterX;
        public float CenterZ;
        
        public void Execute(int index, TransformAccess transform)
        {
            Angles[index] += Speed * DeltaTime;
            
            float angle = Angles[index];
            float x = CenterX + Mathf.Cos(angle) * Radius;
            float z = CenterZ + Mathf.Sin(angle) * Radius;
            
            transform.position = new Vector3(x, transform.position.y, z);
            transform.rotation = Quaternion.LookRotation(new Vector3(-Mathf.Sin(angle), 0, Mathf.Cos(angle)));
        }
    }
    
    private struct LogCalculationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> RandomNumbers;
        public NativeArray<float> Results;
        
        public void Execute(int index)
        {
            Results[index] = Mathf.Log(RandomNumbers[index]);
        }
    }
    
    private void Start()
    {
        SpawnObjects();
        _nextLogTime = Time.time + logCalculationInterval;
        _logResults = new NativeArray<float>(objectCount, Allocator.Persistent);
    }
    
    private void SpawnObjects()
    {
        var transforms = new Transform[objectCount];
        _angles = new NativeArray<float>(objectCount, Allocator.Persistent);
        
        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = Instantiate(objectPrefab);
            
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 pos = new Vector3(randomCircle.x, 0, randomCircle.y);
            
            obj.transform.position = pos;
            obj.transform.rotation = Random.rotation;
            
            transforms[i] = obj.transform;
            _angles[i] = Random.Range(0f, Mathf.PI * 2);
        }
        
        _transformAccessArray = new TransformAccessArray(transforms);
    }
    
    private void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        
        _moveJob = new MoveJob
        {
            Angles = _angles,
            DeltaTime = Time.deltaTime,
            Speed = rotationSpeed,
            Radius = circleRadius,
            CenterX = transform.position.x,
            CenterZ = transform.position.z
        };
        
        _moveJobHandle = _moveJob.Schedule(_transformAccessArray);
        
        if (Time.time >= _nextLogTime)
        {
            ScheduleLogCalculation();
            _nextLogTime = Time.time + logCalculationInterval;
        }
        
        if (performanceText != null)
        {
            float fps = 1.0f / _deltaTime;
            float ms = _deltaTime * 1000f;
            performanceText.text = $"FPS: {fps:F1}\nMS: {ms:F2}\nObjects: {objectCount}";
        }
    }
    
    private void ScheduleLogCalculation()
    {
        var randomNumbers = new NativeArray<float>(objectCount, Allocator.TempJob);
        for (int i = 0; i < objectCount; i++)
        {
            randomNumbers[i] = Random.Range(1f, 100f);
        }
        
        var logJob = new LogCalculationJob
        {
            RandomNumbers = randomNumbers,
            Results = _logResults
        };
        
        JobHandle logJobHandle = logJob.Schedule(objectCount, 64);
        JobHandle.CombineDependencies(logJobHandle, _moveJobHandle).Complete();
        
        randomNumbers.Dispose();
    }
    
    private void LateUpdate()
    {
        _moveJobHandle.Complete();
    }
    
    private void OnDestroy()
    {
        if (_transformAccessArray.isCreated)
            _transformAccessArray.Dispose();
            
        if (_angles.IsCreated)
            _angles.Dispose();
            
        if (_logResults.IsCreated)
            _logResults.Dispose();
    }
}