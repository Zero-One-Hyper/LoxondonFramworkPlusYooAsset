using UnityEngine;

public class ReceptionGuild : MonoBehaviour, IInteractive
{
    private ReceptionGuildManager _manager;
    public void Init(ReceptionGuildManager manager)
    {
        _manager = manager;
        var collider = this.transform.GetChild(0).gameObject.AddComponent<ReceptionGuildCollider>();
        collider.SetOwner(this);
    }

    public void Interactive(InterActiveArgs data)
    {
        _manager.OnClickReceptionGuild();
    }
}
