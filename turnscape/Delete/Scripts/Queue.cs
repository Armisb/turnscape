using UnityEngine;
using UnityEngine.UIElements;

public class Queue : MonoBehaviour
{
    public GameObject panel; 
    public void Connect()
    {
        QueueService.Connect();
        panel.gameObject.SetActive(true);
    }

    public void LeaveQueue()
    {
        QueueService.LeaveQueue();
        panel.gameObject.SetActive(false);
    }
}
