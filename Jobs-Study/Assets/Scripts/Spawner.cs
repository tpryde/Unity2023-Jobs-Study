using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _spawnPrefab;
    [SerializeField] private Transform _spawnParent;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        // Is left mouse button pressed
        if(Input.GetMouseButton(0))
        {
            Vector3 pos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            Instantiate(_spawnPrefab, pos, Quaternion.identity, _spawnParent);
        }
    }
}
