using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DemoK.Module.BusinessObjects;
using System;

namespace DemoK.Module.Controllers
{
    /// <summary>
    /// Controller for Document detail view that provides digital signature functionality
    /// </summary>
    public class DocumentSignatureController : ObjectViewController<DetailView, Document>
    {
        private PopupWindowShowAction signDocumentAction;

        public DocumentSignatureController()
        {
            // Create the popup window action to sign the document
            signDocumentAction = new PopupWindowShowAction(this, "SignDocument", "Edit")
            {
                Caption = "Sign Document",
                ImageName = "Security",
                ToolTip = "Sign this document with your credentials"
            };
            
            signDocumentAction.CustomizePopupWindowParams += SignDocumentAction_CustomizePopupWindowParams;
            signDocumentAction.Execute += SignDocumentAction_Execute;
        }

        private void SignDocumentAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            try
            {
                // Create a separate object space for the popup
                var popupObjectSpace = Application.CreateObjectSpace(typeof(CustomLogonParameters));
                
                // Create a new CustomLogonParameters object for the popup
                var logonParameters = popupObjectSpace.CreateObject<CustomLogonParameters>();
                
                // Create a detail view for the CustomLogonParameters
                var detailView = Application.CreateDetailView(popupObjectSpace, logonParameters, true);
                
                // Configure the popup window parameters
                e.View = detailView;
                e.DialogController.SaveOnAccept =true;
            
            }
            catch (Exception ex)
            {
                Application.ShowViewStrategy.ShowMessage($"Error configuring signature dialog: {ex.Message}", InformationType.Error);
            }
        }

        private void SignDocumentAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            try
            {
                // Get the CustomLogonParameters from the popup window
                var logonParameters = e.PopupWindowView.CurrentObject as CustomLogonParameters;
                if (logonParameters != null)
                {
                    HandleSignatureAuthentication(logonParameters);
                }
                else
                {
                    Application.ShowViewStrategy.ShowMessage("Invalid authentication data.", InformationType.Error);
                }
            }
            catch (Exception ex)
            {
                Application.ShowViewStrategy.ShowMessage($"Error during signature process: {ex.Message}", InformationType.Error);
            }
        }

        private void HandleSignatureAuthentication(CustomLogonParameters logonParameters)
        {
            try
            {
                // Validate input
                if (!ValidateInput(logonParameters))
                {
                    return;
                }

                // Find the user in the database
                var user = FindUser(logonParameters.UserName);
                if (user == null)
                {
                    Application.ShowViewStrategy.ShowMessage("User not found.", InformationType.Error);
                    return;
                }

               

                // Create the document signature
                if (CreateDocumentSignature(user))
                {
                    Application.ShowViewStrategy.ShowMessage("Document signed successfully.", InformationType.Success);
                }
            }
            catch (Exception ex)
            {
                Application.ShowViewStrategy.ShowMessage($"Error during authentication: {ex.Message}", InformationType.Error);
            }
        }

        private bool ValidateInput(CustomLogonParameters logonParameters)
        {
            if (string.IsNullOrEmpty(logonParameters.UserName))
            {
                Application.ShowViewStrategy.ShowMessage("Please enter a username.", InformationType.Warning);
                return false;
            }

            // Basic password validation - ensure password is not empty

            //if (string.IsNullOrEmpty(logonParameters.Password))
            //{
            //    Application.ShowViewStrategy.ShowMessage("Please enter a password.", InformationType.Warning);
            //    return false;
            //}

            return true;
        }

        private ApplicationUser FindUser(string userName)
        {
            var userCriteria = CriteriaOperator.Parse("UserName = ?", userName);
            return ObjectSpace.FindObject<ApplicationUser>(userCriteria);
        }

        private bool ValidatePassword(ApplicationUser user, string password)
        {
            try
            {
                // Check if the user is active
                if (!user.IsActive)
                {
                    return false;
                }
                
                // Basic validation - ensure password is not empty
                if (string.IsNullOrEmpty(password))
                {
                    return false;
                }
                
                // For demonstration purposes, accept any non-empty password
                // TODO: In production, implement proper password hashing verification
                // You would typically use something like:
                // - BCrypt to verify hashed passwords
                // - The same password hasher used during user registration
                // - Integration with the DevExpress security system's password validation
                
                return password.Length > 0;
            }
            catch (Exception)
            {
                // If password validation fails due to any error, deny access
                return false;
            }
        }

        private bool CreateDocumentSignature(ApplicationUser user)
        {
            try
            {
                // Get the current document
                var document = View.CurrentObject as Document;
                if (document == null)
                {
                    Application.ShowViewStrategy.ShowMessage("No document available for signing.", InformationType.Error);
                    return false;
                }

                // Check if user has already signed this document
                if (HasUserAlreadySigned(document, user))
                {
                    Application.ShowViewStrategy.ShowMessage("You have already signed this document.", InformationType.Warning);
                    return false;
                }

                // Create a new document signature
                var signature = ObjectSpace.CreateObject<DocumentSignature>();
                signature.Document = document;
                signature.User = user;
                signature.Date = DateTime.Now;

                // Save the changes
                ObjectSpace.CommitChanges();
                
                // Refresh the view to show the new signature
                View.Refresh();
                
                return true;
            }
            catch (Exception ex)
            {
                Application.ShowViewStrategy.ShowMessage($"Failed to create document signature: {ex.Message}", InformationType.Error);
                return false;
            }
        }

        private bool HasUserAlreadySigned(Document document, ApplicationUser user)
        {
            var existingSignatureCriteria = CriteriaOperator.Parse("Document = ? AND User = ?", document, user);
            var existingSignature = ObjectSpace.FindObject<DocumentSignature>(existingSignatureCriteria);
            return existingSignature != null;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            UpdateActionState();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            UpdateActionState();
        }

        private void UpdateActionState()
        {
            // Enable the action only when viewing a document
            var hasDocument = View.CurrentObject != null;
            signDocumentAction.Enabled.SetItemValue("HasDocument", hasDocument);
        }
    }
}