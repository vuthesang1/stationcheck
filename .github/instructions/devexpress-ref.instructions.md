# Hướng dẫn sử dụng DevExpress Blazor Grid Component

## Tổng quan
DevExpress Blazor Grid (DxGrid) là một component mạnh mẽ để hiển thị và quản lý dữ liệu dạng bảng trong ứng dụng Blazor. Component này cung cấp nhiều tính năng như filtering, sorting, paging, editing, grouping, và nhiều hơn nữa.

## 1. Cài đặt và cấu hình

### 1.1 Package References cần thiết

Thêm các package sau vào file `.csproj`:

```xml
<PackageReference Include="DevExpress.Blazor" VersionOverride="25.1.6" />
<PackageReference Include="DevExpress.AIIntegration.Blazor" VersionOverride="25.1.6" />
<PackageReference Include="Microsoft.EntityFrameworkCore" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" />
```

### 1.2 Cấu hình Services

Trong `Startup.cs` hoặc `Program.cs`, thêm các services cần thiết:

```csharp
// Server-side Blazor
services.AddServerSideBlazor()
    .AddCircuitOptions(x => x.DetailedErrors = detailedErrors)
    .AddHubOptions(options => {
        options.EnableDetailedErrors = detailedErrors;
    });

// DevExpress Services
services.AddDevExpressBlazor();

// Entity Framework (nếu sử dụng)
services.AddDbContextFactory<YourDbContext>(opt => {
    opt.UseSqlServer(connectionString);
    // hoặc opt.UseSqlite(connectionString);
});

// Data Services
services.AddScoped<YourDataService>();
```

### 1.3 Import Namespaces

Trong file `_Imports.razor`:

```razor
@using DevExpress.Blazor
@using DevExpress.Blazor.DocumentMetadata
@using DevExpress.Data.Filtering
@using System.ComponentModel.DataAnnotations
```

### 1.4 CSS và Theme

Trong `_Host.cshtml` hoặc `index.html`:

```html
<!-- DevExpress Theme CSS -->
<link rel="stylesheet" href="_content/DevExpress.Blazor/dx-blazor.css" />

<!-- Custom demo CSS (tùy chọn) -->
<link rel="stylesheet" href="_content/BlazorDemo/css/dx-demo.css" asp-append-version="true" />
<link rel="stylesheet" href="_content/BlazorDemo/css/dx-demo-pages.css" asp-append-version="true" />

<!-- Blazor Framework JS -->
<script src="_framework/blazor.server.js" defer></script>
```

## 2. Cấu trúc cơ bản của Grid

### 2.1 Grid Component đơn giản

```razor
<DxGrid Data="@DataSource" 
        PageSize="15"
        ShowPager="true"
        SizeMode="SizeMode.Medium">
    <Columns>
        <DxGridDataColumn FieldName="Id" Caption="ID" Width="10%" />
        <DxGridDataColumn FieldName="Name" Caption="Tên" MinWidth="200" />
        <DxGridDataColumn FieldName="Email" Caption="Email" Width="25%" />
        <DxGridDataColumn FieldName="CreatedDate" Caption="Ngày tạo" DisplayFormat="dd/MM/yyyy" Width="15%" />
    </Columns>
</DxGrid>

@code {
    IEnumerable<YourDataModel> DataSource { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        DataSource = await YourDataService.GetDataAsync();
    }
}
```

### 2.2 Grid với đầy đủ tính năng

