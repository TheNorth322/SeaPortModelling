namespace SeaPortModelling;

public class Tanker
{
    public TimeGenerator LoadTime { get; }
    public float QueueTime { get; set;  }
    public float ArriveTime { get; set; }
    
    public Tanker(TimeGenerator loadTime, float queueTime, float arriveTime)
    {
        if (loadTime == null)
            throw new ArgumentNullException(nameof(loadTime));
        LoadTime = loadTime;
        QueueTime = queueTime;
        ArriveTime = arriveTime;
    }
}