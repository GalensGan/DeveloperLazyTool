# developer-lazy-tool

## 简介

`developer-lazy-tool` 简称 `dlt` ，它的目的是将复杂的操作步骤聚合成一条终端语句，让程序员过上 ”幸福生活“（治愈懒癌晚期）。

该工具出现的原因是这样的：

每次发布前端版本到服务器，总需要下列步骤：

1. 进入到程序目录，运行 `npm run build:prod`编译
2. 远程桌面到服务器
3. 找到编译结果，拷贝到服务器指定目录

这一过程，虽然简单，但是一通操作下来，挺费时间的，复制的时候还容易出错。

所以，我就想，要是可以一句命令行来实现就好了~

于是，develop-lazy-tool 承载着这样的使命便诞生了！

## 安装

### 手动

下载安装包后，在安装包根目录打开命令行，运行 `dlt install` 安装

### scoop

```
scoop bucket add my-bucket https://gitee.com/galensgan/galens-bucket.git
scoop install developer-lazy-tool
```

## 卸载

### 手动

```
dlt uninstall
```

然后删除安装目录即可。

### scoop

```
scoop uninstsall developer-lazy-tool
```

## 定位

不重复造轮子，只提供一个聚合方式，使得可以用一条命令完成所有任务。

## 程序目录

| 目录名  | 说明             |
| ------- | ---------------- |
| data    | 用户数据文件目录 |
| config  | 系统配置文件     |
| plugins | 插件目录         |

## 程序架构

`developer-lazy-tool` 采用插件式开发，主要三部分组成：

1. 内核程序，命名空间为 `DeveloperLazyTool.Core`
2. 插件接口，命名空间为 `DeveloperLazyTool.Plugin`
3. 一系列插件

## 插件

对外提供的所有功能均由插件实现。插件位于 `plugins` 目录下。在用户配置文件中，插件配置对应的配置字段名为 `命令`+`s`。

例 `plugin` 命令对应的配置字段名为 `plugins`。

有一些命令不需要配置即可运行。

### 系统插件

系统插件为 `DLT_Plugins_System.dll`,该插件提供的功能如下：

| 命令      | 作用                   |
| --------- | ---------------------- |
| config    | 快速查看配置文件       |
| explorer  | 快速打开目录或者文件   |
| install   | 安装本程序（手动安装） |
| uninstall | 卸载本程序（手动安装） |
| list      | 查看某个命令的所有配置 |
| plugin    | 插件管理               |

### 其它插件

#### FTP 插件

该插件只包含一个命令，即 `ftp`，其配置字段名为 `ftps`。

**使用方式:**

```powershell
dlt ftp <name>
```

**配置示例:**

``` json
"ftps": [
    {
      "name": "front-dist",
      "host": "192.168.23.11",
      "port": 21,
      "username": "test",
      "password": "whfy8888",
      "localPath": "E:\\galensShare\\Develop\\swToolsFrontEnd\\dist\\",
      "remotePath": "/"
    }
  ]
```

**配置说明:**

| 名称       | 说明                         |
| ---------- | ---------------------------- |
| name       | 调用时的名称                 |
| host       | ftp 的 ip 地址               |
| port       | ftp 端口号，默认 21          |
| username   | ftp 用户名                   |
| password   | ftp 密码                     |
| localPath  | 待上传的本地文件路径         |
| remotePath | 远程存放目录，`/` 代表根目录 |

#### Minio 插件

该插件只包含一个命令，即 `minio`，其配置字段名为 `minios`。

**使用方式:**

```
dlt minio <name> -p <path1>[path2,...]
```

**配置示例:**

``` json
"minios": [
    {
      "name": "img",
      "endpoint": "yourdomain.com",
      "accessKey": "user",
      "secretKey": "xxxx",
      "region": "",
      "sessionToken": "",
      "useSSL": true,
      "bucketName": "public",
      "objectDir": "files/images"
    }
  ]
```

**配置说明:**

| 名称                | 说明                |
| ------------------- | ------------------- |
| name                | 调用时的名称        |
| endpoint            | minio 域名，必须    |
| accessKey           | minio 的 accessKey  |
| secretKey           | minio 的 secrectKey |
| region/sessionToken | minio 的配置        |
| useSSL              | 是否使用 https      |
| bucketName          | 桶名称              |
| objectDir           | 对象存储目录        |

#### 执行脚本(es)插件

**动词说明：**

`es` 是 `execute script` 的缩写，该动词可以省略。假设配置中有名为 `zip-Dlt`，则完整命令行：

``` 
dlt es --name zip-Dlt
```

简化的命令行：

```
dlt zip-Dlt
```

**参数说明：**

| 参数名         | 作用                                               | 可选 | 默认 |
| -------------- | -------------------------------------------------- | ---- | ---- |
| --name \|\| -n | 指定后，执行 name 相应配置中的脚本，不区分大小写。 | 否   | 是   |

**配置说明：**

执行根目录默认在 script 目录，暂不支持修改.

``` json
"scripts": [
    // 可以直接运行命令
    // 使用 7z 打包 developer-lazy-tool
    {
      "name": "zip-Dlt",
      "fileName": "7z",
      "arguments": "a -t7z E:/DeveloperLazyTool/bin/Release/developer-lazy-tool.7z E:/DeveloperLazyTool/bin/Release/*",
      "successFlag": "Everything is Ok"
    },
    // 也可以运行脚本文件
    {
      "name": "cd-backend",
      "fileName": "cd-backend.ahk",
    }    
  ],
```

#### 管道插件

聚合功能要求每项有 name 属性，这样，才能给管道中每次成果命名，方便后面的管道使用。如果不设置 name，则命名为空，不能指定参数名来引用。

## ToDo

- [x] 安装 install
- [x] 卸载 uninstall
- [x] 打开配置文件
- [x] ftp上传 ftp
- [x] 执行脚本 es
- [ ] 管道
- [ ] 更新 update 更新自己
- [ ] Minio 插件上传添加进度条
- [ ] 增加类似 `z` 的功能
- [ ] 美化命令行输出
