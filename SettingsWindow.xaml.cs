﻿using System;
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
using System.Windows.Shapes;

namespace SWLogger
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        string path = Directory.GetCurrentDirectory();
        public string dataPath { get; set; }
        public string historyPath { get; set; }
        public SettingsWindow()
        {
            InitializeComponent();
            dataPath = path + @"\sked-a20.csv";
            historyPath = path + @"\History.txt";
            FilePath.Text = dataPath;
            HistoryFile.Text = historyPath;
        }
    }
}
