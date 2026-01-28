using UnityEngine;

// @kurtdekker - dead-simple infinite loop finder
// TODO: call BREAKOUT.Check(); from within every suspect loop.
// TODO: optionally supply a custom max callcount

public class BREAKOUT : MonoBehaviour
{
    static BREAKOUT _instance;

    const int MaxCallCount = 100000;

    static int callCount;

    public static void Check(int customMaxCallCount = 0)
    {
        if (!_instance)
        {
            _instance = new GameObject("BREAKOUT").AddComponent<BREAKOUT>();
        }

        if (customMaxCallCount == 0)
        {
            customMaxCallCount = MaxCallCount;
        }

        callCount++;

        // we're still good, continue
        if (callCount < customMaxCallCount)
        {
            return;
        }

        // whoops... we're spinning... 
        Debug.Log("Look at the stack, or put a breakpoint here.");

        throw new System.Exception("Blew a loop check!");
    }

    private void LateUpdate()
    {
        callCount = 0;
    }
}
