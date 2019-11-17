using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gFtp;

namespace gFtpGUI
{
    public enum JobState
    {
        Ready,
        Waiting,
        Pending,
        Running,
        Completed,
        Failed
    }

    public class QueueItem
    {
        public Int32 Order { get; set; }
        public Job Job { get; set; }
        public FileSize Size { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public JobState State { get; set; }


        private TimeSpan? _FinalDuration = null;
        public TimeSpan? Duration
        {
            get
            {
                switch (State)
                {
                    case JobState.Running:
                        return ((TimeSpan?)(DateTime.Now - StartTime)).Value;
                    case JobState.Completed:
                        if (_FinalDuration.HasValue)
                        {
                            return _FinalDuration.Value;
                        }
                        else
                        {
                            _FinalDuration = (TimeSpan?)(EndTime - StartTime);
                            return _FinalDuration.Value;
                        }
                    case JobState.Ready:
                    case JobState.Waiting:
                    case JobState.Pending:
                    case JobState.Failed:
                    default:
                        return null;
                }
            }
        }

        private FileSpeed _FinalSpeed = null;
        public FileSpeed Speed
        {
            get
            {
                if(_FinalSpeed != null)
                {
                    return _FinalSpeed;
                }
                FileSpeed s = new FileSpeed();
                if(Size!= null && Duration != null && State == JobState.Completed)
                {
                    s.Size = Size.Size;
                    s.TotalMilliseconds = Duration.Value.TotalMilliseconds;
                    _FinalSpeed = s;
                }
                return s;
            }
        }
    }
}