```razor
<div class="grid-container">
    <DxGrid @ref="Grid" 
            Data="@DataSource"
            ShowGroupPanel="true"
            ShowFilterRow="true" 
            ShowSearchBox="true"
            TextWrapEnabled="false"
            AutoExpandAllGroupRows="true"
            ColumnResizeMode="GridColumnResizeMode.NextColumn"
            AllowSelectRowByClick="true"
            HighlightRowOnHover="true"
            VirtualScrollingEnabled="true"
            PageSize="15"
            SizeMode="SizeMode.Medium"
            EditMode="GridEditMode.PopupEditForm"
            ValidationEnabled="true"
            CustomizeEditModel="Grid_CustomizeEditModel"
            EditModelSaving="Grid_EditModelSaving"
            DataItemDeleting="Grid_DataItemDeleting"
            FilterMenuButtonDisplayMode="GridFilterMenuButtonDisplayMode.Always"
            @bind-SearchText="@GridSearchText">
        <Columns>
            <!-- Selection Column -->
            <DxGridSelectionColumn Width="75px" />
            
            <!-- Command Column for Edit/Delete -->
            <DxGridCommandColumn Width="160px" />
            
            <!-- Data Columns -->
            <DxGridDataColumn FieldName="Id" Caption="ID" Width="10%" SortIndex="0" />
            
            <DxGridDataColumn FieldName="Name" Caption="Tên" MinWidth="200" AllowGroup="false">
                <CellDisplayTemplate>
                    <button class="btn btn-link grid-btn-link" 
                            @onclick="() => ShowDetails((YourModel)context.DataItem)">
                        @context.HighlightedDisplayText
                    </button>
                </CellDisplayTemplate>
            </DxGridDataColumn>
            
            <DxGridDataColumn FieldName="CategoryId" Caption="Danh mục" Width="220px" GroupIndex="0">
                <EditSettings>
                    <DxComboBoxSettings Data="Categories" 
                                        ValueFieldName="Id" 
                                        TextFieldName="Name"
                                        SearchMode="ListSearchMode.AutoSearch"
                                        SearchFilterCondition="ListSearchFilterCondition.Contains" />
                </EditSettings>
            </DxGridDataColumn>
            
            <DxGridDataColumn FieldName="Price" Caption="Giá" DisplayFormat="c" Width="15%" />
            
            <DxGridDataColumn FieldName="IsActive" Caption="Kích hoạt" Width="10%" />
            
            <DxGridDataColumn FieldName="CreatedDate" Caption="Ngày tạo" 
                              DisplayFormat="dd/MM/yyyy HH:mm" Width="15%" />
        </Columns>
        
        <!-- Group Summary -->
        <GroupSummary>
            <DxGridSummaryItem FieldName="Id" SummaryType="GridSummaryItemType.Count" />
        </GroupSummary>
        
        <!-- Total Summary -->
        <TotalSummary>
            <DxGridSummaryItem FieldName="Id" SummaryType="GridSummaryItemType.Count" FooterColumnName="Name" />
            <DxGridSummaryItem FieldName="Price" SummaryType="GridSummaryItemType.Sum" FooterColumnName="Price" />
        </TotalSummary>
        
        <!-- Custom Edit Form Template -->
        <EditFormTemplate Context="EditFormContext">
            @{
                var model = (YourModel)EditFormContext.EditModel;
            }
            <DxFormLayout CssClass="w-100">
                <DxFormLayoutItem Caption="Tên:" ColSpanMd="6">
                    @EditFormContext.GetEditor("Name")
                </DxFormLayoutItem>
                <DxFormLayoutItem Caption="Email:" ColSpanMd="6">
                    @EditFormContext.GetEditor("Email")
                </DxFormLayoutItem>
                <DxFormLayoutItem Caption="Danh mục:" ColSpanMd="6">
                    <DxComboBox Data="@Categories"
                                NullText="Chọn danh mục..."
                                SearchMode="ListSearchMode.AutoSearch"
                                SearchFilterCondition="ListSearchFilterCondition.Contains"
                                TextFieldName="Name"
                                ValueFieldName="Id"
                                @bind-Value="@model.CategoryId">
                    </DxComboBox>
                </DxFormLayoutItem>
            </DxFormLayout>
        </EditFormTemplate>
    </DxGrid>
</div>

@code {
    IGrid Grid { get; set; }
    IEnumerable<YourModel> DataSource { get; set; }
    IEnumerable<Category> Categories { get; set; }
    string GridSearchText = "";
    
    protected override async Task OnInitializedAsync()
    {
        DataSource = await YourDataService.GetDataAsync();
        Categories = await YourDataService.GetCategoriesAsync();
    }
    
    void Grid_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
    {
        if (e.IsNew)
        {
            var newItem = (YourModel)e.EditModel;
            newItem.CreatedDate = DateTime.Now;
            newItem.IsActive = true;
        }
    }
    
    async Task Grid_EditModelSaving(GridEditModelSavingEventArgs e)
    {
        var model = (YourModel)e.EditModel;
        if (e.IsNew)
            await YourDataService.AddAsync(model);
        else
            await YourDataService.UpdateAsync(model);
        
        await RefreshData();
    }
    
    async Task Grid_DataItemDeleting(GridDataItemDeletingEventArgs e)
    {
        var model = (YourModel)e.DataItem;
        await YourDataService.DeleteAsync(model.Id);
        await RefreshData();
    }
    
    async Task RefreshData()
    {
        DataSource = await YourDataService.GetDataAsync();
        StateHasChanged();
    }
    
    void ShowDetails(YourModel model)
    {
        // Logic hiển thị chi tiết
    }
}
```

## 3. Các tính năng chính

### 3.1 Filtering (Lọc dữ liệu)

