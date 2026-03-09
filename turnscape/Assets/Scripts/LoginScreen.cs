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
    [SerializeField] private GameObject masterPanel;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject playPanel;
    private bool isSignUpScreen = false;
    private Networking nt;
    [SerializeField] private GameObject networkmgr;
    private string playerName;


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
            playerName = mailField.text;
            if (!isSignUpScreen)
            {
                nt.SendLoginRequest($"{{\"UserName\":\"{mailField.text}\",\"Password\":\"{passwordField.text}\"}}");
            }
            else
            {
                nt.SendSignupRequest($"{{\"UserName\":\"{mailField.text}\",\"Password\":\"{passwordField.text}\"}}");
            }
            
        }
    }
    
    
    public void SucessfullLogin()
    {
        errorMessage.gameObject.SetActive(false);
        if (!isSignUpScreen)
        {
            HandleLoggingIn();
        }
        else
        {
            HandleSigningIn();
        }
    }

    public void LogOut()
    {
        isLoggedIn.text = "Not logged in";
    }
    
    private void HandleLoggingIn()
    {
        isLoggedIn.text = $"Logged in: {playerName}";
        loginPanel.SetActive(false);
        playPanel.gameObject.SetActive(true);
        
    }
    private void HandleSigningIn()
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
            //zdisabled temporarily
            var next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
                next.Select();
        }
    }
}
