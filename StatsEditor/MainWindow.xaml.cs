using ProtoBuf;
using StatsEditor.Model;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace StatsEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string DataPath = System.IO.Path.GetFullPath("data/stats.bin");

        private List<CreatureBaseStats> Stats = new List<CreatureBaseStats>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var stat = (CreatureBaseStats)DataListBox.SelectedItem;

            if (stat == null)
                return;

            propertyGrid.SelectedObject = stat;
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            CreatureBaseStats stat = new CreatureBaseStats();
            stat.PlayerClass = CharacterClass.Blademan;
            Stats.Add(stat);
            DataListBox.Items.Refresh();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(DataPath))
                File.Delete(DataPath);

            using (var file = File.Create(DataPath))
            {
                Serializer.SerializeWithLengthPrefix<List<CreatureBaseStats>>(file, Stats, PrefixStyle.Fixed32);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var stat = (CreatureBaseStats)DataListBox.SelectedItem;

            if (stat == null)
                return;

            Stats.Remove(stat);
            DataListBox.Items.Refresh();
            propertyGrid.SelectedObject = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(DataPath))
                using (var file = File.Create(DataPath))
                    Serializer.SerializeWithLengthPrefix<List<CreatureBaseStats>>(file, Stats, PrefixStyle.Fixed32);
            else
                using (FileStream stream = File.OpenRead(DataPath))
                    Stats = Serializer.DeserializeWithLengthPrefix<List<CreatureBaseStats>>(stream, PrefixStyle.Fixed32);

            DataListBox.DataContext = Stats;
        }
    }
}
