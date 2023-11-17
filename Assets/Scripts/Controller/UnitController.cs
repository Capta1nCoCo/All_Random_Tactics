using UnityEngine;

[RequireComponent(typeof(UnitSwitcher), typeof(UnitAnimations), typeof(UnitCamera))]
[RequireComponent(typeof(UnitGravity), typeof(UnitMovement))]
public class UnitController : MonoBehaviour
{
    [SerializeField] private UnitArmature _currentUnit;

    private UnitSwitcher _unitSwitcher;

    private ICurrentUnitUser[] _currentUnitUsers;

    private void Awake()
    {
        _unitSwitcher = GetComponent<UnitSwitcher>();
        _currentUnitUsers = GetComponents<ICurrentUnitUser>();
    }

    private void Start()
    {
        _unitSwitcher.RequestInitialUnit(InitUnitArmature);
    }

    private void InitUnitArmature(UnitArmature currentUnit)
    {
        DisablePrevUnitCamera();
        EnableCurrentUnit(currentUnit);
        UpdateCurrentUnitUsers();
    }

    private void DisablePrevUnitCamera()
    {
        if (_currentUnit != null)
        {
            _currentUnit.EnableUnitCamera(false);
        }
    }

    private void EnableCurrentUnit(UnitArmature currentUnit)
    {
        _currentUnit = currentUnit;
        _currentUnit.EnableUnitCamera(true);
    }

    private void UpdateCurrentUnitUsers()
    {
        for (int i = 0; i < _currentUnitUsers.Length; i++)
        {
            _currentUnitUsers[i].SetCurrentUnit(_currentUnit);
        }
    }
}