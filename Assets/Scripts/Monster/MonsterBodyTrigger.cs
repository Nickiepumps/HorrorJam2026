using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MonsterBodyTrigger : MonoBehaviour
{
    public event Action<Transform, SectionType> OnTranslateToNewSection;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interact_Transition") && collision.TryGetComponent<TriggerSectionTransition>(out var sectionTransition))
        {
            if (sectionTransition.CanEnemyEnter())
            {
                GoToNextSection(sectionTransition.GetTargetSpawnTransform(), sectionTransition.TargetSectionType);
            }
        }
    }


    private void GoToNextSection(Transform newSectionPos, SectionType newSection)
    {
        OnTranslateToNewSection?.Invoke(newSectionPos, newSection);
    }
}
