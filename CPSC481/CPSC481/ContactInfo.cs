using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;

namespace CPSC481
{
    public class ContactInfo : INotifyPropertyChanged
    {
        public String Name { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }

        public static ContactInfo readFromFile()
        {
            var conf = ConfigurationManager.AppSettings;

            return new ContactInfo() { Name = conf["fname"], Email = conf["email"], Phone = conf["phone"] };
        }

        public void save()
        {
            //Update runtime config
            ConfigurationManager.AppSettings.Set("fname", Name);
            ConfigurationManager.AppSettings.Set("phone", Phone);
            ConfigurationManager.AppSettings.Set("email", Email);

            //update config file
            var conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = conf.AppSettings.Settings;

            settings["fname"].Value = Name;
            settings["phone"].Value = Phone;
            settings["email"].Value = Email;

            conf.Save(ConfigurationSaveMode.Full);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }
}
