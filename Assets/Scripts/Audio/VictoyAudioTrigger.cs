using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VictoryAudioTrigger : MonoBehaviour
{
    public AudioMixerSnapshot paused;
    public AudioMixerSnapshot unpaused;

    public AudioClip victory;
    //public AudioClip chase;

    public MusicController musicController;

    void Awake()
    {
        musicController = GameObject.FindGameObjectWithTag("MusicController").GetComponent<MusicController>();

        paused.TransitionTo(.01f);
        musicController.VictoryStinger(victory);
    }

    private void OnDestroy()
    {
        unpaused.TransitionTo(.01f);
    }
}
