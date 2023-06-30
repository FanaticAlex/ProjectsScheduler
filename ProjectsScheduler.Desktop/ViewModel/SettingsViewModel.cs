using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsScheduler.Desktop.ViewModel
{
    internal class SettingsViewModel : BaseVewModel
    {
        private int _timeLimitInSeconds;
        public int TimeLimitInSeconds
        {
            get { return _timeLimitInSeconds; }
            set { _timeLimitInSeconds = value; OnPropertyChanged(nameof(TimeLimitInSeconds)); }
        }

        public SettingsViewModel()
        {
        }

        public void SetSettings(int timeLimitInSeconds)
        {
            TimeLimitInSeconds = timeLimitInSeconds;
        }
    }
}
