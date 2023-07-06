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

using Core;
using SciChartInterface;


/*
 * Main WPF Application
 * 
 */
namespace WPFApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /* ------------------------------------------------------------------*/
        // public delegates

        public delegate void ReloadFunction_del();


        /* ------------------------------------------------------------------*/
        // public functions

        public MainWindow()
        {
            InitializeComponent();

            //this.Loaded += OnLoaded;

            Core.Program core = new Core.Program();
            core.Main();

            SciChartInterface.MainWindow scichart = new SciChartInterface.MainWindow();
            scichart.Init();
            scichart.DeclareSciChartSurface(ref main_grid);


            CompositionTarget.Rendering += oncePerFrame; // Called once per frame


            this.Title = "Canvas Sample";
            /*
            Canvas myParentCanvas = new Canvas();
            myParentCanvas.Width = 400;
            myParentCanvas.Height = 400;
            Canvas.SetTop(myParentCanvas, 0);
            Canvas.SetLeft(myParentCanvas, 0);
            */
            System.Windows.Shapes.Rectangle myRect = new System.Windows.Shapes.Rectangle();
            myRect.Stroke = System.Windows.Media.Brushes.Black;
            myRect.Fill = System.Windows.Media.Brushes.SkyBlue;
            myRect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myRect.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            myRect.Height = 50;
            myRect.Width = 50;
            /*
            myParentCanvas.Children.Add(myRect
            grid1.Children.Add(myParentCanvas);
            */
            main_grid.Children.Add(myRect);


            ///System.Drawing 


            Grid DynamicGrid = new Grid();
            /*
            < Grid.ColumnDefinitions >
                < ColumnDefinition Width = "*" />
                < ColumnDefinition Width = "5" />
                < ColumnDefinition Width = "*" />
            </ Grid.ColumnDefinitions >
            < TextBlock FontSize = "55" HorizontalAlignment = "Center" VerticalAlignment = "Center" TextWrapping = "Wrap" > Left side </ TextBlock >
            < GridSplitter Grid.Column = "1" Width = "5" HorizontalAlignment = "Stretch" />
            < TextBlock Grid.Column = "2" FontSize = "55" HorizontalAlignment = "Center" VerticalAlignment = "Center" TextWrapping = "Wrap" > Right side </ TextBlock >
            */
            ColumnDefinition gridCol1 = new ColumnDefinition();
            gridCol1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition gridCol2 = new ColumnDefinition();
            gridCol2.Width = new GridLength(5);
            ColumnDefinition gridCol3 = new ColumnDefinition();
            gridCol3.Width = new GridLength(1, GridUnitType.Star); ;
            DynamicGrid.ColumnDefinitions.Add(gridCol1);
            DynamicGrid.ColumnDefinitions.Add(gridCol2);
            DynamicGrid.ColumnDefinitions.Add(gridCol3);

            TextBlock txtBlock1 = new TextBlock();
            txtBlock1.Text = "Left side";
            txtBlock1.FontSize = 55;
            txtBlock1.FontWeight = FontWeights.Bold;
            txtBlock1.TextWrapping = TextWrapping.Wrap;
            txtBlock1.HorizontalAlignment = HorizontalAlignment.Center;
            txtBlock1.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(txtBlock1, 0);
            DynamicGrid.Children.Add(txtBlock1);

            GridSplitter gridSplit1 = new GridSplitter();
            gridSplit1.Width = 5.0;
            gridSplit1.HorizontalAlignment = HorizontalAlignment.Stretch;
            gridSplit1.Background = Brushes.AliceBlue;
            Grid.SetColumn(gridSplit1, 1);
            DynamicGrid.Children.Add(gridSplit1);

            TextBlock txtBlock2 = new TextBlock();
            txtBlock2.Text = "Right side";
            txtBlock2.FontSize = 55;
            txtBlock2.FontWeight = FontWeights.Bold;
            txtBlock2.TextWrapping = TextWrapping.Wrap;
            txtBlock2.HorizontalAlignment = HorizontalAlignment.Center;
            txtBlock2.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(txtBlock2, 2);
            DynamicGrid.Children.Add(txtBlock2);

            // <Button x:Name="btn1" Content="Reload Plugin" Height="79" Width="212" Click="Button_Click" HorizontalAlignment="Left" VerticalAlignment="Top" Margin="88,52,0,0"/>
            Button btn = new Button();
            btn.Name = "btn1";
            btn.Content = "Reload Plugin";
            btn.Height = 79.0;
            btn.Width = 212.0;
            btn.Click += this.Button_Click;
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Top;
            btn.Margin = new Thickness(88.0, 52.0, 0.0, 0.0);
            Grid.SetColumn(btn, 2);
            DynamicGrid.Children.Add(btn);

            main_grid.Children.Add(DynamicGrid);


        }

        public void ReloadComponentFunction(ReloadFunction_del func)
        {
            this.reload_func = func;
        }


        /* ------------------------------------------------------------------*/
        // private functions

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // ...
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            // DEBUG
            Button btn = (Button)sender;
            btn.Content = "Reload Count:" + (this.clicks++).ToString();


            if (this.reload_func != null)
            {
                try
                {
                    this.reload_func();
                }
                catch (System.IO.FileNotFoundException)
                {
                    // runtimemessages.Add(MessageLevel.Error, "System.IO.FileNotFoundException Message = Die Datei oder Assembly "FlagBufferComponent ...");
                }
            }
        }

        private void oncePerFrame(object sender, EventArgs args)
        {
            /// TODO Check if grasshopper component is still there ... otherwise close
        }

        /* ------------------------------------------------------------------*/
        // local variables

        private ReloadFunction_del reload_func;
        private int clicks = 0;

    }
}
