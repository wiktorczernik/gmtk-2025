    public class KartClone : Clone
{
    public override void SetFrameState(CloneFrameState state)
    {
        transform.position = state.position + new UnityEngine.Vector3(0,1);
        transform.rotation = state.rotation;
    }
}
