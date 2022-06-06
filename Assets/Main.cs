using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using UnityEngine.UI;
using PiepackerSDK;
using UnityEngine.Networking;
using System.Text;
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

    // this variable is only used for testing using the Unity studio. For actual use, keep this commented
    // and the value of connectionHost to empty string. The value here is that of an active bandicoot url.
    // private static readonly string connectionHost = "bandicoot-kvfl9-tm7nv.api.piepackerstaging.com";
    private static readonly string connectionHost = "";

    // Start is called before the first frame update
    void Start()
    {
        // dropdown Setup
        trueFalseDropdownSetup("muted", mutedDropdown);
        trueFalseDropdownSetup("deaf", deafDropdown);
        trueFalseDropdownSetup("blind", blindDropdown);
        trueFalseDropdownSetup("mv", mvDropdown);
        trueFalseDropdownSetup("wv", wvDropdown);

        // Dropdown setup
        Refresh();
    }

    private string connect() {
        if (connectionHost == "") {
            return Piepacker.ConnectionHost();
        }
        return connectionHost;
    }

    private void trueFalseDropdownSetup(string label, Dropdown d) {
        if (d == null) {
            return;
        }
        Debug.Log(label + " dropdown:" + d);
        d.ClearOptions();
        Dropdown.OptionData data = new Dropdown.OptionData();
        data.text = "true";
        d.options.Add(data);
        Dropdown.OptionData data2 = new Dropdown.OptionData();
        data2.text = "false";
        d.options.Add(data2);
        d.RefreshShownValue();
        d.onValueChanged.AddListener(delegate {
            d.RefreshShownValue();
        });
    }

    public void Refresh () {
        StartCoroutine(RefreshIt());
    }

    public IEnumerator RefreshIt () {
        if (dropdown == null) {
            yield break;
        }
        string url = string.Format("https://{0}/api/v1/player-indexes", connect());
        using (UnityWebRequest www = UnityWebRequest.Get(url)) {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ProtocolError) {
                Debug.Log(www.error);
                yield break;
            }
            var text = www.downloadHandler.text;
            List<PlayerIndex> playerList = JsonHelper.FromJson<PlayerIndex>(JsonHelper.wrapJson(text));
            var message = url + "\n";
            Debug.Log("usid dropdown: " + dropdown);
            dropdown.ClearOptions();
            foreach (PlayerIndex p in playerList) {
                foreach (int idx in p.PlayerIdx) {
                    string msg = string.Format("{0} ({1})", p.UserSessionID, string.Join(",", p.PlayerIdx));
                    message += msg + "\n";
                    Dropdown.OptionData data = new Dropdown.OptionData();
                    data.text = msg;
                    dropdown.options.Add(data);
                }
            }
            dropdown.RefreshShownValue();
            response.text = message;
        }
    }

    // called when clicked on the mute button
    public void Mute() {
        StartCoroutine(doBoolRequest("muted", mutedDropdown));
    }

    public void Blind() {
        StartCoroutine(doBoolRequest("blind", blindDropdown));
    }

    public void Deaf() {
        StartCoroutine(doBoolRequest("deaf", deafDropdown));
    }

    public void MV() {
        StartCoroutine(doBoolRequest("mask-visible", mvDropdown));
    }

    public void WV() {
        StartCoroutine(doBoolRequest("webcam-visible", wvDropdown));
    }

    public void MaskID() {
        StartCoroutine(doStringRequest("mask", maskID));
    }

    public void AnimationID() {
        StartCoroutine(doStringRequest("mask-animation", animationID));
    }

    public IEnumerator doBoolRequest(string action, Dropdown boolD) {
        // 1) Get params
        bool value = Boolean.Parse(boolD.options[boolD.value].text);
        string usid = dropdown.options[dropdown.value].text.Split(' ')[0];

        // 2) Formulate the request
        string url = String.Format("https://{0}/api/v1/{1}?usid={2}&value={3}",
                connect(),
                action,
                usid,
                value
        );
        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Post(url, form)) {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ProtocolError) {
                Debug.Log(www.error);
                yield break;
            }
            response.text = url + "\n" + www.downloadHandler.text;
        }
    }

    public IEnumerator doStringRequest(string action, InputField f) {
        // 1) Get params
        string value = f.text;
        string usid = dropdown.options[dropdown.value].text.Split(' ')[0];

        // 2) Formulate the request
        string url = String.Format("https://{0}/api/v1/{1}?usid={2}&value={3}",
                connect(),
                action,
                usid,
                value
        );
        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Post(url, form)) {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ProtocolError) {
                Debug.Log(www.error);
                yield break;
            }
            response.text = url + "\n" + www.downloadHandler.text;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    public class PlayerIndex {
        public string UserSessionID;
        public List<int> PlayerIdx;
    }

    public static class JsonHelper
    {
        public static string wrapJson(string value) {
            value = "{\"Items\":" + value + "}";
            return value;
        }

        public static List<T> FromJson<T>(string json) {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(List<T> array) {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(List<T> array, bool prettyPrint) {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class Wrapper<T> {
            public List<T> Items;
        }
    }
}