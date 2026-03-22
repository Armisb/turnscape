using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
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


    private void Awake()
    {
        if (AuthManager.AccessToken != null)
        {
            var data = new LoginResponse
            {
                accessToken = AuthManager.AccessToken,
                refreshToken = AuthManager.RefreshToken
            };
            SucessfullLogin(JsonUtility.ToJson(data));
        }
    }

    private void Update()
    {
        HandleTabbingThroughFields();
    }

    public void SetErrorMessage(string errorMessage)
    {
        string formattedError = errorMessage;
        if (!errorMessage.Contains("4"))
            formattedError = "Cannot reach the server.";
        else
        {
            switch (errorMessage.Substring(errorMessage.IndexOf("4"), 3))
            {
                case "400":
                    formattedError = "Username already in use.";
                    break;
                case "401":
                    formattedError = "Invalid Username or Password";
                    break;
                case "404":
                    formattedError = "Cannot connect to the server";
                    break;
                default:
                    break;
            }
        }
        
        this.errorMessage.text = formattedError;
        this.errorMessage.gameObject.SetActive(true);
    }

    public void SwitchToSignUpScreen()
    {
        SetSignUpScreen(true);
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

    private async Task TextFieldFilled()
    {
        if (string.IsNullOrWhiteSpace(mailField.text) || string.IsNullOrWhiteSpace(passwordField.text))
        {
            Debug.Log("Username or password is empty");
            errorMessage.text = "Username or Password field is empty!";
            errorMessage.gameObject.SetActive(true);
        }
        else
        {
            if (!isSignUpScreen)
            {


                await Networking.SendPostGeneric(
                    "user/login",
                    $"{{\"UserName\":\"{mailField.text}\",\"Password\":\"{passwordField.text}\"}}",
                    response => this.SucessfullLogin(response),
                    error => this.SetErrorMessage(error)
                );
            }
            else
            {
                
            
                await Networking.SendPostGeneric(
                    "user/signup",
                    $"{{\"UserName\":\"{mailField.text}\",\"Password\":\"{passwordField.text}\"}}",
                    response => this.SucessfullLogin(response),
                    error => this.SetErrorMessage(error)
                    );
            }
            
        }
    }

    public void SetSignUpScreen(bool sign)
    {
        if  (sign)
        {
            isSignUpScreen =  true;
        }
        else
        {
            isSignUpScreen = false;
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
    
    public void OnTextFieldFilled()
    {
        _ = TextFieldFilled();
    }
    
    private void HandleLoggingIn(string response)
    {
        LoginResponse data = JsonUtility.FromJson<LoginResponse>(response);
        AuthManager.SetAccessToken(data.accessToken);
        AuthManager.SetRefreshToken(data.refreshToken);
        // Extract User ID from JWT
        AuthManager.SetPlayerID((JwtHelper.GetClaim(
            data.accessToken,
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        )));
        AuthManager.SetPlayerName(JwtHelper.GetClaim(
            data.accessToken,
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        ));
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
