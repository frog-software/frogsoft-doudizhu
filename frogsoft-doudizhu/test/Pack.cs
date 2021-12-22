﻿using System;
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
            else return -Suit.CompareTo(other.Suit);
        }
        public int getPriority() { return priority[Value]; }
        public int getId()
        {
            if (Value == 15) return 53;
            if (Value == 14) return 52;
            return (Value - 1) * 4 + (int)Suit;
        }
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
        private readonly List<int> Single;
        private readonly List<int> Double;
        private readonly List<int> Triple;
        private readonly List<int> Quard;
        private Dictionary<int, int> map=new();
        static public void PrintList(List<int> list)
        {
            foreach (var i in list)
                Console.Write("{0} ", i.ToString());
            Console.WriteLine();
        }
        public void Print()
        {
            Console.WriteLine("手中的牌为");
            foreach (Card card in Cards)
                Console.WriteLine("{0} {1}",
                card.Value.ToString(), card.Suit.ToString());

            Console.WriteLine("\n统计量：");
            PrintList(Single);
            PrintList(Double);
            PrintList(Triple);
            PrintList(Quard);

            Console.WriteLine("\n牌型");
            Console.WriteLine(Category.ToString());
            PrintList(getList());
            Console.WriteLine("\n====================");
        }
        public Pack(List<int> _card)
        {
            // 将输入的字符串序列转化为Card序列并排序
            Cards = new List<Card>();
            _card.ForEach(id =>
            {
                Cards.Add(new Card(id));
            });
            Cards.Sort();
            Cards.Reverse();

            // 统计单、对、三的数量，后续判断牌型使用
            Cards.ForEach(card =>
            {
                if (map.ContainsKey(card.getPriority())) map[card.getPriority()]++;
                else map.Add(card.getPriority(), 1);
            });
            Single = (from i in map
                          where i.Value == 1
                          select i.Key).ToList();
            Double = (from i in map
                          where i.Value == 2
                          select i.Key).ToList();
            Triple = (from i in map
                          where i.Value == 3
                          select i.Key).ToList();
            Quard = (from i in map
                         where i.Value == 4
                         select i.Key).ToList();

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
                    if (Quard.Count == 1)
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
        }
        static private bool Continuous(List<int> list)
        {
            if (list[0] == 13) return false;
            for (int i = 1; i < list.Count; i++)
                if (list[i] != list[i - 1] - 1) return false;
            return true;
        }
        public List<int> getList()
        {
            List<int> list = new List<int>();
            foreach (Card card in Cards)
                list.Add(card.getId());
            return list;
        }
        public static bool operator >(Pack pack1, Pack pack2)
        {
            if(pack1 == null || pack2 == null) return false;
            if(pack1.Category != pack2.Category) return false;
            if(pack1.Subtype != pack2.Subtype) return false;
            return pack1.MaxValue > pack2.MaxValue;
        }
        public static bool operator <(Pack pack1, Pack pack2)
        {
            if (pack1 == null || pack2 == null) return false;
            if (pack1.Category != pack2.Category) return false;
            if (pack1.Subtype != pack2.Subtype) return false;
            return pack1.MaxValue < pack2.MaxValue;
        }
        private int __ans = 0;
        private int san1()
        {
            int ans = 0;
            bool canRocket = map.ContainsKey(14) && map[14] == 1 && map.ContainsKey(15) && map[15] == 1;
            int[] count = new int[5];
            foreach (var i in map)
            {
                count[i.Value]++;
            }
            // 四带两对
            while (count[4] > 0 && count[2] > 1)
            {
                count[4]--;
                count[2] -= 2;
                ans++;
            }
            // 四带二（两张一样）
            while (count[4] > 0 && count[2] > 0)
            {
                count[4]--;
                count[2]--;
                ans++;
            }
            // 四带一（两张不一样）
            while (count[4] > 0 && count[1] > 1)
            {
                count[4]--;
                count[1] -= 2;
                ans++;
            }
            // 三带一对
            while (count[3] > 0 && count[2] > 0)
            {
                count[3]--;
                count[2]--;
                ans++;
            }
            // 三带一
            while (count[3] > 0 && count[1] > 0)
            {
                count[3]--;
                count[1]--;
                ans++;
            }
            // 三带一+三带一对
            while (count[3] > 2)
            {
                count[3] -= 3;
                ans += 2;
            }
            // 四带两对
            while (count[4] > 1)
            {
                count[4] -= 2;
                ans++;
            }

            if (canRocket && count[1] > 1) {
                return ans + count[1] + count[2] + count[3] + count[4] - 1;
            } else
            {
                return ans + count[1] + count[2] + count[3] + count[4];
             }
        }
        private void feiji(int step)
        {
            for (int l = 1; l <= 12; l++)
            {
                int len = 0;
                while (l + len <= 12 && map.ContainsKey(l + len) && map[l + len] >= 3) len++;
                for (int i = len; i >= 2; i--)
                {
                    int r = l + i - 1;
                    for (int k = l; k <= r; k++) map[k] -= 3;
                    chupai(step + 1);
                    for (int k = l; k <= r; k++) map[k] += 3;
                }
            }
        }
        private void liandui(int step)
        {
            for (int l = 1; l <= 12; l++)
            {
                int len = 0;
                while (l + len <= 12 && map.ContainsKey(l + len) && map[l + len] >= 2) len++;
                for (int i = len; i >= 3; i--)
                {
                    int r = l + i - 1;
                    for (int k = l; k <= r; k++) map[k] -= 2;
                    chupai(step + 1);
                    for (int k = l; k <= r; k++) map[k] += 2;
                }
            }
        }
        private void shunzi(int step)
        {
            for (int l = 1; l <= 12; l++)
            {
                int len = 0;
                while (l + len <= 12 && map.ContainsKey(l + len) && map[l + len] >= 1) len++;
                for (int i = len; i >= 5; i--)
                {
                    int r = l + i - 1;
                    for (int k = l; k <= r; k++) map[k] -= 1;
                    chupai(step + 1);
                    for (int k = l; k <= r; k++) map[k] += 1;
                }
            }
        }
        private void chupai(int step)
        {
            if (step >= __ans) return;
            int san = san1();
            __ans=Math.Min(__ans, step+san);
            feiji(step);
            liandui(step);
            shunzi(step);
        }
        public int MinCase1()
        {
            __ans = 0x3f3f3f3f;
            chupai(0);
            return __ans;
        }
    }

}