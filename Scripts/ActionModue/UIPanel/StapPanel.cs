using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public partial class StapPanel
    {
        int backNumInput = 0;
        string jumpStapInput;
        int forwardNumInput = 0;

        public void OnbackNumInputEndEdit(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                backNumInput = -int.Parse(value);
            }
            else
            {
                backNumInput = default(int);
            }
        }
        public void OnjumpStapInputEndEdit(string value)
        {
            jumpStapInput = value;
        }
        public void OnforwardNumInputEndEdit(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                forwardNumInput = int.Parse(value);
            }
            else
            {
                forwardNumInput = default(int);
            }
        }

        public Button accept;
        public Button backAstap;
        public Button backMutiStap;
        public Button toTargetStap;
        public Button skipAStap;
        public Button skipMutiStap;
        public Button toEnd;

        public Toggle autoNext;
        /// <summary>
        /// 注册按扭事件
        /// </summary>
        void Awake()
        {
            accept.onClick.AddListener(OnAcceptButtonCilcked);
            backAstap.onClick.AddListener(OnBackAStapButtonClicked);
            backMutiStap.onClick.AddListener(OnBackMutiButtonClicked);
            toTargetStap.onClick.AddListener(OnToGargetButtonClicked);
            skipAStap.onClick.AddListener(OnSkipAstapButtonClicekd);
            skipMutiStap.onClick.AddListener(OnSkipMutiButtonClicked);
            toEnd.onClick.AddListener(ToEndButtonClicked);

            //accept.onClick.AddListener(OnSelected);
            backAstap.onClick.AddListener(OnStapChange);
            backMutiStap.onClick.AddListener(OnStapChange);
            toTargetStap.onClick.AddListener(OnStapChange);
            skipAStap.onClick.AddListener(OnStapChange);
            skipMutiStap.onClick.AddListener(OnStapChange);
            toEnd.onClick.AddListener(OnStapChange);
        }

        void OnAcceptButtonCilcked()
        {
            remoteController.StartExecuteCommand(OnEndExecute);
            textShow.text = remoteController.CurrCommand.StapName;
        }
        void OnBackAStapButtonClicked()
        {
            remoteController.UnDoCommand();
            if (autoNext.isOn)
            {
                OnAcceptButtonCilcked();
            }
        }

        void OnBackMutiButtonClicked()
        {
            remoteController.ExecuteMutliCommand(backNumInput);
            if (autoNext.isOn)
            {
                OnAcceptButtonCilcked();
            }
        }
        void OnToGargetButtonClicked()
        {
            remoteController.ToTargetCommand(jumpStapInput);
            if (autoNext.isOn)
            {
                OnAcceptButtonCilcked();
            }
        }
        void OnSkipAstapButtonClicekd()
        {
            remoteController.ExecuteMutliCommand(1);
            if (autoNext.isOn)
            {
                OnAcceptButtonCilcked();
            }
        }
        void OnSkipMutiButtonClicked()
        {
            remoteController.ExecuteMutliCommand(forwardNumInput);
            if (autoNext.isOn)
            {
                OnAcceptButtonCilcked();
            }
        }
        void ToEndButtonClicked()
        {
            remoteController.ToAllCommandEnd();
            if (autoNext.isOn)
            {
                OnAcceptButtonCilcked();
            }
        }
        void OnStapChange()
        {
            if (autoNext.isOn)
            {
                textShow.text = remoteController.CurrCommand.StapName;
            }
            else
            {
                textShow.text = "点击接收任务";
            }
        }
        void OnEndExecute(bool haveNext)
        {
            if (haveNext)
            {
                if (autoNext.isOn)
                {
                    OnAcceptButtonCilcked();
                }
            }
            else
            {
                textShow.text = "执行完成";
            }


        }
        public Text textShow;
    }
    public partial class StapPanel : MonoBehaviour
    {
        public GameObject panel;
        IRemoteController remoteController;

        void Start()
        {
            panel.SetActive(false);
            ActionSystem.Instance.GetRemoteController(OnControllerCreated);
        }

        void OnControllerCreated(IRemoteController controller)
        {
            remoteController = controller;
            panel.SetActive(true);
        }
    }

}