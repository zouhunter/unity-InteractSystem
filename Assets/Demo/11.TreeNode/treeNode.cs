using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldActionSystem;
using UnityEngine.UI;

public partial class treeNode
{
    int backNumInput = 0;
    string jumpStapInput;
    int forwardNumInput = 0;
   
    public Config config;
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
    public Button start;
    public Button backAstep;
    public Button backMutiStap;
    public Button toTargetStap;
    public Button skipAStap;
    public Button skipMutiStap;
    public Button toEnd;

    public Toggle notice;
    public Toggle autoNext;
    public Toggle autoPlay;

    public InputField backNum;
    public InputField nextNum;
    public InputField jumpStep;
    /// <summary>
    /// 注册按扭事件
    /// </summary>
    void Awake()
    {
        Config.Defult = config;
        accept.onClick.AddListener(OnAcceptButtonCilcked);
        start.onClick.AddListener(OnToStartButtonClicked);
        backAstep.onClick.AddListener(OnBackAStapButtonClicked);
        backMutiStap.onClick.AddListener(OnBackMutiButtonClicked);
        toTargetStap.onClick.AddListener(OnToGargetButtonClicked);
        skipAStap.onClick.AddListener(OnSkipAstepButtonClicekd);
        skipMutiStap.onClick.AddListener(OnSkipMutiButtonClicked);
        toEnd.onClick.AddListener(ToEndButtonClicked);
        notice.onValueChanged.AddListener(OnNoticeStateChanged);
        autoPlay.onValueChanged.AddListener(OnAutoPlayStateChanged);
        //accept.onClick.AddListener(OnSelected);
        start.onClick.AddListener(OnStapChange);
        backAstep.onClick.AddListener(OnStapChange);
        backMutiStap.onClick.AddListener(OnStapChange);
        toTargetStap.onClick.AddListener(OnStapChange);
        skipAStap.onClick.AddListener(OnStapChange);
        skipMutiStap.onClick.AddListener(OnStapChange);
        toEnd.onClick.AddListener(OnStapChange);

        backNum.onValueChanged.AddListener(OnbackNumInputEndEdit);
        jumpStep.onValueChanged.AddListener(OnjumpStapInputEndEdit);
        nextNum.onValueChanged.AddListener(OnforwardNumInputEndEdit);
        
    }

    private void OnAutoPlayStateChanged(bool arg0)
    {
        OnAcceptButtonCilcked();
    }

    void OnAcceptButtonCilcked()
    {
        remoteController.StartExecuteCommand(OnEndExecute, autoPlay.isOn);
        if (remoteController.CurrCommand != null)
        {
            textShow.text = remoteController.CurrCommand.StepName;
        }
    }
    void OnToStartButtonClicked()
    {
        remoteController.ToAllCommandStart();
        if (autoNext.isOn)
        {
            OnAcceptButtonCilcked();
        }
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
    void OnSkipAstepButtonClicekd()
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
            textShow.text = remoteController.CurrCommand != null ? remoteController.CurrCommand.StepName : "结束";
        }
        else
        {
            textShow.text = "点击接收任务";
        }
    }
    void OnEndExecute()
    {
        if (autoNext.isOn)
        {
            OnAcceptButtonCilcked();
        }
        OnStapChange();
    }
    void OnNoticeStateChanged(bool isOn)
    {
        Config.Defult._highLightNotice = isOn;
    }
    public Text textShow;
}
public partial class treeNode : MonoBehaviour {
    [SerializeField]
    private ActionGroup group;
    private ICommandController remoteController { get { return group.RemoteController; } }
    // Use this for initialization
    void Start()
    {
        var dic = new Dictionary<string, string[]>();
        dic.Add("command00", new string[] { "command03_a1", "command03_a2" });
        dic.Add("command01", new string[] { "command02_a", "command02_b" });
        dic.Add("command02_a", new string[] { "command03_a1", "command03_a2" });
        dic.Add("command02_b", new string[] { "command04_b1", "command04_b2" });
        group.LunchActionSystem(dic, ()=>
        {
            Debug.Log("InitOK");
        });
        group.onUserError += OnUserError;
    }

    private void OnUserError(string step, string info)
    {
        Debug.Log(step + ":" + info);
    }
}