```razor
<DxGrid ShowFilterRow="true" FilterMenuButtonDisplayMode="GridFilterMenuButtonDisplayMode.Always">
    <Columns>
        <DxGridDataColumn FieldName="Name" FilterRowValue='"Tìm kiếm"' 
                          FilterRowOperatorType="GridFilterRowOperatorType.Contains">
            <FilterRowCellTemplate Context="filterContext">
                <DxComboBox @bind-Value="filterContext.FilterCriteria"
                            Data="FilterOptions" 
                            ValueFieldName="Criteria" 
                            TextFieldName="DisplayText"
                            ClearButtonDisplayMode="DataEditorClearButtonDisplayMode.Auto" />
            </FilterRowCellTemplate>
        </DxGridDataColumn>
    </Columns>
</DxGrid>
```

### 3.2 Sorting (Sắp xếp)

```razor
<DxGridDataColumn FieldName="Name" SortIndex="0" SortOrder="GridColumnSortOrder.Ascending" />
```

### 3.3 Grouping (Nhóm dữ liệu)

```razor
<DxGrid ShowGroupPanel="true" AutoExpandAllGroupRows="true">
    <Columns>
        <DxGridDataColumn FieldName="Category" GroupIndex="0" />
    </Columns>
    <GroupSummary>
        <DxGridSummaryItem FieldName="Id" SummaryType="GridSummaryItemType.Count" />
    </GroupSummary>
</DxGrid>
```

### 3.4 Paging và Virtual Scrolling

```razor
<!-- Paging -->
<DxGrid PageSize="20" ShowPager="true" PagerNavigationMode="PagerNavigationMode.InputBox">
</DxGrid>

<!-- Virtual Scrolling -->
<DxGrid VirtualScrollingEnabled="true" ShowAllRows="false">
</DxGrid>
```

### 3.5 Selection (Chọn dòng)

```razor
<DxGrid AllowSelectRowByClick="true" 
        SelectionMode="GridSelectionMode.Multiple"
        @bind-SelectedDataItems="@SelectedItems">
    <Columns>
        <DxGridSelectionColumn Width="75px" />
    </Columns>
</DxGrid>

@code {
    IReadOnlyList<object> SelectedItems { get; set; } = new List<object>();
}
```

### 3.6 Master-Detail

```razor
<DxGrid>
    <DetailRowTemplate Context="dataItem">
        <div style="padding: 20px;">
            <h5>Chi tiết cho: @((YourModel)dataItem).Name</h5>
            <DxGrid Data="@GetDetailData((YourModel)dataItem)">
                <Columns>
                    <DxGridDataColumn FieldName="DetailField1" />
                    <DxGridDataColumn FieldName="DetailField2" />
                </Columns>
            </DxGrid>
        </div>
    </DetailRowTemplate>
</DxGrid>
```

## 4. Styling và CSS Classes

### 4.1 CSS Classes quan trọng

```css
/* Container cho grid */
.grid-container {
    height: 600px; /* Chiều cao cố định cho virtual scrolling */
}

/* Custom button trong cell */
.grid-btn-link {
    padding: 0;
    border: none;
    background: none;
    color: var(--bs-primary);
    text-decoration: none;
}

.grid-btn-link:hover {
    text-decoration: underline;
}

/* Row highlighting */
.table-hover tbody tr:hover {
    background-color: var(--bs-secondary-bg);
}
```

### 4.2 Size Modes

```razor
<!-- Small -->
<DxGrid SizeMode="SizeMode.Small">

<!-- Medium (default) -->
<DxGrid SizeMode="SizeMode.Medium">

<!-- Large -->
<DxGrid SizeMode="SizeMode.Large">
```

## 5. Data Binding

### 5.1 Entity Framework Integration

```csharp
// Service class
public class YourDataService
{
    private readonly IDbContextFactory<YourDbContext> _contextFactory;
    
    public YourDataService(IDbContextFactory<YourDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<IEnumerable<YourModel>> GetDataAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.YourModels
            .Include(x => x.Category)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
    
    public async Task AddAsync(YourModel model)
    {
        using var context = _contextFactory.CreateDbContext();
        context.YourModels.Add(model);
        await context.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(YourModel model)
    {
        using var context = _contextFactory.CreateDbContext();
        context.YourModels.Update(model);
        await context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var item = await context.YourModels.FindAsync(id);
        if (item != null)
        {
            context.YourModels.Remove(item);
            await context.SaveChangesAsync();
        }
    }
}
```

### 5.2 Model Classes

```csharp
public class YourModel
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    
    [Range(0, 9999999)]
    public decimal Price { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedDate { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

## 6. Advanced Features

### 6.1 Custom Templates

```razor
<!-- Header Template -->
<DxGridDataColumn FieldName="Status">
    <HeaderTemplate>
        <div class="d-flex align-items-center">
            <i class="fas fa-flag"></i>
            <span class="ms-2">Trạng thái</span>
        </div>
    </HeaderTemplate>
</DxGridDataColumn>

