using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace frogsoft_doudizhu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            leftCardPanel_Upgrade();
            putCardPanel_Upgrade();
            leftPutCardPanel_Upgrade();
            rightPutCardPanel_Upgrade();
        }

        public List<int> leftCardList = new List<int> { 1, 2, 3 };

        public List<int> leftPutCardList = new List<int> { 7, 8, 9 };
        public List<int> rightPutCardList = new List<int> { 54, 53 };
        public List<int> putCardList = new List<int> { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };


        private void putCardPanel_Upgrade() // 更新自己的出牌堆动画
        {
            outCardPanel_Upgrade(putCardPanel, putCardList);
        }

        private void leftPutCardPanel_Upgrade() // 更新上家的出牌堆动画
        {
            outCardPanel_Upgrade(leftPutCardPanel, leftPutCardList);
        }

        private void rightPutCardPanel_Upgrade() // 更新下家的出牌堆动画
        {
            outCardPanel_Upgrade(rightPutCardPanel, rightPutCardList);
        }

        private void outCardPanel_Upgrade(StackPanel cardsPanel, List<int> cardsList) // 更新出牌堆里的动画
        {
            cardsPanel.Children.Clear();
            foreach (int card in cardsList)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("/image/" + card.ToString() + ".png", UriKind.Relative));
                image.Name = "card" + card.ToString();
                image.Width = 70;
                image.Height = 105;
                image.Margin = new Thickness { Left = -46, Bottom = 0 };

                cardsPanel.Children.Add(image);
            }

            (cardsPanel.Children[0] as Image).Margin = new Thickness { Left = (window.Width / 2 - (cardsList.Count - 1) * 46 - 70) / 2 };
        }

        private void leftCardPanel_Upgrade() // 更新剩余手牌堆动画
        {
            leftCardPanel.Children.Clear();
            foreach (int card in leftCardList)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("/image/" + card.ToString() + ".png", UriKind.Relative));

                image.Name = "card" + card.ToString();
                image.Width = 105;
                image.Height = 140;
                image.Margin = new Thickness { Left = -70, Bottom = 0 };
                image.MouseLeftButtonUp += cardImage_Click;

                leftCardPanel.Children.Add(image);
            }

            (leftCardPanel.Children[0] as Image).Margin = new Thickness { Left = (window.Width - (leftCardList.Count - 1) * 70 - 105) / 2 };
        }

        private void cardImage_Click(object sender, RoutedEventArgs e) // 点击牌时的动画
        {
            Image image = sender as Image;
            if (image.Margin.Bottom == 0)
            {
                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = 60 };
            }
            else
            {
                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = 0 };
            }
        }

        private void putCard_Click(object sender, RoutedEventArgs e) // 出牌
        {
            putCardList.Clear();
            foreach (var i in leftCardPanel.Children)
            {
                Image image = i as Image;
                if (image == null) continue;

                if (image.Margin.Bottom != 0)
                {
                    putCardList.Add(Convert.ToInt32(image.Name[4..]));

                    for (var j = 0; j < leftCardList.Count; j++)
                        if (leftCardList[j] == Convert.ToInt32(image.Name[4..]))
                            leftCardList.RemoveAt(j);
                }
            }

            putCardPanel_Upgrade();
            leftCardPanel_Upgrade();
        }

        private void reselect_Click(object sender, RoutedEventArgs e) // 重选
        {
            foreach (var i in leftCardPanel.Children)
            {
                Image image = i as Image;

                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = 0 };
            }
        }
    }
}
