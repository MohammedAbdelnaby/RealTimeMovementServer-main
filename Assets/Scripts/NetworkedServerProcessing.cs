using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkedServerProcessing
{

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int clientConnectionID)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + clientConnectionID);

        string[] csv = msg.Split(',');

        switch (csv[0])
        {
            case "Input":
                SendMessageToAllPlayersButOne(msg + "," + clientConnectionID, clientConnectionID);
                break;
            case "New Player":
                AddPlayer(clientConnectionID, csv[1]);
                SendMessageToAllPlayersButOne("New Player," + clientConnectionID + "," + csv[1], clientConnectionID);
                SendCurrentGameState(clientConnectionID);
                break;
            case "Movement":
                UpdatePlayerList(csv[1], clientConnectionID);
                UpdateGame(clientConnectionID);
                break;
            default:
                break;
        }
    }

    private static void UpdatePlayerList(string pos, int id)
    {
        Player temp = networkedServer.Players.Find((Player player) => player.ID == id);
        networkedServer.Players.Remove(temp);
        temp.playerPositionInPercent = Vector2ToString(pos);
        networkedServer.Players.Add(temp);
    }

    static public void SendMessageToClient(string msg, int clientConnectionID)
    {
        networkedServer.SendMessageToClient(msg, clientConnectionID);
    }

    static public void SendMessageToClientWithSimulatedLatency(string msg, int clientConnectionID)
    {
        networkedServer.SendMessageToClientWithSimulatedLatency(msg, clientConnectionID);
    }

    static public void SendMessageToAllPlayersButOne(string msg, int id)
    {
        foreach (Player player in networkedServer.Players)
        {
            if (player.ID != id)
                SendMessageToClient(msg, player.ID);
        }
    }

    static public void SendCurrentGameState(int id)
    {
        string temp = "GameState,";
        foreach (Player player in networkedServer.Players)
        {
            temp += ":" + player.ID + ";" + player.playerPositionInPercent.x + ";" + player.playerPositionInPercent.y;
        }
        SendMessageToClient(temp, id);
    }

    static public void UpdateGame(int id)
    {
        string temp = "Movement,";
        foreach (Player player in networkedServer.Players)
        {
            if (player.ID != id)
            {
                temp += ":" + player.ID + ";" + player.playerPositionInPercent.x + ";" + player.playerPositionInPercent.y;
                SendMessageToClient(temp, player.ID);
            }
        }
    }

    static public Vector2 Vector2ToString(string msg)
    {
        string[] sTemp = msg.Split(';');
        Vector2 temp = new Vector2(float.Parse(sTemp[0]), float.Parse(sTemp[1]));
        return temp;
    }

    static public Vector2 StringToVector(string msg)
    {
        string[] coord = msg.Split(';');
        Vector2 temp;
        temp = new Vector2(float.Parse(coord[0]), float.Parse(coord[1]));
        return temp;
    }

    #endregion

    #region Connection Events

    static public void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Connection, ID == " + clientConnectionID);
        //SendMessageToAllPlayersButOne("New Player," + clientConnectionID, clientConnectionID);
    }

    static public void AddPlayer(int id, string position)
    {
        Player player = new Player(id);
        player.playerPositionInPercent = Vector2ToString(position);
        networkedServer.Players.Add(player);
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Disconnection, ID == " + clientConnectionID);
    }

    #endregion

    #region Setup
    static NetworkedServer networkedServer;
    static GameLogic gameLogic;

    static public void SetNetworkedServer(NetworkedServer NetworkedServer)
    {
        networkedServer = NetworkedServer;
    }
    static public NetworkedServer GetNetworkedServer()
    {
        return networkedServer;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion
}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int asd = 1;
}

static public class ServerToClientSignifiers
{
    public const int asd = 1;
}

#endregion