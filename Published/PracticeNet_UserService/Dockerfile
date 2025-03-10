# Sử dụng runtime ASP.NET Core chính thức
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Đặt thư mục làm thư mục làm việc
WORKDIR /practicenet_userservice

# Sao chép tất cả các file từ thư mục PracticeNet_UserService vào container
COPY publish /practicenet_userservice

# Mở cổng 2222 để truy cập API
EXPOSE 2222

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "UserService.dll"]
