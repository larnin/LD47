using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FrameStatusChangedEvent
{
    public FrameStatus lastStatus;
    public FrameStatus status;

    public FrameStatusChangedEvent(FrameStatus _status, FrameStatus _lastStatus)
    {
        lastStatus = _lastStatus;
        status = _status;
    }
}
