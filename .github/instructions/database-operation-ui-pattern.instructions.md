# Database Operation UI Pattern Instructions

## Overview
This instruction file defines the standard UI/UX pattern for all database operations in the StationCheck application.

## Rules

### 0. Grid Component Standard
**Applies to:** All pages displaying tabular data

You MUST use **DxGrid from DevExpress Blazor** for all data tables. Follow the exact configuration pattern from the Stations page.

#### Required DxGrid Configuration:
```razor
<DxGrid @ref="Grid"
        Data="@InstantFeedbackSource" 
        PageSize="20"
        ShowPager="true"
        PagerNavigationMode="PagerNavigationMode.InputBox"
        PageSizeSelectorVisible="true"
        AllowSelectRowByClick="false"
        HighlightRowOnHover="true"
        TextWrapEnabled="false"
        FilterMenuButtonDisplayMode="GridFilterMenuButtonDisplayMode.Always"
        @bind-SearchText="@SearchText"
        ColumnResizeMode="GridColumnResizeMode.NextColumn"
        SizeMode="SizeMode.Medium">
    <Columns>
        <!-- Your columns here -->
    </Columns>
    <ToolbarTemplate>
        <DxToolbar ItemRenderStyleMode="ToolbarRenderStyleMode.Plain">
            <Items>
                <DxToolbarItem BeginGroup="true" Alignment="ToolbarItemAlignment.Right">
                    <Template Context="toolbarContext">
                        <DxTextBox @bind-Text="@SearchText" 
                                   ClearButtonDisplayMode="DataEditorClearButtonDisplayMode.Auto"
                                   NullText="@GetText("your.search_key", "Tìm kiếm...")"
                                   ShowValidationIcon="false"
                                   style="width: 300px;"
                                   @onkeydown:stopPropagation="true">
                        </DxTextBox>
                    </Template>
                </DxToolbarItem>
            </Items>
        </DxToolbar>
    </ToolbarTemplate>
</DxGrid>
```

#### Server-Side Data (EntityInstantFeedbackSource):
```csharp
IGrid Grid { get; set; } = null!;
EntityInstantFeedbackSource InstantFeedbackSource { get; set; } = null!;
string SearchText = string.Empty;

protected override async Task OnInitializedAsync()
{
    var yourService = ScopedServices.GetRequiredService<IYourService>();
    InstantFeedbackSource = new EntityInstantFeedbackSource(e => {
        e.KeyExpression = nameof(YourEntity.Id);
        e.QueryableSource = yourService.GetYourEntitiesQueryable();
    });
}

public void Dispose()
{
    InstantFeedbackSource?.Dispose();
}
```

### 1. Localization Required
**Applies to:** All pages and components

You MUST implement localization for both Vietnamese (vi) and English (en) using the LocalizationStateService.

#### Localization Setup:
```razor
@inject LocalizationStateService LocalizationState

@code {
    // Column captions - reactive properties
    private string YourColumnCaption => GetText("your.column_key", "Tên Cột");
    
    protected override async Task OnInitializedAsync()
    {
        // Detect browser language
        string browserLanguage = "vi";
        try
        {
            var detected = await JS.InvokeAsync<string>("eval", "navigator.language || navigator.userLanguage");
            if (detected.StartsWith("vi"))
                browserLanguage = "vi";
            else if (detected.StartsWith("en"))
                browserLanguage = "en";
            else
                browserLanguage = "vi";
        }
        catch
        {
            browserLanguage = "vi";
        }
        
        // Load from localStorage if exists
        try
        {
            var savedLanguage = await JS.InvokeAsync<string>("localStorage.getItem", "preferredLanguage");
            if (!string.IsNullOrEmpty(savedLanguage))
                browserLanguage = savedLanguage;
        }
        catch { }
        
        // Initialize LocalizationState
        await LocalizationState.InitializeAsync(browserLanguage);
        
        // Initialize your data here...
    }
    
    private string GetText(string key, string defaultText)
    {
        try
        {
            if (LocalizationState?.IsLoaded == true)
            {
                var translation = LocalizationState[key];
                return !string.IsNullOrEmpty(translation) && translation != key ? translation : defaultText;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetText] Error: {ex.Message}");
        }
        return defaultText;
    }
}
```

