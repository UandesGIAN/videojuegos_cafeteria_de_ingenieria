using UnityEngine;

[System.Serializable]
public class Question
{
    public string category; // "Civil", "Computacion", "Ambiental", "Obras", "PlanComun"
    public string questionText;
    [TextArea] public string[] answers = new string[4];
    public int correctAnswerIndex; 
}
