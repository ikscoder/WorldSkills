using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LocalServer
{
    class Diagram
    {
        public Border Diagramma { get; private set; }
        public SolidColorBrush Color { get; set; }

        Grid diagram { get; set; }

        List<double> Data;
        uint MaxData;

        public Diagram(uint DataCount, uint _Width, uint _Height)
        {
            Data = new List<double>();
            MaxData = DataCount > 1 ? DataCount : 10;
            Color = Brushes.Black;
            diagram = new Grid();
            Diagramma = new Border() { Width = _Width, Height = _Height,BorderThickness=new Thickness(1),BorderBrush=Color, Padding = new Thickness(5), Child=diagram };
        }

        public void addData(double data)
        {
            if (Data.Count < MaxData)
            {
                Data.Add(data);
            }
            else
            {
                Data.Remove(Data.First());
                Data.Add(data);
            }
            Draw();
        }

        void Draw()
        {
            diagram.Children.Clear();
            double Max = Data.Max();
            double Min = Data.Min();
            Line l;
            Label lab;
            double x = 0;
            double y = Diagramma.Height;
            int i = 1;
            foreach (double data in Data)
            {
                l = new Line()
                {
                    StrokeThickness = 1,
                    Stroke = Color,
                    X1 = x,
                    X2 = Diagramma.Width / MaxData * i,
                    Y1 = y,
                    Y2 = Diagramma.Height-Diagramma.Height * (data - Min) / (Max - Min),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                x = l.X2;
                y = l.Y2;
                lab = new Label()
                {
                    Content = data,
                    Margin = new Thickness(x-2, y-2, 0, 0),
                    FontSize=8
                };
                i++;
                diagram.Children.Add(l);
                diagram.Children.Add(lab);
            }
        }
    }
}
