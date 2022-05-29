using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using UnityEngine.UI;
using PiepackerSDK;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System;

public class Main : MonoBehaviour
{
    public Text response;

    public Dropdown dropdown;

    public Dropdown mutedDropdown;

    public Dropdown deafDropdown;

    public Dropdown blindDropdown;

    public Dropdown mvDropdown;

    public Dropdown wvDropdown;

    private static readonly HttpClient client = new HttpClient();

    private static readonly string connectionHost = "bandicoot-kvfl9-t5cmk.api.piepackerstaging.com";

    // Start is called before the first frame update
    void Start()
    {
        // Muted Setup
        trueFalseDropdownSetup(mutedDropdown);

        // Dropdown setup
        dropdown.ClearOptions();
        dropdown.onValueChanged.AddListener(delegate {
            dropdown.RefreshShownValue();
        });
        Refresh();
    }

    private void trueFalseDropdownSetup(Dropdown d) {
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

    public async void Refresh () {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format("https://{0}/api/v1/player-indexes", connectionHost));
        request.Content = new StringContent("", Encoding.UTF8, "application/json");
        var resp = await client.SendAsync(request);
        var text = await resp.Content.ReadAsStringAsync();
        List<PlayerIndex> t = JsonConvert.DeserializeObject<List<PlayerIndex>>(text);
        // PlayerIndex[] t = JsonHelper.FromJson<PlayerIndex>(text);
        var message = request.RequestUri + "\n";
        foreach (PlayerIndex p in t) {
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

    // called when clicked on the mute button
    public async void Mute() {
        doBoolRequest("muted", mutedDropdown);
    }

    public async void Blind() {
        doBoolRequest("blind", blindDropdown);
    }

    public async void Deaf() {
        doBoolRequest("deaf", deafDropdown);
    }

    public async void MV() {
        doBoolRequest("mask-visible", mvDropdown);
    }

    public async void WV() {
        doBoolRequest("webcam-visible", wvDropdown);
    }

    public async void doBoolRequest(string action, Dropdown boolD) {
        // 1) Get params
        bool value = Boolean.Parse(boolD.options[boolD.value].text);
        string usid = dropdown.options[dropdown.value].text;

        // 2) Formulate the request
        string uri = String.Format("https://{0}/api/v1/{1}?usid={2}&value={3}",
                connectionHost,
                action,
                usid,
                value
        );
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
        var resp = await client.SendAsync(request);
        // 3) populate response
        response.text = uri + "\n" + resp.StatusCode.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public class PlayerIndex {
        public string UserSessionID { get; set; }
        public List<int> PlayerIdx { get; set; }
    }
}