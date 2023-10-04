using UnityEngine;
using UnityEngine.UI;
using Reversi;


[RequireComponent(typeof(Image))]
public class ReversiDiscObject : MonoBehaviour
{
    [SerializeField]
    Image image;

    [SerializeField]
    Disc _disc;

    public void Set(Disc disc)
    {
        _disc = disc;
        ApplyAppearance(disc.color);
    }

    public void Place()
    {
        
    }

    void ApplyAppearance(DiscColor color)
    {
        switch(color)
        {
            case DiscColor.White:
            image.color = Color.white;
            break;
            case DiscColor.Black:
            image.color = Color.black;
            break;
            case DiscColor.Empty:
            image.color = new Color(0.0f,0.0f,0.0f,0.0f);
            break;
            case DiscColor.Wall:
            image.color = Color.red;
            break;
        }
    }

    void Reset()
    {
        if(!TryGetComponent<Image>(out image))
        {
            Debug.LogError("ReversiDiscObject: Failed to get Image component.");
        }
    }

    void OnValidate()
    {
        ApplyAppearance(_disc.color);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!TryGetComponent<Image>(out image))
        {
            Debug.LogError("ReversiDiscObject: Failed to get Image component.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
