namespace SeaPortModelling;

public class Port
{
    private FillingStation[] Stations { get; set; }
    private List<Tanker> Queue { get; }
    private float FirstTankerFrequency { get; }
    private float SecondTankerFrequency { get; }
    private float ThirdTankerFrequency { get; }
    private TimeGenerator SpecialTankerCycle { get; }
    private TimeGenerator ArriveTimeGenerator { get; }
    
    public Port(int stationsAmount, TimeGenerator arriveTimeGenerator, TimeGenerator specialTankerCycle, float firstTankerFrequency, float secondTankerFrequency, float thirdTankerFrequency)
    {
        Queue = new List<Tanker>();
        Stations = new FillingStation[stationsAmount];
        if (arriveTimeGenerator == null)
            throw new ArgumentNullException(nameof(arriveTimeGenerator));
        if (specialTankerCycle == null)
            throw new ArgumentNullException(nameof(specialTankerCycle));
        if (firstTankerFrequency <= 0 || firstTankerFrequency > 1)
            throw new ArgumentException(nameof(firstTankerFrequency));
        if (secondTankerFrequency <= 0 || secondTankerFrequency > 1)
            throw new ArgumentException(nameof(firstTankerFrequency));
        if (thirdTankerFrequency <= 0 || thirdTankerFrequency > 1)
            throw new ArgumentException(nameof(firstTankerFrequency));

        ArriveTimeGenerator = arriveTimeGenerator;
        SpecialTankerCycle = specialTankerCycle;
        FirstTankerFrequency = firstTankerFrequency;
        SecondTankerFrequency = secondTankerFrequency;
        ThirdTankerFrequency = thirdTankerFrequency;
    }

    public void StartModelling(float modellingDuration)
    {
        float currentTime = 0;
        while (currentTime < modellingDuration)
        {
            Tanker tanker = GenerateTanker(currentTime);
            GetStation(tanker);
            currentTime = tanker.ArriveTime;
        } 
    }

    private void GetStation(Tanker tanker)
    {
        foreach (FillingStation station in Stations)
        {
            if (tanker.ArriveTime >= station.ReleaseTime)
            {
                station.IdleTime += tanker.ArriveTime - station.ReleaseTime; 
                station.ReleaseTime += tanker.LoadTime.Get();
                return;
            }
        }

        Random r = new Random();
        int value = r.Next(0, Stations.Length);
        tanker.QueueTime += Stations[value].ReleaseTime - tanker.ArriveTime;
        Stations[value].ReleaseTime += tanker.LoadTime.Get();
    }

    private Tanker GenerateTanker(float currentTime)
    {
        Random r = new Random();
        float arriveTime = currentTime + ArriveTimeGenerator.Get(), tankerType = (float) r.NextDouble();
        TimeGenerator timeGenerator;
        
        if (tankerType < FirstTankerFrequency)
            timeGenerator = new TimeGenerator(18, 2);
        else if (tankerType < FirstTankerFrequency + SecondTankerFrequency)
            timeGenerator = new TimeGenerator(24, 3);
        else
            timeGenerator = new TimeGenerator(35, 4);

        return new Tanker(timeGenerator, 0, arriveTime);
    }
}