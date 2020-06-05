using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Net;
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
        Downloading d = new Downloading();
        Schedule scheduleE;
        List<Schedule> schEntries = new List<Schedule>();
        List<Schedule> onAirEntries = new List<Schedule>();
        List<Contact> historyList = new List<Contact>();
        List<string> stationName = new List<string>();
        List<string> languages = new List<string> { "All" };
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        DispatcherTimer minuteTick = new System.Windows.Threading.DispatcherTimer();
        string dataPath;
        string historyPath;
        string[] schedEntry;
        string language = "";
        string stationText = "";
        int hitIndex = 0;
        bool stationS = true;
        DateTime UTC = DateTime.UtcNow;
        TimeSpan CurrentTime = new TimeSpan(0, 0, 0);

        string uri = "http://www.eibispace.de/dx/";
        string file = "sked-a20.csv";
        WebClient client = new WebClient();

        public MainWindow()
        {
            InitializeComponent();

            string WebResource = uri + file;


            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(200);
            dispatcherTimer.Start();

            minuteTick.Tick += new EventHandler(minuteTick_Tick);
            minuteTick.Interval = new TimeSpan(0, 1, 0);
            minuteTick.Start();

            dataPath = settingsWindow.dataPath;
            historyPath = settingsWindow.historyPath;


            try
            {
                d.status.Text = string.Format("Looking for schedule file...");
                schedEntry = File.ReadAllLines(dataPath);
                d.Show();
                d.status.Text = string.Format("Sorting schedule file...");
                d.Close();
                SortDataFile();
            }
            catch (FileNotFoundException)
            {
                d.status.Text = string.Format("Schedule file not foud!");
                d.Show();
                d.status.Text = string.Format("Downloading data file {0} from {1}...", file, uri);
                client.DownloadFile(new Uri(WebResource), dataPath);
                d.status.Text = string.Format("Successfully downloaded file {0} from {1}...", file, uri);
                schedEntry = File.ReadAllLines(dataPath);
                d.status.Text = string.Format("Sorting schedule file...");
                d.Close();
                SortDataFile();
            }
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
            
            UpdateHistory();
        }
        private void UpdateHistory()
        {
            HistoryGrid.Items.Clear();
            string[] loadHistory = File.ReadAllLines(historyPath);
            foreach (string entry in loadHistory)
            {
                string[] split = entry.Split('|');
                Contact newContact = new Contact(split, bool.Parse(split[7]), DateTime.Parse(split[split.Length - 1]),"Updating");

                HistoryGrid.Items.Add(newContact);
            }
        }

        private void minuteTick_Tick(object sender, EventArgs e)
        {
            OnAirGrid.Items.Clear();
            DrawLive();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            UTC = DateTime.UtcNow;
            CurrentTime = new TimeSpan(UTC.Hour, UTC.Minute, UTC.Second);
            ClockUTC.Content = UTC.TimeOfDay.ToString();
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

                if ((now > entry.StartTime) && (now < entry.EndTime))
                {
                    if (language == entry.Language || language == "All" || language == "")
                    {
                        switch (stationS)
                        {
                            case true:
                                if (stationText == entry.Station)
                                {
                                    onAirEntries.Add(entry);
                                    OnAirGrid.Items.Add(entry);
                                    break;
                                }

                                if (stationText == "")
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
            
            Schedule quickContact = onAirEntries[hitIndex];

            string[] contact = new string[] { quickContact.Frequency, quickContact.Station, quickContact.Country, quickContact.Language, quickContact.BroadcastTime, "", CurrentTime.ToString() };
            Contact newContact = new Contact(contact, WebSDRCheck.IsChecked, DateTime.Today.Date);
            historyList.Add(newContact);
            string writeable = ("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}\n");

            File.AppendAllText(historyPath, string.Format(writeable,
                newContact.Frequency,
                newContact.Station,
                newContact.Country,
                newContact.Language,
                newContact.TimeHeard,
                newContact.Broadcast,
                newContact.Notes,
                newContact.WebSDR,
                newContact.Date));

            UpdateHistory();
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
            stationText = StationBox.Text;
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

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            FreqBox.Text = "";
            StationBox.Text = "";
            CountryText.Content = "";
            LanguageText.Content = "";
            BroadcastText.Content = "";
            stationText = "";
            NotesBox.Text = "";
            WebSDRCheck.IsChecked = false;
            StationSearch_Click(sender, e);
        }

        private void OnSubmit(object sender, RoutedEventArgs e)
        {            
            string[] contact = new string[] { FreqBox.Text, StationBox.Text, CountryText.Content.ToString(), LanguageText.Content.ToString(), BroadcastText.Content.ToString(), NotesBox.Text, TimeText.Content.ToString() };
            Contact newContact = new Contact(contact, WebSDRCheck.IsChecked, DateTime.Today.Date);
            historyList.Add(newContact);
            string writeable = ("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}\n");
            
            File.AppendAllText(historyPath, string.Format(writeable, 
                newContact.Frequency, 
                newContact.Station, 
                newContact.Country, 
                newContact.Language, 
                newContact.TimeHeard, 
                newContact.Broadcast, 
                newContact.Notes, 
                newContact.WebSDR,
                newContact.Date));
            UpdateHistory();
        }

            private void StationBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StationSearch.IsDefault = true;
            FreqSearch.IsDefault = false;
            submit.IsDefault = false;
        }

        private void FreqBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            StationSearch.IsDefault = false;
            FreqSearch.IsDefault = true;
            submit.IsDefault = false;
        }

        private void NotesBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            StationSearch.IsDefault = false;
            FreqSearch.IsDefault = false;
            submit.IsDefault = true;
        }

        private void SortDataFile()
        {
            schedEntry = schedEntry.Skip(1).ToArray();
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

