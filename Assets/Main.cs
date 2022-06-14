using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PiepackerSDK;
using UnityEngine.Networking;
using System;

public class Main : MonoBehaviour
{
    public Text response;
    public InputField maskID;
    public InputField animationID;
    public Dropdown dropdown;
    public Dropdown mutedDropdown;
    public Dropdown deafDropdown;
    public Dropdown blindDropdown;
    public Dropdown mvDropdown;
    public Dropdown wvDropdown;

    // this variable is only used for testing using the Unity Editor.
    // The value here is that of an active bandicoot url.
#if UNITY_EDITOR 
    private const string connectionHost = "bandicoot-kvfl9-tm7nv.api.piepackerstaging.com";
#endif

    // Start is called before the first frame update
    void Start()
    {
        // dropdown Setup
        BooleanDropdownSetup(mutedDropdown);
        BooleanDropdownSetup(deafDropdown);
        BooleanDropdownSetup(blindDropdown);
        BooleanDropdownSetup(mvDropdown);
        BooleanDropdownSetup(wvDropdown);

        // Dropdown setup
        Refresh();
    }

    private string Connect() {
#if UNITY_EDITOR 
        return connectionHost;
#else
        return Piepacker.ConnectionHost();
#endif
    }

    private void BooleanDropdownSetup(Dropdown d) {
        d.ClearOptions();
        
        var data = new Dropdown.OptionData {
            text = "true"
        };
        d.options.Add(data);
        
        var data2 = new Dropdown.OptionData {
            text = "false"
        };
        d.options.Add(data2);
        
        d.RefreshShownValue();
        
        d.onValueChanged.AddListener(delegate {
            d.RefreshShownValue();
        });
    }

    public void Refresh () {
        StartCoroutine(RefreshIt());
    }

    private IEnumerator RefreshIt () {
        if (dropdown == null) {
            yield break;
        }
        string url = $"https://{Connect()}/api/v1/player-indexes";
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ProtocolError) {
            response.text = www.error;
            yield break;
        }
        var text = www.downloadHandler.text;
        List<PlayerIndex> playerList = JsonHelper.FromJson<PlayerIndex>(JsonHelper.wrapJson(text));
        var message = url + "\n";
        dropdown.ClearOptions();
        foreach (PlayerIndex p in playerList) {
            var msg = $"{p.userSessionID} ({string.Join(",", p.playerIdx)})";
            message += msg + "\n";
            var data = new Dropdown.OptionData {
                text = msg
            };
            dropdown.options.Add(data);
        }
        dropdown.RefreshShownValue();
        response.text = message;
    }

    // called when clicked on the mute button
    public void Mute() {
        FillRequest("deaf", mutedDropdown.options[mutedDropdown.value].text);
    }

    public void Blind() {
        FillRequest("deaf", blindDropdown.options[blindDropdown.value].text);
    }

    public void Deaf() {
        FillRequest("deaf", deafDropdown.options[deafDropdown.value].text);
    }

    public void MV() {
        FillRequest("mask-visible", mvDropdown.options[mvDropdown.value].text);
    }

    public void WV() {
        FillRequest("webcam-visible", wvDropdown.options[wvDropdown.value].text);
    }

    public void MaskID() {
        FillRequest("mask", maskID.text);
    }

    public void AnimationID() {
        FillRequest("mask", animationID.text);
    }

    private void FillRequest(string action, string value)
    {
        // 1) Get params
        string usid = dropdown.options[dropdown.value].text.Split(' ')[0];

        // 2) Formulate the request
        string url = $"https://{Connect()}/api/v1/{action}?usid={usid}&value={value}";

        StartCoroutine(DoRequest(url));
    }
    
    private IEnumerator DoRequest(string url) {
        var form = new WWWForm();
        using UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ProtocolError) {
            response.text = www.error;
            yield break;
        }
        response.text = url + "\n" + www.downloadHandler.text;
    }

    [Serializable]
    public class PlayerIndex {
        public string userSessionID;
        public List<int> playerIdx;
    }

    public static class JsonHelper {
        public static string wrapJson(string value) {
            value = "{\"Items\":" + value + "}";
            return value;
        }

        public static List<T> FromJson<T>(string json) {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        [Serializable]
        private class Wrapper<T> {
            public List<T> Items;
        }
    }
}