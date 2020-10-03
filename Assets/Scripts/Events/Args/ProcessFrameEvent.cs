using UnityEngine;
using System.Collections;

public class ProcessFrameEvent
{
    public int frame;
    public int maxFrame;
    public float time;
    public float maxTime;
    public FrameStatus status;

    public ProcessFrameEvent(int _frame, int _maxFrame, float _time, float _maxTime, FrameStatus _status)
    {
        frame = _frame;
        maxFrame = _maxFrame;
        time = _time;
        maxTime = _maxTime;
        status = _status;
    }
}
