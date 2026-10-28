using UnityEngine;

[CreateAssetMenu(fileName = "BodyPartUISO", menuName = "Player/BodyPart")]
public class BodyPartSO : ScriptableObject
{
    public Sprite Icon;
    public string nameThai;
    public string nameEng;
    public string descriptionThai;
    public string descriptionEng;
}
