using SeaPortModelling;

public static class Program {
    public static void Main()
    {
        Run();    
    }

    private static void Run()
    {
        Port port = new Port(3, new TimeGenerator(11, 3), 
            new TimeGenerator(240, 24), new float[] {0.25f, 0.55f, 0.2f}, new Storm(new TimeGenerator(4, 2), 1.0f / 48.0f));
        
        port.StartModelling(8640, true);
    }
}