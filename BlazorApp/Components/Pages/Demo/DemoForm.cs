using BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlazorApp.Components.Pages.Demo
{
    public partial class DemoForm : ComponentBase
    {
        private DemoFormVM model = new();

        private EditContext? editContext;
        private ValidationMessageStore? messageStore;

        [Inject] // Dependency Injection
        private NavigationManager? NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            editContext = new EditContext(model);
            messageStore = new ValidationMessageStore(editContext);

            model.CheckboxOptions.AddRange(new List<SelectListItem> {
                new() { Text = "Option 1", Value = "Value1", Selected = true },
                new() { Text = "Option 2", Value = "Value2" },
                new() { Text = "Option 3", Value = "Value3", Selected = true }
            });
        }

        public void HandleValidSubmit()
        {
            if (messageStore == null || editContext == null)
                throw new Exception("Something went Pete Tong");

            bool isValid = true; // Assume the data is valid
            messageStore.Clear(); // Clear existing messages

            // Custom validation logic

            if (model.BirthDate == null)
            {
                messageStore.Add(editContext.Field(nameof(model.BirthDate)), "Date of Birth is a required field.");
                isValid = false;
            }

            // If the data does not pass validation, update the UI accordingly

            if (!isValid)
            {
                editContext.NotifyValidationStateChanged();
                return;
            }

            // TODO... Process the data

            // Navigate to a new page

            NavigationManager?.NavigateTo("/");
        }
    }
}
