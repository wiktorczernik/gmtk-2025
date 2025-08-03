using System.Collections;
using UnityEngine;

public abstract class Clone : MonoBehaviour
{
    public VisibilityState visiblity { get; private set; } = VisibilityState.Hidden;
    public bool isChangingVisibility { get; private set; } = false;

    [Header("Visibility Settings")]
    public float showDuration = 0.5f;
    public float hideDuration = 0.5f;
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
    public virtual void SetVisibility(VisibilityState newState)
    {
        if (isChangingVisibility) return;
        if (newState == visiblity) return;

        isChangingVisibility = true;
        if (newState == VisibilityState.Visible)
            StartCoroutine(ShowSequence());
        else
            StartCoroutine(HideSequence());
    }
    public abstract void SetFrameState(CloneFrameState state);

    IEnumerator ShowSequence()
    {
        float time = 0;
        while (time <= showDuration)
        {
            time += Time.deltaTime;
            float fraction = Mathf.Clamp01(time / showDuration);
            Vector3 newLocalScale = Vector3.Lerp(Vector3.zero, targetLocalScale, fraction);
            _model.transform.localScale = newLocalScale;
            foreach(var particle in particles)
            {
                particle.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, fraction);
            }
            yield return new WaitForEndOfFrame();
        }
        _model.transform.localScale = targetLocalScale;
        foreach (var particle in particles)
        {
            particle.transform.localScale = Vector3.one;
        }
        isChangingVisibility = false;
        visiblity = VisibilityState.Visible;
    }
    IEnumerator HideSequence()
    {
        float time = 0;
        while (time <= hideDuration)
        {
            time += Time.deltaTime;
            float fraction = Mathf.Clamp01(time / hideDuration);
            Vector3 newLocalScale = Vector3.Lerp(targetLocalScale, Vector3.zero, fraction);
            _model.transform.localScale = newLocalScale;
            foreach (var particle in particles)
            {
                particle.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, fraction);
            }
            yield return new WaitForEndOfFrame();
        }
        _model.transform.localScale = Vector3.zero;
        foreach (var particle in particles)
        {
            particle.transform.localScale = Vector3.zero;
        }
        isChangingVisibility = false;
        visiblity = VisibilityState.Hidden;
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
