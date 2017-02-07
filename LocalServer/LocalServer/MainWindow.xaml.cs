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
            Lamp.Maximum= DataModel.Instance.BrightnessMax;
            DataModel.Instance.addLampListener(Lamp);
            Lamp.ValueChanged += (s, e) => {
                DataModel.Instance.LampBrightness = Lamp.Value;
            };
        }


        private void Update_Click(object sender, RoutedEventArgs e)
        {
            //var selected = (string)Ports.SelectedItem;
                //Ports.DataContext = SerialPort.GetPortNames().ToList();
                //TryReadFromPortAsync();
            //Ports.SelectedItem = selected;
        }

        private void Ports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TryReadFromPortAsync();
        }

        private void Speeds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TryReadFromPortAsync();
        }

        SerialPort currentPort = null;
        volatile bool checker=true;
        private async void TryReadFromPortAsync()
        {
            checker = false;
            try
            {               
                currentPort = new SerialPort((string)Ports.SelectedItem ?? SerialPort.GetPortNames()[0], (int?)Speeds.SelectedItem ?? 9600);
            }
            catch { }
            await Task.Run(async () =>
                {
                    System.Threading.Thread.Sleep(500);
                    checker = true;
                    currentPort.BaudRate = 9600;
                    currentPort.DtrEnable = true;
                    currentPort.ReadTimeout = 1500;
                    currentPort.Open();
                    /* while (currentPort.BytesToRead != 0)
                     {
                         output.Text += currentPort.ReadExisting();
                         currentPort.DiscardInBuffer();
                         System.Threading.Thread.Sleep(500);
                     }*/
                    while (checker)
                {
                    try
                    {
                            string line= currentPort.ReadLine();
                            string[] data = line.Split(' ');


                            await Dispatcher.BeginInvoke(new Action(delegate ()
                            {
                                try
                                {
                                    DataModel.Instance.Temperature = double.Parse(data[0].Substring(data[0].IndexOf("Temperature=") + "Temperature=".Length));
                                    DataModel.Instance.Brightness = double.Parse(data[1].Substring(data[1].IndexOf("Brightness=") + "Brightness=".Length));
                                    DataModel.Instance.Humidity = double.Parse(data[2].Substring(data[2].IndexOf("Humidity=") + "Humidity=".Length));
                                }
                                catch { }
                                
                                output.Text += line;
                            }));
                            System.Threading.Thread.Sleep(50);
                        }
                    catch (TimeoutException) { }
                }
                    currentPort.Close();

        });


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //string responsetext = new StreamReader(WebRequest.Create("УРЛ вашего сервиса").GetResponse().GetResponseStream()).ReadToEnd();

           /* WebClient wc = new WebClient();
            wc.DownloadStringCompleted +=
                delegate (object sender, DownloadStringCompletedEventArgs e)
                {
                    string responsetext = e.Result;
                };
            wc.DownloadStringAsync(new Uri("URL вашего сервиса"));

            WebClient wc = new WebClient();
            wc.UploadDataCompleted +=
                delegate (object sender, UploadDataCompletedEventArgs e)
                {
                    string responsetext = Encoding.UTF8.GetString(e.Result);
                };
            wc.UploadDataAsync(new Uri("URL вашего сервиса"), Encoding.UTF8.GetBytes("То что вы будете отправлять POSTом"));*/
        }

        private void Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataModel.Instance.GroupId = (uint)(sender as ListBox).SelectedIndex;
        }
    }
}
