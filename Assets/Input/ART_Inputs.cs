using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ART_Inputs : MonoBehaviour
{
    [Header("Character Input Values")]
    [SerializeField] private Vector2 move;
    [SerializeField] private Vector2 look;
    [SerializeField] private bool jump;
    [SerializeField] private bool sprint;

    private bool nextUnit;
    private bool prevUnit;
    [SerializeField] private bool lightAttack;

    [Header("Movement Settings")]
    [SerializeField] private bool analogMovement;

    [Header("Mouse Cursor Settings")]
    [SerializeField] private bool cursorLocked = true;
    [SerializeField] private bool cursorInputForLook = true;

    public Vector2 getMove { get { return move; } }
    public Vector2 getLook { get { return look; } }
    public bool getJump { get { return jump; } }
    public bool getSprint { get { return sprint; } }
    public bool getNextUnit { get { return nextUnit; } }
    public bool getPrevUnit { get { return prevUnit; } }
    public bool getLightAttack { get { return lightAttack; } }
    public bool getAnalogMovement { get { return analogMovement; } }

    public bool setJump { set { jump = value; } }
    public bool setNextUnit { set { nextUnit = value; } }
    public bool setPrevUnit { set { prevUnit = value; } }
    public bool setLightAttack { set { lightAttack = value; } }

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnNextUnit(InputValue value)
    {
        NextUnitInput(value.isPressed);
    }

    public void OnPrevUnit(InputValue value)
    {
        PrevUnitInput(value.isPressed);
    }

    public void OnLightAttack(InputValue value)
    {
        LightAttackInput(value.isPressed);
    }
#endif


    private void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    private void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    private void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    private void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    private void NextUnitInput(bool newNextUnitState)
    {
        nextUnit = newNextUnitState;
    }

    private void PrevUnitInput(bool newPrevUnitState)
    {
        prevUnit = newPrevUnitState;
    }

    private void LightAttackInput(bool newLightAttackState)
    {
        lightAttack = newLightAttackState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}