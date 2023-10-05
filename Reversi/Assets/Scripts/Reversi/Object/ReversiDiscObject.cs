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

    public Point Point { get {return _disc;} }

    public DiscType DiscColor {get { return _disc.discColor;} set { _disc.discColor = value;}}

    public void Init(Disc disc)
    {
        _disc = disc;
        image.color =_disc.discColor.ToColor();
    }

    public void SetDiscColor(DiscType discColor)
    {
        _disc.discColor = discColor;
        image.color = discColor.ToColor();
    }

    public void SetImageColor(Color color)
    {
        image.color = color;
    }
    
    public void Place()
    {
        ReversiBoardObject.PlaceDisc(_disc);
    }

    public void ApplyAppearance(DiscType color)
    {
        image.color = color.ToColor();
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
