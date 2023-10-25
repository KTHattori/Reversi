using T0R1;

public class LandscapeUIElement : AspectAffectable
{
    protected override void OnPortrait(object payload)
    {
        gameObject.SetActive(false);
    }

    protected override void OnLandscape(object payload)
    {
        gameObject.SetActive(true);
    }
}
