using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChangeInput : MonoBehaviour
{
    private EventSystem system;
    [SerializeField] private TMP_InputField mailField, passwordField;
    [SerializeField] private TMP_Text errorMessage; 
    
  

    private void Start()
    {
        system = EventSystem.current;
    }

    private void Update()
    {
        HandleTabbingThroughFields();
        
    }

    public void TextFieldFilled()
    {
        if (string.IsNullOrWhiteSpace(mailField.text) || string.IsNullOrWhiteSpace(passwordField.text))
        {
            Debug.Log("email or password is empty");
            errorMessage.text = "Mail or password is empty!";
            errorMessage.gameObject.SetActive(true);
        }
        else
        {
            GameManagerSc.LoadScene("MainMenuScene");
        }
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
}
