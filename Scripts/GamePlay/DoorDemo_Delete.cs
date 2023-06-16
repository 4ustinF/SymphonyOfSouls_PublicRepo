using DG.Tweening;
using UnityEngine;

public class DoorDemo_Delete : MonoBehaviour
{
    [SerializeField] private Transform closedPos=null;
    public void OpenDoor()
    {
        this.gameObject.transform.DOMove(closedPos.position, 3.0f);
        //  this.gameObject.transform.DORotate(closedPos.rotation.eulerAngles, 3.0f);
    }
}
