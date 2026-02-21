using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeInput : MonoBehaviour
{
    private EventSystem system;

    private void Start()
    {
        system = EventSystem.current;
    }

    private void Update()
    {
        HandleTabbingThroughFields();
    }

    /// <summary>
    /// allows switching typing fields with the press of tab button 
    /// </summary>
    private void HandleTabbingThroughFields()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            var next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
                next.Select();
        }
    }

    public static void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }

}
