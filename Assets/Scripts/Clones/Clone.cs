using System.Collections;
using UnityEngine;

public abstract class Clone : MonoBehaviour
{
    public VisibilityState visiblity { get; private set; } = VisibilityState.Hidden;
    public bool isChangingVisibility { get; private set; } = false;

    [Header("Visibility Settings")]
    public float showLerp = 4;
    public float hideLerp = 4;
    [SerializeField] GameObject _model;

    Vector3 targetLocalScale;
    ParticleSystem[] particles;

    public void Show()
    {
        SetVisibility(VisibilityState.Visible);
    }
    public void Hide()
    {
        SetVisibility(VisibilityState.Hidden);
    }
    public void HideForce()
    {
        SetVisibility(VisibilityState.Hidden);
        _model.transform.localScale = Vector3.zero;
    }
    public virtual void SetVisibility(VisibilityState newState)
    {
        visiblity = newState;
    }
    public abstract void SetFrameState(CloneFrameState state);


    private void FixedUpdate()
    {
        Vector3 scale = visiblity == VisibilityState.Visible ? targetLocalScale : Vector3.zero;
        Vector3 particleScale = visiblity == VisibilityState.Visible ? Vector3.one : Vector3.zero;
        float lerp = visiblity == VisibilityState.Visible ? showLerp : hideLerp;
        lerp *= Time.fixedDeltaTime;
        _model.transform.localScale = Vector3.Lerp(_model.transform.localScale, scale, lerp);
        foreach (var particle in particles)
            particle.transform.localScale = Vector3.Lerp(particle.transform.localScale, particleScale, lerp);
    }
    private void Awake()
    {
        targetLocalScale = _model.transform.localScale;
        particles = _model.GetComponentsInChildren<ParticleSystem>();
    }


    public enum VisibilityState
    {
        Visible,
        Hidden
    }
}
