using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MomMonsterBodyTrigger : MonoBehaviour
{
    public event Action<Transform> OnTranslateToNewSection;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interact_Transition") && collision.TryGetComponent<TriggerSectionTransition>(out var sectionTransition))
        {
            if (sectionTransition.CanEnemyEnter())
            {
                GoToNextSection(sectionTransition.GetTargetSpawnTransform());
            }
        }
    }


    private void GoToNextSection(Transform newSection)
    {
        OnTranslateToNewSection?.Invoke(newSection);
    }
}
