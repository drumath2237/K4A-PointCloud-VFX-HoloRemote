using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA;

namespace K4APointCloudVFX
{
    public class HoloRemote : MonoBehaviour
    {
        [SerializeField] private string IP = "192.168.11.64";

        private bool connected = false;

        void Connect()
        {
            if (HolographicRemoting.ConnectionState != HolographicStreamerConnectionState.Connected)
            {
                HolographicRemoting.Connect(IP);
            }
        }
        
        void Update()
        {
            if (!connected && HolographicRemoting.ConnectionState == HolographicStreamerConnectionState.Connected)
            {
                connected = true;

                StartCoroutine(LoadDevice("WindowsMR"));
            }
        }

        IEnumerator LoadDevice(string newDevice)
        {
            XRSettings.LoadDeviceByName(newDevice);
            yield return null;
            XRSettings.enabled = true;
        }

        private void Start()
        {
            Connect();
        }

        private void OnDestroy()
        {
            HolographicRemoting.Disconnect();
            connected = false;
            while (HolographicRemoting.ConnectionState == HolographicStreamerConnectionState.Connected)
            {
                ;
            }
        }

//        private void OnGUI()
//        {
//            IP = GUI.TextField(new Rect(10, 10, 200, 30), IP, 25);
//
//            string button = (connected ? "Disconnect" : "Connect");
//
//            if (GUI.Button(new Rect(220, 10, 100, 30), button))
//            {
//                if (connected)
//                {
//                    HolographicRemoting.Disconnect();
//                    connected = false;
//                }
//                else
//                    Connect();
//
//                Debug.Log(button);
//            }
//        }
    }
}