using UnityEngine;
public class BaseBodyPart : MonoBehaviour
{
    [SerializeField] private string textBuffDesc;
    [SerializeField] private string textDebuffDesc;
    public string GetBuffDesc => textBuffDesc;
    public string GetDebuffDesc => textDebuffDesc;
    public virtual void BuffEffect() { }
    public virtual void DebuffEffect() { }
}
