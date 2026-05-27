using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    private Button btn;
    public AudioClip sound;
    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        if (sound == null)
            UIAudioManager.Instance.PlayClick();
        else 
            UIAudioManager.Instance.PlayCustomSound(sound);
    }
    
}
