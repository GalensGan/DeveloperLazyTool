# developer-lazy-tool

## 简介

该工具出现的原因是这样的：

每次发布前端版本到服务器，都需要经过编译，然后打开远程桌面，拷贝更新的文件到服务器这一过程。

这一过程，虽然简单，但是一通操作下来，挺费时间的，有时间，复制还容易出错。

所以，我就想，要是可以一句命令行来实现就好了~

于是，develop-lazy-tool 就这样诞生了。

## 安装

### 手动

下载安装包后，在安装包根目录打开命令行，运行 `dlt install` 安装

### scoop

```
scoop bucket add my-bucket https://gitee.com/noctiflorous/galens-bucket.git
scoop install developer-lazy-tool
```

## 卸载

### 手动

```
dlt uninstall
```

然后删除安装目录即可。

## 定位

不重复造轮子，只提供一个聚合方式，使得可以用一条命令完成所有任务。

## 程序目录

- data
  存放用户数据

- config
  存放系统配置
  
- system

  系统除配置外的文件

- script
  所有脚本

## 功能

### 安装(install)

1. 将程序添加到用户变量中

### 卸载(uninstall)

1. 从用户变量中移除本程序
2. 提示删除安装目录

### 打开配置(config)

- 用户配置(--user || -u)
- 系统配置(--system || -s)
- 打开脚本目录(--scriptdir)
- 打开安装目录(--setupdir)

### FTP上传(ftp)

**参数说明：**

| 参数名         | 作用                                               | 可选 | 默认 |
| -------------- | -------------------------------------------------- | ---- | ---- |
| --name \|\| -n | 指定后，上传 name 相应设置中的文件，不区分大小写。 | 是   | 是   |

**配置说明：**

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

### 执行脚本(es)

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

## 列出配置(list)

**参数说明：**

| 参数名         | 作用                                        | 可选 | 默认 |
| -------------- | ------------------------------------------- | ---- | ---- |
| --name \|\| -n | 获取指定名称的配置，比如 `dlt list scripts` | 否   | 是   |

### 聚合

聚合功能要求每项有 name 属性，这样，才能给管道中每次成果命名，方便后面的管道使用。如果不设置 name，则命名为空，不能指定参数名来引用。

## ToDo

- [x] 安装 install
- [x] 卸载 uninstall
- [x] 打开配置文件
- [x] ftp上传 ftp
- [x] 执行脚本 es
- [ ] 聚合 aggregate
- [ ] 更新 update 更新自己
