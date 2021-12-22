using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    enum CardSuit
    {
        SPADE,
        HEART,
        CLUB,
        DIAMOND
    }
    internal class Card : IComparable<Card>
    {
        public CardSuit Suit { get; }
        public int Value { get; }
        public Card(int id)
        {
            Value = id / 4 + 1;
            Suit = (CardSuit)(id % 4);
            if (id == 53) Value = 15;
        }
        static readonly int[] priority = new int[] { 0, 12, 13, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14, 15 };
        public int CompareTo(Card other)
        {
            if (Value != other.Value)
                return priority[Value].CompareTo(priority[other.Value]);
            else return Suit.CompareTo(other.Suit);
        }
        public int getPriority() { return priority[Value]; }
    }
    enum Category
    {
        SOLO,
        PAIR,
        TRIO,
        TRIO_WITH_SOLO,
        TRIO_WITH_PAIR,
        CHAIN_OF_SOLO,
        CHAIN_OF_PAIR,
        CHAIN_OF_TRIO,
        PLANE_WITH_SOLO,
        PLANE_WITH_PAIR,
        QUAD_WITH_SOLO,
        QUAD_WITH_PAIR,
        BOMB,
        ROCKET,
        UNDEFINED
    }
    class Pack
    {
        public Category Category { get; }
        public List<Card> Cards { get; }
        public int MaxValue { get; }
        public int Subtype { get; } = 0;
        public Pack(List<int> _card)
        {
            // 将输入的字符串序列转化为Card序列并排序
            Cards = new List<Card>();
            Dictionary<int, int> map = new Dictionary<int, int>();
            _card.ForEach(id =>
            {
                Cards.Add(new Card(id));
            });
            Cards.Sort();
            Cards.Reverse();
            Console.WriteLine("手中的牌为");
            foreach (Card card in Cards)
                Console.WriteLine("{0} {1}",
                card.Value.ToString(), card.Suit.ToString());

            Console.WriteLine("\n统计量：");
            // 统计单、对、三的数量，后续判断牌型使用
            Cards.ForEach(card =>
            {
                if (map.ContainsKey(card.getPriority())) map[card.getPriority()]++;
                else map.Add(card.getPriority(), 1);
            });
            var Single = (from i in map
                          where i.Value == 1
                          select i.Key).ToList();
            foreach (var i in Single)
                Console.Write("{0} ", i.ToString());
            Console.WriteLine();
            var Double = (from i in map
                          where i.Value == 2
                          select i.Key).ToList();
            foreach (var i in Double)
                Console.Write("{0} ", i.ToString());
            Console.WriteLine();
            var Triple = (from i in map
                          where i.Value == 3
                          select i.Key).ToList();
            foreach (var i in Triple)
                Console.Write("{0} ", i.ToString());
            Console.WriteLine();
            var Quard = (from i in map
                         where i.Value == 4
                         select i.Key).ToList();
            foreach (var i in Quard)
                Console.Write("{0} ", i.ToString());
            Console.WriteLine();

            // 判断牌型
            Category = Category.UNDEFINED;
            switch (_card.Count)
            {

                case 1:
                    // 一张牌一定为单
                    Category = Category.SOLO;
                    MaxValue = Single[0];
                    break;
                case 2:
                    // 对子
                    if (Double.Count == 1)
                    {
                        Category = Category.PAIR;
                        MaxValue = Double[0];
                    }
                    // 王炸
                    if (Cards[0].Value == 15 && Cards[1].Value == 14)
                        Category = Category.ROCKET;
                    break;
                case 3:
                    // 三
                    if (Triple.Count == 1)
                    {
                        Category = Category.TRIO;
                        MaxValue = Triple[0];
                    }
                    break;
                case 4:
                    // 炸弹
                    if (Quard.Count == 1)
                    {
                        MaxValue = Quard[0];
                        Category = Category.BOMB;
                    }
                    // 三带一
                    if (Triple.Count == 1)
                    {
                        Category = Category.TRIO_WITH_SOLO;
                        MaxValue = Triple[0];
                    }
                    break;
                case 5:
                    // 三带一对
                    if (Triple.Count == 1 && Double.Count == 1)
                    {
                        Category = Category.TRIO_WITH_PAIR;
                        MaxValue = Triple[0];
                    }
                    break;
                case 6:
                    // 四带单
                    if (Quard.Count == 1 && Single.Count == 2)
                    {
                        MaxValue = Quard[0];
                        Category = Category.QUAD_WITH_SOLO;
                    }
                    break;
                case 8:
                    // 四带双
                    if (Quard.Count == 1 && Double.Count == 2)
                    {
                        MaxValue = Quard[0];
                        Category = Category.QUAD_WITH_PAIR;
                    }
                    break;
            }
            // 顺子
            if (Single.Count == Cards.Count && Single.Count >= 5 && Continuous(Single) && Single[0]<14)
            {
                Category = Category.CHAIN_OF_SOLO;
                MaxValue = Single[0];
                Subtype = Single.Count;
            }
            // 连对
            if (Double.Count * 2 == Cards.Count && Double.Count >= 3 && Continuous(Double) && Double[0] < 14)
            {
                Category = Category.CHAIN_OF_PAIR;
                MaxValue = Double[0];
                Subtype = Double.Count;
            }
            // 飞机
            if (Triple.Count >= 2 && Continuous(Triple) && Triple[0] < 14)
            {
                // 飞机不带
                if (Triple.Count * 3 == Cards.Count )
                {
                    Category = Category.CHAIN_OF_TRIO;
                    MaxValue = Triple[0];
                    Subtype = Triple.Count;
                }
                else
                {
                    // 飞机带单
                    if (Triple.Count * 4 == Cards.Count)
                    {
                        Category = Category.PLANE_WITH_SOLO;
                        MaxValue = Triple[0];
                        Subtype = Triple.Count;
                    }
                    // 飞机带对
                    else if (Double.Count == Triple.Count && Triple.Count * 5 == Cards.Count)
                    {
                        Category = Category.PLANE_WITH_PAIR;
                        MaxValue = Triple[0];
                        Subtype = Triple.Count;
                    }
                }
            }
            Console.WriteLine("\n牌型");
            Console.WriteLine(Category.ToString());
            Console.WriteLine("\n====================");
        }
        static private bool Continuous(List<int> list)
        {
            if (list[0] == 13) return false;
            for (int i = 1; i < list.Count; i++)
                if (list[i] != list[i - 1] - 1) return false;
            return true;
        }
    }

}
