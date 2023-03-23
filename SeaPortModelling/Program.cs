using SeaPortModelling;

public static class Program {
    public static void Main()
    {
        Run(12);
    }

    private static void Run(int N)
    {
        int counter = 0;

        Console.WriteLine("Моделирование работы порта");
        Console.WriteLine("Значения в скобках относятся к ситуациям обычных условий моделирования и с танкерами 4-го типа соответственно");
        Console.WriteLine("\nТеоретически ожидается:\n1) танкеров 1-3 типов — 664\n2) танкеров 4 типа - 180\n3) штормов - 166\n4) ср. время обслуживания - 24.7\n");
        Console.WriteLine("В таблице представлено среднее время ожидания танкеров\nСпециальные танкеры:\n   нет     есть");
        for (int i = 0; i < N; i++)
        {
            counter += IsAddingSpecialTankersOk(8640, 3f, new TimeGenerator(13, 10));
        }

        Console.WriteLine($"\nОтвет: большинство результатов поделирования {((float)N / (float)counter < 2 ? "ЗА заключение" : "ПРОТИВ заключения")} контракта ({counter} из {N})");
    }

    private static int IsAddingSpecialTankersOk(int hours, float queueTimeDifference, TimeGenerator arriveTime)
    {
        Port port1 = new Port(3, arriveTime, new TimeGenerator(240, 24),
            new float[] { 0.25f, 0.55f, 0.2f }, new Storm(new TimeGenerator(4, 2), 1.0f / 48.0f));
        Port port2 = new Port(3, arriveTime, new TimeGenerator(240, 24),
            new float[] { 0.25f, 0.55f, 0.2f }, new Storm(new TimeGenerator(4, 2), 1.0f / 48.0f));
        
        port1.StartModelling(hours, false);
        port2.StartModelling(hours, true);

        Console.WriteLine("| {0, 5:N3} | {1, 5:N3} | (танкеров прибыло: {2} и {3}, из них 4 типа: {4} и {5}, штормов {6} и {7}, ср. время обслуживания {8} и {9})",
            Math.Round(port1.QueueTime, 3), Math.Round(port2.QueueTime, 3), port1.TankersArrived, port2.TankersArrived,
            port1.SpecialTankersArrived, port2.SpecialTankersArrived, port1.StormAmount, port2.StormAmount,
            port1.GetAverageSpentTime(), port2.GetAverageSpentTime());

        return Math.Abs(port1.QueueTime - port2.QueueTime) > queueTimeDifference ? 0 : 1;
    }
}
