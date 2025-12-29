using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Notepad : MonoBehaviour
{
    [TextArea(3, 30)]
    public string note;
}
