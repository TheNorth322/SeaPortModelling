using SeaPortModelling;

public static class Program {
    public static void Main()
    {
        Run(12);
    }

    private static void Run(int N)
    {
        int counter = 0;

        Console.WriteLine("МОДЕЛИРОВАНИЕ РАБОТЫ ПОРТА");
        Console.WriteLine("\nТеоретически ожидается:\n1) танкеров 1-3 типов — 665\n2) танкеров 4 типа - 180\n3) штормов - 166\n4) ср. время обслуживания танкеров 1-3 типов - 24,7\n5) ср. время обслуживания танкеров 1-4 типов - 23,9\n");
        Console.WriteLine("СХЕМА: *критерий*: *результат моделирования в обычных условиях* и *результат моделирования при наличии танкеров 4-го типа*\n");

        for (int i = 0; i < N; i++)
        {
            counter += IsAddingSpecialTankersOk(8640, 3f, new TimeGenerator(13, 10));
        }

        Console.WriteLine($"\nОтвет: большинство результатов моделирования {((float)N / (float)counter <= 2 ? "ЗА заключение" : "ПРОТИВ заключения")} контракта ({counter} из {N})");
    }

    private static int IsAddingSpecialTankersOk(int hours, float queueTimeDifference, TimeGenerator arriveTime)
    {
        Port port1 = new Port(3, arriveTime, new TimeGenerator(240, 24),
            new float[] { 0.25f, 0.55f, 0.2f }, new Storm(new TimeGenerator(4, 2), 1.0f / 48.0f));
        Port port2 = new Port(3, arriveTime, new TimeGenerator(240, 24),
            new float[] { 0.25f, 0.55f, 0.2f }, new Storm(new TimeGenerator(4, 2), 1.0f / 48.0f));
        
        port1.StartModelling(hours, false);
        port2.StartModelling(hours, true);

        Console.WriteLine("| ср. время ожидания: {0, 5:N3} и {1, 5:N3} | танкеров прибыло: {2} и {3}, из них 4 типа: {4} и {5} | штормов {6} и {7} | ср. время обслуживания {8, 4:N1} и {9, 4:N1} |",
            Math.Round(port1.QueueTime, 3), Math.Round(port2.QueueTime, 3), port1.TankersArrived, port2.TankersArrived,
            port1.SpecialTankersArrived, port2.SpecialTankersArrived, port1.StormAmount, port2.StormAmount,
            Math.Round(port1.GetAverageSpentTime(), 1), Math.Round(port2.GetAverageSpentTime(), 1));

        return Math.Abs(port1.QueueTime - port2.QueueTime) > queueTimeDifference ? 0 : 1;
    }
}
