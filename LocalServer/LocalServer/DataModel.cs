using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LocalServer
{
    class DataModel
    {
        public const int GROUP_COUNT = 5;
        public const int StopDelay = 5000;

        static DataModel _instance { get; set; }
        public static DataModel Instance { get { return _instance ?? (_instance = new DataModel()); } }

        uint _groupId { get; set; }
        double _temperature { get; set; }
        double _brightness { get; set; }
        double _humidity { get; set; }
        bool _ventState { get; set; }
        bool _pumpState { get; set; }
        double _lampBrightness { get; set; }
        double[] TemperatureBoundariesMin { get; set; }
        double[] TemperatureBoundariesMax { get; set; }
        public double BrightnessMax { get; private set; }
        public double BrightnessMin { get; private set; }
        public double HumidityMax { get; private set; }
        public double HumidityMin { get; private set; }


        public uint GroupId { get { return _groupId; } set { if (value != _groupId) { _groupId = value % GROUP_COUNT; } } }
        public double Temperature { get { return _temperature; } set { if (value != _temperature) { _temperature = value; TemperatureNotify(); VentMangement(); } } }
        public double Brightness { get { return _brightness; } set { if (value != _brightness) { _brightness = value; BrightnessNotify(); BrightnessMangement(); } } }
        public double Humidity { get { return _humidity; } set { if (value != _humidity) { _humidity = value; HumidityNotify(); HumidityMangement(); } } }
        public bool VentState { get { return _ventState; } set { if (value != _ventState) { _ventState = value; } } }
        public bool PumpState { get { return _pumpState; } set { if (value != _pumpState) { _pumpState = value; } } }
        public double LampBrightness { get { return _lampBrightness; } set { if (value != _lampBrightness) { _lampBrightness = value; LampNotify(); } } }

        List<object> TempListener;
        List<object> BrighListener;
        List<object> HumiListener;
        List<object> LampListener;

        public void addTemperatureListener(object listener) { TempListener.Add(listener); }
        public void addBrightnessListener(object listener) { BrighListener.Add(listener); }
        public void addHumidityListener(object listener) { HumiListener.Add(listener); }
        public void addLampListener(object listener) { LampListener.Add(listener); }
        public void removeTemperatureListener(object listener) { TempListener.Remove(listener); }
        public void removeBrightnessListener(object listener) { BrighListener.Remove(listener); }
        public void removeHumidityListener(object listener) { HumiListener.Remove(listener); }
        public void removeLampListener(object listener) { LampListener.Remove(listener); }

        void TemperatureNotify()
        {
            foreach (var obj in TempListener)
            {
                if (obj is Label)
                {
                    (obj as Label).Content = _temperature;
                }
                if (obj is Diagram)
                {
                    (obj as Diagram).addData(_temperature);
                }
                if (obj is Button)
                {
                    if (VentState)
                    {
                        (obj as Button).Content = "Off";
                    }
                    else { (obj as Button).Content = "On"; }
                }
            }
        }

        void BrightnessNotify()
        {
            foreach (var obj in BrighListener)
            {
                if (obj is Label)
                {
                    (obj as Label).Content = _brightness;
                }
                if (obj is Diagram)
                {
                    (obj as Diagram).addData(_brightness);
                }
            }
        }

        void HumidityNotify()
        {
            foreach (var obj in HumiListener)
            {
                if (obj is Label)
                {
                    (obj as Label).Content = _humidity;
                }
                if (obj is Diagram)
                {
                    (obj as Diagram).addData(_humidity);
                }
                if (obj is Button)
                {
                    if (PumpState)
                    {
                        (obj as Button).Content = "Off";
                    }
                    else
                    {
                        (obj as Button).Content = "On";
                    }
                }
            }
        }

        void LampNotify() {
            foreach (var obj in LampListener)
            {
                if (obj is Label)
                {
                    (obj as Label).Content = LampBrightness;
                }
                if (obj is Diagram)
                {
                    (obj as Diagram).addData(LampBrightness);
                }
                if (obj is Slider)
                {
                    (obj as Slider).Value = _lampBrightness;
                }
            }
        }
        void VentMangement()
        {
            if (_temperature >= TemperatureBoundariesMax[_groupId])
                VentState = false;
            if (_temperature <= TemperatureBoundariesMin[_groupId])
                VentState = true;
        }
        void HumidityMangement()
        {
            if (_humidity >= HumidityMax)
                PumpState = false;
            if (_humidity <= HumidityMin)
                PumpState = true;
        }
        void BrightnessMangement()
        {
            if (_brightness >= BrightnessMax || _brightness <= BrightnessMin)
            {
                LampBrightness = BrightnessMax - _brightness;
                if (LampBrightness < 0) LampBrightness = 0;
            }
        }

        DataModel()
        {
            TempListener = new List<object>();
            BrighListener = new List<object>();
            HumiListener = new List<object>();
            LampListener = new List<object>();
            TemperatureBoundariesMax = new double[GROUP_COUNT] { 15, 20, 25, 30, 30 };
            TemperatureBoundariesMin = new double[GROUP_COUNT] { 0, 5, 15, 20, 10 };
            BrightnessMin = 20;
            BrightnessMax = 40;
            HumidityMax = 100;
            HumidityMin = 10;

            VentState = false;
            PumpState = true;
            LampBrightness = 0;
        }
    }
}