#### Translation Keys Structure:
Follow this naming convention for translation keys:
- `{page}.page_title` - Page title
- `{page}.list_title` - Grid title
- `{page}.add_button` - Add button
- `{page}.{field}_column` - Grid column captions
- `{page}.{field}_label` - Form field labels
- `{page}.{field}_placeholder` - Input placeholders
- `{page}.edit_title_add` - Add modal title
- `{page}.edit_title_edit` - Edit modal title
- `{page}.actions_column` - Actions column
- `{page}.search_placeholder` - Search box placeholder
- `button.save` - Common save button
- `button.cancel` - Common cancel button
- `message.confirm_delete` - Delete confirmation
- `message.success` - Success message
- `message.error` - Error message

#### Add Translations to Database:
You MUST add translation entries to both Vietnamese and English in the `Translations` table via database seed or manual insert:

```csharp
// Vietnamese translations
new Translation { LanguageCode = "vi", Key = "yourpage.page_title", Value = "Tiêu đề Tiếng Việt", Category = "yourpage" },
new Translation { LanguageCode = "vi", Key = "yourpage.name_column", Value = "Tên", Category = "yourpage" },

// English translations  
new Translation { LanguageCode = "en", Key = "yourpage.page_title", Value = "English Title", Category = "yourpage" },
new Translation { LanguageCode = "en", Key = "yourpage.name_column", Value = "Name", Category = "yourpage" },
```

### 2. Confirmation Dialog Required
**Applies to:** All database operations (Create, Update, Delete)

### 2. Confirmation Dialog Required
**Applies to:** All database operations (Create, Update, Delete)

Before executing ANY database operation, you MUST display a confirmation popup/dialog to the user using **DxPopup**.

#### Confirmation Dialog Requirements:
- **Title:** Clear action description (e.g., "Xác nhận xóa", "Xác nhận cập nhật", "Xác nhận thêm mới")
- **Message:** Specific details about what will be changed/deleted/created
- **Buttons:**
  - Confirm button (e.g., "Xác nhận", "Đồng ý", "OK")
  - Cancel button (e.g., "Hủy", "Cancel")
- **Style:** Use appropriate danger/warning styling for destructive operations (delete)

#### Example for DevExpress Blazor DxPopup:
```razor
<DxPopup @bind-Visible="@showConfirmDialog"
         HeaderText="@GetText("message.confirm_delete_title", "Xác nhận xóa")"
         Width="400px"
         ShowCloseButton="true">
    <BodyContentTemplate>
        <p>@GetText("message.confirm_delete_text", "Bạn có chắc chắn muốn xóa")  <strong>@selectedItem?.Name</strong>?</p>
        <p class="text-danger">@GetText("message.cannot_undo", "Hành động này không thể hoàn tác.")</p>
    </BodyContentTemplate>
    <FooterContentTemplate>
        <DxButton Text="@GetText("button.cancel", "Hủy")" 
                  Click="@(() => showConfirmDialog = false)" 
                  RenderStyle="ButtonRenderStyle.Secondary" />
        <DxButton Text="@GetText("button.confirm", "Xác nhận")" 
                  Click="@ConfirmDelete" 
                  RenderStyle="ButtonRenderStyle.Danger" />
    </FooterContentTemplate>
</DxPopup>
```

### 3. Toast Notification After Success
**Applies to:** All successful database operations

### 3. Toast Notification After Success
**Applies to:** All successful database operations

After a database operation completes successfully, you MUST display a toast notification in the **top-right corner** of the screen using **DxToast**.

#### Toast Notification Requirements:
- **Position:** Top-right corner of the screen
- **Duration:** 3-5 seconds (auto-dismiss)
- **Type:** Success (green/checkmark icon)
- **Message:** Clear success message describing what was accomplished (localized)
- **Styling:** Consistent with application theme

#### Example for DevExpress Blazor Toast:
```razor
@inject IJSRuntime JS

@code {
    private async Task ShowSuccessToast(string message)
    {
        var toastMessage = GetText(message, "Thao tác thành công");
        await JS.InvokeVoidAsync("showToast", "success", toastMessage);
    }
    
    private async Task ShowErrorToast(string message)
    {
        var toastMessage = GetText(message, "Có lỗi xảy ra");
        await JS.InvokeVoidAsync("showToast", "error", toastMessage);
    }
}
```

