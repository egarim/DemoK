using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
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
            //https://docs.devexpress.com/eXpressAppFramework/118592/ui-construction/application-model-ui-settings-storage/customize-application-model-in-code/apply-application-model-changes-to-the-current-view-immediately
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
                
                // Update the Password property editor caption
                UpdatePasswordCaption(customLogonParams?.ShowPassword ?? false);
            }
        }
        
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        
        /// <summary>
        /// Updates the Password property editor caption based on the ShowPassword value
        /// </summary>
        /// <param name="showPassword">The current value of ShowPassword property</param>
        private void UpdatePasswordCaption(bool showPassword)
        {
            try
            {
                // Access the view model
                var detailViewModel = View.Model as IModelDetailView;
                if (detailViewModel == null)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to access detail view model");
                    return;
                }

                // Find the Password property editor model
                var passwordPropertyEditor = detailViewModel.Items["Password"] as IModelPropertyEditor;
                if (passwordPropertyEditor == null)
                {
                    System.Diagnostics.Debug.WriteLine("Password property editor not found in model");
                    return;
                }

                // Following the DevExpress pattern for applying model changes immediately
                // https://docs.devexpress.com/eXpressAppFramework/118592
                
                // Obtain and save the view
                var savedView = View;

                // Detach the View from the Frame - Don't dispose the old view
                if (Frame.SetView(view: null, true, null, disposeOldView: false))
                {
                    // Change the Application Model
                    string newCaption = $"Password - {showPassword.ToString().ToLower()}";
                    ((IModelViewItem)passwordPropertyEditor).Caption = newCaption;
                    
                    System.Diagnostics.Debug.WriteLine($"Updated Password caption to: {newCaption}");

                    // Load Model changes into the View
                    savedView.LoadModel(false);

                    // Re-attach the View back to its Frame
                    Frame.SetView(savedView);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating password caption: {ex.Message}");
            }
        }
    }
}
