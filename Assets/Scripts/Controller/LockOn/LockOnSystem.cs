using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LockOnSystem : MonoBehaviour, ICurrentUnitUser
{
    [SerializeField] private CinemachineVirtualCamera _lockOnCamera;
    [SerializeField] private Transform _unitTransform;

    [SerializeField] private List<Transform> _avaliableTargets;
    [SerializeField] private Transform _currentTarget;

    private int _currentTargetIndex;

    private ART_Inputs _inputs;

    [Inject]
    public void InjectDependencies(ART_Inputs inputs)
    {
        _inputs = inputs;
    }

    public void SetCurrentUnit(UnitArmature unit)
    {
        _unitTransform = unit.transform;
        _lockOnCamera.Follow = unit.getCameraRoot.transform;
        _avaliableTargets = unit.getAvaliableTargets;
    }

    public void LockOnCurrentTarget()
    {
        LockOn(_currentTarget);
    }

    private void Update()
    {
        if (_inputs.getLockOn)
        {
            _inputs.setLockOn = false;
            LockOnAvaliableTarget();
        }
        else if (_inputs.getEsc && _lockOnCamera.gameObject.activeSelf)
        {
            _inputs.setEsc = false;
            LookAt(null);
            _currentTargetIndex = 0;
            Debug.Log("Lock-on: Released");
        }
    }

    private void LockOnAvaliableTarget()
    {
        int targetCount = _avaliableTargets.Count;
        if (targetCount > 0)
        {
            int targetIndex = _currentTargetIndex < targetCount ? _currentTargetIndex : _currentTargetIndex = 0;
            LookAt(_avaliableTargets[targetIndex]);
            _currentTargetIndex++;
            Debug.Log($"Locked-on: {_currentTarget.name}");
        }
        else
        {
            Debug.Log("Lock-on: No Targets Avaliable");
        }
    }

    private void LookAt(Transform target)
    {
        _currentTarget = target;
        if (_lockOnCamera != null)
        {
            GameEvents.OnLockOn?.Invoke(target);
            _lockOnCamera.gameObject.SetActive(target);
            _lockOnCamera.LookAt = target;
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
