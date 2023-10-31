using T0R1;

public class PortraitUIElement : AspectAffectable
{
    protected override void OnPortrait(object payload)
    {
        gameObject.SetActive(true);
    }

    protected override void OnLandscape(object payload)
    {
        gameObject.SetActive(false);
    }
}
