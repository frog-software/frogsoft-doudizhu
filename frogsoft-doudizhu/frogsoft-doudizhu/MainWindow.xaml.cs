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
            LeftCardPanel_Upgrade();
            PutCardPanel_Upgrade();
            LeftPutCardPanel_Upgrade();
            RightPutCardPanel_Upgrade();
            ButtonPanel_Upgrade(BUTTON_ON_PLAY);
        }

        private const int BUTTON_ON_PLAY = 0;
        private const int BUTTON_ON_CALL = 1;

        private const int CARD_DESELECT_MARGIN = -60;
        private const int CARD_SELECT_MARGIN = 0;

        public List<int> leftCardList = new List<int> { 1, 2, 3 };

        public List<int> leftPutCardList = new List<int> { 1 };
        public List<int> rightPutCardList = new List<int> { 9, 13, 17, 21, 25, 29 };
        public List<int> putCardList = new List<int> { 1, 1, 1, 1, 1, 1, 1, 1 };


        private void PutCardPanel_Upgrade() // 更新自己的出牌堆动画
        {
            OutCardPanel_Upgrade(putCardPanel, putCardList, 1);
        }

        private void LeftPutCardPanel_Upgrade() // 更新上家的出牌堆动画
        {
            OutCardPanel_Upgrade(leftPutCardPanel, leftPutCardList, 2);
        }

        private void RightPutCardPanel_Upgrade() // 更新下家的出牌堆动画
        {
            OutCardPanel_Upgrade(rightPutCardPanel, rightPutCardList, 3);
        }

        private void OutCardPanel_Upgrade(StackPanel cardPanel, List<int> cardList, int sideCase)
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
            {
                switch (sideCase)
                {
                    case 1:
                        (cardPanel.Children[0] as Image).Margin = new Thickness { Left = (window.Width - (cardList.Count - 1) * 24 - 70) / 2 };
                        break;
                    case 2:
                        (cardPanel.Children[0] as Image).Margin = new Thickness { Left = 200 };
                        break;
                    case 3:
                        (cardPanel.Children[0] as Image).Margin = new Thickness { Left = 550 - (cardList.Count - 1) * 24 - 70 };
                        break;
                }
            }
        }

        private void LeftCardPanel_Upgrade() // 更新剩余手牌堆动画
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
                image.MouseLeftButtonUp += CardImage_Click;

                leftCardPanel.Children.Add(image);
            }

            if (leftCardList.Count > 0)
                (leftCardPanel.Children[0] as Image).Margin = new Thickness { Left = (window.Width - (leftCardList.Count - 1) * 35 - 105) / 2, Bottom = CARD_DESELECT_MARGIN };
        }

        private void CardImage_Click(object sender, RoutedEventArgs e) // 点击牌时的动画
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

        private void ButtonPanel_Upgrade(int buttonType)
        {
            buttonPanel.Children.Clear();

            switch (buttonType)
            {
                case BUTTON_ON_CALL:
                    break;
                case BUTTON_ON_PLAY:
                    Button reselectButton = new Button();
                    reselectButton.Content = "重选";
                    reselectButton.Width = 120;
                    reselectButton.Height = 50;
                    reselectButton.Click += ReselectButton_Click;
                    reselectButton.Margin = new Thickness { Left = 420 - 100, Top = 25 };
                    buttonPanel.Children.Add(reselectButton);

                    Button skipCardButton = new Button();
                    skipCardButton.Content = "不出";
                    skipCardButton.Width = 120;
                    skipCardButton.Height = 50;
                    skipCardButton.Click += SkipCardButton_Click;
                    skipCardButton.Margin = new Thickness { Left = 100, Top = 25 };
                    buttonPanel.Children.Add(skipCardButton);

                    Button putCardButton = new Button();
                    putCardButton.Content = "出牌";
                    putCardButton.Width = 120;
                    putCardButton.Height = 50;
                    putCardButton.Click += PutCardButton_Click;
                    putCardButton.Margin = new Thickness { Left = 100, Top = 25 };
                    buttonPanel.Children.Add(putCardButton);
                    break;
            }
        }

        private void ReselectButton_Click(object sender, RoutedEventArgs e) // 重选
        {
            foreach (var i in leftCardPanel.Children)
            {
                Image image = i as Image;
                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = CARD_DESELECT_MARGIN };
            }
        }

        private void SkipCardButton_Click(object sender, RoutedEventArgs e) // 不出
        {
            
        }

        private void PutCardButton_Click(object sender, RoutedEventArgs e) // 出牌
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

            PutCardPanel_Upgrade();
            LeftCardPanel_Upgrade();
        }
    }
}