#### Required JavaScript (wwwroot/js/site.js or similar):
```javascript
window.showToast = function(type, message) {
    // Using a toast library like Toastify or Bootstrap Toast
    // Position: top-right
    // Auto-close: 4000ms
    
    // Example with Toastify:
    Toastify({
        text: message,
        duration: 4000,
        gravity: "top",
        position: "right",
        backgroundColor: type === "success" ? "#28a745" : "#dc3545",
        stopOnFocus: true
    }).showToast();
    
    // Or use Bootstrap Toast, DevExpress Toast, etc.
};
```

### 4. Error Handling
If a database operation fails, also show a toast notification with error details:
- **Type:** Error (red/error icon)
- **Position:** Top-right corner
- **Message:** Clear error message (avoid technical jargon when possible)
- **Duration:** 5-7 seconds (longer for errors)

### 4. Operation Types

#### Create Operations
- Confirmation message: "Bạn có chắc chắn muốn thêm mới [đối tượng] không?"
- Success message: "[Đối tượng] đã được thêm mới thành công"

#### Update Operations
- Confirmation message: "Bạn có chắc chắn muốn cập nhật [đối tượng] không?"
- Success message: "[Đối tượng] đã được cập nhật thành công"

#### Delete Operations
- Confirmation message: "Bạn có chắc chắn muốn xóa [đối tượng] không? Hành động này không thể hoàn tác."
- Success message: "[Đối tượng] đã được xóa thành công"
- Use danger/warning styling

### 5. Bulk Operations
For bulk operations (delete multiple items, update multiple records):
- Confirmation dialog MUST show the count of affected items
- Example: "Bạn có chắc chắn muốn xóa 5 trạm đã chọn không?"
- Success toast should include count: "Đã xóa thành công 5 trạm"

## Implementation Checklist

When implementing or modifying database operations, ensure:

- [ ] Confirmation dialog is shown before operation
- [ ] Dialog has clear title and message
- [ ] Dialog has Cancel and Confirm buttons
- [ ] Success toast appears in top-right corner after successful operation
- [ ] Error toast appears in top-right corner if operation fails
- [ ] Toast messages are clear and user-friendly
- [ ] Toast auto-dismisses after appropriate duration
- [ ] Destructive operations (delete) use warning/danger styling

### 4. Error Handling
If a database operation fails, also show a toast notification with error details:
- **Type:** Error (red/error icon)
- **Position:** Top-right corner
- **Message:** Clear error message (localized, avoid technical jargon when possible)
- **Duration:** 5-7 seconds (longer for errors)

### 5. Operation Types

#### Create Operations
- Confirmation message: `GetText("message.confirm_add", "Bạn có chắc chắn muốn thêm mới [đối tượng] không?")`
- Success message: `GetText("message.add_success", "[Đối tượng] đã được thêm mới thành công")`

#### Update Operations
- Confirmation message: `GetText("message.confirm_update", "Bạn có chắc chắn muốn cập nhật [đối tượng] không?")`
- Success message: `GetText("message.update_success", "[Đối tượng] đã được cập nhật thành công")`

#### Delete Operations
- Confirmation message: `GetText("message.confirm_delete", "Bạn có chắc chắn muốn xóa [đối tượng] không? Hành động này không thể hoàn tác.")`
- Success message: `GetText("message.delete_success", "[Đối tượng] đã được xóa thành công")`
- Use danger/warning styling (ButtonRenderStyle.Danger)

### 6. Bulk Operations
For bulk operations (delete multiple items, update multiple records):
- Confirmation dialog MUST show the count of affected items
- Example: `GetText("message.confirm_delete_multiple", "Bạn có chắc chắn muốn xóa {0} trạm đã chọn không?")`
- Success toast should include count: `GetText("message.delete_multiple_success", "Đã xóa thành công {0} trạm")`

## Implementation Checklist

When implementing or modifying database operations, ensure:

- [ ] Using DxGrid with EntityInstantFeedbackSource for server-side data
- [ ] DxGrid configured with all required properties (PageSize=20, ShowPager, FilterMenuButtonDisplayMode, etc.)
- [ ] Search toolbar with DxTextBox in top-right corner
- [ ] LocalizationStateService injected and initialized
- [ ] All UI text uses GetText() method with translation keys
- [ ] Translation keys added to database for both Vietnamese (vi) and English (en)
- [ ] Column captions are reactive properties using GetText()
- [ ] Confirmation DxPopup shown before operation
- [ ] DxPopup has clear localized title and message
- [ ] DxPopup has Cancel and Confirm buttons with proper styling
- [ ] Success toast appears in top-right corner after successful operation
- [ ] Error toast appears in top-right corner if operation fails
- [ ] Toast messages are localized and user-friendly
- [ ] Toast auto-dismisses after appropriate duration
- [ ] Destructive operations (delete) use ButtonRenderStyle.Danger
- [ ] InstantFeedbackSource.Refresh() called after data changes
- [ ] Component implements IDisposable and disposes InstantFeedbackSource

