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
using System.Windows.Shapes;

namespace GraphProject.AlgorithmsDemo.PlanningAlgorithm
{
    /// <summary>
    /// Interaction logic for PlanningCostWindow.xaml
    /// </summary>
    public partial class PlanningCostWindow : Window
    {
        private readonly int countNode;
        private List<TextBox> txbsCost;
        private PlanningCostResult resultData;
        private Button btnAccept;

        public PlanningCostWindow(int countNode)
        {
            InitializeComponent();

            this.countNode = countNode;
            this.txbsCost = new List<TextBox>();
            this.resultData = new PlanningCostResult(PlanningCostDigits.Cancel);
            this.btnAccept = null;

            this.Tag = this.resultData;
        }

        private ListBoxItem CreateNodeItemLayout(int nodeIndex)
        {
            // < ListBoxItem>
            //    < DockPanel >
            //        < TextBlock > Text block </ TextBlock >
            //        < TextBox ></ TextBox >   
            //    </ DockPanel >   
            //</ ListBoxItem >
            ListBoxItem lsbItem = new ListBoxItem();
            DockPanel dock = new DockPanel();
            TextBlock textBlock = new TextBlock();
            TextBox textBox = new TextBox();

            lsbItem.Content = dock;

            dock.Children.Add(textBlock);
            dock.Children.Add(textBox);

            textBlock.Text = $"Cost[{nodeIndex}] : ";

            this.txbsCost.Add(textBox);

            return lsbItem;
        }

        private ListBoxItem CreateButtonAcceptLayout()
        {
            //< ListBoxItem >
            //    < Button > ok </ Button >
            //</ ListBoxItem > 
            ListBoxItem lsbItem = new ListBoxItem();
            Button btn = new Button();

            lsbItem.Content = btn;
            btn.Content = "Ok";

            this.btnAccept = btn;
            btn.Click += BtnAccept_Click;

            return lsbItem;
        }

        /// <returns>return -1 if can send data, otherwise return number line where raise error</returns>
        private int ConvertData()
        {
            this.resultData.Results = new List<double>();
            for (int i = 0; i < this.countNode; i++)
            {
                double value;
                if (!double.TryParse(this.txbsCost[i].Text, out value))
                {
                    return i;
                }
                this.resultData.Results.Add(value);
            }
            return -1;
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            int indexError = ConvertData();
            if (indexError == -1) // success
            {
                this.resultData.Digit = PlanningCostDigits.Accept;
                //this.Tag = this.resultData;
                this.Close();
            }
            else // fail
            {
                MessageBox.Show($"Cost[{indexError}] is invalid");
                txbsCost[indexError].Focus();
                txbsCost[indexError].SelectAll();
            }
        }

        private void PlanningCostWindow_Loaded(object sender, RoutedEventArgs e)
        {
            for (int nodeIndex = 0; nodeIndex < this.countNode; nodeIndex++)
            {
                lsbLayout.Items.Add(CreateNodeItemLayout(nodeIndex));                
            }
            lsbLayout.Items.Add(CreateButtonAcceptLayout());
        }

        private void PlanningCostWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.btnAccept.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            }
        }
    }
}
