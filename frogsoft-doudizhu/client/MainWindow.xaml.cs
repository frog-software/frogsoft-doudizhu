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
using System.Threading;

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
            ButtonPanel_Upgrade(BUTTON_ON_PLAY);
        }

        private const int BUTTON_ON_PLAY = 0;   // 打牌时的按钮
        private const int BUTTON_ON_CALL = 1;   // 叫分时的按钮

        private const int CARD_DESELECT_MARGIN = -60;   // 不被选中的牌
        private const int CARD_SELECT_MARGIN = 0;       // 被选中的牌

        // private List<int> ready = new List<int> { 9, 13, 17, 21, 25, 29 };
        private List<int> leftCardList = new List<int> { 9, 13, 17, 21, 25, 29 }; // 自己手上的牌

        private List<int> leftPutCardList = new List<int> { 1 };    // 上家出的牌
        private List<int> rightPutCardList = new List<int> { };     // 下家出的牌

        private List<int> selectCardList = new List<int> { };       // 已选中的牌
        private List<int> putCardList = new List<int> { };          // 打出去的牌

        // private int test = 0;

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

        private void AllCardPanel_Clear() // 清除牌桌上的牌
        {
            putCardPanel.Children.Clear();
            rightPutCardPanel.Children.Clear();
            leftPutCardPanel.Children.Clear();
        }

        private void OutCardPanel_Upgrade(StackPanel cardPanel, List<int> cardList, int sideCase) // 更新牌堆中的动画
        {
            cardPanel.Children.Clear();
            foreach (int card in cardList)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("/images/cards/" + card.ToString() + ".png", UriKind.Relative));
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
                image.Source = new BitmapImage(new Uri("/images/cards/" + card.ToString() + ".png", UriKind.Relative));

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

        private void CardImage_Click(object sender, RoutedEventArgs e) // 牌被点击时
        {
            Image image = sender as Image;
            if (image.Margin.Bottom == CARD_DESELECT_MARGIN)
            {
                selectCardList.Add(Convert.ToInt32(image.Name[4..]));
                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = CARD_SELECT_MARGIN };
            }
            else
            {
                for (var i = 0; i < selectCardList.Count; i++)
                {
                    if (selectCardList[i] == Convert.ToInt32(image.Name[4..]))
                    {
                        selectCardList.RemoveAt(i);
                        break;
                    }
                }

                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = CARD_DESELECT_MARGIN };
            }
        }

        private void ButtonPanel_Upgrade(int buttonType) // 添加游戏按钮
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
            selectCardList.Clear();
            foreach (var i in leftCardPanel.Children)
            {
                Image image = i as Image;
                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = CARD_DESELECT_MARGIN };
            }
        }

        private void SkipCardButton_Click(object sender, RoutedEventArgs e) // 不出
        {
            selectCardList.Clear();
            foreach (var i in leftCardPanel.Children)
            {
                Image image = i as Image;
                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = CARD_DESELECT_MARGIN };
            }

            /*if (test < ready.Count)
            {
                leftCardList.Add(ready[test]);
                LeftCardPanel_Upgrade();
                test++;
            }*/
        }

        private void PutCardButton_Click(object sender, RoutedEventArgs e) // 出牌
        {
            if (selectCardList.Count > 0) // 有选择牌
            {
                putCardList.Clear();

                // 此处对selectCardList做一个排序

                foreach (var card in selectCardList)
                    putCardList.Add(card);

                if (true) // 允许出牌
                {
                    foreach (var i in leftCardPanel.Children)
                    {
                        Image image = i as Image;

                        if (image.Margin.Bottom == CARD_SELECT_MARGIN)
                        {
                            for (var j = 0; j < leftCardList.Count; j++)
                            {
                                if (leftCardList[j] == Convert.ToInt32(image.Name[4..]))
                                {
                                    leftCardList.RemoveAt(j);
                                    j--;
                                }
                            }
                        }
                    }

                    selectCardList.Clear();
                    PutCardPanel_Upgrade();
                    LeftCardPanel_Upgrade();
                }
                else // 不允许出牌
                {
                    MessageBox.Show("不能出");
                }

            }
            else // 未选牌错误处理
            {
                MessageBox.Show("请选择牌！");
            }
        }
    }
}