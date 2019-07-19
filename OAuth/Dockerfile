FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
COPY . .
EXPOSE 80
# 编译放到CI/CD中间件中处理
# 1.拉取发布成功后将此文件拷贝到publish文件中手动创建镜像
# 2.只复制当前目录内容到镜像中,命令：docker build -t=fh/oauth .
# 3.中间件再执行启动容器 命令：docker run --name fh-oauth -it -p 5000:80 fh/oauth
# 
ENTRYPOINT ["dotnet", "OAuth.dll"]