// See https://aka.ms/new-console-template for more information
using test;

Random random= new Random();
for (int t = 0; t < 100;)
{
    List<int> list = new List<int>();
    int num = random.Next(17) + 1;
    HashSet<int> set = new HashSet<int>();
    for (int i = 0; i < num; i++)
    {
        int card;
        do
        {
            card = random.Next(54);
        } while (set.Contains(card));
        set.Add(card);
        list.Add(card);
    }
    Pack pack = new Pack(list);
    if (pack.Category != Category.UNDEFINED &&
        pack.Category != Category.SOLO &&
        pack.Category != Category.PAIR)
    {
        Console.ReadKey();
        t++;
    }
}
