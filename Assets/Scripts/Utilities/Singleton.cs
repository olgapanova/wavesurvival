using System;

public class Singleton<TClass> where TClass : class 
{
    public static TClass Instance
    {
        get;
        private set;
    }

    public static TClass CreateInstance()
    {
        if (Instance == null)
            Instance = Activator.CreateInstance<TClass>();

        return Instance;
    }
}
