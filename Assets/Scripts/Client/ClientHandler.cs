using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClientHandler : NetworkBehaviour
{
    //Supported devices for client
    private enum Devices
    {
        Android,
        Hololens
    }

    public string playerName; //Client's name
    [SerializeField] private GameObject androidCanvas; //Android canvas we want to instantiate when user disconnects 
    [SerializeField] private GameObject raiseArmButton; //Button that client uses for making a question
    [SerializeField] private Devices device; //Client device
    [SerializeField] private TextMeshProUGUI labelButton; //Content text of raiseArmButton
    private bool raisedArm; //Check if arm is reised or not

    void Start()
    {
        if (!IsOwner)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            //On start we want to add user to the "connected user list" in server side
            CallAddUserServerRpc(OwnerClientId, playerName);
            raiseArmButton.GetComponent<Image>().color = new Color32(43, 180, 45, 255);
            raisedArm = false;
        }
    }

    public void CallRaiseArm(bool flagCall = true)
    {
        raisedArm = !raisedArm;

        if (device == Devices.Android)
        {
            if (raisedArm)
            {
                raiseArmButton.GetComponent<Image>().color = new Color32(227, 224, 50, 255);
                labelButton.text = "Ritira";
            }
            else
            {
                raiseArmButton.GetComponent<Image>().color = new Color32(43, 180, 45, 255);
                labelButton.text = "Domanda";
            }
        }
        else
        {
            //Disattiva hololens button -------------------------------------- TO_DO ------------------------------------
        }

        if (flagCall)
            RaiseArmServerRpc(OwnerClientId, raisedArm);
    }

    public void EnableDisableNotificationButton(bool enable)
    {
        raiseArmButton.gameObject.SetActive(enable);
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    //Disconnect client ------------------------ TO_DO FOR HOLOLENS -----------------------------------------------
    public void Exit()
    {
        //We delete the user from server "connected user list" and we disconnect it
        DeleteStudentServerRpc(OwnerClientId);

        //Then we instantiate again the canvas of the client. Now is ready to put again the name and the lesson code
        Instantiate(androidCanvas);
    }

    [ServerRpc]
    private void RaiseArmServerRpc(ulong playerID, bool flagRaisedArm)
    {
        NetworkManager.Singleton.GetComponent<StartLesson>().ModifyUserArm(playerID, flagRaisedArm);
    }

    [ServerRpc]
    private void CallAddUserServerRpc(ulong clientID, string studentName)
    {
        NetworkManager.Singleton.GetComponent<StartLesson>().AddUser(clientID, studentName);
    }

    [ServerRpc]
    public void DeleteStudentServerRpc(ulong clientId)
    {
        NetworkManager.Singleton.GetComponent<StartLesson>().RemoveUser(clientId);
        NetworkManager.Singleton.DisconnectClient(clientId);
    }
}