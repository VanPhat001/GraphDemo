using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphProject
{
    public class MyListViewControl
    {
        private ListView listView;

        public MyListViewControl(ListView listView)
        {
            this.listView = listView;
        }

        public void Clear()
        {
            this.listView.Items.Clear();
        }

        public void AddText(string text)
        {
            this.listView.Items.Add(new ListViewItem() { Content = text });
        }

        public void ChangeBackgroundItem(int itemIndex, Brush color)
        {
            (this.listView.Items[itemIndex] as ListBoxItem).Background = color;
        }

        public void ResetBackgroundItems()
        {
            foreach (var item in this.listView.Items)
            {
                var listViewItem = item as ListViewItem;
                listViewItem.Background = Brushes.Transparent;
            }
        }

        public void SetVisible()
        {
            this.listView.Opacity = 1.0;
            this.listView.Background = Brushes.White;
        }

        public void SetHidden()
        {
            this.listView.Opacity = 0;
        }
    }
}
