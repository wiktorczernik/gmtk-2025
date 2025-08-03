using UnityEngine;

public class KartClone : Clone
{
    public bool isVisible = false;

    [SerializeField] GameObject _model;
    
    Vector3 targetScale;

    private void Awake()
    {
        targetScale = _model.transform.localScale;
    }

    public override void SetFrameState(CloneFrameState state)
    {
        transform.position = state.position + new UnityEngine.Vector3(0,1);
        transform.rotation = state.rotation;
    }
}
