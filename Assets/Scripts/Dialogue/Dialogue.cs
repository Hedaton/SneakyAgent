using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public string characterName;
    [TextArea(3, 10)]
    public string[] sentences;
}
