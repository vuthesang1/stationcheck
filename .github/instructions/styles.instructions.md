---
applyTo: '**'
---
Provide project context and coding guidelines that AI should follow when generating code, answering questions, or reviewing changes.

# UI Component Standards for StationCheck Project

## IMPORTANT: Always Use These Components

### Data Display
- ✅ **DxDataGrid** (DevExpress) - For ALL data tables
  - Features: Paging, sorting, filtering, column reorder, export
  - DO NOT use HTML `<table>` for data lists
  
### Forms & Inputs
- ✅ **DxFormLayout** (DevExpress) - For form structure
- ✅ **DxTextBox** (DevExpress) - For text inputs
- ✅ **DxMaskedInput** (DevExpress) - For phone numbers, formatted inputs
- ✅ **DxDateEdit** (DevExpress) - For date/time inputs
- ✅ **DxComboBox** (DevExpress) - For dropdowns
- ✅ **DxCheckBox** (DevExpress) - For checkboxes
- ✅ **DxTextArea** (DevExpress) - For multi-line text

### Modals & Popups
- ✅ **DxPopup** (DevExpress) - For all modals/dialogs
- ❌ DO NOT use Bootstrap modals

### Buttons & Actions
- ✅ **Bootstrap 4.6.2 buttons** - `btn`, `btn-primary`, `btn-danger`, etc.
- ✅ **Font Awesome 6.4.0 icons** - `<i class="fas fa-plus"></i>`

### Layout
- ✅ **SBAdmin 2 template** - Sidebar, topbar, cards
- ✅ **Bootstrap 4.6.2 grid** - `container`, `row`, `col-*`
- ✅ **SBAdmin cards** - `card`, `card-header`, `card-body`

## Code Examples

### Table with DxDataGrid
```razor
<div class="card shadow mb-4">
    <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">Danh sách</h6>
    </div>
    <div class="card-body">
        <DxDataGrid Data="@items"
                    PageSize="20"
                    ShowFilterRow="true"
                    AllowColumnDragDrop="true"
                    ColumnResizeMode="DataGridColumnResizeMode.NextColumn">
            <Columns>
                <DxDataGridColumn Field="@nameof(Item.Id)" Width="80px" />
                <DxDataGridColumn Field="@nameof(Item.Name)" />
                <DxDataGridDateEditColumn Field="@nameof(Item.CreatedAt)" 
                                          Caption="Ngày tạo"
                                          Format="dd/MM/yyyy HH:mm" />
                <DxDataGridCommandColumn Width="200px">
                    <CellDisplayTemplate>
                        <button class="btn btn-sm btn-warning" @onclick="() => Edit(context)">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-sm btn-danger" @onclick="() => Delete(context)">
                            <i class="fas fa-trash"></i>
                        </button>
                    </CellDisplayTemplate>
                </DxDataGridCommandColumn>
            </Columns>
        </DxDataGrid>
    </div>
</div>
```

### Form with DxFormLayout
```razor
<DxPopup @bind-Visible="@showModal"
         HeaderText="Form Title"
         Width="600px">
    <Content>
        <EditForm Model="@formModel" OnValidSubmit="@SaveAsync">
            <DataAnnotationsValidator />
            <DxFormLayout>
                <DxFormLayoutItem Caption="Tên:" ColSpanMd="12">
                    <Template>
                        <DxTextBox @bind-Text="@formModel.Name" />
                    </Template>
                </DxFormLayoutItem>
                
                <DxFormLayoutItem Caption="Số điện thoại:" ColSpanMd="6">
                    <Template>
                        <DxMaskedInput @bind-Value="@formModel.Phone"
                                       Mask="0000-000-000"
                                       MaskMode="MaskedInputMaskMode.RegEx" />
                    </Template>
                </DxFormLayoutItem>
                
                <DxFormLayoutItem Caption="Ngày tạo:" ColSpanMd="6">
                    <Template>
                        <DxDateEdit @bind-Date="@formModel.CreatedAt"
                                    TimeSectionVisible="true" />
                    </Template>
                </DxFormLayoutItem>
                
                <DxFormLayoutItem Caption="Mô tả:" ColSpanMd="12">
                    <Template>
                        <DxTextArea @bind-Text="@formModel.Description"
                                    Rows="3" />
                    </Template>
                </DxFormLayoutItem>
                
                <DxFormLayoutItem Caption="Kích hoạt:" ColSpanMd="12">
                    <Template>
                        <DxCheckBox @bind-Checked="@formModel.IsActive"
                                    Text="Kích hoạt giám sát" />
                    </Template>
                </DxFormLayoutItem>
                
                <DxFormLayoutItem ColSpanMd="12">
                    <Template>
                        <div class="text-right">
                            <DxButton Text="Hủy"
                                      RenderStyle="ButtonRenderStyle.Secondary"
                                      Click="@CloseModal" />
                            <DxButton Text="Lưu"
                                      RenderStyle="ButtonRenderStyle.Primary"
                                      SubmitFormOnClick="true" />
                        </div>
                    </Template>
                </DxFormLayoutItem>
            </DxFormLayout>
        </EditForm>
    </Content>
</DxPopup>
```

### Phone Number Validation
```csharp
public class StationFormModel
{
    [Required(ErrorMessage = "Tên là bắt buộc")]
    public string Name { get; set; } = string.Empty;
    
    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại phải là 10-11 chữ số")]
    public string? ContactPhone { get; set; }
}
```

## File Structure Standards
```
Pages/
  ├── Stations.razor          # Use DxDataGrid
  ├── UserManagement.razor    # Use DxDataGrid
  ├── MyStation.razor         # Use DxDataGrid for schedules
  └── MotionMonitoring.razor  # Use DxDataGrid for alerts

Shared/
  ├── SBAdminLayout.razor     # SBAdmin template
  ├── RedirectToLogin.razor
  └── EmptyLayout.razor

Components/
  └── (reusable DevExpress-based components)
```

## DO NOT
- ❌ Use HTML `<table>` for data grids
- ❌ Use HTML `<input>` - use DxTextBox instead
- ❌ Use HTML `<select>` - use DxComboBox instead
- ❌ Use Bootstrap modals - use DxPopup instead
- ❌ Mix different UI libraries (e.g., Radzen, MudBlazor)

## ALWAYS
- ✅ Use DevExpress components for data and forms
- ✅ Use SBAdmin styling (cards, badges, colors)
- ✅ Use Bootstrap 4.6.2 for layout (grid, spacing)
- ✅ Use Font Awesome 6.4.0 for icons
- ✅ Follow Vietnamese labels and placeholders
- ✅ Add proper validation with DataAnnotations
