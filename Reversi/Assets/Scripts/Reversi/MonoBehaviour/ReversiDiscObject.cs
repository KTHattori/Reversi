using UnityEngine;
using UnityEngine.UI;
using Reversi;


[RequireComponent(typeof(Image))]
public class ReversiDiscObject : MonoBehaviour
{
    // private
    [SerializeField]
    private Image _image;

    [SerializeField]
    private Disc _disc;

    // public 
    public Point Point { get {return _disc;} }

    public DiscType DiscColor {get { return _disc.discType;} set { _disc.discType = value;}}

    /// <summary>
    /// ディスク情報をセットする。
    /// </summary>
    /// <param name="disc"></param>
    public void SetDisc(Disc disc)
    {
        _disc = disc;
        ApplyColor(disc.discType);
    }

    /// <summary>
    /// ディスクの色情報をセットする。
    /// </summary>
    /// <param name="discColor"></param>
    public void SetDiscColor(DiscType discColor)
    {
        _disc.discType = discColor;
        ApplyColor(discColor);
    }

    /// <summary>
    /// 画像色をセットする。
    /// </summary>
    /// <param name="color"></param>
    public void SetImageColor(Color color)
    {
        _image.color = color;
    }
    
    /// <summary>
    /// ボード上に配置する処理
    /// </summary>
    public void Place()
    {
        ReversiBoardObject.PlaceDisc(_disc);
    }

    /// <summary>
    /// 石色を画像に適用する
    /// </summary>
    /// <param name="color"></param>
    void ApplyColor(DiscType color)
    {
        _image.color = color.ToColor();
    }

    /// <summary>
    /// コンポーネントがアタッチされた時にコールされる関数
    /// </summary>
    void Reset()
    {
        if(!TryGetComponent<Image>(out _image))
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
        if(!TryGetComponent<Image>(out _image))
        {
            Debug.LogError("ReversiDiscObject: Failed to get Image component.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
