using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SWLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SettingsWindow settingsWindow = new SettingsWindow();        
        Schedule scheduleE;
        List<Schedule> schEntries = new List<Schedule>();
        List<Schedule> onAirEntries = new List<Schedule>();
        List<string> stationName = new List<string>();
        List<string> languages = new List<string> { "All" };
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        DispatcherTimer minuteTick = new System.Windows.Threading.DispatcherTimer();
        string dataPath;
        string historyPath;
        string[] schedEntry;
        string language = "";
        int hitIndex = 0;
        bool stationS = true;
        DateTime UTC = DateTime.UtcNow;

        public MainWindow()
        {
            InitializeComponent();

            

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(200);
            dispatcherTimer.Start();

            minuteTick.Tick += new EventHandler(minuteTick_Tick);
            minuteTick.Interval = new TimeSpan(0, 1, 0);
            minuteTick.Start();

            dataPath = settingsWindow.dataPath;
            historyPath = settingsWindow.historyPath;
            schedEntry = File.ReadAllLines(dataPath);
            StationBox.ItemsSource = stationName;

            foreach (string entry in schedEntry)
            {
                string[] split = entry.Split(';');
                scheduleE = new Schedule(split);
                schEntries.Add(scheduleE);
                
                bool dupe = false;
                
                for (int i = 0; i < stationName.Count; i++)
                {
                    if (scheduleE.Station == stationName[i])
                    {
                        dupe = true;
                    }
                }
                if (dupe != true)
                {
                    stationName.Add(scheduleE.Station);
                }
                dupe = false;
                for (int i = 0; i < languages.Count; i++)
                {
                    if (languages[i] == scheduleE.Language)
                    {
                        dupe = true;
                    }
                }
                if (dupe != true)
                {
                    languages.Add(scheduleE.Language);
                }                
            }

            stationName.Sort();
            languages.Sort();
            LanguageCombo.ItemsSource = languages;
            LanguageCombo.SelectedIndex = 12;
            DrawLive();
        }

        private void minuteTick_Tick(object sender, EventArgs e)
        {
            DrawLive();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            UTC = DateTime.UtcNow;
            ClockUTC.Content = UTC.TimeOfDay.ToString();
        }

        private void OnAirGrid_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void DrawLive()
        {
            
            TimeSpan now = UTC.TimeOfDay;
            
            foreach (Schedule entry in schEntries)
            {
                /*
                bool dupe = false;
                for (int i = 0; i < stationName.Count; i++)
                {
                    if (stationName[i] == entry.Station)
                    {
                        dupe = true;
                    }
                }

                if (dupe != true)
                {
                    stationName.Add(entry.Station);
                }
                dupe = false;
                for (int i = 0; i < languages.Count; i++)
                {
                    if (languages[i] == entry.Language)
                    {
                        dupe = true;
                    }
                }
                
                if (dupe != true)
                {
                    languages.Add(entry.Language);
                }
                */

                if ((now > entry.StartTime) && (now < entry.EndTime))
                {
                    if (language == entry.Language || language == "All" || language == "")
                    {
                        switch (stationS)
                        {
                            case true:
                                if (StationBox.Text == entry.Station)
                                {
                                    onAirEntries.Add(entry);
                                    OnAirGrid.Items.Add(entry);
                                    break;
                                }

                                if (StationBox.Text == "")
                                {
                                    onAirEntries.Add(entry);
                                    OnAirGrid.Items.Add(entry);
                                }
                                break;

                            case false:
                                if (FreqBox.Text == entry.Frequency)
                                {
                                    onAirEntries.Add(entry);
                                    OnAirGrid.Items.Add(entry);
                                    break;
                                }
                                break;
                        }                        
                    }
                }
            }
            
            //languages[0] = "All";
            
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            settingsWindow.ShowDialog();
        }

        private void OnAirGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu m = new ContextMenu();
            m = RightClickMenu;
            //m.Items.Add("");
            //m.Items.Add("Populate list");
            m.IsOpen = true;

            HitTestResult hitTestResult = VisualTreeHelper.HitTest(OnAirGrid, e.GetPosition(OnAirGrid));
            DataGridRow dataGridRow = hitTestResult.VisualHit.GetParentOfType<DataGridRow>();
            try
            {
                hitIndex = dataGridRow.GetIndex();
            }
            catch (Exception) { }
        }       
        private void OnAirGrid_DoubleClick(object sender, RoutedEventArgs e)
        {
            hitIndex = OnAirGrid.SelectedIndex;
            
            Populate_Click(sender, e);
        }

        private void QuickAdd_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Populate_Click(object sender, RoutedEventArgs e)
        {
            FreqBox.Text = onAirEntries[hitIndex].Frequency;
            StationBox.Text = onAirEntries[hitIndex].Station;
            CountryText.Content = onAirEntries[hitIndex].Country;
            LanguageText.Content = onAirEntries[hitIndex].Language;
            BroadcastText.Content = onAirEntries[hitIndex].BroadcastTime;
            TimeText.Content = new TimeSpan(UTC.Hour, UTC.Minute, UTC.Second).ToString(); ;
        }

        private void StationSearch_Click(object sender, RoutedEventArgs e)
        {
            OnAirGrid.Items.Clear();
            stationS = true;
            onAirEntries.Clear();
            DrawLive();
        }

        private void LanguageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            language = LanguageCombo.SelectedItem.ToString();
            OnAirGrid.Items.Clear();
            onAirEntries.Clear();
            DrawLive();
        }

        private void FreqSearch_Click(object sender, RoutedEventArgs e)
        {
            language = LanguageCombo.SelectedItem.ToString();
            stationS = false;
            OnAirGrid.Items.Clear();
            onAirEntries.Clear();
            DrawLive();
        }
    }
    public static class Extensions
    {
        public static T GetParentOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            Type type = typeof(T);
            if (element == null) return null;
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent == null && ((FrameworkElement)element).Parent is DependencyObject) parent = ((FrameworkElement)element).Parent;
            if (parent == null) return null;
            else if (parent.GetType() == type || parent.GetType().IsSubclassOf(type)) return parent as T;
            return GetParentOfType<T>(parent);
        }
    }
}

