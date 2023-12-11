using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _spawnPrefab;
    [SerializeField] private Transform _spawnParent;
    [SerializeField] private int _spawnCount;
    [SerializeField] private ObjectType _spawnType;

    private Camera _mainCamera;
    private Vector3 _previousMousePosition;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        // Is left mouse button pressed
        if(Input.GetMouseButton(0))
        {
            Vector3 currentMousePOsition = Input.mousePosition;
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(currentMousePOsition);
            worldPosition.z = 0f;

            for (int i = 0; i < _spawnCount; ++i)
            {
                var go = Instantiate(_spawnPrefab, worldPosition, Quaternion.identity, _spawnParent);

                var element = go.GetComponent<Element>();
                element.Init(_spawnType);

                Vector3 velocityOffset = Vector3.zero;
                if(_spawnCount > 1)
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
        _previousMousePosition = Input.mousePosition;
    }
}
