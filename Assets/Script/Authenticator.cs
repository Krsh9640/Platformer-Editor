using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Authenticator : MonoBehaviour
{
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;

    private string email;
    private string password;
    private bool rememberMe = true;

    public TMP_InputField CreatorNameInput;
    public string CreatorName;

    public GameObject allsignin, allsignup;
    public GameObject signinBtn, signupBtn;

    public GameObject resetPasswordPnl;

    private bool signinActive, signupActive;

    private void Awake()
    {
        signupBtn.GetComponent<Image>().color = Color.gray;
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
        }
    }

    public void SignUp()
    {
        EmailInput.text = email;
        PasswordInput.text = password;
        LootLockerSDKManager.WhiteLabelSignUp(email, password, (response) =>
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

    public void SignIn()
    {
        EmailInput.text = email;
        PasswordInput.text = password;
        LootLockerSDKManager.WhiteLabelLogin(email, password, rememberMe, (response) =>
        {
            if (response.success)
            {
                CheckSession();
                StartSession();
            }
            else
            {
                return;
            }
            string token = response.SessionToken;
        });
    }

    public void CheckSession()
    {
        LootLockerSDKManager.CheckWhiteLabelSession(response => 
        {
            if (response)
            {

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

    public void PlayerName()
    {
        CreatorName = CreatorNameInput.text;

        LootLockerSDKManager.SetPlayerName(CreatorName, (response) =>
        {
            if (response.success)
            {

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
}
