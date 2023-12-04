using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class UnitArmature : MonoBehaviour
{
    [SerializeField] private GameObject _unitCameraRoot;
    [SerializeField] private GameObject _unitCamera;
    [SerializeField] private TargetFinder _targetFinder;
    [SerializeField] private UnitAbilities _unitAbilities;

    [SerializeField] private AudioClip LandingAudioClip;
    [SerializeField] private AudioClip[] FootstepAudioClips;
    [SerializeField] [Range(0, 1)] private float FootstepAudioVolume = 0.5f;

    private List<Transform> _avaliableTargets = new List<Transform>();

    private Animator _animator;
    private CharacterController _controller;
    private UnitSpecificAnimations _unitSpecificAnimations;

    public GameObject getCameraRoot { get { return _unitCameraRoot; } }
    public Animator getAnimator { get { return _animator; } }
    public CharacterController getController { get { return _controller; } }
    public UnitSpecificAnimations getUnitSpecificAnimations { get {  return _unitSpecificAnimations; } }
    public List<Transform> getAvaliableTargets { get { return _avaliableTargets; } }
    public TargetFinder getTargetFinder { get { return _targetFinder; } }
    public UnitAbilities getUnitAbilities {  get { return _unitAbilities; } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _unitSpecificAnimations = GetComponent<UnitSpecificAnimations>();
        _unitCamera.SetActive(false);
        if (_targetFinder != null)
        {
            _targetFinder.Init(this);
        }
    }

    public void EnableUnitCamera(bool value)
    {
        _unitCamera.SetActive(value);
    }

    // Used by Animation Events
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    // Used by Animation Events
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }
}
