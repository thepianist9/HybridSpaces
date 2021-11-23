using System.Collections;
using System.Collections.Generic;
using agora_utilities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
#if(UNITY_2018_3_OR_NEWER)
using UnityEngine.iOS;
using UnityEngine.Android;
#endif

public class ButtonHandle : MonoBehaviour
{
    private static AgoraInterface app = null;  
    private int dropDownValue = 0;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        findWebCams();

        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.Log("webcam found");
        }
        else
        {
            Debug.Log("webcam not found");
        }

        findMicrophones();

        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.Log("Microphone found");
        }
        else
        {
            Debug.Log("Microphone not found");
        }
    }

    void findWebCams()
    {
        foreach (var device in WebCamTexture.devices)
        {
            Debug.Log("Name: " + device.name);
        }
    }

    void findMicrophones()
    {
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onButtonClick()
    {
        Debug.Log("Button CLicked" + name);

        if (name.CompareTo("JoinButton") == 0)
        {
            //join
            OnJoinButtonClicked();
        }
        else if(name.CompareTo("LeaveButton") == 0)
        {
            //leave
            OnLeaveButtonClicked();
        }
        
    }

    private void OnJoinButtonClicked()
    {

        
        Debug.Log("Join Button Clicked");
        //get channel name
        GameObject input1 = GameObject.Find("ChannelName");
        InputField channelName = input1.GetComponent<InputField>();
        //get channel key
        GameObject input2 = GameObject.Find("ChannelKey");
        InputField channelKey = input2.GetComponent<InputField>();
        //get dropdown value
        GameObject mDropdown = GameObject.Find("Mode");
        Dropdown dropDown = mDropdown.GetComponent<Dropdown>();
        dropDownValue = dropDown.value;
        string mode = dropDown.options[dropDownValue].text;
        
        Debug.Log("mode:" + mode);
        //init agora engine
        if (ReferenceEquals(app, null))
        {
            app = new AgoraInterface();
            app.loadEngine();
        }
        //join channel
        app.joinChannel(channelKey.text,channelName.text);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
        
        //loading scene
        if (mode.Equals("Normal Mode"))
        {
            SceneManager.LoadScene("ChatScene", LoadSceneMode.Single);
        }
        if (mode.Equals("AR Mode"))
        {
            SceneManager.LoadScene("ChatSceneAR", LoadSceneMode.Single);
        }

        if (mode.Equals("Interactive Mode"))
        {
            //TODO
        }


    }

    private void OnLeaveButtonClicked()
    {
        Debug.Log("Leave Button Clicked");

        if (!ReferenceEquals(app, null))
        {
            app.leaveChannel();
            app.unloadEngine();
            app = null;
            SceneManager.LoadScene("WelcomeScene", LoadSceneMode.Single);
        }
    }

    public void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.CompareTo("ChatScene") == 0)
        {
            if (!ReferenceEquals(app, null))
            {
                app.OnChatSceneLoaded();
            }

            SceneManager.sceneLoaded -= OnSceneFinishedLoading; 
        }
    }
}
