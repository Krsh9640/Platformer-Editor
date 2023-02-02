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
    [SerializeField] private string CreatorNameTemp;

    public GameObject allsignin, allsignup;
    public GameObject signinBtn, signupBtn;

    public GameObject resetPasswordPnl, AuthenticatorPnl, HomeButtons,
                      verivyNotifPnl, UsernamePnl;

    [SerializeField] private bool signinActive, signupActive, hasStarted;

    private DownloadScene downloadScene;

    private SaveHandler saveHandler;
    public GameObject loading;

    private void Awake()
    {
        signupBtn.GetComponent<Image>().color = Color.gray;

        downloadScene = GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>();
        saveHandler = GameObject.Find("DownloadSceneManager").GetComponent<SaveHandler>();
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

        LootLockerSDKManager.WhiteLabelSignUp(email, password, (response) =>
        {
            if (!response.success)
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

        yield return new WaitForSeconds(2f);

        if (hasStarted == true)
        {
            GetPlayerName();

            yield return new WaitForSeconds(1f);

            loading.SetActive(false);

            if (CreatorName == CreatorNameTemp && CreatorName != null)
            {
                UsernamePnl.SetActive(true);
            }
            else
            {
                AuthenticatorPnl.SetActive(false);

                HomeButtons.SetActive(true);
            }
        }
        yield return null;
    }

    public void CheckSession()
    {
        loading.gameObject.SetActive(true);
        LootLockerSDKManager.CheckWhiteLabelSession(response =>
        {
            Debug.Log("Checking Session");
            if (response)
            {
                StartCoroutine(SigninOrder());
            }
            else
            {
                AuthenticatorPnl.SetActive(true);
            }
        });
    }

    public void StartSession()
    {
        LootLockerSDKManager.StartWhiteLabelSession((response) =>
        {
            if (!response.success)
            {
                return;
            }
            else
            {
                hasStarted = true;
                downloadScene.hasLoggedin = true;
            }
        });
    }

    public void SubmitUsernameBtn()
    {
        CreatorNameTemp = CreatorNameInput.text;

        CreatorName = CreatorNameTemp;

        SetPlayerName(CreatorName);

        AuthenticatorPnl.SetActive(false);

        HomeButtons.SetActive(true);
    }

    public void SetPlayerName(string playerName)
    {
        LootLockerSDKManager.SetPlayerName(playerName, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Success setting Player Name");
                saveHandler.playerName = CreatorName;
            }
            else
            {
                Debug.Log("Error setting Player Name");
            }
        });
    }

    public void GetPlayerName()
    {
        LootLockerSDKManager.GetPlayerName((response) =>
        {
            if (response.success)
            {
                CreatorName = response.name;
                saveHandler.playerName = CreatorName;
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
        downloadScene.hasLoggedin = false;
    }
}