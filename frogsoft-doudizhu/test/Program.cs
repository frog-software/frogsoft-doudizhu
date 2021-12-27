// See https://aka.ms/new-console-template for more information
using test;

//Random random = new Random();
//for (int t = 0; t < 10;)
//{
//    List<int> list = new List<int>();
//    int num = random.Next(17) + 1;
//    HashSet<int> set = new HashSet<int>();
//    for (int i = 0; i < num; i++)
//    {
//        int card;
//        do
//        {
//            card = random.Next(54);
//        } while (set.Contains(card));
//        set.Add(card);
//        list.Add(card);
//    }
//    Pack pack = new Pack(list);
//    if (pack.Category != Category.UNDEFINED &&
//        pack.Category != Category.SOLO &&
//        pack.Category != Category.PAIR)
//    {
//        Console.ReadKey();
//        t++;
//    }
//}


//Pack pack1 = new Pack(new List<int> { 25, 26, 27, 43 });
//Pack pack2 = new Pack(new List<int> { 37, 38, 39, 46 });
//pack1.Print();
//pack2.Print();
//Pack.PrintList(pack1.getList());
//Pack.PrintList(pack2.getList());
//Console.WriteLine(pack1 < pack2);
//Console.WriteLine(pack1 > pack2);

int read()
{
    int f = 1;
    int s = 0;
    int c = Console.Read();
    for (; c < '0' || c > '9'; c = Console.Read()) if (c == '-') f = -1;
    for (; c >= '0' && c <= '9'; c = Console.Read()) s = s * 10 + c - '0';
    return f * s;
}

int T = read();
int n = read();
while (T-- > 0)
{
    List<int> list = new List<int>();
    for (int i = 0; i < n; i++)
    {
        int a = read();
        int b = read();
        if (a == 0)
        {
            // 小王
            if (b == 1) list.Add(52);
            // 大王
            else if (b == 2) list.Add(53);
        }
        else list.Add(a * 4 + b - 5);
    }
    Pack pack = new Pack(list);
    pack.Print();
    while (pack.Count > 0)
    {
        Console.WriteLine(pack.MinCase1());
        new Pack(pack.getAnsShouldOut()).Print();
        pack -= new Pack(pack.getAnsShouldOut());
    }
    pack = new Pack(list);
    pack.NextPack(new Pack(new List<int> { 11 }));
    new Pack(pack.NextPack(new Pack(new List<int> { 3 }))).Print();
    Console.WriteLine("end");
}

