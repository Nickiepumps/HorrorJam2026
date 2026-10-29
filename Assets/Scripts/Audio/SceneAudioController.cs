using System.Collections.Generic;
using UnityEngine;

public class SceneAudioController : MonoBehaviour
{
    [SerializeField] private List<AudioSource> sceneAudioSourceList;
    [SerializeField] private AudioSource audioSourceAmbience;

    public void AddAudioSource(AudioSource audioSource)
    {
        sceneAudioSourceList.Add(audioSource);
    }
    public void ToggleSceneAudio(bool status)
    {
        foreach(AudioSource audioSource in sceneAudioSourceList)
        {
            audioSource.enabled = status;
        }
    }
}