## Complete Implementation Example

### Page Structure (YourPage.razor)
```razor
@page "/yourpage"
@using StationCheck.Models
@using StationCheck.Interfaces
@using DevExpress.Data.Linq
@inherits OwningComponentBase
@implements IDisposable
@inject IYourService YourService
@inject IJSRuntime JS
@inject LocalizationStateService LocalizationState

<PageTitle>@GetText("yourpage.page_title", "Quản lý Your Entity")</PageTitle>

<!-- Page Heading -->
<div class="d-sm-flex align-items-center justify-content-between mb-4">
    <h1 class="h3 mb-0 text-gray-800">@GetText("yourpage.page_title", "Quản lý Your Entity")</h1>
    <button class="btn btn-sm btn-primary shadow-sm" @onclick="ShowAddModal">
        <i class="fas fa-plus fa-sm text-white-50"></i> @GetText("yourpage.add_button", "Thêm Mới")
    </button>
</div>

<!-- DxGrid -->
<div class="card shadow mb-4">
    <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">@GetText("yourpage.list_title", "Danh sách")</h6>
    </div>
    <div class="card-body">
        <DxGrid @ref="Grid"
                Data="@InstantFeedbackSource" 
                PageSize="20"
                ShowPager="true"
                PagerNavigationMode="PagerNavigationMode.InputBox"
                PageSizeSelectorVisible="true"
                AllowSelectRowByClick="false"
                HighlightRowOnHover="true"
                TextWrapEnabled="false"
                FilterMenuButtonDisplayMode="GridFilterMenuButtonDisplayMode.Always"
                @bind-SearchText="@SearchText"
                ColumnResizeMode="GridColumnResizeMode.NextColumn"
                SizeMode="SizeMode.Medium">
            <Columns>
                <DxGridDataColumn FieldName="Id" Caption="ID" Visible="false" />
                <DxGridDataColumn FieldName="Name" Caption="@NameColumnCaption" />
                <DxGridDataColumn FieldName="Description" Caption="@DescriptionColumnCaption" />
                
                <DxGridDataColumn FieldName="Id" Caption="@GetText("yourpage.actions_column", "Thao tác")" Width="150px" AllowSort="false">
                    <CellDisplayTemplate>
                        @{
                            var itemId = (Guid)context.Value;
                        }
                        <div class="d-flex justify-content-center gap-1">
                            <button class="btn btn-warning btn-sm" 
                                    title="@GetText("yourpage.edit_tooltip", "Chỉnh sửa")" 
                                    @onclick="@(() => ShowEditModalAsync(itemId))">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="btn btn-danger btn-sm" 
                                    title="@GetText("yourpage.delete_tooltip", "Xóa")" 
                                    @onclick="@(() => ShowDeleteConfirmation(itemId))">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    </CellDisplayTemplate>
                </DxGridDataColumn>
            </Columns>
            <ToolbarTemplate>
                <DxToolbar ItemRenderStyleMode="ToolbarRenderStyleMode.Plain">
                    <Items>
                        <DxToolbarItem BeginGroup="true" Alignment="ToolbarItemAlignment.Right">
                            <Template Context="toolbarContext">
                                <DxTextBox @bind-Text="@SearchText" 
                                           ClearButtonDisplayMode="DataEditorClearButtonDisplayMode.Auto"
                                           NullText="@GetText("yourpage.search_placeholder", "Tìm kiếm...")"
                                           ShowValidationIcon="false"
                                           style="width: 300px;"
                                           @onkeydown:stopPropagation="true">
                                </DxTextBox>
                            </Template>
                        </DxToolbarItem>
                    </Items>
                </DxToolbar>
            </ToolbarTemplate>
        </DxGrid>
    </div>
</div>

<!-- Add/Edit Modal -->
<DxPopup @bind-Visible="@showModal"
         HeaderText="@(editingItem == null ? GetText("yourpage.edit_title_add", "Thêm Mới") : GetText("yourpage.edit_title_edit", "Chỉnh sửa"))"
         ShowCloseButton="true"
         Width="600px"
         MaxWidth="90%"
         CloseOnOutsideClick="false">
    <BodyContentTemplate>
        <EditForm Model="formModel" OnValidSubmit="SaveItem">
            <DataAnnotationsValidator />
            <DxFormLayout>
                <DxFormLayoutItem Caption="@GetText("yourpage.name_label", "Tên")" ColSpanMd="12" CaptionPosition="CaptionPosition.Vertical">
                    <Template>
                        <DxTextBox @bind-Text="@formModel.Name" 
                                   NullText="@GetText("yourpage.name_placeholder", "Nhập tên")"
                                   ShowValidationIcon="true">
                        </DxTextBox>
                        <ValidationMessage For="@(() => formModel.Name)" />
                    </Template>
                </DxFormLayoutItem>
                
                <DxFormLayoutItem ColSpanMd="12">
                    <Template>
                        <div class="d-flex justify-content-end mt-3">
                            <DxButton Text="@GetText("button.cancel", "Hủy")" 
                                      RenderStyle="ButtonRenderStyle.Secondary"
                                      Click="@CloseModal"
                                      CssClass="me-2" />
                            <DxButton Text="@GetText("button.save", "Lưu")" 
                                      RenderStyle="ButtonRenderStyle.Primary"
                                      RenderStyleMode="ButtonRenderStyleMode.Contained"
                                      SubmitFormOnClick="true" />
                        </div>
                    </Template>
                </DxFormLayoutItem>
            </DxFormLayout>
        </EditForm>
    </BodyContentTemplate>
</DxPopup>

<!-- Delete Confirmation Modal -->
<DxPopup @bind-Visible="@showDeleteDialog"
         HeaderText="@GetText("message.confirm_delete_title", "Xác nhận xóa")"
         Width="400px"
         ShowCloseButton="true">
    <BodyContentTemplate>
        <p>@GetText("message.confirm_delete_text", "Bạn có chắc chắn muốn xóa") <strong>@selectedItem?.Name</strong>?</p>
        <p class="text-danger">@GetText("message.cannot_undo", "Hành động này không thể hoàn tác.")</p>
    </BodyContentTemplate>
    <FooterContentTemplate>
        <DxButton Text="@GetText("button.cancel", "Hủy")" 
                  Click="@(() => showDeleteDialog = false)" 
                  RenderStyle="ButtonRenderStyle.Secondary" />
        <DxButton Text="@GetText("button.confirm", "Xác nhận xóa")" 
                  Click="@ConfirmDelete" 
                  RenderStyle="ButtonRenderStyle.Danger" />
    </FooterContentTemplate>
</DxPopup>

@code {
    // Grid properties
    IGrid Grid { get; set; } = null!;
    EntityInstantFeedbackSource InstantFeedbackSource { get; set; } = null!;
    string SearchText = string.Empty;
    
    // Modal state
    private bool showModal = false;
    private bool showDeleteDialog = false;
    private YourEntity? editingItem = null;
    private YourEntity? selectedItem = null;
    private YourFormModel formModel = new();
    
    // Column captions - reactive properties
    private string NameColumnCaption => GetText("yourpage.name_column", "Tên");
    private string DescriptionColumnCaption => GetText("yourpage.description_column", "Mô tả");
    
    protected override async Task OnInitializedAsync()
    {
        // Detect browser language
        string browserLanguage = "vi";
        try
        {
            var detected = await JS.InvokeAsync<string>("eval", "navigator.language || navigator.userLanguage");
            browserLanguage = detected.StartsWith("vi") ? "vi" : detected.StartsWith("en") ? "en" : "vi";
        }
        catch { }
        
        // Load from localStorage
        try
        {
            var savedLanguage = await JS.InvokeAsync<string>("localStorage.getItem", "preferredLanguage");
            if (!string.IsNullOrEmpty(savedLanguage))
                browserLanguage = savedLanguage;
        }
        catch { }
        
        // Initialize LocalizationState
        await LocalizationState.InitializeAsync(browserLanguage);
        
        // Initialize InstantFeedbackSource
        var service = ScopedServices.GetRequiredService<IYourService>();
        InstantFeedbackSource = new EntityInstantFeedbackSource(e => {
            e.KeyExpression = nameof(YourEntity.Id);
            e.QueryableSource = service.GetYourEntitiesQueryable();
        });
    }
    
    private void ShowAddModal()
    {
        editingItem = null;
        formModel = new YourFormModel();
        showModal = true;
    }
    
    private async Task ShowEditModalAsync(Guid itemId)
    {
        var item = await YourService.GetByIdAsync(itemId);
        if (item != null)
        {
            editingItem = item;
            formModel = new YourFormModel
            {
                Name = item.Name,
                // Map other properties...
            };
            showModal = true;
        }
    }
    
    private void CloseModal()
    {
        showModal = false;
        editingItem = null;
        formModel = new YourFormModel();
    }
    
    private async Task SaveItem()
    {
        try
        {
            if (editingItem == null)
            {
                // Create new
                var newItem = new YourEntity
                {
                    Name = formModel.Name,
                    // Map other properties...
                };
                await YourService.CreateAsync(newItem);
                await ShowSuccessToast("message.add_success", "Thêm mới thành công");
            }
            else
            {
                // Update existing
                editingItem.Name = formModel.Name;
                // Update other properties...
                await YourService.UpdateAsync(editingItem);
                await ShowSuccessToast("message.update_success", "Cập nhật thành công");
            }
            
            InstantFeedbackSource?.Refresh();
            CloseModal();
        }
        catch (Exception ex)
        {
            await ShowErrorToast("message.error", $"Lỗi: {ex.Message}");
        }
    }
    
    private async Task ShowDeleteConfirmation(Guid itemId)
    {
        selectedItem = await YourService.GetByIdAsync(itemId);
        if (selectedItem != null)
        {
            showDeleteDialog = true;
        }
    }
    
    private async Task ConfirmDelete()
    {
        showDeleteDialog = false;
        
        if (selectedItem == null) return;
        
        try
        {
            await YourService.DeleteAsync(selectedItem.Id);
            InstantFeedbackSource?.Refresh();
            await ShowSuccessToast("message.delete_success", "Xóa thành công");
        }
        catch (Exception ex)
        {
            await ShowErrorToast("message.error", $"Lỗi khi xóa: {ex.Message}");
        }
        finally
        {
            selectedItem = null;
        }
    }
    
    private async Task ShowSuccessToast(string key, string defaultText)
    {
        var message = GetText(key, defaultText);
        await JS.InvokeVoidAsync("showToast", "success", message);
    }
    
    private async Task ShowErrorToast(string key, string defaultText)
    {
        var message = GetText(key, defaultText);
        await JS.InvokeVoidAsync("showToast", "error", message);
    }
    
    private string GetText(string key, string defaultText)
    {
        try
        {
            if (LocalizationState?.IsLoaded == true)
            {
                var translation = LocalizationState[key];
                return !string.IsNullOrEmpty(translation) && translation != key ? translation : defaultText;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetText] Error: {ex.Message}");
        }
        return defaultText;
    }
    
    public void Dispose()
    {
        InstantFeedbackSource?.Dispose();
    }
    
    public class YourFormModel
    {
        [Required(ErrorMessage = "Tên là bắt buộc")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        // Add other properties with validation...
    }
}
}
```

