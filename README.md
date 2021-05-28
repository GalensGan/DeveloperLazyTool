# Developer-lazy-tool

## 实现方式

采用MongoDB的查询思路

## 定位

聚合多个步骤，实现仅一行命令完成所有任务

## 程序目录

- data
  存放用户数据

- config
  存放系统配置
- system
- script
  脚本存放
  存放系统脚本

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

  设置环境变量，方便后续调用
  
- [x] 卸载 uninstall

  1. 将环境变量移除，
  
  删除自己

- [ ] 打开配置文件

- [x] ftp上传 ftp
- [ ] 打包并压缩 package
- [ ] 上传 upload
- [ ] 自定义 custom
- [ ] 更新 update 更新自己
