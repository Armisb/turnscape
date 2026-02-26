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
    [SerializeField] private TMP_Text loginHeader;
    [SerializeField] private TMP_Text loginText;

    [SerializeField] private GameObject loginPanel;
    private bool isSignUpScreen = false;
    Networking nt;
    PlayerData playerData;
    [SerializeField] GameObject networkmgr;


    private void Start()
    {
        system = EventSystem.current;
        nt = networkmgr.GetComponent<Networking>();
    }

    private void Update()
    {
        HandleTabbingThroughFields();
        
    }

    public void SetErrorMessage(string errorMessage)
    {
        this.errorMessage.text = errorMessage;
        this.errorMessage.gameObject.SetActive(true);
    }

    public void SwitchToSignUpScreen()
    {
        isSignUpScreen = true;
        loginHeader.text = "Signup to the game";
        loginText.text = "Sign up";

        // continue handling logic over here for the sign up 

    }

    public void ResetTextFields()
    {
        mailField.text = "";
        passwordField.text = "";
        loginHeader.text = "Login to the game";
        loginText.text = "Login";

        errorMessage.gameObject.SetActive(false);
        errorMessage.text = "";
        isSignUpScreen = false;
        loginPanel.SetActive(false);
    }

    public void TextFieldFilled()
    {
        if (string.IsNullOrWhiteSpace(mailField.text) || string.IsNullOrWhiteSpace(passwordField.text))
        {
            Debug.Log("email or password is empty");
            errorMessage.text = "email or password is empty!";
            errorMessage.gameObject.SetActive(true);
        }
        else
        {
            // should read the input fields and check if the credentials are correct
            // for the now the placeholder is to hide the inputs



            nt.SendPostRequest(mailField.text, passwordField.text);
            // handle login logic here, for example, send a request to the server to authenticate the user
            // for now the placeholder logic is to hide the login panel
            //ResetTextFields();

            
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
