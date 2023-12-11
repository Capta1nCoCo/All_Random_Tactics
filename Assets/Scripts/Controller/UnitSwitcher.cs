using UnityEngine;
using Zenject;

public class UnitSwitcher : MonoBehaviour
{
    [SerializeField] private UnitArmature[] _unitArmatures;

    private int _currentUnitIndex = 0;
    private bool _lockSwitching;

    public delegate void InitUnitMethod(UnitArmature newUnitArmature);
    private InitUnitMethod InitNewUnit;

    private ART_Inputs _inputs;
    private UnitGravity _unitGravity;

    [Inject]
    public void InjectDependencies(ART_Inputs inputs, UnitGravity unitGravity)
    {
        _inputs = inputs;
        _unitGravity = unitGravity;
    }

    public void RequestInitialUnit(InitUnitMethod InitUnitArmature)
    {
        InitNewUnit = InitUnitArmature;
        InitNewUnit(_unitArmatures[_currentUnitIndex]);
    }

    private void OnEnable()
    {
        GameEvents.OnLockOn += LockSwitching;
        GameEvents.OnAbilityUIMenu += LockSwitching;
    }

    private void OnDisable()
    {
        GameEvents.OnLockOn -= LockSwitching;
        GameEvents.OnAbilityUIMenu -= LockSwitching;
    }

    private void LockSwitching(bool locked)
    {
        _lockSwitching = locked;
    }

    private void Update()
    {
        if (!_lockSwitching)
        {
            if (_unitGravity.getGrounded)
            {
                SwitchUnit();
            }
            else if (_inputs.getNextUnit || _inputs.getPrevUnit)
            {
                DisposeSwitchInputs();
            }
        }
    }

    public void SwitchUnit()
    {
        if (_unitArmatures.Length > 0)
        {
            ChooseNext();
            ChoosePrev();
        }
    }

    private void ChooseNext()
    {
        if (_inputs.getNextUnit)
        {
            _inputs.setNextUnit = false;
            SwitchToNextIndex();
            InitNewUnit(_unitArmatures[_currentUnitIndex]);
        }
    }

    private void SwitchToNextIndex()
    {
        if (_currentUnitIndex < _unitArmatures.Length - 1)
        {
            _currentUnitIndex++;
        }
        else
        {
            _currentUnitIndex = 0;
        }
    }

    private void ChoosePrev()
    {
        if (_inputs.getPrevUnit)
        {
            _inputs.setPrevUnit = false;
            SwitchToPrevIndex();
            InitNewUnit(_unitArmatures[_currentUnitIndex]);
        }
    }

    private void SwitchToPrevIndex()
    {
        if (_currentUnitIndex > 0)
        {
            _currentUnitIndex--;
        }
        else
        {
            _currentUnitIndex = _unitArmatures.Length - 1;
        }
    }

    private void DisposeSwitchInputs()
    {
        _inputs.setNextUnit = false;
        _inputs.setPrevUnit = false;
    }
}