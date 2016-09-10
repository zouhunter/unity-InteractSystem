abstract public class Singleton<T> where T:new()
{
    private static T instance = default(T);
    private static object lockHelper = new object();
    public static bool mManualReset = false;

    protected Singleton() { }
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockHelper)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }
            }
            return instance;
        }
    }
};

