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
        GameEvents.OnLockOn += OnLockOn;
    }

    private void OnDisable()
    {
        GameEvents.OnLockOn -= OnLockOn;
    }

    private void OnLockOn(bool locked)
    {
        _lockSwitching = locked;
    }

    private void Update()
    {
        if (_unitGravity.getGrounded && !_lockSwitching)
        {
            SwitchUnit();
        }
        else if (_inputs.getNextUnit || _inputs.getPrevUnit)
        {
            _inputs.setNextUnit = false;
            _inputs.setPrevUnit = false;
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
            if (_currentUnitIndex < _unitArmatures.Length - 1)
            {
                _currentUnitIndex++;
            }
            else
            {
                _currentUnitIndex = 0;
            }
            InitNewUnit(_unitArmatures[_currentUnitIndex]);
        }
    }

    private void ChoosePrev()
    {
        if (_inputs.getPrevUnit)
        {
            _inputs.setPrevUnit = false;
            if (_currentUnitIndex > 0)
            {
                _currentUnitIndex--;
            }
            else
            {
                _currentUnitIndex = _unitArmatures.Length - 1;
            }
            InitNewUnit(_unitArmatures[_currentUnitIndex]);
        }
    }
}