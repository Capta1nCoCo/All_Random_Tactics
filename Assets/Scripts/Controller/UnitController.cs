using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(ART_Inputs), typeof(PlayerInput), typeof(UnitSwitcher))]
[RequireComponent(typeof(UnitGravity), typeof(UnitMovement), typeof(UnitCamera))]
[RequireComponent(typeof(UnitAnimations))]
public class UnitController : MonoBehaviour
{
    [SerializeField] private UnitArmature _currentUnit;

    private ART_Inputs _input;
    private UnitSwitcher _unitSwitcher;
    private UnitGravity _unitGravity;
    private UnitMovement _unitMovement;
    private UnitCamera _unitCamera;
    private UnitAnimations _unitAnimations;

    private void Awake()
    {
        _input = GetComponent<ART_Inputs>();
        _unitSwitcher = GetComponent<UnitSwitcher>();
        _unitGravity = GetComponent<UnitGravity>();
        _unitMovement = GetComponent<UnitMovement>();
        _unitCamera = GetComponent<UnitCamera>();
        _unitAnimations = GetComponent<UnitAnimations>();
    }

    private void Start()
    {
        _unitSwitcher.Init(_input, InitUnitArmature);
        _unitGravity.Init(_unitAnimations, _input);
        _unitMovement.Init(_unitAnimations, _input, _unitGravity);
        _unitCamera.Init(_input);
        InitUnitArmature(_unitSwitcher.getNewUnitArmature);
    }

    private void InitUnitArmature(UnitArmature currentUnit)
    {
        DisablePrevUnit();
        EnableCurrentUnit(currentUnit);
        UpdateDependencies();
    }

    private void DisablePrevUnit()
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

    private void UpdateDependencies()
    {
        _unitGravity.setCurrentUnit = _currentUnit;
        _unitMovement.setCurrentUnit = _currentUnit;
        _unitMovement.setController = _currentUnit.getController;
        _unitCamera.SetNewCameraTarget(_currentUnit.getCameraRoot);
        _unitAnimations.SetNewAnimator(_currentUnit.getAnimator);
    }
}
