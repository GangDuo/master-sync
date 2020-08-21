# master-sync

## データベース接続情報

Master.Sync.exeと同じフォルダに、下記ファイルを配置する。

```xml:db.config
<?xml version="1.0" encoding="utf-8"?>
<appSettings>
    <add key="db:server" value= "{server address}"/>
    <add key="db:port" value="{port}"/>
    <add key="db:user" value= "{user name}"/>
    <add key="db:password" value= "{password}"/>
    <add key="db:database" value= "{database}"/>
</appSettings>
```





