using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;


namespace MineSweeper
{
    public partial class Game : Microsoft.Phone.Controls.PhoneApplicationPage
    {
        int gridSize;
        Boolean gameOn=false;
        Boolean flagOn=false;
        Random rand;
        Button[,] buttons;
        Boolean[,] opened,flagged;
        int numOfMines,numOfOpened=0;
        Button[] mineButtons;
        int minutes = 0, seconds = 0;
        public Game()
        {
            InitializeComponent();
            rand = new Random();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            String value = "";
            NavigationContext.QueryString.TryGetValue("size",out value);
            this.gridSize = int.Parse(value);

            numOfMines = gridSize / 2 + (int)(gridSize * rand.NextDouble());
            mineButtons = new Button[numOfMines];
            buttons = new Button[gridSize, gridSize];
            opened = new Boolean[gridSize, gridSize];
            flagged = new Boolean[gridSize, gridSize];
            generateButtons();
            deployMines();
            fillBoard();
            
            Thread th=new Thread(new ThreadStart(this.manageTimer));
            th.Start();
        }
        private void generateButtons()
        {
            for (int i = 0; i < gridSize; i++)
            {
                gridPanel.RowDefinitions.Add(new RowDefinition());
                gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < gridSize; j++)
                {
                    buttons[i, j] = new Button()
                    {
                        Content = "0",
                        FontSize = 18,
                        FontWeight=FontWeights.ExtraBold,
                        Foreground = new SolidColorBrush(Colors.Transparent),
                        Background = new SolidColorBrush(Color.FromArgb(0xff,0x0c,0x1b,0x27)),
                        Margin = new Thickness(-12)
                    };
                    buttons[i, j].SetValue(Grid.RowProperty,i);
                    buttons[i, j].SetValue(Grid.ColumnProperty, j);
                    buttons[i, j].Click += new RoutedEventHandler(cell_Click);
                    opened[i, j] = false;
                    this.gridPanel.Children.Add(buttons[i, j]);
                }
            }
        }

        private void cell_Click(object sender, RoutedEventArgs e)
        {
            Button current=(Button)sender;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (buttons[i, j].Equals(current))
                    {
                        if (flagOn) 
                        {
                            ImageBrush background = new ImageBrush();
                            background.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("flagImage.jpg", UriKind.Relative));
                            if (flagged[i, j]) { current.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x0c, 0x1b, 0x27)); flagged[i, j] = false; }
                            else { current.Background = background; flagged[i, j] = true; }
                            return;
                        } 
                        else if (buttons[i, j].Content.Equals("*"))
                        {
                            overGame();
                            return;
                        }
                        revealCells(i, j);
                        return; 
                    }
                }
            }
        }
        private void revealCells(int i,int j)
        {
                if(!gameOn)
                    gameOn = true;
                
                    String s = (String)(buttons[i, j].Content);
                    buttons[i, j].Foreground = new SolidColorBrush(Colors.Black);
                    buttons[i, j].Background = new SolidColorBrush(Color.FromArgb(0xff, 0x4b, 0xa0, 0xe6));
                    opened[i, j] = true;
                    numOfOpened++;
                    buttons[i, j].IsHitTestVisible = false;
                    if (numOfOpened == (gridSize * gridSize) - numOfMines)
                        winGame();
                    if (s.Equals("0"))
                    {
                        buttons[i, j].Foreground = new SolidColorBrush(Colors.Transparent);
                        if ((i >= 0 && i < gridSize) && (j >= 0 && j < gridSize))
                        {
                            if (((i - 1) >= 0) && (!opened[i - 1, j]))
                                revealCells(i - 1, j);
                            if (((i + 1) <= gridSize - 1) && (!opened[i + 1, j]))
                                revealCells(i + 1, j);
                            if (((j - 1) >= 0) && (!opened[i, j - 1]))
                                revealCells(i, j - 1);
                            if (((j + 1) <= gridSize - 1) && (!opened[i, j + 1]))
                                revealCells(i, j + 1);
                        }
                    }
        }
        private void overGame()
        {
            gameOn = false;
            foreach (Button b in mineButtons)
            {
                b.Background = new SolidColorBrush(Colors.Red);
                b.Foreground = new SolidColorBrush(Colors.Black);
            }
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    buttons[i, j].IsHitTestVisible = false;
                }
            }

            MessageBox.Show("Blasted ! Game Over.");
            backButton.Content = "Lost!!! (Back to Menu)";
        }
        private void winGame()
        {
            gameOn = false;
            foreach (Button b in mineButtons)
            {
                b.Background = new SolidColorBrush(Colors.Green);
                b.Foreground = new SolidColorBrush(Colors.Black);
                b.IsHitTestVisible = false;
            }
            MessageBox.Show("Congrats ! you won in "+(minutes*60+seconds)+" seconds");
            backButton.Content = "Won!!! (Back For a new game)";
        }

        private void deployMines()
        {
            for (int i = 0; i < numOfMines; )
            {
                int x = (int)(gridSize * rand.NextDouble());
                int y = (int)(gridSize * rand.NextDouble());
                String s = (String)buttons[x, y].Content;
                if (!(s.Equals("*")))
                {
                    buttons[x, y].Content = "*";
                    mineButtons[i] = buttons[x, y];
                    i++;
                }
            }
        }
        private void fillBoard()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    String s = (String)buttons[i, j].Content;
                    if (!(s.Equals("*")))
                    {
                        for (int k = i - 1; k <= i + 1; k++)
                        {
                            for (int l = j - 1; l <= j + 1; l++)
                            {
                                if ((k >= 0 && k < gridSize) && (l >= 0 && l < gridSize))
                                {
                                    String s1 = (String)buttons[k, l].Content;
                                    if (s1.Equals("*"))
                                        buttons[i, j].Content = "" + (int.Parse((String)buttons[i, j].Content) + 1);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void manageTimer()
        {
            Boolean inGame = false ;
            while (true)
            {
                while (gameOn)
                {
                    inGame = true;
                    seconds++;
                    if (seconds == 60) { minutes++; seconds = 0; }
                    Deployment.Current.Dispatcher.BeginInvoke(() => { timerButton.Content = "" + minutes + ":" + seconds; });
                    Thread.Sleep(1000);
                }
                if (inGame) break;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            flagOn = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            flagOn = false;
        }
    }
}
