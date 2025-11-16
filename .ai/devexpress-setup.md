# DevExpress Blazor Setup Guide

## Option 1: Trial Version (30 Days Free)

### Step 1: Get Trial NuGet Feed URL

1. Visit https://nuget.devexpress.com/#feed-url
2. Click "Obtain Feed URL" button
3. Fill in your email (no credit card required)
4. Copy the feed URL you receive (format: `https://nuget.devexpress.com/{YOUR-FEED-KEY}/api`)

### Step 2: Register Feed

```bash
dotnet nuget add source https://nuget.devexpress.com/{YOUR-FEED-KEY}/api -n DevExpressTrial
```

## Option 2: Paid License

Run this command (replace `{YOUR-KEY}` with your actual authorization key):

```bash
dotnet nuget add source https://nuget.devexpress.com/{YOUR-KEY}/api/v3/index.json -n DXFeed
```

### 2. Install DevExpress.Blazor Package

```bash
dotnet add package DevExpress.Blazor
```

### 3. Register DevExpress Services in Program.cs

Add to `Program.cs`:

```csharp
builder.Services.AddDevExpressBlazor();
```

### 4. Add DevExpress Resources to _Host.cshtml

Add CSS:
```html
<link href="_content/DevExpress.Blazor.Themes/blazing-berry.bs5.css" rel="stylesheet" />
```

Add JS (before `</body>`):
```html
<script src="_content/DevExpress.Blazor/dx-blazor.js"></script>
```

### 5. Update _Imports.razor

Add:
```razor
@using DevExpress.Blazor
```

## Component Usage Standards

### DxDataGrid
Use for all data tables with these features:
- Server-side paging
- Sorting
- Filtering
- Column reordering (drag-drop)
- Export to Excel/CSV

Example:
```razor
<DxDataGrid Data="@stations"
            PageSize="20"
            ShowFilterRow="true"
            ColumnResizeMode="DataGridColumnResizeMode.NextColumn"
            AllowColumnDragDrop="true">
    <Columns>
        <DxDataGridColumn Field="@nameof(Station.Id)" Caption="ID" Width="80px" />
        <DxDataGridColumn Field="@nameof(Station.Name)" Caption="Tên Trạm" />
        <DxDataGridColumn Field="@nameof(Station.CreatedAt)" Caption="Ngày tạo">
            <CellDisplayTemplate>
                @context.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            </CellDisplayTemplate>
        </DxDataGridColumn>
    </Columns>
</DxDataGrid>
```

### DxFormLayout
Use for forms instead of raw HTML forms:
```razor
<DxFormLayout>
    <DxFormLayoutItem Caption="Tên trạm:" ColSpanMd="12">
        <DxTextBox @bind-Text="@stationForm.Name" />
    </DxFormLayoutItem>
</DxFormLayout>
```

### DxPopup
Use for modals instead of Bootstrap modals:
```razor
<DxPopup @bind-Visible="@showModal"
         HeaderText="@(editingStation == null ? "Thêm Trạm" : "Sửa Trạm")"
         Width="600px">
    <Content>
        <!-- Form content -->
    </Content>
</DxPopup>
```

## Reference
- Docs: https://docs.devexpress.com/Blazor/
- Demos: https://demos.devexpress.com/blazor/
- API: https://docs.devexpress.com/Blazor/DevExpress.Blazor
