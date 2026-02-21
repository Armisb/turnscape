using UnityEngine;
using UnityEngine.UI;

public class ToggleObjectSc : MonoBehaviour
{
    public GameObject MenuObj;
    public Toggle toggle;
    public Image body;

    public void Start()
    {
        ChangeObjectState();
    }

    public void ChangeObjectState()
    {
        MenuObj.SetActive(toggle.isOn);

        if (body != null && toggle.isOn)
        {
            body.color = Color.gray;
        }
        else
        {
            body.color = Color.white;
        }
    }
}
