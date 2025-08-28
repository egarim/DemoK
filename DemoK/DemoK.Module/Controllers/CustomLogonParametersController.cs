using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DemoK.Module.BusinessObjects;

namespace DemoK.Module.Controllers
{
    public class CustomLogonParametersController : ObjectViewController<DetailView, CustomLogonParameters>
    {
        private PropertyEditor showPasswordEditor;
        private PropertyEditor passwordEditor;

        protected override void OnActivated()
        {
            base.OnActivated();
            
            // Get references to the property editors
            showPasswordEditor = View.FindItem("ShowPassword") as PropertyEditor;
            passwordEditor = View.FindItem("Password") as PropertyEditor;
            
            if (showPasswordEditor != null)
            {
                showPasswordEditor.ControlCreated += ShowPasswordEditor_ControlCreated;
            }
        }

        private void ShowPasswordEditor_ControlCreated(object sender, EventArgs e)
        {
            if (showPasswordEditor?.Control != null)
            {
                // Subscribe to the property change event
                if (View.CurrentObject is CustomLogonParameters currentObject)
                {
                    currentObject.PropertyChanged += CurrentObject_PropertyChanged;
                }
            }
        }

        private void CurrentObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CustomLogonParameters.ShowPassword))
            {
                UpdatePasswordVisibility();
            }
        }

        private void UpdatePasswordVisibility()
        {
            if (passwordEditor?.Control != null && View.CurrentObject is CustomLogonParameters currentObject)
            {
                // Use reflection to access password control properties since we don't have Win-specific references
                var control = passwordEditor.Control;
                var controlType = control.GetType();
                
                // Try to find Properties property (common in DevExpress controls)
                var propertiesProperty = controlType.GetProperty("Properties");
                if (propertiesProperty != null)
                {
                    var properties = propertiesProperty.GetValue(control);
                    var propertiesType = properties.GetType();
                    
                    // Set UseSystemPasswordChar
                    var useSystemPasswordCharProperty = propertiesType.GetProperty("UseSystemPasswordChar");
                    if (useSystemPasswordCharProperty != null)
                    {
                        useSystemPasswordCharProperty.SetValue(properties, !currentObject.ShowPassword);
                    }
                    
                    // Set PasswordChar
                    var passwordCharProperty = propertiesType.GetProperty("PasswordChar");
                    if (passwordCharProperty != null)
                    {
                        passwordCharProperty.SetValue(properties, currentObject.ShowPassword ? '\0' : '*');
                    }
                }
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            
            // Set initial password visibility state
            UpdatePasswordVisibility();
        }

        protected override void OnDeactivated()
        {
            // Unsubscribe from events to prevent memory leaks
            if (showPasswordEditor != null)
            {
                showPasswordEditor.ControlCreated -= ShowPasswordEditor_ControlCreated;
            }
            
            if (View.CurrentObject is CustomLogonParameters currentObject)
            {
                currentObject.PropertyChanged -= CurrentObject_PropertyChanged;
            }
            
            base.OnDeactivated();
        }
    }
}