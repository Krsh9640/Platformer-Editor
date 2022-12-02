using LootLocker.Requests;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Authenticator : MonoBehaviour
{
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;

    [SerializeField] private string email;
    [SerializeField] private string password;
    private bool rememberMe = true;

    public TMP_InputField CreatorNameInput;
    [SerializeField] private string CreatorName;

    public GameObject allsignin, allsignup;
    public GameObject signinBtn, signupBtn;

    public GameObject resetPasswordPnl, AuthenticatorPnl, HomeButtons, verivyNotifPnl;

    private bool signinActive, signupActive;

    private string hasNamed;
    [SerializeField] private int hasNamedValue = 0;

    private void Awake()
    {
        signupBtn.GetComponent<Image>().color = Color.gray;

        CheckSession();
    }

    public void SignInTab()
    {
        if (signinActive == false)
        {
            allsignup.SetActive(false);
            signupBtn.GetComponent<Image>().color = Color.gray;
            signinBtn.GetComponent<Image>().color = Color.white;
            allsignin.SetActive(true);
            signupActive = false;

            EmailInput.text = email;
            PasswordInput.text = password;
        }
    }

    public void SignUpTab()
    {
        if (signupActive == false)
        {
            allsignin.SetActive(false);
            signinBtn.GetComponent<Image>().color = Color.gray;
            signupBtn.GetComponent<Image>().color = Color.white;
            allsignup.SetActive(true);
            signinActive = false;

            EmailInput.text = email;
            PasswordInput.text = password;
        }
    }

    public void SignUp()
    {
        email = EmailInput.text;
        password = PasswordInput.text;
        CreatorName = CreatorNameInput.text;

        LootLockerSDKManager.WhiteLabelSignUp(email, password, (response) =>
        {
            if (response.success)
            {
                PlayerPrefs.SetInt(hasNamed, 0);
            }
            else
            {
                return;
            }
        });
        

        verivyNotifPnl.SetActive(true);
        SignInTab();
    }

    public void SignIn()
    {
        email = EmailInput.text;
        password = PasswordInput.text;
        LootLockerSDKManager.WhiteLabelLogin(email, password, rememberMe, (response) =>
        {
            if (!response.success)
            {
                return;
            }

            string token = response.SessionToken;

            StartCoroutine(SigninOrder());
        });
    }

    public IEnumerator SigninOrder()
    {
        StartSession();
        AuthenticatorPnl.SetActive(false);
        HomeButtons.SetActive(true);

        hasNamedValue = PlayerPrefs.GetInt(hasNamed);

        yield return new WaitForSeconds(1.0f);
        if (hasNamedValue == 0)
        {
            SetPlayerName(CreatorName);
            Debug.Log("Has Set Player Name");
            
        }
        else if(hasNamedValue == 1)
        {

        }
        yield return null;
    }

    public void CheckSession()
    {
        LootLockerSDKManager.CheckWhiteLabelSession(response =>
        {
            if (response)
            {
                StartCoroutine(SigninOrder());
            }
            else
            {
            }
        });
    }

    public void StartSession()
    {
        LootLockerSDKManager.StartWhiteLabelSession((response) =>
        {
            if (response.success)
            {
            }
            else
            {
                return;
            }
        });
    }

    public void SetPlayerName(string playerName)
    {
        LootLockerSDKManager.SetPlayerName(playerName, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Success setting Player Name");
            }
            else
            {
                
            }
        });

        PlayerPrefs.SetInt(hasNamed, 1);
    }

    public void GetPlayerName()
    {
        LootLockerSDKManager.GetPlayerName((response) => 
        {
            if (response.success)
            {
                CreatorName = response.name;
            }
            else
            {

            }
        });
    }

    public void ResetPassword()
    {
        LootLockerSDKManager.WhiteLabelRequestPassword(email, (response) =>
        {
            if (response.success)
            {
                resetPasswordPnl.SetActive(true);
            }
            else
            {
                return;
            }
        });
    }

    public void Logout()
    {
       LootLockerSDKManager.EndSession((response) =>
       {
           if (response.success)
           {

           }
           else
           {

           }
       });

        HomeButtons.SetActive(false);
        AuthenticatorPnl.SetActive(true);
        SignInTab();
        email = null;
        password = null;
    }
}