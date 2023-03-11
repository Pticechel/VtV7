public class StringPair
{
    public string Original { get; }
    public string Encrypted { get; }

    public StringPair(string original)
    {
        Original = original;
        Encrypted = EncryptString(original);
    }

    private string EncryptString(string original)
    {
        char[] chars = original.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = (char)(chars[i] + 1);
        }
        return new string(chars);
    }
}

public class StringPairCollection
{
    private List<StringPair> _pairs = new List<StringPair>();

    public void Add(string original)
    {
        _pairs.Add(new StringPair(original));
    }

    public void ProcessPairsWithThreadPool()
    {
        int threadsCount = Environment.ProcessorCount;
        ThreadPool.SetMaxThreads(threadsCount, threadsCount);

        foreach (var pair in _pairs)
        {
            ThreadPool.QueueUserWorkItem(ProcessPair, pair);
        }

        Thread.Sleep(1000);
    }

    private void ProcessPair(object obj)
    {
        StringPair pair = (StringPair)obj;
        Console.WriteLine($"Original string: {pair.Original}, Encrypted string: {pair.Encrypted}, Thread Id: {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(100);
    }
}

class Program
{
    static void Main(string[] args)
    {
        StringPairCollection collection = new StringPairCollection();
        collection.Add("Hello");
        collection.Add("World");
        collection.Add("C# is awesome!");

        collection.ProcessPairsWithThreadPool();

        Console.ReadKey();
    }
}