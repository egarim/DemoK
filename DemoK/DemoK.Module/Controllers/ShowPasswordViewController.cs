using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoK.Module.Controllers
{
    public class ShowPasswordViewController : ViewController
    {
        public ShowPasswordViewController() : base()
        {
            // Target required Views (use the TargetXXX properties) and create their Actions.
            this.TargetObjectType = typeof(DemoK.Module.BusinessObjects.CustomLogonParameters);
        }
        
        protected override void OnActivated()
        {
            base.OnActivated();
            // Subscribe to property change events
            if (View.CurrentObject is DemoK.Module.BusinessObjects.CustomLogonParameters customLogonParams)
            {
                if (customLogonParams is INotifyPropertyChanged notifyPropertyChanged)
                {
                    notifyPropertyChanged.PropertyChanged += CustomLogonParams_PropertyChanged;
                }
            }
        }
        
        protected override void OnDeactivated()
        {
            // Unsubscribe from property change events
            if (View.CurrentObject is DemoK.Module.BusinessObjects.CustomLogonParameters customLogonParams)
            {
                if (customLogonParams is INotifyPropertyChanged notifyPropertyChanged)
                {
                    notifyPropertyChanged.PropertyChanged -= CustomLogonParams_PropertyChanged;
                }
            }
            base.OnDeactivated();
        }
        
        private void CustomLogonParams_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DemoK.Module.BusinessObjects.CustomLogonParameters.ShowPassword))
            {
                var customLogonParams = sender as DemoK.Module.BusinessObjects.CustomLogonParameters;
                // Handle the ShowPassword property change here
                // You can add your custom logic here
                System.Diagnostics.Debug.WriteLine($"ShowPassword changed to: {customLogonParams?.ShowPassword}");
            }
        }
        
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
    }
}
