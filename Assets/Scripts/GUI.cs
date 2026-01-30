using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    // Adjust via the Inspector
    public int maxLines = 8;
    private Queue<string> queue = new Queue<string>();
    private string currentText = "";

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Delete oldest message
        if (queue.Count >= maxLines) queue.Dequeue();

        queue.Enqueue(logString);

        var builder = new StringBuilder();
        foreach (string st in queue)
        {
            builder.Append(st).Append("\n");
        }

        currentText = builder.ToString();
    }

    void OnGUI()
    {
        GUI.Label(
           new Rect(
               Screen.width - 705,                   // x, left offset
               5, // y, bottom offset
               700f,                // width
               250f                 // height
           ),
           currentText,             // the display text
           GUI.skin.textArea        // use a multi-line text area
        );
    }
}