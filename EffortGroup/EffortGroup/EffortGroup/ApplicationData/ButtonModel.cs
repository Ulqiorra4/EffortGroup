using EffortGroup.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace EffortGroup.ApplicationData
{
    public class ButtonModel
    {
        public string Content { get; set; }
        public BitmapImage ImageSource { get; set; }

        public int IdEmp { get; set; }
        public Action<int> OpenProfile { get; set; }

        public ButtonModel(string name, string imagePath, int idEmp)
        {
            Content = name;
            IdEmp = idEmp;

            if (!string.IsNullOrEmpty(imagePath))
            {
                ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            }
            OpenProfile = (id) => {
                EditEmployeeWindow editWindow = new EditEmployeeWindow(id);
                editWindow.ShowDialog();
            };
        }

    }
}