## Required Translations

Add these translation keys to the database for every page that performs database operations:

### Vietnamese (vi)
```sql
INSERT INTO Translations (LanguageCode, [Key], Value, Category, CreatedAt, CreatedBy) VALUES
-- Page titles
('vi', 'yourpage.page_title', 'Quản lý Your Entity', 'yourpage', GETDATE(), 'System'),
('vi', 'yourpage.list_title', 'Danh sách', 'yourpage', GETDATE(), 'System'),
('vi', 'yourpage.add_button', 'Thêm Mới', 'yourpage', GETDATE(), 'System'),

-- Grid columns
('vi', 'yourpage.name_column', 'Tên', 'yourpage', GETDATE(), 'System'),
('vi', 'yourpage.description_column', 'Mô tả', 'yourpage', GETDATE(), 'System'),
('vi', 'yourpage.actions_column', 'Thao tác', 'yourpage', GETDATE(), 'System'),

-- Form labels
('vi', 'yourpage.name_label', 'Tên', 'yourpage', GETDATE(), 'System'),
('vi', 'yourpage.name_placeholder', 'Nhập tên', 'yourpage', GETDATE(), 'System'),

-- Modal titles
('vi', 'yourpage.edit_title_add', 'Thêm Mới', 'yourpage', GETDATE(), 'System'),
('vi', 'yourpage.edit_title_edit', 'Chỉnh sửa', 'yourpage', GETDATE(), 'System'),

-- Tooltips
('vi', 'yourpage.edit_tooltip', 'Chỉnh sửa', 'yourpage', GETDATE(), 'System'),
('vi', 'yourpage.delete_tooltip', 'Xóa', 'yourpage', GETDATE(), 'System'),

-- Search
('vi', 'yourpage.search_placeholder', 'Tìm kiếm...', 'yourpage', GETDATE(), 'System'),

-- Common messages
('vi', 'message.confirm_delete_title', 'Xác nhận xóa', 'message', GETDATE(), 'System'),
('vi', 'message.confirm_delete_text', 'Bạn có chắc chắn muốn xóa', 'message', GETDATE(), 'System'),
('vi', 'message.cannot_undo', 'Hành động này không thể hoàn tác.', 'message', GETDATE(), 'System'),
('vi', 'message.add_success', 'Thêm mới thành công', 'message', GETDATE(), 'System'),
('vi', 'message.update_success', 'Cập nhật thành công', 'message', GETDATE(), 'System'),
('vi', 'message.delete_success', 'Xóa thành công', 'message', GETDATE(), 'System'),
('vi', 'message.error', 'Có lỗi xảy ra', 'message', GETDATE(), 'System'),

-- Common buttons
('vi', 'button.save', 'Lưu', 'button', GETDATE(), 'System'),
('vi', 'button.cancel', 'Hủy', 'button', GETDATE(), 'System'),
('vi', 'button.confirm', 'Xác nhận', 'button', GETDATE(), 'System');
```

