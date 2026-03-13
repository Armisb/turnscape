using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
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


    private void Start()
    {
        nt = GetComponentInChildren(typeof(Networking)) as Networking;
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
            AuthManager.SetPlayerName(mailField.text);
            if (!isSignUpScreen)
            {
                nt.SendPostGeneric(
                    "https://localhost:7232/user/login",
                    $"{{\"UserName\":\"{mailField.text}\",\"Password\":\"{passwordField.text}\"}}",
                    response => this.SucessfullLogin(response),
                    error => this.SetErrorMessage(error)
                    );
            }
            else
            {
                nt.SendPostGeneric(
                    "https://localhost:7232/user/signup",
                    $"{{\"UserName\":\"{mailField.text}\",\"Password\":\"{passwordField.text}\"}}",
                    response => this.SucessfullLogin(response),
                    error => this.SucessfullLogin(error)
                    );
            }
            
        }
    }
    
    
    public void SucessfullLogin(string response)
    {
        errorMessage.gameObject.SetActive(false);
        if (!isSignUpScreen)
        {
            HandleLoggingIn(response);
        }
        else
        {
            HandleSigningIn();
        }
    }

    public void LogOut()
    {
        AuthManager.ClearTokens();
        isLoggedIn.text = "Not logged in";
    }
    
    private void HandleLoggingIn(string response)
    {
        LoginResponse data = JsonUtility.FromJson<LoginResponse>(response);
        AuthManager.SetAccessToken(data.accessToken);
        AuthManager.SetRefreshToken(data.refreshToken);
        isLoggedIn.text = $"Logged in: {AuthManager.PlayerName}";
        loginPanel.SetActive(false);
        masterPanel.SetActive(false);
        playPanel.gameObject.SetActive(true);
    }
    private void HandleSigningIn()
    {
        loginPanel.SetActive(false);
        masterPanel.SetActive(true);
        ResetTextFields();
    }

    /// <summary>
    /// allows switching typing fields with the press of tab button 
    /// </summary>
    private void HandleTabbingThroughFields()
    {
        // if tab is pressed
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            GameObject current = EventSystem.current.currentSelectedGameObject;
            if (current == null) return;

            Selectable currentSelectable = current.GetComponent<Selectable>();
            if (currentSelectable == null) return;

            // Tab = down, Shift+Tab = up
            Selectable next = Keyboard.current.shiftKey.isPressed
                ? currentSelectable.FindSelectableOnUp()
                : currentSelectable.FindSelectableOnDown();
            
            if (next != null)
                next.Select();
            
        }
    }
}
