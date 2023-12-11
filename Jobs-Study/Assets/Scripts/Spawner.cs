using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _spawnPrefab;
    [SerializeField] private Transform _spawnParent;
    [SerializeField] private int _spawnCount;

    private Camera _mainCamera;
    private Vector3 _previousMousePosition;

    private const int _spawnCap = 2000;
    private int _spwnedCount = 0;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_spwnedCount < _spawnCap)
        {
            // Is left mouse button pressed
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 currentMousePOsition = Input.mousePosition;
                Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(currentMousePOsition);
                worldPosition.z = 0f;

                for (int i = 0; i < _spawnCount; ++i)
                {
                    ++_spwnedCount;
                    var go = Instantiate(_spawnPrefab, worldPosition, Quaternion.identity, _spawnParent);

                    var element = go.GetComponent<Element>();
                    element.Init(ObjectType.Water);

                    Vector3 velocityOffset = Vector3.zero;
                    if (_spawnCount > 1)
                    {
                        velocityOffset = new Vector3(Random.Range(-0.1f, 0.1f),
                                                     Random.Range(-0.1f, 0.1f),
                                                     Random.Range(-0.1f, 0.1f));
                    }
                    Vector3 velocity = currentMousePOsition - _previousMousePosition;
                    var rigidBody2D = go.GetComponent<Rigidbody2D>();
                    rigidBody2D.velocity = velocity + velocityOffset;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Vector3 currentMousePOsition = Input.mousePosition;
                Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(currentMousePOsition);
                worldPosition.z = 0f;

                for (int i = 0; i < _spawnCount; ++i)
                {
                    ++_spwnedCount;
                    var go = Instantiate(_spawnPrefab, worldPosition, Quaternion.identity, _spawnParent);

                    var element = go.GetComponent<Element>();
                    element.Init(ObjectType.Fire);

                    Vector3 velocityOffset = Vector3.zero;
                    if (_spawnCount > 1)
                    {
                        velocityOffset = new Vector3(Random.Range(-0.1f, 0.1f),
                                                     Random.Range(-0.1f, 0.1f),
                                                     Random.Range(-0.1f, 0.1f));
                    }
                    Vector3 velocity = currentMousePOsition - _previousMousePosition;
                    var rigidBody2D = go.GetComponent<Rigidbody2D>();
                    rigidBody2D.velocity = velocity + velocityOffset;
                }
            }
        }
        _previousMousePosition = Input.mousePosition;
    }
}
