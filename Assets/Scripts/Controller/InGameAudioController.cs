using UnityEngine;

public class InGameAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSourceEnvArr;
    [SerializeField] private AudioSource audioSourceBGM;
    [SerializeField] private AudioClip clipBGM;

    public void ToggleAllEnvAudio(bool status)
    {
        foreach(AudioSource source in audioSourceEnvArr)
        {
            source.enabled = status;
        }
    }
}
