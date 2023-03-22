namespace SeaPortModelling;

public class Port
{
    private FillingStation[] Stations { get; set; }
    private float[] TankerFrequencies { get; }
    private TimeGenerator SpecialTankerCycle { get; }
    private TimeGenerator ArriveTimeGenerator { get; }

    private Storm Storm { get; }
    private int SpecialTankersArrived { get; set; }
    private int StormAmount { get; set; }
    private Tanker[] SpecialTankers { get; }

    public Port(int stationsAmount, TimeGenerator arriveTimeGenerator, TimeGenerator specialTankerCycle,
        float[] tankerFrequencies, Storm storm)
    {
        Stations = new FillingStation[stationsAmount];
        SpecialTankers = new Tanker[5];
        InitializeStations();
        if (arriveTimeGenerator == null)
            throw new ArgumentNullException(nameof(arriveTimeGenerator));
        if (specialTankerCycle == null)
            throw new ArgumentNullException(nameof(specialTankerCycle));
        if (storm == null)
            throw new ArgumentNullException(nameof(storm));
        CheckTankerFrequencies(tankerFrequencies);

        Storm = storm;
        ArriveTimeGenerator = arriveTimeGenerator;
        SpecialTankerCycle = specialTankerCycle;
        TankerFrequencies = tankerFrequencies;
    }

    private void InitializeSpecialTankers(float currentTime)
    {
        for (int i = 0; i < SpecialTankers.Length; i++)
            SpecialTankers[i] = new Tanker(new TimeGenerator(21, 3), 0, currentTime + SpecialTankerCycle.Get());
    }

    private void InitializeStations()
    {
        for (int i = 0; i < Stations.Length; i++)
            Stations[i] = new FillingStation();
    }

    private void CheckTankerFrequencies(float[] frequencies)
    {
        foreach (float frequency in frequencies)
            if (frequency < 0 || frequency > 1)
                throw new ArgumentException(nameof(frequency));
    }

    public void StartModelling(float modellingDuration, bool specialTankers)
    {
        float currentTime = 0;

        if (specialTankers)
            InitializeSpecialTankers(currentTime);
        
        while (currentTime < modellingDuration)
        {
            if (currentTime >= Storm.ArrivalTime && currentTime <= Storm.ArrivalTime + Storm.CurrentDuration)
            {
                currentTime = Storm.ArrivalTime + Storm.CurrentDuration;
            }
            else
            {
                Tanker tanker = GenerateCommonTanker(currentTime);
                GetStation(tanker);
                if (specialTankers)
                    CheckSpecialTankersArrival(currentTime);
                currentTime = tanker.ArriveTime;
            }

            if (Storm.ArrivalTime <= currentTime)
            {
                Storm.Get();
                StormAmount++;
            }
        }

        CalculateSpentTime(modellingDuration);
    }

    private void CalculateSpentTime(float modellingDuration)
    {
        foreach (FillingStation station in Stations)
            station.SpentTime = modellingDuration - station.IdleTime;
    }

    private void CheckSpecialTankersArrival(float currentTime)
    {
        foreach (Tanker tanker in SpecialTankers)
        {
            if (currentTime > tanker.ArriveTime)
            {
                GetStation(tanker);
                SpecialTankersArrived++;
                tanker.ArriveTime = currentTime + SpecialTankerCycle.Get();
            }
        }
    }

    private int GetTankersAmount()
    {
        int count = 0;
        foreach (FillingStation station in Stations)
            count += station.ServedAmount;
        return count;
    }

    private void GetStation(Tanker tanker)
    {
        foreach (FillingStation station in Stations)
        {
            if (tanker.ArriveTime >= station.ReleaseTime)
            {
                station.IdleTime += tanker.ArriveTime - station.ReleaseTime;
                station.ReleaseTime = tanker.ArriveTime + tanker.LoadTime.Get();
                station.ServedAmount++;
                return;
            }
        }

        Random r = new Random();
        int value = r.Next(0, Stations.Length);
        tanker.QueueTime += Stations[value].ReleaseTime - tanker.ArriveTime;
        Stations[value].ReleaseTime += tanker.LoadTime.Get();
        Stations[value].ServedAmount++;
    }

    private Tanker GenerateCommonTanker(float currentTime)
    {
        Random r = new Random();
        float arriveTime = currentTime + ArriveTimeGenerator.Get(), tankerType = (float)r.NextDouble();
        TimeGenerator timeGenerator;

        if (tankerType < TankerFrequencies[0])
            timeGenerator = new TimeGenerator(18, 2);
        else if (tankerType < TankerFrequencies[0] + TankerFrequencies[1])
            timeGenerator = new TimeGenerator(24, 3);
        else
            timeGenerator = new TimeGenerator(35, 4);

        return new Tanker(timeGenerator, 0, arriveTime);
    }
}