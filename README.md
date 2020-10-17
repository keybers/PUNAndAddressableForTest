# PUNAndAddressableForTest

#### 介绍
主要基于PUN和Addressable的结合demo,确保毕设项目可行性和自我高效率工作

使用Unity2019.4.4f1开发，基于PUN2网络框架，Addressable Asset作为可寻址资产框架，完成在Cloud Content Delivery中云存储资源的提取。在UPR渲染管线的条件下，可以轻松使用ShaderGraph完成自定义的材质设计，还有诸如使用Gaia地图绘制，AV Pro Vedio视频拍摄，Cinemachine摄像头逻辑控制,Odin提高代码的可读性来弥补缺陷完成开发。额外还基于ML-agents做AI训练测试。

#### 软件架构
Unity2019.4.4f1
URP 7.3.1
Addressables 1.8.5
Clound Content Delivery 1.0.0

#### 使用说明

1.  在[PUN2官网](https://www.photonengine.com/en-us/Photon)中注册账号，申请Photon Cloud Applications,获取AppID
2.  使用AddressableAsset可以本地寻址加载或者网络云端加载，项目使用的是网络资源云加载，需要使用到腾讯的[云服务Assetstreaming](https://cloud.tencent.com/solution/ucg)
3.  开通服务设置好后获取得到COS Key
4.  下载项目后使用Unity2019.4以上版本打开
5.  将AppID和COS Key分别设置到PhotonServerSettings和PlayerSettings下的Cloud Content Delivery
6.  运行场景例子Rooms

#### 注意

1.  务必获得腾讯云服务和开通PUN2服务器后使用该项目
2.  供学习使用，不作商业用途
3.  在编辑器运行会出现出乎意料的网络问题，以最终输出为项目为准测试，设计可在编辑器。
（PS：可能免费不讨好）
