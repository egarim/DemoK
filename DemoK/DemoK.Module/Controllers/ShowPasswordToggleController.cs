using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DemoK.Module.BusinessObjects;
using System.ComponentModel;

namespace DemoK.Module.Controllers
{
    /// <summary>
    /// Simple controller that specifically handles the ShowPassword toggle functionality
    /// for CustomLogonParameters in any context (login form, detail view, etc.)
    /// </summary>
    public class ShowPasswordToggleController : Controller
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            
            // Handle the current view if it's already set
            HandleView(Frame.View);
            
            // Subscribe to frame events
            Frame.ViewChanged += Frame_ViewChanged;
        }

        private void Frame_ViewChanged(object sender, ViewChangedEventArgs e)
        {
            HandleView(Frame.View);
        }

        private void HandleView(View view)
        {
            if (view is DetailView detailView && 
                detailView.ObjectTypeInfo?.Type == typeof(CustomLogonParameters))
            {
                SetupShowPasswordToggle(detailView);
            }
        }

        private void SetupShowPasswordToggle(DetailView detailView)
        {
            // Find the ShowPassword and Password editors
            var showPasswordEditor = detailView.FindItem("ShowPassword") as PropertyEditor;
            var passwordEditor = detailView.FindItem("Password") as PropertyEditor;
            
            if (showPasswordEditor == null || passwordEditor == null)
                return;

            // Subscribe to control created events
            EventHandler<EventArgs> setupHandler = (s, e) => WirePasswordToggle(detailView, passwordEditor);
            
            showPasswordEditor.ControlCreated += setupHandler;
            passwordEditor.ControlCreated += setupHandler;
            
            // Also listen for view controls created event
            detailView.ControlsCreated += (s, e) => WirePasswordToggle(detailView, passwordEditor);
            
            // If controls are already created, wire them up immediately
            if (showPasswordEditor.Control != null && passwordEditor.Control != null)
            {
                WirePasswordToggle(detailView, passwordEditor);
            }
        }

        private void WirePasswordToggle(DetailView detailView, PropertyEditor passwordEditor)
        {
            if (detailView.CurrentObject is CustomLogonParameters logonParams)
            {
                // Subscribe to ShowPassword property changes
                PropertyChangedEventHandler propertyChangedHandler = (s, e) =>
                {
                    if (e.PropertyName == nameof(CustomLogonParameters.ShowPassword))
                    {
                        TogglePasswordVisibility(passwordEditor, logonParams.ShowPassword);
                    }
                };

                logonParams.PropertyChanged += propertyChangedHandler;
                
                // Set initial state
                TogglePasswordVisibility(passwordEditor, logonParams.ShowPassword);
            }
        }

        private void TogglePasswordVisibility(PropertyEditor passwordEditor, bool showPassword)
        {
            try
            {
                var control = passwordEditor?.Control;
                if (control == null) return;

                // Use reflection to access DevExpress TextEdit properties
                var propertiesProperty = control.GetType().GetProperty("Properties");
                if (propertiesProperty?.GetValue(control) is object properties)
                {
                    var propertiesType = properties.GetType();
                    
                    // Toggle UseSystemPasswordChar (when true, uses system password char)
                    var useSystemPasswordCharProp = propertiesType.GetProperty("UseSystemPasswordChar");
                    useSystemPasswordCharProp?.SetValue(properties, !showPassword);
                    
                    // Set custom password character (when not using system char)
                    var passwordCharProp = propertiesType.GetProperty("PasswordChar");
                    passwordCharProp?.SetValue(properties, showPassword ? '\0' : '?');
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                System.Diagnostics.Debug.WriteLine($"Error toggling password visibility: {ex.Message}");
            }
        }

        protected override void OnDeactivated()
        {
            Frame.ViewChanged -= Frame_ViewChanged;
            base.OnDeactivated();
        }
    }
}