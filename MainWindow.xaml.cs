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

namespace SWLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SettingsWindow settingsWindow = new SettingsWindow();
        string dataPath;
        string historyPath;
        Schedule scheduleE;
        List<Schedule> schEntries = new List<Schedule>();
        public MainWindow()
        {
            InitializeComponent();
            
            dataPath = settingsWindow.dataPath;
            historyPath = settingsWindow.historyPath;
            DrawLive();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void DrawLive()
        {
            string[] schedEntry = File.ReadAllLines(dataPath);
            
            foreach (string entry in schedEntry)
            {
                string[] split = entry.Split(';');
                scheduleE = new Schedule(split);
                schEntries.Add(scheduleE);
                OnAirGrid.Items.Add(scheduleE);
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
    }
}
