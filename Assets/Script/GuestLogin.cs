using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class GuestLogin : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (!response.success)
                {
                    Debug.Log("Error starting LootLocker");
                    return;
                }
                else
                {
                    Debug.Log("Success starting LootLocker");
                }
            });
    }
}
