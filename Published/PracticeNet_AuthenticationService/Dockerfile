# Sử dụng runtime ASP.NET Core chính thức
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Đặt thư mục làm thư mục làm việc
WORKDIR /practicenet_authenticationservice

# Sao chép tất cả các file từ thư mục practicenet_authenticationservice vào container
COPY publish /practicenet_authenticationservice

# Mở cổng 1111 để truy cập API
EXPOSE 1111

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]
