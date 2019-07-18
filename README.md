# FH.OAuth
认证中心

IdentityServer4 是为ASP.NET Core 2.系列量身打造的一款基于 OpenID Connect 和 OAuth 2.0 认证框架。

参考资料：

IdentityServer4介绍:

https://www.cnblogs.com/stulzq/p/8119928.html

OpenID Connect介绍：

https://www.jianshu.com/p/be7cc032a4e9
https://www.jianshu.com/p/be7cc032a4e9

  OpenID Connect是基于OAuth 2.0规范族的可互操作的身份验证协议。
它使用简单的REST / JSON消息流来实现，和之前任何一种身份认证协议相比，开发者可以轻松集成。

OpenID：Authentication 认证

OAuth ：Authorization   授权


OAuth2.0介绍：

http://www.ruanyifeng.com/blog/2019/04/oauth_design.html
http://www.ruanyifeng.com/blog/2019/04/oauth-grant-types.html

1.OAuth2.0是什么？
  流行的授权机制，一种授权协议,用来授权第三方应用，获取第三方数据。
OAuth 就是一种授权机制。数据的所有者告诉系统，同意授权第三方应用进入系统，获取这些数据。
系统从而产生一个短期的进入令牌（token），用来代替密码，供第三方应用使用

2.令牌与密码对比
  令牌是短期的，到期会自动失效，用户自己无法修改。密码一般长期有效，用户不修改，就不会发生变化。
  令牌可以被数据所有者撤销，会立即失效。密码一般不允许被他人撤销。
  令牌有权限范围（scope），对于网络服务来说，只读令牌就比读写令牌更安全。密码一般是完整权限。

3.四种授权类型

  授权码（authorization-code）
  隐藏式（implicit）
  密码式（password）：
  客户端凭证（client credentials）

----------------------------------------------------
Oauth创建迁移脚本 (EF-Framework迁移脚本)


-- PersistedGrantDbContext
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb

-- ConfigurationDbContext
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb

-- ApplicationDbContext
dotnet ef migrations add CreateIdentitySchema -c ApplicationDbContext -o Data/Migrations/Users