### English (en)
```sql
INSERT INTO Translations (LanguageCode, [Key], Value, Category, CreatedAt, CreatedBy) VALUES
-- Page titles
('en', 'yourpage.page_title', 'Manage Your Entity', 'yourpage', GETDATE(), 'System'),
('en', 'yourpage.list_title', 'List', 'yourpage', GETDATE(), 'System'),
('en', 'yourpage.add_button', 'Add New', 'yourpage', GETDATE(), 'System'),

-- Grid columns
('en', 'yourpage.name_column', 'Name', 'yourpage', GETDATE(), 'System'),
('en', 'yourpage.description_column', 'Description', 'yourpage', GETDATE(), 'System'),
('en', 'yourpage.actions_column', 'Actions', 'yourpage', GETDATE(), 'System'),

-- Form labels
('en', 'yourpage.name_label', 'Name', 'yourpage', GETDATE(), 'System'),
('en', 'yourpage.name_placeholder', 'Enter name', 'yourpage', GETDATE(), 'System'),

-- Modal titles
('en', 'yourpage.edit_title_add', 'Add New', 'yourpage', GETDATE(), 'System'),
('en', 'yourpage.edit_title_edit', 'Edit', 'yourpage', GETDATE(), 'System'),

-- Tooltips
('en', 'yourpage.edit_tooltip', 'Edit', 'yourpage', GETDATE(), 'System'),
('en', 'yourpage.delete_tooltip', 'Delete', 'yourpage', GETDATE(), 'System'),

-- Search
('en', 'yourpage.search_placeholder', 'Search...', 'yourpage', GETDATE(), 'System'),

-- Common messages
('en', 'message.confirm_delete_title', 'Confirm Delete', 'message', GETDATE(), 'System'),
('en', 'message.confirm_delete_text', 'Are you sure you want to delete', 'message', GETDATE(), 'System'),
('en', 'message.cannot_undo', 'This action cannot be undone.', 'message', GETDATE(), 'System'),
('en', 'message.add_success', 'Added successfully', 'message', GETDATE(), 'System'),
('en', 'message.update_success', 'Updated successfully', 'message', GETDATE(), 'System'),
('en', 'message.delete_success', 'Deleted successfully', 'message', GETDATE(), 'System'),
('en', 'message.error', 'An error occurred', 'message', GETDATE(), 'System'),

-- Common buttons
('en', 'button.save', 'Save', 'button', GETDATE(), 'System'),
('en', 'button.cancel', 'Cancel', 'button', GETDATE(), 'System'),
('en', 'button.confirm', 'Confirm', 'button', GETDATE(), 'System');
```

## Notes
- This pattern applies to ALL pages that perform database operations
- Consistency is key - always use the same position (top-right) and duration
- Make messages user-friendly and in Vietnamese (primary language)
- For bilingual support, use localization keys for all messages
