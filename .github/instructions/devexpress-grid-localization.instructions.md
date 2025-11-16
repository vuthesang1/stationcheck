# DevExpress Grid & Localization Pattern - Instruction Guide

> **Based on**: `Pages/Stations.razor`  
> **Purpose**: Standard pattern for creating CRUD pages with DevExpress Grid and multi-language support  
> **Version**: 1.0  
> **Last Updated**: Nov 5, 2025

---

## 📋 Table of Contents
1. [Page Structure](#page-structure)
2. [Required Dependencies](#required-dependencies)
3. [Localization Setup](#localization-setup)
4. [DevExpress Grid Configuration](#devexpress-grid-configuration)
5. [Modal Form Pattern](#modal-form-pattern)
6. [Translation Keys Convention](#translation-keys-convention)
7. [Complete Example](#complete-example)

---

## 1. Page Structure

### File Organization
```
Pages/
  └── [EntityName].razor
      ├── @page directive
      ├── Using statements
      ├── Dependency injection
      ├── Page title with localization
      ├── UI markup
      └── @code block
```

### Required Directives & Usings
```csharp
@page "/[entity-name]"
@using System.ComponentModel.DataAnnotations
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using StationCheck.Models
@using StationCheck.Interfaces
@using StationCheck.Services
@using DevExpress.Data.Linq
@inherits OwningComponentBase
@implements IDisposable
@attribute [Authorize(Roles = "Admin")]  // or appropriate role
```

### Required Injections
```csharp
@inject I[Entity]Service [Entity]Service
@inject IJSRuntime JS
@inject LocalizationStateService LocalizationState
```

---

## 2. Required Dependencies

### Service Interface
```csharp
public interface I[Entity]Service
{
    IQueryable<[Entity]> Get[Entities]Queryable();
    Task<[Entity]?> Get[Entity]ByIdAsync(int id);
    Task<[Entity]> Create[Entity]Async([Entity] entity);
    Task Update[Entity]Async(int id, [Entity] entity);
    Task Delete[Entity]Async(int id);
}
```

### Model Requirements
- Must have `Id` property as primary key
- Should have `CreatedAt`, `ModifiedAt` timestamps
- Must have validation attributes on FormModel

---

## 3. Localization Setup

### A. Page Title
```csharp
<PageTitle>@GetText("[entity].page_title", "Default Title")</PageTitle>
```

### B. Initialize Localization in OnInitializedAsync
```csharp
protected override async Task OnInitializedAsync()
{
    // 1. Detect browser language
    string browserLanguage = "vi";
    try
    {
        var detected = await JS.InvokeAsync<string>("eval", 
            "navigator.language || navigator.userLanguage");
        
        if (detected.StartsWith("vi"))
            browserLanguage = "vi";
        else if (detected.StartsWith("en"))
            browserLanguage = "en";
        else
            browserLanguage = "vi";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[PageName] Error detecting language: {ex.Message}");
        browserLanguage = "vi";
    }
    
    // 2. Load from localStorage if exists
    try
    {
        var savedLanguage = await JS.InvokeAsync<string>(
            "localStorage.getItem", "preferredLanguage");
        if (!string.IsNullOrEmpty(savedLanguage))
        {
            browserLanguage = savedLanguage;
        }
    }
    catch { }
    
    // 3. Initialize LocalizationState
    await LocalizationState.InitializeAsync(browserLanguage);
    
    // 4. Initialize grid data source
    var service = ScopedServices.GetRequiredService<I[Entity]Service>();
    InstantFeedbackSource = new EntityInstantFeedbackSource(e => {
        e.KeyExpression = nameof([Entity].Id);
        e.QueryableSource = service.Get[Entities]Queryable();
    });
}
```

### C. Reactive Column Captions
```csharp
// Use property getters for reactive translation updates
private string NameColumnCaption => GetText("[entity].name_column", "Default");
private string DescriptionColumnCaption => GetText("[entity].description_column", "Default");
private string StatusColumnCaption => GetText("[entity].status_column", "Default");
// ... other columns
```

### D. GetText Helper Method
```csharp
private string GetText(string key, string defaultText)
{
    try
    {
        if (LocalizationState?.IsLoaded == true)
        {
            var translation = LocalizationState[key];
            return !string.IsNullOrEmpty(translation) && translation != key 
                ? translation 
                : defaultText;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[GetText] Error: {ex.Message}");
    }
    return defaultText;
}
```

### E. Trigger UI Update After Render
```csharp
private bool _hasInitialized = false;

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender && !_hasInitialized)
    {
        _hasInitialized = true;
        StateHasChanged();
    }
}
```

---

## 4. DevExpress Grid Configuration

### A. Grid Properties Setup
```csharp
@code {
    // Grid references and data
    IGrid Grid { get; set; } = null!;
    EntityInstantFeedbackSource InstantFeedbackSource { get; set; } = null!;
    string SearchText = string.Empty;
}
```

### B. Grid Markup Structure
```razor
<div class="card shadow mb-4">
    <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">
            @GetText("[entity].list_title", "Default List Title")
        </h6>
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
                <!-- Column definitions here -->
            </Columns>
            
            <ToolbarTemplate>
                <!-- Toolbar content here -->
            </ToolbarTemplate>
        </DxGrid>
    </div>
</div>
```

### C. Column Definitions

#### Standard Data Column
```razor
<DxGridDataColumn FieldName="Name" 
                  Caption="@NameColumnCaption" 
                  MinWidth="150" />
```

#### Date Column with Format
```razor
<DxGridDataColumn FieldName="CreatedAt" 
                  Caption="@CreatedAtColumnCaption" 
                  DisplayFormat="dd/MM/yyyy" 
                  Visible="false" />
```

#### Actions Column with Buttons
```razor
<DxGridDataColumn FieldName="Id" 
                  Caption="@GetText('[entity].actions_column', 'Actions')" 
                  Width="200px" 
                  AllowSort="false">
    <CellDisplayTemplate>
        @{
            var entityId = (int)context.Value;
        }
        <div class="d-flex justify-content-center gap-1">
            <button class="btn btn-warning btn-sm" 
                    title="@GetText('[entity].edit_tooltip', 'Edit')" 
                    @onclick="@(() => ShowEditModalAsync(entityId))">
                <i class="fas fa-edit"></i>
            </button>
            <button class="btn btn-danger btn-sm" 
                    title="@GetText('[entity].delete_tooltip', 'Delete')" 
                    @onclick="@(() => Delete[Entity](entityId))">
                <i class="fas fa-trash"></i>
            </button>
        </div>
    </CellDisplayTemplate>
</DxGridDataColumn>
```

### D. Toolbar with Search
```razor
<ToolbarTemplate>
    <DxToolbar ItemRenderStyleMode="ToolbarRenderStyleMode.Plain">
        <Items>
            <DxToolbarItem BeginGroup="true" 
                          Alignment="ToolbarItemAlignment.Right">
                <Template Context="toolbarContext">
                    <DxTextBox @bind-Text="@SearchText" 
                               ClearButtonDisplayMode="DataEditorClearButtonDisplayMode.Auto"
                               NullText="@GetText('[entity].search_placeholder', 'Search...')"
                               ShowValidationIcon="false"
                               style="width: 300px;">
                    </DxTextBox>
                </Template>
            </DxToolbarItem>
        </Items>
    </DxToolbar>
</ToolbarTemplate>
```

---

## 5. Modal Form Pattern

### A. Page Header with Add Button
```razor
<div class="d-sm-flex align-items-center justify-content-between mb-4">
    <h1 class="h3 mb-0 text-gray-800">
        @GetText("[entity].page_title", "Default Title")
    </h1>
    <button class="d-none d-sm-inline-block btn btn-sm btn-primary shadow-sm" 
            @onclick="ShowAddModal">
        <i class="fas fa-plus fa-sm text-white-50"></i> 
        @GetText("[entity].add_button", "Add New")
    </button>
</div>
```

### B. Modal State Properties
```csharp
@code {
    private bool showModal = false;
    private [Entity]? editing[Entity] = null;
    private [Entity]FormModel [entity]Form = new();
}
```

### C. Modal Popup Structure
```razor
<DxPopup @bind-Visible="@showModal"
         HeaderText="@(editing[Entity] == null 
             ? GetText('[entity].edit_title_add', 'Add New') 
             : GetText('[entity].edit_title_edit', 'Edit'))"
         ShowCloseButton="true"
         Width="600px"
         MaxWidth="90%"
         CloseOnOutsideClick="false">
    <BodyContentTemplate Context="popupContext">
        <EditForm Model="[entity]Form" OnValidSubmit="Save[Entity]" Context="formContext">
            <DataAnnotationsValidator />
            <DxFormLayout CssClass="w-100">
                
                <!-- Form fields here -->
                
                <!-- Action buttons -->
                <DxFormLayoutItem ColSpanMd="12">
                    <Template>
                        <div class="d-flex justify-content-end mt-3">
                            <DxButton Text="@GetText('button.cancel', 'Cancel')" 
                                      RenderStyle="ButtonRenderStyle.Secondary"
                                      Click="@CloseModal"
                                      CssClass="me-2" />
                            <DxButton Text="@GetText('button.save', 'Save')" 
                                      RenderStyle="ButtonRenderStyle.Primary"
                                      RenderStyleMode="ButtonRenderStyleMode.Contained"
                                      SubmitFormOnClick="true"
                                      IconCssClass="fas fa-save" />
                        </div>
                    </Template>
                </DxFormLayoutItem>
            </DxFormLayout>
        </EditForm>
    </BodyContentTemplate>
</DxPopup>
```

### D. Form Field Types

#### Text Input
```razor
<DxFormLayoutItem Caption="@GetText('[entity].name_label', 'Name')" 
                  ColSpanMd="12" 
                  CaptionPosition="CaptionPosition.Vertical">
    <Template>
        <DxTextBox @bind-Text="@[entity]Form.Name" 
                   NullText="@GetText('[entity].name_placeholder', 'Enter name')"
                   ShowValidationIcon="true">
        </DxTextBox>
        <ValidationMessage For="@(() => [entity]Form.Name)" />
    </Template>
</DxFormLayoutItem>
```

#### Textarea (Memo)
```razor
<DxFormLayoutItem Caption="@GetText('[entity].description_label', 'Description')" 
                  ColSpanMd="12" 
                  CaptionPosition="CaptionPosition.Vertical">
    <Template>
        <DxMemo @bind-Text="@[entity]Form.Description"
                Rows="3"
                NullText="@GetText('[entity].description_placeholder', 'Enter description')">
        </DxMemo>
    </Template>
</DxFormLayoutItem>
```

#### Masked Input (Phone)
```razor
<DxFormLayoutItem Caption="@GetText('[entity].phone_label', 'Phone')" 
                  ColSpanMd="6" 
                  CaptionPosition="CaptionPosition.Vertical">
    <Template>
        <DxMaskedInput @bind-Value="@[entity]Form.Phone"
                       Mask="0000-000-000"
                       NullText="@GetText('[entity].phone_placeholder', 'Enter phone')"
                       ShowValidationIcon="true">
        </DxMaskedInput>
        <ValidationMessage For="@(() => [entity]Form.Phone)" />
    </Template>
</DxFormLayoutItem>
```

#### Checkbox
```razor
<DxFormLayoutItem Caption="@GetText('[entity].active_label', 'Active')" 
                  ColSpanMd="12" 
                  CaptionPosition="CaptionPosition.Vertical">
    <Template>
        <DxCheckBox @bind-Checked="@[entity]Form.IsActive" />
    </Template>
</DxFormLayoutItem>
```

### E. CRUD Operations

#### Show Add Modal
```csharp
private void ShowAddModal()
{
    editing[Entity] = null;
    [entity]Form = new [Entity]FormModel { IsActive = true };
    showModal = true;
}
```

#### Show Edit Modal
```csharp
private async Task ShowEditModalAsync(int [entity]Id)
{
    var [entity] = await [Entity]Service.Get[Entity]ByIdAsync([entity]Id);
    if ([entity] != null)
    {
        editing[Entity] = [entity];
        [entity]Form = new [Entity]FormModel
        {
            Name = [entity].Name,
            Description = [entity].Description,
            IsActive = [entity].IsActive
            // ... map other properties
        };
        showModal = true;
    }
}
```

#### Close Modal
```csharp
private void CloseModal()
{
    showModal = false;
    editing[Entity] = null;
}
```

#### Save (Create/Update)
```csharp
private async Task Save[Entity]()
{
    try
    {
        if (editing[Entity] == null)
        {
            // Create new
            var new[Entity] = new [Entity]
            {
                Name = [entity]Form.Name,
                Description = [entity]Form.Description,
                IsActive = [entity]Form.IsActive
                // ... map other properties
            };
            await [Entity]Service.Create[Entity]Async(new[Entity]);
        }
        else
        {
            // Update existing
            editing[Entity].Name = [entity]Form.Name;
            editing[Entity].Description = [entity]Form.Description;
            editing[Entity].IsActive = [entity]Form.IsActive;
            // ... update other properties
            
            await [Entity]Service.Update[Entity]Async(editing[Entity].Id, editing[Entity]);
        }

        InstantFeedbackSource?.Refresh();
        CloseModal();
    }
    catch (Exception ex)
    {
        await JS.InvokeVoidAsync("alert", 
            $"{GetText('message.error', 'Error')}: {ex.Message}");
    }
}
```

#### Delete with Confirmation
```csharp
private async Task Delete[Entity](int [entity]Id)
{
    var confirmed = await JS.InvokeAsync<bool>("confirm", 
        GetText("message.confirm_delete_[entity]", 
                "Are you sure you want to delete this item?"));
    
    if (confirmed)
    {
        try
        {
            await [Entity]Service.Delete[Entity]Async([entity]Id);
            InstantFeedbackSource?.Refresh();
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("alert", 
                $"{GetText('message.delete_error', 'Delete error')}: {ex.Message}");
        }
    }
}
```

### F. Form Model with Validation
```csharp
public class [Entity]FormModel
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [RegularExpression(@"^[\d\-]+$", ErrorMessage = "Invalid phone format")]
    [MaxLength(20)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;
}
```

### G. Dispose Pattern
```csharp
public void Dispose()
{
    InstantFeedbackSource?.Dispose();
}
```

---

## 6. Translation Keys Convention

### Required Keys per Entity

#### Page Level
```
[entity].page_title          - Page title and header
[entity].list_title          - Grid card header
[entity].add_button          - Add new button text
```

#### Grid Columns
```
[entity].name_column         - Name column header
[entity].description_column  - Description column header
[entity].status_column       - Status column header
[entity].created_column      - Created date column header
[entity].modified_column     - Modified date column header
[entity].actions_column      - Actions column header
[entity].search_placeholder  - Search box placeholder
```

#### Modal Form
```
[entity].edit_title_add      - Add modal title
[entity].edit_title_edit     - Edit modal title
[entity].name_label          - Name field label
[entity].name_placeholder    - Name field placeholder
[entity].description_label   - Description field label
[entity].description_placeholder
[entity].phone_label
[entity].phone_placeholder
[entity].active_label
```

#### Actions & Tooltips
```
[entity].edit_tooltip        - Edit button tooltip
[entity].delete_tooltip      - Delete button tooltip
[entity].manage_[related]_tooltip  - Related entity button
```

#### Messages
```
message.confirm_delete_[entity]  - Delete confirmation
message.error                    - General error
message.delete_error             - Delete error
```

#### Common Buttons (Shared across all pages)
```
button.save     - Save button
button.cancel   - Cancel button
button.edit     - Edit button
button.delete   - Delete button
```

### DbSeeder Entry Format
```csharp
// Vietnamese
new Translation { 
    LanguageCode = "vi", 
    Key = "[entity].page_title", 
    Value = "Tiếng Việt Text", 
    Category = "[entity]", 
    CreatedAt = DateTime.UtcNow 
},

// English
new Translation { 
    LanguageCode = "en", 
    Key = "[entity].page_title", 
    Value = "English Text", 
    Category = "[entity]", 
    CreatedAt = DateTime.UtcNow 
},
```

---

## 7. Complete Example

See `Pages/Stations.razor` for the complete working implementation following all patterns above.

### Key Files to Reference
- **Page**: `Pages/Stations.razor`
- **Service Interface**: `Interfaces/IStationService.cs`
- **Service Implementation**: `Services/StationService.cs`
- **Model**: `Models/Station.cs`
- **Localization Service**: `Services/LocalizationStateService.cs`
- **Translations Seeder**: `Data/DbSeeder.cs`

---

## 🎯 Checklist for New Page

- [ ] Add all `@using` statements and directives
- [ ] Inject required services (Service, JS, LocalizationState)
- [ ] Setup `EntityInstantFeedbackSource` in `OnInitializedAsync`
- [ ] Initialize localization with browser detection
- [ ] Create reactive column caption properties
- [ ] Configure DxGrid with all required properties
- [ ] Add toolbar with search box
- [ ] Create all column definitions with localized captions
- [ ] Implement actions column with buttons
- [ ] Create modal form with DxPopup and EditForm
- [ ] Add all form fields with proper components
- [ ] Implement CRUD methods (Add, Edit, Delete)
- [ ] Create FormModel class with validation
- [ ] Add GetText helper method
- [ ] Implement Dispose method
- [ ] Add all translation keys to DbSeeder (VI + EN)
- [ ] Test language switching functionality
- [ ] Verify all tooltips and placeholders translate
- [ ] Test CRUD operations end-to-end

---

## 📚 Additional Resources

- **DevExpress Blazor Documentation**: https://docs.devexpress.com/Blazor/
- **DxGrid API Reference**: https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid
- **DxFormLayout API**: https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxFormLayout

---

**Last Updated**: November 5, 2025  
**Maintained by**: StationCheck Development Team
