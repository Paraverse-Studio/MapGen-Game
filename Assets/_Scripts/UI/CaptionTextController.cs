using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CaptionTextController : MonoBehaviour
{
    public static CaptionTextController Instance;

    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private GameObject _container;

    private List<string> requests = new();

    private void Awake()
    {
        Instance = this;
        _container.SetActive(false);
    }
        
    public void SetText(string msg)
    {
        if (requests.Contains(msg))
        {
            RemoveText(msg);
        }

        requests.Add(msg);
        _text.text = msg;
        _container.SetActive(true);
    }

    public void RemoveText(string msg)
    {
        requests.Remove(msg);

        if (requests.Count > 0 && null != requests[0])
        {
            _text.text = requests[0];
        }
        else
        {
            _container.SetActive(false);
        }
    }

}
