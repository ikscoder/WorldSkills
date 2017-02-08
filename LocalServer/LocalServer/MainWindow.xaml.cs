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
using System.Timers;
using System.IO.Ports;
using System.Net;
using System.IO;
using System.Threading;

namespace LocalServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        public MainWindow()
        {
            InitializeComponent();
            Ports.DataContext = SerialPort.GetPortNames().ToList();
            DataModel.Instance.addTemperatureListener(Temperature);
            DataModel.Instance.addBrightnessListener(Brightness);
            DataModel.Instance.addHumidityListener(Humidity);
            DataModel.Instance.addLampListener(LampIndicator);
            Diagram temp = new Diagram(20, 280, 200);
            temp.Diagramma.Margin = new Thickness(5,170,0,0);
            temp.Diagramma.VerticalAlignment = VerticalAlignment.Top;
            temp.Diagramma.HorizontalAlignment = HorizontalAlignment.Left;
            temp.Color= (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF558B2F"));
            Visualization.Children.Add(temp.Diagramma);
            DataModel.Instance.addTemperatureListener(temp);
           
            temp = new Diagram(20, 280, 200);
            temp.Diagramma.Margin = new Thickness(290, 170, 0, 0);
            temp.Diagramma.VerticalAlignment = VerticalAlignment.Top;
            temp.Diagramma.HorizontalAlignment = HorizontalAlignment.Left;
            temp.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF558B2F"));
            Visualization.Children.Add(temp.Diagramma);
            DataModel.Instance.addBrightnessListener(temp);

            temp = new Diagram(20, 280, 200);
            temp.Diagramma.Margin = new Thickness(580, 170, 0, 0);
            temp.Diagramma.VerticalAlignment = VerticalAlignment.Top;
            temp.Diagramma.HorizontalAlignment = HorizontalAlignment.Left;
            temp.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF558B2F"));
            Visualization.Children.Add(temp.Diagramma);
            DataModel.Instance.addHumidityListener(temp);

            DataModel.Instance.addTemperatureListener(VentState);
            VentState.Click += (s, e) =>
            {
                DataModel.Instance.VentState = !DataModel.Instance.VentState;
            };
            DataModel.Instance.addHumidityListener(PumpState);
            PumpState.Click += (s, e) =>
            {
                DataModel.Instance.PumpState = !DataModel.Instance.PumpState;
            };

            Lamp.Minimum = 0;
            Lamp.Maximum= 255;
            DataModel.Instance.addLampListener(Lamp);
            Lamp.ValueChanged += (s, e) => {
                DataModel.Instance.LampBrightness = Lamp.Value;
            };
        }

        private void Ports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSerial();        
        }

        private void SetSerial()
        {
            if (DataModel.Instance.currentPort != null) return;
            DataModel.Instance.checker = false;
            try
            {
                DataModel.Instance.currentPort = new SerialPort((string)Ports.SelectedItem ?? SerialPort.GetPortNames()[0], 115200);
            }
            catch { }
            DataModel.Instance.TryReadFromPortAsync(Dispatcher,output);
        }

        private void Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataModel.Instance.GroupId = (uint)(sender as ListBox).SelectedIndex;
        }
    }
}
