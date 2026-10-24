using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Character")]
public class CharacterData : ScriptableObject
{
    public string displayName;
    public Sprite sprite;               // single sprite, no emotions
    public Color nameColor = Color.white;
}
