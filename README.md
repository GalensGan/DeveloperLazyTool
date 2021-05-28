# developer-lazy-tool

## 简介

该工具出现的原因是这样的：

每次发布前端版本到服务器，都需要经过编译，然后打开远程桌面，拷贝更新的文件到服务器这一过程。

这一过程，虽然简单，但是一通操作下来，挺费时间的，有时间，复制还容易出错。

所以，我就想，要是可以一句命令行来实现就好了~

于是，develop-lazy-tool 就这样诞生了。

## 安装

### 手动

### scoop

## 实现方式

采用MongoDB的查询思路

## 定位

一条命令完成所有任务。

## 程序目录

- data
  存放用户数据

- config
  存放系统配置
  
- system

  系统除配置外的文件

- script
  用户脚本

## 技术点

命令检测
多层级错误检测

## 功能

### 安装(install)

1. 将程序添加到用户变量中

### 卸载(uninstall)

1. 从用户变量中移除本程序
2. 提示删除安装目录

### 打开配置(config)

- 用户配置(--user || -u)

- 系统配置(--system || -s)

### FTP上传(ftp)

**参数说明：**

| 参数名 | 作用                               | 可选 |
| ------ | ---------------------------------- | ---- |
| --name | 指定后，上传 name 相应设置中的文件 | YES  |

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



## ToDo

- [x] 安装 install

- [x] 卸载 uninstall

- [x] 打开配置文件
- [x] ftp上传 ftp
- [ ] 聚合 aggregate
- [ ] 打包并压缩 zip
- [ ] 上传 upload
- [ ] 更新 update 更新自己
