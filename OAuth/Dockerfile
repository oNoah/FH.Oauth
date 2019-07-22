FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
COPY . .
EXPOSE 80
ENV OAUTH_ISSUER="http://172.16.89.91:5000" #环境变量,OAuth地址,注意事项 ENV两种写法一种是等于， 一种是空格
# 编译放到CI/CD中间件中处理
# 1.设置此文件的属性 自动拷贝到生成目录
# 2.只复制当前目录内容到镜像中,命令：docker build -t=fh/oauth .
# 3.中间件再执行启动容器 命令：docker run --name fh-oauth -it -p 5000:5000 fh/oauth
# 
ENTRYPOINT ["dotnet", "OAuth.dll"]