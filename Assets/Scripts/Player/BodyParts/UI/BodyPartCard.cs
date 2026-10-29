using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class BodyPartCard : MonoBehaviour
{
    [SerializeField] private Image imageIcon;
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Button buttonClickArea;
    public void Initialize(BodyPartSO bodyPart, Action<BodyPartSO> onClicked)
    {
        imageIcon.sprite = bodyPart.Icon;
        name.text = bodyPart.nameThai;
        description.text = bodyPart.descriptionThai;
        buttonClickArea.onClick.AddListener(() => onClicked(bodyPart));
    }
}
