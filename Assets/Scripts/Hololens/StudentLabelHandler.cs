using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StudentLabelHandler : MonoBehaviour
{
    [SerializeField] GameObject handButton;
    [SerializeField] MeshRenderer iconMeshRendererHand;
    [SerializeField] MeshRenderer iconMeshRendererBlock;
    [SerializeField] MeshRenderer backPlateHandButton;
    private ulong clientID;
    [SerializeField] Material materialYellow;
    [SerializeField] Material materialRed;
    [SerializeField] Material materialGreen;
    [SerializeField] Material materialWhite;
    [SerializeField] Material materialGrey;

    // Start is called before the first frame update
    void Start()
    {
        handButton.GetComponent<Interactable>().IsEnabled = false;
    }

    public void SetClientID(ulong id) { 
        clientID = id;
    }

    public ulong GetClientID()
    {
        return clientID;
    }

    public void EnableHandButton() {
        handButton.GetComponent<Interactable>().IsEnabled = true;
        iconMeshRendererHand.material = materialYellow;
        backPlateHandButton.material = materialGreen;
    }

    public void RemoveNotification(bool flagSend = true) {

        handButton.GetComponent<Interactable>().IsEnabled = false;
        
        if (flagSend) {
            NetworkManager.Singleton.GetComponent<StartLesson>().ModifyUserArm(clientID, false);
            GameObject model = GameObject.FindGameObjectsWithTag("SpawnedModel")[0];
            model.GetComponent<SendInfoClient>().RemoveNotificationClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientID },
                }
            });
        }

        
        iconMeshRendererHand.material = materialWhite;
        backPlateHandButton.material = materialGrey;

    }

    public void EnableDisableClientAction(bool flagSend) {

        bool enable = !handButton.activeSelf;
        handButton.SetActive(enable);

        if (enable)
            iconMeshRendererBlock.material = materialRed;

        if (flagSend)
        {
            NetworkManager.Singleton.GetComponent<StartLesson>().ModifyUserBlock(clientID);
            GameObject model = GameObject.FindGameObjectsWithTag("SpawnedModel")[0];
            model.GetComponent<SendInfoClient>().EnableDisableClientRpc(enable, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientID },
                }
            });
        }

    }
}
