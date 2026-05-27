using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    private Button btn;
    private Toggle toggle;

    public AudioClip sound;

    private void Start()
    {
        btn = GetComponent<Button>();
        toggle = GetComponent<Toggle>();

        if (btn != null)
            btn.onClick.AddListener(PlaySound);

        if (toggle != null)
            toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool value)
    {
        PlaySound();
    }

    void PlaySound()
    {
        if (sound == null)
            UIAudioManager.Instance.PlayClick();
        else
            UIAudioManager.Instance.PlayCustomSound(sound);
    }
}