using Unity.Android.Gradle;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _spawnPrefab;
    [SerializeField] private Transform _spawnParent;
    [SerializeField] private int _spawnCount;

    private Element[] _objectPool = new Element[2000];
    private Vector3 _previousMousePosition;
    private Camera _mainCamera;
    private int _nextSpawnIndex = 0;

    public static float s_sharedDeltaTime = 0f;
    public static bool s_hasUpdatedDeltaTime = false;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        s_hasUpdatedDeltaTime = false;

        // Is left mouse button pressed
        if (Input.GetMouseButtonDown(0))
        {
            SpawnObjectOfType(ObjectType.Water);
        }
        else // Is right mouse button pressed
        if (Input.GetMouseButtonDown(1)) 
        {
            SpawnObjectOfType(ObjectType.Fire);
        }
        _previousMousePosition = Input.mousePosition;
    }

    private void SpawnObjectOfType(ObjectType type)
    {
        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(currentMousePosition);
        worldPosition.z = 0f;

        for (int i = 0; i < _spawnCount; ++i)
        {
            if (!TryFindNextIndex(_objectPool.Length))
                return;

            Element element = _objectPool[_nextSpawnIndex];
            element.Init(type);

            Transform transform = element.transform;
            transform.position = worldPosition;

            Vector3 velocityOffset = Vector3.zero;
            if (_spawnCount > 1)
            {
                velocityOffset = new Vector3(Random.Range(-0.1f, 0.1f),
                                             Random.Range(-0.1f, 0.1f),
                                             Random.Range(-0.1f, 0.1f));
            }
            Vector3 velocity = currentMousePosition - _previousMousePosition;
            var rigidBody2D = element.GetComponent<Rigidbody2D>();
            float magnitude = Mathf.Clamp((velocity + velocityOffset).magnitude, -10f, 10f);

            rigidBody2D.velocity = (velocity + velocityOffset).normalized * magnitude;
        }
    }

    private bool TryFindNextIndex(int count)
    {
        _nextSpawnIndex %= _objectPool.Length;
        Element element = _objectPool[_nextSpawnIndex];

        if (_objectPool[_nextSpawnIndex] == null)
        {
            _objectPool[_nextSpawnIndex] = Instantiate(_spawnPrefab, Vector3.zero, Quaternion.identity, _spawnParent).GetComponent<Element>();
            return true;
        }
        else if (!element.enabled)
        {
            return true;
        }
        else
        {
            --count;
            ++_nextSpawnIndex;

            if (count >= 0)
                return TryFindNextIndex(count);
            else
                return false;
        }
    }
}
