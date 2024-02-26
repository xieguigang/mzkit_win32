# bootstrap-table-editor
最近开发的一个业务平台，是一个低代码业务平台。其中用到的了bootstrap-table组件。但是bootstrap-table自身不带编辑功能。
通过搜索发现，网上大部分的解决方案都是使用x-editable, x-editable是一个通用的编辑能力组件，可以给任何元素都加上编辑能力，功能强大，可以编辑文本，数字，选项，时间等等各种类型的数据。

但是x-editable有一个比较不好的地方，x-editable的编辑模式是弹框的形式，如下图所示：
![image.png](https://upload-images.jianshu.io/upload_images/6271001-764cef80584d5f15.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

我希望的是直接在单元格进行编辑的行内编辑，所以感觉x-editable并不是很合适。  但是发现并没有其他更好的方案，于是自己动手写了一个简单的组件bootstrap-table-editor。
bootstrap-table-editor可以用于BootstrapTable行内编辑，支持文本，数字，下拉选项等。
编辑方式如下所示：
![image.png](https://upload-images.jianshu.io/upload_images/6271001-977478edd0088f6b.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/620)

![image.png](https://upload-images.jianshu.io/upload_images/6271001-660ee7da81376f5b.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/620)

![image.png](https://upload-images.jianshu.io/upload_images/6271001-c53d549f9fef4801.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/620)

要实现图中所示，首先是要引入bootstrap-table-editor： 
```
 <script src="./libs/bootstrap-table-editor.js"></script>
```
然后在表格的属性中指定editable未true：
```
 let tableOptions = {
          columns:columns,
          editable:true, //editable需要设置为 true
        }
```
然后在需要编辑的列上面指定editable属性，该属性上面可以指定编辑器的类型，目前支持文本，数字和下拉框。
```
  let columns = [{
            title: "编号",
            field: "id",
            sortable: true,
            width:200,
            editable:false,
        },{
            title: "月份",
            field: "month",
            sortable: true,
            width:200,
            formatter:function(v){
              return v + "月"
            },
            editable:{
              type:"select",
              options:{
                items:[{
                  value:1,
                  label:"1月",
                },{
                  value:2,
                  label:"2月",
                },{
                  value:3,
                  label:"3月",
                },{
                  value:4,
                  label:"4月",
                },{
                  value:5,
                  label:"5月",
                }]
              }
            }
        },{
            title: "部门",
            field: "department",
            sortable: true,
            width:200,
            editable:{
              type:"select",
              options:{
                items:[
                  "技术部","生产部","管理中心"
                ]
              }
            }
        },{
            title: "管理费用",
            field: "fee",
            sortable: true,
            width:200,
            editable:{
              type:"number"
            }
        },{
            title: "备注",
            field: "comment",
            sortable: true,
            width:200,
            editable:true,
            // editable:{
            //   type:"text"
            // }
        },];
```
其中editable为true的时候，默认是文本编辑器，指定编辑器类型未select时候，需要指定下拉框的items。


以上是主要的说明，目前该组件功能还比较间的，但是已经适合了我们业务系统的需要了。如果客户需要更加丰富的功能，可以基于组件进行扩展，该组件的开源地址如下：
https://gitee.com/netcy/bootstrap-table-editor

其中包括了组件代码和相关示例代码。

同时介绍下 使用该组件的业务平台，EasyBPM（易施业务流程管理平台），是一个低代码业务平台。 可以自定义物业表单，定义业务流程，审批流程，报表，权限等等。 通过该平台可以组建CRM，进销存，OA等业务管理平台。 参考地址：
http://www.easybpm.co/

有兴趣的可以关注。

更多优秀内容，欢迎关注公众号 “易施管理软件EasyBPM” 。












#### 介绍
bootstrap table editable；
bootstrap-table-editor, 给bootstrap-table扩展编辑功能。
目前支持文本，数字和下拉框编辑的形式。

#### 软件架构
软件架构说明


#### 安装教程

1.  xxxx
2.  xxxx
3.  xxxx

#### 使用说明

1.  xxxx
2.  xxxx
3.  xxxx

#### 参与贡献

1.  Fork 本仓库
2.  新建 Feat_xxx 分支
3.  提交代码
4.  新建 Pull Request


#### 特技

1.  使用 Readme\_XXX.md 来支持不同的语言，例如 Readme\_en.md, Readme\_zh.md
2.  Gitee 官方博客 [blog.gitee.com](https://blog.gitee.com)
3.  你可以 [https://gitee.com/explore](https://gitee.com/explore) 这个地址来了解 Gitee 上的优秀开源项目
4.  [GVP](https://gitee.com/gvp) 全称是 Gitee 最有价值开源项目，是综合评定出的优秀开源项目
5.  Gitee 官方提供的使用手册 [https://gitee.com/help](https://gitee.com/help)
6.  Gitee 封面人物是一档用来展示 Gitee 会员风采的栏目 [https://gitee.com/gitee-stars/](https://gitee.com/gitee-stars/)
