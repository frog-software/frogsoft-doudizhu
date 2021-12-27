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
using WebSocketSharp;
using Newtonsoft.Json;
using client.Models;

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

            WebSocketInitialize();
        }

        private const int NO_BUTTON = 0;        // 不显示按钮
        private const int BUTTON_ON_PLAY = 1;   // 打牌时的按钮
        private const int BUTTON_ON_CALL = 2;   // 叫分时的按钮

        private const int CARD_DESELECT_MARGIN = -80;   // 不被选中的牌
        private const int CARD_SELECT_MARGIN = -20;     // 被选中的牌

        private List<int> lordCardList = new List<int> { 54, 54, 54 };  // 地主牌

        private List<int> ownCardList = new List<int> { }; // 自己手上的牌

        private List<int> leftPutCardList = new List<int> { };     // 上家出的牌
        private List<int> rightPutCardList = new List<int> { };    // 下家出的牌

        private List<int> selectCardList = new List<int> { };       // 已选中的牌
        private List<int> ownPutCardList = new List<int> { };       // 打出去的牌

        private List<string> playerText = new List<string> { "", "不叫", "一分", "两分", "三分", "不出" };

        private WebSocket ws = new WebSocket("ws://localhost:5174/api/games/com/frogsoft/doudizhu/room");

        private PlayerModel currentPlayer = new PlayerModel();  // 存储固定的本地个人信息
        private GameModel currentGame = new GameModel();        // 存储本场游戏的信息

        private bool isAuto = false;

        private void LordCardPanel_Upgrade() // 更新地主牌动画
        {
            OutCardPanel_Upgrade(lordCardPanel, lordCardList, 0);
        }

        private void OwnPutCardPanel_Upgrade() // 更新自己的出牌堆动画
        {
            OutCardPanel_Upgrade(ownPutCardPanel, ownPutCardList, 1);
        }

        private void LeftPutCardPanel_Upgrade() // 更新上家的出牌堆动画
        {
            OutCardPanel_Upgrade(leftPutCardPanel, leftPutCardList, 2);
        }

        private void RightPutCardPanel_Upgrade() // 更新下家的出牌堆动画
        {
            OutCardPanel_Upgrade(rightPutCardPanel, rightPutCardList, 3);
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
                image.Margin = new Thickness { Left = sideCase > 0 ? -46 : 2, Bottom = 0 };

                cardPanel.Children.Add(image);
            }

            if (cardList.Count > 0)
            {
                switch (sideCase)
                {
                    case 0:
                        (cardPanel.Children[0] as Image).Margin = new Thickness { Left = (window.Width - cardList.Count * 70) / 2 };
                        break;
                    case 1:
                        (cardPanel.Children[0] as Image).Margin = new Thickness { Left = (window.Width - (cardList.Count - 1) * 24 - 70) / 2 };
                        break;
                    case 2:
                        (cardPanel.Children[0] as Image).Margin = new Thickness { Left = 240 };
                        break;
                    case 3:
                        (cardPanel.Children[0] as Image).Margin = new Thickness { Left = 590 - (cardList.Count - 1) * 24 - 70 };
                        break;
                }
            }
        }

        private void OwnCardPanel_Upgrade() // 更新剩余手牌堆动画
        {
            ownCardPanel.Children.Clear();
            foreach (int card in ownCardList)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("/images/cards/" + card.ToString() + ".png", UriKind.Relative));

                image.Name = "card" + card.ToString();
                image.Width = 105;
                image.Height = 140;
                image.Margin = new Thickness { Left = -70, Bottom = CARD_DESELECT_MARGIN };
                image.MouseLeftButtonUp += CardImage_Click;

                ownCardPanel.Children.Add(image);
            }

            if (ownCardList.Count > 0)
                (ownCardPanel.Children[0] as Image).Margin = new Thickness { Left = (window.Width - (ownCardList.Count - 1) * 35 - 105) / 2, Bottom = CARD_DESELECT_MARGIN };
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

        private void ButtonPanel_Upgrade(int buttonType) // 牌区按钮选择
        {
            buttonPanelOnPlay.Visibility = Visibility.Collapsed;
            buttonPanelOnCall.Visibility = Visibility.Collapsed;

            switch (buttonType)
            {
                case NO_BUTTON:
                    break;
                case BUTTON_ON_CALL:
                    buttonPanelOnCall.Visibility = Visibility.Visible;
                    break;
                case BUTTON_ON_PLAY:
                    buttonPanelOnPlay.Visibility = Visibility.Visible;
                    autoPlayButton.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ReselectButton_Click(object sender, RoutedEventArgs e) // 重选
        {
            selectCardList.Clear();
            foreach (var i in ownCardPanel.Children)
            {
                Image image = i as Image;
                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = CARD_DESELECT_MARGIN };
            }
        }

        private void SkipCardButton_Click(object sender, RoutedEventArgs e) // 不出
        {
            selectCardList.Clear();
            foreach (var i in ownCardPanel.Children)
            {
                Image image = i as Image;
                image.Margin = new Thickness { Left = image.Margin.Left, Bottom = CARD_DESELECT_MARGIN };
            }

            currentGame.MessageType = MessageType.UPDATE;
            currentGame.GetPlayerById(currentPlayer.Id).CardsOut = selectCardList;
            ws.Send(JsonConvert.SerializeObject(currentGame));
        }

        private void PutCardButton_Click(object sender, RoutedEventArgs e) // 出牌
        {
            if (selectCardList.Count > 0) // 有选择牌
            {
                var selected = new Pack(selectCardList);
                var lastOut = new Pack(currentGame.LastCombination);

                if (skipCardButton.Visibility == Visibility.Hidden && selected.Category != Category.UNDEFINED || lastOut.Count > 0 && selected > lastOut) // 允许出牌
                {
                    currentGame.MessageType = MessageType.UPDATE;
                    currentGame.GetPlayerById(currentPlayer.Id).CardsOut = selectCardList;
                    ws.Send(JsonConvert.SerializeObject(currentGame));

                    selectCardList.Clear();
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

        private void LogoGrid_Upgrade() // Logo更新动画
        {

        }

        private void StartGame_Click(object sender, RoutedEventArgs e) // 开始匹配
        {
            ws.Connect();
        }

        private void QuitGame_Click(object sender, RoutedEventArgs e) // 退出游戏
        {
            window.Close();
        }

        private void CallNo_Click(object sender, RoutedEventArgs e) // 不叫
        {
            ownCallTextBlock.Text = "不叫";
            ButtonPanel_Upgrade(NO_BUTTON);

            var __currentPlayer = currentGame.GetPlayerById(currentPlayer.Id);
            __currentPlayer.CallScore = 0;
            currentGame.MessageType = MessageType.UPDATE;
            ws.Send(JsonConvert.SerializeObject(currentGame));
        }

        private void CallOne_Click(object sender, RoutedEventArgs e) // 一分
        {
            ownCallTextBlock.Text = "一分";
            ButtonPanel_Upgrade(NO_BUTTON);

            var __currentPlayer = currentGame.GetPlayerById(currentPlayer.Id);
            __currentPlayer.CallScore = 1;
            currentGame.MessageType = MessageType.UPDATE;
            ws.Send(JsonConvert.SerializeObject(currentGame));
        }

        private void CallTwo_Click(object sender, RoutedEventArgs e) // 两分
        {
            ownCallTextBlock.Text = "两分";
            ButtonPanel_Upgrade(NO_BUTTON);

            var __currentPlayer = currentGame.GetPlayerById(currentPlayer.Id);
            __currentPlayer.CallScore = 2;
            currentGame.MessageType = MessageType.UPDATE;
            ws.Send(JsonConvert.SerializeObject(currentGame));
        }

        private void CallThree_Click(object sender, RoutedEventArgs e) // 三分
        {
            ownCallTextBlock.Text = "三分";
            ButtonPanel_Upgrade(BUTTON_ON_PLAY);
            Call_Clear();

            lordCardList.Clear();

            var __currentPlayer = currentGame.GetPlayerById(currentPlayer.Id);
            __currentPlayer.CallScore = 3;
            currentGame.MessageType = MessageType.UPDATE;
            ws.Send(JsonConvert.SerializeObject(currentGame));

            LordCardPanel_Upgrade();
        }

        private void Call_Clear() // 清除场上的叫分情况
        {
            ownCallTextBlock.Text = string.Empty;
            leftCallTextBlock.Text = string.Empty;
            rightCallTextBlock.Text = string.Empty;
        }

        private void WebSocketInitialize() // WebSocket初始化
        {
            ws.OnOpen += (sender, e) =>
            {
                Random random = new Random();
                currentPlayer.Id = "user" + random.Next(1000).ToString();
                currentGame.CurrentPlayer = currentPlayer.Id;
                currentGame.RoomNo = "1";
                currentGame.MessageType = MessageType.JOIN;

                ws.Send(JsonConvert.SerializeObject(currentGame));

                mainGrid.Visibility = Visibility.Collapsed;
            };

            ws.OnMessage += (sender, e) =>
            {
                // 本场游戏
                currentGame = JsonConvert.DeserializeObject<GameModel>(e.Data);
                // 动态的个人信息
                var myself = currentGame.GetPlayerById(currentPlayer.Id);

                if (currentGame.list.Count == 0) // 进入房间
                {
                    gameGrid.Dispatcher.Invoke(() =>
                    {
                        gameGrid.Visibility = Visibility.Visible;
                    });

                    if (myself.Status != PlayerStatus.READY)
                    {
                        myself.Status = PlayerStatus.READY;
                        currentGame.MessageType = MessageType.UPDATE;
                        currentGame.CurrentPlayer = currentPlayer.Id;
                        ws.Send(JsonConvert.SerializeObject(currentGame));
                    }
                }
                else // 开始游戏
                {
                    ownCardList = new Pack(myself.CardsInHand).getList();

                    var leftPlayer = currentGame.GetNextPlayerById(currentGame.GetNextPlayerById(currentPlayer.Id).Id);
                    var rightPlayer = currentGame.GetNextPlayerById(currentPlayer.Id);

                    leftPutCardList = leftPlayer.CardsOut;
                    rightPutCardList = rightPlayer.CardsOut;
                    ownPutCardList = new Pack(myself.CardsOut).getList();

                    gameGrid.Dispatcher.Invoke(async () =>
                    {
                        skipCardButton.Visibility = Visibility.Visible;

                        // 还未分出地主，并且到自己时
                        if (currentGame.CurrentPlayer == myself.Id && !(myself.Status == PlayerStatus.LANDLORD || myself.Status == PlayerStatus.PEASANT))
                            ButtonPanel_Upgrade(BUTTON_ON_CALL);
                        // 已分出地主，并且到自己时
                        else if (currentGame.CurrentPlayer == myself.Id && (myself.Status == PlayerStatus.LANDLORD || myself.Status == PlayerStatus.PEASANT))
                        {
                            ButtonPanel_Upgrade(BUTTON_ON_PLAY);

                            if (leftPlayer.CardsOut.Count == 0 && rightPlayer.CardsOut.Count == 0)
                                skipCardButton.Visibility = Visibility.Hidden;
                            else
                                skipCardButton.Visibility = Visibility.Visible;

                            // 托管
                            if (isAuto)
                            {
                                await System.Threading.Tasks.Task.Delay(2000);

                                if (skipCardButton.Visibility == Visibility.Hidden)
                                {
                                    var pack = new Pack(ownCardList);
                                    pack.MinCase1();
                                    selectCardList = pack.getAnsShouldOut();
                                }
                                else
                                {
                                    selectCardList = new Pack(ownCardList).NextPack(new Pack(currentGame.LastCombination));
                                }

                                currentGame.MessageType = MessageType.UPDATE;
                                currentGame.GetPlayerById(currentPlayer.Id).CardsOut = selectCardList;
                                ws.Send(JsonConvert.SerializeObject(currentGame));

                                selectCardList.Clear();
                            }
                        }
                        // 无论分没分出地主，并且没到自己时
                        else
                            ButtonPanel_Upgrade(NO_BUTTON);
                            

                        // 已分出地主，揭开地主牌
                        if (myself.Status == PlayerStatus.LANDLORD || myself.Status == PlayerStatus.PEASANT)
                        {
                            lordCardList = currentGame.list.GetRange(51, 3);
                            Call_Clear();
                        }
                        // 还没分出地主，不揭开地主牌
                        else
                        {
                            lordCardList.Clear();
                            for (int i = 1; i <= 3; i++)
                                lordCardList.Add(54);

                            leftCallTextBlock.Text = playerText[leftPlayer.CallScore + 1];
                            ownCallTextBlock.Text = playerText[myself.CallScore + 1];
                            rightCallTextBlock.Text = playerText[rightPlayer.CallScore + 1];
                        }

                        

                        LordCardPanel_Upgrade();
                        LeftPutCardPanel_Upgrade();
                        RightPutCardPanel_Upgrade();
                        OwnPutCardPanel_Upgrade();
                        OwnCardPanel_Upgrade();
                    });
                }
            };

            ws.OnError += (sender, e) =>
            {
                MessageBox.Show(e.Message);
            };
        }

        private async void AutoPlayButton_Click(object sender, RoutedEventArgs e)
        {
            isAuto = !isAuto;

            // 托管状态
            if (isAuto)
            {
                autoPlayButton.Content = "取消托管";

                await System.Threading.Tasks.Task.Delay(2000);

                // 如果必须到自己出牌（不出按钮隐藏的情况下）
                if (skipCardButton.Visibility == Visibility.Hidden)
                {
                    var pack = new Pack(ownCardList);
                    pack.MinCase1();
                    selectCardList = pack.getAnsShouldOut();
                }
                // 其他出牌（可以选择不出）
                else
                {
                    selectCardList = new Pack(ownCardList).NextPack(new Pack(currentGame.LastCombination));
                }

                currentGame.MessageType = MessageType.UPDATE;
                currentGame.GetPlayerById(currentPlayer.Id).CardsOut = selectCardList;
                ws.Send(JsonConvert.SerializeObject(currentGame));

                selectCardList.Clear();
            }
            // 非托管状态
            else
                autoPlayButton.Content = "托管";
        }
    }
}