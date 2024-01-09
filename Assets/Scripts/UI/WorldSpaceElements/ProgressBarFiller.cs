using UnityEngine.UI;

public class ProgressBarFiller
{
    public static void Fill(Image progressBarImage, float fillAmount) => progressBarImage.fillAmount = fillAmount;
}
