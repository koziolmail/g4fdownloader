using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ClassLibrary6;

namespace LinkCreator
{
    class MainWindowModel : INotifyPropertyChanged
    {

        private string user;
        public string User
        {
            get { return user; }
            set
            {
                user = value;
                OnPropertyChanged(nameof(User));
                CalcLink();

            }
        }

        private void CalcLink()
        {
            Link = Newtonsoft.Json.JsonConvert.SerializeObject(new LinkDto(address, user, Pass, Path));
            OnPropertyChanged(nameof(Link));
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Link);
            Base64Link = System.Convert.ToBase64String(plainTextBytes);
            OnPropertyChanged(nameof(Base64Link));
        }

        public string Base64Link { get; set; }

        private string pass;
        public string Pass
        {
            get { return pass; }
            set
            {
                pass = value;
                OnPropertyChanged(nameof(Pass));
                CalcLink();
            }
        }
        private string path;
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                OnPropertyChanged(nameof(Path));
                CalcLink();
            }
        }
        public string Link { get; set; }
        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
                CalcLink();
            }
        }

        public MainWindowModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
