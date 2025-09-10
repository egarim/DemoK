using DevExpress.CodeParser.VB;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DemoK.Module.Controllers
{
    public class ShowPasswordViewController : ViewController<DetailView>
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


                //var PasswordPropertyEditor=  this.View.FindItem("Password") as PropertyEditor;
                //PasswordPropertyEditor.IsPassword = !(customLogonParams?.ShowPassword ?? false);

                UpdatePasswordCaption(!customLogonParams.ShowPassword);


                // Update the Password property editor caption
                //UpdatePasswordCaption(customLogonParams?.ShowPassword ?? false);
            }
        }
        
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
          
        }
        private void SetMinValue(ViewItem viewItem)
        {
            var spinEdit = viewItem.Control as DevExpress.XtraEditors.TextEdit;
            var dxeditor= viewItem as PropertyEditor;
            dxeditor.IsPassword = false;

            //if (spinEdit != null) {
            //    spinEdit.Properties.UseSystemPasswordChar = false;
            //}
          

        }
        /// <summary>
        /// Updates the Password property editor caption and IsPassword behavior based on the ShowPassword value
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
                    // Change the Application Model - Update caption
                    string newCaption = $"Password - {showPassword.ToString().ToLower()}";
                    IModelViewItem passwordPropertyEditor1 = ((IModelViewItem)passwordPropertyEditor);
                    passwordPropertyEditor1.Caption = newCaption;

                    IModelCommonMemberViewItem p = passwordPropertyEditor as IModelCommonMemberViewItem;
                    p.IsPassword = showPassword;


                    //var passwordEditor = passwordPropertyEditor as StringPropertyEditor;
                    //passwordEditor.IsPassword = showPassword;

                    //UpdatePasswordEditorBehavior(showPassword);
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

        /// <summary>
        /// Updates the StringPropertyEditor IsPassword property to control password masking
        /// </summary>
        /// <param name="showPassword">The current value of ShowPassword property</param>
        private void UpdatePasswordEditorBehavior(bool showPassword)
        {
            try
            {
                // Cast the View to DetailView to access the Items collection
                var detailView = View as DetailView;
                if (detailView == null)
                {
                    System.Diagnostics.Debug.WriteLine("View is not a DetailView");
                    return;
                }

                // Find the actual Password property editor control
                var passwordEditor = detailView.FindItem("Password") as StringPropertyEditor;
                if (passwordEditor != null)
                {

                    // Check if the property editor has an IsPassword property
                    var isPasswordProperty = passwordEditor.GetType().GetProperty("IsPassword");
                    if (isPasswordProperty != null && isPasswordProperty.CanWrite)
                    {
                        // Set IsPassword to the inverse of showPassword
                        // When showPassword is true, we want the field to show text (IsPassword = false)
                        // When showPassword is false, we want the field to be masked (IsPassword = true)
                        bool newIsPasswordValue = showPassword;

                        passwordEditor.IsPassword = newIsPasswordValue;
                        //isPasswordProperty.SetValue(passwordEditor, newIsPasswordValue);

                        System.Diagnostics.Debug.WriteLine($"Updated Password IsPassword property to: {newIsPasswordValue}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("IsPassword property not found or not writable on the property editor");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("PropertyEditor for Password not found");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating password editor behavior: {ex.Message}");
            }
        }
    }
}
