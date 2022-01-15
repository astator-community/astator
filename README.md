# astator
astator的目标是使用c#作为脚本的安卓自动化软件

<br/>

## 功能
- [x] 脚本UI (使用安卓原生) 
- [ ] 控件操作
- [x] 图色操作
- [x] OCR (未实现部分: x86)
- [x] 代码混淆
- [x] apk打包
- [ ] 代码编辑器 (优先级低, 编辑器深坑)
- [x] nuget包引用
- [x] vscode插件
- [ ] 其他

<br/>

## 工作原理
使用roslyn进行编译

使用assemblyLoadContext进行插件隔离域的动态装卸

<br/>

## 鸣谢
### 引用项目
- [Obfuscar](https://github.com/obfuscar/obfuscar)  :  代码混淆支持
- [PaddleOCR](https://github.com/PaddlePaddle/PaddleOCR) :  OCR支持
- [ApkSigner](https://android.googlesource.com/platform/build/+/dd910c5/tools/signapk/src/com/android/signapk) :  apkV2签名, android规定targetSdk为30以上的apk必须拥有V2签名
- [ZipAligner](https://github.com/TimScriptov/ZipAligner-for-Android) :  apk包对齐, android规定targetSdk为30以上的apk必须使用zipalign对齐优化
- [IconPark](https://iconpark.oceanengine.com/home) :  图标库



### 参考项目
- [tiny-sign](https://code.google.com/archive/p/tiny-sign/downloads) :  apkV1签名
- [AndroidBinaryXml](https://github.com/senswrong/AndroidBinaryXml) :  安卓二进制xml解析





