# Take note
gRPC does not work when deploying in IIS

# Migrate DB
cmd Project DB, then run: dotnet ef migrations add [namemigrations] .

Finally, run : dotnet ef database update

# 🚀 Deploy ASP.NET Core API with Docker

## 📌 Prerequisites
Make sure you have installed:
- [Docker](https://www.docker.com/get-started)
- [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet)

*This script only support to run folder build of application

## 🛠 Step 1: Publish ASP.NET Core API
Before deploying to Docker, publish your API:
```sh
dotnet publish -c Release -o ./publish
```
This will generate the `publish` folder containing all necessary files.

---

## 🛠 Step 2: Create `Dockerfile`
Inside your project folder, create a `Dockerfile` with the following content:

```dockerfile
# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published files into the container
COPY publish .

# Expose the port (adjust as needed)
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "YourProjectName.dll"]
```
📌 **Replace `YourProjectName.dll`** with the actual DLL name inside `publish/`.

---

## 🛠 Step 3: Build the Docker Image
Run the following command to build the Docker image:
```sh
docker build -t myaspnetcoreapi .
```

---

## 🛠 Step 4: Run the Docker Container
Run the container and map ports:
```sh
docker run -d -p 8080:8080 --name myapi myaspnetcoreapi
```
📌 This maps **port 8080 on your machine** to **port 8080 inside the container**.

---

## 🛠 Step 5: Verify the Deployment
Check if the container is running:
```sh
docker ps
```
Access the API:
```sh
http://localhost:8080/swagger
```

---

## 🛠 Step 6: Manage the Container
- **Stop the container:**
  ```sh
  docker stop myapi
  ```
- **Restart the container:**
  ```sh
  docker start myapi
  ```
- **Remove the container:**
  ```sh
  docker rm myapi
  ```

---

## 🛠 Step 7: Push to Docker Hub (Optional)
- **Tag your image:**
  ```sh
  docker tag myaspnetcoreapi yourdockerhubusername/myaspnetcoreapi
  ```
- **Login to Docker Hub:**
  ```sh
  docker login
  ```
- **Push the image:**
  ```sh
  docker push yourdockerhubusername/myaspnetcoreapi
  ```
- **Pull the image anywhere:**
  ```sh
  docker pull yourdockerhubusername/myaspnetcoreapi
  ```

---

## 🎯 Summary
✔ **Publish** the API with `dotnet publish`
✔ **Create a `Dockerfile`** with runtime setup
✔ **Build the image** using `docker build`
✔ **Run the container** using `docker run`
✔ **Verify API access** via `http://localhost:8080`
✔ **Manage** the container with `docker ps`, `docker stop`, `docker start`
✔ *(Optional)* **Push the image** to Docker Hub

---

Now your ASP.NET Core API is running inside Docker! 🚀🔥

