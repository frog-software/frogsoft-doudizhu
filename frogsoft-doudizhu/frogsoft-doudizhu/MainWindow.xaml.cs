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

        private const int CARD_DESELECT_MARGIN = -60;
        private const int CARD_SELECT_MARGIN = 0;

        public List<int> leftCardList = new List<int> { 1, 2, 3 };

        public List<int> leftPutCardList = new List<int> { 1 };
        public List<int> rightPutCardList = new List<int> { 1, 1, 1, 1, 1, 1 };
        public List<int> putCardList = new List<int> {  1, 1, 1, 1, 1, 1, 1, 1 };


        private void putCardPanel_Upgrade() // 更新自己的出牌堆动画
        {
            outCardPanel_Upgrade(putCardPanel, putCardList, 1);
        }

        private void leftPutCardPanel_Upgrade() // 更新上家的出牌堆动画
        {
            outCardPanel_Upgrade(leftPutCardPanel, leftPutCardList, 2);
        }

        private void rightPutCardPanel_Upgrade() // 更新下家的出牌堆动画
        {
            outCardPanel_Upgrade(rightPutCardPanel, rightPutCardList, 3);
        }

        private void outCardPanel_Upgrade(StackPanel cardPanel, List<int> cardList, int sideCase)
        {
            cardPanel.Children.Clear();
            foreach (int card in cardList)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("/image/" + card.ToString() + ".png", UriKind.Relative));
                image.Name = "card" + card.ToString();
                image.Width = 70;
                image.Height = 105;
                image.Margin = new Thickness { Left = -46, Bottom = 0 };

                cardPanel.Children.Add(image);
            }

            if (cardList.Count > 0)
                switch (sideCase)
            {
                case 1:
                    (cardPanel.Children[0] as Image).Margin = new Thickness { Left = (window.Width - (cardList.Count - 1) * 24 - 70) / 2 };
                    break;
                case 2:
                    (cardPanel.Children[0] as Image).Margin = new Thickness { Left = window.Width / 20 };
                    break;
                case 3:
                    (cardPanel.Children[0] as Image).Margin = new Thickness { Left = window.Width * 19 / 20 - (cardList.Count - 1) * 24 - 70 };
                    break;
            }
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
                image.Margin = new Thickness { Left = -70, Bottom = CARD_DESELECT_MARGIN };
                image.MouseLeftButtonUp += cardImage_Click;

                leftCardPanel.Children.Add(image);
            }

            if (leftCardList.Count > 0)
                (leftCardPanel.Children[0] as Image).Margin = new Thickness { Left = (window.Width - (leftCardList.Count - 1) * 35 - 105) / 2, Bottom = CARD_DESELECT_MARGIN };
        }

        private void cardImage_Click(object sender, RoutedEventArgs e) // 点击牌时的动画
        {
            Image image = sender as Image;
            if (image.Margin.Bottom == CARD_DESELECT_MARGIN)
            {
                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = CARD_SELECT_MARGIN };
            }
            else
            {
                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = CARD_DESELECT_MARGIN };
            }
        }

        private void putCard_Click(object sender, RoutedEventArgs e) // 出牌
        {
            putCardList.Clear();
            foreach (var i in leftCardPanel.Children)
            {
                Image image = i as Image;
                if (image == null) continue;

                if (image.Margin.Bottom == CARD_SELECT_MARGIN)
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
                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = CARD_DESELECT_MARGIN };
            }
        }
    }
}
