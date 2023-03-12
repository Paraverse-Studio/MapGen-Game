using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CaptionTextController : MonoBehaviour
{
    public struct Request
    {
        public string msg;
        public GameObject requester;
        public Request (string m, GameObject g)
        {
            msg = m; requester = g;
        }
    }

    public static CaptionTextController Instance;

    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private GameObject _container;

    private List<Request> requests = new();

    private void Awake()
    {
        Instance = this;
        _container.SetActive(false);
    }
        
    private void SetText(string s)
    {
        _text.text = s;
    }

    public void SetText(string msg, GameObject obj)
    {
        RemoveText(msg, obj);

        Request newReq = new(msg, obj); 
        requests.Add(newReq);
        SetText(msg);
        _container.SetActive(true);
    }

    public void RemoveText(string msg, GameObject obj)
    {
        Request checkExistingRequest = requests.FirstOrDefault(r => r.requester == obj);

        if (!string.IsNullOrEmpty(checkExistingRequest.msg))
        {
            requests.Remove(checkExistingRequest);
        }

        if (requests.Count > 0)
        {
            SetText(requests[0].msg);
        }
        else
        {
            if (_container) _container.SetActive(false);
        }
    }

}
