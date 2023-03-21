namespace SeaPortModelling;

public class Storm
{
    public TimeGenerator Duration { get; }
    public float StartTime { get; set;  }
    private float lambda { get; }
    
    public Storm(TimeGenerator duration, float _lambda)
    {
        if (duration == null)
            throw new ArgumentNullException(nameof(duration));
        Duration = duration;
        lambda = _lambda;
    }

    public void Update()
    {
        Random r = new Random();
        StartTime += 1 / lambda * (float) Math.Log(1 - r.NextDouble());
    }
}