using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class LoginScreen : MonoBehaviour
{
    private EventSystem system;
    [SerializeField] private TMP_InputField mailField, passwordField;
    [SerializeField] private TMP_Text errorMessage;
    [SerializeField] private TMP_Text loginHeader;
    [SerializeField] private TMP_Text loginText;
    [SerializeField] private TMP_Text isLoggedIn;
    [SerializeField] private Button playButton;

    [SerializeField] private GameObject loginPanel;
    private bool isSignUpScreen = false;
    Networking nt;
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
            PlayerData playerData =  new PlayerData(mailField.text, passwordField.text);
            nt.SendPostRequest(playerData);
            
        }
    }
    public void SucessfullLogin(PlayerData playerData)
    {
        errorMessage.gameObject.SetActive(false);
        if (!isSignUpScreen)
        {
            HandleLoggingIn(playerData);
        }
        else
        {
            HandleSigningIn(playerData);
        }
    }
    
    
    private void HandleLoggingIn(PlayerData data)
    {
        isLoggedIn.text = $"Logged in: {data.getUsername()}";
        loginPanel.SetActive(false);
        playButton.gameObject.SetActive(true);
    }
    private void HandleSigningIn(PlayerData data)
    {
        loginPanel.SetActive(false);
        ResetTextFields();
    }

    /// <summary>
    /// allows switching typing fields with the press of tab button 
    /// </summary>
    private void HandleTabbingThroughFields()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            // disabled temporarily
            // var next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            // if (next != null)
            //     next.Select();
        }
    }
}
