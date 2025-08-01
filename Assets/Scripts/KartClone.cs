public class KartClone : Clone
{
    public override void SetFrameState(CloneFrameState state)
    {
        transform.position = state.position;
        transform.rotation = state.rotation;
    }
}
