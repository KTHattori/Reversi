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

    public Point point { get{return _disc;} }

    public DiscColor color {get { return _disc.color;}}

    public void Set(Disc disc)
    {
        _disc = disc;
        ApplyAppearance(_disc.color);
    }

    public void Place()
    {
        ReversiBoardObject.PlaceDisc(_disc);
    }

    public void ApplyAppearance(DiscColor color)
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
        // ApplyAppearance();
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
