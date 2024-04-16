using Caliburn.Micro;
using SchacksMacroManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchacksMacroManager
{
    public class CaliburnBootstrapper : BootstrapperBase
    {
        private WindowManager _wm;
        public CaliburnBootstrapper() : base(true)
        {
            _wm = new WindowManager();
            Initialize();
        }


        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            Assembly asm = typeof(CaliburnBootstrapper).Assembly;
            yield return asm;
            
            yield return typeof(MacrosViewModel).Assembly;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            _wm.ShowWindowAsync(new MacrosViewModel(this));
        }



        public void ShowDialog(Screen screen)
        {
            _wm.ShowDialogAsync(screen);
        }

        public void ShowWindow(Screen screen)
        {
            _wm.ShowWindowAsync(screen);
        }

    }
}