<!-- Cell Display Template -->
<DxGridDataColumn FieldName="Status">
    <CellDisplayTemplate>
        <div class="d-flex align-items-center">
            <span class="badge @GetStatusBadgeClass(context.Value)">
                @context.DisplayText
            </span>
        </div>
    </CellDisplayTemplate>
</DxGridDataColumn>

<!-- Cell Edit Template -->
<DxGridDataColumn FieldName="Notes">
    <CellEditTemplate>
        <DxMemo @bind-Text="@((string)context.CellValue)" 
                Rows="3" 
                ResizeMode="MemoResizeMode.Vertical" />
    </CellEditTemplate>
</DxGridDataColumn>
```

### 6.2 Export Data

```razor
<DxGrid @ref="Grid" Data="@DataSource">
    <ToolbarTemplate>
        <DxToolbar>
            <Items>
                <DxToolbarItem Text="Export to Excel" 
                               Click="@(() => Grid.ExportToXlsxAsync("data.xlsx"))" />
                <DxToolbarItem Text="Export to CSV" 
                               Click="@(() => Grid.ExportToCsvAsync("data.csv"))" />
            </Items>
        </DxToolbar>
    </ToolbarTemplate>
</DxGrid>
```

### 6.3 Validation

```razor
<DxGrid ValidationEnabled="true" EditMode="GridEditMode.PopupEditForm">
    <Columns>
        <DxGridDataColumn FieldName="Name">
            <EditSettings>
                <DxTextBoxSettings>
                    <DxTextBoxValidationSettings>
                        <RequiredValidator />
                        <StringLengthValidator MaxLength="200" />
                    </DxTextBoxValidationSettings>
                </DxTextBoxSettings>
            </EditSettings>
        </DxGridDataColumn>
        
        <DxGridDataColumn FieldName="Email">
            <EditSettings>
                <DxTextBoxSettings>
                    <DxTextBoxValidationSettings>
                        <RequiredValidator />
                        <EmailValidator />
                    </DxTextBoxValidationSettings>
                </DxTextBoxSettings>
            </EditSettings>
        </DxGridDataColumn>
    </Columns>
</DxGrid>
```

## 7. Performance Tips

### 7.1 Virtual Scrolling cho dữ liệu lớn

```razor
<DxGrid Data="@LargeDataSource" 
        VirtualScrollingEnabled="true" 
        ShowAllRows="false"
        PageSize="50">
</DxGrid>
```

### 7.2 Lazy Loading

```csharp
public async Task<IEnumerable<YourModel>> GetDataAsync(int skip = 0, int take = 50)
{
    using var context = _contextFactory.CreateDbContext();
    return await context.YourModels
        .Skip(skip)
        .Take(take)
        .ToListAsync();
}
```

## 8. Troubleshooting

### 8.1 Các lỗi thường gặp

1. **Grid không hiển thị dữ liệu:**
   - Kiểm tra Data binding
   - Đảm bảo DataSource không null
   - Gọi StateHasChanged() sau khi load dữ liệu

2. **CSS không load:**
   - Kiểm tra thứ tự load CSS
   - Đảm bảo có DevExpress theme CSS
   - Xóa cache browser

3. **Edit không hoạt động:**
   - Kiểm tra EditMode setting
   - Implement các event handler cần thiết
   - Kiểm tra validation

### 8.2 Debug Tips

```csharp
// Log data trong OnInitializedAsync
protected override async Task OnInitializedAsync()
{
    DataSource = await YourDataService.GetDataAsync();
    Console.WriteLine($"Loaded {DataSource?.Count()} items");
    StateHasChanged();
}

// Log trong event handlers
void Grid_EditModelSaving(GridEditModelSavingEventArgs e)
{
    Console.WriteLine($"Saving model: {JsonSerializer.Serialize(e.EditModel)}");
    // Your save logic
}
```

## 9. Best Practices

1. **Luôn set chiều cao cho grid container khi sử dụng Virtual Scrolling**
2. **Sử dụng IDbContextFactory thay vì DbContext trực tiếp**
3. **Implement proper error handling trong data operations**
4. **Sử dụng async/await cho tất cả data operations**
5. **Cache dữ liệu reference (categories, lookup data)**
6. **Sử dụng display formats cho dates và numbers**
7. **Implement proper validation cho edit forms**

## 10. Kết luận

DevExpress Blazor Grid là một component rất mạnh mẽ với nhiều tính năng. Để sử dụng hiệu quả:

- Đọc kỹ documentation
- Test các feature trên demo project
- Implement từng tính năng một cách từ từ
- Quan tâm đến performance với dữ liệu lớn
- Sử dụng proper CSS styling

Với hướng dẫn này, bạn có thể implement một grid hoàn chỉnh với đầy đủ các tính năng cần thiết cho ứng dụng của mình.