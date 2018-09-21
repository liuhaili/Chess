using Chess.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FightClient : ObjectBase
{
    public SendServerCommand SendCommand;
    public Button btnMatch;
    private void Start()
    {
        EventListener.Get(btnMatch.gameObject).onClick = OnClicked;
    }

    void OnClicked(GameObject sender)
    {
        SendCommand.Match();
    }
}