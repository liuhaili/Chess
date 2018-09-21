using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Assets.Scripts;
using UnityEngine.SceneManagement;
using Assets.Scripts.Services;
using Chess.Entity;
using Chess.Message;
using Chess.Common;
using DG.Tweening;
using Chess.Message.Enum;

public class Player2Test : MonoBehaviour
{
    public PokerSendCommand SendCommand;
    private void Start()
    {
        SendCommand.InitClient();
        this.Invoke("BattleStart", 2);
    }

    void BattleStart()
    {
        SendCommand.Join("");
    }
}
