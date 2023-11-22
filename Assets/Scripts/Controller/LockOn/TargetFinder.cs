using Cinemachine;
using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private Transform _unitTransform;
    [SerializeField] private CinemachineVirtualCamera _lockOnVirtualCamera;

    private void OnDisable()
    {
        LookAt(null);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Target is in reach!");
        LookAt(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Target is out of reach!");
        LookAt(null);
    }

    private void LookAt(Transform target)
    {
        if (_lockOnVirtualCamera != null)
        {
            GameEvents.OnLockOn?.Invoke(target);
            _lockOnVirtualCamera.gameObject.SetActive(target);
            _lockOnVirtualCamera.LookAt = target;
            LockOn(target);
        }
    }

    private void LockOn(Transform target)
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, _unitTransform.position.y, target.position.z);
            _unitTransform.LookAt(targetPosition);
        }
    }
}
