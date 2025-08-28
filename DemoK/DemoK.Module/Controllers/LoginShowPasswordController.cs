using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DemoK.Module.BusinessObjects;

namespace DemoK.Module.Controllers
{
    /// <summary>
    /// Controller that handles the ShowPassword functionality in the login form
    /// </summary>
    public class LoginShowPasswordController : WindowController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            
            // Subscribe to view creation events to handle login forms
            Application.DetailViewCreated += Application_DetailViewCreated;
        }

        private void Application_DetailViewCreated(object sender, DetailViewCreatedEventArgs e)
        {
            // Check if this is a login form with CustomLogonParameters
            if (e.View.ObjectTypeInfo?.Type == typeof(CustomLogonParameters))
            {
                SetupPasswordVisibilityToggle(e.View);
            }
        }

        private void SetupPasswordVisibilityToggle(DetailView detailView)
        {
            var showPasswordEditor = detailView.FindItem("ShowPassword") as PropertyEditor;
            var passwordEditor = detailView.FindItem("Password") as PropertyEditor;
            
            if (showPasswordEditor != null && passwordEditor != null)
            {
                // Subscribe to control creation events
                showPasswordEditor.ControlCreated += (s, e) => {
                    WireUpPasswordToggle(detailView, showPasswordEditor, passwordEditor);
                };
                
                passwordEditor.ControlCreated += (s, e) => {
                    WireUpPasswordToggle(detailView, showPasswordEditor, passwordEditor);
                };
            }
        }

        private void WireUpPasswordToggle(DetailView detailView, PropertyEditor showPasswordEditor, PropertyEditor passwordEditor)
        {
            if (detailView.CurrentObject is CustomLogonParameters logonParams)
            {
                // Subscribe to property changes
                logonParams.PropertyChanged += (s, e) => {
                    if (e.PropertyName == nameof(CustomLogonParameters.ShowPassword))
                    {
                        UpdatePasswordVisibility(passwordEditor, logonParams.ShowPassword);
                    }
                };
                
                // Set initial state
                UpdatePasswordVisibility(passwordEditor, logonParams.ShowPassword);
            }
        }

        private void UpdatePasswordVisibility(PropertyEditor passwordEditor, bool showPassword)
        {
            if (passwordEditor?.Control != null)
            {
                // Use reflection to access control properties since we don't have platform-specific references
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
                        useSystemPasswordCharProperty.SetValue(properties, !showPassword);
                    }
                    
                    // Set PasswordChar
                    var passwordCharProperty = propertiesType.GetProperty("PasswordChar");
                    if (passwordCharProperty != null)
                    {
                        passwordCharProperty.SetValue(properties, showPassword ? '\0' : '*');
                    }
                }
            }
        }

        protected override void OnDeactivated()
        {
            Application.DetailViewCreated -= Application_DetailViewCreated;
            base.OnDeactivated();
        }
    }
}