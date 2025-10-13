# Cash Plus Web Application

A web application built with ASP.NET Core 3.1 MVC.

## Prerequisites

- [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet/3.1)
- Visual Studio 2019+ or Visual Studio Code
- SQL Server (Local or Express)

## Project Structure

```plaintext
IOITWebApp31/
├── Controllers/         # MVC Controllers
├── Models/             # Data models and view models
├── Views/              # Razor views
├── wwwroot/           # Static files (CSS, JS, images)
│   ├── css/           # CSS files
│   ├── js/            # JavaScript files
│   └── images/        # Image files
└── appsettings.json   # Application configuration
```

## Cài đặt và Chạy Ứng dụng

### Cài đặt

1. Clone repository về máy của bạn:
   ```bash
   git clone <repository-url>
   cd cpl_corporate_web
   ```

2. Khôi phục các packages:
   dotnet restore IOITCore.sln

### Chạy ứng dụng

1. Chạy ứng dụng bằng .NET CLI:
   1.cd IOITWebApp31
   2.dotnet run 
   dotnet watch run --project ./IOITWebApp31.csproj 
   // Chạy không hot reload (.net core 6.1 < chưa hỗ trợ)
   dotnet watch run --no-hot-reload --launch-profile "IOITWebApp31" --project ./IOITWebApp31.csproj 
   

2. Hoặc chạy bằng Visual Studio:
   - Mở file `IOITCore.sln` trong Visual Studio
   - Nhấn F5 hoặc click vào nút Start để chạy ứng dụng

3. Truy cập ứng dụng tại: 
`https://localhost:5001` hoặc `http://localhost:5000`
 - Thay đổi host:
        File: appsettings.json 
         "ConnectionStrings": {
            "DefaultConnection": ... ,
                                 "urls" : "http://localhost:xxxx"
            
## Build Ứng dụng

### Build cho môi trường Development
dotnet build IOITCore.sln

### Build cho môi trường Production

dotnet build IOITCore.sln --configuration Release

### Publish ứng dụng
dotnet publish IOITWebApp31/IOITWebApp31.csproj -c Release -o ./publish

## Làm việc với CSS

### Thêm file CSS mới

1. Tạo file CSS mới trong thư mục `IOITWebApp31/wwwroot/css/`:
   # Ví dụ tạo file style.css
   touch IOITWebApp31/wwwroot/css/style.css

2. Hoặc sử dụng Visual Studio/VS Code để tạo file mới trong thư mục `wwwroot/css/`

### Sử dụng CSS trong View

1. Thêm reference đến file CSS trong layout hoặc view cụ thể:
   <link rel="stylesheet" href="~/css/style.css" />

2. Để sử dụng CSS cho một trang cụ thể, thêm vào phần head của trang:
   @section Styles {
       <link rel="stylesheet" href="~/css/style.css" />
   }

### Sử dụng CSS từ thư viện bên ngoài

1. Tải file CSS vào thư mục `wwwroot/css/`
2. Hoặc sử dụng CDN và thêm reference trong layout:
   <link rel="stylesheet" href="https://cdn.example.com/library.css" />

## Cấu hình Database

1. Cập nhật connection string trong file `appsettings.json`:
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CashPlusDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }

2. Chạy migrations để cập nhật database:
   dotnet ef database update

3. Tạo migration mới khi thay đổi model:
   dotnet ef migrations add MigrationName

## Deployment

### Triển khai lên IIS

1. Publish ứng dụng:
   dotnet publish IOITWebApp31/IOITWebApp31.csproj -c Release -o ./publish

2. Cấu hình IIS:
   - Tạo website mới trong IIS
   - Trỏ đường dẫn vật lý đến thư mục publish
   - Cấu hình Application Pool sử dụng .NET Core

### Triển khai lên Azure
dotnet publish IOITWebApp31/IOITWebApp31.csproj -c Release
az webapp up --name YourAppName --resource-group YourResourceGroup

