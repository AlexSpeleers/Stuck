using UnityEngine;

public sealed class ColorPicker 
{
    private Color color;
    private Color initialColor;    
    private float doneSteps= 0f;
    private float allSteps = 7f;
    public ColorPicker() 
    {
        color = Random.ColorHSV(0f, 1f, 0.3f, 1f, 0.8f, 1f);
        initialColor = color;
    }
    public Color GradientColor() 
    {
        var result = Color.Lerp(initialColor, color, doneSteps / allSteps);
        doneSteps++;
        if (result == color)
        {
            initialColor = color;
            color = Random.ColorHSV(0f, 1f, 0f, 1f, 0.65f, 1f);
            doneSteps = 0f;
        }
        return result;
    }
}