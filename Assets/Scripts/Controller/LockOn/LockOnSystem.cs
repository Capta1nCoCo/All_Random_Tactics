using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LockOnSystem : MonoBehaviour, ICurrentUnitUser
{
    [SerializeField] private CinemachineVirtualCamera _lockOnCamera;

    private int _currentTargetIndex;

    private List<Transform> _avaliableTargets;
    private Transform _unitTransform;
    private Transform _currentTarget;

    private ART_Inputs _inputs;
    private UnitGravity _gravity;

    public Transform getCurrentTarget { get { return _currentTarget; } }

    [Inject]
    public void InjectDependencies(ART_Inputs inputs, UnitGravity gravity)
    {
        _inputs = inputs;
        _gravity = gravity;
    }

    public void SetCurrentUnit(UnitArmature unit)
    {
        _unitTransform = unit.transform;
        _lockOnCamera.Follow = unit.getCameraRoot.transform;
        _avaliableTargets = unit.getAvaliableTargets;
    }

    public void LockOnCurrentTarget()
    {
        LockOnTarget(_currentTarget);
    }

    public void ReleaseCurrentTarget()
    {
        ReleaseLockOn();
    }

    private void Update()
    {
        LockOn();
    }

    private void LockOn()
    {
        if (_inputs.getLockOn)
        {
            ProcessLockOn();
        }
        else if (_inputs.getEsc && _lockOnCamera.gameObject.activeSelf)
        {
            ResetLockOn();
        }
    }

    private void ProcessLockOn()
    {
        _inputs.setLockOn = false;
        if (_gravity.getGrounded)
        {
            LockOnAvaliableTarget();
        }
    }

    private void ResetLockOn()
    {
        _inputs.setEsc = false;
        ReleaseLockOn();
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
            LockOnTarget(target);
        }
    }

    private void LockOnTarget(Transform target)
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, _unitTransform.position.y, target.position.z);
            _unitTransform.LookAt(targetPosition);
        }
    }

    private void ReleaseLockOn()
    {
        LookAt(null);
        _currentTargetIndex = 0;
        Debug.Log("Lock-on: Released");
    }
}
