# BaseCore Fashion E-Commerce Solution

## Overview

BaseCore là một ứng dụng thương mại điện tử đa dịch vụ với kiến trúc microservices.
Nó bao gồm:
- `BaseCore.ApiGateway`: API Gateway sử dụng Ocelot.
- `BaseCore.APIService`: Dịch vụ backend chính xử lý sản phẩm, giỏ hàng, đơn hàng, review, hình ảnh.
- `BaseCore.AuthService`: Dịch vụ xác thực, đăng nhập, đăng ký và quản lý người dùng.
- `BaseCore.StatsService`: Dịch vụ thống kê Python FastAPI.
- `BaseCore.WebClient`: Frontend React + Vite.
- `BaseCore.Repository`, `BaseCore.Services`, `BaseCore.Common`, `BaseCore.DTO`, `BaseCore.Entities`, `BaseCore.Libs`: thư viện chia sẻ dùng bởi backend.

> Mục tiêu: cài đặt nhanh, chạy được toàn bộ hệ thống và dùng giao diện web cho BaseCore.

## Yêu cầu trước khi cài đặt

- Windows 10/11 hoặc hệ điều hành tương thích với SQL Server.
- .NET SDK 8.0 hoặc mới hơn.
- SQL Server / LocalDB.
- Node.js v18+ (để chạy frontend React + Vite).
- Python 3.12+ nếu muốn chạy `BaseCore.StatsService`.

## Dự án và cổng dịch vụ

| Dự án | Mô tả | Cổng mặc định |
|---|---|---|
| `BaseCore.ApiGateway` | Gateway Ocelot chuyển tiếp các request đến dịch vụ nội bộ | `http://localhost:5000` |
| `BaseCore.APIService` | Backend chính xử lý sản phẩm, đơn hàng, giỏ hàng | `http://localhost:5001` |
| `BaseCore.AuthService` | Auth service xử lý JWT, login/register, người dùng, role | `http://localhost:5002` |
| `BaseCore.StatsService` | Python FastAPI service cho thống kê | `http://localhost:5003` |
| `BaseCore.WebClient` | Frontend React + Tailwind | `http://localhost:3000` |

## Cấu hình cơ sở dữ liệu

Tập tin cấu hình chính cho SQL Server nằm ở:
- `BaseCore.APIService/appsettings.json`
- `BaseCore.AuthService/appsettings.json`

Mặc định, cả hai đều dùng chuỗi kết nối:

```json
"Data Source=localhost;Initial Catalog=BaseCoreSales;Integrated Security=True;Trust Server Certificate=True"
```

Nếu bạn dùng SQL Server khác, hãy cập nhật `ConnectedDb` trong cả hai tệp.

### Khởi tạo cơ sở dữ liệu

Khi chạy `BaseCore.APIService` và `BaseCore.AuthService`, ứng dụng sẽ tự động gọi `db.Database.Migrate()`.
Dịch vụ `BaseCore.APIService` cũng seed một tài khoản admin mặc định:

- `username`: `admin`
- `password`: `admin`

## Cài đặt và chạy backend

### 1. Cài đặt .NET SDK

Tải và cài đặt .NET 8 từ:
https://dotnet.microsoft.com/en-us/download/dotnet/8.0

### 2. Chạy API Service

Mở terminal tại thư mục gốc và chạy:

```powershell
cd BaseCore.APIService
dotnet restore
dotnet run --project BaseCore.APIService.csproj
```

Service sẽ chạy ở `http://localhost:5001`.

### 3. Chạy Auth Service

Mở terminal mới và chạy:

```powershell
cd BaseCore.AuthService
dotnet restore
dotnet run --project BaseCore.AuthService.csproj
```

Service sẽ chạy ở `http://localhost:5002`.

### 4. Chạy API Gateway

Mở terminal mới và chạy:

```powershell
cd BaseCore.ApiGateway
dotnet restore
dotnet run --project BaseCore.ApiGateway.csproj
```

Gateway sẽ chạy ở `http://localhost:5000`.

### 5. Chạy Stats Service (tuỳ chọn)

Nếu bạn muốn dùng dịch vụ thống kê Python, cài Python và thư viện:

```powershell
cd BaseCore.StatsService
python -m venv .venv
.\.venv\Scripts\Activate.ps1
python -m pip install --upgrade pip
python -m pip install -r requirements.txt
uvicorn main:app --host 0.0.0.0 --port 5003
```

> `BaseCore.ApiGateway` cấu hình proxy có thể chuyển các yêu cầu `/api/external-brands` tới service `5004`, nhưng `StatsService` chính thức chạy ở cổng `5003`.

## Cài đặt và chạy frontend

### 1. Cài đặt Node.js

Tải Node.js từ:
https://nodejs.org/

### 2. Chạy frontend

```powershell
cd BaseCore.WebClient
npm install
npm run dev
```

Frontend sẽ khởi chạy ở `http://localhost:3000`.

### 3. Proxy API

Frontend cấu hình proxy Vite để chuyển các request:
- `/api` → `http://localhost:5000`
- `/images` → `http://localhost:5000`

Do đó, khi frontend gọi `/api/...`, request sẽ đi qua `BaseCore.ApiGateway`.

## Cấu trúc ảnh và thư mục media

Dịch vụ backend `BaseCore.APIService` phục vụ tệp ảnh từ thư mục `Media`:
- `Media/products`

Khi dịch vụ khởi động, nếu thư mục chưa tồn tại, ứng dụng sẽ tự tạo thư mục `Media/products`.

## Tính năng chính

- Kiến trúc microservices với API Gateway.
- Xác thực JWT và role-based access.
- Quản lý sản phẩm, danh mục, giỏ hàng, đơn hàng, coupon, đánh giá.
- Thư viện chia sẻ giữa backend và các dự án khác.
- Frontend React + Tailwind + Vite.
- Dịch vụ thống kê Python FastAPI.

## Sử dụng nhanh

1. Mở SQL Server và đảm bảo `BaseCoreSales` khả dụng.
2. Mở các terminal để chạy:
   - `BaseCore.APIService`
   - `BaseCore.AuthService`
   - `BaseCore.ApiGateway`
   - `BaseCore.WebClient`
3. Truy cập `http://localhost:3000`.
4. Đăng nhập bằng tài khoản admin:
   - `admin` / `admin`

## Tư vấn triển khai lên GitHub

- Đặt `README.md` này ở thư mục gốc của repo.
- Đảm bảo không commit thông tin bí mật như `SecretKey` hoặc chuỗi kết nối thực tế.
- Nếu triển khai môi trường production, bạn cần:
  - Cập nhật chuỗi kết nối cơ sở dữ liệu.
  - Đặt biến môi trường JWT secrets.
  - Bổ sung HTTPS và cấu hình `UseHttpsRedirection()`.

## Ghi chú thêm

- Cả `BaseCore.APIService` và `BaseCore.AuthService` dùng `Integrated Security=True` theo cấu hình mặc định.
- Nếu cần đổi sang SQL Server authentication, hãy sửa `Data Source`, `Initial Catalog`, `User ID` và `Password` trong `appsettings.json`.
- Nếu bạn muốn build frontend production, dùng `npm run build` trong `BaseCore.WebClient`.

---

Cảm ơn bạn đã sử dụng BaseCore. Chúc bạn triển khai và thử nghiệm thành công!
